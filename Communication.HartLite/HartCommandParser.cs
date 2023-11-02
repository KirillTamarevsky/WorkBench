using System;
using System.Runtime.Remoting;
using System.Linq;
using log4net;
using m4dHART._2_DataLinkLayer.Wired_Token_Passing;
using System.Net;

namespace Communication.HartLite
{
    internal class HartCommandParser
    {
        public enum ReceiveState
        {
            NotInCommand,
            Preamble,
            StartDelimiter,
            Address,
            /// <summary>
            /// optional
            /// </summary>
            ExpansionBytes,
            Command,
            ByteCount,
            /// <summary>
            /// optional
            /// </summary>
            Data,
            CheckByte,
            CorrectPDUReceived,
            WrongCheckSumDUReceived
        }

        public enum ReceiveError
        {
            CheckSumError,
            ResponseError
        }
        public bool ReceivingPDU
        {
            get
            {
                return
                       //_nextByteType == ReceiveState.NotInCommand
                     _nextByteType == ReceiveState.Preamble
                    || _nextByteType == ReceiveState.StartDelimiter
                    || _nextByteType == ReceiveState.Address
                    || _nextByteType == ReceiveState.ExpansionBytes
                    || _nextByteType == ReceiveState.Command
                    || _nextByteType == ReceiveState.ByteCount
                    || _nextByteType == ReceiveState.Data
                    || _nextByteType == ReceiveState.CheckByte
                    ;

            }
        }
        private static readonly ILog Log = LogManager.GetLogger("HART");
        
        private ReceiveState _currentByteType = ReceiveState.NotInCommand;
        private ReceiveState _nextByteType = ReceiveState.NotInCommand;
        public ReceiveState CurrentByteType => _currentByteType;
        private byte[] _inputBuffer { get; set; } = new byte[1024];
        private int _currentIndex { get; set; }
        private int preamblesReceived { get; set; } = 0;

        public HARTDatagram LastReceivedDatagram { get; set; }

        public void ParseNextBytes(Byte[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                ParseByte(data[i]);
            }
        }

        public ReceiveState ParseByte(byte data)
        {
            //lock (lockobject)
            {

                switch (_nextByteType)
                {
                    case ReceiveState.NotInCommand:
                        ParseNotInCommand(data);
                        break;
                    case ReceiveState.Preamble:
                        //Log.Info("Parsing Preamble");
                        ParsePreamble(data);
                        break;
                    case ReceiveState.StartDelimiter:
                        //Log.Info("Parsing Delimiter");
                        ParseStartDelimiter(data);
                        break;
                    case ReceiveState.Address:
                        //Log.Info("Parsing Address");
                        ParseAddress(data);
                        break;
                    case ReceiveState.ExpansionBytes:
                        //Log.Info("Parsing ExpansionBytes");
                        ParseExpansionBytes(data);
                        break;
                    case ReceiveState.Command:
                        //Log.Info("Parsing Command");
                        ParseCommand(data);
                        break;
                    case ReceiveState.ByteCount:
                        //Log.Info("Parsing ByteCount");
                        ParseByteCount(data);
                        break;
                    case ReceiveState.Data:
                        //Log.Info("Parsing Data");
                        ParseData(data);
                        break;
                    case ReceiveState.CheckByte:
                        //Log.Info("Parsing CheckSum");
                        ParseChecksum(data);
                        break;
                }
                return _currentByteType;
            }
        }

        private int _delimiterIndex => 0;
        private byte _delimiter => _inputBuffer[0];
        private int _addressIndex => 1;
        private int _addressLength => _addressType == 1 ? 5 : 1;
        private int _expansionBytesCount => (_inputBuffer[0] & 0b01100000) >> 5;
        private int _commandByteIndex => _addressLength + _expansionBytesCount + 1;
        private byte _commandByte => _inputBuffer[_commandByteIndex];
        private int _dataBytesCountByteIndex => _commandByteIndex + 1;
        private int _dataBytesCount => _inputBuffer[_dataBytesCountByteIndex];
        private int _firstDataByteIndex => _dataBytesCountByteIndex + 1;
        private int _lastDataByteIndex => _dataBytesCountByteIndex + _dataBytesCount;


        private FrameType frameType
        {
            get
            {
                var frameType = _inputBuffer[0] & 0b00000111;
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
        private int _addressType
        {
            get
            {
                var AddressType = (_inputBuffer[0] & 0b10000000) >> 7;
                return AddressType;
            }
        }

        private IAddress _address
        {
            get
            {
                switch (_addressType)
                {
                    case 0:
                        var shortAddr = new ShortAddress(  _inputBuffer[1]);
                        return shortAddr;
                    case 1:
                        var longAddr = new LongAddress(new byte[] { _inputBuffer[1],
                                                                _inputBuffer[2],
                                                                _inputBuffer[3],
                                                                _inputBuffer[4],
                                                                _inputBuffer[5]});
                        return longAddr;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
        private void ParseNotInCommand(byte data)
        {
            _currentByteType = ReceiveState.NotInCommand;

            if (data == 0xFF)
            {
                _nextByteType = ReceiveState.Preamble;
                _currentByteType = ReceiveState.Preamble;
                preamblesReceived = 1;
            }
        }
        private void ParsePreamble(byte data)
        {
            if (data == 0xFF)
            {
                preamblesReceived++;
            }
            else if (data != 0xFF & preamblesReceived >= 2)
            {
                _nextByteType = ReceiveState.StartDelimiter;
                ParseByte(data);
            }
            else
            {
                Reset();
            }
        }

        private void ParseStartDelimiter(byte data)
        {
            //check delimiter for correctness
            var PhysicalLayerType = (data & 0b00011000) >> 3;
            var FrameType = data & 0b00000111;
            if (
                   !(
                    (PhysicalLayerType == 0 | PhysicalLayerType == 1)
                    &
                    (FrameType == 1 | FrameType == 2 | FrameType == 6)
                    )
                )
            {
                Reset();
                return;
            }

            BufferAddByte(data, ReceiveState.StartDelimiter);
            _nextByteType = ReceiveState.Address;
        }

        private void ParseAddress(byte data)
        {
            BufferAddByte(data, ReceiveState.Address);
            if (_currentIndex == _addressLength)
            {
                if (_expansionBytesCount > 0)
                {
                    _nextByteType = ReceiveState.ExpansionBytes;
                }
                else
                {
                    _nextByteType = ReceiveState.Command;
                }
            }
        }

        private void ParseExpansionBytes(byte data)
        {
            BufferAddByte(data, ReceiveState.ExpansionBytes);
            var addressLength = _addressType == 1 ? 5 : 1;
            if (_currentIndex == addressLength + _expansionBytesCount)
            {
                _nextByteType = ReceiveState.Command;
            }
        }
        private void ParseCommand(byte data)
        {
            BufferAddByte(data, ReceiveState.Command);
            _nextByteType = ReceiveState.ByteCount;
        }

        private void ParseByteCount(byte byteCount)
        {
            // All BACK and ACK messages must containt at least two data bytes. If the most significant bit (i.e., bit 7) of first data byte is set then the byte contains Communication Error information
            // No Communication Error shall be indicated in BACK (Burst Frames) frames as there no corresponding STX about whick to report a communication error.
            // Aside from interpreting the communication error status byte (if it is present), Data Link Layer implementations shall not make any interpretation of Data field.
            if ((frameType == FrameType.BACK | frameType == FrameType.ACK) & byteCount < 2)
            {
                Reset();
                //TODO: ERROR.indicate
                return;
            }
            BufferAddByte(byteCount, ReceiveState.ByteCount);
            if (byteCount == 0)
            {
                _nextByteType = ReceiveState.CheckByte;
            }
            else
            {
                _nextByteType = ReceiveState.Data;
            }

        }

        private void ParseData(byte data)
        {
            BufferAddByte(data, ReceiveState.Data);
            if (_currentIndex == _lastDataByteIndex)
            {
                _nextByteType = ReceiveState.CheckByte;
            }
        }

        private void ParseChecksum(byte receivedCheckSum)
        {
            BufferAddByte(receivedCheckSum, ReceiveState.CheckByte);
            var calculatedCheckSum = 0;
            for (int i = 0; i < _currentIndex + 1; i++)
            {
                calculatedCheckSum ^= _inputBuffer[i];
            }

            var CheckSumIsCorrect = calculatedCheckSum == 0;

            if (!CheckSumIsCorrect)
            {
                Log.Warn("Checksum is wrong!");
                // TODO: ERROR.indicate
                _currentByteType = ReceiveState.WrongCheckSumDUReceived;
                Reset();
                return;
            }
            byte[] respCodes;
            if (frameType == FrameType.BACK | frameType == FrameType.ACK)
            {
                respCodes = new byte[2];
                respCodes[0] = _inputBuffer[_firstDataByteIndex];
                respCodes[1] = _inputBuffer[_firstDataByteIndex + 1];

                if ((respCodes[0] & 0b10000000) == 0b10000000)
                {
                    Log.Warn("Communication error. First bit of response code byte is set.");

                    if ((respCodes[0] & 0x40) == 0x40)
                        Log.WarnFormat("Vertical Parity Error - The parity of one or more of the bytes received by the device was not odd.");
                    if ((respCodes[0] & 0x20) == 0x20)
                        Log.WarnFormat("Overrun Error - At least one byte of data in the receive buffer of the UART was overwritten before it was read (i.e., the slave did not process incoming byte fast enough).");
                    if ((respCodes[0] & 0x10) == 0x10)
                        Log.WarnFormat("Framing Error - The Stop Bit of one or more bytes received by the device was not detected by the UART (i.e. a mark or 1 was not detected when a Stop Bit should have occoured)");
                    if ((respCodes[0] & 0x08) == 0x08)
                        Log.WarnFormat("Longitudinal Partity Error - The Longitudinal Partity calculated by the device did not match the Check Byte at the end of the message.");
                    if ((respCodes[0] & 0x02) == 0x02)
                        Log.WarnFormat("Buffer Overflow - The message was too long for the receive buffer of the device.");
                }
            }
            else { respCodes = new byte[0]; }

            byte[] data = new byte[0];
            if (_dataBytesCount > 0)
            {
                var firstDataByteIndex = _firstDataByteIndex;
                var lastDataByteIndex = _lastDataByteIndex;
                if (frameType == FrameType.BACK | frameType == FrameType.ACK)
                {
                    firstDataByteIndex += 2;
                }
                var dataLengthWithoutResponseCode = _dataBytesCount - 2;
                if (dataLengthWithoutResponseCode > 0)
                {
                    data = new byte[dataLengthWithoutResponseCode];
                    for (int i = 0; i < dataLengthWithoutResponseCode; i++)
                    {
                        data[i] = _inputBuffer[i + firstDataByteIndex];
                    }
                }
            }
            LastReceivedDatagram = new HARTDatagram(preamblesReceived, _delimiter, _address, _commandByte, respCodes, data);
            _currentByteType = ReceiveState.CorrectPDUReceived;
            Reset();
        }

        private void BufferAddByte(byte data, ReceiveState byteType)
        {
            _currentIndex++;
            _inputBuffer[_currentIndex] = data;
            _currentByteType = byteType;
        }
        public void Reset()
        {
            _currentIndex = -1;
            preamblesReceived = 0;
            _nextByteType = ReceiveState.NotInCommand;
        }
    }
}