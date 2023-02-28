﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces.InstrumentChannel
{
    public interface IInstrumentChannel
    {
        int NUM { get; }
        string Name { get; }
        
        //IInstrument parent { get; }
        IInstrumentChannelSpan[] AvailableSpans { get; }
        IInstrumentChannelSpan ActiveSpan { get; }
    }

}
