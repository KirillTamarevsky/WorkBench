using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces.InstrumentChannel
{
    public interface IInstrumentChannelSpanGenerator : IInstrumentChannelSpan
    {
        OneMeasure SetPoint { get; set; }
    }
}
