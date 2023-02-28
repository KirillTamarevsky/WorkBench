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
    class EPascalCommand_Set_SetPoint : InstrumentCmd
    {
        ElmetroPascalChannelSpan _epascalChannelSpan;

        OneMeasure _setPoint;
        public EPascalCommand_Set_SetPoint(ElmetroPascalChannelSpan epascalChannelSpan, OneMeasure setPoint)
        {
            _epascalChannelSpan = epascalChannelSpan;
            
            var ScaleMin = new OneMeasure(_epascalChannelSpan.Scale.Min, _epascalChannelSpan.Scale.UOM);
            var ScaleMax = new OneMeasure(_epascalChannelSpan.Scale.Max, _epascalChannelSpan.Scale.UOM);

            OneMeasure correctedSetPoint = setPoint;

            if (setPoint < ScaleMin) ScaleMin.TryConvertTo(setPoint.UOM, out correctedSetPoint); 
            
            if (setPoint > ScaleMax) ScaleMax.TryConvertTo(setPoint.UOM, out correctedSetPoint);

            _setPoint = correctedSetPoint;
        }
        public override void Execute()
        {
            var epascal = _epascalChannelSpan.parentChannel.parent as ElmetroPascal;

            if (epascal != null && _setPoint != null)
            {
                if (_setPoint.TryConvertTo(new kPa(), out OneMeasure setPointInKPA))
                {
                    _epascalChannelSpan.SetPoint = _setPoint;
                 
                    epascal.SetSetPoint(setPointInKPA.Value);
                }

            }
        }
    }
}
