using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_SetOperationMode : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _chan;

        PressureControllerOperationMode _newOpMod;

        public CPC6000Command_SetOperationMode(CPC6000 cpc, CPC6000ChannelNumber chan, PressureControllerOperationMode newOpMod)
        {
            _cpc = cpc;

            _chan = chan;

            _newOpMod = newOpMod;
        }

        public override void Execute()
        {
            _cpc.OperationMode = _newOpMod;
        }
    }
}
