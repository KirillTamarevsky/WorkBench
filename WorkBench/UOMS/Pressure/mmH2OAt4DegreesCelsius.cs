using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.UOMS.Pressure
{
    public class mmH2OAt4DegreesCelsius : PressureUOM
    {
        public override string Name => "mmH2O@4°C";

        public override double Factor => 9.806378 ;
    }
}
