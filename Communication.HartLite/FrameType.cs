using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART._2_DataLinkLayer.Wired_Token_Passing
{
    /// <summary>
    /// BACK = 1 (Burst Frame)
    /// STX = 2 (Master to Field Device)
    /// ACK = 6 (Field Device to Master)
    /// </summary>
    public enum FrameType
    {
        BACK = 1, // Burst Frame
        STX = 2,  // Master to Field Device
        ACK = 6,   // Field Device to Master
        UNKNOWN = 0b111 
    }
}
