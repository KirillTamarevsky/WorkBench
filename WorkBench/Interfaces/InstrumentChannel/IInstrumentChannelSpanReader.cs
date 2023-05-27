using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.Interfaces.InstrumentChannel
{
    public interface IInstrumentChannelSpanReader : IInstrumentChannelSpan
    {
        OneMeasure Read(IUOM uom);
    }

}
