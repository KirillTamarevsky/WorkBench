using Communication.HartLite.CommandResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_20_ReadLongTag : HARTCommand
    {
        public override byte Number => 20;
        public override byte[] Data { get; } = new byte[0];
        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            return new HART_Result_20_Read_Long_Tag(datagram);
        }
    }
}
