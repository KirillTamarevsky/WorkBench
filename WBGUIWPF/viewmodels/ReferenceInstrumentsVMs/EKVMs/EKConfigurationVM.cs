using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WBGUIWPF.Interfaces;
using WorkBench;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK;

namespace WBGUIWPF.viewmodels
{
    public class EKConfigurationVM : BaseVM, IInstrumentWithSerialPort, IInstrumentCreator, IBenchComponent
    {
        private const string THIS_BENCH_COMPONENT_ID = "Elmentro-Kalvin";
        public string SerialNumber { get; set; }
        public ObservableCollection<SerialPortConfigurationVM> AvailableSerialPortConfigurations { get; set; }
        
        public event EventHandler<SerialPortConfigurationVM>? SerialPortChanged;
        private SerialPortConfigurationVM? serialPortConfigurationVM { get; set; }
        public SerialPortConfigurationVM? SelectedSerialPortConfiguration 
        {
            get => serialPortConfigurationVM;
            set
            {
                if (value != null)
                {
                    SerialPortChanged?.Invoke(this, value);
                }
                serialPortConfigurationVM = value;
                RaisePropertyChanged();
            }
        }
#if DEBUG
        [Obsolete("Only for Design data", true)]
        public EKConfigurationVM()
        {
            SerialNumber = "DebugSerialNumber";
            AvailableSerialPortConfigurations = new() { new SerialPortConfigurationVM("demo1", 115200, 8, StopBits.None, Parity.None),
                                                        new SerialPortConfigurationVM("demo2", 115200, 8, StopBits.None, Parity.None)};
        }
#endif
        public EKConfigurationVM(string serialNumber, 
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
                return Factory.GetFakeEK(SelectedSerialPortConfiguration.SerialPortName);
            }
            else
            {
                return Factory.GetEK_on_SerialPort(
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
            var dictionary = new Dictionary<string, JsonNode?>
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
                    var EKconfigVM = new EKConfigurationVM(serialNumber, benchConfigurationVM.SerialPorts);
                    var serialPortConfigVM = benchConfigurationVM.SerialPorts.FirstOrDefault(s => s.SerialPortName == comPortName);
                    EKconfigVM.SelectedSerialPortConfiguration = serialPortConfigVM;
                    benchConfigurationVM.AddReferenceInstrument(EKconfigVM);
                    //benchConfigurationVM.ReferenceInstruments.Add(EKconfigVM);
                }
            }
        }
    }
}
