using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.TestEquipment.EK
{
    class EKCommandRead_0_20_mA : EKCommunicationCommand
    {
        EKchanNum _eKchanNum;

        EK _eK;

        public EKCommandRead_0_20_mA(EKchanNum eKchanNum, EK eK)
        {
            _eKchanNum = eKchanNum;

            _eK = eK;
        }

        public override void Execute()
        {
            var res = _eK.Read_0_20_Current_with_ext_pwr(_eKchanNum);

            _eK[_eKchanNum].lastValue = res;
        }
    }
}
