using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_Set_UOM_On_Channel : CPC6000CommandBase
    {
        string _uomname;

        public CPC6000Command_Set_UOM_On_Channel( string uomname)
        {
            _uomname = uomname;
        }

        public override void Execute()
        {
            CPC.SetUOMOnChannel((CPC6000ChannelNumber)ChannelNumber, _uomname);
        }
    }
}
