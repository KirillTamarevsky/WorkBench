using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;

namespace WorkBench.UOM
{
    public class mbar : IUOM
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
                return "mbar";
            }
        }

        public double Factor
        {
            get
            {
                return 100;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
