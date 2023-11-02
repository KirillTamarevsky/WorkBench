using System;
using m4dHART._2_DataLinkLayer.Wired_Token_Passing;

namespace Communication.HartLite
{
    public class HARTDatagram
    {
        public int PreambleLength { get; set; }
        public byte StartDelimiter { get; set; }
        public IAddress Address { get; set; }
        public byte CommandNumber { get; set; }
        public byte[] CommandStatusBytes { get; set; }
        public byte[] Data { get; set; }

        public MasterAddress MasterAddress => Address._masterAddress;


        public HARTDatagram()
        {
        }

        public HARTDatagram(int preambleLength, byte startDelimiter, IAddress address, byte commandNumber, byte[] responseCode, byte[] data)
        {
            PreambleLength = preambleLength;
            StartDelimiter = startDelimiter;
            Address = address;
            CommandNumber = commandNumber;
            CommandStatusBytes = responseCode;
            Data = data;
        }

        public FrameType FrameType
        {
            get
            {
                var frameType = StartDelimiter & 0b00000111;
                switch (frameType)
                {
                    case 1:
                        return FrameType.BACK;
                    case 2:
                        return FrameType.STX;
                    case 6:
                        return FrameType.ACK;
                    default:
                        return FrameType.UNKNOWN;
                }
            }
        }

        public virtual Byte[] ToByteArray()
        {
            StartDelimiter = (byte)(StartDelimiter | ((Address is LongAddress) ? 0x80 : 0x00));

            byte[] commandAsByteArray = BuildByteArray();

            commandAsByteArray[commandAsByteArray.Length - 1] = CalculateChecksum();

            return commandAsByteArray;
        }

        private byte[] BuildByteArray()
        {
            const int SIZE_OF_START_DELIMITER = 1;
            const int SIZE_OF_COMMAND_NUMBER = 1;
            const int SIZE_OF_DATA_BYTE_COUNT = 1;
            const int SIZE_OF_CHECKSUM = 1;

            int commandLength = PreambleLength + Data.Length + CommandStatusBytes.Length + Address.ToByteArray().Length +
                                SIZE_OF_START_DELIMITER + SIZE_OF_COMMAND_NUMBER +
                                SIZE_OF_DATA_BYTE_COUNT + SIZE_OF_CHECKSUM;
            var commandAsByteArray = new byte[commandLength];

            int currentIndex = 0;
            for (int i = 0; i < PreambleLength; ++i)
            {
                commandAsByteArray[currentIndex] = 255;
                currentIndex++;
            }
            commandAsByteArray[currentIndex] = StartDelimiter;
            currentIndex += SIZE_OF_START_DELIMITER;
            CopyArrayInArray(commandAsByteArray, Address.ToRawBytesArray(), currentIndex);
            currentIndex += Address.ToRawBytesArray().Length;
            commandAsByteArray[currentIndex] = CommandNumber;
            currentIndex += SIZE_OF_COMMAND_NUMBER;
            commandAsByteArray[currentIndex] = (byte)(Data.Length + CommandStatusBytes.Length);
            currentIndex += SIZE_OF_DATA_BYTE_COUNT;
            CopyArrayInArray(commandAsByteArray, CommandStatusBytes, currentIndex);
            currentIndex += CommandStatusBytes.Length;
            CopyArrayInArray(commandAsByteArray, Data, currentIndex);

            return commandAsByteArray;
        }

        private static void CopyArrayInArray(byte[] destination, byte[] source, int offset)
        {
            for (int i = 0; i < source.Length; ++i)
            {
                destination[i + offset] = source[i];
            }
        }

        internal byte CalculateChecksum()
        {
            byte[] data = BuildByteArray();
            byte checksum = 0;
            for (int i = PreambleLength; i < data.Length - 1; ++i)
            {
                checksum ^= data[i];
            }
            return checksum;
        }
    }
}