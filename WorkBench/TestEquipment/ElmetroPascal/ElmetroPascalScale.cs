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
        public ElmetroPascalScale(double min, double max, IUOM uom) : base(min, max, uom)
        {
        }

        public int Module { get; set; }
        public int RangeNum { get; set; }

        public override string ToString()
        {
            return String.Format("M{0}:{1}", Module, base.ToString());
        }
    }
}
