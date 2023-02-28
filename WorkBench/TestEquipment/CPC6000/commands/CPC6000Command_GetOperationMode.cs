using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Enums;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_GetOperationMode : CPC6000CommandBase
    {
        public PressureControllerOperationMode Result { get; private set; }

        Action<PressureControllerOperationMode> ReportTo;
        public CPC6000Command_GetOperationMode(Action<PressureControllerOperationMode> reportTo)
        {
            ReportTo = reportTo;
        }

        public override void Execute()
        {
            CPC.SetCurrentChannelNum((CPC6000ChannelNumber)ChannelNumber);

            Result = CPC.GetOperationMode();

            ReportTo?.Invoke(Result);

        }
    }
}
