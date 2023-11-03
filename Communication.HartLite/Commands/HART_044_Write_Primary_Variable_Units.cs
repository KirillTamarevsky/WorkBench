using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication.HartLite.CommandResults;

namespace Communication.HartLite.Commands
{
    public class HART_044_Write_Primary_Variable_Units : HARTCommand
    {
        public override byte Number => 44;
        public override byte[] Data => new byte[] { _unitCode}; 
        private byte _unitCode;
        public HART_044_Write_Primary_Variable_Units(byte unitCode)
        {
            _unitCode = unitCode;
        }
        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            if (datagram.CommandNumber == Number) return new HART_Result_044_Write_Primary_Variable_Units(datagram);
            
            return base.ToCommandResult(datagram);
        }
    }
}
