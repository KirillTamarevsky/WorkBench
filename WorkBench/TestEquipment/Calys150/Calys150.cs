using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.Calys150
{
    public class Calys150 : IInstrument
    {
        public IInstrumentChannel[] Channels => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        public bool Close()
        {
            throw new NotImplementedException();
        }

        public bool Open()
        {
            throw new NotImplementedException();
        }
    }
}
