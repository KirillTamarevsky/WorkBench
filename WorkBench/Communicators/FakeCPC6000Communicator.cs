using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.Communicators
{
    public class FakeCPC6000Communicator : ICommunicator, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("CPC6000Communication");

        SerialPort _serialPort;

        string _sendedCommand;

        string _currentUOM = "bar";

        string _setpt = "0.1234";

        string _currentChannel = "A";

        public FakeCPC6000Communicator(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPort = new SerialPort(serialPortName, baudrate, parity, dataBits, stopBits);
         
            _serialPort.NewLine = "\n";
        }

        public bool Close()
        {

            return true;
        }

        public bool Open()
        {

            return true;
        }

        public string ReadLine()
        {
            logger.Debug("Start ReadLine( " + _serialPort.PortName + " )");

            string answer = "";
            
            logger.Debug("Start ReadLine( " + _serialPort.PortName + " ) is open");

            switch (_sendedCommand.Trim())
            {
                case "ID?":
                    answer = "EMENSOR, 600, 12345, 666";
                    break;
                case "RangeMin?":
                    switch (_currentChannel)
                    {
                        case "A":
                            answer = " -1";
                            break;
                        case "B":
                            answer = " 0";
                            break;
                        default:
                            break;
                    }
                    break;
                case "RangeMax?":
                    switch (_currentChannel)
                    {
                        case "A":
                            answer = " 15";
                            break;
                        case "B":
                            answer = " 65";
                            break;
                        default:
                            break;
                    }
                    break;
                case "Units?":
                    answer = string.Format(" {0}", _currentUOM);
                    break;
                case "A?":
                    Thread.Sleep(25);
                    answer = string.Format(
                        " {0}", 
                        ((new System.Random()).NextDouble() * 35).ToString("N4")
                        );
                    break;
                case "B?":
                    Thread.Sleep(250);
                    answer = string.Format(
                        " {0}",
                        ((new System.Random()).NextDouble() * 35 + 35).ToString("N4")
                        );
                    break;
                case "Setpt?":
                    answer = _setpt;
                    break;
                case "":

                    break;
                case "Mode?":
                    answer = " CONTROL";
                    break;
                default :
                    break;
            }

            logger.Info(
                string.Format(
                    "ReadLine( {0} ) answer = \"{1}\" | {2}",
                    _serialPort.PortName,
                    answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(answer))));

            return answer;
        }

        public bool SendLine(string cmd)
        {
            logger.Debug("Start SendLine( " + _serialPort.PortName + " )");

            var dataToSend = cmd + "\r";

            logger.Info(
                string.Format("SendLine( {0} ) dataToSend = \"{1}\" | {2}",
                _serialPort.PortName, 
                dataToSend.Replace("\r", "\\r").Replace("\n", "\\n"),
                BitConverter.ToString(Encoding.ASCII.GetBytes(dataToSend)))
                );

            _sendedCommand =dataToSend;

            var cmdparts = _sendedCommand.Split(' ');

            switch (cmdparts[0])
            {
                case "Units":
                    _currentUOM = cmdparts[1];
                    break;
                case "Setpt":
                    _setpt = cmdparts[1];
                    break;
                case "Chan":
                    _currentChannel = cmdparts[1].Trim();
                    break;
                default:
                    break;
            }

            return true;

        }

        public string QueryCommand(string cmd)
        {
            return SendLine(cmd) ? ReadLine() : "";
        }

        public void Dispose()
        {
            Close();
        }

        public override string ToString()
        {
            return string.Format("демо CPC порт {0}", _serialPort.PortName);
        }
    }
}
