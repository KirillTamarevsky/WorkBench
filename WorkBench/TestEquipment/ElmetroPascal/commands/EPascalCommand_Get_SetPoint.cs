using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.ElmetroPascal;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    class EPascalCommand_Get_SetPoint : InstrumentCmd
    {
        ElmetroPascalChannelSpan _epascalChannelSpan;
        Action<OneMeasure> ReportTo;
        public EPascalCommand_Get_SetPoint(ElmetroPascalChannelSpan epascalChannelSpan, Action<OneMeasure> _reportTo)
        {
            _epascalChannelSpan = epascalChannelSpan;
            ReportTo = _reportTo;
            
        }
        public override void Execute()
        {
            ReportTo?.Invoke(_epascalChannelSpan.SetPoint);
        }
    }
}
