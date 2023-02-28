using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK.channelSpans;

namespace WorkBench.TestEquipment.EK.commands
{
    class EKCommand_Read_0_20_mA : InstrumentCmd
    {
        EKChannelSpan_0_20_mA _eKchan;
        Action<OneMeasure> ReportTo;
        public EKCommand_Read_0_20_mA(EKChannelSpan_0_20_mA eKchan, Action<OneMeasure> reportTo)
        {
            _eKchan = eKchan;
            ReportTo = reportTo;
        }

        public override void Execute()
        {
            if (_eKchan.parentChannel.parent is EK ek)
            {
                var res = ek.Read_0_20_Current_with_ext_pwr(((EKChannel)_eKchan.parentChannel).eKchanNum);

                _eKchan.LastValue = res;

                ReportTo?.Invoke(res);
            }
        }
    }
}
