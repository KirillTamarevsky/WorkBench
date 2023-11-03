using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_040_Simulate_Current_Command : HARTCommand
    {
        public float CurrentToSimulate { get; }
        public override byte Number => 40;
        public override byte[] Data => CurrentToSimulate.Single_to_HART_bytearray();
        public HART_040_Simulate_Current_Command(float currentReading)
        {
            CurrentToSimulate = currentReading;
        }
    }
}
