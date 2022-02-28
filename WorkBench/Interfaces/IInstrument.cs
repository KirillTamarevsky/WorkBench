using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public interface IInstrument
    {
        IChannel this[int i] { get; }
        IChannel[] Channels { get; }
        string Name { get; }
        string Description { get; }
        bool Open();
        bool Close();
    }
}
