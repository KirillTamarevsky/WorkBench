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
            }
        }

        public string Query(string cmd)
        {
            ResetError();
            
            string answer = Communicator.QueryCommand(cmd);
            
            if (answer.StartsWith("E"))
            {
                GetLastError();
            
                answer = answer.TrimStart(new char[] { 'E' });
            }
            
            answer = answer.Trim();
            
            return answer;
        }

        //####################################################
        #endregion

        #region CPC6000 high level commands

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
        #endregion
    }
}
