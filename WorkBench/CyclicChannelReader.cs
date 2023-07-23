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
    public class CyclicChannelSpanReader : ICyclicChannelSpanReader
    {
        readonly IInstrumentChannelSpanReader InstrumentChannelSpanReader;
        readonly IUOM UOM;

        public event EventHandler<OneMeasure> OneMeasureReaded;

        Task cyclicReaderTask;

        bool Active;

        public CyclicChannelSpanReader(IInstrumentChannelSpanReader instrumentChannelSpanReader, IUOM uOM)
        {
            InstrumentChannelSpanReader = instrumentChannelSpanReader ?? throw new ArgumentNullException(nameof(instrumentChannelSpanReader));
            UOM = uOM ?? throw new ArgumentNullException(nameof(uOM));
        }

        public void Start()
        {
            if (cyclicReaderTask != null) throw new Exception("cyclic reader task already exists");
            Active = true;
            cyclicReaderTask = Task.Factory.StartNew(() =>
            {
                while (Active)
                {
                    var oneMeasure = InstrumentChannelSpanReader.Read(UOM);
                    OneMeasureReaded?.Invoke(this, oneMeasure);
                }
            });

        }
        public void Stop()
        {
            Active = false;
            log4net.LogManager.GetLogger("Communication").Debug("stopping cyclic reader");
            cyclicReaderTask.Wait();
            log4net.LogManager.GetLogger("Communication").Debug("cyclic reader stopped");
            cyclicReaderTask.Dispose();
            cyclicReaderTask = null;
        }

    }
}
