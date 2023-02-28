using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Enums;

namespace WorkBench.Interfaces.InstrumentChannel
{
    public interface IInstrumentChannelSpanPressureGenerator : IInstrumentChannelSpanGenerator
    {
        void GetPressureOperationMode(Action<PressureControllerOperationMode> reportTo);
        void SetPressureOperationMode(PressureControllerOperationMode value);
    }
}
