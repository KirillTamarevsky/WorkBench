using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Communicators
{
    public class WBSerialPortWrapper : IWBSerialPortWrapper
    {
        readonly SerialPort port;
        public WBSerialPortWrapper(
            string serialPortName,
            int baudrate,
            Parity parity,
            int dataBits,
            StopBits stopBits)
        {
            port = new SerialPort(serialPortName, baudrate, parity, dataBits, stopBits);
        }

        public string PortName => port.PortName;

        public int BytesToRead => port.BytesToRead;

        public int WriteTimeout
        {
            get => port.WriteTimeout;
            set => port.WriteTimeout = value;
        }
        public int ReadTimeout
        {
            get => port.ReadTimeout;
            set => port.ReadTimeout = value;
        }
        public int BaudRate 
        {
            get => port.BaudRate;
            set => port.BaudRate = value;
        }

        public bool IsOpen => port.IsOpen;

        public event SerialDataReceivedEventHandler DataReceived;
        public void Open()
        {
            port.Open();
            port.DataReceived += new SerialDataReceivedEventHandler( OnPortDataReceived ); 
        }
        public void Close()
        {
            port.DataReceived -= OnPortDataReceived; 
            port.Close();
        }
        private void OnPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public virtual void DiscardInBuffer()
        {
            port.DiscardInBuffer();
        }

        public virtual void DiscardOutBuffer()
        {
            port.DiscardOutBuffer();
        }


        public virtual int ReadByte()
        {
            try
            {
                return port.ReadByte();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Write(string str)
        {
            port.Write(str);
        }
        public override string ToString()
        {
            return string.Format("последовательный порт {0}", port.PortName);
        }

        public virtual Stream BaseStream
        {
            get => port.BaseStream;
        }
    }
}
