﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Communicators;
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
                            Query("RANGE 1,1", out string reply);
                            Thread.Sleep(2000);
                            Query("ON_KEY_VENT", out reply);
                            Thread.Sleep(100);
                            if (reply == "VENT_OFF")
                            {
                                Query("ON_KEY_VENT", out reply);
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

        internal TextCommunicatorQueryCommandStatus Query(string cmd, out string answer)
        {
            return Communicator.QueryCommand(cmd, out answer, null);
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
