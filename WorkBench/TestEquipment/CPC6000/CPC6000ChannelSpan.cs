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
        internal CPC6000Channel parentChannel { get; }
        private ITextCommunicator Communicator => parentChannel.Communicator;
        internal CPC6000PressureModule module { get; }
        private bool IsOpen => parentChannel.parentCPC6000.IsOpen;
        private string Query(string cmd) => parentChannel.parentCPC6000.Query(cmd);

        private IUOM GetPUnit() => parentChannel.GetPUnit();
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
        public CPC6000ChannelSpan(CPC6000Channel _parentChannel, CPC6000PressureModule _module, int _turndown, PressureType pressureType)
        {
            parentChannel = _parentChannel;
            module = _module;
            turndown = _turndown;
            PressureType = pressureType;
            parentChannel.SetActiveTurndown(this);
            var unit = GetPUnit();
            RangeMin = new OneMeasure(double.Parse(Query("RangeMin?").Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture), unit);
            RangeMax = new OneMeasure(double.Parse(Query("RangeMax?").Trim().Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture), unit);

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
                    var punit = GetPUnit();
                    double setpoint = double.NaN;
                    string answer = Query("Setpt?").Replace(',', '.');
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

        public OneMeasure Read(IUOM uom)
        {
            lock (Communicator)
            {

                if (uom.UOMType != UOMType.Pressure) throw new Exception($"not possible to read uom type {uom.Name} ");
                parentChannel.SetActiveTurndown(this);
                var unit = GetPUnit();
                if (unit.Factor != uom.Factor)
                {
                    SetPUnit(uom);
                    unit = GetPUnit();
                }
                var reply = Communicator.QueryCommand(parentChannel.readPressureCommand).Trim().Replace(",", ".");
                var pressureValue = double.NaN;
                if (!double.TryParse(reply, NumberStyles.Float, CultureInfo.InvariantCulture, out pressureValue))
                {
                    reply = Communicator.QueryCommand(parentChannel.readPressureCommand).Trim().Replace(",", ".");
                    double.TryParse(reply, NumberStyles.Float, CultureInfo.InvariantCulture, out pressureValue);
                }
                return new OneMeasure(pressureValue, unit);
            }
        }

    }
}
