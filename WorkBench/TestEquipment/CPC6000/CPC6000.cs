using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using WorkBench.Communicators;
using WorkBench.Interfaces;
using WorkBench.UOM;
using System.Collections.Concurrent;

namespace WorkBench.TestEquipment.CPC6000
{
    public partial class CPC6000 : IInstrument, IDisposable
    {
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CPC6000));

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

        public CPC6000(ICommunicator communicator)
        {
            logger.Info("CPC created for " + communicator.ToString());

            #region Setup Channels
            _channels = new CPC6000ChannelBase[2];
            this[CPC6000ChannelNumber.A] = new CPC6000ChannelBase();
            this[CPC6000ChannelNumber.B] = new CPC6000ChannelBase();
            //this[CPC6000ChannelNumber.Baro] = new CPC6000ChannelBase();
            #endregion

            _communicator = communicator;
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
                if (i > 0 | i < 3)
                {
                    return _channels[i];
                }

                return null;
            }
        }

        public CPC6000ChannelBase this[CPC6000ChannelNumber chanNum]
        {
            set
            {
                value.NUM = (int)chanNum;

                value.parent = this;

                _channels[(int)chanNum - 1] = value;
            }
            get
            {
                return (CPC6000ChannelBase)_channels[(int)chanNum - 1];
            }
        }

        /// <summary>
        /// название типа прибора
        /// </summary>
        public string Name 
        { 
            get 
            {
                return "CPC6000"; 
            } 
        }

        /// <summary>
        /// Наименование прибора
        /// </summary>
        public string Description 
        { 
            get
            {
                return "Калибратор давления"; 
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
            if (!_connected)
            {

                bool cpcanswered = false;

                var logger = log4net.LogManager.GetLogger("CPC6000Communication");
            
                logger.Debug(string.Format( "CPC6000.Open( {0} ) ", _communicator.ToString() ));

                bool communicatorOpened = _communicator.Open();

                logger.Debug(string.Format("CPC6000.Open( {0} ) communicatorOpened = {1}", _communicator.ToString(), communicatorOpened));

                if (communicatorOpened)
                {
                    string answer = Query("ID?");

                    string[] answerParts;

                    if (answer != "")
                    {
                        answerParts = answer.Split(new char[] { ',' });
                        for (int i = 0; i < answerParts.Length; i++)
                        {
                            answerParts[i] = answerParts[i].Trim();
                        }

                        if (answerParts[0] == "MENSOR" && answerParts[1] == "600")
                        {
                            cpcanswered = true;
                            _serialNo = answerParts[2];
                            _swVer = answerParts[3];
                        }


                    }

                    if (cpcanswered)
                    {
                        _connected = true; // если пришел ответ, то прибор першел в режим удаленной работы. необходимо перевести в локальный режим по окончании сеанса связи

                        tokenSource = new CancellationTokenSource();

                        token = tokenSource.Token;

                        //CyclicReaderTask = Task.Run(() => new Task(() => CyclicReader(token), token, TaskCreationOptions.LongRunning));

                        CyclicReaderTask = new Task(() => CyclicReader(token), token);

                        CyclicReaderTask.Start();

                        log4net.LogManager.GetLogger("CPC6000Communication").Debug(
                            string.Format("CPC6000 answered( {0} ), Cyclic Reader Task started ", _communicator.ToString())
                            );

                        SetUOMOnChannel( CPC6000ChannelNumber.A, "kPa");
                        SetUOMOnChannel( CPC6000ChannelNumber.B, "kPa");
                    }
                    else
                    {
                        log4net.LogManager.GetLogger("CPC6000Communication").Debug(
                            string.Format("communicator close( {0} ) " , _communicator.ToString() )
                            );
                        _communicator.Close();
                    }
                }
            return cpcanswered;
            }

            return true;
        }
        public bool Close()
        {
            log4net.LogManager.GetLogger("CPC6000Communication").Debug(
                       string.Format("CPC closing communicator ( {0} ) ", _communicator.ToString())
                       );
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
                logger.Debug(string.Format(
                    "Task TaskCanceledException in Close( {0} ) for {1} {2} ",
                    _communicator.ToString(),
                    Description,
                    Name
                    ));
            }
            if (_connected)
            {
                _connected = false;
            }
            return _communicator.Close();
        }

        public ConcurrentQueue<CPC6000cmd> cPC6000CommunicationCommands = new ConcurrentQueue<CPC6000cmd>();

         void CyclicReader(CancellationToken ct)
        {
            CPC6000cmd cPC6000CommunicationCommand;
            do
            {
                if (!ct.IsCancellationRequested & cPC6000CommunicationCommands.TryDequeue(out cPC6000CommunicationCommand))
                {
                    cPC6000CommunicationCommand.Execute();
                }
            } while (!ct.IsCancellationRequested);

        }

        public void Dispose()
        {
            Close();
        }

        public override string ToString()
        {
            return String.Format("{0} {1} ( {2} )", Description, Name, _communicator.ToString());
        }
    }
}