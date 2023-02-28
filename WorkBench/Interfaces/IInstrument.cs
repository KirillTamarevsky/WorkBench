using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.Interfaces
{
    public interface IInstrument
    {
        //ITextCommunicator _communicator { get; }
        //IInstrumentChannel this[int i] { get; }
        IInstrumentChannel[] Channels { get; }
        string Name { get; }
        string Description { get; }
        Task<bool> Open();
        bool Close();
        bool IsOpen { get; }
    }
}
