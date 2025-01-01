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
            lastCurrentPoll = DateTime.Now;
        }
        public double MinSpan { get; set; }
        public double MaxSpan { get; set; }
        private DateTime lastCurrentPoll {  get; set; }
        private double tauMilliSeconds { get => 850; }
        public Double OutputCurrent 
        { get 
            {
                var _lastCurrentPoll = DateTime.Now;
                var pollTimeDelta = (_lastCurrentPoll - lastCurrentPoll).TotalMilliseconds;
                lastCurrentPoll = _lastCurrentPoll;
                var outputPressureDelta = InputPressure -  (InputPressure - outputpressure) * Math.Exp(-(pollTimeDelta / tauMilliSeconds) );
                outputpressure = outputPressureDelta; // (InputPressure - outputpressure) / 45;
                return  outputpressure/(MaxSpan - MinSpan) * 16 + 4;
            } 
        }
        double _inputPressure;
        public double InputPressure { get => _inputPressure; set { _inputPressure = value; } }
        private double outputpressure;

    }
}
