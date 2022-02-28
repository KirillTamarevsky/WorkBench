using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_GetChannelRange : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _cpcHannel;

        Action<Scale> _actionOnReaded;

        public CPC6000Command_GetChannelRange( 
            CPC6000 cpc, 
            CPC6000ChannelNumber cpcHannel, 
            Action<Scale> actionOnReaded)
        {
            _cpc = cpc;

            _cpcHannel = cpcHannel;

            _actionOnReaded = actionOnReaded;
        }

        public override void  Execute()
        {
            _cpc.CurrentChannelNum = _cpcHannel;

            var rngMin = _cpc.RangeMin;
            var rngMax = _cpc.RangeMax;
            var rngUOM = _cpc.UOM;

            _actionOnReaded(new Scale() { Min = rngMin, Max = rngMax, UOM = rngUOM });
        }

    }
}
