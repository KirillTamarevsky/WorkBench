using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Timers;
using Communication.HartLite.Commands;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Util;
using m4dHART._2_DataLinkLayer.Wired_Token_Passing;

namespace Communication.HartLite
{
    public class HartCommunicationLite
    {
        private enum MasterState
        {
            WATCHING,
            ENABLED,
            USING
        }
        private enum TRANSMITResult
        {
            success,
            fail
        }
        //private static readonly ILog Log = LogManager.GetLogger(typeof(HartCommunicationLite));
        private static readonly ILog Log = LogManager.GetLogger("HART");
        private bool MSG_PENDING { get; set; } = false;
        private bool BURST { get; set; } = false;
        private MasterState masterState { get; set; }
        public MasterAddress MasterAddress { get; set; } = MasterAddress.Primary;
        private HartCommandParser _parser { get; } = new HartCommandParser();
        private ManualResetEvent RCV_MSG_mre { get; set; } = new ManualResetEvent(false);
        private ManualResetEvent TRANSMITconfirm { get; set; } = new ManualResetEvent(false);
        private HARTDatagram _lastReceivedFrame { get; set; }
        private bool frameReceived { get; set; }
        private TRANSMITResult TransmitResult { 
            get; 
            set; }

        private bool closing ;
        private int _retriesCount;
        private int retriesCOUNT
        {
            get => _retriesCount;
            set
            {
                _retriesCount = value;
                Log.Debug($"RETRIES COUNT = {value}");
                if (_retriesCount >= MaxNumberOfRetries)
                {
                    Log.Debug($"RETRIES COUNT >= MaxNumberOfRetries ({MaxNumberOfRetries})");
                    MSG_PENDING = false;
                    Log.Info("MSG_PENDING = false");
                    TransmitResult = TRANSMITResult.fail;
                    TRANSMITconfirm.Set();
                    Log.Info("TRANSMIT.confirm(fail)");
                }
            }
        }
        private HARTDatagram TRANSMITrequest { 
            get; 
            set; } = null;

        private const double ONE_BYTE_TRANSMISSION_TIME = 9.1525;
        private const int STO = 28; // Slave Time-Out
        private const int HOLD = 2; // 
        private const int RT2 = 8; // Link Grant Time
        private const int RT1_pri = 33; // Link Quiet Time Primary
        private const int RT1_sec = 41; // Link Quiet Time Secondary
        private int RT1
        {
            get
            {
                switch (MasterAddress) 
                { 
                    case MasterAddress.Primary:
                        return RT1_pri;
                    case MasterAddress.Secondary:
                        return RT1_sec;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public event Action<CommandResult> BACKReceived;

        /// <summary>
        /// Gets or sets the max number of retries.
        /// </summary>
        /// <value>The max number of retries.</value>
        public int MaxNumberOfRetries { get; set; }
        
        private System.Timers.Timer _timer { get; set; } 
        

        /// <summary>
        /// Initializes a new instance of the <see cref="HartCommunicationLite"/> class.
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        public HartCommunicationLite(string comPort) : this(comPort, 4)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="HartCommunicationLite"/> class.
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        /// <param name="maxNumberOfRetries">The max number of retries.</param>
        public HartCommunicationLite(string comPort, int maxNumberOfRetries)
        {
            MaxNumberOfRetries = maxNumberOfRetries;

            Port = new SerialPortWrapper(comPort, 1200, Parity.Odd, 8, StopBits.One);
            //Port = new SerialPortWrapperSNT(comPort, 1200, SnT.IO.Ports.Parity.Odd, 8, SnT.IO.Ports.StopBits.One);
        }

        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Log.Debug("onTimerElapsed()");
            if (Port.CDHolding || _parser.ReceivingPDU)
            {
                Log.Debug($"onTimerElapsed() Port.CDHolding = {Port.CDHolding} || _parser.ReceivingPDU = {_parser.ReceivingPDU}");

                return;
            }
            if (masterState == MasterState.WATCHING)
            {
                BURST = false;
                masterState = MasterState.ENABLED;
                Log.Info("ENABLED");
                _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * HOLD;
                _timer.Start();
                Log.Info("timer.start() watching to enabled - HOLD");
                if (TRANSMITrequest != null && !MSG_PENDING)
                {
                    _timer.Stop();
                }
                if (MSG_PENDING && retriesCOUNT < MaxNumberOfRetries)
                {
                    _timer.Stop();
                    Log.Debug($"timer.stop() before sending command");
                    SendCommand(TRANSMITrequest);
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1_pri;
                    _timer.Start();
                Log.Info("timer.start() = RT1 after command sended");
                    masterState = MasterState.USING;
                Log.Info("USING");
                }
            }
            if (masterState == MasterState.ENABLED)
            {
                masterState = MasterState.WATCHING;
                Log.Info("WATCHING");
                _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * 2 * RT1;
                _timer.Start();
                Log.Info("timer.start() enabled to watching 2*RT1");
                return;
            }
            if (masterState == MasterState.USING && !_timer.Enabled)
            {
                masterState = MasterState.WATCHING;
                Log.Info("WATCHING");

                switch (BURST)
                {
                    case true:
                        retriesCOUNT++;
                        _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1;
                        _timer.Start();
                Log.Info("timer.start() using to watching burst == true");
                        break;
                    case false:
                        retriesCOUNT++;
                        _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * ((RT1 - RT1_pri) == 0? 0.01: RT1 - RT1_pri);
                        _timer.Start();
                Log.Info("timer.start() using to watching burst = false");
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public ISerialPortWrapper Port { get; }

        public OpenResult Open()
        {
            closing = false;
            try
            {
                //_parser.CorrectPDUReceived += OnCorrectPDUReceived;
                //_parser.ErrorPDUReceived += OnErrorPDUReceived;

                Port.DataReceived += OnDataReceived;

                Port.RtsEnable = true;
                Port.DtrEnable = false;
                
                masterState = MasterState.WATCHING;
                _timer = new System.Timers.Timer() { AutoReset = false, Enabled = false,  };
                _timer.Elapsed += onTimerElapsed;

                Port.Open();

                Port.RtsEnable = false;
                Port.DtrEnable = true;

                _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1 ;
                _timer.Start();
                Log.Info("timer.start() initial timer on OPEN()");

                return OpenResult.Opened;
            }
            catch (ArgumentException exception)
            {
                _timer.Enabled = false;
                _timer.Elapsed -= onTimerElapsed;
                Port.DataReceived -= OnDataReceived;
                //_parser.CorrectPDUReceived -= OnCorrectPDUReceived;
                //_parser.ErrorPDUReceived -= OnErrorPDUReceived;
                Log.Warn("Cannot open port.", exception);
                return OpenResult.ComPortNotExisting;
            }
            catch (UnauthorizedAccessException exception)
            {
                Port.DataReceived -= OnDataReceived;
                Log.Warn("Cannot open port.", exception);
                return OpenResult.ComPortIsOpenAlreadyOpen;
            }
            catch (Exception exception)
            {
                Port.DataReceived -= OnDataReceived;
                Log.Warn("Cannot open port.", exception);
                return OpenResult.UnknownComPortError;
            }
        }


        public CloseResult Close()
        {
            closing = true;
            try
            {
                TRANSMITconfirm.WaitOne();
                _timer.Stop();
                
                Port.DataReceived -= OnDataReceived;

                Port.Close();
                return CloseResult.Closed;
            }
            catch (InvalidOperationException exception)
            {
                Log.Warn("Cannot close port.", exception);
                return CloseResult.PortIsNotOpen;
            }
        }

        public CommandResult Send (int preambleLength, IAddress address, HARTCommand hartCommand)
        {
            
            var hartDatagram = new HARTDatagram(preambleLength, 2, address.ToSTXAddress(MasterAddress), hartCommand.Number, new byte[0], hartCommand.Data);
            
            if (closing)
            {
                return null;
            }
            _lastReceivedFrame = null;
            if (Port.IsOpen)
            {

                try
                {
                    _parser.Reset();
                    
                    TRANSMITconfirm.Reset();
                    TRANSMITrequest = hartDatagram;
                    MSG_PENDING = true;
                    Log.Info($"TRANSMIT.request");
                    retriesCOUNT = 0;
                    
                    TRANSMITconfirm.WaitOne();
                    Log.Info("TRANSMIT.confirmed");
                    if (_lastReceivedFrame != null)
                    {
                        return hartCommand.ToCommandResult( _lastReceivedFrame );
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Unexpected exception!", exception);

                    return null;
                }
            }
            return null;        }

        public CommandResult SendZeroCommand()
        {
            return Send(20, new ShortAddress(0), new HART_Zero_Command());
        }


        private void SendCommand(HARTDatagram command)
        {
            _parser.Reset();

            byte[] bytesToSend = command.ToByteArray();

            Port.DtrEnable = false;
            Log.Info($"Before RTS enable: CD = [{Port.CDHolding}]");
            Port.RtsEnable = true;
            Log.Info($"RTS [{Port.RtsEnable}]");

            //Thread.Sleep(Convert.ToInt32(ADDITIONAL_WAIT_TIME_BEFORE_SEND));

            DateTime startTime = DateTime.Now;

            Log.Debug($"Data sent to {Port.PortName}: {BitConverter.ToString(bytesToSend)}");
            Port.Write(bytesToSend, 0, bytesToSend.Length);

            SleepAfterSend(bytesToSend.Length, startTime);
            Port.RtsEnable = false;
            Port.DtrEnable = true;
            Log.Info($"RTS [{Port.RtsEnable}]");

        }

        private void OnCorrectPDUReceived(HARTDatagram args)
        {
            if (args.FrameType == FrameType.STX && args.Address._masterAddress == this.MasterAddress)
            {
                switch (this.MasterAddress)
                {
                    case MasterAddress.Secondary:
                        this.MasterAddress = MasterAddress.Primary;
                        break;
                    case MasterAddress.Primary:
                        this.MasterAddress = MasterAddress.Secondary;
                        break;
                }
            }
            if (args.FrameType == FrameType.BACK | (args.FrameType == FrameType.ACK & args.Address._fieldDeviceInBurstMode))
            {
                BURST = true;
            }
            if (masterState == MasterState.USING)
            {
                if (args.FrameType == FrameType.ACK && !BURST)
                {
                    _lastReceivedFrame = args;
                    masterState = MasterState.WATCHING;
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT2;
                    _timer.Start();
                    Log.Info("timer.start() RT2 FrameType.ACK && !BURST");

                    MSG_PENDING = false;
                    Log.Info("MSG_PENDING = false");
                    TransmitResult = TRANSMITResult.success;
                    TRANSMITconfirm.Set();
                }
                if (args.FrameType == FrameType.ACK && BURST)
                {
                    _lastReceivedFrame = args;
                    masterState = MasterState.WATCHING;
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1;
                    _timer.Start();
                    Log.Info("timer.start() RT1 FrameType.ACK && BURST");
                    if (Port.CDHolding)
                    {
                        _timer.Stop();
                        Log.Info("timer.stop() ON CD Holding");
                    }
                    MSG_PENDING = false;
                    Log.Info("MSG_PENDING = false");
                    TransmitResult = TRANSMITResult.success;
                    TRANSMITconfirm.Set();
                }
                if (args.FrameType != FrameType.ACK)
                {
                    retriesCOUNT++;

                    masterState = MasterState.WATCHING;
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1;
                    _timer.Start();
                    Log.Info("timer.start() FrameType != FrameType.ACK");


                }
                return;
            }
            if (masterState == MasterState.WATCHING)
            {
                if (
                    (args.FrameType == FrameType.STX && !BURST)
                    || 
                    (args.FrameType == FrameType.ACK & args.Address._masterAddress != MasterAddress & BURST)
                    )
                {
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1;
                    _timer.Start();
                    Log.Info("timer.start() RT1 (RCV_MSG == STX) && !BURST || (RCV_MSG == OACK) && BURST");

                }
                if (args.FrameType == FrameType.BACK && args.Address._masterAddress == MasterAddress)
                {
                    BACKReceived?.Invoke(new CommandResult(args));
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1;
                    _timer.Start();
                    Log.Info("timer.start() RT1 (RCV_MSG == BACK)");

                }
                if (args.FrameType == FrameType.STX && BURST)
                {
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * RT1 * 2 ;
                    _timer.Start();
                    Log.Info("timer.start() 2 * RT1 (RCV_MSG == STX && BURST)");

                }
                //RCV_MSG == OBACK
                if (
                    (args.FrameType == FrameType.BACK && args.Address._masterAddress != MasterAddress)
                    )
                {
                    Timer_Start(ONE_BYTE_TRANSMISSION_TIME * HOLD / 2, "HOLD(RCV_MSG == OBACK)");
                    BACKReceived?.Invoke(new CommandResult(args));
                }
                //RCV_MSG == OACK && !BURST
                if (
                    (args.FrameType == FrameType.ACK & args.Address._masterAddress != MasterAddress & !BURST)
                    )
                {
                    _timer.Interval = ONE_BYTE_TRANSMISSION_TIME * HOLD;
                    _timer.Start();
                    Log.Info("timer.start() HOLD (RCV_MSG == OACK)");
                }
                return;
            }

        }
        private void Timer_Start(double milliseconds, string reason)
        {
            _timer.Interval = milliseconds; 
            _timer.Start();
            Log.Debug($"timer.start({milliseconds}) on {reason}");
        }
        private void Timer_Stop(string reason)
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                Log.Debug($"timer.stop() on {reason}");
            }
        }

        private void OnDataReceived(FSKReceivedByte fskReceivedByte)
        {
            if (masterState == MasterState.WATCHING )
            {
                Timer_Stop($"data receive detected while in {masterState}");
            }
            if (masterState == MasterState.ENABLED )
            {
                Timer_Stop($"data receive detected while in {masterState}, switch to {MasterState.WATCHING}");
                masterState = MasterState.WATCHING;
            }
            if (masterState == MasterState.USING )
            {
                Timer_Stop($"data receive detected while in {masterState}");
            }

            var parserState = _parser.ParseByte(fskReceivedByte.ReceivedByte);

            Log.Debug($"Received data from {Port.PortName}: {BitConverter.ToString(new byte[] { fskReceivedByte.ReceivedByte })} - {parserState}");

            bool rcv_success = _parser.CurrentByteType == HartCommandParser.ReceiveState.CorrectPDUReceived;
            if (rcv_success)
            {
                var correctPDU = _parser.LastReceivedDatagram;
                Log.Debug($"Received Correct PDU {Port.PortName} [{correctPDU.Address._masterAddress} master] [{correctPDU.FrameType}] command number {correctPDU.CommandNumber} <=========== Hart comm lite {masterState}");
                OnCorrectPDUReceived(correctPDU);
            }
            Timer_Start(ONE_BYTE_TRANSMISSION_TIME * HOLD, "HOLD after data received");

        }

        private static void SleepAfterSend(int dataLength, DateTime startTime)
        {
            TimeSpan waitTime = CalculateWaitTime(dataLength, startTime);

            if (waitTime.Milliseconds > 0)
                Thread.Sleep(waitTime);
        }

        private static TimeSpan CalculateWaitTime(int dataLength, DateTime startTime)
        {
            TimeSpan requiredTransmissionTime = TimeSpan.FromMilliseconds(Convert.ToInt32(ONE_BYTE_TRANSMISSION_TIME * dataLength ));
            return startTime + requiredTransmissionTime - DateTime.Now;
        }

    }
}
