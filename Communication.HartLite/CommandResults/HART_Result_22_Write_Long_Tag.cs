using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{
    public class HART_Result_22_Write_Long_Tag : CommandResult
    {
        internal HART_Result_22_Write_Long_Tag(HARTDatagram command) : base(command)
        {
        }
        public string LongTag => ASCIIEncoding.ASCII.GetString(Data);
    }
}
