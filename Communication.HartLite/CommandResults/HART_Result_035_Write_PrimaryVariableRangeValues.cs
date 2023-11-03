using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Communication.HartLite.CommandResults
{
    public class HART_Result_035_Write_PrimaryVariableRangeValues : CommandResult
    {
        internal HART_Result_035_Write_PrimaryVariableRangeValues(HARTDatagram command) : base(command)
        {
        }
        public byte UnitsCode => Data[0];
        public double PVUpperRangeValue =>(new byte[] { Data[1], Data[2], Data[3], Data[4] }).To_HART_Single();
        public double PVLowerRangeValue => (new byte[] { Data[5], Data[6], Data[7], Data[8] }).To_HART_Single();
    }
}
