using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WBGUIWPF.Interfaces
{
    public interface IInstrumentCreator
    {
        IInstrument GetInstrument();
        bool CanGetInstrument { get; }
    }
}
