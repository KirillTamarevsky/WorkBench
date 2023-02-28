using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.AbstractClasses.Instrument;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public partial class ElmetroPascal : AbstractInstrument // IInstrument, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ElmetroPascal));

        IInstrumentChannel[] _channels = new ElmetroPascalChannel[1];

        private bool _in_REMOTE_mode;
        public override bool IsOpen => base.IsOpen && _in_REMOTE_mode;

        public ElmetroPascal(ITextCommunicator communicator): base(communicator)
        {
            logger.Info($"ElmetroPascal created for {communicator}");

            _in_REMOTE_mode = false;

        }
        public IInstrumentChannel this[int i]
        {
            get
            {
                if (i != 0) throw new ArgumentOutOfRangeException($"номер канала {i} вне допустимого диапазона");

                return _channels[i];
            }
        }

        public override IInstrumentChannel[] Channels
        {
            get
            {
                return _channels;
            }
        }

        public override string Name
        {
            get => "Элметро-Паскаль";
        }

        public override string Description
        {
            get => "Калибратор-контроллер давления";
        }

        public override bool Close()
        {
            base.Close();

            SwitchToLOCALMode();

            Communicator.Close();

            return true;
        }

        public override async Task<bool> Open()
        {
             return await Task.Run(async () =>
            {

                if (Communicator.Open())
                {
                    await base.Open();

                    if (SwitchToREMOTEMode())
                    {

                        #region Setup Channels
                        var epch = new ElmetroPascalChannel(this);

                        //epch._parent = this;

                        _channels[0] = epch;

                        #endregion

                        return true;
                    }

                    base.Close();
                    Communicator.Close();
                }

                return false;
            });

        }
        public override string ToString()
        {
            return String.Format("{0} {1}({2})", Description, Name, Communicator.ToString()); 
        }
    }
}
