using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.Instrument;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.TestEquipment.EK.commands;

namespace WorkBench.TestEquipment.EVolta
{
    public class EVolta : AbstractInstrument
    {
        public EVolta(ITextCommunicator communicator) : base(communicator)
        {
        }

        private string SerialNumber = "НД";

        private IInstrumentChannel[] _channels;
        public override IInstrumentChannel[] Channels => _channels;

        public override string Name => "Элметро-Вольта";

        public override string Description => "Многофункциональный калибратор";

        public override Task<bool> Open()
        {
            return Task.Run(async () =>
            {

                //check if communicator available
                if (Communicator == null) return false;

                if (!Communicator.Open()) return false;

                var res = Communicator.QueryCommand("REMOTE");

                if (res != "OK")
                {
                    Communicator.Close();
                    return false;
                }

                SerialNumber = Communicator.QueryCommand("DEVICE?");

                //setup channels
                
                var ReaderChannel = new EVoltaReaderChannel() { parent = this };

                _channels = new IInstrumentChannel[] { ReaderChannel };

                return await base.Open();
            });
        }

        public override bool Close()
        {
            base.Close();

            Communicator.Close();

            Communicator.QueryCommand("LOCAL");

            return true;
        }

        internal override void EnqueueInstrumentCmd(InstrumentCmd instrumentCmd)
        {
            if (instrumentCmd is EVoltaCommandBase evoltaCmd)
            {
                evoltaCmd.Volta = this;
                base.EnqueueInstrumentCmd(instrumentCmd);
            }
        }

        public override string ToString()
        {
            return $"{Description} {Name} № {SerialNumber} ({Communicator})";
        }
    }
}
