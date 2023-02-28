using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_SetSetPoint : CPC6000CommandBase
    {
        OneMeasure _newSP;

        public CPC6000Command_SetSetPoint(OneMeasure newSetPoint)
        {
            _newSP = newSetPoint;
        }
        public override void Execute()
        {
            if (CPC == null | ChannelNumber == null) throw new ArgumentNullException();

            CPC.SetCurrentChannelNum((CPC6000ChannelNumber)ChannelNumber);
            CPC.SetUOMOnChannel((CPC6000ChannelNumber)ChannelNumber, _newSP.UOM.Name);
            CPC.SetSetPoint(_newSP.Value);
        }
    }
}
