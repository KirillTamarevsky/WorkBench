using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Enums
{
    [Flags]
    public enum ChannelCapabilitiesForScale : ulong
    {
        CanRead       = 1, // 0b0000000000000001,
        CanGenerate   = 2, // 0b0000000000000010,
    }
}
