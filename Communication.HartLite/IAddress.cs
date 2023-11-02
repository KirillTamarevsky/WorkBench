namespace Communication.HartLite
{
    public interface IAddress
    {
        MasterAddress _masterAddress { get; }
        bool _fieldDeviceInBurstMode { get; }
        byte[] ToByteArray();
        byte[] ToRawBytesArray();
        IAddress ToSTXAddress(MasterAddress masterAddress);
    }
}