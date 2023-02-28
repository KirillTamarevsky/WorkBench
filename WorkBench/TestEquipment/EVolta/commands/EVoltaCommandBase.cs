using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.TestEquipment.EVolta;

namespace WorkBench.TestEquipment.EK.commands
{
    internal abstract class EVoltaCommandBase : InstrumentCmd
    {
        internal EVolta.EVolta Volta { get; set; }
    }
}
