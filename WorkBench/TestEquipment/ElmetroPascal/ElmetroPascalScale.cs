using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public class ElmetroPascalScale : Scale
    {
        public ElmetroPascalScale(double min, double max, IUOM uom, int modulenum, int rangenum) : base(min, max, uom)
        {
            Module = modulenum;
            RangeNum = rangenum;
        }

        public int Module { get; }
        public int RangeNum { get; }

        public override string ToString()
        {
            return String.Format($"M{Module}:{base.ToString()}");
        }
    }
}
