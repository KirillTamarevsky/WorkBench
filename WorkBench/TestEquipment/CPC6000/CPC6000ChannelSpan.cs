using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Enums;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Globalization;
using log4net.Util;

namespace WorkBench.TestEquipment.CPC6000
{
    internal class CPC6000ChannelSpan : IInstrumentChannelSpanPressureGenerator, IInstrumentChannelSpanReader
    {
        private CPC6000Channel parentChannel { get; }
        private ITextCommunicator Communicator => parentChannel.Communicator;
        internal CPC6000PressureModule module { get; }
        private bool IsOpen => parentChannel.parent.IsOpen;
        private string Query(string cmd) => parentChannel.parent.Query(cmd);
        internal int turndown { get; }
        public Scale Scale { get
            {
                lock (Communicator)
                {

                    parentChannel.SetActiveTurndown(this);
                    var unit = GetPUnit();
                    var minRange = double.Parse(Query("RangeMin?").Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);
                    var maxRange = double.Parse(Query("RangeMax?").Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);
                    return new Scale(minRange, maxRange, unit);
                }
            }
        }
        public CPC6000ChannelSpan(CPC6000Channel _parentChannel, CPC6000PressureModule _module, int _turndown)
        {
            parentChannel = _parentChannel;
            module = _module;
            turndown = _turndown;
        }
        public PressureControllerOperationMode PressureOperationMode
        {
            get
            {
                PressureControllerOperationMode mode = PressureControllerOperationMode.UNKNOWN;
                if (IsOpen)
                {
                    lock (Communicator)
                    {

                        parentChannel.SetActiveTurndown(this);
                        string answer = Query("Mode?");
                        switch (answer.ToUpper())
                        {
                            case "STANDBY":
                                mode = PressureControllerOperationMode.STANDBY;
                                break;
                            case "MEASURE":
                                mode = PressureControllerOperationMode.MEASURE;
                                break;
                            case "CONTROL":
                                mode = PressureControllerOperationMode.CONTROL;
                                break;
                            case "VENT":
                                mode = PressureControllerOperationMode.VENT;
                                break;
                        }
                    }
                }
                return mode;
            }
            set
            {
                if (IsOpen)
                {
                    lock (Communicator)
                    {

                        parentChannel.SetActiveTurndown(this);
                        switch (value)
                        {
                            case PressureControllerOperationMode.STANDBY:
                                Communicator.SendLine("Mode STANDBY");
                                break;
                            case PressureControllerOperationMode.MEASURE:
                                Communicator.SendLine("Mode MEASURE");
                                break;
                            case PressureControllerOperationMode.CONTROL:
                                Communicator.SendLine("Mode CONTROL");
                                break;
                            case PressureControllerOperationMode.VENT:
                                Communicator.SendLine("Mode VENT");
                                break;
                            default:
                                log4net.LogManager.GetLogger("CPC6000Communication").Debug(
                                    string.Format("CPC6000 OperationMode( {0} ) - invalid operation mode", Communicator.ToString()));
                                throw new Exception("CPC6000 OperationMode() - invalid operation mode");
                        }
                    }
                }
            }
        }
        public OneMeasure SetPoint
        {
            get
            {
                lock (Communicator)
                {

                    parentChannel.SetActiveTurndown(this);

                    double setpoint = double.NaN;
                    string answer = Query("Setpt?").Replace(',', '.');
                    double.TryParse(answer,
                                     NumberStyles.Float,
                                     CultureInfo.InvariantCulture,
                                     out setpoint);
                    return new OneMeasure(setpoint, GetPUnit());
                }
            }
            set
            {
                lock (Communicator)
                {

                    parentChannel.SetActiveTurndown(this);

                    var setpoint_str = value.Value.ToString("E04", CultureInfo.InvariantCulture);
                    Communicator.SendLine($"Setpt {setpoint_str}");
                }
            }
        }


        public void Zero()
        {
            lock (Communicator)
            {

                parentChannel.SetActiveTurndown(this);

                Communicator.SendLine("Autozero");
            }
        }

        public override string ToString()
        {
            return $"{parentChannel} - {Scale}";
        }

        public OneMeasure Read(IUOM uom)
        {
            lock (Communicator)
            {

                if (uom.UOMType != UOMType.Pressure) throw new Exception($"not possible to read uom type {uom.Name} ");

                parentChannel.SetActiveTurndown(this);
                Communicator.SendLine("Outform 1");
                var reply = Communicator.QueryCommand(parentChannel.readPressureCommand).Trim().Replace(",", ".");
                var pressureValue = double.Parse(reply, NumberStyles.Float, CultureInfo.InvariantCulture);
                var unit = GetPUnit();
                return new OneMeasure(pressureValue, unit);
            }
        }
        private IUOM GetPUnit()
        {
            var unit = Query("Units?").ToUpper();
            Func<string, double> doubleParser = (s) => double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
            switch (unit)
            {
                case "PSI": return new customPressureUOM("psi", doubleParser("6.894757E+03"));
                case "INHG @0C": return new customPressureUOM("inHg @0C", doubleParser("3.386390E+03"));
                case "INHG @60F": return new customPressureUOM("inHg @60F", doubleParser("3.376850E+03"));
                case "INH2O @4C": return new customPressureUOM("inH2O @4C", doubleParser("2.490820E+02"));
                case "INH2O @20C": return new customPressureUOM("inH2O @20C", doubleParser("2.486410E+02"));
                case "INH2O @60F": return new customPressureUOM("inH2O @60F", doubleParser("2.488400E+02"));
                case "FTH2O @4C": return new customPressureUOM("ftH2O @4C", doubleParser("2.988980E+03"));
                case "FTH2O @20C": return new customPressureUOM("ftH2O @20C", doubleParser("2.983692E+03"));
                case "FTH2O @60F": return new customPressureUOM("ftH2O @60F", doubleParser("2.986080E+03"));
                case "MTORR": return new customPressureUOM("mTorr", doubleParser("1.333220E-01"));
                case "INSW @0C 3.5% SALINITY": return new customPressureUOM("inSW @0C 3.5% salinity", doubleParser("2.560885E+02"));
                case "FTSW @0C 3.5% SALINITY": return new customPressureUOM("ftSW @0C 3.5% salinity", doubleParser("3.073062E+03"));
                case "ATM": return new customPressureUOM("atm", doubleParser("1.013250E+05"));
                case "BAR": return new customPressureUOM("bar", doubleParser("1.00000E+05"));
                case "MBAR": return new customPressureUOM("mbar", doubleParser("1.00000E+02"));
                case "MMH2O @4C": return new customPressureUOM("mmH2O @4C", doubleParser("9.806378E+00"));
                case "CMH2O @4C": return new customPressureUOM("cmH2O @4C", doubleParser("9.806378E+01"));
                case "MH2O @4C": return new customPressureUOM("mH2O @4C", doubleParser("9.806378E+03"));
                case "MMHG @0C": return new customPressureUOM("mmHg @0C", doubleParser("1.333220E+02"));
                case "CMHG @0C": return new customPressureUOM("cmHg @0C", doubleParser("1.333220E+03"));
                case "TORR": return new customPressureUOM("Torr", doubleParser("1.333220E+02"));
                case "KPA": return new customPressureUOM("kPa", doubleParser("1.00000E+03"));
                case "PA": return new customPressureUOM("Pa", doubleParser("1.00000E+00"));
                case "DYN/SQ CM": return new customPressureUOM("dyn/sq cm", doubleParser("1.00000E-01"));
                case "G/SQ CM": return new customPressureUOM("g/sq cm", doubleParser("9.806647E+01"));
                case "KG/SQ CM": return new customPressureUOM("kg/sq cm", doubleParser("9.806647E+04"));
                case "MSW @0C 3.5% SALINITY": return new customPressureUOM("mSW @0C 3.5% salinity", doubleParser("1.008222E+04"));
                case "OZ/SI": return new customPressureUOM("oz/si", doubleParser("4.309223E+02"));
                case "PSF": return new customPressureUOM("psf", doubleParser("4.788025E+01"));
                case "TONS/SQ FT": return new customPressureUOM("tons/sq ft", doubleParser("9.576052E+04"));
                case "MICRONHG @0C": return new customPressureUOM("micronHg @0C", doubleParser("1.333220E-01"));
                case "TONS/SQ IN": return new customPressureUOM("tons/sq in", doubleParser("1.378951E+07"));
                case "HPA": return new customPressureUOM("hPa", doubleParser("1.00000E+02"));
                case "MPA": return new customPressureUOM("MPa", doubleParser("1.00000E+06"));
                case "MMH2O @20C": return new customPressureUOM("mmH2O @20C", doubleParser("9.789017E+00"));
                case "CMH2O @20C": return new customPressureUOM("cmH2O @20C", doubleParser("9.789017E+01"));
                case "MH2O @20C": return new customPressureUOM("mH2O @20C", doubleParser("9.789017E+03"));
                default:
                    throw new Exception($"unknown pressure units [{unit}]");
            }
        }

    }
}
