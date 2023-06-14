using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.Calys150
{
    internal class Calys150ReaderChannel : IInstrumentChannel
    {
        internal string Query(string command) => parentCalys150.Query(command);
        private Calys150 parentCalys150 { get; }
        public int NUM => 1;

        public string Name => "измерительный канал";

        public IInstrumentChannelSpan[] AvailableSpans => new IInstrumentChannelSpan[] { new Calys150_4_20Current_ReaderSpan(this)
                                                                                         , new Calys150_0Current_ReaderSpan(this) 
                                                                                         , new Calys150_4Current_ReaderSpan(this)
                                                                                         , new Calys150_0_100Current_ReaderSpan(this)
                                                                                        };

        public Calys150ReaderChannel(Calys150 calys150)
        {
            parentCalys150 = calys150;
        }

        public override string ToString()
        {
            return $"канал {NUM} (левый)";
        }
    }
}
