using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_Zero_Command : HARTCommand
    {
        public override byte Number => 0;
        public override byte[] Data { get; } = new byte[0];
    }
}
