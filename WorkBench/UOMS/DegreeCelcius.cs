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
        public UOMType UOMType
        {
            get
            {
                return UOMType.Temperature;
            }
        }

        public string Name
        {
            get
            {
                return "°C";
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
