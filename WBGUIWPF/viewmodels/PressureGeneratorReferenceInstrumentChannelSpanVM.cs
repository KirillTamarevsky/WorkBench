using inst2022;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WorkBench;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace WBGUIWPF.viewmodels
{
    public class PressureGeneratorReferenceInstrumentChannelSpanVM : BaseVM, IDisposable
    {
        private IInstrumentChannelSpanPressureGenerator _instrumentChannelSpanPressureGenerator { get; }
        private ICyclicChannelSpanReader _channelSpanReader { get; }
#if DEBUG
        [Obsolete("Only for WPF Design", true)]
        public PressureGeneratorReferenceInstrumentChannelSpanVM()
        {
            LastMeasure = "test";
            var epascal = Factory.GetFakeEPascal("demodemo");
                epascal.Open();
                _instrumentChannelSpanPressureGenerator = epascal.Channels.FirstOrDefault().AvailableSpans.Where(sp => sp is IInstrumentChannelSpanPressureGenerator).Cast<IInstrumentChannelSpanPressureGenerator>().FirstOrDefault();
            var spanReader = _instrumentChannelSpanPressureGenerator as IInstrumentChannelSpanReader;
            if (spanReader != null)
            {
                LastMeasure = "spanReader != null";
                SetPressureModeStandByCommand = GetSetPressureModeStandByCommand();
                SetPressureModeMeasureCommand = GetSetPressureModeMeasureCommand();
                SetPressureModeControlCommand = GetSetPressureModeControlCommand();
                SetPressureModeVentCommand = GetSetPressureModeVentCommand();

                _channelSpanReader = new CyclicChannelSpanReader(spanReader, new kPa());
                _channelSpanReader.OneMeasureReaded += _onNewMeasureReaded;
                _channelSpanReader.Start();
            }
        }
#endif
        public PressureGeneratorReferenceInstrumentChannelSpanVM(IInstrumentChannelSpanPressureGenerator instrumentChannelSpanPressureGenerator)
        {
            LastMeasure = "подключение...";
            SetPressureModeStandByCommand = GetSetPressureModeStandByCommand();
            SetPressureModeMeasureCommand = GetSetPressureModeMeasureCommand();
            SetPressureModeControlCommand = GetSetPressureModeControlCommand();
            SetPressureModeVentCommand = GetSetPressureModeVentCommand();
            _instrumentChannelSpanPressureGenerator = instrumentChannelSpanPressureGenerator;
            if (_instrumentChannelSpanPressureGenerator is IInstrumentChannelSpanReader spanReader)
            {
                _channelSpanReader = new CyclicChannelSpanReader(spanReader, new kPa());
                _channelSpanReader.OneMeasureReaded += _onNewMeasureReaded;
                _channelSpanReader.Start();
            }
        }
        private string _lastMeasure;
        public string LastMeasure { get => _lastMeasure; private set
            {
                _lastMeasure = value;
                RaisePropertyChanged(nameof(LastMeasure));
            }
        }
        private void _onNewMeasureReaded (object? sender, OneMeasure oneMeasure)
        {
            if (oneMeasure != null)
            {
                LastMeasure = $"{oneMeasure.Value.ToString("N4")} {oneMeasure.UOM}";
            }
        }
        public ICommand SetPressureModeStandByCommand { get; }
        public ICommand SetPressureModeMeasureCommand {get;}
        public ICommand SetPressureModeControlCommand {get;}
        public ICommand SetPressureModeVentCommand { get;}
        private ICommand GetSetPressureModeStandByCommand()
        {
            return new RelayCommand(
                o => _instrumentChannelSpanPressureGenerator.PressureOperationMode = WorkBench.Enums.PressureControllerOperationMode.STANDBY);
        }
        private ICommand GetSetPressureModeMeasureCommand()
        {
            return new RelayCommand(
                o => _instrumentChannelSpanPressureGenerator.PressureOperationMode = WorkBench.Enums.PressureControllerOperationMode.MEASURE);
        }
        private ICommand GetSetPressureModeControlCommand()
        {
            return new RelayCommand(
                o => _instrumentChannelSpanPressureGenerator.PressureOperationMode = WorkBench.Enums.PressureControllerOperationMode.CONTROL);
        }
        private ICommand GetSetPressureModeVentCommand()
        {
            return new RelayCommand(
                o => _instrumentChannelSpanPressureGenerator.PressureOperationMode = WorkBench.Enums.PressureControllerOperationMode.VENT);
        }

        public void Dispose()
        {
            if (_channelSpanReader != null)
            {
                _channelSpanReader.OneMeasureReaded -= _onNewMeasureReaded;
                _channelSpanReader.Stop();
            }
        }
    }
}
