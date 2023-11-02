using System;
using System.IO.Ports;

namespace Communication.HartLite
{
    public interface ISerialPortWrapper
    {
        void Open();
        void Close();

        int Read(FSKReceivedByte[] buffer, int offset, int count);
        void Write(byte[] buffer, int offset, int count);
        int BytesToRead { get; }
        string PortName { get; set; }

        bool DtrEnable { get; set; }
        bool RtsEnable { get; set; }
        bool CtsHolding { get; }
        bool CDHolding { get; }
        bool IsOpen { get; }
        int ReadTimeout { get; set; }

        event Action<FSKReceivedByte> DataReceived;
    }
}