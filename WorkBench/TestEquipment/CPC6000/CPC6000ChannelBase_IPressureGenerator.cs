using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000ChannelBase : IPressureGenerator
    {
        public OneMeasureResult SetPoint 
        {
            get
            {
                bool readed = false;

                OneMeasureResult readedSP = null ;

                var cmd = new CPC6000Command_GetSetPoint(
                    parent, 
                    (CPC6000ChannelNumber)NUM, 
                    (OneMeasureResult _sp) => 
                    {
                        readedSP = _sp;

                        readed = true;
                    }
                    );

                parent.cPC6000CommunicationCommands.Enqueue(cmd);

                while (!readed){}

                return readedSP;
            }
            set
            {
                CPC6000cmd cmd = new CPC6000Command_SetSetPoint(parent, (CPC6000ChannelNumber)this.NUM, value);
                parent.cPC6000CommunicationCommands.Enqueue(cmd);
            }
        }

        public PressureControllerOperationMode OperationMode
        { 
            get
            {
                bool readed = false;

                PressureControllerOperationMode operationMode = PressureControllerOperationMode.UNKNOWN;

                var cmd = new CPC6000Command_GetOperationMode(
                    parent,
                    (CPC6000ChannelNumber) NUM,
                    (PressureControllerOperationMode opmod) =>
                    {
                        operationMode = opmod;

                        readed = true;
                    }
                    );
                parent.cPC6000CommunicationCommands.Enqueue(cmd);

                while (!readed){}

                return operationMode;
            }
            set
            {
                var cmd = new CPC6000Command_SetOperationMode(parent, (CPC6000ChannelNumber)NUM, value);

                parent.cPC6000CommunicationCommands.Enqueue(cmd);
            } 
        }

        public bool CanGeneratePressureForGivenScale(Scale scale)
        {
            throw new NotImplementedException();
        }

        public bool PrepareForGenerationOnGivenScale(Scale scale)
        {
            throw new NotImplementedException();
        }
    }
}
