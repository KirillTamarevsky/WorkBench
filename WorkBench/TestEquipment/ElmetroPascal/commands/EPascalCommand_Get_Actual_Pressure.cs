using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.ElmetroPascal;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    class EPascalCommand_Get_Actual_Pressure : InstrumentCmd
    {
        ElmetroPascalChannelSpan _epascalChannelSpan;

        IUOM _uom;

        Action<OneMeasure> ReportTo;
        public EPascalCommand_Get_Actual_Pressure(ElmetroPascalChannelSpan epascalChannelSpan, IUOM uom, Action<OneMeasure> reportTo )
        {
            _epascalChannelSpan = epascalChannelSpan;

            _uom = uom;

            ReportTo = reportTo;
        }
        public override void Execute()
        {
            var epascal = _epascalChannelSpan.parentChannel.parent as ElmetroPascal;
            if (epascal != null)
            {
                var res = epascal.GetActualPressure();
         
                //_epascalChannelSpan.LastValue = new OneMeasure(res.Value * (res.UOM.Factor / _uom.Factor), _uom, res.TimeStamp);
                
                if(res != null && res.TryConvertTo(_uom, out OneMeasure convertedResult))
                {
                    _epascalChannelSpan.LastValue = convertedResult;
                }
                
                ReportTo?.Invoke(_epascalChannelSpan.LastValue);
                
            }
        }
    }
}
