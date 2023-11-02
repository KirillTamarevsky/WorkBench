using System;

namespace Communication.HartLite
{
    public class ShortAddress : IAddress
    {
        public MasterAddress _masterAddress { get; private set; }
        public bool _fieldDeviceInBurstMode { get; private set; }

        private byte _pollingAddress;
        public byte PollingAddress => _pollingAddress;

        public ShortAddress(MasterAddress address, bool fieldDeviceInBurstMode, byte pollingAddress)
        {
            if (pollingAddress > 15 || pollingAddress < 0)
                throw new ArgumentException();
            
            _masterAddress = address;
            
            _fieldDeviceInBurstMode = fieldDeviceInBurstMode;

            _pollingAddress = pollingAddress;
        }
        public ShortAddress(byte xACK_header_address_raw_byte)
        {
            _masterAddress = (xACK_header_address_raw_byte & 0b10000000) == 0 ? MasterAddress.Secondary : MasterAddress.Primary;
            _fieldDeviceInBurstMode = (xACK_header_address_raw_byte & 0b01000000) == 0b01000000;
            _pollingAddress = (byte)(xACK_header_address_raw_byte & 0b00111111);
        }
        public byte[] ToByteArray()
        {
            byte addr = (byte)(_pollingAddress | (byte)((byte)_masterAddress << 7));
            if (_fieldDeviceInBurstMode)
            {
                addr |= 0b01000000;
            }
            return new [] { addr };
        }
        public byte[] ToRawBytesArray()
        {
            return ToByteArray();
        }
        public IAddress ToSTXAddress(MasterAddress masterAddress)
        {
            return new ShortAddress(masterAddress, false, _pollingAddress);
        }

    }
}