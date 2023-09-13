using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.UOMS.Pressure
{
    public class psi : PressureUOM
    {
        public override string Name => "psi";

        public override double Factor => 6894.757;
    }
}
