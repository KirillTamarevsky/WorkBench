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
using WorkBench.Communicators;
using log4net;
using System.Threading;

namespace WorkBench.TestEquipment.CPC6000
{
    internal class CPC6000ChannelTurnDown : IInstrumentChannelSpanPressureGenerator, IInstrumentChannelSpanReader
    {
        readonly ILog logger = LogManager.GetLogger("Communication");

        internal CPC6000Channel parentChannel { get; }
        private ITextCommunicator Communicator => parentChannel.Communicator;
        internal CPC6000PressureModule module { get; }
        private bool IsOpen => parentChannel.parentCPC6000.IsOpen;
        private TextCommunicatorQueryCommandStatus Query(string cmd, out string res) => parentChannel.Query(cmd, out res);
        private TextCommunicatorQueryCommandStatus Query(string cmd, out string res, Func<string, bool> validationRule) => parentChannel.Query(cmd, out res, validationRule);

        private IUOM GetPUnit()
        {
            var uom = parentChannel.GetPUnit();
            return uom;

        }
        private void SetPUnit(IUOM targetUOM) => parentChannel.SetPUnit(targetUOM);
        internal int turndown { get; }
        public PressureType PressureType { get; }
        internal OneMeasure RangeMin { get; }
        internal OneMeasure RangeMax { get; }
        public Scale Scale { get
            {
                var unit = parentChannel.ActiveUOM;
                OneMeasure rngmin, rngmax;
                if (RangeMin.TryConvertTo(unit, out rngmin) && RangeMax.TryConvertTo(unit, out rngmax))
                {
                    return new Scale(rngmin.Value, rngmax.Value, unit);
                }
                throw new Exception();
            }
        }
        internal CPC6000ChannelTurnDown(CPC6000Channel _parentChannel, CPC6000PressureModule _module, int _turndown, PressureType pressureType, OneMeasure rngMin, OneMeasure rngMax)
        {
            parentChannel = _parentChannel;
            module = _module;
            turndown = _turndown;
            PressureType = pressureType;

            RangeMin = rngMin;
            RangeMax = rngMax;

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
                        var answerStatus = Query("Mode?", out string answer);
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
                    var punit = GetPUnit();
                    double setpoint = double.NaN;
                    var answerStatus = Query("Setpt?", out string answer);
                    answer = answer.Replace(',', '.');
                    double.TryParse(answer,
                                     NumberStyles.Float,
                                     CultureInfo.InvariantCulture,
                                     out setpoint);
                    return new OneMeasure(setpoint, punit);
                }
            }
            set
            {
                lock (Communicator)
                {

                    parentChannel.SetActiveTurndown(this);
                    SetPUnit(value.UOM);
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

            logger.Info($"{parentChannel.ChannelNumber} autozero command");

            float secondsToComplete = 1;
            int autoZeroState = 0;
            Func<string, bool> AutozeroValidationRule = (s) =>
            {
                var answerParts = s.Split(',');
                if (answerParts.Length >= 2) 
                {
                    if ((new[]{
                        "0" // complete
                        ,"1" // local autozero
                        ,"2"  // remote autozero
                    }).Contains(answerParts[0].Trim()))
                    {
                        autoZeroState = int.Parse(answerParts[0].Trim());
                        if (float.TryParse( answerParts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out secondsToComplete))
                        {
                            return true;
                        }
                    }
                }
                return false;
            };

            do
            {
                logger.Info($"{parentChannel.ChannelNumber} sleep for {secondsToComplete}");
                Thread.Sleep( TimeSpan.FromSeconds( (int)secondsToComplete) );
                TextCommunicatorQueryCommandStatus queryRes;
                string res;
                lock (Communicator)
                {
                    queryRes = Communicator.QueryCommand("Autozero?", out res, AutozeroValidationRule);
                }
                if (queryRes != TextCommunicatorQueryCommandStatus.Success)
                {
                    logger.Info($"{parentChannel.ChannelNumber} autozero? command answer != Success");
                    break;
                }
                logger.Info($"{parentChannel.ChannelNumber} autozero? command answer = {res}");
            }while (autoZeroState > 0);
                
            logger.Info($"{parentChannel.ChannelNumber} set active channel {this.turndown}");

            parentChannel.SetActiveTurndownForced(this);
        }

        public override string ToString()
        {
            string presstype = string.Empty;
            switch (PressureType)
            {
                case PressureType.Absolute:
                    presstype = "(abs)";
                    break;
                case PressureType.Gauge:
                    presstype = "(gauge)";
                    break;
                default:
                    break;
            }
            return $"{parentChannel} - {Scale} {presstype}";
        }

        internal string readPressureCommand => parentChannel.ChannelNumber switch
        {
            CPC6000ChannelNumber.A => "A?",
            CPC6000ChannelNumber.B => "B?",
            CPC6000ChannelNumber.Baro => "Baro?",
            _ => throw new NotImplementedException()
        };

        public OneMeasure Read(IUOM uom)
        {
            logger.Info($"{this.parentChannel.ChannelNumber}{this.module}{this.turndown}{this.PressureType}");
            Func<string, bool> floatValidationRule = (s) => double.TryParse(s.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out double _);
            
            if (uom.UOMType != UOMType.Pressure) return null;
            
            lock (Communicator)
            {
                parentChannel.SetActiveTurndown(this);
                var unit = GetPUnit();
                if (unit == null)
                {
                    return null;
                }
                if (unit.Factor != uom.Factor)
                {
                    SetPUnit(uom);
                    unit = GetPUnit();
                    if (unit == null)
                    {
                        return null;
                    }
                }
                var replyStatus = Query(readPressureCommand, out string reply, floatValidationRule);
                if (replyStatus == TextCommunicatorQueryCommandStatus.Success)
                {
                    var pressureValue = double.Parse(reply.Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);
                    return new OneMeasure(pressureValue, unit);
                }
                return null;
            }
        }

    }
}
