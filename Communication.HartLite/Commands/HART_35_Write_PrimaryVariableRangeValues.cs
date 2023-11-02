using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_35_Write_PrimaryVariableRangeValues : HARTCommand
    {
        public override byte Number => 35;
        public override byte[] Data => ToByteArray();
        byte _unitCode { get; }
        double _PVUpperRangeValue { get; }
        double _PVLowerRangeValue { get; }
        public HART_35_Write_PrimaryVariableRangeValues(byte unitCode, double PVLowerRangeValue, double PVUpperRangeValue)
        {
            _unitCode = unitCode;
            _PVLowerRangeValue = PVLowerRangeValue;
            _PVUpperRangeValue = PVUpperRangeValue;
        }
        private byte[] ToByteArray()
        {
            var upper = ((float)_PVUpperRangeValue).Single_to_HART_bytearray();
            var lower = ((float)_PVLowerRangeValue).Single_to_HART_bytearray();
            var res = new byte[9] {
                _unitCode,
                upper[0],
                upper[1],
                upper[2],
                upper[3],
                lower[0],
                lower[1],
                lower[2],
                lower[3]
            };
            return res;
        }
    }
}
