using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Enums
{
    [Flags]
    enum ChannelCapabilities : ulong
    {
        None                    = 0, // 0b0000000000000000,
        CanRead020Current       = 1, // 0b0000000000000001,
        CanGenerate020Current   = 2, // 0b0000000000000010,
        CanRead420Current       = 4, // 0b0000000000000100,
        CanGenerate420Current   = 8, // 0b0000000000001000,

    }
}
