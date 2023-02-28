using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan
{
    public abstract class AbstractInstrumentChannelSpan : IInstrumentChannelSpan
    {        
        public Scale Scale { get; internal set; }
        public AbstractInstrumentChannel parentChannel { get; internal set; }

        internal virtual void EnqueueInstrumentCmd(InstrumentCmd instrumentCmd)
        {
            parentChannel.EnqueueInstrumentCmd(instrumentCmd);
        }


        //public event InstrumentChannelSpanSelectedAsActiveDelegate InstrumentChannelSpanSelectedAsActive;

        //internal void RaiseInstrumentChannelSpanSelectedAsActive()
        //{
        //    if (InstrumentChannelSpanSelectedAsActive != null)
        //    {
        //        InstrumentChannelSpanSelectedAsActive(this);
        //    }
        //}

        public abstract void Activate();
        public abstract void Zero();
    }
}
