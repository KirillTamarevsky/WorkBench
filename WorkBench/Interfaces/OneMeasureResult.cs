using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public class OneMeasureResult
    {
        private double _value;
        public double Value { set { _value = value; } get { return _value; } }
        public IUOM UOM { get; set; }
        public DateTime dateTimeOfMeasurement { get; set; }
    }
}
