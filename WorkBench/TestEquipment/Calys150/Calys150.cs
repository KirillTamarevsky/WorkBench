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
        ITextCommunicator Communicator { get; }
        internal bool SendLine(string command) => Communicator.SendLine(command);
        internal string ReadLine(TimeSpan readLineTimeout) => Communicator.ReadLine(readLineTimeout);
        internal string Query(string command) => Communicator.QueryCommand(command);
        public Calys150(ITextCommunicator textCommunicator) 
        {
            Communicator = textCommunicator;
            Channels = new IInstrumentChannel[] { new Calys150ReaderChannel(this) };
        }
        public Calys150(string SerialPortName) : this(GetSerialPortCommunicatorWithDefaultSettings(SerialPortName)){}
        public static ITextCommunicator GetSerialPortCommunicatorWithDefaultSettings(string serialPortName)
        {
            return new SerialEKCommunicator(new WBSerialPortWrapper(serialPortName, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One), "\r\n");
        }
        public static ITextCommunicator GetSimulationCalys150SerialPortCommunicatorWithDefaultSettings(string serialPortName)
        {
            return new SerialEKCommunicator(new FakeCalys150SerialPort($"DEMO[{serialPortName}]", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One), "\r\n");
        }

        public IInstrumentChannel[] Channels { get; }

        public bool IsOpen => Communicator.IsOpen;
        public bool Open()
        {
            lock (Communicator)
            {

                if (Communicator.Open())
                {
                    Communicator.SendLine("REM");
                    var resp = Communicator.QueryCommand("*IDN?");
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
    }
}
