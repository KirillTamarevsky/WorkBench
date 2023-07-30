using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Communicators;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.EK
{

    public class FakeEKSerialPort : IWBSerialPortWrapper, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("Communication");

        Random random { get; }

        bool isopened;

        string _serialPortName;
        Parity _parity;
        int _dataBits;
        StopBits _stopBits;
        string _serialPortLineEndToken;

        Queue<byte> answerBytesQueue = new Queue<byte>();
        public FakeEKSerialPort(string serialPortName,
                                int baudrate,
                                Parity parity,
                                int dataBits,
                                StopBits stopBits)
        {
            _serialPortName = serialPortName;
            BaudRate = baudrate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;

            _serialPortLineEndToken = "\r\n";

            random = new Random();
        }
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
        public int WriteTimeout { get; set; }
        public int BaudRate { get; set; }
        public Stream BaseStream => throw new NotImplementedException();
        internal int _timeout;
        public int ReadTimeout { get => _timeout; set => _timeout = value; }

        int activeChannel { get; set; } = 1;
        public void Write(string str)
        {
            str = str.TrimEnd(new char[] { '\r', '\n' });
            string answer = string.Empty;
            Thread.Sleep(250);
            switch (str.Trim())
            {
                case "REMOTE":
                    answer = "R";
                    break;
                case "LOCAL":
                    answer = "L";
                    break;
                case "TCURR?":
                    answer = (new Random().NextDouble() * 1 + activeChannel - .5).ToString("N4");
                    break;
                case "CURR?":
                    //prevValue += (new System.Random()).NextDouble() * 0.0016;
                    //answer = prevValue.ToString("N4");
                    answer = (new Random().NextDouble() * 1 + activeChannel - .5 + 10).ToString("N4");
                    break;
                case "CHAN 1":
                    answer = "1";
                    activeChannel = 1;
                    break;
                case "CHAN 2":
                    answer = "2";
                    activeChannel = 2;
                    break;
                case "CHAN 3":
                    answer = "3";
                    activeChannel = 3;
                    break;
                default:
                    break;
            }
            logger.Info(
                string.Format(
                    "Readline = {0} | {1}",
                    answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(answer))));

            if (random.Next(100) > 40)
            {
                answer = string.Empty;
            }

            answer += _serialPortLineEndToken;
            foreach (byte item in answer.ToCharArray())
            {
                answerBytesQueue.Enqueue(item);
            }
            RaiseDataReceived();
        }

        public void DiscardOutBuffer() { }
        public void DiscardInBuffer()
        {
            answerBytesQueue.Clear();
        }
        public int ReadByte()
        {
            return answerBytesQueue.Dequeue();
        }
        public override string ToString()
        {
            return string.Format("демо порт {0}", _serialPortName);
        }
        public void Dispose()
        {
            Close();
        }
    }
}
