using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART
{
    public interface IHartAddress
    {
        byte[] AddressBytes { get; }
        int Length { get; }
    }
}
