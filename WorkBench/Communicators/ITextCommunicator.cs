using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBench.Communicators
{
    public interface ITextCommunicator : ICommunicator
    {
        TextCommunicatorSendLineStatus SendLine(string cmd);
        TextCommunicatorReadLineStatus ReadLine(TimeSpan readLineTimeout, out string result);
        TextCommunicatorQueryCommandStatus QueryCommand(string cmd, out string result);

    }
}
