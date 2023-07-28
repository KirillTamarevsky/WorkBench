using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.TestEquipment.CPC6000
{
    public enum CPC6000ChannelNumber
    {
        Unknown = 0,
        A = 1,
        B = 2,
        Baro =3,
    }

    internal enum CPC6000PressureModule
    {
        Unknown = 0,
        Primary = 1,
        Secondary = 2,
        Barometer = 3
    }
}
