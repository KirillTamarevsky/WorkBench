using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOMS
{
    public class DegreeCelcius : IUOM
    {
        public UOMType UOMType => UOMType.Temperature;

        public string Name => "°C";

        public double Factor => 1;

        public override string ToString()
        {
            return Name;
        }
    }
}
