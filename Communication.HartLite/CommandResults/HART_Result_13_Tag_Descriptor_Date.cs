using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.HartLite.CommandResults
{
    public class HART_Result_13_Tag_Descriptor_Date : CommandResult
    {
        internal HART_Result_13_Tag_Descriptor_Date(HARTDatagram command) : base(command)
        {
        }
        public string Tag
        {
            get
            {
                return new byte[]
                {
                    Data[0],
                    Data[1],
                    Data[2],
                    Data[3],
                    Data[4],
                    Data[5]
                }.HART_unpack_string().TrimEnd();
            }
        }
        public string Descriptor
        {
            get
            {
                return new byte[]
                {
                    Data[6],
                    Data[7],
                    Data[8],

                    Data[9],
                    Data[10],
                    Data[11],
                    
                    Data[12],
                    Data[13],
                    Data[14],
                    
                    Data[15],
                    Data[16],
                    Data[17]
                }.HART_unpack_string().TrimEnd();
            }
        }
        public DateTime Date
        {
            get
            {
                return new byte[]
                {
                    Data[18],
                    Data[19],
                    Data[20]
                }.HART_unpack_DateCode();
            }
        }
    }
}
