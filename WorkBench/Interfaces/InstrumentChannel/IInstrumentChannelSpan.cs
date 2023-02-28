using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces.InstrumentChannel
{
    public interface IInstrumentChannelSpan
    {
        Scale Scale { get; }

        //IInstrumentChannel parentChannel { get; }

        //event InstrumentChannelSpanSelectedAsActiveDelegate InstrumentChannelSpanSelectedAsActive;

        void Activate();

        void Zero();
    }

    //public delegate void InstrumentChannelSpanSelectedAsActiveDelegate(IInstrumentChannelSpan instrumentChannelSpan);
}
