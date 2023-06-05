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
                    answer = $" {_currentUOM}";
                    break;
                case "A?":
                    //Thread.Sleep(5);
                    answer = $" {(new System.Random().NextDouble() / 10 + 5).ToString("N4")}";
                    break;
                case "B?":
                    //Thread.Sleep(500);
                    answer = $" {(new System.Random().NextDouble() * 35 + 35).ToString("N4")}";
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
                case "List?":
                    switch (_currentChannel)
                    {
                        case "A":
                            answer = " PRI,1;SEC,1;BAR,1";
                            break;
                        case "B":
                            answer = " PRI,1;SEC,1";
                            break;
                    }
                    break;
                case "Listrange?":
                    switch (_currentChannel)
                    {
                        case "A":
                            answer = " PRI,1, +0.0000,+11.0000;SEC,1,+0.00000,+5.20000;BAR,1,+0.55158,+1.17211";
                            break;
                        case "B":
                            answer = " PRI,1,  +0.000,+101.000;SEC,1, +0.0000,+31.0000";
                            break;
                    }
                    break;
                default:
                    break;
            }
            answer += _serialPortLineEndToken;
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
