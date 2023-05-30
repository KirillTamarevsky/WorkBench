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
    public partial class CPC6000Channel : IInstrumentChannel
    {
        public IInstrumentChannelSpan[] AvailableSpans { get; }

        internal CPC6000ChannelNumber ChannelNumber;
        public CPC6000Channel(CPC6000 _parent, CPC6000ChannelNumber channum)
        {
            parent = _parent;
            ChannelNumber = channum;
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

        public int NUM { get ; protected internal set ; }

        public override string ToString()
        {
            return Name;
        }
    }
}
