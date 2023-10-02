using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART._2_DataLinkLayer.Wired_Token_Passing
{
    public class Delimiter
    {
        internal AddressType AddressType { get; }
        internal int NumberOfExpansionBytes { get; }
        internal PhysicalLayerType PhysicalLayerType { get; }
        internal FrameType FrameType { get; }
        public Delimiter(AddressType addressType, int numberOfExpansionBytes, PhysicalLayerType physicalLayerType, FrameType frameType)
        {
            AddressType = addressType;
            NumberOfExpansionBytes = numberOfExpansionBytes;
            PhysicalLayerType = physicalLayerType;
            FrameType = frameType;
        }
        public byte ToByte()
        {
            return
                (byte)(

                ((int)AddressType << 7)
                | (NumberOfExpansionBytes << 5)
                | (int)PhysicalLayerType << 3
                | (int)FrameType
                );

        }
    }
}
