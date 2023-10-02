using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART._2_DataLinkLayer
{
    /// <summary>
    /// Asyncronous = 0 (FSK)
    /// Syncronous = 1 (i.e. PSK)
    /// </summary>
    public enum PhysicalLayerType
    {
        Asyncronous = 0,
        Syncronous = 1
    }
}
