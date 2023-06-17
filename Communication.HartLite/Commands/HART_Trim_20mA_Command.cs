using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_Trim_20mA_Command : HARTCommand
    {
        private float CurrentReadingToTrim { get; }
        public HART_Trim_20mA_Command(float currentReading)
        {
            CommandNumber = 46;
            CurrentReadingToTrim = currentReading;
        }
        public override byte[] ToByteArray()
        {
            Data = CurrentReadingToTrim.Single_to_HART_bytearray();
            return base.ToByteArray();
        }
    }
}
