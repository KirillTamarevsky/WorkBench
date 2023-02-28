using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.TestEquipment.ElmetroPascal;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public class EPascalCommand_Set_Active_Module_Range : InstrumentCmd
    {
        ElmetroPascalChannelSpan _epspan;

        public EPascalCommand_Set_Active_Module_Range(ElmetroPascalChannelSpan epspan)
        {
            _epspan = epspan;
        }
        public override void Execute()
        {
            ElmetroPascal ep = _epspan.parentChannel.parent as ElmetroPascal;
            if (ep != null)
            {
                var res = ep.SetActiveModule_Range(_epspan);
            }
        }
    }
}
