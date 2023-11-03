using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_046_Trim_Trim_Loop_Current_Gain : HARTCommand
    {
        private float CurrentReadingToTrim { get; }
        public override byte Number => 46;
        public override byte[] Data => CurrentReadingToTrim.Single_to_HART_bytearray();
        public HART_046_Trim_Trim_Loop_Current_Gain(float currentReading)
        {
            CurrentReadingToTrim = currentReading;
        }
    }
}
