using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.Interfaces;
using WorkBench.Enums;
using WorkBench.UOMS;
using System.Globalization;
using System.Threading;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public class ElmetroPascalChannelSpan : IInstrumentChannelSpanPressureGenerator, IInstrumentChannelSpanReader
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("EPascalCommunication");
        private readonly ElmetroPascalChannel parentChannel;
        private readonly ElmetroPascalScale thisScale;
        private OneMeasure LastValue;
        private OneMeasure _setPoint;
        public ElmetroPascalChannelSpan(ElmetroPascalChannel epChannel, ElmetroPascalScale epScale)
        {
            parentChannel = epChannel;

            thisScale = epScale;

            _setPoint = new OneMeasure(0, new kPa(), DateTime.Now);

            LastValue = new OneMeasure(0, new kPa(), DateTime.Now);
        }

        public OneMeasure SetPoint
        {
            get => _setPoint;
            set 
            {
                //Элметро-Паскаль позволяет задать уставку за диапазоном измерений поддиапазона
                //в пределах ± 10 % от всей шкалы этого диапазона
                var scaleRange = thisScale.Max - thisScale.Min;
                var ScaleMin = new OneMeasure(thisScale.Min - scaleRange * 0.1 , thisScale.UOM);
                var ScaleMax = new OneMeasure(thisScale.Max + scaleRange * 0.1, thisScale.UOM);

                OneMeasure correctedSetPoint = value;

                if (value < ScaleMin) ScaleMin.TryConvertTo(value.UOM, out correctedSetPoint);

                if (value > ScaleMax) ScaleMax.TryConvertTo(value.UOM, out correctedSetPoint);

                lock (parentChannel.parentEPascal.Communicator)
                {
                    if (correctedSetPoint.TryConvertTo(new kPa(), out OneMeasure setPointInKPA))
                    {
                        var epreplyStatus = parentChannel.parentEPascal.Communicator.QueryCommand($"TARGET {setPointInKPA.Value.ToString(CultureInfo.InvariantCulture)}", out string epreply);
                        if (epreply.Contains("OK"))
                        {
                            _setPoint = correctedSetPoint;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// indicates that some thread is reading Pressure now. So, no need to read it again, just grab readed value
        /// </summary>
        private bool readingnow;
        /// <summary>
        /// Read Actual pressure on channel.
        /// </summary>
        /// <param name="uom">must be UOMType.Pressure</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"> throws if UOM type is not Pressure</exception>
        public OneMeasure Read(IUOM uom)
        {
            logger.Info($"read uom = { uom.Name }");

            if (uom.UOMType != UOMType.Pressure) throw new ArgumentException($"wrong UOM! ({uom.UOMType} - {uom.Name})");

            var skipActualReading = readingnow == true;
            lock (parentChannel.parentEPascal.Communicator)
            {
                if (!skipActualReading)
                {
                    readingnow = true;
                    parentChannel.ActivateSpan(this);
                    
                    LastValue = null;
                    OneMeasure res = null;

                    var epreplyStatus = parentChannel.parentEPascal.Communicator.QueryCommand("PRES?", out string epreply);
                    
                    epreply = epreply.Trim().Replace(',', '.');
                    
                    if (float.TryParse(epreply, NumberStyles.Float, new System.Globalization.NumberFormatInfo(), out float pressureValue))
                        res = new OneMeasure(pressureValue, new kPa());
                    
                    if (res != null && res.TryConvertTo(uom, out OneMeasure convertedResult))
                        LastValue = convertedResult;
                    
                    readingnow = false;
                }
            }
            return LastValue;
        }


        #region IInstrumentChannelSpanPressureGenerator
        private PressureControllerOperationMode _pressureOperationMode { get => parentChannel._pressureOperationMode; }
        public PressureControllerOperationMode PressureOperationMode 
        {
            get
            {
                return _pressureOperationMode;
            }
            set
            {
                lock (parentChannel.parentEPascal.Communicator)
                {
                    switch (value)
                    {
                        case PressureControllerOperationMode.UNKNOWN:
                            break;
                        case PressureControllerOperationMode.STANDBY:
                        case PressureControllerOperationMode.MEASURE:
                            if (_pressureOperationMode == PressureControllerOperationMode.CONTROL)
                            {
                                parentChannel.ControlToggle();
                            }
                            if (_pressureOperationMode == PressureControllerOperationMode.VENT)
                            {
                                parentChannel.VentToggle();
                            }
                            break;
                        case PressureControllerOperationMode.CONTROL:
                            if (_pressureOperationMode == PressureControllerOperationMode.VENT)
                            {
                                parentChannel.VentToggle();
                            }
                            parentChannel.ControlToggle();
                            if (_pressureOperationMode == PressureControllerOperationMode.MEASURE
                                || _pressureOperationMode == PressureControllerOperationMode.STANDBY)
                            {
                                parentChannel.ControlToggle();
                            }
                            break;
                        case PressureControllerOperationMode.VENT:
                            if (_pressureOperationMode == PressureControllerOperationMode.CONTROL)
                            {
                                parentChannel.ControlToggle();
                                parentChannel.VentToggle();
                            }
                            if (_pressureOperationMode == PressureControllerOperationMode.MEASURE
                                || _pressureOperationMode == PressureControllerOperationMode.STANDBY)
                            {
                                parentChannel.VentToggle();
                            }
                            break;
                        default:
                            throw new Exception($"set {value} while {_pressureOperationMode}");
                    }
                }

            }
        }

        public Scale Scale => thisScale;

        #endregion

        public override string ToString() => $" {parentChannel.parentEPascal.Name}({parentChannel.parentEPascal.Communicator}) {parentChannel.Name} {thisScale} ";

        public void Zero()
        {
            parentChannel.Zero();
        }

    }
}
