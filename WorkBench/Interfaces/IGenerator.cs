using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public interface IGenerator
    {
        OneMeasureResult SetPoint { get; set; }
    }
}
