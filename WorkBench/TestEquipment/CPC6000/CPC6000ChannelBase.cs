using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.AbstractClasses.InstrumentChannel;
using WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan;
using WorkBench.AbstractClasses.Instrument;

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000Channel : AbstractInstrumentChannel
    {
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

        UOMType lastReadType = UOMType.Pressure;

        CPC6000 _parent;
        public override AbstractInstrument parent 
        {
            get => _parent;

            protected internal set
            {
                if (!(value is CPC6000 p)) throw new ArgumentException($"{value.GetType()} is not {typeof(CPC6000)}");

                _parent = p;
            }
        }
        public override string Name
        {
            get
            {
                return $"{parent.Description} {parent.Name} канал {NUM} {supportedMeasureTypes.FirstOrDefault()}";
            }
            protected internal set { }
        }

        internal override void EnqueueInstrumentCmd(InstrumentCmd instrumentCmd)
        {
            if (!(instrumentCmd is CPC6000CommandBase cmd)) throw new ArgumentException($"{instrumentCmd.GetType()} не является {typeof(CPC6000CommandBase)}");

            cmd.ChannelNumber = ChannelNumber;

            base.EnqueueInstrumentCmd(instrumentCmd);
        }
        public override int NUM { get ; protected internal set ; }

        public override string ToString()
        {
            return Name;
        }
    }
}
