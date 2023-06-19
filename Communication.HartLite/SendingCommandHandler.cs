namespace Communication.HartLite
{
    public delegate void SendingCommandHandler(object sender, CommandRequest args);

    public class CommandRequest
    {
        private HARTDatagram _command { get; }
        public int PreambleLength => _command.PreambleLength;
        public byte Delimiter => _command.StartDelimiter;
        public IAddress Address => _command.Address; 
        public byte CommandNumber => _command.CommandNumber;
        public byte[] Data => _command.Data; 
        public byte Checksum => _command.CalculateChecksum(); 

        internal CommandRequest(HARTDatagram command)
        {
            _command = command;
        }
    }
}