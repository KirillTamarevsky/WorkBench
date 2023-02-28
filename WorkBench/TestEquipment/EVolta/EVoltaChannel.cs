using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.Instrument;
using WorkBench.AbstractClasses.InstrumentChannel;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.TestEquipment.EVolta.channelSpans;

namespace WorkBench.TestEquipment.EVolta
{
    public class EVoltaReaderChannel : AbstractInstrumentChannel
    {
        public override int NUM 
        {
            get { return 1; }
            protected internal set { } 
        }
        public override string Name { get => "Канал измерений"; protected internal set { } }
        public override AbstractInstrument parent { get; protected internal set; }

        public EVoltaReaderChannel()
        {
            AvailableSpans = new IInstrumentChannelSpan[] { new EVoltaPassiveCurrentReaderSpan() { parentChannel = this } };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
