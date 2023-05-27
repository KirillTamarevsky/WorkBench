using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using WBGUIWPF.Interfaces;
using WorkBench;
using WorkBench.Interfaces;

namespace WBGUIWPF.viewmodels
{
    public class EPascalConfigurationVM : BaseVM, IInstrumentWithSerialPort, IInstrumentCreator, IBenchComponent
    {
        private const string THIS_BENCH_COMPONENT_ID = "Elmentro-Pascal";

        public string SerialNumber { get; set; }
        public ObservableCollection<SerialPortConfigurationVM> AvailableSerialPortConfigurations { get; set; }
        
        public event EventHandler<SerialPortConfigurationVM> SerialPortChanged;
        
        private SerialPortConfigurationVM? selectedSerialPortConfigurationVM;

        public SerialPortConfigurationVM? SelectedSerialPortConfiguration 
        {
            get => selectedSerialPortConfigurationVM; 
            set
            {
                if (value != null)
                {
                    value.BaudRate = 19200;
                    value.DataBits = 8;
                    value.StopBits = StopBits.One;
                    value.Parity = Parity.Odd;
                    SerialPortChanged?.Invoke(this, value);
                }
                selectedSerialPortConfigurationVM = value;
                RaisePropertyChanged(nameof(SelectedSerialPortConfiguration));
            }
        }
#if DEBUG
        [Obsolete("Only for Design data", true)]
        public EPascalConfigurationVM()
        {
            SerialNumber = "DebugSerialNumber";
            AvailableSerialPortConfigurations = new() { new SerialPortConfigurationVM("demo1", 115200, 8, StopBits.None, Parity.None),
                                                        new SerialPortConfigurationVM("demo2", 115200, 8, StopBits.None, Parity.None)};
        }
#endif
        public EPascalConfigurationVM(string serialNumber, 
            ObservableCollection <SerialPortConfigurationVM> availableSerialPortConfigurations)
        {
            SerialNumber = serialNumber;
            AvailableSerialPortConfigurations = availableSerialPortConfigurations;
        }
        public bool CanGetInstrument
        {
            get
            {
                return SelectedSerialPortConfiguration != null;
            }
        }
        public IInstrument GetInstrument()
        {
            if (SelectedSerialPortConfiguration == null) throw new ArgumentException();

            if (SelectedSerialPortConfiguration.SerialPortName.ToLower().Contains("demo"))
            {
                return Factory.GetFakeEPascal(SelectedSerialPortConfiguration.SerialPortName);
            }
            else
            {
                return Factory.GetEPascal_on_SerialPort(
                    SelectedSerialPortConfiguration.SerialPortName,
                    SelectedSerialPortConfiguration.BaudRate,
                    SelectedSerialPortConfiguration.Parity,
                    SelectedSerialPortConfiguration.DataBits,
                    SelectedSerialPortConfiguration.StopBits
                    );
            }
        }

        public JsonObject GetConfig()
        {
            var dictionary = new Dictionary<string, JsonNode>
            {
                ["BenchComponentID"] = THIS_BENCH_COMPONENT_ID,
                ["SerialNumber"] = SerialNumber,
                ["ComPortName"] = SelectedSerialPortConfiguration == null ? "none" : SelectedSerialPortConfiguration.SerialPortName
            };

            var jsonObject = new JsonObject(dictionary);

            return jsonObject;
        }

        public static void InitVMFromJsonElement(JsonElement element, BenchConfigurationVM benchConfigurationVM)
        {
            if (element.TryGetProperty("BenchComponentID", out JsonElement bcid))
            {
                if (bcid.GetString() == THIS_BENCH_COMPONENT_ID)
                {
                    var serialNumber = element.GetProperty("SerialNumber").GetString();
                    var comPortName = element.GetProperty("ComPortName").GetString();
                    var EPascalConfigVM = new EPascalConfigurationVM(serialNumber, benchConfigurationVM.SerialPorts);
                    var serialPortConfigVM = benchConfigurationVM.SerialPorts.FirstOrDefault(s => s.SerialPortName == comPortName);
                    EPascalConfigVM.SelectedSerialPortConfiguration = serialPortConfigVM;
                    benchConfigurationVM.AddReferenceInstrument(EPascalConfigVM);
                    //benchConfigurationVM.ReferenceInstruments.Add(EPascalConfigVM);
                }
            }
        }

    }
}
