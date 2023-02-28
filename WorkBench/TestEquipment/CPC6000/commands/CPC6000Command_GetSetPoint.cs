using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_GetSetPoint : CPC6000CommandBase
    {
        public OneMeasure SetPoint { get; private set; }
        
        Action<OneMeasure> ReportTo;
        public CPC6000Command_GetSetPoint(Action<OneMeasure> reportTo)
        {
            ReportTo= reportTo;
        }
        public override void Execute()
        {
            if (CPC == null | ChannelNumber == null)
            {
                throw new InvalidOperationException($"CPC == null({CPC == null}); ChannelNumber == null({ChannelNumber == null})");
            }
            CPC.SetCurrentChannelNum((CPC6000ChannelNumber)ChannelNumber);

            var _SP = CPC.GetSetPoint();

            var _uom = CPC.GetPUnits();

            SetPoint = new OneMeasure(_SP, _uom, DateTime.Now);

            ReportTo(SetPoint);

        }
    }
}
