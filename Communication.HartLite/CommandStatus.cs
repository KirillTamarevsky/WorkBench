using System;

namespace Communication.HartLite
{
    public class CommandStatus
    {
        private byte _firstByte;
        private FieldDeviceStatus _secondByte;
        public CommandStatusFirstByte FirstByte 
        {
            get
            {
                if ((_firstByte & 0x80) == 0x80)
                {
                    return new CommandComminicationStatus(_firstByte);
                }
                else
                {
                    return new CommandResponseCode(_firstByte);
                }

            }

        }
        public FieldDeviceStatus FieldDeviceStatus => _secondByte;
        private CommandStatus(byte firstByte, byte secondByte)
        {
            _firstByte = firstByte;
            _secondByte = new FieldDeviceStatus( secondByte );
        }

        public static CommandStatus ToResponseCode(byte[] responseCodeBytes)
        {
            if (responseCodeBytes.Length != 2)
                throw new ArgumentException("ResponseCode needs exactly two bytes.", "responseCodeBytes");

            return new CommandStatus(responseCodeBytes[0], responseCodeBytes[1]);
        }

    }
    
    public abstract class CommandStatusFirstByte
    {
        public byte Data { get; protected set; }
        public CommandStatusFirstByte(byte databyte)
        {
            Data = databyte;
        }

    }

    public class CommandComminicationStatus : CommandStatusFirstByte
    {
        public CommandComminicationStatus(byte databyte) : base(databyte) { }
        public bool VerticalParityError => (Data & 0x40) == 0x40;
        public bool OverrunError => (Data & 0x20) == 0x02;
        public bool FramingError => (Data & 0x10) == 0x01;
        public bool LongitudinalParityError => (Data & 0x08) == 0x08;
        /// <summary>
        /// Only for master Data-Links and I/O systems to indicate
        /// communication to Field Device
        /// </summary>
        public bool CommunicationFailure => (Data & 0x04) == 0x04;
        public bool BufferOverflow => (Data & 0x02) == 0x02;
        //public bool Reserved => (Data & 0x01) == 0x01;
    }
    public class CommandResponseCode: CommandStatusFirstByte
    {
        public CommandResponseCode(byte databyte) : base(databyte) { }
        public bool NotImplemented => Data == 64;
    }

    public class FieldDeviceStatus
    {
        private byte Data;
        public FieldDeviceStatus(byte databyte)
        {
            Data = databyte;
        }
        public bool DeviceMalfunction => (Data & 0x80) == 0x80;
        public bool ConfigurationChanged => (Data & 0x40) == 0x40;
        public bool ColdStart => (Data & 0x20) == 0x20;
        public bool MoreStatusAvailable => (Data & 0x10) == 0x10;
        public bool LoopCurrentFixed => (Data & 0x08) == 0x08;
        public bool LoopCurrentSaturated => (Data & 0x04) == 0x04;
        public bool NonPrimaryVariableOutOfLimits => (Data & 0x02) == 0x02;
        public bool PrimaryVariableOutOfLimits => (Data & 0x01) == 0x01;
    }
}
