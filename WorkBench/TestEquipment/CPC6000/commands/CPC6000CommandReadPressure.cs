using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000CommandReadPressure : CPC6000cmd
    {

        CPC6000ChannelNumber _channum;

        CPC6000 _cpc6000;

        public CPC6000CommandReadPressure( CPC6000 cPC6000, CPC6000ChannelNumber cPC6000ChannelNumber )
        {
            _cpc6000 = cPC6000;

            _channum = cPC6000ChannelNumber;
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
