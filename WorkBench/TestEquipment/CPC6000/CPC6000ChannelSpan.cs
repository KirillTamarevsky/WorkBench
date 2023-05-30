using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan;
using WorkBench.Enums;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.CPC6000
{
    internal class CPC6000ChannelSpan : IInstrumentChannelSpanPressureGenerator, IInstrumentChannelSpanReader
    {
        private CPC6000Channel parentChannel { get; }
        private Scale Scale { get; }
        public CPC6000ChannelSpan(CPC6000Channel _parentChannel, Scale _scale)
        {
            parentChannel = _parentChannel;

            Scale = _scale;
        }

        public void Zero()
        {
            parentChannel.parent.Communicator.SendLine("Autozero");
        }

        public void GetPressureOperationMode(Action<PressureControllerOperationMode> reportTo)
        {
            EnqueueInstrumentCmd(new CPC6000Command_GetOperationMode(reportTo));
        }
        public void SetPressureOperationMode(PressureControllerOperationMode value)
        {
            var cmd = new CPC6000Command_SetOperationMode(value);
            EnqueueInstrumentCmd(cmd);
        }

        public override string ToString()
        {
            return $"{parentChannel} - {Scale}";
        }
    }
}
