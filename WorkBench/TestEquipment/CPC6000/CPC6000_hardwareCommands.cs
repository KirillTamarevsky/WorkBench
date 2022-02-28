using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.UOM;

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000
    {
        #region CPC6000 hardware interface commands

        //####################################################
        private bool _connected = false;
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
            string answer = _communicator.QueryCommand("Errorno?");
            string[] errorparts = answer.Split(new char[] { '-' });
            if (errorparts.Length == 2)
            {
                Int32.TryParse(errorparts[0].TrimStart(new char[] { 'E' }), out _lastErrNo);
                _lastErrDesc = errorparts[1];
            }
            log4net.LogManager.GetLogger("CPC6000Communication").Debug(String.Format("{0} === {1}", _lastErrNo, _lastErrDesc));
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
                if (_connected)
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
        public double ChanAReading
        {
            get
            {
                var rawdata = Query("A?").Replace(",", ".");
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
                if (_connected)
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
        public double ChanBReading
        {
            get
            {
                var rawdata = Query("B?").Replace(",", ".");
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
        public PressureControllerOperationMode OperationMode
        {
            get
            {
                PressureControllerOperationMode mode = PressureControllerOperationMode.UNKNOWN;
                if (_connected)
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
            set
            {
                switch (value)
                {
                    case PressureControllerOperationMode.STANDBY:
                        _communicator.SendLine("Mode STANDBY");
                        break;
                    case PressureControllerOperationMode.MEASURE:
                        _communicator.SendLine("Mode MEASURE");
                        break;
                    case PressureControllerOperationMode.CONTROL:
                        _communicator.SendLine("Mode CONTROL");
                        break;
                    case PressureControllerOperationMode.VENT:
                        _communicator.SendLine("Mode VENT");
                        break;
                    default:
                        log4net.LogManager.GetLogger("CPC6000Communication").Debug(
                            string.Format("CPC6000 OperationMode( {0} ) - invalid operation mode", _communicator.ToString() )); 
                        throw new Exception("CPC6000 OperationMode() - invalid operation mode");
                }
            }
        }
        internal double SetPoint
        {
            get
            {
                double setpoint = double.MinValue;
                    string answer = Query("Setpt?").Replace(',','.');
                    double.TryParse(answer,
                                     NumberStyles.Float,
                                     new CultureInfo((int)CultureTypes.NeutralCultures),
                                     out setpoint);
                return setpoint;
            }
            set
            {
                _communicator.SendLine(
                        String.Format("Setpt {0}", 
                        value.ToString("E04", new CultureInfo((int)CultureTypes.NeutralCultures) )));
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
                    _communicator.SendLine(String.Format("OUTFORM {0}", value));
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
        public CPC6000ChannelNumber CurrentChannelNum
        {
            get
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
            set
            {
                switch (value)
                {
                    case CPC6000ChannelNumber.A:
                        _communicator.SendLine("Chan A");
                        return;
                    case CPC6000ChannelNumber.B:
                        _communicator.SendLine("Chan B");
                        return;
                    default:
                        throw new Exception("invalid channel number");
                }
            }
        }
        public double RangeMin
        {
            get
            {
                _communicator.SendLine("Sensor P,1");
                var rawdata = Query("RangeMin?").Replace(",", ".");
                double res;
                double.TryParse(
                    rawdata, 
                    NumberStyles.Float, 
                    new CultureInfo( (int)CultureTypes.NeutralCultures ),
                    out res);
                return res;
            }
        }
        public double RangeMax
        {
            get
            {
                _communicator.SendLine("Sensor P,1");
                var rawdata = Query("RangeMax?").Replace(",", ".");
                double res;
                double.TryParse(
                    rawdata,
                    NumberStyles.Float,
                    new CultureInfo( (int)CultureTypes.NeutralCultures ),
                    out res);
                return res;
            }
        }
        public bool AbsPressureSupported
        {
            get
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
                            string.Format("AbsPressureSupported( {0} ) error", _communicator.ToString())
                            );
                        throw new Exception("AbsPressureSupported() error");
                }
                return _absSupported;

            }

        }
        public string PUnits
        {
            get
            {
                return Query("Units?");
            }
            set
            {
                _communicator.SendLine(string.Format("Units {0}", value));
            }
        }
        public Interfaces.IUOM UOM
        {
            get
            {
                switch (Query("Units?").ToUpper())
                {
                    case "BAR":
                        return new UOM.bar();
                    case "MBAR":
                        return new UOM.mbar();
                    case "KPA":
                        return new UOM.kPa();
                    case "MPA":
                        return new UOM.MPa();
                    default:
                        throw new Exception("not implemented pressure units");
                }
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
                        _communicator.SendLine(String.Format("Ptype {0}", value));
                        return;
                    default:
                        throw new Exception("invalid pressure type");
                }
            }
        }
        public void ToggleAbsGauge()
        {
            _communicator.SendLine("_pcs4 func F1");
            return;
        }
        public string GetOpts
        {
            get
            {
                return Query("_pcs4 opt?");
            }
        }

        public string Query(string cmd)
        {
            ResetError();
            
            string answer = _communicator.QueryCommand(cmd);
            
            if (answer.StartsWith("E"))
            {
                GetLastError();
            
                answer = answer.Trim(new Char[] { 'E' });
            }
            
            answer = answer.Trim();
            
            return answer;
        }

        //####################################################
        #endregion

        #region CPC6000 high level commands

        internal Scale GetActualScaleOnChannel(CPC6000ChannelNumber cPC6000ChannelNumber)
        {
            bool readed = false;

            Scale readedScale = null;

            CPC6000cmd cmd = new CPC6000Command_GetChannelRange(
                this,
                cPC6000ChannelNumber,
                (Scale scale) =>
                {
                    readedScale = scale;
                    readed = true;
                }
                );


            var asdfg = new Task(() => { cPC6000CommunicationCommands.Enqueue(cmd); });
            asdfg.Start();
            while (!readed) { } ;
            return readedScale;
            //return new Scale() { Min = -1.25, Max = 1.25, UOM = new kPa() };

        }

        internal void SetUOMOnChannel(CPC6000ChannelNumber cPC6000ChannelNumber, string uomname)
        {
            CurrentChannelNum = cPC6000ChannelNumber;

            PUnits = uomname;
        }

        internal OneMeasureResult GetActualPressureOnChannel ( CPC6000ChannelNumber channum)
        {
            CurrentChannelNum = channum;

            var uomonchannel = UOM;

            double chanReading;

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
            return new OneMeasureResult() { Value = chanReading, UOM = uomonchannel, dateTimeOfMeasurement = DateTime.Now };
        }
        #endregion
    }
}
