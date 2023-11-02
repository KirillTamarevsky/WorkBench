using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite
{
    public class FSKReceivedByte
    {
        public DateTime TimeStamp { get; private set; }
        public byte ReceivedByte { get; private set; }
        public FSKReceivedByte(DateTime _timestamp, byte _receivedbyte)
        {
            TimeStamp = _timestamp; 
            ReceivedByte = _receivedbyte;
        }
    }
}
