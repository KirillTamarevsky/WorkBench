using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using WBGUIWPF.Interfaces;
using WBGUIWPF.viewmodels;
using WorkBench;
using WorkBench.Interfaces;

namespace WBGUIWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindowVM MainWindowVM;
        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            //mainWindow.Content = new PressureInstrumentCalibrationVM(instrumentList);
            //var serialPortConfigVM = new SerialPortConfigurationVM("DebugSerialNumber", 
            //                                                        115200,
            //                                                        8, 
            //                                                        System.IO.Ports.StopBits.One,
            //                                                        System.IO.Ports.Parity.None
            //                                                        );
            //var ekConfigVM = new EKConfigurationVM("DebugSerialNumber", serialPortConfigVM);
            var benchConfigurationVM = new BenchConfigurationVM();
            
            if (Path.Exists(GetConfigJsonFilePath()))
            {

                var finalJSonConfig = File.ReadAllText(GetConfigJsonFilePath());
                try
                {
#if DEBUG
                    string debugJSonConfig = """{ "SerialPorts":[{ "SerialPortName":"demo1","BaudRate":115200,"Parity":0,"DataBits":8,"StopBits":1},{ "SerialPortName":"demo2","BaudRate":19200,"Parity":0,"DataBits":8,"StopBits":1}],"BenchComponents":[{ "BenchComponentID":"Elmentro-Kalvin","SerialNumber":"na","ComPortName":"demo1"},{ "BenchComponentID":"Elmentro-Kalvin","SerialNumber":"na","ComPortName":"demo2"}]}""";
#endif
                    JsonDocument jsonDocument = JsonDocument.Parse(finalJSonConfig);
                    if (jsonDocument.RootElement.TryGetProperty("SerialPorts", out JsonElement serialPortsJsonElement))
                    {
                        var serialPorts = JsonSerializer.Deserialize<ObservableCollection<SerialPortConfigurationVM>>(serialPortsJsonElement);
                        if (serialPorts != null && serialPorts.Any())
                        {
                            foreach (var serialPort in serialPorts)
                            {
                                benchConfigurationVM.SerialPorts.Add(serialPort);
                            }
                        }
                    }

                    if (jsonDocument.RootElement.TryGetProperty("BenchComponents", out JsonElement benchComponentsJsonElement))
                    {
                        var benchComponentsTypes = System.Reflection.Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(mytype => mytype.GetInterfaces().Contains(typeof(IBenchComponent)));
                        if (benchComponentsTypes != null && benchComponentsTypes.Any())
                        {
                            foreach (var benchComponentJsonElement in benchComponentsJsonElement.EnumerateArray())
                            {
                                foreach (var item in benchComponentsTypes)
                                {
                                    item.GetMethod(nameof(IBenchComponent.InitVMFromJsonElement)).Invoke(null, new object[] {benchComponentJsonElement, benchConfigurationVM});
                                }
                            }
                        }
                    }

                }
                catch (Exception)
                {
                }

            }



            MainWindowVM = new MainWindowVM( benchConfigurationVM );

            mainWindow.DataContext = MainWindowVM;
            mainWindow.Show();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            var benchJSONConfig = new JsonObject();
            
            var serialPortsConfigJsonNode = JsonSerializer.SerializeToNode(MainWindowVM.BenchConfigurationVM.SerialPorts);
            benchJSONConfig.Add("SerialPorts", serialPortsConfigJsonNode);
            
            Collection<JsonNode> benchComponents = new ();

            foreach (var referenceInstrument in MainWindowVM.BenchConfigurationVM.ReferenceInstruments)
            {
                var benchConfig = referenceInstrument as IBenchComponent;
                if (benchConfig != null)
                {
                    var conf = benchConfig.GetConfig();
                    benchComponents.Add(conf);
                }
            }

            benchJSONConfig.Add("BenchComponents", new JsonArray(benchComponents.ToArray()));
            var finalConfig = benchJSONConfig.ToJsonString();

            File.WriteAllText(GetConfigJsonFilePath(), finalConfig);
        }
        private string GetConfigJsonFilePath()
        {
            var localAppDataFolderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            return Path.Combine(localAppDataFolderPath, "benchConfig.json");
        }
    }
}
