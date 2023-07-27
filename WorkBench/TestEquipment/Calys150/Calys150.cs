using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WorkBench.Communicators;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;

namespace WorkBench.TestEquipment.Calys150
{
    public class Calys150 : IInstrument
    {
        public string Name => "Calys 150";
        public string Description => "многофункциональный калибратор";
        public string SerialNumber { get; private set; }
        public string FWVersion{ get; private set; }
        internal ITextCommunicator Communicator { get; }
        internal TextCommunicatorSendLineStatus SendLine(string command) => Communicator.SendLine(command);
        internal TextCommunicatorReadLineStatus ReadLine(TimeSpan readLineTimeout, out string result) => Communicator.ReadLine(readLineTimeout, out result);
        internal TextCommunicatorQueryCommandStatus Query(string command, out string result) => Communicator.QueryCommand(command, out result, null);
#region Constructors
        public Calys150(ITextCommunicator textCommunicator) 
        {
            Communicator = textCommunicator;
            Channels = new IInstrumentChannel[] { new Calys150ReaderChannel(this) };
        }
        public Calys150(string SerialPortName) : this(GetSerialPortCommunicatorWithDefaultSettings(SerialPortName)){}
        public static ITextCommunicator GetSerialPortCommunicatorWithDefaultSettings(string serialPortName)
        {
            return new SerialPortTextCommunicator(new WBSerialPortWrapper(serialPortName, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One), "\r\n", timeout: 5);
        }
        public static ITextCommunicator GetSimulationCalys150SerialPortCommunicatorWithDefaultSettings(string serialPortName)
        {
            return new SerialPortTextCommunicator(new FakeCalys150SerialPort($"DEMO[{serialPortName}]", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One), "\r\n");
        }
#endregion
        public IInstrumentChannel[] Channels { get; }
        Calys150ReaderChannel ActiveChannel { get; set; }
        public bool IsOpen => Communicator.IsOpen;
        public bool Open()
        {
            lock (Communicator)
            {

                if (Communicator.Open())
                {
                    Communicator.SendLine("REM");
                    var respStatus = Query("*IDN?", out string resp);
                    if (resp.Length > 0)
                    {
                        var respparts = resp.Split(",");
                        if (respparts.Length == 4)
                        {
                            if (respparts[1].ToUpper().Contains("CALYS150"))
                            {
                                SerialNumber = respparts[2];
                                FWVersion = respparts[3];
                                Communicator.SendLine("*CLS");
                                return true;
                            }
                        }
                    }
                    Communicator.SendLine("LOC");
                    Communicator.Close();
                }
                return false;
            }
        }

        public bool Close()
        {
            lock (Communicator)
            {
                Communicator.SendLine("LOC");
                Communicator.Close();
                return true;
            }
        }

        public override string ToString()
        {
            return $"{Description} {Name} - {Communicator}";
        }

        internal void ActivateChannel(Calys150ReaderChannel calys150ReaderChannel)
        {
            if (calys150ReaderChannel.parentCalys150 != this) throw new ArgumentException("not my channel");
            if (ActiveChannel != calys150ReaderChannel)
            {
                Communicator.SendLine($"SENS{calys150ReaderChannel.NUM}:RUN");
                ActiveChannel = calys150ReaderChannel;
            }
        }
    }
}
