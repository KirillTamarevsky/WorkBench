using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_Zero_Command : HARTCommand
    {
        public HART_Zero_Command() : this(20) { }

        public HART_Zero_Command(int preambleLength): this (0, preambleLength) { }

        public HART_Zero_Command(byte pollingAddress) : this(pollingAddress, 20) { }

        public HART_Zero_Command(byte pollingAddress, int preambleLength)
        {
            PreambleLength = preambleLength;
            Address = new ShortAddress(pollingAddress);
            CommandNumber = 0;
            Data = new byte[0];
            ResponseCode = new byte[0];
            StartDelimiter = 2;
        }
    }
}
