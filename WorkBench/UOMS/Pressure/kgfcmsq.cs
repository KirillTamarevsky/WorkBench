using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.UOMS
{
    public class Kgfcmsq : PressureUOM
    {
        public override string Name => "kgf/cm²";

        public override double Factor => 98066.5;
    }
}
