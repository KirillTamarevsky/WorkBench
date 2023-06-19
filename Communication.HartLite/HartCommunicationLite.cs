using System;
using System.Collections;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using Communication.HartLite.Commands;
using log4net;

namespace Communication.HartLite
{
    public class HartCommunicationLite
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HartCommunicationLite));
        private HartCommandParser _parser { get; } = new HartCommandParser();
        private AutoResetEvent _waitForResponse { get; set; }
        private CommandResult _lastReceivedCommand { get; set; }
        //private IAddress _currentAddress { get; set; }
        private int _numberOfRetries { get; set; }

        private Queue _commandQueue { get; } = new Queue();
        private  BackgroundWorker _worker { get; }


        private const double ADDITIONAL_WAIT_TIME_BEFORE_SEND = 5.0;
        private const double ADDITIONAL_WAIT_TIME_AFTER_SEND = 50.0;
        private const double REQUIRED_TRANSMISSION_TIME_FOR_BYTE = 9.1525;

        /// <summary>
        /// Raises the event if a command is completed receive. 
        /// </summary>
        public event ReceiveHandler Receive;
        /// <summary>
        /// Raises the event before a command was sended. 
        /// </summary>
        public event SendingCommandHandler SendingCommand;
        /// <summary>
        /// Gets or sets the length of the preamble.
        /// </summary>
        /// <value>The length of the preamble.</value>
        //public int PreambleLength { get; set; }
        /// <summary>
        /// Gets or sets the max number of retries.
        /// </summary>
        /// <value>The max number of retries.</value>
        public int MaxNumberOfRetries { get; set; }
        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; }
        public bool AutomaticZeroCommand { get; set; }

        //public IAddress Address
        //{
        //    get { return _currentAddress; }
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="HartCommunicationLite"/> class.
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        public HartCommunicationLite(string comPort) : this(comPort, 2)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="HartCommunicationLite"/> class.
        /// </summary>
        /// <param name="comPort">The COM port.</param>
        /// <param name="maxNumberOfRetries">The max number of retries.</param>
        public HartCommunicationLite(string comPort, int maxNumberOfRetries)
        {
            MaxNumberOfRetries = maxNumberOfRetries;
            //PreambleLength = 10;
            Timeout = TimeSpan.FromSeconds(2);
            AutomaticZeroCommand = true;

            Port = new SerialPortWrapper(comPort, 1200, Parity.Odd, 8, StopBits.One);
            _worker = new BackgroundWorker();
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        public ISerialPortWrapper Port { get; }

        public OpenResult Open()
        {
            try
            {
                _parser.CommandComplete += CommandComplete;

                _worker.DoWork += SendCommandAsync;
                _worker.RunWorkerCompleted += SendCommandAsyncComplete;

                Port.DataReceived += DataReceived;

                Port.RtsEnable = true;
                Port.DtrEnable = false;
                
                Port.Open();

                Port.RtsEnable = false;
                Port.DtrEnable = true;
                    
                return OpenResult.Opened;
            }
            catch (ArgumentException exception)
            {
                Port.DataReceived -= DataReceived;
                Log.Warn("Cannot open port.", exception);
                return OpenResult.ComPortNotExisting;
            }
            catch (UnauthorizedAccessException exception)
            {
                Port.DataReceived -= DataReceived;
                Log.Warn("Cannot open port.", exception);
                return OpenResult.ComPortIsOpenAlreadyOpen;
            }
            catch (Exception exception)
            {
                Port.DataReceived -= DataReceived;
                Log.Warn("Cannot open port.", exception);
                return OpenResult.UnknownComPortError;
            }
        }

        public CloseResult Close()
        {
            try
            {
                _parser.CommandComplete -= CommandComplete;
                Port.DataReceived -= DataReceived;

                _worker.DoWork -= SendCommandAsync;
                _worker.RunWorkerCompleted -= SendCommandAsyncComplete;
                
                Port.Close();
                _commandQueue.Clear();

                return CloseResult.Closed;
            }
            catch (InvalidOperationException exception)
            {
                Log.Warn("Cannot close port.", exception);
                return CloseResult.PortIsNotOpen;
            }
        }

        public CommandResult Send(byte command)
        {
            return Send(command, new byte[0]);
        }

        public CommandResult Send(byte command, byte[] data)
        {
            var hartCommand = new HARTCommand(command, data);

            return Send(20, new ShortAddress(0), hartCommand);
        }

        public CommandResult Send (int preambleLength, IAddress address, HARTCommand hartCommand)
        {
            _numberOfRetries = MaxNumberOfRetries;

            var hartDatagram = new HARTDatagram(preambleLength, address, hartCommand.Number, new byte[0], hartCommand.Data);
            
            _commandQueue.Enqueue(hartDatagram);
            
            return ExecuteCommand();
        }

        public CommandResult SendZeroCommand()
        {
            _numberOfRetries = MaxNumberOfRetries;
            _commandQueue.Enqueue(new HARTDatagram(20, new ShortAddress(0), 0, new byte[0], new byte[0]));
            return ExecuteCommand();
        }

        public void SendAsync(byte command)
        {
            SendAsync(command, new byte[0]);
        }

        public void SendAsync(byte command, byte[] data)
        {
            ExecuteCommandAsync(new HARTDatagram(20, new ShortAddress(0), command, new byte[0], data), MaxNumberOfRetries);
        }

        //public void SwitchAddressTo(IAddress address)
        //{
        //    _currentAddress = address;
        //}

        private void ExecuteCommandAsync(HARTDatagram hartDatagram, int maxNumberOfRetries)
        {
            _commandQueue.Enqueue(hartDatagram);

            if(!_worker.IsBusy)
            {
                _numberOfRetries = maxNumberOfRetries;
                _worker.RunWorkerAsync();
            }
        }

        private void SendCommandAsync(object sender, DoWorkEventArgs e)
        {
            if(_commandQueue.Count > 0)
                SendCommandSynchronous((HARTDatagram)_commandQueue.Dequeue());
        }

        private void SendCommandAsyncComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_commandQueue.Count > 0 && !_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private CommandResult ExecuteCommand()
        {
            lock (_commandQueue)
            {
                return SendCommandSynchronous((HARTDatagram) _commandQueue.Dequeue());
            }
        }

        private bool ShouldRetry()
        {
            return _numberOfRetries-- > 0;
        }

        private bool HasCommunicationError()
        {
            if (_lastReceivedCommand.ResponseCode.FirstByte < 128)
                return false;

            Log.Warn("Communication error. First bit of response code byte is set.");

            if ((_lastReceivedCommand.ResponseCode.FirstByte & 0x40) == 0x40)
                Log.WarnFormat("Vertical Parity Error - The parity of one or more of the bytes received by the device was not odd.");
            if ((_lastReceivedCommand.ResponseCode.FirstByte & 0x20) == 0x20)
                Log.WarnFormat("Overrun Error - At least one byte of data in the receive buffer of the UART was overwritten before it was read (i.e., the slave did not process incoming byte fast enough).");
            if ((_lastReceivedCommand.ResponseCode.FirstByte & 0x10) == 0x10)
                Log.WarnFormat("Framing Error - The Stop Bit of one or more bytes received by the device was not detected by the UART (i.e. a mark or 1 was not detected when a Stop Bit should have occoured)");
            if ((_lastReceivedCommand.ResponseCode.FirstByte & 0x08) == 0x08)
                Log.WarnFormat("Longitudinal Partity Error - The Longitudinal Partity calculated by the device did not match the Check Byte at the end of the message.");
            if ((_lastReceivedCommand.ResponseCode.FirstByte & 0x02) == 0x02)
                Log.WarnFormat("Buffer Overflow - The message was too long for the receive buffer of the device.");

            return true;
        }

        private CommandResult SendCommandSynchronous(HARTDatagram requestCommand)
        {
            Receive += CommandReceived;
            try
            {
                SendCommand(requestCommand);
                if (!_waitForResponse.WaitOne(Timeout))
                {
                    Receive -= CommandReceived;

                    if (ShouldRetry())
                        return SendCommandSynchronous(requestCommand);

                    return null;
                }

                Receive -= CommandReceived;

                if (HasCommunicationError())
                    return ShouldRetry() ? SendCommandSynchronous(requestCommand) : _lastReceivedCommand;

                return _lastReceivedCommand;
            }
            catch (Exception exception)
            {
                Log.Error("Unexpected exception!", exception);
                Receive -= CommandReceived;

                if (ShouldRetry())
                    return SendCommandSynchronous(requestCommand);

                return null;
            }
        }

        private void SendCommand(HARTDatagram command)
        {
            _waitForResponse = new AutoResetEvent(false);
            _parser.Reset();

            byte[] bytesToSend = command.ToByteArray();

            if(SendingCommand != null)
                SendingCommand.BeginInvoke(this, new CommandRequest(command), null, null);

            Thread.Sleep(100);

            Port.DtrEnable = false;
            Port.RtsEnable = true;

            Thread.Sleep(Convert.ToInt32(ADDITIONAL_WAIT_TIME_BEFORE_SEND));

            DateTime startTime = DateTime.Now;

            Log.Debug(string.Format("Data sent to {1}: {0}", BitConverter.ToString(bytesToSend), Port.PortName));
            Port.Write(bytesToSend, 0, bytesToSend.Length);

            SleepAfterSend(bytesToSend.Length, startTime);
            Port.RtsEnable = false;
            Port.DtrEnable = true;
        }

        private void CommandReceived(object sender, CommandResult args)
        {
            _lastReceivedCommand = args;
            _waitForResponse.Set();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] receivedBytes = new byte[Port.BytesToRead];
            Port.Read(receivedBytes, 0, receivedBytes.Length);
            Log.Debug(string.Format("Received data from {1}: {0}", BitConverter.ToString(receivedBytes), Port.PortName));

            _parser.ParseNextBytes(receivedBytes);
        }

        private static void SleepAfterSend(int dataLength, DateTime startTime)
        {
            TimeSpan waitTime = CalculateWaitTime(dataLength, startTime);

            if (waitTime.Milliseconds > 0)
                Thread.Sleep(waitTime);
        }

        private static TimeSpan CalculateWaitTime(int dataLength, DateTime startTime)
        {
            TimeSpan requiredTransmissionTime = TimeSpan.FromMilliseconds(Convert.ToInt32(REQUIRED_TRANSMISSION_TIME_FOR_BYTE * dataLength + ADDITIONAL_WAIT_TIME_AFTER_SEND));
            return startTime + requiredTransmissionTime - DateTime.Now;
        }

        private void CommandComplete(HARTDatagram command)
        {
            if(command.CommandNumber == 0)
            {
                //PreambleLength = command.PreambleLength;

                //_currentAddress = new LongAddress(command.Data[1], command.Data[2], new [] { command.Data[9], command.Data[10], command.Data[11] });
            }

            Receive?.Invoke(this, new CommandResult(command));
        }
    }
}
