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
    internal class CPC6000ChannelSpan : AbstractInstrumentChannelSpanReader_and_Generator, IInstrumentChannelSpanPressureGenerator
    {
        public CPC6000ChannelSpan(CPC6000Channel _parentChannel, Scale _scale)
        {
            parentChannel = _parentChannel;

            Scale = _scale;
        }
        public override void GetSetPoint(Action<OneMeasure> reportTo)
        {
            EnqueueInstrumentCmd(new CPC6000Command_GetSetPoint(reportTo));
        }
        public override void SetSetPoint(OneMeasure value)
        {
            EnqueueInstrumentCmd(new CPC6000Command_SetSetPoint(value));
        }

        public override void Activate()
        {
            parentChannel.ActiveSpan = this;
        }

        public override void Read(IUOM uom, Action<OneMeasure> reportTo)
        {
                if (uom.UOMType != UOMType.Pressure) throw new ArgumentException($"wrong UOM! {uom.UOMType} is not {UOMType.Pressure}");

                var cmd = new CPC6000Command_Get_Actual_Pressure_on_channel(uom, reportTo);

                EnqueueInstrumentCmd(cmd);

        }

        public override void Zero()
        {
            var cmd = new cpc6000Command_AutoZero();
            EnqueueInstrumentCmd(cmd);
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
