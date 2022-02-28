using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_GetOperationMode : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _chan;

        Action<PressureControllerOperationMode> _actionOnReaded;

        public CPC6000Command_GetOperationMode(CPC6000 cpc, CPC6000ChannelNumber chan, Action<PressureControllerOperationMode> actionOnReaded)
        {
            _cpc = cpc;

            _chan = chan;

            _actionOnReaded = actionOnReaded;
        }

        public override void Execute()
        {
            _cpc.CurrentChannelNum = _chan;

            var _MODE = _cpc.OperationMode;

            _actionOnReaded(_MODE);
        }
    }
}
