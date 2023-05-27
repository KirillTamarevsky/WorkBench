using System;

namespace WorkBench.Interfaces
{
    public interface ICyclicChannelSpanReader
    {
        event EventHandler<OneMeasure> OneMeasureReaded;
        void Start();
        void Stop();
    }
}