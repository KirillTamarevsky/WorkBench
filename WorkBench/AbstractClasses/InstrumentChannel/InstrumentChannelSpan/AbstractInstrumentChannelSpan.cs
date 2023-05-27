using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan
{
    public abstract class AbstractInstrumentChannelSpan : IInstrumentChannelSpan
    {        
        public Scale Scale { get; internal set; }
        public virtual IInstrumentChannel parentChannel { get; internal set; }
        public abstract void Zero();
    }
}
