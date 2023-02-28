using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_GetChannelRange : CPC6000CommandBase
    {
        Action<Scale> ScaleReportTo;
        public Scale Result { get; private set; }

        public CPC6000Command_GetChannelRange(Action<Scale> scaleReportTo)
        {
            ScaleReportTo = scaleReportTo;
        }

        public override void  Execute()
        {
            Result = CPC.GetActualScaleOnChannel((CPC6000ChannelNumber)this.ChannelNumber);

            if (ScaleReportTo != null)
            {
                ScaleReportTo(Result);
            }
        }

    }
}
