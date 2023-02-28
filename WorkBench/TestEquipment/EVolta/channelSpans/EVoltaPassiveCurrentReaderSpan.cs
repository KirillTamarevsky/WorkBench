using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK.commands;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EVolta.channelSpans
{
    public class EVoltaPassiveCurrentReaderSpan : AbstractInstrumentChannelSpanReader
    {
        public EVoltaPassiveCurrentReaderSpan()
        {
            Scale = new Scale(0, 20, new mA());
        }
        public override void Activate()
        {
            parentChannel.ActiveSpan = this;
        }

        public override void Read(IUOM uom, Action<OneMeasure> reportTo)
        {
            EnqueueInstrumentCmd(new EVoltaReadPassiveCurrentCommand(reportTo));
        }

        public override void Zero()
        {
            
        }

        public override string ToString()
        {
            return $"Измерение тока {Scale}, внешний источник питания";
        }

    }
}
