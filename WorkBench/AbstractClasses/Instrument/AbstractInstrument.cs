using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.Instrument
{
    public abstract class AbstractInstrument : IInstrument, IDisposable
    {
        /// <summary>
        /// канал связи
        /// </summary>
        public ITextCommunicator Communicator { get; internal set; }

        //public abstract IInstrumentChannel this[int i] { get; }

        public abstract IInstrumentChannel[] Channels { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public virtual bool IsOpen => Communicator != null && Communicator.IsOpen;
        public AbstractInstrument(ITextCommunicator communicator)
        {
            Communicator = communicator;
        }
        public abstract bool Close();
        public abstract bool Open();
        public virtual void Dispose()
        {
            Close();
        }

    }
}
