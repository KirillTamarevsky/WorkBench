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
using WorkBench.Communicators;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.Calys150
{

    public class FakeCalys150SerialPort : IWBSerialPortWrapper, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("Communication");

        private Random random = new Random();

        bool isopened;

        string _serialPortName;
        Parity _parity;
        int _dataBits;
        StopBits _stopBits;
        string AnswerTerminationToken { get; }

        Queue<byte> answerBytesQueue = new Queue<byte>();
        public FakeCalys150SerialPort(string serialPortName,
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

            AnswerTerminationToken = "\r\n";
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
        public void Write(string str)
        {
            string answer = string.Empty;
            str = str.TrimEnd(new char[] { '\r', '\n' });
            Thread.Sleep(100);
            switch (str.Trim().ToUpper())
            {
                case "REMOTE":
                case "REM":
                    break;
                case "LOCAL":
                case "LOC":
                    break;
                case "*IDN?":
                    answer = "AOIP˽SAS,CALYS150,SN_1234,A00";
                    break;
                case "MEAS1?":
                case "MEAS2?":
                    var randnum = (random.NextDouble() * 16.9 + 3.5).ToString("N4", CultureInfo.InvariantCulture);
                    answer = $"{randnum},vA";
                    break;
                default:
                    break;
            }
            if (answer.Length > 0)
            {
                answer += AnswerTerminationToken;
                foreach (byte item in answer.ToCharArray())
                {
                    answerBytesQueue.Enqueue(item);
                }
                RaiseDataReceived();
            }
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
