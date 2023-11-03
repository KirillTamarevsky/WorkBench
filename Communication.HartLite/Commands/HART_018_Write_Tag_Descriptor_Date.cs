using Communication.HartLite.CommandResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.Commands
{
    public class HART_018_Write_Tag_Descriptor_Date: HARTCommand
    {
        public override byte Number => 18;
        public override byte[] Data => ToByteArray();


        private string _tag;
        private string _descriptor;
        private DateTime datecode;
        public HART_018_Write_Tag_Descriptor_Date(string tag, string descriptor, DateTime dateTime)
        {
            if (string.IsNullOrEmpty(tag))
            {
                _tag = string.Empty.PadRight(8, '?');
            }
            if (tag.Length > 8)
            {
                _tag = tag.Substring(0, 8);
            }
            else
            {
                _tag = tag.PadRight(8);
            }

            if (string.IsNullOrEmpty(descriptor))
            {
                _descriptor = string.Empty.PadRight(16, '?');
            }
            if (descriptor.Length > 16)
            {
                _descriptor = descriptor.Substring(0, 16);
            }
            else
            {
                _descriptor = descriptor.PadRight(16);
            }
            datecode = dateTime;
        }
        private byte[] ToByteArray()
        {
            var tagbytes = _tag.HART_pack_string();
            var descriptorbytes = _descriptor.HART_pack_string();
            byte day = (byte)datecode.Day;
            byte month = (byte)datecode.Month;
            byte year = (byte)(datecode.Year - 1900);

            var res = tagbytes.Concat(descriptorbytes).Append(day).Append(month).Append(year).ToArray();
            return res;
        }

        public override CommandResult ToCommandResult(HARTDatagram datagram)
        {
            return datagram.CommandNumber == Number? new HART_Result_018_Write_Tag_Descriptor_Date(datagram): base.ToCommandResult(datagram);
        }
    }
}
