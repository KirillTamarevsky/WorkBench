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

namespace WorkBench.TestEquipment.ElmetroPascal
{

    public class FakeEPascalSerialPort : IWBSerialPortWrapper, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("EPascalCommunication");

        bool isopened;
        bool venton;
        bool regulationActive;
        string _serialPortName;
        float setpoint;
        Parity _parity;
        int _dataBits;
        StopBits _stopBits;
        string _serialPortLineEndToken;

        Queue<byte> answerBytesQueue = new Queue<byte>();

        public FakeEPascalSerialPort(string serialPortName,
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
            str = str.TrimEnd(new char[] { '\r', '\n' });
            string answer = string.Empty;
            Thread.Sleep(50);
            switch (str.Trim())
            {
                case "R":
                    answer = "REMOTE";
                    venton = true;
                    regulationActive = false;
                    setpoint = 0;
                    break;
                case "LOCAL":
                    answer = "OK";
                    break;
                case "PRES?":
                    answer = (new Random().NextDouble() * .2 + setpoint).ToString("N4");
                    break;
                case "READ_M1?":
                    answer = "2: [   0.0 3500.0][   0.0 1600.0]";
                    break;
                case "SEEK_MODUL":
                    answer = "OK";
                    break;
                case "READ_M2?":
                    //answer = "0: ";
                    answer = "4: [   0.0 6000.0][   0.0 4000.0][   0.0 2500.0][   0.0 1600.0]";
                    //answer = "3: [   0.0  25.0] [   0.0  16.0] [   0.0  10.0] ";
                    //answer = "1: [   0.0 1600.0]";
                    break;
                case "CLEAR_P":
                    answer = "OK";
                    break;
                case "ON_KEY_VENT":
                    answer = venton ? "VENT_OFF" : "VENT_ON";
                    venton = !venton;
                    break;
                case "ON_KEY_START":
                    answer = regulationActive ? "STOP_REGULATION" : "START_REGULATION";
                    regulationActive = !regulationActive;
                    break;
                default:
                    break;
            }
            if (str.Contains("TARGET"))
            {
                answer = "OK";
                var parts = str.Split(' ');
                if (parts.Length > 1)
                {
                    float.TryParse(parts[parts.Length - 1], CultureInfo.InvariantCulture, out setpoint);
                    setpoint = float.Parse(parts[parts.Length - 1], CultureInfo.InvariantCulture);
                }
            }
            if (str.Contains("RANGE")) { answer = "OK"; regulationActive = false; venton = false; }
            logger.Info(
                string.Format(
                    "Readline = {0} | {1}",
                    answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(answer))));
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
