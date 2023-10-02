using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART
{
    public sealed class LongAddress : IHartAddress
    {
        public byte[] AddressBytes { get; }
        public int Length => AddressBytes.Length;
        public LongAddress(byte[] address)
        {
            if (address.Length != 5) throw new ArgumentException("long address Must be 5 bytes long!");
            AddressBytes = address;
        }

    }
}
