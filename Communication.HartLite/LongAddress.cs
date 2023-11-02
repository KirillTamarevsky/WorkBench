using System;

namespace Communication.HartLite
{
    public class LongAddress : IAddress
    {
        public MasterAddress _masterAddress { get; private set; }
        public bool _fieldDeviceInBurstMode { get; private set; }
        private byte[] _expandedDeviceTypeCode;
        private readonly byte[] _deviceIdentificationNumber;
        public LongAddress(MasterAddress masterAddress, bool fielDeviceInBurstMode, byte[] expandedDeviceTypeCode, byte[] uniqueDeviceIdentifier)
        {
            if (uniqueDeviceIdentifier.Length != 3 | expandedDeviceTypeCode.Length != 2)
                throw new ArgumentException();
            _masterAddress = masterAddress;
            _fieldDeviceInBurstMode = fielDeviceInBurstMode;
            _expandedDeviceTypeCode = expandedDeviceTypeCode;
            _deviceIdentificationNumber = uniqueDeviceIdentifier;
        }
        public LongAddress(byte[] xACK_header_address_raw_bytes)
        {
            if (xACK_header_address_raw_bytes.Length != 5)
                throw new ArgumentException();
            _masterAddress = (xACK_header_address_raw_bytes[0] & 0b10000000) == 0 ? MasterAddress.Secondary : MasterAddress.Primary;
            _fieldDeviceInBurstMode = (xACK_header_address_raw_bytes[0] & 0b01000000) == 0b01000000;
            _expandedDeviceTypeCode = new byte[2];
            _expandedDeviceTypeCode[0] = (byte)(xACK_header_address_raw_bytes[0] & 0b00111111);
            _expandedDeviceTypeCode[1] = xACK_header_address_raw_bytes[1];
            _deviceIdentificationNumber = new byte[3];
            _deviceIdentificationNumber[0] = xACK_header_address_raw_bytes[2];
            _deviceIdentificationNumber[1] = xACK_header_address_raw_bytes[3];
            _deviceIdentificationNumber[2] = xACK_header_address_raw_bytes[4];
        }
        public byte[] ToByteArray()
        {
            byte[] address = new byte[5];
            address[0] = (byte)(_expandedDeviceTypeCode[0] | (byte)((byte)_masterAddress << 7));
            if (_fieldDeviceInBurstMode)
            {
                address[0] |= 0b01000000;
            }
            address[1] = _expandedDeviceTypeCode[1];
            address[2] = _deviceIdentificationNumber[0];
            address[3] = _deviceIdentificationNumber[1];
            address[4] = _deviceIdentificationNumber[2];
            return address;
        }
        public byte[] ToRawBytesArray()
        {
            return ToByteArray();
        }
        public IAddress ToSTXAddress(MasterAddress masterAddress)
        {
            return new LongAddress(masterAddress, false, _expandedDeviceTypeCode, _deviceIdentificationNumber);
        }
    }
}