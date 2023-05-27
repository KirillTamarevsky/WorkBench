using inst2022;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WBGUIWPF.Interfaces;
using WorkBench.Interfaces;

namespace WBGUIWPF.viewmodels
{
    public class MainWindowVM : BaseVM
    {
        public BenchConfigurationVM BenchConfigurationVM { get; internal set; }
        PressureInstrumentCalibrationVM pressureInstrumentCalibrationVM;
        public BaseVM CurrentVM { get; private set; }
        public ICommand SwitchToConfigCommand { get; }
        public ICommand SwitchToPressureCalibrationCommand { get; }
        public MainWindowVM( BenchConfigurationVM benchConfigurationVM)
        {
            BenchConfigurationVM = benchConfigurationVM;
            CurrentVM = BenchConfigurationVM;
            SwitchToConfigCommand = GetSwitchToConfigurationCommand();
            SwitchToPressureCalibrationCommand = GetSwitchToPressureCalibrationCommand();
        }
        private ICommand GetSwitchToConfigurationCommand()
        {
            return new RelayCommand(
                o =>
                {

                    if (pressureInstrumentCalibrationVM != null)
                    {
                        pressureInstrumentCalibrationVM.Dispose();
                        pressureInstrumentCalibrationVM = null;
                    }

                    CurrentVM = BenchConfigurationVM;
                    RaisePropertyChanged(nameof(CurrentVM));
                },
                o => !(CurrentVM is BenchConfigurationVM)
                );
        }
        private ICommand GetSwitchToPressureCalibrationCommand()
        {
            return new RelayCommand(
                o =>
                {
                    List<IInstrument> availableInstruments = new();
                    foreach (var referenceInstrument in BenchConfigurationVM.ReferenceInstruments)
                    {
                        var instrumentCreator = referenceInstrument as IInstrumentCreator;
                        if (instrumentCreator != null)
                        {
                            var instrument = instrumentCreator.GetInstrument();
                            if (instrument != null ) availableInstruments.Add(instrument);
                        }
                    }
                    pressureInstrumentCalibrationVM = new PressureInstrumentCalibrationVM(availableInstruments);
                    CurrentVM = pressureInstrumentCalibrationVM;
                    RaisePropertyChanged(nameof(CurrentVM));
                },
                o => !(CurrentVM is PressureInstrumentCalibrationVM) && BenchConfigurationVM.ReferenceInstruments.Where(i => i is IInstrumentCreator).Cast<IInstrumentCreator>().All(i => i.CanGetInstrument)
                );
        }
    }
}
