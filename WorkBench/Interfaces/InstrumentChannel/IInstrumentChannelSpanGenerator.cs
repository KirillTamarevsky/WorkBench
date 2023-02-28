using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces.InstrumentChannel
{
    public interface IInstrumentChannelSpanGenerator : IInstrumentChannelSpan
    {
        void GetSetPoint(Action<OneMeasure> reportTo);
        void SetSetPoint(OneMeasure value);
    }
}
