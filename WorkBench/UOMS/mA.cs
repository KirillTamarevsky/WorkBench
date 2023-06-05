using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOMS
{
    public class mA : IUOM
    {
        public UOMType UOMType => UOMType.Current;

        public string Name => "mA";

        public double Factor => 1;

        public override string ToString()
        {
            return Name;
        }
    }
}
