using inst2022;
using Microsoft.AspNetCore.Http.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
    internal class PressureInstrumentCalibrationVM : BaseVM, IDisposable
    {
        List<IInstrument> ReferenceInstruments { get; }
#if DEBUG
        [Obsolete("Only for design data", true)]
        public PressureInstrumentCalibrationVM() : this(new List<IInstrument> { Factory.GetFakeEPascal("444"), Factory.GetFakeEK("333") })
        { }
#endif
        public PressureInstrumentCalibrationVM(List<IInstrument> referenceInstruments)
        {
            ReferenceInstruments = referenceInstruments;
            ReferenceInstruments.ForEach(i => i.Open());
            RaisePropertyChanged(nameof(AvailableReferencePressureGeneratorChannels));
            StartCurrentChannelRead = new RelayCommand(
                (o) => 
                {
                    CurrentChannelSpanCyclicReader = new CyclicChannelSpanReader(SelectedReferenceCurrentMeasuringChannel, new mA());
                    CurrentChannelSpanCyclicReader.OneMeasureReaded += OnCurrentChannelSpanNewValueReaded;
                    CurrentChannelSpanCyclicReader.Start();
                }
                , o => SelectedReferenceCurrentMeasuringChannel != null && CurrentChannelSpanCyclicReader == null );
            StopCurrentChannelRead = new RelayCommand(
                (o) =>
                {
                    if (CurrentChannelSpanCyclicReader != null)
                    {
                        CurrentChannelSpanCyclicReader.OneMeasureReaded -= OnCurrentChannelSpanNewValueReaded;
                        CurrentChannelSpanCyclicReader.Stop();
                        CurrentChannelSpanCyclicReader = null;
                    }
                }
                , o => CurrentChannelSpanCyclicReader != null);

            StartPressureChannelRead = new RelayCommand(
                (o) =>
                {
                    var pressGenerator = SelectedReferencePressureMeasuringChannel as IInstrumentChannelSpanPressureGenerator;
                    if (pressGenerator != null)
                        PressureGeneratorReferenceInstrumentChannelSpanVM = new PressureGeneratorReferenceInstrumentChannelSpanVM(pressGenerator);
                }
                , o => 
                SelectedReferencePressureMeasuringChannel != null 
                && SelectedReferencePressureMeasuringChannel is IInstrumentChannelSpanPressureGenerator 
                && PressureGeneratorReferenceInstrumentChannelSpanVM == null );
            StopPressureChannelRead = new RelayCommand(
                (o) =>
                {
                    if (PressureGeneratorReferenceInstrumentChannelSpanVM != null)
                    {
                        PressureGeneratorReferenceInstrumentChannelSpanVM.Dispose();
                        PressureGeneratorReferenceInstrumentChannelSpanVM = null;
                    }
                }
                , o => PressureGeneratorReferenceInstrumentChannelSpanVM != null);
        }

        private void OnCurrentChannelSpanNewValueReaded(object? sender, OneMeasure e)
        {
            LastCurrentMeasure = e;
            RaisePropertyChanged(nameof(LastCurrentMeasure));
        }


        public List<IInstrumentChannelSpanPressureGenerator> AvailableReferencePressureGeneratorChannels 
        {
            get 
            {
                var aaa = ReferenceInstruments.SelectMany(i => i.Channels).ToList();
                var bbb = aaa.SelectMany(ic => ic.AvailableSpans).ToList().Where(avsp => avsp is IInstrumentChannelSpanPressureGenerator).Cast<IInstrumentChannelSpanPressureGenerator>().ToList();
                return bbb;
            }
        }
        public List<IInstrumentChannelSpanReader> AvailableReferenceCurrentMeasuringChannels
        {
            get
            {
                var aaa = ReferenceInstruments.SelectMany(i => i.Channels).ToList();
                var bbb = aaa.SelectMany(ic => ic.AvailableSpans).ToList().Where(avsp => avsp is IInstrumentChannelSpanReader && avsp.Scale.UOM.UOMType == WorkBench.Enums.UOMType.Current).Cast<IInstrumentChannelSpanReader>().ToList();
                return bbb;
            }
        }
        public IInstrumentChannelSpanReader? SelectedReferenceCurrentMeasuringChannel
        { get; set; }
        public ICyclicChannelSpanReader? CurrentChannelSpanCyclicReader
        { get; private set; }
        public OneMeasure? LastCurrentMeasure { get; private set; }
        public ICommand StartCurrentChannelRead { get; }
        public ICommand StopCurrentChannelRead { get; }

        private IInstrumentChannelSpanReader? _selectedReferencePressureMeasuringChannel;
        public IInstrumentChannelSpanReader? SelectedReferencePressureMeasuringChannel
        { get => _selectedReferencePressureMeasuringChannel;
            set
            {
                if (PressureGeneratorReferenceInstrumentChannelSpanVM != null)
                {
                    StopPressureChannelRead.Execute(null);
                    _selectedReferencePressureMeasuringChannel = value;
                    if (StartPressureChannelRead.CanExecute(null))
                    {
                        StartPressureChannelRead.Execute(null);
                    }
                }
                else
                {
                    _selectedReferencePressureMeasuringChannel = value;
                }
                RaisePropertyChanged(nameof(SelectedReferencePressureMeasuringChannel));
            }
        }
        
        private PressureGeneratorReferenceInstrumentChannelSpanVM? _pressureGeneratorReferenceInstrumentChannelSpanVM;
        public PressureGeneratorReferenceInstrumentChannelSpanVM? PressureGeneratorReferenceInstrumentChannelSpanVM 
        { get => _pressureGeneratorReferenceInstrumentChannelSpanVM; set { _pressureGeneratorReferenceInstrumentChannelSpanVM = value; RaisePropertyChanged(nameof(PressureGeneratorReferenceInstrumentChannelSpanVM)); } }
        public ICommand StartPressureChannelRead { get; }
        public ICommand StopPressureChannelRead { get; }
        public void Dispose()
        {
            StopCurrentChannelRead.Execute(null);
            StopPressureChannelRead.Execute(null);
            ReferenceInstruments.ForEach(i => i.Close());
        }
    }
}
