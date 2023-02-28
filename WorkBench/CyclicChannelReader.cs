using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
namespace WorkBench
{
    public class CyclicChannelSpanReader
    {
        IInstrumentChannelSpanReader InstrumentChannelSpanReader;

        IUOM UOM;

        public event EventHandler<OneMeasure> OneMeasureReaded;

        Task cyclicReaderTask;

        CancellationTokenSource cancellationTokenSource;

        CancellationToken cancellationToken;

        bool Active;

        System.Threading.EventWaitHandle waitingForNewMeasure = new EventWaitHandle(false, EventResetMode.ManualReset);
        public CyclicChannelSpanReader(IInstrumentChannelSpanReader instrumentChannelSpanReader, IUOM uOM)
        {
            if (instrumentChannelSpanReader == null ) throw new ArgumentNullException(nameof(instrumentChannelSpanReader));
            
            if (uOM == null) throw new ArgumentNullException(nameof(uOM));

            InstrumentChannelSpanReader = instrumentChannelSpanReader;
            UOM = uOM;
        }

        public void Start()
        {
            //cancellationTokenSource = new CancellationTokenSource();
            //cancellationToken = cancellationTokenSource.Token;
            //cyclicReaderTask = Task.Run(() => CyclicReader()); //, cancellationToken, TaskCreationOptions.LongRunning);
            //cyclicReaderTask.ConfigureAwait(false);
            //cyclicReaderTask.Start();

            Active = true;
            InstrumentChannelSpanReader.Read(UOM, OnOneMeasureReaded);

        }
        public void Stop()
        {
            //cancellationTokenSource.Cancel();
            //cyclicReaderTask.Wait();
            //cancellationTokenSource.Dispose();

            Active = false;
        }

        //private void CyclicReader()
        //{
        //    do
        //    {
        //        waitingForNewMeasure.Reset();
        //        InstrumentChannelSpanReader.Read(UOM, OnOneMeasureReaded);
        //        waitingForNewMeasure.WaitOne(1500);

        //    } while (!cancellationToken.IsCancellationRequested);
        //}

        private void OnOneMeasureReaded(OneMeasure oneMeasure)
        {
            if (Active)
            {
                if (oneMeasure != null)
                {
                    OneMeasureReaded?.Invoke(this, oneMeasure);
                }
                //waitingForNewMeasure.Set();
                InstrumentChannelSpanReader.Read(UOM, OnOneMeasureReaded);
            }

        }
    }
}
