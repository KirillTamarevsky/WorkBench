using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{
    public class HART_Result_020_Read_Long_Tag : CommandResult
    {
        internal HART_Result_020_Read_Long_Tag(HARTDatagram command) : base(command)
        {
        }
        public string LongTag
        {
            get
            {
                if (Data != null && Data.Length == 32)
                {
                    return Encoding.ASCII.GetString(Data);
                }
                else { return string.Empty; }
            }
        }
    }
}
