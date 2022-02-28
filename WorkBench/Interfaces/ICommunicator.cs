using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public interface ICommunicator
    {
        bool SendLine(string cmd);
        string ReadLine();
        string QueryCommand(string cmd);
        bool Open();
        bool Close();
    }
}
