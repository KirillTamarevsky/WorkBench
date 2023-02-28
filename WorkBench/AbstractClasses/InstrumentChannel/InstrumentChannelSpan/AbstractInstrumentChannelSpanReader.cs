using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.AbstractClasses.InstrumentChannel.InstrumentChannelSpan
{
    public abstract class AbstractInstrumentChannelSpanReader : AbstractInstrumentChannelSpan, IInstrumentChannelSpanReader
    {
        //internal bool _cyclicRead;
        //public bool CyclicRead { 
        //    get
        //    {
        //        return _cyclicRead;
        //    }
        //    set
        //    { 
        //        _cyclicRead = value;
        //    }
        //}

        OneMeasure _lastValue;
        public OneMeasure LastValue
        {
            get { return _lastValue; }
            set 
            { 
                _lastValue = value;
                //RaiseNewValueReaded(_lastValue);
                //if (CyclicRead)
                //{
                //    Read(LastValue.UOM);
                //}
            }
        }

        //public event NewValueReaded NewValueReaded;

        //void RaiseNewValueReaded(OneMeasure oneMeasure)
        //{
        //    NewValueReaded?.Invoke(oneMeasure);
        //}

        public abstract void Read(IUOM uom, Action<OneMeasure> reportTo);
    }
}