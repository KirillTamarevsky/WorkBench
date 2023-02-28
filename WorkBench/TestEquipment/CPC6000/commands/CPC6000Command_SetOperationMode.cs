using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Enums;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_SetOperationMode : CPC6000CommandBase
    {
        PressureControllerOperationMode _newOpMod;

        public CPC6000Command_SetOperationMode(PressureControllerOperationMode newOpMod)
        {
            _newOpMod = newOpMod;
        }

        public override void Execute()
        {
            CPC.SetCurrentChannelNum((CPC6000ChannelNumber)ChannelNumber);

            CPC.SetOperationMode(_newOpMod);

        }
    }
}
