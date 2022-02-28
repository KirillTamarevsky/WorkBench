using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.TestEquipment.CPC6000
{
    public delegate void AutoVentProgress(string progressMessage);

    public delegate void AutoventCompleted();

    class cpc6000Command_AutoVENT : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _chan;

        public cpc6000Command_AutoVENT(CPC6000 cpc, CPC6000ChannelNumber chan, AutoVentProgress autoVentProgress, AutoventCompleted autoventCompleted)
        {
            _cpc = cpc;

            _chan = chan;
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
