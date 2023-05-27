using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.Instrument;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.InstrumentChannel
{
    public abstract class AbstractInstrumentChannel : IInstrumentChannel
    {
        public abstract int NUM { get; protected internal set; }
        public abstract string Name { get; protected internal set; }
        AbstractInstrument _parentInstrument;
        public virtual IInstrument ParentInstrument { get => _parentInstrument; protected internal set => _parentInstrument = (AbstractInstrument)value; }
        public virtual IInstrumentChannelSpan[] AvailableSpans { get; protected internal set; }
        public IInstrumentChannelSpan ActiveSpan { get; internal set; }
    }

}

