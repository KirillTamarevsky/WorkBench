using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.TestEquipment.ElmetroPascal;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    class EPascalCommand_Set_Vent_Close : InstrumentCmd
    {
        ElmetroPascal _epascal;

        public EPascalCommand_Set_Vent_Close(ElmetroPascal epascal)
        {
            _epascal = epascal;
        }
        public override void Execute()
        {
            var res = _epascal.VentClose();
        }
    }
}
