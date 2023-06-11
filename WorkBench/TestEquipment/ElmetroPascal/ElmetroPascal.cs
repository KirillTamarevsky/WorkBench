using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public partial class ElmetroPascal : IInstrument, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("EPascalCommunication");

        internal ITextCommunicator Communicator { get; }
        public IInstrumentChannel[] Channels { get; }

        private bool _in_REMOTE_mode;
        public bool IsOpen => _in_REMOTE_mode;

        public ElmetroPascal(ITextCommunicator communicator)
        {
            logger.Info($"ElmetroPascal created for {communicator}");
            Communicator = communicator;
            Channels = new ElmetroPascalChannel[1];
            _in_REMOTE_mode = false;
        }

        public string Name => "Элметро-Паскаль";

        public string Description => "Калибратор-контроллер давления";

        public bool Close()
        {
            if (_in_REMOTE_mode)
            {
                lock (Communicator)
                {
                    SwitchToLOCALMode();

                    Communicator.Close();

                    return true;
                }
            }
            return false;
        }

        public bool Open()
        {
            lock (Communicator)
            {
                if (!IsOpen)
                {
                    if (Communicator.Open())
                    {
                        if (SwitchToREMOTEMode())
                        {
                            Communicator.QueryCommand("RANGE 1,1");
                            Thread.Sleep(2000);
                            var reply = Communicator.QueryCommand("ON_KEY_VENT");
                            Thread.Sleep(100);
                            if (reply == "VENT_OFF")
                            {
                                Communicator.QueryCommand("ON_KEY_VENT");
                                Thread.Sleep(100);
                            }
                            #region Setup Channels
                            var epChannel = new ElmetroPascalChannel(this);
                            
                            Channels[0] = epChannel;
                            
                            #endregion

                            return true;
                        }

                        Communicator.Close();
                    }
                }
            }

            return false;
        }
        public override string ToString()
        {
            return $"{Description} {Name}({Communicator})";
        }

        public void Dispose()
        {
            Close();
        }
    }
}
