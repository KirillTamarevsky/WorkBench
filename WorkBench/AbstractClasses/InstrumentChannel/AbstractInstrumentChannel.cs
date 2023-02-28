using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.Instrument;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.InstrumentChannel
{
    public abstract class AbstractInstrumentChannel : IInstrumentChannel
    {
        public abstract int NUM { get; protected internal set; }
        public abstract string Name { get; protected internal set; }
        public abstract AbstractInstrument parent { get; protected internal set; }
        public virtual IInstrumentChannelSpan[] AvailableSpans { get; protected internal set; }
        public IInstrumentChannelSpan ActiveSpan { get; internal set; }

        internal virtual void EnqueueInstrumentCmd(InstrumentCmd instrumentCmd)
        {
            parent.EnqueueInstrumentCmd(instrumentCmd);
        }

    }

}

