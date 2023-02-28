﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public interface ICommunicator
    {
        bool Open();
        bool Close();
        bool IsOpen { get; }
    }
}
