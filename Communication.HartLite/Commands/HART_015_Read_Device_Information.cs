using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.HartLite.CommandResults;
namespace Communication.HartLite.Commands
{
    public class HART_015_Read_Device_Information : HARTCommand
    {
        public override byte Number => 15;
        public override byte[] Data => new byte[] { };
        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            if (datagram.CommandNumber == Number) return new HART_Result_015_Read_Device_Information(datagram);
            return base.ToCommandResult(datagram);
        }
    }
}
