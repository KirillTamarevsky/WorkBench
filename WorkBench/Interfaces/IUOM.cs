using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.Interfaces
{
    public interface IUOM
    {
        UOMType UOMType { get; }
        string Name { get; }
        double Factor { get; }
    }
}
