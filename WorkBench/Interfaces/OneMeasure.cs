using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public class OneMeasure
    {
        public OneMeasure(double value, IUOM uom, DateTime timestamp)
        {
            Value = value;
            UOM = uom;
            TimeStamp = timestamp;
        }
        public OneMeasure(double value, IUOM uom): this(value, uom, DateTime.Now)  {}
        public double Value { get; }
        public IUOM UOM { get; }
        public DateTime TimeStamp { get; }

        public bool TryConvertTo(IUOM uom, out OneMeasure result)
        {
            if (UOM.UOMType != uom.UOMType)
            {
                result = null;
                return false;
            }
            var resultValue = Value * (UOM.Factor / uom.Factor);
            result = new OneMeasure(resultValue, uom, DateTime.Now);
            return true;
        }
        
        public static bool operator > (OneMeasure left, OneMeasure right)
        {
            if (left.UOM.UOMType != right.UOM.UOMType) throw new Exception($"left UOM {left.UOM.UOMType} != right UOM {right.UOM.UOMType}");

            var rightConvertedToLeftUOM = right.Value * right.UOM.Factor / left.UOM.Factor;
            return left.Value > rightConvertedToLeftUOM;
        }
        public static bool operator < (OneMeasure left, OneMeasure right)
        {
            return !(left > right);
        }
        public override string ToString()
        {
            return string.Format("{0} {1}", Value, UOM.Name);
        }
    }
}
