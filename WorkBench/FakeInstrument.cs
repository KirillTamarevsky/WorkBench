using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.UOMS;

namespace WorkBench
{
    public class FakeInstrument
    {
        public FakeInstrument() 
        {
            MinSpan = 0;
            MaxSpan = 40;
            InputPressure = 0;
            outputpressure = 0;
        }
        public double MinSpan { get; set; }
        public double MaxSpan { get; set; }
        public Double OutputCurrent 
        { get 
            {
                outputpressure += (InputPressure - outputpressure) / 45;
                return  outputpressure/(MaxSpan - MinSpan) * 16 + 4;
            } 
        }
        public double InputPressure { get; set; }
        private double outputpressure;
    }
}
