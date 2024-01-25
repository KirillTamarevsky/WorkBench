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
    public class CPC6000Channel : IInstrumentChannel
    {
        internal CPC6000 parentCPC6000 { get; }
        public CPC6000ChannelNumber ChannelNumber { get; }
        public IInstrumentChannelSpan[] AvailableSpans { get; }
        
        internal ITextCommunicator Communicator { get => parentCPC6000.Communicator; }
        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer) => parentCPC6000.Query(cmd, out answer);
        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer, Func<string, bool> validationRule) => parentCPC6000.Query(cmd, out answer, validationRule);
        
        private CPC6000PressureModule? ActivePressureModule { get; set; }
        private int? ActiveTurnDownNumber { get; set; }
        private PressureType? ActivePressureType { get; set; }
        internal IUOM ActiveUOM { get; private set; }

        private OneMeasure thisChannelRangeMin { get; }
        private OneMeasure thisChannelRangeMax { get; }

        public CPC6000Channel(CPC6000 _parent, CPC6000ChannelNumber chanNumber)
        {
            parentCPC6000 = _parent;
            ChannelNumber = chanNumber;
            parentCPC6000.SetActiveChannel(chanNumber);
            //SetPUnit(new kPa());
            parentCPC6000.Communicator.SendLine("Outform 1");
            List<CPC6000ChannelTurnDown> availableSpans = new List<CPC6000ChannelTurnDown>();
            foreach (var pressureType in new List<PressureType>() { PressureType.Absolute, PressureType.Gauge })
            {
                SetActivePressureType(pressureType);
                var activePressureType = GetActivePressureType();

                if (activePressureType == pressureType)
                {
                    Func<string, bool> validationRule = (s) =>
                    {
                        return s.ToUpper().Contains("PRI");
                    };

                    var replyStatus = Query("List?", out string existingTurnDowns, validationRule);
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
                                var turnDownNumber = int.Parse(turdown.Trim());

                                var cpc6000channelspan = Get_new_CPC6000ChannelSpan(modType, turnDownNumber, pressureType);

                                if (cpc6000channelspan != null)
                                {
                                    availableSpans.Add(cpc6000channelspan);
                                }
                            }
                        }
                    }
                }
            }
            AvailableSpans = availableSpans.ToArray();

            thisChannelRangeMin = availableSpans.OrderBy(sp => sp.RangeMin.Value).First().RangeMin;
            thisChannelRangeMax = availableSpans.OrderByDescending(sp => sp.RangeMax.Value).First().RangeMax;
        }
        internal static CPC6000Channel GetCPC6000Channel(CPC6000 _parentCPC6000, CPC6000ChannelNumber chanNumber)
        {
            try
            {
                var cpc6000Channel = new CPC6000Channel(_parentCPC6000, chanNumber);
                return cpc6000Channel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private CPC6000ChannelTurnDown Get_new_CPC6000ChannelSpan(CPC6000PressureModule modType, int turnDownNumber, PressureType pressureType)
        {
            SetActiveTurndown(modType, turnDownNumber, pressureType);
            var unit = GetPUnit();
            if (unit == null)
            {
                return null;
            }
            Func<string, bool> floatValidationRule = (s) => double.TryParse(s.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out double _);

            var answerStatus = Query("RangeMin?", out string answer, floatValidationRule);
            if (answerStatus != TextCommunicatorQueryCommandStatus.Success)
            {
                return null;
            }
            var RangeMin = new OneMeasure(double.Parse(answer.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture), unit);

            answerStatus = Query("RangeMax?", out answer, floatValidationRule);
            if (answerStatus != TextCommunicatorQueryCommandStatus.Success)
            {
                return null;
            }
            var RangeMax = new OneMeasure(double.Parse(answer.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture), unit);

            return new CPC6000ChannelTurnDown(this, modType, turnDownNumber, pressureType, RangeMin, RangeMax);
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


        public int NUM { get => (int)ChannelNumber; }
        public override string ToString() => Name;

        internal void SetActiveTurndown(CPC6000ChannelTurnDown td)
        {
            if (td == null || td.parentChannel != this)
            {
                return;
            }

            if ( ActivePressureModule == null || ActivePressureModule != td.module || ActiveTurnDownNumber == null || ActiveTurnDownNumber != td.turndown)
            {
                SetActiveTurndown(td.module, td.turndown, td.PressureType);
            }
            
        }
        internal void SetActiveTurndownForced(CPC6000ChannelTurnDown td)
        {
            if (td == null || td.parentChannel != this)
            {
                return;
            }

            SetActiveTurndown(td.module, td.turndown, td.PressureType);

        }
        private void SetActiveTurndown(CPC6000PressureModule pressureModule, int turnDownNumber, PressureType pressureType)
        {
            parentCPC6000.SetActiveChannel(ChannelNumber);

            switch (pressureModule)
            {
                case CPC6000PressureModule.Primary:
                    Communicator.SendLine($"Sensor P,{turnDownNumber}");
                    break;
                case CPC6000PressureModule.Secondary:
                    Communicator.SendLine($"Sensor S,{turnDownNumber}");
                    break;
            }
            ActivePressureModule = pressureModule;
            ActiveTurnDownNumber = turnDownNumber;

            SetActivePressureType(pressureType);
        }

        internal PressureType GetActivePressureType()
        {
            parentCPC6000.SetActiveChannel(ChannelNumber);

            PressureType res = PressureType.Unknown;

            var validationRule = (string s) => s.Trim().ToUpper().Contains("GAUGE") | s.Trim().ToUpper().Contains("ABSOLUTE");
            var answerStatus = Query("Ptype?", out string answer, validationRule);
            if (answerStatus == TextCommunicatorQueryCommandStatus.Success)
            {
                switch (answer.Trim().ToUpper())
                {
                    case "GAUGE":
                        res = PressureType.Gauge;
                        break;
                    case "ABSOLUTE":
                        res = PressureType.Absolute;
                        break;
                }
            }
            return res;
        }
        internal void SetActivePressureType(PressureType pressureType)
        {
            parentCPC6000.SetActiveChannel(ChannelNumber);

            if (ActivePressureType == null || ActivePressureType != pressureType)
            {
                switch (pressureType)
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

                ActivePressureType = pressureType;
            }
        }
        internal IUOM GetPUnit()
        {
            parentCPC6000.SetActiveChannel(ChannelNumber);

            IUOM readedUOM = null;
            List<string> possibleUnits = new List<string>()
            {
            "PSI", "INHG 0C", "INHG 60F", "INH2O 4C", "INH2O 20C", "INH2O 60F", "FTH2O 4C", "FTH2O 20C", "FTH2O 60F", "MTORR",
            "INSW", "FTSW", "ATM", "BAR", "MBAR", "MMH2O 4C", "CMH2O 4C", "MH2O 4C", "MMHG 0C", "CMHG 0C","TORR", "KPA", "PASCAL",
            "DY/CM2", "GM/CM2", "KG/CM2", "MSW", "OSI", "PSF", "TSF", "UHG 0C", "TSI", "HPA", "MPA", "MMH2O 20C", "CMH2O 20C", "MH2O 20C",
            };
            var replyStatus = Query("Units?", out string unit, s => possibleUnits.Any( u => s.ToUpper().Contains(u)));
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
            parentCPC6000.SetActiveChannel(ChannelNumber);

            var cpc6000UnitCode = targetUOM.Name;
            if (targetUOM.GetType() == typeof(Kgfcmsq))
            {
                cpc6000UnitCode = "KG/CM2";
            }
            if (targetUOM.GetType() == typeof(mmH2OAt4DegreesCelsius))
            {
                cpc6000UnitCode = "MMH2O 4C";
            }
            if (targetUOM.Name.ToUpper().Trim() == "PA")
            {
                cpc6000UnitCode = "PASCAL";
            }
            Communicator.SendLine($"Units {cpc6000UnitCode}");
            ActiveUOM = targetUOM;
        }
    }
}
