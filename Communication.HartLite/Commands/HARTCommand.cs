using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HARTCommand
    {
        public virtual byte Number { get; }
        public virtual byte[] Data { get; }
        public HARTCommand()
        {}
        public HARTCommand(byte number, byte[] data)
        {
            Number = number;
            Data = data;
        }
    }
}
