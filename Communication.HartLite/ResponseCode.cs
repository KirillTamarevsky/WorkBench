using System;

namespace Communication.HartLite
{
    public class ResponseCode
    {
        public byte FirstByte { get; }
        public byte SecondByte { get; }

        private ResponseCode(byte firstByte, byte secondByte)
        {
            FirstByte = firstByte;
            SecondByte = secondByte;
        }

        public static ResponseCode ToResponseCode(byte[] responseCodeBytes)
        {
            if (responseCodeBytes.Length != 2)
                throw new ArgumentException("ResponseCode needs exactly two bytes.", "responseCodeBytes");

            return new ResponseCode(responseCodeBytes[0], responseCodeBytes[1]);
        }

        public byte this[int i]
        {
            get
            {
                if(i != 0 && i != 1)
                    throw new IndexOutOfRangeException();

                return i == 0 ? FirstByte : SecondByte;
            }
        }
    }
}