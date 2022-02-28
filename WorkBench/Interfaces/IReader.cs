using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.Interfaces
{
    public interface IReader
    {
        bool CanRead(Scale scale);
        void Read(UOMType uOMType);
        OneMeasureResult lastValue { get; }

        event NewValueReaded NewValueReaded;
        bool CyclicRead { get; set; }
    }

    public delegate void NewValueReaded(IReader reader);
}
