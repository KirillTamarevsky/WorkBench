using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOMS
{
    public class Pa : PressureUOM
    {
        public override string Name => "Pa";

        public override double Factor => 1;

    }
}
