using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.UOMS;
using WorkBench.UOMS.Pressure;

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000
    {

        #region CPC6000 hardware interface commands

        //####################################################
        private string _serialNo;
        public string SerialNo
        {
            get
            {
                return _serialNo;
            }
        }
        private string _swVer;
        public string SwVer
        {
            get
            {
                return _swVer;
            }
        }
        private int _lastErrNo;
        public int LastErrNo
        {
            get
            {
                return _lastErrNo;
            }
        }
        private string _lastErrDesc;
        public string LastErrDesc
        {
            get
            {
                return _lastErrDesc;
            }
        }
        private void GetLastError()
        {
            string answer = Communicator.QueryCommand("Errorno?");
            string[] errorparts = answer.Split(new char[] { '-' });
            if (errorparts.Length == 2)
            {
                int.TryParse(errorparts[0].TrimStart(new char[] { 'E' }), out _lastErrNo);
                _lastErrDesc = errorparts[1];
            }
            log4net.LogManager.GetLogger("CPC6000Communication").Debug($"{_lastErrNo} === {_lastErrDesc}");
            return;
        }
        private void ResetError()
        {
            _lastErrNo = 0;
            _lastErrDesc = "";
        }
        public bool ChanAStable
        {
            get
            {
                bool flag = false;
                if (IsOpen)
                {
                    switch (Query("AS?").ToUpper())
                    {
                        case "YES":
                            flag = true;
                            break;
                        case "NO":
                            flag = false;
                            break;
                        default:
                            throw new Exception("Get Chan A Stable flag error");
                    }
                }
                return flag;
            }
        }
        public double? ChanAReading
        {
            get
            {
                var rawdata = Query("A?").Replace(",", ".");
                if (string.IsNullOrEmpty(rawdata))
                {
                    return null;
                }
                double res;
                double.TryParse(
                    rawdata,
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures),
                    out res);
                return res;
            }
        }
        public bool ChanBStable
        {
            get
            {
                bool flag = false;
                if (IsOpen)
                {
                    switch (Query("BS?").ToUpper())
                    {
                        case "YES":
                            flag = true;
                            break;
                        case "NO":
                            flag = false;
                            break;
                        default:
                            throw new Exception("Get Chan B Stable flag error");
                    }
                }
                return flag;
            }
        }
        public double? ChanBReading
        {
            get
            {
                var rawdata = Query("B?").Replace(",", ".");
                if (string.IsNullOrEmpty(rawdata))
                {
                    return null;
                }
                double res;
                double.TryParse(
                    rawdata,
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures),
                    out res);
                return res;
            }
        }

        public double ChanBaroReading
        {
            get
            {
                return double.Parse(Query("Baro?"), NumberStyles.Float, new CultureInfo((int)CultureTypes.NeutralCultures));
            }
        }

        public PressureControllerOperationMode GetOperationMode()
        {
            PressureControllerOperationMode mode = PressureControllerOperationMode.UNKNOWN;
            if (IsOpen)
            {
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
            return mode;

        }
        public void SetOperationMode(PressureControllerOperationMode value)
        {
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

        internal OneMeasure GetSetPoint()
        {
            double setpoint = double.NaN;
            string answer = Query("Setpt?").Replace(',', '.');
            double.TryParse(answer,
                             NumberStyles.Float,
                             CultureInfo.InvariantCulture,
                             out setpoint);
            return setpoint;
        }
        internal void SetSetPoint(double value)
        {
            var setpoint_str = value.ToString("E04", CultureInfo.InvariantCulture);
            Communicator.SendLine(
                        $"Setpt {setpoint_str}"
                        );
        }
        public int OutForm
        {
            get
            {
                int outFormNum;
                Int32.TryParse(Query("OUTFORM?"), out outFormNum);
                return outFormNum;
            }
            set
            {
                if (value > 0 & value < 8)
                {
                    Communicator.SendLine(String.Format("OUTFORM {0}", value));
                }
            }
        }
        public string OutFormName
        {
            get
            {
                switch (OutForm)
                {
                    case 1:
                        return "pressure value";
                    case 2:
                        return "pressure, units number, mode";
                    case 3:
                        return "pressure, pressure rate";
                    case 4:
                        return "pressure, minimum peak, maximum peak";
                    case 5:
                        return "pressure, active sensor (P or S), active turndown (1-4)";
                    case 6:
                        return "pressure, control point, “stable” or “slewing”";
                    case 7:
                        return "pressure, “no barometer” or baro reading";
                    default:
                        return "error";
                }
            }
        }

        public CPC6000ChannelNumber GetCurrentChannelNum()
        {
            switch (Query("Chan?"))
            {
                case "A":
                    return CPC6000ChannelNumber.A;
                case "B":
                    return CPC6000ChannelNumber.B;
                default:
                    throw new Exception("invalid channel number");
            }
        }
        public void SetCurrentChannelNum(CPC6000ChannelNumber value)
        {
            switch (value)
            {
                case CPC6000ChannelNumber.A:
                    Communicator.SendLine("Chan A");
                    return;
                case CPC6000ChannelNumber.B:
                    Communicator.SendLine("Chan B");
                    return;
                default:
                    throw new Exception("invalid channel number");
            }
        }

        public double GetRangeMin()
        {
            Communicator.SendLine("Sensor P,1");
            var rawdata = Query("RangeMin?").Replace(",", ".");
            double res;
            double.TryParse(
                rawdata,
                NumberStyles.Float,
                new CultureInfo((int)CultureTypes.NeutralCultures),
                out res);
            return res;
        }

        public double GetRangeMax()
        {
            Communicator.SendLine("Sensor P,1");
            var rawdata = Query("RangeMax?").Replace(",", ".");
            double res;
            double.TryParse(
                rawdata,
                NumberStyles.Float,
                new CultureInfo((int)CultureTypes.NeutralCultures),
                out res);
            return res;
        }

        public bool GetAbsPressureSupported()
        {
            bool _absSupported = false;
            string ptp = PType;
            switch (ptp)
            {
                case "A":
                case "ABSOLUTE":
                case "Absolute":
                    _absSupported = true;
                    break;
                case "G":
                case "GAUGE":
                case "Gauge":
                    ToggleAbsGauge();
                    string ptp1 = PType;
                    switch (ptp1)
                    {
                        case "A":
                        case "ABSOLUTE":
                        case "Absolute":
                            _absSupported = true;
                            break;
                    }
                    break;
                default:
                    log4net.LogManager.GetLogger("CPC6000Communication").Debug(
                        string.Format("AbsPressureSupported( {0} ) error", Communicator.ToString())
                        );
                    throw new Exception("AbsPressureSupported() error");
            }
            return _absSupported;

        }

        public void SetPUnits(string value)
        {
            Communicator.SendLine($"Units {value}"); 
        }
        public IUOM GetPUnits()
        {
            var unit = Query("Units?").ToUpper();
            Func<string, double> doubleParser = (s) => double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
            switch (unit)
            {
                case "PSI" : return new customPressureUOM("psi", doubleParser("6.894757E+03"));
                case "INHG @0C" : return new customPressureUOM("inHg @0C", doubleParser("3.386390E+03"));
                case "INHG @60F" : return new customPressureUOM("inHg @60F", doubleParser("3.376850E+03"));
                case "INH2O @4C" : return new customPressureUOM("inH2O @4C", doubleParser("2.490820E+02"));
                case "INH2O @20C" : return new customPressureUOM("inH2O @20C", doubleParser("2.486410E+02"));
                case "INH2O @60F" : return new customPressureUOM("inH2O @60F", doubleParser("2.488400E+02"));
                case "FTH2O @4C" : return new customPressureUOM("ftH2O @4C", doubleParser("2.988980E+03"));
                case "FTH2O @20C" : return new customPressureUOM("ftH2O @20C", doubleParser("2.983692E+03"));
                case "FTH2O @60F" : return new customPressureUOM("ftH2O @60F", doubleParser("2.986080E+03"));
                case "MTORR" : return new customPressureUOM("mTorr", doubleParser("1.333220E-01"));
                case "INSW @0C 3.5% SALINITY" : return new customPressureUOM("inSW @0C 3.5% salinity", doubleParser("2.560885E+02"));
                case "FTSW @0C 3.5% SALINITY" : return new customPressureUOM("ftSW @0C 3.5% salinity", doubleParser("3.073062E+03"));
                case "ATM" : return new customPressureUOM("atm", doubleParser("1.013250E+05"));
                case "BAR" : return new customPressureUOM("bar", doubleParser("1.00000E+05"));
                case "MBAR" : return new customPressureUOM("mbar", doubleParser("1.00000E+02"));
                case "MMH2O @4C" : return new customPressureUOM("mmH2O @4C", doubleParser("9.806378E+00"));
                case "CMH2O @4C" : return new customPressureUOM("cmH2O @4C", doubleParser("9.806378E+01"));
                case "MH2O @4C" : return new customPressureUOM("mH2O @4C", doubleParser("9.806378E+03"));
                case "MMHG @0C" : return new customPressureUOM("mmHg @0C", doubleParser("1.333220E+02"));
                case "CMHG @0C" : return new customPressureUOM("cmHg @0C", doubleParser("1.333220E+03"));
                case "TORR" : return new customPressureUOM("Torr", doubleParser("1.333220E+02"));
                case "KPA" : return new customPressureUOM("kPa", doubleParser("1.00000E+03"));
                case "PA" : return new customPressureUOM("Pa", doubleParser("1.00000E+00"));
                case "DYN/SQ CM" : return new customPressureUOM("dyn/sq cm", doubleParser("1.00000E-01"));
                case "G/SQ CM" : return new customPressureUOM("g/sq cm", doubleParser("9.806647E+01"));
                case "KG/SQ CM" : return new customPressureUOM("kg/sq cm", doubleParser("9.806647E+04"));
                case "MSW @0C 3.5% SALINITY" : return new customPressureUOM("mSW @0C 3.5% salinity", doubleParser("1.008222E+04"));
                case "OZ/SI" : return new customPressureUOM("oz/si", doubleParser("4.309223E+02"));
                case "PSF" : return new customPressureUOM("psf", doubleParser("4.788025E+01"));
                case "TONS/SQ FT" : return new customPressureUOM("tons/sq ft", doubleParser("9.576052E+04"));
                case "MICRONHG @0C" : return new customPressureUOM("micronHg @0C", doubleParser("1.333220E-01"));
                case "TONS/SQ IN" : return new customPressureUOM("tons/sq in", doubleParser("1.378951E+07"));
                case "HPA" : return new customPressureUOM("hPa", doubleParser("1.00000E+02"));
                case "MPA" : return new customPressureUOM("MPa", doubleParser("1.00000E+06"));
                case "MMH2O @20C" : return new customPressureUOM("mmH2O @20C", doubleParser("9.789017E+00"));
                case "CMH2O @20C" : return new customPressureUOM("cmH2O @20C", doubleParser("9.789017E+01"));
                case "MH2O @20C" : return new customPressureUOM("mH2O @20C", doubleParser("9.789017E+03"));
                default:
                    throw new Exception($"unknown pressure units [{unit}]");
            }
        }
        public string PType
        {
            get
            {
                return Query("Ptype?");
            }
            set
            {
                switch (value)
                {
                    case "G":
                    case "GAUGE":
                    case "A":
                    case "ABSOLUTE":
                        Communicator.SendLine(String.Format("Ptype {0}", value));
                        return;
                    default:
                        throw new Exception("invalid pressure type");
                }
            }
        }
        public void ToggleAbsGauge()
        {
            Communicator.SendLine("_pcs4 func F1");
            return;
        }
        public string GetOpts
        {
            get
            {
                return Query("_pcs4 opt?");
            }
        }
        
        [Obsolete("НЕ РАБОТАЕТ! НА ЭКРАНЕ ОТОБРАЖАЮТСЯ СООБЩЕНИЯ ОБ ОШИБКЕ", true)]
        internal void KeyLock(bool state)
        {
            switch (state)
            {
                case true:
                    Communicator.SendLine("Keylock Yes");
                    break;
                case false:
                    Communicator.SendLine("Keylock No");
                    break;
                default:
                    break;
            }
        }

        public string Query(string cmd)
        {
            ResetError();
            
            string answer = Communicator.QueryCommand(cmd);
            
            if (answer.StartsWith("E"))
            {
                GetLastError();
            
                answer = answer.TrimStart(new Char[] { 'E' });
            }
            
            answer = answer.Trim();
            
            return answer;
        }

        //####################################################
        #endregion

        #region CPC6000 high level commands

        internal Scale GetActualScaleOnChannel(CPC6000ChannelNumber cPC6000ChannelNumber)
        {
            SetCurrentChannelNum(cPC6000ChannelNumber);
            var rngMin = GetRangeMin();
            var rngMax = GetRangeMax();
            var rngUOM = GetPUnits();
            var Result = new Scale(rngMin, rngMax, rngUOM);
            return Result;
        }
        internal void SetUOMOnChannel(CPC6000ChannelNumber cPC6000ChannelNumber, string uomname)
        {
            SetCurrentChannelNum(cPC6000ChannelNumber);
            if (uomname == "kgf/cm²") 
            {
                SetPUnits("kg/sq cm");
                return;
            }
            if (uomname == new mmH2OAt4DegreesCelsius().Name)
            {
                SetPUnits("mmH2O @4C");
                return;
            }
            SetPUnits(uomname);
        }

        internal OneMeasure GetActualPressureOnChannel ( CPC6000ChannelNumber channum)
        {
            SetCurrentChannelNum(channum);

            var uomonchannel = GetPUnits();

            double? chanReading;

            switch (channum)
            {
                case CPC6000ChannelNumber.A:
                    chanReading = ChanAReading;
                    break;
                case CPC6000ChannelNumber.B:
                    chanReading = ChanBReading;
                    break;
                case CPC6000ChannelNumber.Baro:
                    chanReading = ChanBaroReading;
                    break;
                default:
                    throw new Exception("Invalid channum!");
            }
            if (chanReading == null)
            {
                return null;
            }
            return new OneMeasure((double)chanReading, uomonchannel, DateTime.Now );
        }
        #endregion
    }
}
