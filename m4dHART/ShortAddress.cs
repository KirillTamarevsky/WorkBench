using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART
{
    public sealed class ShortAddress : IHartAddress
    {
        public byte[] AddressBytes { get; }
        public int Length => AddressBytes.Length;
        public ShortAddress( byte address)
        {
            AddressBytes = new byte[] { address };
        }

    }
}
