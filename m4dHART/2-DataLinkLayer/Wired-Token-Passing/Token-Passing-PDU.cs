using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace m4dHART._2_DataLinkLayer.Wired_Token_Passing
{
    public class Token_Passing_PDU
    {
        public Delimiter Delimiter { get; }
        public IHartAddress Address { get; }
        public byte[] ExpansionBytes { get; }
        public byte Command { get; }
        public byte ByteCount { get => (byte)Data.Length; }
        public byte[] Data { get; }
        public byte CheckByte { get
            {
                return ToByteArray().Last();
            }
        }
        private int pduLength => 1 + Address.Length + ExpansionBytes.Length + 1 + 1 + Data.Length + 1;

        public Token_Passing_PDU(Delimiter delimiter, IHartAddress address, byte[] expansionBytes, byte command, byte[] data)
        {
            Delimiter = delimiter;
            Address = address;
            ExpansionBytes = expansionBytes;
            Command = command;
            Data = data;
        }
        public byte[] ToByteArray()
        {        
            var res = new byte[pduLength];

            int i = 0;
            res[i] = Delimiter.ToByte();
            i++;
            for (int al = 0; al < Address.Length; al++)
            {
                res[i] = Address.AddressBytes[al];
                i++;
            }
            for (int al = 0; al < ExpansionBytes.Length; al++)
            {
                res[i] = ExpansionBytes[al];
                i++;
            }
            res[i] = Command; i++;
            res[i] = ByteCount; i++;
            for (int al = 0; al < Data.Length; al++)
            {
                res[i] = Data[al];
                i++;
            }

            byte checkByte = 0;
            for (int al = 0; al < res.Length -1 ; al++)
            {
                checkByte ^= res[al];
            }
            res[i] = checkByte;

            return res;
        }

        public bool IsCorrect()
        {
            var bytes = ToByteArray();
            byte res = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                res ^= bytes[i];
            }
            return res == 0;
        }



    }
}
