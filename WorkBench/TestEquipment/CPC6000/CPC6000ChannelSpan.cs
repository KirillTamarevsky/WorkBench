using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Enums;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.CPC6000
{
    internal class CPC6000ChannelSpan : IInstrumentChannelSpanPressureGenerator, IInstrumentChannelSpanReader
    {
        private CPC6000Channel parentChannel { get; }
        public Scale Scale { get; }
        public CPC6000ChannelSpan(CPC6000Channel _parentChannel, Scale _scale)
        {
            parentChannel = _parentChannel;

            Scale = _scale;
        }
        public PressureControllerOperationMode PressureOperationMode
        {
            get => parentChannel.parent.GetOperationMode();
            set => parentChannel.parent.SetOperationMode(value);
        }
        public OneMeasure SetPoint 
        { 
            get => parentChannel.parent.GetSetPoint(); 
            set => parentChannel.parent.SetSetPoint(value); 
        }


        public void Zero()
        {
            parentChannel.parent.Communicator.SendLine("Autozero");
        }

        public override string ToString()
        {
            return $"{parentChannel} - {Scale}";
        }

        public OneMeasure Read(IUOM uom)
        {
            throw new NotImplementedException(); 
        }
    }
}
