using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkBench.Communicators;
using WorkBench.Interfaces;
using WorkBench.UOM;

namespace WorkBench.TestEquipment.EK
{
    public partial class EK :  IInstrument, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EK));

        private IChannel[] _channels;

        /// <summary>
        /// канал связи
        /// </summary>
        internal ICommunicator _communicator;

        /// <summary>
        /// задача по циклическому опросу прибора поступающими в очередь запросами
        /// </summary>
        Task CyclicReaderTask;
        
        CancellationTokenSource tokenSource;
        /// <summary>
        /// токен для прекращения задачи циклического опроса прибора
        /// </summary>
        CancellationToken token;

        /// <summary>
        /// <see langword="true"/> если прибор переведен в режим удаленной работы
        /// </summary>
        internal bool _in_REMOTE_mode;
        
        public EK(ICommunicator communicator)
        {
            logger.Info("EK created for " + communicator.ToString());
            
            #region Setup Channels
            _channels = new EKChannelBase[8];
            this[EKchanNum._1] = new EKChannelBase();
            this[EKchanNum._2] = new EKChannelBase();
            this[EKchanNum._3] = new EKChannelBase();
            this[EKchanNum._4] = new EKChannelBase();
            this[EKchanNum._5] = new EKChannelBase();
            this[EKchanNum._6] = new EKChannelBase();
            this[EKchanNum._7] = new EKChannelBase();
            this[EKchanNum._8] = new EKChannelBase();
            #endregion
            
            _communicator = communicator;
            
            _in_REMOTE_mode = false;

        }

        public IChannel[] Channels
        {
            get
            {
                return _channels;
            }
        }

        public IChannel this[int i]
        {
            get
            {
                if (i > 0 | i < 9)
                {
                    return _channels[i];
                }

                return null;
            }
        }

        public EKChannelBase this[EKchanNum eKchanNum]
        {
            set
            {
                value.NUM = (int)eKchanNum;

                value.parent = this;
                
                _channels[(int)eKchanNum - 1] = value;
            }
            get
            {
                return (EKChannelBase)_channels[(int)eKchanNum - 1];
            }
        }

        /// <summary>
        /// название типа прибора
        /// </summary>
        public string Name 
        {
            get
            {
                return "Элметро-Кельвин";
            }
            set
            { }
        }

        /// <summary>
        /// Наименование прибора
        /// </summary>
        public string Description
        {
            get
            {
                return "Мультиметр многоканальный";
            }
        }
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

                logger.Debug(string.Format("EK.Open( {0} ) ", _communicator.ToString() ));
            
                bool communicatorOpened = _communicator.Open();

                logger.Debug(string.Format("EK.Open( {0} ) communicatorOpened = {1}", _communicator.ToString(), communicatorOpened));
            
                if (communicatorOpened)
                {
                    string answer = _communicator.QueryCommand("REMOTE");
                
                    ekanswered = answer == "R";
                
                    if (ekanswered)
                    {
                        _in_REMOTE_mode = true; // если пришел ответ, то прибор першел в режим удаленной работы. необходимо перевести в локальный режим по окончании сеанса связи

                        tokenSource = new CancellationTokenSource();
                    
                        token = tokenSource.Token;
                    
                        CyclicReaderTask = new Task(() => CyclicReader(token), token);
                    
                        CyclicReaderTask.Start();
                    }
                    else
                    {
                        log4net.LogManager.GetLogger("Communication").Debug("communicator close() " + _communicator.ToString());
                        _communicator.Close();
                    }
                }
                return ekanswered;
            }
            return true;
        }

        public bool Close()
        {
            try
            {
                if (CyclicReaderTask != null && !CyclicReaderTask.IsCompleted)
                {
                    tokenSource.Cancel();
                    CyclicReaderTask.Wait();
                    tokenSource.Dispose();
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                logger.Debug("Task Cancelled in Close() for " + Description + " " + Name + " on " + _communicator.ToString());
            }
            if (_in_REMOTE_mode)
            {
                _communicator.QueryCommand("LOCAL");
                _in_REMOTE_mode = false;
            }
            return _communicator.Close();
        }


        
        public ConcurrentQueue<EKCommunicationCommand> eKCommunicationCommands = new ConcurrentQueue<EKCommunicationCommand>();

        void CyclicReader (CancellationToken ct)
        {
            EKCommunicationCommand eKCommunicationCommand;
            do
            {
                if (!ct.IsCancellationRequested & eKCommunicationCommands.TryDequeue(out eKCommunicationCommand))
                {
                    eKCommunicationCommand.Execute();
                }
            } while (!ct.IsCancellationRequested);
        }

        public void Dispose()
        {
            Close();
        }

        public override string ToString()
        {
            return String.Format("{0} {1}({2})", Description, Name, _communicator.ToString()); 
        }

    }
}
