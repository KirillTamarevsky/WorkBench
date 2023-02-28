using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkBench.Interfaces;

namespace WorkBench.Communicators
{

    public class FakeEKCommunicator : ITextCommunicator
    {
        bool isopened;
        private string _portName;

        private string _sendedLine;

        //private double prevValue = 3.5;

        public FakeEKCommunicator(string portName)
        {
            _portName = portName;
        }

        public bool Open()
        {
            isopened = true;
            return true;
        }
        public bool Close()
        {
            isopened = false;
            return true;
        }
        public bool IsOpen => isopened;
        public string ReadLine(TimeSpan readLineTimeout)
        {
            string answer = "";
            Thread.Sleep(5);
            switch (_sendedLine.Trim())
            {
                case "REMOTE":
                    answer = "R";
                    break;
                case "LOCAL":
                    answer = "L";
                    break;
                case "TCURR?":
                    answer = ((new System.Random()).NextDouble() * 16.9 + 3.5) .ToString("N4");
                    break;
                case "CURR?":
                    //prevValue += (new System.Random()).NextDouble() * 0.0016;
                    //answer = prevValue.ToString("N4");
                    answer = ((new System.Random()).NextDouble() * 0.009 + 13.5) .ToString("N4");
                    break;
                case "CHAN 1":
                    answer = "1";
                    break;
                default:
                    break;
            }
            log4net.LogManager.GetLogger("Communication").Info(
                string.Format(
                    "Readline = {0} | {1}", 
                    answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(answer))));

            return answer;
        }

        public bool SendLine(string cmd)
        {
            _sendedLine = cmd + "\r";

            log4net.LogManager.GetLogger("Communication").Info(
                string.Format(
                    "SendLine = {0} | {1}",
                    cmd.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(cmd))));
            return true;
        }

        public string QueryCommand(string cmd)
        {
            SendLine(cmd);
            return ReadLine(TimeSpan.FromSeconds(3));
        }

        public override string ToString()
        {
            return string.Format("демо порт {0}", _portName);
        }
    }
}
