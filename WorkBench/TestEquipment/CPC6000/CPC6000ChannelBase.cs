using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.CPC6000
{
    public abstract class CPC6000Channel : IInstrumentChannel
    {
        internal CPC6000 parent { get; }
        internal ITextCommunicator Communicator { get => parent.Communicator; }
        private string Query(string cmd) => parent.Query(cmd);
        public IInstrumentChannelSpan[] AvailableSpans { get; }
        private CPC6000ChannelSpan ActiveSpan { get; set; }

        public abstract CPC6000ChannelNumber ChannelNumber { get; }
        public CPC6000Channel(CPC6000 _parent)
        {
            parent = _parent;
            parent.SetActiveChannel(ChannelNumber);
            List<IInstrumentChannelSpan> availableSpans = new List<IInstrumentChannelSpan>();
            Communicator.SendLine("Ptype A");
            if (Query("Ptype?").Trim().ToUpper() == "ABSOLUTE")
            {
                //get turndowns for current channel
                //List? => [PRI,1;SEC,1;BAR,1]
                var existingTurnDowns = Query("List?").Trim();
                var modules = existingTurnDowns.Split(";");
                foreach (var module in modules)
                {
                    var mod_turndowns = module.Split(",");
                    var modType = mod_turndowns[0].Trim().ToUpper() switch
                    {
                        "PRI" => CPC6000PressureModule.Primary,
                        "SEC" => CPC6000PressureModule.Secondary,
                        "BAR" => CPC6000PressureModule.Barometer,
                        _ => throw new ArgumentException(),
                    };
                    foreach (var turdown in mod_turndowns.Skip(1))
                    {
                        var cpc6000channelspan = new CPC6000ChannelSpan(this, modType, int.Parse(turdown), PressureType.Absolute);
                        availableSpans.Add(cpc6000channelspan);
                    }
                }
            }
            AvailableSpans = availableSpans.ToArray();
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

        public int NUM { get => (int)ChannelNumber ; }
        public override string ToString() => Name;

        internal void SetActiveTurndown(CPC6000ChannelSpan cPC6000ChannelSpan)
        {
            if (cPC6000ChannelSpan == null) throw new Exception();
            if (!AvailableSpans.Any(sp => sp == cPC6000ChannelSpan)) throw new Exception("this span is not mine!");
            parent.SetActiveChannel(this);
            if (!(ActiveSpan == cPC6000ChannelSpan))
            {

                switch (cPC6000ChannelSpan.module)
                {
                    case CPC6000PressureModule.Primary:
                        Communicator.SendLine($"Sensor P,{cPC6000ChannelSpan.turndown}");
                        break;
                    case CPC6000PressureModule.Secondary:
                        Communicator.SendLine($"Sensor S,{cPC6000ChannelSpan.turndown}");
                        break;
                }
                ActiveSpan = cPC6000ChannelSpan;
            }
        }
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
