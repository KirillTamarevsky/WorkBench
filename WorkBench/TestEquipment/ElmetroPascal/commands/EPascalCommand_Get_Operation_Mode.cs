using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Enums;
using WorkBench.TestEquipment.ElmetroPascal;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    class EPascalCommand_Get_Operation_Mode: InstrumentCmd
    {
        ElmetroPascalChannelSpan _epspan;
        Action<PressureControllerOperationMode> ReportTo;
        public EPascalCommand_Get_Operation_Mode(ElmetroPascalChannelSpan epspan, Action<PressureControllerOperationMode> reportTo)
        {
            _epspan = epspan;

            ReportTo = reportTo;
        }
        public override void Execute()
        {
            ReportTo(_epspan.OperationMode);
        }
    }
}
