using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan
{
    public abstract class AbstractInstrumentChannelSpanReader : AbstractInstrumentChannelSpan, IInstrumentChannelSpanReader
    {
        OneMeasure _lastValue;
        public abstract OneMeasure Read(IUOM uom);
    }
}