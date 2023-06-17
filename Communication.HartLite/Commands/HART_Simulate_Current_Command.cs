using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_Simulate_Current_Command : HARTCommand
    {
        private float CurrentToSimulate { get; }
        public HART_Simulate_Current_Command(float currentReading)
        {
            CommandNumber = 40;
            CurrentToSimulate = currentReading;
        }
        public override byte[] ToByteArray()
        {
            Data = CurrentToSimulate.Single_to_HART_bytearray();
            return base.ToByteArray();
        }
    }
}
