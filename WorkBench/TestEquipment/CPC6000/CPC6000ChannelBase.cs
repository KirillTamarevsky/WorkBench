using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000ChannelBase : IChannel, IReader, IPressureGenerator
    {
        List<Scale> supportedMeasureTypes
        {
            get
            {
                Scale scale = parent.GetActualScaleOnChannel((CPC6000ChannelNumber)NUM);

                return new List<Scale>() { scale };
            }
        }

        protected internal CPC6000 parent;

        UOMType lastReadType = UOMType.Pressure;

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

            CPC6000cmd cmd = new CPC6000Command_Set_UOM_On_Channel(parent, (CPC6000ChannelNumber)this.NUM, scale.UOM.Name );

            parent.cPC6000CommunicationCommands.Enqueue(cmd);

            return supportedMeasureTypes
                .FindAll(s => s.UOM.UOMType == scale.UOM.UOMType)
                .FindAll(s => s.UOM.Name == scale.UOM.Name)
                .FindAll(s => (s.Min <= scale.Min))
                .FindAll(s => (s.Max >= scale.Max))
                .Count > 0;
        }

        public void Read(UOMType uOMType)
        {
            switch (uOMType)
            {
                case UOMType.Pressure:
                    CPC6000cmd cmd = new CPC6000Command_Get_Actual_Pressure_on_channel(parent, (CPC6000ChannelNumber)_num);
                    parent.cPC6000CommunicationCommands.Enqueue(cmd);
                    lastReadType = uOMType;
                    break;
                default:
                    throw new Exception("wrong UOM!");
            }
        }

        public CPC6000ChannelBase()
        {
        }

        public string Name
        {
            get
            {
                return String.Format("{0} {1} канал {2} {3}",
                    parent.Description,
                    parent.Name,
                    NUM,
                    supportedMeasureTypes.FirstOrDefault().ToString()
                    ) ;
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
