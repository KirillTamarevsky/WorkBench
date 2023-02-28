using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using log4net;

namespace WorkBench.Communicators
{
    public class SerialEKCommunicator : ITextCommunicator, IDisposable
    {
        ILog logger = LogManager.GetLogger("Communication");

        SerialPort _serialPort;

        List<byte> _receivedBytes = new List<byte>();

        private string NewLine { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; }

        AutoResetEvent _waitForSerialData;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serialPortName"></param>
        /// <param name="baudrate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="lineEndToken">символы конца строки</param>
        public SerialEKCommunicator(
            string serialPortName, 
            int baudrate, 
            Parity parity, 
            int dataBits, 
            StopBits stopBits, 
            string lineEndToken)
        {
            _serialPort = new SerialPort(serialPortName, baudrate, parity, dataBits, stopBits);

            NewLine = lineEndToken;

            Timeout = TimeSpan.FromSeconds(3); // 3 seconds
        }

        public bool Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                //_serialPort.DataReceived -= OnDataReceivedFromSerialPort;
                
                logger.Info($"Closing SerialEKCommunicator on {_serialPort.PortName}");

                _serialPort.Close();
            }
            return true;
        }

        public bool Open()
        {
            bool serialPortOpened = false;

            if (_serialPort != null)
            {
                try
                {
                    logger.Debug(string.Format("try to Open {0} ", _serialPort.PortName));
                    logger.Debug(string.Format("before tryOpenSerialPort Open( {0} )", _serialPort.PortName));
                    serialPortOpened = tryOpenSerialPort(_serialPort);
                    logger.Debug(string.Format("after tryOpenSerialPort Open ( {0} ) opened = {1}", _serialPort.PortName, serialPortOpened));
                    
                    //_serialPort.DataReceived += OnDataReceivedFromSerialPort;
                    _waitForSerialData = new AutoResetEvent(false);
                }
                catch (Exception e)
                {
                    logger.Debug($"System.IO.IOException while try to open {_serialPort.PortName} | {e}");
                }
            }
            return serialPortOpened;
        }

        public string ReadLine(TimeSpan readLineTimeout)
        {


            _serialPort.RtsEnable = false;

            _serialPort.DtrEnable = true;

            logger.Debug($"{_serialPort.PortName} Start ReadLine(), Readtimeout = {readLineTimeout}");

            //Thread.Sleep(50);
            if (!_serialPort.IsOpen)
            {
                var errorMessage = $"SerialPort {_serialPort.PortName} not opened while try to ReadLine";
                logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }
            
            _serialPort.ReadTimeout = (int)Timeout.TotalMilliseconds;
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
                return string.Empty;
            }
            catch (Exception ex)
            {
                logger.Debug($"ReceiveByteFromSerialPort {_serialPort.PortName} | Exception = {ex}");
            }
            //if (!_waitForSerialData.WaitOne(readLineTimeout))
            //{
            //    logger.Warn(string.Format("{0} ReadLine() TimeOut, data in receive buffer = \"{1}\"|{2}", 
            //        _serialPort.PortName,
            //        ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray()).Replace("\r", "\\r").Replace("\n", "\\n"),
            //        BitConverter.ToString(_receivedBytes.ToArray())
            //        ));
            //    return string.Empty;
            //}

            string answer = ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray());
            logger.Info(
                string.Format("{0} ReadLine() answer = \"{1}\" | {2}",
                _serialPort.PortName,
                answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                BitConverter.ToString(Encoding.ASCII.GetBytes(answer))
                ));

            return answer.TrimEnd(NewLine.ToCharArray());

        }
        private void OnDataReceivedFromSerialPort(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;

            logger.Debug(string.Format("OnDataReceivedFromSerialPort fired for {0} | bytes to read = {1}", sp.PortName, sp.BytesToRead));

            switch (e.EventType)
            {
                case SerialData.Chars:

                    while (sp.BytesToRead > 0)
                    {
                        byte nextByte = (byte)sp.ReadByte();
                        
                        _receivedBytes.Add(nextByte);

                        if (ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray()).EndsWith(NewLine))
                        {
                            _waitForSerialData.Set();
                            return;
                        }
                    }
                    break;

                case SerialData.Eof:
                    
                    _waitForSerialData.Set();
                    break;

                default:
                    break;
            }
        }

        public bool SendLine(string cmd)
        {
            logger.Debug($"Start SendLine( {_serialPort.PortName} )");

            if (!_serialPort.IsOpen)
            {
                var errorMessage = $"SerialPort {_serialPort.PortName} not opened while try to SendLine";
                logger.Error(errorMessage);
                throw new Exception(errorMessage);
            }

            _serialPort.WriteTimeout = 1000;

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

                _serialPort.DtrEnable = false;
                _serialPort.RtsEnable = true;

                DateTime startTime = DateTime.Now;

                _serialPort.Write(dataToSend);

                SleepAfterSend(dataToSend.Length, startTime);

                _serialPort.RtsEnable = false;
                _serialPort.DtrEnable = true;
            }
            catch (TimeoutException)
            {
                logger.Debug($"SendLine({_serialPort.PortName}) TimeOut");
                return false;
            }
            return true;
        }

        public string QueryCommand(string cmd)
        {
            _receivedBytes.Clear();

            _serialPort.DiscardInBuffer(); // очищаем буфер приема
            
            _waitForSerialData.Reset();

            return SendLine(cmd) ? ReadLine(Timeout) : "";
        }

        private void SleepAfterSend(int dataLength, DateTime startTime)
        {
            TimeSpan waitTime = CalculateWaitTime(dataLength, startTime);

            if (waitTime.Milliseconds > 0)
                Thread.Sleep(waitTime);
        }

        private TimeSpan CalculateWaitTime(int dataLength, DateTime startTime)
        {
            var REQUIRED_TRANSMISSION_TIME_FOR_BYTE = 1000 / _serialPort.BaudRate * 11;

            var ADDITIONAL_WAIT_TIME_AFTER_SEND = 20;

            TimeSpan requiredTransmissionTime = TimeSpan.FromMilliseconds(
                Convert.ToInt32(REQUIRED_TRANSMISSION_TIME_FOR_BYTE * dataLength + ADDITIONAL_WAIT_TIME_AFTER_SEND));
            return startTime + requiredTransmissionTime - DateTime.Now;
        }

        public void Dispose()
        {
            Close();
        }


        // the Serial Port detection routine 
        private bool testSerialPort(SerialPort sp)
        {

            try
            {
                sp.Open();
            }
            catch (Exception e)
            {
                // users don't want to experience this
                logger.Debug("testSerialPort( " + sp.PortName + " ) catch (Exception) busy EXCEPTION " + e);
                return false;
            }
            logger.Debug("testSerialPort( " + sp.PortName + " ) isopnen = " + sp.IsOpen);
            if (sp.IsOpen)
            {
                logger.Debug("testSerialPort( " + sp.PortName + " )  if (sp.IsOpen) ");
                return true;
            }
            logger.Debug("testSerialPort( " + sp.PortName + " ) after if (sp.IsOpen) ");
            return false;
        }

        // the callback function of button checks the serial ports
        private bool tryOpenSerialPort(SerialPort serialPort)
        {
            bool res = false;

            Task<bool> t = new Task<bool>(() => testSerialPort(serialPort));
            try
            {
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") t.Start(); ");
                t.Start();
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") t.Wait(2500); ");
                t.Wait(2500);
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") after t.Wait(2500); " + t.Status);

            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                logger.Debug("t.Wait(); for " + serialPort.PortName + " canceled");
            }
            catch (Exception e)
            {
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") TaskCanceledException EXCEPTION " + e);
                return false;
            }
            if (t.Status == TaskStatus.RanToCompletion)
            {
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") return true before res = t.Result ");
                res = t.Result;
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") return true after res = t.Result ");
                return res;
            }
            logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") return false; ");
            return false;
        }

        public override string ToString()
        {
            return string.Format("последовательный порт {0}", _serialPort.PortName);
        }

        public bool IsOpen => _serialPort != null &&_serialPort.IsOpen;
    }
}

