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
        public int Module { get; set; }
        public int RangeNum { get; set; }
    }
}
