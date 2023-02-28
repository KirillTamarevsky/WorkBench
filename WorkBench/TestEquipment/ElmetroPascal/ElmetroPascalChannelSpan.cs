using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan;
using WorkBench.Interfaces;
using WorkBench.Enums;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public class ElmetroPascalChannelSpan : AbstractInstrumentChannelSpanReader_and_Generator, IInstrumentChannelSpanPressureGenerator
    {
        public ElmetroPascalChannelSpan()
        {
//            CyclicRead = false;

            //OperationMode = PressureControllerOperationMode.UNKNOWN;

            SetPoint = new OneMeasure(0, new kPa(), DateTime.Now);

            LastValue = new OneMeasure(0, new kPa(), DateTime.Now);

        }

        #region AbstractInstrumentChannelSpanReader_and_Generator
        private OneMeasure _setPoint;
        public OneMeasure SetPoint { get{return _setPoint;}internal set{_setPoint = value;}
        }

        public override void GetSetPoint(Action<OneMeasure> reportTo)
        {
            EnqueueInstrumentCmd(new EPascalCommand_Get_SetPoint(this, reportTo));
        }
        public override void SetSetPoint(OneMeasure value)
        {
            //((ElmetroPascal)parentChannel.parent).EnqueueInstrumentCmd(
            //    new EPascalCommand_Set_SetPoint((ElmetroPascal)parentChannel.parent, value)
            //    ); 
                EnqueueInstrumentCmd(new EPascalCommand_Set_SetPoint(this, value));
        }

        public override void Read(IUOM uom, Action<OneMeasure> reportTo)
        {
            var logger = log4net.LogManager.GetLogger("");
            logger.Info(string.Format("read uom = {0}", uom.Name));

            if (uom.UOMType != UOMType.Pressure) throw new ArgumentException($"wrong UOM! ({uom.UOMType} - {uom.Name})");
            
            var cmd = new EPascalCommand_Get_Actual_Pressure(this, uom, reportTo);
            EnqueueInstrumentCmd(cmd);
        }

        #endregion

        #region IInstrumentChannelSpanPressureGenerator

        public PressureControllerOperationMode OperationMode { get; private set; }

        public void GetPressureOperationMode(Action<PressureControllerOperationMode> reportTo)
        {
            EnqueueInstrumentCmd(new EPascalCommand_Get_Operation_Mode(this, reportTo));
            //reportTo(OperationMode);
        }

        public void SetPressureOperationMode(PressureControllerOperationMode value)
        {

            var ep = parentChannel.parent as ElmetroPascal;
            switch (value)
            {
                case PressureControllerOperationMode.UNKNOWN:
                    break;
                case PressureControllerOperationMode.STANDBY:
                case PressureControllerOperationMode.MEASURE:
                    EnqueueInstrumentCmd(new EPascalCommand_Set_Generation_OFF(ep));
                    break;
                case PressureControllerOperationMode.CONTROL:
                    if (OperationMode == PressureControllerOperationMode.VENT)
                    {
                        EnqueueInstrumentCmd(new EPascalCommand_Set_Vent_Close(ep));
                    }
                    EnqueueInstrumentCmd(new EPascalCommand_Set_Generation_ON(ep));
                    break;
                case PressureControllerOperationMode.VENT:
                    if (OperationMode == PressureControllerOperationMode.CONTROL)
                    {
                        EnqueueInstrumentCmd(new EPascalCommand_Set_Generation_OFF(ep));
                    }
                    EnqueueInstrumentCmd(new EPascalCommand_Set_Vent_Open(ep));
                    break;
                default:
                    break;
            }
            OperationMode = value;
        }
        #endregion

        public override string ToString()
        {
            return Scale.ToString();
        }

        public override void Activate()
        {
            EnqueueInstrumentCmd(new EPascalCommand_Set_Active_Module_Range(this));
            parentChannel.ActiveSpan = this;

        }

        public override void Zero()
        {
//                var epascal = parentChannel.parent as ElmetroPascal;
//                if (epascal == null) throw new NullReferenceException($"{parentChannel} parent is null");

//                epascal.Zeroing();

                var cmd = new EPascalCommand_Zeroing_Pressure(this);
                EnqueueInstrumentCmd(cmd);
        }
    }
}
