using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK.commands;

namespace WorkBench.TestEquipment.EK.channelSpans
{
    public class EKChannelSpan_0_20_mA : AbstractInstrumentChannelSpanReader
    {
        public override void Activate()
        {
            parentChannel.ActiveSpan = this;
        }

        public override void Read(IUOM uom, Action<OneMeasure> reportTo)
        {
            var cmd = new EKCommand_Read_0_20_mA(this, reportTo);
            
            EnqueueInstrumentCmd(cmd);

        }

        public override string ToString()
        {
            return $"измерение тока {Scale}, внешний источник питания";
        }

        public override void Zero()
        {
            throw new NotImplementedException("EK channel 0 - 20 mA zeriong not implemented");
        }
    }
}
