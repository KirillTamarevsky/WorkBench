using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public class Scale
    {
        private double _min;
        public double Min { set { _min = value; } get { return _min; } }

        private double _max;
        public double Max { set { _max = value; } get { return _max; } }

        public IUOM UOM { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ... {1} {2}", _min, _max, UOM.Name );
        }
    }
}
