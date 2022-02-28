using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_Get_Actual_Pressure_on_channel : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _chan;

        public CPC6000Command_Get_Actual_Pressure_on_channel(CPC6000 cpc, CPC6000ChannelNumber chan)
        {
            _cpc = cpc;

            _chan = chan;
        }
        public override void Execute()
        {
            var res = _cpc.GetActualPressureOnChannel(_chan);

            _cpc[_chan].lastValue = res;
        }
    }
}
