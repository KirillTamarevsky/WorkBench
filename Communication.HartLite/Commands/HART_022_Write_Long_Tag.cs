using Communication.HartLite.CommandResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_022_Write_Long_Tag : HARTCommand
    {
        public override byte Number => 22;
        public override byte[] Data => ToByteArray();


        private string NewLongTag { get; set; }
        public HART_022_Write_Long_Tag(string newLongTag)
        {
            if (string.IsNullOrEmpty(newLongTag))
            {
                newLongTag = string.Empty;
            }
            if (newLongTag.Length > 32)
            {
                newLongTag = newLongTag.Substring(0, 32);
            }
            NewLongTag = newLongTag;
        }
        private byte[] ToByteArray()
        {
            var res = new byte[32];

            var strbytes = Encoding.ASCII.GetBytes(NewLongTag);

            Array.Copy(strbytes, 0, res, 0, strbytes.Length);

            return res;
        }

        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            return datagram.CommandNumber == Number? new HART_Result_022_Write_Long_Tag(datagram) : base.ToCommandResult(datagram);
        }
    }
}
