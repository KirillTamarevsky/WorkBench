using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.Communicators
{
    public class SerialCPC6000Communicator : ICommunicator, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("CPC6000Communication");

        SerialPort _serialPort;

        public SerialCPC6000Communicator(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(serialPortName, baudrate, parity, dataBits, stopBits);
         
            _serialPort.NewLine = "\n";
        }

        public bool Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                logger.Info(string.Format("Closing SerialCPC6000Communicator on {0}", _serialPort.PortName));

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
                    opened = tryOpenSerialPort(_serialPort);
                }
                catch (Exception e)
                {
                    logger.Debug("System.IO.IOException while try to open " + _serialPort.PortName + " " + e);
                }
            }
            return opened;
        }

        public string ReadLine()
        {
            logger.Debug("Start ReadLine( " + _serialPort.PortName + " )");

            _serialPort.ReadTimeout = 3000;
            
            string answer = "";
            
            try
            {
                if (_serialPort.IsOpen)
                {
                    logger.Debug("Start ReadLine( " + _serialPort.PortName + " ) is open");

                    answer = _serialPort.ReadLine();

                    logger.Debug("ReadLine( " + _serialPort.PortName + " ) answer = " + answer.Replace("\r", "\\r").Replace("\n", "\\n") + " " + BitConverter.ToString(Encoding.ASCII.GetBytes(answer)));
                }
            }
            catch (TimeoutException)
            {
                answer = "";

                logger.Debug("ReadLine( " + _serialPort.PortName + " ) TimeOut ");
            }

            return answer;
        }

        public bool SendLine(string cmd)
        {
            logger.Debug("Start SendLine( " + _serialPort.PortName + " )");

            if (_serialPort.IsOpen)
            {
                _serialPort.WriteTimeout = 1000;
                try
                {
                    var dataToSend = cmd + "\r\n";

                    logger.Info(
                        string.Format("SendLine( {0} ) dataToSend = {1} | {2}",
                        _serialPort.PortName, 
                        dataToSend.Replace("\r", "\\r").Replace("\n", "\\n"),
                        BitConverter.ToString(Encoding.ASCII.GetBytes(dataToSend)))
                        );

                    _serialPort.Write(dataToSend);
                }
                catch (TimeoutException)
                {
                    logger.Debug(string.Format("SendLine( {0} ) TimeOut", _serialPort.PortName));
                    return false;
                }
                return true;
            }
            return false;
        }

        public string QueryCommand(string cmd)
        {
            return SendLine(cmd) ? ReadLine() : "";
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
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") t.Wait(500); ");
                t.Wait(500);
                logger.Debug("tryOpenSerialPort(" + serialPort.PortName + ") after t.Wait(500); " + t.Status);

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
    }
}
