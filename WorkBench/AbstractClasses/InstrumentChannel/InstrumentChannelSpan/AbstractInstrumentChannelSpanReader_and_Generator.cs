using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan
{
    public abstract class AbstractInstrumentChannelSpanReader_and_Generator : AbstractInstrumentChannelSpanReader, IInstrumentChannelSpanGenerator
    {
        public abstract OneMeasure GetSetPoint();
        public abstract void SetSetPoint(OneMeasure value);
    }
}
