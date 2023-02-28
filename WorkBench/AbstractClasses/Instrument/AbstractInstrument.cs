using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.AbstractClasses.InstrumentCommand;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.Instrument
{
    public abstract class AbstractInstrument : IInstrument, IDisposable
    {
        /// <summary>
        /// канал связи
        /// </summary>
        public ITextCommunicator Communicator { get; internal set; }

        //public abstract IInstrumentChannel this[int i] { get; }

        public abstract IInstrumentChannel[] Channels { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }

        public virtual bool IsOpen => Communicator != null && Communicator.IsOpen;

        public AbstractInstrument(ITextCommunicator communicator)
        {
            Communicator = communicator;
        }

        public virtual bool Close()
        {
            StopCommandQueueProcessor();
            return true;
        }

        public virtual Task<bool> Open()
        {
            return Task.Run(() =>
            {
                StartCommandQueueProcessor();
                return true;
            });
        }
        public virtual void Dispose()
        {
            Close();
        }

        #region CommandQueueProcessor
        /// <summary>
        /// задача по циклическому опросу прибора поступающими в очередь запросами
        /// </summary>
        Task CommandQueueProcessorTask;

        CancellationTokenSource tokenSource;

        /// <summary>
        /// токен для прекращения задачи циклического опроса прибора
        /// </summary>
        CancellationToken token;

        private readonly ConcurrentQueue<InstrumentCmd> InstrumentCommandsQueue = new ConcurrentQueue<InstrumentCmd>();

        internal virtual void EnqueueInstrumentCmd(InstrumentCmd instrumentCmd)
        {
            InstrumentCommandsQueue.Enqueue(instrumentCmd);
        }

        void CommandQueueProcessor(CancellationToken ct)
        {
            do
            {
                if (!ct.IsCancellationRequested & InstrumentCommandsQueue.TryDequeue(out InstrumentCmd InstrumentCommand))
                {
                    try
                    {
                        InstrumentCommand.Execute();
                    }
                    catch (Exception e)
                    {
                        log4net.LogManager.GetLogger("AbstractInstrumentCommandQueueProcessor").Error($"Exception {e}");
                    }
                }
            } while (!ct.IsCancellationRequested);

        }

        internal void StartCommandQueueProcessor()
        {
            tokenSource = new CancellationTokenSource();

            token = tokenSource.Token;

            //CyclicReaderTask = Task.Run(() => new Task(() => CyclicReader(token), token, TaskCreationOptions.LongRunning));

            CommandQueueProcessorTask = new Task(() => CommandQueueProcessor(token), token);

            CommandQueueProcessorTask.Start();

            log4net.LogManager.GetLogger("AbstractInstrument").Debug(
                $"Cyclic Reader Task started for {Communicator} on {this.GetType()}"
                );
        }

        internal void StopCommandQueueProcessor()
        {
            log4net.LogManager.GetLogger("AbstractInstrument").Debug(
                       string.Format("Cyclic Reader Task stopping for ( {0} ) on {1}", Communicator.ToString(), this.GetType().ToString())
                       );
            try
            {
                if (CommandQueueProcessorTask != null && !CommandQueueProcessorTask.IsCompleted)
                {
                    tokenSource.Cancel();
                    CommandQueueProcessorTask.Wait();
                    tokenSource.Dispose();
                    while(InstrumentCommandsQueue.Count > 0)
                    {
                        InstrumentCommandsQueue.TryDequeue(out InstrumentCmd cmd);
                    }

                }

            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                log4net.LogManager.GetLogger("AbstractInstrument").Debug(
                    string.Format(
                    "Task TaskCanceledException in Close( {0} ) for {1} {2} ",
                    Communicator.ToString(),
                    Description,
                    Name
                    ));
            }
        }
        
        #endregion
    }
}
