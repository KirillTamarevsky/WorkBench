using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.TestEquipment.ElmetroPascal;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    class EPascalCommand_Zeroing_Pressure : InstrumentCmd
    {
        ElmetroPascalChannelSpan _epspan;

        public EPascalCommand_Zeroing_Pressure(ElmetroPascalChannelSpan epspan)
        {
            _epspan = epspan;
        }
        public override void Execute()
        {
            var ep = _epspan.parentChannel.parent as ElmetroPascal;
            if (ep != null)
            {
                var res = ep.Zeroing();
            }
        }
    }
}
