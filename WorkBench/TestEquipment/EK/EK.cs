using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkBench.AbstractClasses.Instrument;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Communicators;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EK
{
    public partial class EK : AbstractInstrument //  IInstrument, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EK));

        private readonly IInstrumentChannel[] _channels;
        public override IInstrumentChannel[] Channels
        {
            get
            {
                return _channels;
            }
        }
        
        /// <summary>
        /// <see langword="true"/> если прибор переведен в режим удаленной работы
        /// </summary>
        internal bool _in_REMOTE_mode;

        public override bool IsOpen => base.IsOpen && _in_REMOTE_mode;

        public EK(ITextCommunicator communicator) : base(communicator)
        {
            logger.Info($"EK created for {communicator}");
            
            #region Setup Channels
            _channels = new EKChannel[8];
            this[EKchanNum._1] = new EKChannel();
            this[EKchanNum._2] = new EKChannel();
            this[EKchanNum._3] = new EKChannel();
            this[EKchanNum._4] = new EKChannel();
            this[EKchanNum._5] = new EKChannel();
            this[EKchanNum._6] = new EKChannel();
            this[EKchanNum._7] = new EKChannel();
            this[EKchanNum._8] = new EKChannel();
            #endregion
            
            //_communicator = communicator;
            
            _in_REMOTE_mode = false;

        }

        public IInstrumentChannel this[int i]
        {
            get
            {
                if (i < 1 | i > 8)
                {
                    throw new ArgumentOutOfRangeException($"номер канала ({i}) вне допустимого диапазона (1...8)");
                }

               return _channels[i];
            }
        }

        public EKChannel this[EKchanNum eKchanNum]
        {
            set
            {
                value.NUM = (int)eKchanNum;

                value.parent = this;

                value.eKchanNum = eKchanNum;

                _channels[(int)eKchanNum - 1] = value;
            }
            get
            {
                return (EKChannel)_channels[(int)eKchanNum - 1];
            }
        }

        /// <summary>
        /// название типа прибора
        /// </summary>
        public override string Name 
        {
            get => "Элметро-Кельвин";
        }

        /// <summary>
        /// Наименование прибора
        /// </summary>
        public override string Description
        {
            get => "Мультиметр многоканальный";
        }
        /// <summary>
        /// устанавливает связь с прибором.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> в случае успеха
        /// </returns>
        public override async Task<bool> Open()
        {
            return await Task.Run(async () =>
            {

                if (!_in_REMOTE_mode)
                {

                    bool ekanswered = false;

                    var logger = log4net.LogManager.GetLogger("Communication");

                    logger.Debug(string.Format("EK.Open( {0} ) ", Communicator.ToString()));

                    bool communicatorOpened = Communicator.Open();

                    logger.Debug(string.Format("EK.Open( {0} ) communicatorOpened = {1}", Communicator.ToString(), communicatorOpened));

                    if (communicatorOpened)
                    {
                        string answer = Communicator.QueryCommand("REMOTE");

                        ekanswered = answer == "R";

                        if (ekanswered)
                        {
                            _in_REMOTE_mode = true; // если пришел ответ, то прибор першел в режим удаленной работы. необходимо перевести в локальный режим по окончании сеанса связи

                            await base.Open();
                        }
                        else
                        {
                            log4net.LogManager.GetLogger("Communication").Debug("communicator close() " + Communicator.ToString());
                            Communicator.Close();
                        }
                    }
                    return ekanswered;
                }
                return Communicator.IsOpen;
            });
        }

        public override bool Close()
        {
            base.Close();
            
            if (_in_REMOTE_mode)
            {
                Communicator.QueryCommand("LOCAL");
                _in_REMOTE_mode = false;
            }
            
            return Communicator.Close();
        }
        public override void Dispose()
        {
            base.Close();
        }

        public override string ToString()
        {
            return String.Format("{0} {1}({2})", Description, Name, Communicator.ToString()); 
        }

    }
}
