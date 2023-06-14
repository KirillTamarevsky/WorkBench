using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.Calys150
{
    internal class Calys150ReaderChannel : IInstrumentChannel
    {
        internal string Query(string command) => parentCalys150.Query(command);
        internal Calys150 parentCalys150 { get; }
        internal ITextCommunicator Communicator => parentCalys150.Communicator;
        Calys150_Current_ReaderSpan_base ActiveSpan { get; set; }
        void Activate(Calys150ReaderChannel Calys150ReaderChannel) => parentCalys150.ActivateChannel(Calys150ReaderChannel);
        public int NUM => 1;

        public string Name => "измерительный канал";

        public IInstrumentChannelSpan[] AvailableSpans { get; }

        public Calys150ReaderChannel(Calys150 calys150)
        {
            parentCalys150 = calys150;
            AvailableSpans = new IInstrumentChannelSpan[] {
                                                                new Calys150_0Current_ReaderSpan(this)
                                                                , new Calys150_4Current_ReaderSpan(this)
                                                                , new Calys150_0_100Current_ReaderSpan(this)
                                                            };
        }
        internal void ActivateSpan(Calys150_Current_ReaderSpan_base calys150Span)
        {
            if (calys150Span.parentChannel != this) throw new ArgumentException("not my span");
            if (ActiveSpan != calys150Span)
            {
                Activate(this);
                var activationCmd = $"SENS{NUM}:{calys150Span.SetupStringCommand}";
                Communicator.SendLine(activationCmd);
                ActiveSpan = calys150Span;
            }
        }
        public override string ToString()
        {
            return $"{Name} №{NUM} (левый)";
        }
    }
}
