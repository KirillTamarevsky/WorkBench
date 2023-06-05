using System;
using System.Collections.Generic;
using System.Globalization;
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
        internal CPC6000 parent { get; }
        internal ITextCommunicator Communicator { get => parent.Communicator; }
        public IInstrumentChannelSpan[] AvailableSpans { get; }
        private CPC6000ChannelSpan ActiveSpan { get; set; }

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

        public string Name => $"{CPC6000.Description} {CPC6000.Name} канал {NUM} {supportedMeasureTypes.FirstOrDefault()}";
        internal abstract string readPressureCommand { get; }
        internal OneMeasure ReadPressure()
        {
            parent.Communicator.SendLine("Outform 1");
            var reply = parent.Communicator.QueryCommand(readPressureCommand).Trim();
            var pressureValue = double.Parse(reply, NumberStyles.Float, CultureInfo.InvariantCulture);
            var unit = parent.GetPUnit();
            return new OneMeasure(pressureValue, unit);
        }
        public int NUM { get => (int)ChannelNumber ; }
        public override string ToString() => Name;
    }

    public class CPC6000Channel_A : CPC6000Channel
    {
        public CPC6000Channel_A(CPC6000 parent) : base(parent){}
        public override CPC6000ChannelNumber ChannelNumber => CPC6000ChannelNumber.A;
        internal override string readPressureCommand => "A?";
    }
    public class CPC6000Channel_B : CPC6000Channel
    {
        public CPC6000Channel_B(CPC6000 parent) : base(parent) { }
        public override CPC6000ChannelNumber ChannelNumber => CPC6000ChannelNumber.B;
        internal override string readPressureCommand => "B?";

    }
}
