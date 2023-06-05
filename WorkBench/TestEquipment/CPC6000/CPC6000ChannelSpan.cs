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

namespace WorkBench.TestEquipment.CPC6000
{
    internal abstract class CPC6000ChannelSpan : IInstrumentChannelSpanPressureGenerator, IInstrumentChannelSpanReader
    {
        private CPC6000Channel parentChannel { get; }
        private ITextCommunicator Communucator { get => parentChannel.Communicator}
        internal CPC6000PressureModule module { get; }
        internal int turndown { get; }
        public Scale Scale { get; }
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
                if (parent.IsOpen)
                {
                    string answer = parent.Query("Mode?");
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
                if (parent.IsOpen)
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
            }
        }
        public OneMeasure SetPoint
        { 
            get => parentChannel.parent.GetSetPoint(); 
            set => parentChannel.parent.SetSetPoint(value); 
        }


        public void Zero()
        {
            parentChannel.parent.Communicator.SendLine("Autozero");
        }

        public override string ToString()
        {
            return $"{parentChannel} - {Scale}";
        }

        public OneMeasure Read(IUOM uom)
        {
            if (uom.UOMType != UOMType.Pressure) throw new Exception($"not possible to read uom type {uom.Name} ");
            
            return parentChannel.ReadPressure(); 
        }
    }
}
