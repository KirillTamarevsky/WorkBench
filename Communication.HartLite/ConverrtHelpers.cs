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
        public static Single To_HART_Single(this byte[] data)
        {
            if (data.Length < 4) throw new ArgumentException("must be 4 bytes long");

            return BitConverter.ToSingle(data.Reverse().ToArray(), 0);
        }
        public static string HART_unpack_string(this byte[] bytes)
        {
            if (bytes.Length < 3) return string.Empty;
            var res = new StringBuilder();
            int chunks = bytes.Length / 3;
            for (int i = 0; i < chunks * 3; i += 3)
            {
                // 11111122 22223333 33444444
                var c1 = (bytes[0 + i] & 0b11111100) >> 2;
                var c2 = (bytes[0 + i] & 0b00000011) << 4 | (bytes[1 + i] & 0b11110000) >> 4;
                var c3 = (bytes[1 + i] & 0b00001111) << 2 | (bytes[2 + i] & 0b11000000) >> 6;
                var c4 = (bytes[2 + i] & 0b00111111);

                c1 &= 0b01111111;
                c2 &= 0b01111111;
                c3 &= 0b01111111;
                c4 &= 0b01111111;

                c1 |= ((c1 & 0b00100000) ^ 0b00100000) << 1;
                c2 |= ((c2 & 0b00100000) ^ 0b00100000) << 1;
                c3 |= ((c3 & 0b00100000) ^ 0b00100000) << 1;
                c4 |= ((c4 & 0b00100000) ^ 0b00100000) << 1;


                var chunk = System.Text.Encoding.ASCII.GetString(new byte[] { (byte)c1, (byte)c2, (byte)c3, (byte)c4 });

                res.Append(chunk);

            }
            return res.ToString();
        }
        public static byte[] HART_pack_string(this string str)
        {
            str = str.ToUpper();
            if (str.Length < 4) return new byte[] { };
            List<byte> res = new List<byte>();
            int chunks = str.Length / 4;
            for (int i = 0; i < chunks * 4; i += 4)
            {
                // 11111122 22223333 33444444
                byte b1 = (byte)((str[0 + i] & 0b00111111) << 2 | (str[1 + i] & 0b00110000) >> 4);
                byte b2 = (byte)((str[1 + i] & 0b00001111) << 4 | (str[2 + i] & 0b00111100) >> 2);
                byte b3 = (byte)((str[2 + i] & 0b00000011) << 6 | (str[3 + i] & 0b00111111));

                res.Add(b1);
                res.Add(b2);
                res.Add(b3);
            }
            return res.ToArray();
        }
        public static DateTime HART_unpack_DateCode(this byte[] bytes)
        {
            if (bytes.Length != 3) return new DateTime(1900, 1, 1);
            int day = bytes[0];
            int month = bytes[1];
            int year = bytes[2] + 1900;
            return new DateTime(year, month, day);
        }
    }
}
