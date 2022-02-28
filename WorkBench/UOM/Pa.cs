using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOM
{
    public class Pa : IUOM
    {
        public UOMType UOMType
        {
            get
            {
                return UOMType.Pressure;
            }
        }

        public string Name
        {
            get
            {
                return "Pa";
            }
        }

        public double Factor
        {
            get
            {
                return 1;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
