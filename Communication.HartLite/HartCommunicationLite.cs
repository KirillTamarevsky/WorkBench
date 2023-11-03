using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
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

        private MasterState _currentState;
        private MasterState CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                Log.Debug($"Switching State to {value}");
            }
        }
        public MasterAddress MasterAddress { get; set; } = MasterAddress.Primary;
        private HartCommandParser _parser { get; } = new HartCommandParser();
        private ManualResetEvent RCV_MSG_mre { get; set; } = new ManualResetEvent(false);
        private ManualResetEvent TRANSMITconfirm { get; set; } = new ManualResetEvent(false);
        private HARTDatagram _lastReceivedFrame { get; set; }
        //private bool frameReceived { get; set; }
        private TRANSMITResult TransmitResult
        {
            get;
            set;
        }

        private bool closing;
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
        private HARTDatagram TRANSMITrequest
        {
            get;
            set;
        } = null;

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

        //private System.Timers.Timer _timer { get; set; } 


        /// <summary>
        /// Initializes a new instance of the <see cref="HartCommunicationLite"/> class.
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        public HartCommunicationLite(string comPort) : this(comPort, 4)
        { }

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
                Port.DataReceived += OnDataReceived;

                Port.RtsEnable = true;
                Port.DtrEnable = false;

                Port.Open();

                Port.RtsEnable = false;
                Port.DtrEnable = true;

                return OpenResult.Opened;
            }
            catch (ArgumentException exception)
            {
                Port.DataReceived -= OnDataReceived;
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
            finally
            {
                mainloopCTS = new CancellationTokenSource();

                mainloop = new Thread(() => MasterStateMachineMainLoop(mainloopCTS.Token));
                mainloop.Priority = ThreadPriority.AboveNormal;
                mainloop.Name = "HART_MasterStateMachine" + mainloop.ManagedThreadId;
                mainloop.Start();
            }
        }
        CancellationTokenSource mainloopCTS;
        Thread mainloop;

        public CloseResult Close()
        {
            closing = true;
            try
            {
                mainloopCTS.Cancel();
                mainloop.Join();

                TRANSMITconfirm.WaitOne();

                Port.DataReceived -= OnDataReceived;

                Port.Close();

                mainloopCTS.Dispose();

                return CloseResult.Closed;
            }
            catch (InvalidOperationException exception)
            {
                Log.Warn("Cannot close port.", exception);
                return CloseResult.PortIsNotOpen;
            }
        }

        public CommandResult Send(int preambleLength, IAddress address, HARTCommand hartCommand)
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
                        return hartCommand.ToCommandResult(_lastReceivedFrame);
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Unexpected exception!", exception);

                    return null;
                }
            }
            return null;
        }
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

            DateTime startTime = DateTime.Now;

            Log.Debug($"Data sent to {Port.PortName}: {BitConverter.ToString(bytesToSend)}");
            Port.Write(bytesToSend, 0, bytesToSend.Length);

            SleepAfterSend(bytesToSend.Length, startTime);
            Port.RtsEnable = false;
            Port.DtrEnable = true;
            Log.Info($"RTS [{Port.RtsEnable}]");

        }
        private void OnDataReceived(FSKReceivedByte fskReceivedByte)
        {

            var parserState = _parser.ParseByte(fskReceivedByte.ReceivedByte);

            Log.Debug($"Received data from {Port.PortName}: {BitConverter.ToString(new byte[] { fskReceivedByte.ReceivedByte })} - {parserState}");

            bool rcv_success = _parser.CurrentByteType == HartCommandParser.ReceiveState.CorrectPDUReceived;
            if (rcv_success)
            {
                var correctPDU = _parser.LastReceivedDatagram;
                Log.Debug($"Received Correct PDU {Port.PortName} [{correctPDU.Address._masterAddress} master] [{correctPDU.FrameType}] command number {correctPDU.CommandNumber} <=========== Hart comm lite {CurrentState}");
            }


        }

        private static void SleepAfterSend(int dataLength, DateTime startTime)
        {
            TimeSpan waitTime = CalculateWaitTime(dataLength, startTime);

            if (waitTime.Milliseconds > 0)
                Thread.Sleep(waitTime);
        }

        private static TimeSpan CalculateWaitTime(int dataLength, DateTime startTime)
        {
            TimeSpan requiredTransmissionTime = TimeSpan.FromMilliseconds(Convert.ToInt32(ONE_BYTE_TRANSMISSION_TIME * dataLength));
            return startTime + requiredTransmissionTime - DateTime.Now;
        }
        private enum RCV_MSG_state
        {
            None,
            Receiving,
            Success,
            Err
        }
        private void MasterStateMachineMainLoop(CancellationToken cts)
        {
            CurrentState = MasterState.WATCHING;
            RCV_MSG_state RCV_MSG = RCV_MSG_state.None;
            DateTime LastLoopTime = DateTime.Now;
            TimeSpan Timer = TimeSpan.Zero;
            DateTime TimerStartTime = DateTime.Now;

            Action<string, double> startTimer = (string reason, double interval) =>
            {
                Log.Debug($"start timer on {reason} = {interval}");
                Timer = TimeSpan.FromMilliseconds(interval);
                TimerStartTime = DateTime.Now;
            };

            Action<string> stopTimer = (string reason) =>
            {
                Log.Debug($"stop timer on {reason}"); Timer = TimeSpan.Zero;
            };

            Action<TRANSMITResult> transmitConfirm = (TRANSMITResult confirm) =>
            {
                Log.Debug($"Transmit.confirm({confirm})");
                TransmitResult = confirm;
                TRANSMITconfirm.Set();
                RCV_MSG = RCV_MSG_state.None;
            };
            Action<HARTDatagram> cyclicIndicate = (HARTDatagram BACKdatagram) =>
            {
                Log.Debug($"Cyclic.Indicate(BACK)");
                BACKReceived?.Invoke(new CommandResult(BACKdatagram));
                RCV_MSG = RCV_MSG_state.None;
            };

            startTimer("Initial RT1", ONE_BYTE_TRANSMISSION_TIME * RT1);
            
            var Enable_Indicate_time_lag = TimeSpan.FromMilliseconds(10);
            var Enable_Indicate = false;
            var Enable_Indicate_switch_time = DateTime.Now - Enable_Indicate_time_lag; 

            while (!cts.IsCancellationRequested)
            {
                Thread.Sleep(
                    5);
                if (Enable_Indicate != Port.CDHolding)
                {
                    Enable_Indicate = Port.CDHolding;
                    Enable_Indicate_switch_time = DateTime.Now;
                }
                var CurrentTime = DateTime.Now;
                if (Timer > TimeSpan.Zero)
                {
                    var elapsedTime = CurrentTime - TimerStartTime;
                    if (elapsedTime >= Timer)
                    {
                        //Timer Elapsed
                        Timer = TimeSpan.Zero;

                        switch (CurrentState)
                        {
                            case MasterState.WATCHING:
                                BURST = false;
                                startTimer($"{CurrentState} to {MasterState.ENABLED} on timeout = 0, HOLD", ONE_BYTE_TRANSMISSION_TIME * HOLD);
                                CurrentState = MasterState.ENABLED;
                                continue;
                            case MasterState.ENABLED:
                                startTimer($"{CurrentState} to {MasterState.WATCHING} on timeout = 0, 2 * RT1", ONE_BYTE_TRANSMISSION_TIME * RT1 * 2);
                                CurrentState = MasterState.WATCHING;
                                continue;
                            case MasterState.USING:
                                if (BURST)
                                {
                                    retriesCOUNT++;
                                    startTimer($"[{CurrentState}] (timeout = 0) && BURST, RT1", ONE_BYTE_TRANSMISSION_TIME * RT1);
                                }
                                else
                                {
                                    retriesCOUNT++;
                                    startTimer($"[{CurrentState}] (timeout = 0) && !BURST, RT1 - RT1primary", ONE_BYTE_TRANSMISSION_TIME * (RT1 - RT1_pri + 0.1));
                                }
                                CurrentState = MasterState.WATCHING;
                                continue;
                            default:
                                throw new Exception($"no such state ({CurrentState}) for timeout");
                        }

                    }
                }
                if (CurrentState == MasterState.ENABLED && TRANSMITrequest != null && !MSG_PENDING)
                {
                    retriesCOUNT++;
                    MSG_PENDING = true;
                    continue;
                }
                if (CurrentState == MasterState.ENABLED && MSG_PENDING && (retriesCOUNT < MaxNumberOfRetries))
                {
                    SendCommand(TRANSMITrequest);
                    RCV_MSG = RCV_MSG_state.None;
                    _lastReceivedFrame = null;
                    startTimer($"[{MasterState.ENABLED}] switch to [{MasterState.USING}] after XMT_MSG , RT1 primary", ONE_BYTE_TRANSMISSION_TIME * RT1_pri);
                    CurrentState = MasterState.USING;
                    continue;
                }
                switch (RCV_MSG)
                {
                    case RCV_MSG_state.None:
                        break;
                    case RCV_MSG_state.Receiving:
                        if (_parser.CurrentByteType == HartCommandParser.ReceiveState.CorrectPDUReceived)
                        {
                            RCV_MSG = RCV_MSG_state.Success;
                            continue;
                        }
                        if ((Enable_Indicate_switch_time + Enable_Indicate_time_lag <= DateTime.Now) && !Enable_Indicate)
                        {
                            Log.Debug("~ENABLE.Indicate => RCV_MSG = Err");
                            _parser.Reset();
                            RCV_MSG = RCV_MSG_state.Err;
                            continue;
                        }
                        continue;
                    case RCV_MSG_state.Success:
                        BURST = _parser.LastReceivedDatagram.FrameType == FrameType.BACK | _parser.LastReceivedDatagram.Address._fieldDeviceInBurstMode;
                        RCV_MSG = RCV_MSG_state.None;
                        var lrdg = _parser.LastReceivedDatagram;
                        _lastReceivedFrame = lrdg;
                        switch (CurrentState)
                        {
                            case MasterState.WATCHING:
                                if (lrdg.FrameType == FrameType.BACK & lrdg.Address._masterAddress == MasterAddress)
                                {
                                    cyclicIndicate(lrdg);
                                    startTimer("cyclic.indicate() RT1", ONE_BYTE_TRANSMISSION_TIME * RT1);
                                }
                                if (lrdg.FrameType == FrameType.STX && !BURST
                                    | (lrdg.FrameType == FrameType.ACK & lrdg.Address._masterAddress != MasterAddress && BURST))
                                {
                                    startTimer("((RCV_MSG == STX) && !BURST) || ((RCV_MSG == OACK) && BURST) RT1", ONE_BYTE_TRANSMISSION_TIME * RT1);
                                }
                                if (lrdg.FrameType == FrameType.STX && BURST)
                                {
                                    startTimer("((RCV_MSG == STX) && BURST) 2 * RT1", ONE_BYTE_TRANSMISSION_TIME * RT1 * 2);

                                }
                                if (lrdg.FrameType == FrameType.BACK & lrdg.Address._masterAddress != MasterAddress)
                                {
                                    startTimer("RCV_MSG == OBACK, HOLD", ONE_BYTE_TRANSMISSION_TIME * HOLD);
                                    cyclicIndicate(lrdg);
                                    CurrentState = MasterState.ENABLED;
                                }
                                continue;
                            case MasterState.ENABLED:
                                throw new Exception($"receive success isn't possible in {CurrentState} state");
                            case MasterState.USING:
                                if (lrdg.FrameType == FrameType.ACK && !BURST)
                                {
                                    startTimer($"(RCV_MSG == ACK) && ~BURST, RT2", ONE_BYTE_TRANSMISSION_TIME * RT2);
                                    MSG_PENDING = false;
                                    transmitConfirm(TRANSMITResult.success);
                                    CurrentState = MasterState.WATCHING;
                                    continue;
                                }
                                if (lrdg.FrameType == FrameType.ACK && BURST)
                                {
                                    startTimer($"(RCV_MSG == ACK) && BURST, RT1", ONE_BYTE_TRANSMISSION_TIME * RT1);
                                    MSG_PENDING = false;
                                    transmitConfirm(TRANSMITResult.success);
                                    CurrentState = MasterState.WATCHING;
                                    continue;
                                }
                                if (lrdg.FrameType != FrameType.ACK)
                                {
                                    retriesCOUNT++;
                                    startTimer($"(RCV_MSG != ACK), RT1", ONE_BYTE_TRANSMISSION_TIME * RT1);
                                    CurrentState = MasterState.WATCHING;
                                    continue;
                                }
                                break;
                            default:
                                break;
                        }
                        continue;
                    case RCV_MSG_state.Err:
                        if (CurrentState == MasterState.USING)
                        {
                            CurrentState = MasterState.WATCHING;
                            retriesCOUNT++;
                            startTimer("USING RCV_MSG != ACK (RCV_MSG == Err) RT1", ONE_BYTE_TRANSMISSION_TIME * RT1);
                        }
                        break;
                    default:
                        break;
                }
                if (Enable_Indicate)
                {
                    RCV_MSG = RCV_MSG_state.Receiving;

                    switch (CurrentState)
                    {
                        case MasterState.WATCHING:
                            continue;
                        case MasterState.ENABLED:
                            CurrentState = MasterState.WATCHING;
                            RCV_MSG = RCV_MSG_state.Receiving;
                            continue;
                        case MasterState.USING:
                            stopTimer($"{CurrentState} => Enable.Indicate ");
                            continue;
                        default:
                            continue;
                    }
                }
            }

            transmitConfirm(TRANSMITResult.fail);
        }
    }
}