using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.Communicators
{
    public class SerialEKCommunicator : ICommunicator, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("Communication");

        SerialPort _serialPort;

        List<byte> _receivedBytes = new List<byte>();
        
        string _newLine = "\r\n";

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        /// <value>The timeout.</value>
        public TimeSpan Timeout { get; set; }

        AutoResetEvent _waitForSerialData;


        public SerialEKCommunicator(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(serialPortName, baudrate, parity, dataBits, stopBits);
            Timeout = new TimeSpan(0, 0, 0, 3); // 3 seconds

        //_serialPort.NewLine = "\n";
    }

    public bool Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            return true;
        }

        public bool Open()
        {
            bool opened = false;

            if (_serialPort != null)
            {
                try
                {
                    logger.Debug(string.Format("try to Open {0} ", _serialPort.PortName));
                    logger.Debug(string.Format("before tryOpenSerialPort Open( {0} )", _serialPort.PortName));
                    opened = tryOpenSerialPort( _serialPort);
                    logger.Debug(string.Format("after tryOpenSerialPort Open ( {0} ) opened = {1}", _serialPort.PortName, opened));
                }
                catch(Exception e)
                {
                    logger.Debug(string.Format("System.IO.IOException while try to open {0} | {1}", _serialPort.PortName, e));
                }
            }
            return opened;
        }

        public string ReadLine()
        {
            _serialPort.DataReceived += OnDataReceivedFromSerialPort;

            logger.Debug(string.Format("Start ReadLine( {0} )", _serialPort.PortName ));
            
            //_serialPort.ReadTimeout = 3000;

            logger.Debug(string.Format("{0} Readtimeout = {1}", _serialPort.PortName, Timeout));
            
            string answer = "";

            _receivedBytes.Clear();

            _serialPort.DiscardInBuffer(); // очищаем буфер приема

            Thread.Sleep(50);

            _waitForSerialData = new AutoResetEvent(false);

            _serialPort.RtsEnable = false;

            _serialPort.DtrEnable = true;

            if (!_waitForSerialData.WaitOne(Timeout))
            {
                
                _serialPort.DataReceived -= OnDataReceivedFromSerialPort;

                logger.Warn(string.Format("ReadLine( {0} ) TimeOut", _serialPort.PortName));

                return answer;
            }

            _serialPort.DataReceived -= OnDataReceivedFromSerialPort;

            var receivedString = ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray());

            logger.Info(string.Format("ReadLine( {0} ) answer = {1} | {2}",
                _serialPort.PortName, 
                answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                BitConverter.ToString(Encoding.ASCII.GetBytes(answer))
                ));

            return receivedString.TrimEnd(_newLine.ToCharArray());

            //try
            //{
            //    if (_serialPort.IsOpen)
            //    {
            //        logger.Debug("Start ReadLine( " + _serialPort.PortName + ") is open");

            //        answer = _serialPort.ReadTo("\r\n");

            //        logger.Debug("ReadLine( " + _serialPort.PortName + " ) answer = " + answer.Replace("\r", "\\r").Replace("\n", "\\n") + " " + BitConverter.ToString(Encoding.ASCII.GetBytes(answer)));
            //    }
            //}
            //catch (TimeoutException)
            //{
            //    answer = "";

            //    logger.Debug("ReadLine( " +_serialPort.PortName + " ) TimeOut " );
            //}

            //return answer;
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
                        _receivedBytes.Add((byte)sp.ReadByte());

                        if (ASCIIEncoding.ASCII.GetString(_receivedBytes.ToArray()).EndsWith(_newLine))
                        {
                            sp.DiscardInBuffer();
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
            logger.Debug(string.Format("Start SendLine( {0} )", _serialPort.PortName ));
            
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteTimeout = 1000;

                logger.Debug(string.Format("{0} WriteTimeout = {1}", _serialPort.PortName, _serialPort.WriteTimeout));

                try
                {
                    var dataToSend = cmd;
                    
                    logger.Info(
                        string.Format("SendLine( {0} ) dataToSend = {1} | {2}",
                        _serialPort.PortName,
                        (dataToSend + "\r\n").Replace("\r", "\\r").Replace("\n", "\\n"),
                        BitConverter.ToString(Encoding.ASCII.GetBytes(dataToSend)))
                        );
                    _serialPort.DtrEnable = false;
                    _serialPort.RtsEnable = true;

                    DateTime startTime = DateTime.Now;

                    _serialPort.Write(dataToSend);
                    _serialPort.Write("\r\n");

                    SleepAfterSend(dataToSend.Length * 2, startTime);

                    _serialPort.RtsEnable = false;
                    _serialPort.DtrEnable = true;
                }
                catch (TimeoutException)
                {
                    logger.Debug("SendLine() TimeOut" + _serialPort.PortName);
                    return false;
                }
                return true;
            }
            return false;
        }

        public string QueryCommand(string cmd)
        {
            return SendLine(cmd)? ReadLine() : "" ;
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
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") after t.Wait(2500); " +  t.Status);

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
                res =  t.Result;
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") return true after res = t.Result ");
                return res;
            }
            logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") return false; ");
            return false;
        }

        public override string ToString()
        {
            return string.Format( "последовательный порт {0}" , _serialPort.PortName);
        }
    }





}

