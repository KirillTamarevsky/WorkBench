using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench
{
    public class Scale
    {
        public Scale(double min, double max, IUOM uom)
        {
            Min = min;
            Max = max;
            UOM = uom;
        }
        public double Min { get;}
        public double Max { get ;}
        public IUOM UOM { get; }
        public override string ToString()
        {
            return $"{Min} ... {Max} {UOM.Name}";
        }
    }
}
