using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.Communicators
{
    public class FakeCPC6000SerialPort : IWBSerialPortWrapper, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("CPC6000Communication");

        string _serialPortName;
        Parity _parity;
        int _dataBits;
        StopBits _stopBits;

        string _serialPortLineEndToken;

        string _sendedCommand;

        string _currentUOM = "Pascal";

        string _setpt = "0.1234";

        string _currentChannel = "A";


        bool isopened;
        string answer = string.Empty;

        Queue<byte> answerBytesQueue = new Queue<byte> ();

        public event SerialDataReceivedEventHandler DataReceived;
        void RaiseDataReceived()
        {
            ConstructorInfo constructor = typeof(SerialDataReceivedEventArgs).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new[] { typeof(SerialData) },
                null);
                        
            SerialDataReceivedEventArgs eventArgs =
                (SerialDataReceivedEventArgs)constructor.Invoke(new object[] { SerialData.Chars });

            DataReceived?.Invoke(this, eventArgs);
        }

        public FakeCPC6000SerialPort(string serialPortName, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            _serialPortName = serialPortName;
            BaudRate = baudrate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;

            _serialPortLineEndToken = "\n";
        }
        public void Open()
        {
            isopened = true;
        }

        public void Close()
        {
            isopened = false;
        }
        public bool IsOpen => isopened;

        public string PortName => _serialPortName;

        public int BytesToRead => answerBytesQueue.Count;

        public int WriteTimeout { get ; set; }
        public int BaudRate { get ; set ; }

        public Stream BaseStream => throw new NotImplementedException();
        internal int _timeout;
        public int ReadTimeout { get => _timeout; set => _timeout = value; }

        public Task<string> ReadLine(TimeSpan readLineTimeout)
        {
            logger.Debug($"Start ReadLine({this})");

            string answer = "";
            
            logger.Debug($"Start ReadLine({this}) is open");

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
                    Thread.Sleep(500);
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
                    this.ToString(),
                    answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(answer))));

            return Task.FromResult( answer);
        }

        public bool SendLine(string cmd)
        {
            logger.Debug($"Start SendLine({this})");

            var dataToSend = cmd + "\r";

            logger.Info(
                string.Format("SendLine( {0} ) dataToSend = \"{1}\" | {2}",
                this.ToString(), 
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

        public void Dispose()
        {
            Close();
        }

        public override string ToString()
        {
            return string.Format("демо CPC порт {0}", _serialPortName);
        }

        public void Write(string str)
        {
            str = str.TrimEnd(new char[] { '\r', '\n' });

            var cmdparts = str.Split(' ');

            var first = cmdparts[0];

            switch (first)
            {
                case "ID?":
                    Thread.Sleep(250);
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
                    //Thread.Sleep(5);
                    answer = string.Format(
                        " {0}",
                        ((new System.Random()).NextDouble()/10 + 5).ToString("N4")
                        );
                    break;
                case "B?":
                    //Thread.Sleep(500);
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
            answer += "\n";
            foreach (byte item in answer.ToCharArray())
            {
                answerBytesQueue.Enqueue(item);
            }
            RaiseDataReceived();
        }

        public void DiscardOutBuffer()
        {
        }

        public void DiscardInBuffer()
        {
            answer = string.Empty;
            answerBytesQueue.Clear();
        }

        public int ReadByte()
        {
            return answerBytesQueue.Dequeue();
        }


    }
}
