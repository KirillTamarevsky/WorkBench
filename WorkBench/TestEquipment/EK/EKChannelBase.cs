using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.EK
{ 
    public class EKChannelBase : IChannel, IReader
    {

        List<Scale> supportedMeasureTypes = new List<Scale>()
            {
                new Scale(){Min = 0, Max = 20, UOM = new UOM.mA()}
            };

        protected internal EK parent;

        UOMType lastReadType;

        public event NewValueReaded NewValueReaded;

        protected void RaiseNewValueReaded()
        {
            if (NewValueReaded != null) 
            { 
                NewValueReaded(this); 
            }
        }

        private int _num;

        public int NUM
        {
            get
            {
                return _num;
            }
            protected internal set
            {
                _num = value;
            }
        }

        public bool CanRead(Scale scale)
        {
            return supportedMeasureTypes.FindAll(s => s.UOM.UOMType == scale.UOM.UOMType).FindAll(s => (s.Min <= scale.Min)).FindAll(s => (s.Max >= scale.Max)).Count > 0;
        }

        public virtual void Read(WorkBench.Enums.UOMType uOMType)
        {
            //lastValue = parent.Read_0_20_Current_with_ext_pwr((EKchanNum)NUM);
            //RaiseNewValueReaded();
            if (supportedMeasureTypes.FindAll(s => s.UOM.UOMType == uOMType).Count > 0)
            {
                lastReadType = uOMType;
                
                switch (uOMType)
                {
                    case UOMType.Current:
                        EKCommunicationCommand cmd = new EKCommandRead_0_20_mA((EKchanNum)_num, parent);
                        parent.eKCommunicationCommands.Enqueue(cmd);
                        break;
                    case UOMType.Pressure:
                        break;
                    case UOMType.Resistance:
                        break;
                    default:
                        break;
                }
            }
            //throw new Exception();
        }


        public EKChannelBase()
        {

        }

        public string Name
        {
            get
            {
                return String.Format("{0} канал {1}", parent.Name, NUM);
            }
        }
        private OneMeasureResult _lastValue;
        public OneMeasureResult lastValue 
        { 
            get
            {
                return _lastValue;
            }
            protected internal set 
            {
                _lastValue = value;
                RaiseNewValueReaded();
                if (CyclicRead)
                {
                    Read(lastReadType);
                }
            } 
        }
        public bool CyclicRead { get; set ; }

        public override string ToString()
        {
            return Name;
        }
    }
}
