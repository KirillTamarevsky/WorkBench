using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOMS
{
    public abstract class PressureUOM : IUOM
    {
        public UOMType UOMType => UOMType.Pressure;

        public abstract string Name { get; }

        public abstract double Factor { get; }

        public override string ToString()
        {
            return Name;
        }

    }
}
