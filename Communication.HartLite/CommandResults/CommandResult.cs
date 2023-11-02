using m4dHART._2_DataLinkLayer.Wired_Token_Passing;
using System;

namespace Communication.HartLite
{
    public class CommandResult
    {
        private readonly HARTDatagram _command;

        public byte Delimiter => _command.StartDelimiter; 
        public IAddress Address => _command.Address; 
        public byte CommandNumber => _command.CommandNumber; 
        public CommandStatus CommandStatus => CommandStatus.ToResponseCode(_command.CommandStatusBytes);
        public byte[] Data => _command.Data; 
        public byte Checksum => _command.CalculateChecksum(); 

        public FrameType FrameType => _command.FrameType;
        public MasterAddress MasterAddress => _command.MasterAddress;
        internal CommandResult(HARTDatagram command)
        {
            _command = command;
         }

        public bool HasCommunicationError => CommandStatus.FirstByte is CommandComminicationStatus; //.Data & 0b10000000) == 0b10000000;
    }
}