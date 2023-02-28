using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOMS
{
    public class bar : PressureUOM
    {
        public override string Name => "bar";

        public override double Factor => 100000;
    }

}
