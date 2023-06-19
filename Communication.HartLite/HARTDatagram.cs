using System;

namespace Communication.HartLite
{
    public class HARTDatagram
    {
        public int PreambleLength { get; set; }
        public byte StartDelimiter { get; set; }
        public IAddress Address { get; set; }
        public byte CommandNumber { get; set; }
        public byte[] ResponseCode { get; set; }
        public byte[] Data { get; set; }

        private static byte MasterToSlaveStartDelimiter => 0x2; // 2
        public static byte SlaveToMasterStartDelimiter => 6;

        public HARTDatagram()
        {
        }

        public HARTDatagram(int preambleLength, IAddress address, byte commandNumber, byte[] responseCode, byte[] data)
        {
            PreambleLength = preambleLength;
            StartDelimiter = MasterToSlaveStartDelimiter;
            Address = address;
            CommandNumber = commandNumber;
            ResponseCode = responseCode;
            Data = data;
        }



        public bool IsChecksumCorrect(byte checksum)
        {
            return CalculateChecksum() == checksum;
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

            int commandLength = PreambleLength + Data.Length + ResponseCode.Length + Address.ToByteArray().Length +
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
            CopyArrayInArray(commandAsByteArray, Address.ToByteArray(), currentIndex);
            currentIndex += Address.ToByteArray().Length;
            commandAsByteArray[currentIndex] = CommandNumber;
            currentIndex += SIZE_OF_COMMAND_NUMBER;
            commandAsByteArray[currentIndex] = (byte)(Data.Length + ResponseCode.Length);
            currentIndex += SIZE_OF_DATA_BYTE_COUNT;
            CopyArrayInArray(commandAsByteArray, ResponseCode, currentIndex);
            currentIndex += ResponseCode.Length;
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