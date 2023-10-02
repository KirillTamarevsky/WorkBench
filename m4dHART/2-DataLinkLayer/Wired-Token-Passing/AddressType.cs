using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART._2_DataLinkLayer.Wired_Token_Passing
{
    /// <summary>
    /// Polling = 0 (1 Byte Address)
    /// Unique = 1 (5 Bytes Address)
    /// </summary>
    public enum AddressType
    {
        Polling = 0,
        Unique = 1
    }
}
