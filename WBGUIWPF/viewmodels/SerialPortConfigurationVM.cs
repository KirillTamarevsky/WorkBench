using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WBGUIWPF.viewmodels
{
    public class SerialPortConfigurationVM : BaseVM
    {
        private string serialPortName;
        public string SerialPortName { get => serialPortName; set { serialPortName = value; RaiseAllPropertiesChanged(); } }

        private int baudRate;
        public int BaudRate { get => baudRate; set { baudRate = value; RaiseAllPropertiesChanged(); } }
        
        private Parity parity;
        public Parity Parity { get => parity; set { parity = value; RaiseAllPropertiesChanged(); } }
        
        private int dataBits;
        public int DataBits { get => dataBits; set { dataBits = value; RaiseAllPropertiesChanged(); } }

        private StopBits stopBits;
        public StopBits StopBits { get => stopBits; set { stopBits = value; RaiseAllPropertiesChanged(); } }
        
        [JsonIgnore]
        public string DisplayShortName { get => ToString(); }

        [JsonIgnore]
        public List<int> AvailableBaudRates { get; } = new List<int>() { 115200, 57600, 38400, 19200, 9600, 4800, 2400, 1200 };

        [JsonIgnore]
        public List<Parity> AvailableParities { get; } = new List<Parity>() { Parity.Odd, Parity.Even, Parity.None, Parity.Mark };

        [JsonIgnore]
        public List<int> AvailableDataBits { get; } = new List<int>() { 8, 7 };

        [JsonIgnore]
        public List<StopBits> AvailableStopBits { get; } = new List<StopBits>() { StopBits.None, StopBits.One, StopBits.OnePointFive, StopBits.Two };
#if DEBUG
        [Obsolete("Only for Design data", true)]
        public SerialPortConfigurationVM()
        {
            serialPortName = "COM333";
            baudRate = 115200;
            parity = Parity.Odd;
            dataBits = 8;
            stopBits = StopBits.One;
        }
#endif
#if RELEASE
        public SerialPortConfigurationVM()
        {
            serialPortName = "n/a";
        }
#endif
        public SerialPortConfigurationVM(
            string _serialPortName,
            int _baudRate,
            int _dataBits,
            StopBits _stopBits,
            Parity _parity
            )
        {
            serialPortName = _serialPortName;
            baudRate = _baudRate;
            parity = _parity;
            dataBits = _dataBits;
            stopBits = _stopBits;
        }

        private void RaiseAllPropertiesChanged()
        {
            RaisePropertyChanged(nameof(DisplayShortName));
            RaisePropertyChanged(nameof(SerialPortName));
            RaisePropertyChanged(nameof(BaudRate));
            RaisePropertyChanged(nameof(DataBits));
            RaisePropertyChanged(nameof(StopBits));
            RaisePropertyChanged(nameof(Parity));
        }

        public override string ToString()
        {
            return $"{SerialPortName},{BaudRate},{DataBits},{StopBits},{Parity}";
        }
    }
}