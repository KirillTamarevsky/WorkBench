using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WorkBench.Communicators
{

    public class FakeEVoltaCommunicator : ITextCommunicator
    {
        bool isopened;
        private string _portName;

        private string _sendedLine;

        //private double prevValue = 3.5;

        public FakeEVoltaCommunicator(string portName)
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
        public TextCommunicatorReadLineStatus ReadLine(TimeSpan readLineTimeout, out string result)
        {
            string answer = "";
            Thread.Sleep(5);
            switch (_sendedLine.Trim())
            {
                case "REMOTE":
                    answer = "OK";
                    break;
                case "LOCAL":
                    answer = "OK";
                    break;
                case "CURR?":
                    //prevValue += (new System.Random()).NextDouble() * 0.0016;
                    //answer = prevValue.ToString("N4");
                    answer = ((new System.Random()).NextDouble() * 0.009 + 13.5) .ToString("N4");
                    break;
                case "DEVICE?":
                    answer = "42";
                    break;
                default:
                    break;
            }
            log4net.LogManager.GetLogger("Communication").Info(
                string.Format(
                    "Readline = {0} | {1}", 
                    answer.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(answer))));
            result = answer;
            return TextCommunicatorReadLineStatus.Success;
        }

        public TextCommunicatorSendLineStatus SendLine(string cmd)
        {
            _sendedLine = cmd + "\r";

            log4net.LogManager.GetLogger("Communication").Info(
                string.Format(
                    "SendLine = {0} | {1}",
                    cmd.Replace("\r", "\\r").Replace("\n", "\\n"),
                    BitConverter.ToString(Encoding.ASCII.GetBytes(cmd))));
            return TextCommunicatorSendLineStatus.Success;
        }

        public TextCommunicatorQueryCommandStatus QueryCommand(string cmd, out string result)
        {
            SendLine(cmd);
            ReadLine(TimeSpan.FromSeconds(3), out result);

            return TextCommunicatorQueryCommandStatus.Success; 
        }

        public override string ToString()
        {
            return string.Format("демо порт {0}", _portName);
        }
    }
}
