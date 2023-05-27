using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.Interfaces
{
    public interface IInstrument
    {
        //ITextCommunicator _communicator { get; }
        //IInstrumentChannel this[int i] { get; }
        IInstrumentChannel[] Channels { get; }
        static string Name { get; }
        static string Description { get; }
        bool Open();
        bool Close();
        bool IsOpen { get; }
    }
}
