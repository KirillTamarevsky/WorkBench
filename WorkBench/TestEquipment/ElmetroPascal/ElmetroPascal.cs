using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.TestEquipment.ElmetroPascal
{
    public partial class ElmetroPascal : IInstrument, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ElmetroPascal));

        IChannel[] _channels = new ElmetroPascalChannel[1];

        internal ICommunicator _communicator;

        bool _in_REMOTE_mode;

        public ElmetroPascal(ICommunicator communicator)
        {
            logger.Info("ElmetroPascal created for " + communicator.ToString());

            _communicator = communicator;

            _in_REMOTE_mode = false;

            this[0] = new ElmetroPascalChannel();

        }
        public IChannel this[int i]
        {
            set
            {
                if (i == 0)
                {
                    ((ElmetroPascalChannel)value).parent = this;
                    _channels[i] = value;
                }
            }
            get
            {
                if (i == 0)
                {
                    return _channels[i];
                }
                return null;
            }
        }

        public IChannel[] Channels
        {
            get
            {
                return _channels;
            }
        }

        public string Name
        {
            get
            {
                return "Элметро-Кельвин";
            }
        }

        public string Description
        {
            get
            {
                return "Калибратор-контроллер давления";
            }
        }

        public bool Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool Open()
        {
            #region Setup Channels
            
            #endregion

            return true;
        }
    }
}
