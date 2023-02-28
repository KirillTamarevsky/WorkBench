using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.CPC6000
{
    class CPC6000Command_Get_Actual_Pressure_on_channel : CPC6000CommandBase
    {
        private IUOM UOM;
        
        Action<OneMeasure> ReportTo;
        public CPC6000Command_Get_Actual_Pressure_on_channel(IUOM uom, Action<OneMeasure> reportTo)
        {
            UOM = uom;
            ReportTo = reportTo;
        }
        public override void Execute()
        {
            if (CPC == null | ChannelNumber == null) throw new ArgumentNullException();

            OneMeasure Result = CPC.GetActualPressureOnChannel((CPC6000ChannelNumber)ChannelNumber);

            if (UOM.UOMType != Result.UOM.UOMType || UOM.Factor != Result.UOM.Factor)
            {
                CPC.SetUOMOnChannel((CPC6000ChannelNumber)ChannelNumber, UOM.Name);
                Result = CPC.GetActualPressureOnChannel((CPC6000ChannelNumber)ChannelNumber);
            }

            ReportTo?.Invoke(Result);

        }
    }
}
