using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Communicators
{
    public interface IWBSerialPortWrapper
    {
        string PortName { get; }
        int BytesToRead { get; }
        int WriteTimeout { get; set; }
        int ReadTimeout { get; set; }
        int BaudRate { get; set; }
        void Write(string str);
        void DiscardOutBuffer();
        void DiscardInBuffer();
        int ReadByte();
        bool IsOpen { get; }
        void Open();
        void Close();

        event SerialDataReceivedEventHandler DataReceived;

        Stream BaseStream{ get; }
    }
}
