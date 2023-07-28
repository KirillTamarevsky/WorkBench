using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Primitives;

namespace WorkBench.Communicators
{
    public class SerialPortTextCommunicator : ITextCommunicator, IDisposable
    {
        readonly ILog logger = LogManager.GetLogger("Communication");

        readonly IWBSerialPortWrapper _serialPort;

        readonly List<byte> _receivedBytes = new();

        private string NewLine { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; }

        public int CommunicationRetries { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="baudrate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="lineEndToken">символы конца строки</param>
        public SerialPortTextCommunicator(
            IWBSerialPortWrapper serialport,
            string lineEndToken,
            int timeout = 3,
            int communicationRetries = 20)
        {
            _serialPort = serialport;

            NewLine = lineEndToken;

            Timeout = TimeSpan.FromSeconds(timeout); // 3 seconds

            CommunicationRetries = communicationRetries;
        }

        public bool Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                //_serialPort.DataReceived -= OnDataReceivedFromSerialPort;

                logger.Info($"Closing SerialEKCommunicator on {_serialPort.PortName}");

                _serialPort.Close();
            }
            return false;
        }

        public bool Open()
        {
            bool serialPortOpened = false;

            if (_serialPort != null)
            {
                try
                {
                    serialPortOpened = TryOpenSerialPort(_serialPort);
                }
                catch (Exception e)
                {
                    logger.Debug($"System.IO.IOException while try to open {_serialPort.PortName} | {e}");
                }
            }
            return serialPortOpened;
        }

        public TextCommunicatorReadLineStatus ReadLine(TimeSpan readLineTimeout, out string result)
        {
            result = string.Empty;

            logger.Debug($"{_serialPort.PortName} Start ReadLine(), Readtimeout = {readLineTimeout}");

            if (!_serialPort.IsOpen)
            {
                var errorMessage = $"SerialPort {_serialPort.PortName} not opened while try to ReadLine";
                logger.Error(errorMessage);
                return TextCommunicatorReadLineStatus.CommunicationChannelClosed;
            }

            _serialPort.ReadTimeout = (int)readLineTimeout.TotalMilliseconds;

            try
            {
                do
                {
                    byte nextByte = (byte)_serialPort.ReadByte();
                    logger.Debug($"InfiniteReceiveByteFromSerialPort readed from {_serialPort.PortName} | byte = {nextByte} | char = {Encoding.ASCII.GetString(new byte[] { nextByte }).Replace("\r", "\\r").Replace("\n", "\\n")}");
                    _receivedBytes.Add(nextByte);

                } while (!ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray()).EndsWith(NewLine));
            }
            catch (TimeoutException ex)
            {
                logger.Warn(string.Format("{0} ReadLine() TimeOut, data  in receive buffer = \"{1}\"|{2} \\ bytes to read in serial port = {3}",
                    _serialPort.PortName,
                    ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray()).Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(_receivedBytes.ToArray()),
                    _serialPort.BytesToRead
                    ));
                logger.Warn($"{ex.Message}");
                return TextCommunicatorReadLineStatus.TimedOut;
            }
            catch (Exception ex)
            {
                logger.Debug($"ReceiveByteFromSerialPort {_serialPort.PortName} | Exception = {ex}");
                return TextCommunicatorReadLineStatus.CommunicationError;
            }

            string answer = ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray());
            logger.Info(
                string.Format("{0} ReadLine() answer = \"{1}\" | {2}",
                _serialPort.PortName,
                answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                BitConverter.ToString(Encoding.ASCII.GetBytes(answer))
                ));

            result = answer.TrimEnd(NewLine.ToCharArray());

            return TextCommunicatorReadLineStatus.Success;

        }

        public TextCommunicatorSendLineStatus SendLine(string cmd)
        {
            logger.Debug($"Start SendLine( {_serialPort.PortName} )");

            if (!_serialPort.IsOpen)
            {
                var errorMessage = $"SerialPort {_serialPort.PortName} not opened while try to SendLine";
                logger.Error(errorMessage);
                return TextCommunicatorSendLineStatus.CommunicationChannelClosed;
            }

            _serialPort.WriteTimeout = (int)Timeout.TotalMilliseconds;

            logger.Debug($"{_serialPort.PortName} WriteTimeout = {_serialPort.WriteTimeout}");

            try
            {
                var dataToSend = cmd + NewLine;

                logger.Info(
                    string.Format("SendLine( {0} ) dataToSend = {1} | {2}",
                    _serialPort.PortName,
                    dataToSend.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(dataToSend)))
                    );

                DateTime startTime = DateTime.Now;

                _serialPort.Write(dataToSend);

                SleepAfterSend(dataToSend.Length, startTime);

                return TextCommunicatorSendLineStatus.Success;
            }
            catch (TimeoutException)
            {
                logger.Debug($"SendLine({_serialPort.PortName}) TimeOut");
                return TextCommunicatorSendLineStatus.TimedOut;
            }
            catch (Exception)
            {
                return TextCommunicatorSendLineStatus.CommunicationError;
            }
        }

        public TextCommunicatorQueryCommandStatus QueryCommand(string cmd, out string result, Func<string, bool> validationRule)
        {
            result = string.Empty;

            int trynumber = 0;

            TextCommunicatorQueryCommandStatus status = TextCommunicatorQueryCommandStatus.CommunicationChannelClosed;

            while (trynumber < CommunicationRetries & status != TextCommunicatorQueryCommandStatus.Success)
            {
            _receivedBytes.Clear();

            _serialPort.DiscardOutBuffer();
            _serialPort.DiscardInBuffer(); // очищаем буфер приема

                switch (SendLine(cmd))
                {
                    case TextCommunicatorSendLineStatus.Success:
                        var readlineStatus = ReadLine(Timeout, out result);
                        switch (readlineStatus)
                        {
                            case TextCommunicatorReadLineStatus.Success:
                                if (validationRule == null || validationRule(result))
                                {
                                    status = TextCommunicatorQueryCommandStatus.Success;
                                }
                                else
                                {
                                    status = TextCommunicatorQueryCommandStatus.CommunicationError;
                                }
                                break;

                            case TextCommunicatorReadLineStatus.TimedOut:
                                status = TextCommunicatorQueryCommandStatus.ReadLineTimedOut;
                                break;

                            case TextCommunicatorReadLineStatus.CommunicationError:
                                status = TextCommunicatorQueryCommandStatus.CommunicationError;
                                break;

                            case TextCommunicatorReadLineStatus.CommunicationChannelClosed:
                                status = TextCommunicatorQueryCommandStatus.CommunicationChannelClosed;
                                break;
                        }
                        break;

                    case TextCommunicatorSendLineStatus.TimedOut:
                        status = TextCommunicatorQueryCommandStatus.SendTimedOut;
                        break;

                    case TextCommunicatorSendLineStatus.CommunicationError:
                        status = TextCommunicatorQueryCommandStatus.CommunicationError;
                        break;

                    case TextCommunicatorSendLineStatus.CommunicationChannelClosed:
                        status = TextCommunicatorQueryCommandStatus.CommunicationChannelClosed;
                        break;
                }
                trynumber++;
            }
            return status;
        }
        private void SleepAfterSend(int dataLength, DateTime startTime)
        {
            TimeSpan waitTime = CalculateTransmissionWaitTime(dataLength, startTime);

            if (waitTime.Milliseconds > 0)
                Thread.Sleep(waitTime);
        }

        private TimeSpan CalculateTransmissionWaitTime(int dataLength, DateTime startTime)
        {
            var REQUIRED_TRANSMISSION_TIME_FOR_BYTE = 1000 / _serialPort.BaudRate * 11;

            var ADDITIONAL_WAIT_TIME_AFTER_SEND = 20;

            TimeSpan requiredTransmissionTime = TimeSpan.FromMilliseconds(
                REQUIRED_TRANSMISSION_TIME_FOR_BYTE * dataLength + ADDITIONAL_WAIT_TIME_AFTER_SEND);
            //Convert.ToInt32(REQUIRED_TRANSMISSION_TIME_FOR_BYTE * dataLength + ADDITIONAL_WAIT_TIME_AFTER_SEND));
            return startTime + requiredTransmissionTime - DateTime.Now;
        }

        public void Dispose()
        {
            Close();
        }

        // the Serial Port detection routine 
        private bool TestSerialPort(IWBSerialPortWrapper sp)
        {

            try
            {
                sp.Open();
            }
            catch (Exception e)
            {
                // users don't want to experience this
                logger.Debug($"testSerialPort( { sp.PortName } ) catch (Exception) busy EXCEPTION { e }");
                return false;
            }
            logger.Debug($"testSerialPort( { sp.PortName } ) isopnen = { sp.IsOpen }");
            if (sp.IsOpen)
            {
                logger.Debug($"testSerialPort( { sp.PortName } )  if (sp.IsOpen) ");
                return true;
            }
            logger.Debug($"testSerialPort( { sp.PortName } ) after if (sp.IsOpen) ");
            return false;
        }

        private bool TryOpenSerialPort(IWBSerialPortWrapper serialPort)
        {
            bool res = false;

            var t = new Task<bool>(() => TestSerialPort(serialPort));
            try
            {
                logger.Debug($"tryOpenSerialPort( { serialPort.PortName} ) t.Start(); ");
                t.Start();
                logger.Debug($"tryOpenSerialPort( { serialPort.PortName} ) t.Wait(2500); ");
                t.Wait(2500);
                logger.Debug($"tryOpenSerialPort( {serialPort.PortName } ) after t.Wait(2500); { t.Status }");

            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                logger.Debug($"t.Wait(); for { serialPort.PortName } canceled");
            }
            catch (Exception e)
            {
                logger.Debug($"tryOpenSerialPort( { serialPort.PortName } ) TaskCanceledException EXCEPTION { e }");
                return false;
            }
            if (t.Status == TaskStatus.RanToCompletion)
            {
                logger.Debug($"tryOpenSerialPort( { serialPort.PortName } ) return true before res = t.Result ");
                res = t.Result;
                logger.Debug($"tryOpenSerialPort( { serialPort.PortName } ) return true after res = t.Result ");
                return res;
            }
            logger.Debug($"tryOpenSerialPort( { serialPort.PortName } ) return false; ");
            return false;
        }

        public override string ToString()
        {
            return $"{_serialPort}";
        }

        public bool IsOpen => _serialPort != null &&_serialPort.IsOpen;
    }
}

