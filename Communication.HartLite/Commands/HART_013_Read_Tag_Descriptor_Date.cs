using Communication.HartLite.CommandResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_013_Read_Tag_Descriptor_Date : HARTCommand
    {
        public override byte Number => 13;
        public override byte[] Data { get; } = new byte[0];
        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            return new HART_Result_013_Tag_Descriptor_Date (datagram);
        }
    }
}
