using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOMS
{
    public class Mbar : PressureUOM
    {
        public override string Name => "mbar";

        public override double Factor => 100;

    }
}
