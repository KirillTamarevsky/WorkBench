using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_GetSetPoint : CPC6000cmd
    {
        CPC6000 _cpc;

        CPC6000ChannelNumber _chan;

        Action<OneMeasureResult> _actionOnReaded;

        public CPC6000Command_GetSetPoint(CPC6000 cpc, CPC6000ChannelNumber chan, Action<OneMeasureResult> actionOnReaded)
        {
            _cpc = cpc;

            _chan = chan;

            _actionOnReaded = actionOnReaded;
        }
        public override void Execute()
        {
            _cpc.CurrentChannelNum = _chan;

            var _SP = _cpc.SetPoint ;

            var _uom = _cpc.UOM;

            _actionOnReaded(new OneMeasureResult() {  Value = _SP, UOM =_uom, dateTimeOfMeasurement = DateTime.Now});
        }
    }
}
