using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{
    public interface IReadUniqueIdetifierCommand
    {
        LongAddress LongAddress { get; }
    }
}
