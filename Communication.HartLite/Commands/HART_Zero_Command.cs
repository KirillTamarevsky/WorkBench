using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.HartLite.CommandResults;

namespace Communication.HartLite.Commands
{
    public class HART_Zero_Command : HARTCommand
    {
        public override byte Number => 0;
        public override byte[] Data { get; } = new byte[0];
        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            if (datagram.CommandNumber == Number)
            {
                if (datagram.Data[4] == 5)
                {
                    return new HART_Result_Zero_Command_v5(datagram);
                }
                if (datagram.Data[4] == 7)
                {
                    return new HART_Result_Zero_Command_v7(datagram);
                }
            }
            return base.ToCommandResult(datagram);
        }
    }
    
}
