using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using WorkBench.AbstractClasses.Instrument;
using WorkBench.Communicators;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WorkBench.TestEquipment.EK
{
    public partial class EK : IInstrument, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EK));

        public IInstrumentChannel[] Channels { get; }

        /// <summary>
        /// <see langword="true"/> если прибор переведен в режим удаленной работы
        /// </summary>
        internal bool _in_REMOTE_mode;

        public bool IsOpen => _in_REMOTE_mode;

        public ITextCommunicator Communicator { get; }

        public string SerialNumber { get; }

        public EK(ITextCommunicator communicator)
        {
            logger.Info($"EK created for {communicator}");
            
            #region Setup Channels
            Channels = new EKChannel[] { 
                                        new EKChannel(this, EKchanNum._1),
                                        new EKChannel(this, EKchanNum._2),
                                        new EKChannel(this, EKchanNum._3),
                                        new EKChannel(this, EKchanNum._4),
                                        new EKChannel(this, EKchanNum._5),
                                        new EKChannel(this, EKchanNum._6),
                                        new EKChannel(this, EKchanNum._7),
                                        new EKChannel(this, EKchanNum._8)
                                        };
            #endregion

            Communicator = communicator;
            
            _in_REMOTE_mode = false;

            SetActiveChannel(EKchanNum.None);
        }

        /// <summary>
        /// название типа прибора
        /// </summary>
        public string Name => "Элметро-Кельвин";

        /// <summary>
        /// Наименование прибора
        /// </summary>
        public string Description => "Мультиметр многоканальный";
        /// <summary>
        /// устанавливает связь с прибором.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> в случае успеха
        /// </returns>
        public bool Open()
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
                    var answerStatus = Communicator.QueryCommand("REMOTE", out string answer);

                    ekanswered = answer == "R";

                    if (ekanswered)
                    {
                        _in_REMOTE_mode = true; // если пришел ответ, то прибор першел в режим удаленной работы. необходимо перевести в локальный режим по окончании сеанса связи
                        SetActiveChannel(EKchanNum._1);
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
        }

        public bool Close()
        {
            if (_in_REMOTE_mode)
            {
                lock (Communicator)
                {
                    Communicator.QueryCommand("LOCAL", out string reoply);
                    _in_REMOTE_mode = false;
                    SetActiveChannel(EKchanNum.None);
                }
            }
            
            return Communicator.Close();
        }
        public void Dispose() => Close();

        public override string ToString() => $"{Description} {Name}({Communicator})";

    }
}
