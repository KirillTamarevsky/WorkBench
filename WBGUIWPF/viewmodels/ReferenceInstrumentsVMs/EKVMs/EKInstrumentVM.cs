using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench;
using WorkBench.Communicators;
using WorkBench.TestEquipment.EK;

namespace WBGUIWPF.viewmodels.ReferenceInstrumentsVMs.EKVMs
{
    public class EKInstrumentVM : BaseVM
    {
        EK EK;
#if DEBUG
        [Obsolete("Only for Design data", true)]
        public EKInstrumentVM()
        {
            EK = Factory.GetFakeEK("DesignTimePort");
        }
#endif
        public EKInstrumentVM( EK ek) 
        {
            EK = ek;
        }
        
    }
}
