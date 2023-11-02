using Communication.HartLite.CommonTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{
    public class HART_Result_109_Burst_Mode_Control : CommandResult
    {
        internal HART_Result_109_Burst_Mode_Control(HARTDatagram command) : base(command)
        {
        }
        public _009_Burst_Mode_Control_Code Burst_Mode_Control_Code => (_009_Burst_Mode_Control_Code)Data[0];
        public byte BurstMessage => Data[1];
    }
}
