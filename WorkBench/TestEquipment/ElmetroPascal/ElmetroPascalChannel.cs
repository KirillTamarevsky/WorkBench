using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.UOM;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public class ElmetroPascalChannel : IChannel, IReader, IPressureGenerator
    {
        protected internal ElmetroPascal parent;

        #region IPressureGenerator
        
        PressureControllerOperationMode _operationMode;
        public PressureControllerOperationMode OperationMode
        {
            get
            {
                return _operationMode;
            }
            set
            {
                _operationMode = value;
                switch (value)
                {
                    case PressureControllerOperationMode.UNKNOWN:
                        break;
                    case PressureControllerOperationMode.STANDBY:
                    case PressureControllerOperationMode.MEASURE:
                        parent.StopPressureGeneration();
                        break;
                    case PressureControllerOperationMode.CONTROL:
                        parent.StartPressureGeneration();
                        break;
                    case PressureControllerOperationMode.VENT:
                        parent.VentOpen();
                        break;
                    default:
                        break;
                }
            }
        }

        OneMeasureResult _setPoint;
        public OneMeasureResult SetPoint 
        {
            get 
            {
                return _setPoint;
            }
            
            set
            {
                if (value.UOM.UOMType == UOMType.Pressure)
                {
                    _setPoint = value;
                    parent.SetSetPoint(value.Value * (value.UOM.Factor / new kPa().Factor));
                }
                
            }

        }
        #endregion

        #region IChannel


        public int NUM
        {
            get
            {
                return 1;
            }
        }

        public string Name
        {
            get
            {
                return "канал давления";
            }
        }

        #endregion

        #region IReader

        public OneMeasureResult lastValue => throw new NotImplementedException();
      
        private bool _cyclicRead;
        public bool CyclicRead 
        { 
            get
            {
                return _cyclicRead;
            }
            set
            {
                _cyclicRead = value;
            }
        }

        public event NewValueReaded NewValueReaded;

        public bool CanRead(Scale scale)
        {
            
            throw new NotImplementedException();
        }

        public void Read(UOMType uOMType)
        {
            throw new NotImplementedException();
        }

        public bool CanGeneratePressureForGivenScale(Scale scale)
        {
            throw new NotImplementedException();
        }

        public bool PrepareForGenerationOnGivenScale(Scale scale)
        {
            throw new NotImplementedException();
        }

        #endregion

        public ElmetroPascalChannel()
        {
            OperationMode = PressureControllerOperationMode.UNKNOWN;
        }




    }
}
