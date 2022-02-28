using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_Set_UOM_On_Channel : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _cpcchan;

        string _uomname;

        public CPC6000Command_Set_UOM_On_Channel(CPC6000 cpc, CPC6000ChannelNumber cpcchannel, string uomname)
        {
            _cpc = cpc;

            _cpcchan = cpcchannel;

            _uomname = uomname;
        }

        public override void Execute()
        {
            _cpc.SetUOMOnChannel(_cpcchan, _uomname);
        }
    }
}
