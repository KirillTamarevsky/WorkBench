﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_Trim_4mA_Command : HARTCommand
    {
        private float CurrentReadingToTrim { get; }
        public override byte Number => 45;
        public override byte[] Data => CurrentReadingToTrim.Single_to_HART_bytearray();
        public HART_Trim_4mA_Command(float currentReading)
        {
            CurrentReadingToTrim = currentReading;
        }
    }
}
