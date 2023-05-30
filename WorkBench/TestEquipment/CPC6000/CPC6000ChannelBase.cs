using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.CPC6000
{
    public abstract class CPC6000Channel : IInstrumentChannel
    {
        public IInstrumentChannelSpan[] AvailableSpans { get; }

        public abstract CPC6000ChannelNumber ChannelNumber { get; }
        public CPC6000Channel(CPC6000 _parent)
        {
            parent = _parent;
            var _scale = _parent.GetActualScaleOnChannel(ChannelNumber);
            AvailableSpans = new IInstrumentChannelSpan[] { new CPC6000ChannelSpan(this, _scale) };
        }

        List<Scale> supportedMeasureTypes
        {
            get
            {
                //Scale scale = ((CPC6000)parent).GetActualScaleOnChannel((CPC6000ChannelNumber)NUM);

                //return new List<Scale>() { scale };
                return new List<Scale>(AvailableSpans.Select(span => span.Scale));
            }
        }

        internal CPC6000 parent { get; }
        public string Name
        {
            get
            {
                return $"{CPC6000.Description} {CPC6000.Name} канал {NUM} {supportedMeasureTypes.FirstOrDefault()}";
            }
        }

        public int NUM { get => (int)ChannelNumber ; }

        public override string ToString() => Name;
    }

    public class CPC6000Channel_A : CPC6000Channel
    {
        public CPC6000Channel_A(CPC6000 parent) : base(parent){}
        public override CPC6000ChannelNumber ChannelNumber => CPC6000ChannelNumber.A;

    }
    public class CPC6000Channel_B : CPC6000Channel
    {
        public CPC6000Channel_B(CPC6000 parent) : base(parent) { }
        public override CPC6000ChannelNumber ChannelNumber => CPC6000ChannelNumber.B;

    }
}
