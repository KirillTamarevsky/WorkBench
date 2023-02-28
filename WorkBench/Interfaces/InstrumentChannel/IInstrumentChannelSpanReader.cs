using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.Interfaces.InstrumentChannel
{
    public interface IInstrumentChannelSpanReader
    {
        void Read(IUOM uom, Action<OneMeasure> reportTo);
        //bool CyclicRead { get; set; }

        //event NewValueReaded NewValueReaded;
        //OneMeasure LastValue { get; }
    }

}
