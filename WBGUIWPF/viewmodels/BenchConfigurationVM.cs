using inst2022;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using WBGUIWPF.Interfaces;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK;

namespace WBGUIWPF.viewmodels
{
    public class BenchConfigurationVM : BaseVM
    {
        public ObservableCollection<SerialPortConfigurationVM> SerialPorts { get; }

        public SerialPortConfigurationVM? SelectedSerialPort { get; set; }

        public ICommand AddSerialPortCommand {  get; }

        public ICommand RemoveSerialPortCommand {  get; }


        public ObservableCollection<BaseVM> ReferenceInstruments { get; }

        public BaseVM? SelectedReferenceInstrument { get; set; }

        public ICommand AddReferenceInstrumentCommand { get; }
        public ICommand RemoveReferenceInstrumentCommand { get; }
        
        public Collection <ReferenceMIConfigViewToCreate> AvailableReferenceMIToCreate { get; }

        public BenchConfigurationVM()
        {
            SerialPorts = new ObservableCollection<SerialPortConfigurationVM>();
            AddSerialPortCommand = GetAddSerialPortCommand();
            AddSerialPortCommand.Execute(null);
            AddSerialPortCommand.Execute(null);
            RemoveSerialPortCommand = GetRemoveSerialPortCommand();

            ReferenceInstruments = new ObservableCollection<BaseVM>();
            AddReferenceInstrumentCommand = GetAddReferenceInstrumentCommand();
            RemoveReferenceInstrumentCommand = GetRemoveReferenceInstrumentCommand();
            
            AvailableReferenceMIToCreate = new Collection<ReferenceMIConfigViewToCreate>() { new ReferenceMIConfigViewToCreate( "Элметро-Кельвин", () => new EKConfigurationVM("na", SerialPorts)),
                                                                                             new ReferenceMIConfigViewToCreate( "Элметро-Паскаль", () => new EPascalConfigurationVM("na", SerialPorts))};

        }

        private void OnInstrumentChangedSerialPort(object? sender, SerialPortConfigurationVM serialPortConfigurationVM) 
        {
            foreach (var instrument in ReferenceInstruments)
            {
                var instrument_with_serialPort = instrument as IInstrumentWithSerialPort;
                if (instrument_with_serialPort != null && instrument_with_serialPort.SelectedSerialPortConfiguration == serialPortConfigurationVM)
                {
                    instrument_with_serialPort.SelectedSerialPortConfiguration = null;
                }
            }
        }

        private ICommand GetAddSerialPortCommand()
        {
            return new RelayCommand(
                (o) =>
                {
                    var portname = o as string;
                    if (portname != null )
                    {
                        SerialPorts.Add(new SerialPortConfigurationVM( portname, 115200, 8, System.IO.Ports.StopBits.One, System.IO.Ports.Parity.None ));
                        RaisePropertyChanged( nameof( SerialPorts ) );
                    }
                },
                o =>
                {
                    var portname = o as string;
                    if ( portname == null ) return false;
                    if ( portname.Length == 0 ) return false;
                    if ( SerialPorts.Any( sp => sp.SerialPortName == portname ) ) return false;
                    return true;
                }
                );

        }

        private ICommand GetRemoveSerialPortCommand()
        {
            return new RelayCommand(
                (o) =>
                {
                    if ( SelectedSerialPort != null ) SerialPorts.Remove( SelectedSerialPort );
                    RaisePropertyChanged( nameof( SerialPorts ) );
                },
                o => SelectedSerialPort != null 
                     && !( ReferenceInstruments.Where( ri => ri is IInstrumentWithSerialPort ).Any( ri => ( ( IInstrumentWithSerialPort )ri ).SelectedSerialPortConfiguration == SelectedSerialPort ) )
                );

        }
        
        private ICommand GetAddReferenceInstrumentCommand()
        {
            return new RelayCommand(
                (o) =>
                {
                    var ff = o as ReferenceMIConfigViewToCreate;
                    if (ff != null)
                    {
                        var referenceInstrumentConfigurationVM = ff.GetInstrumentConfigurationVM();
                        AddReferenceInstrument(referenceInstrumentConfigurationVM);
                    }

                },
                o => o != null && o is ReferenceMIConfigViewToCreate
                );
        }
        public void AddReferenceInstrument(BaseVM referenceInstrumentConfigurationVM)
        {
                ReferenceInstruments.Add(referenceInstrumentConfigurationVM);
                if (referenceInstrumentConfigurationVM is IInstrumentWithSerialPort referenceInstrumentConfigurationVM_withSerialPort )
                {
                    referenceInstrumentConfigurationVM_withSerialPort.SerialPortChanged += OnInstrumentChangedSerialPort;
                }

        }
        private ICommand GetRemoveReferenceInstrumentCommand()
        {
            return new RelayCommand(
                (o) =>
                {
                    if (SelectedReferenceInstrument != null)
                    {
                        var referenceInstrumentConfigurationVM_withSerialPort = SelectedReferenceInstrument as IInstrumentWithSerialPort;
                        if (referenceInstrumentConfigurationVM_withSerialPort != null)
                        {
                            referenceInstrumentConfigurationVM_withSerialPort.SerialPortChanged -= OnInstrumentChangedSerialPort;
                        }
                        ReferenceInstruments.Remove(SelectedReferenceInstrument);

                    }
                },
                o => SelectedReferenceInstrument != null
                ) ;
        }
        public class ReferenceMIConfigViewToCreate
        {
            public string ReferenceMIName { get; }
            public Func<BaseVM> GetInstrumentConfigurationVM { get; }
            public ReferenceMIConfigViewToCreate(string referenceMIName, Func<BaseVM> getReferenceMIConfigViewFunc)
            {
                ReferenceMIName = referenceMIName;
                GetInstrumentConfigurationVM = getReferenceMIConfigViewFunc;
            }
        }

    }
}
