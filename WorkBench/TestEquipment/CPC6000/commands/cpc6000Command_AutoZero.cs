using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;

namespace WorkBench.TestEquipment.CPC6000
{
    class cpc6000Command_AutoZero : CPC6000CommandBase
    {

        public cpc6000Command_AutoZero()
        {
        }

        public override void Execute()
        {
            CPC.Communicator.SendLine("Autozero");

        }
    }
}
