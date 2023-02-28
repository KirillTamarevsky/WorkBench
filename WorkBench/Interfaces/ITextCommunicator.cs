using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Interfaces
{
    public interface ITextCommunicator: ICommunicator
    {
        bool SendLine(string cmd);
        string ReadLine(TimeSpan readLineTimeout);
        string QueryCommand(string cmd);

    }
}
