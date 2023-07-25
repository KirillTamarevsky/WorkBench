using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using WorkBench.Communicators;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;
using WorkBench.UOMS.Pressure;

namespace WorkBench.TestEquipment.CPC6000
{
    public abstract class CPC6000Channel : IInstrumentChannel
    {
        internal CPC6000 parentCPC6000 { get; }
        internal ITextCommunicator Communicator { get => parentCPC6000.Communicator; }
        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer) => parentCPC6000.Query(cmd, out answer);
        public IInstrumentChannelSpan[] AvailableSpans { get; }
        private CPC6000ChannelSpan ActiveSpan { get; set; }
        internal IUOM ActiveUOM { get; private set; }
        private OneMeasure thisChannelRangeMin { get; }
        private OneMeasure thisChannelRangeMax { get; }

        public abstract CPC6000ChannelNumber ChannelNumber { get; }
        public CPC6000Channel(CPC6000 _parent)
        {
            parentCPC6000 = _parent;
            parentCPC6000.SetActiveChannel(ChannelNumber);
            SetPUnit(new kPa());
            Communicator.SendLine("Outform 1");
            List<CPC6000ChannelSpan> availableSpans = new List<CPC6000ChannelSpan>();
            foreach (var pressureType in new List<PressureType>() { PressureType.Absolute, PressureType.Gauge })
            {
                var command = pressureType switch { PressureType.Gauge => "Ptype G", PressureType.Absolute => "Ptype A" };
                var expectedResponse = pressureType switch { PressureType.Gauge => "GAUGE", PressureType.Absolute => "ABSOLUTE" };
                Communicator.SendLine(command);
                var answerStatus = Query("Ptype?", out string answer);
                if (answer.Trim().ToUpper() == expectedResponse)
                {

                    var replyStatus = Query("List?", out string existingTurnDowns);
                    existingTurnDowns = existingTurnDowns.Trim();
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
                        if (modType == CPC6000PressureModule.Primary || modType == CPC6000PressureModule.Secondary)
                        {
                            foreach (var turdown in mod_turndowns.Skip(1))
                            {
                                var cpc6000channelspan = new CPC6000ChannelSpan(this, modType, int.Parse(turdown), pressureType);
                                availableSpans.Add(cpc6000channelspan);
                            }
                        }
                    }
                }
            }
            AvailableSpans = availableSpans.ToArray();

            thisChannelRangeMin = availableSpans.OrderBy(sp => sp.RangeMin.Value).First().RangeMin;
            thisChannelRangeMax = availableSpans.OrderByDescending(sp => sp.RangeMax.Value).First().RangeMax;
        }

        public string Name
        {
            get
            {
                thisChannelRangeMin.TryConvertTo(ActiveUOM, out OneMeasure rngmin);
                thisChannelRangeMax.TryConvertTo(ActiveUOM, out OneMeasure rngmax);

                return $"{parentCPC6000.Description} {parentCPC6000.Name} канал {NUM} {rngmin.Value}...{rngmax.Value} {ActiveUOM.Name}";
            }
        }
        internal abstract string readPressureCommand { get; }

        public int NUM { get => (int)ChannelNumber ; }
        public override string ToString() => Name;

        internal void SetActiveTurndown(CPC6000ChannelSpan cPC6000ChannelSpan)
        {
            if (cPC6000ChannelSpan == null) throw new Exception();
            if (cPC6000ChannelSpan.parentChannel != this) throw new Exception("this span is not mine!");
            parentCPC6000.SetActiveChannel(this);
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
                switch (cPC6000ChannelSpan.PressureType)
                {
                    case PressureType.Absolute:
                        Communicator.SendLine("Ptype A");
                        break;
                    case PressureType.Gauge:
                        Communicator.SendLine("Ptype G");
                        break;
                    default:
                        break;
                }
                ActiveSpan = cPC6000ChannelSpan;
            }
        }
        internal IUOM GetPUnit()
        {
            IUOM readedUOM = null;
            var replyStatus = Query("Units?", out string unit);
            if (replyStatus == TextCommunicatorQueryCommandStatus.Success)
            {
                unit = unit.ToUpper();
                Func<string, double> doubleParser = (s) => double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
                switch (unit)
                {
                    case "PSI": readedUOM = new customPressureUOM("psi", doubleParser("6.894757E+03")); break;
                    case "INHG 0C": readedUOM = new customPressureUOM("inHg @0C", doubleParser("3.386390E+03")); break;
                    case "INHG 60F": readedUOM = new customPressureUOM("inHg @60F", doubleParser("3.376850E+03")); break;
                    case "INH2O 4C": readedUOM = new customPressureUOM("inH2O @4C", doubleParser("2.490820E+02")); break;
                    case "INH2O 20C": readedUOM = new customPressureUOM("inH2O @20C", doubleParser("2.486410E+02")); break;
                    case "INH2O 60F": readedUOM = new customPressureUOM("inH2O @60F", doubleParser("2.488400E+02")); break;
                    case "FTH2O 4C": readedUOM = new customPressureUOM("ftH2O @4C", doubleParser("2.988980E+03")); break;
                    case "FTH2O 20C": readedUOM = new customPressureUOM("ftH2O @20C", doubleParser("2.983692E+03")); break;
                    case "FTH2O 60F": readedUOM = new customPressureUOM("ftH2O @60F", doubleParser("2.986080E+03")); break;
                    case "MTORR": readedUOM = new customPressureUOM("mTorr", doubleParser("1.333220E-01")); break;
                    case "INSW": readedUOM = new customPressureUOM("inSW @0C 3.5% salinity", doubleParser("2.560885E+02")); break;
                    case "FTSW": readedUOM = new customPressureUOM("ftSW @0C 3.5% salinity", doubleParser("3.073062E+03")); break;
                    case "ATM": readedUOM = new customPressureUOM("atm", doubleParser("1.013250E+05")); break;
                    case "BAR": readedUOM = new customPressureUOM("bar", doubleParser("1.00000E+05")); break;
                    case "MBAR": readedUOM = new customPressureUOM("mbar", doubleParser("1.00000E+02")); break;
                    case "MMH2O 4C": readedUOM = new customPressureUOM("mmH2O @4C", doubleParser("9.806378E+00")); break;
                    case "CMH2O 4C": readedUOM = new customPressureUOM("cmH2O @4C", doubleParser("9.806378E+01")); break;
                    case "MH2O 4C": readedUOM = new customPressureUOM("mH2O @4C", doubleParser("9.806378E+03")); break;
                    case "MMHG 0C": readedUOM = new customPressureUOM("mmHg @0C", doubleParser("1.333220E+02")); break;
                    case "CMHG 0C": readedUOM = new customPressureUOM("cmHg @0C", doubleParser("1.333220E+03")); break;
                    case "TORR": readedUOM = new customPressureUOM("Torr", doubleParser("1.333220E+02")); break;
                    case "KPA": readedUOM = new customPressureUOM("kPa", doubleParser("1.00000E+03")); break;
                    case "PASCAL": readedUOM = new customPressureUOM("Pa", doubleParser("1.00000E+00")); break;
                    case "DY/CM2": readedUOM = new customPressureUOM("dyn/sq cm", doubleParser("1.00000E-01")); break;
                    case "GM/CM2": readedUOM = new customPressureUOM("g/sq cm", doubleParser("9.806647E+01")); break;
                    case "KG/CM2": readedUOM = new customPressureUOM("kg/sq cm", doubleParser("9.806647E+04")); break;
                    case "MSW": readedUOM = new customPressureUOM("mSW @0C 3.5% salinity", doubleParser("1.008222E+04")); break;
                    case "OSI": readedUOM = new customPressureUOM("oz/si", doubleParser("4.309223E+02")); break;
                    case "PSF": readedUOM = new customPressureUOM("psf", doubleParser("4.788025E+01")); break;
                    case "TSF": readedUOM = new customPressureUOM("tons/sq ft", doubleParser("9.576052E+04")); break;
                    case "UHG 0C": readedUOM = new customPressureUOM("micronHg @0C", doubleParser("1.333220E-01")); break;
                    case "TSI": readedUOM = new customPressureUOM("tons/sq in", doubleParser("1.378951E+07")); break;
                    case "HPA": readedUOM = new customPressureUOM("hPa", doubleParser("1.00000E+02")); break;
                    case "MPA": readedUOM = new customPressureUOM("MPa", doubleParser("1.00000E+06")); break;
                    case "MMH2O 20C": readedUOM = new customPressureUOM("mmH2O @20C", doubleParser("9.789017E+00")); break;
                    case "CMH2O 20C": readedUOM = new customPressureUOM("cmH2O @20C", doubleParser("9.789017E+01")); break;
                    case "MH2O 20C": readedUOM = new customPressureUOM("mH2O @20C", doubleParser("9.789017E+03")); break;
                }
                ActiveUOM = readedUOM;
            }
            return readedUOM;
        }
        internal void SetPUnit(IUOM targetUOM)
        {
            var cpc6000UnitCode = targetUOM.Name;
            if (targetUOM.GetType() == typeof(Kgfcmsq))
            {
                cpc6000UnitCode = "KG/CM2";
            }
            if (targetUOM.GetType() == typeof(mmH2OAt4DegreesCelsius))
            {
                cpc6000UnitCode = "MMH2O 4C";
            }
            Communicator.SendLine($"Units {cpc6000UnitCode}");
            ActiveUOM = targetUOM;
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
