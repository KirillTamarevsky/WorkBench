using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite
{
    public static class ConverrtHelpers
    {
        public static byte[] Single_to_HART_bytearray(this Single number)
        {
            var bytes = BitConverter.GetBytes(number);
            var data = bytes.Reverse().ToArray();
            return data;
        }
    }
}
