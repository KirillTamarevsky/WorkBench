using Communication.HartLite.CommandResults;
using Communication.HartLite.CommonTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_109_Burst_Mode_Control : HARTCommand
    {
        public override byte Number => 109;
        public override byte[] Data => ToByteArray();

        _009_Burst_Mode_Control_Code _mode;
        byte _burstMessage;
        public HART_109_Burst_Mode_Control(_009_Burst_Mode_Control_Code mode, byte burstMessage)
        {
            _mode = mode;
            _burstMessage = burstMessage;
        }
        private byte[] ToByteArray()
        {
            return new byte[2] { (byte)_mode, _burstMessage };
        }
        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            return new HART_Result_109_Burst_Mode_Control(datagram);
        }
    }
}
