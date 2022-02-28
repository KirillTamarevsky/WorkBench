using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_SetSetPoint : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _chan;

        OneMeasureResult _newSP;

        public CPC6000Command_SetSetPoint(CPC6000 cpc, CPC6000ChannelNumber chan, OneMeasureResult newSetPoint)
        {
            _cpc = cpc;

            _chan = chan;

            _newSP = newSetPoint;
        }
        public override void Execute()
        {
            _cpc.CurrentChannelNum = _chan;
            _cpc.SetUOMOnChannel(_chan, _newSP.UOM.Name);
            _cpc.SetPoint = _newSP.Value;
        }
    }
}
