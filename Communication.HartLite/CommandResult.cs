﻿namespace Communication.HartLite
{
    public class CommandResult
    {
        private readonly HARTCommand _command;

        public byte CommandNumber => _command.CommandNumber; 
        public byte[] Data => _command.Data; 
        public byte Delimiter => _command.StartDelimiter; 
        public ResponseCode ResponseCode => ResponseCode.ToResponseCode(_command.ResponseCode);
        public IAddress Address => _command.Address; 
        public int PreambleLength => _command.PreambleLength; 
        public byte Checksum => _command.CalculateChecksum(); 
        internal CommandResult(HARTCommand command)
        {
            _command = command;
        }
    }
}