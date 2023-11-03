using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{

    public class HART_Result_044_Write_Primary_Variable_Units : CommandResult
    {
        internal HART_Result_044_Write_Primary_Variable_Units(HARTDatagram command) : base(command)
        {
        }
        public byte UnitCode => Data[0];
    }
}
