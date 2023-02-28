using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;

namespace WorkBench.TestEquipment.CPC6000
{
    internal abstract class CPC6000CommandBase : InstrumentCmd
    {
        public CPC6000 CPC { get; internal set; }
        public CPC6000ChannelNumber? ChannelNumber { get; internal set; }
    }
}
