﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorkBench;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.UOMS;

namespace benchGUI
{
    public partial class MainForm
    {
        #region Current Measure Instrument

        StabilityCalculator currentStabilityCalc;

        IInstrument CurrentMeasuringInstrument;

        private bool startedEK => CurrentMeasuringInstrument != null && CurrentMeasuringInstrument.IsOpen;

        private IInstrumentChannelSpanReader currentReaderSpan { get; set; }

        private void btn_openCurrentMeasureInstrument_Click(object sender, EventArgs e)
        {
            btn_openCurrentMeasureInstrument.Enabled = false;
            switch (startedEK)
            {
                case false:
                    CurrentMeasuringInstrument = (IInstrument)cb_CurrentMeasuringInstruments.SelectedItem;

                    Task.Run(() =>
                    {
                        if (CurrentMeasuringInstrument.Open())
                        {
                            InvokeControlAction(Current_Instrument_Start);
                        }
                        InvokeControlAction(() => btn_openCurrentMeasureInstrument.Enabled = true);
                    });
                    break;
                case true:
                    StopCurrentCyclicReading();
                    CurrentMeasuringInstrument.Close();
                    btn_openCurrentMeasureInstrument.Text = "Установить связь";
                    btn_openCurrentMeasureInstrument.BackColor = Control.DefaultBackColor;
                    lbl_cnahValue.Text = "--.----";
                    cb_CurrentInstrumentChannels.Items.Clear();
                    cb_CurrentInstrumentChannels.Enabled = false;
                    cb_currentReaderSpan.Items.Clear();
                    cb_currentReaderSpan.Enabled = false;

                    currentStabilityCalc.Reset();
                    lbl_cnahValue.BackColor = Color.Transparent;

                    btn_openCurrentMeasureInstrument.Enabled = true;


                    break;
            }
        }

        private void Current_Instrument_Start()
        {
            //для обновления информации, считанной из прибора, например, серийный номер
            cb_CurrentMeasuringInstruments.Items.Remove(CurrentMeasuringInstrument);
            cb_CurrentMeasuringInstruments.Items.Add(CurrentMeasuringInstrument);
            cb_CurrentMeasuringInstruments.SelectedItem = CurrentMeasuringInstrument;

            btn_openCurrentMeasureInstrument.Text = "Разорвать связь";
            btn_openCurrentMeasureInstrument.BackColor = Color.LightYellow;
            cb_CurrentInstrumentChannels.Items.Clear();
            cb_CurrentInstrumentChannels.Items.AddRange(CurrentMeasuringInstrument.Channels);

            if (cb_CurrentInstrumentChannels.Items.Count > 0)
            {
                cb_CurrentInstrumentChannels.SelectedIndex = 0;
                if (cb_CurrentInstrumentChannels.Items.Count > 1)
                {
                    cb_CurrentInstrumentChannels.Enabled = true;
                }
                else
                {
                    cb_CurrentInstrumentChannels.Enabled = false;
                }
            }
        }
        private void cb_currentReaderSpan_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopCurrentCyclicReading();
            currentStabilityCalc.Reset();
            currentReaderSpan = ((IInstrumentChannelSpanReader)((ComboBox)sender).SelectedItem);

            StartCurrentCyclicReading();
        }
        private void cb_chanNUM_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopCurrentCyclicReading();

            cb_currentReaderSpan.Items.Clear();
            if (sender is ComboBox cb)
            {
                if (cb.SelectedItem is IInstrumentChannel instrChan)
                {
                    foreach (var span in instrChan.AvailableSpans.Where(sp => sp.Scale.UOM.UOMType == UOMType.Current || sp.Scale.UOM.UOMType == UOMType.Temperature))
                    {
                        cb_currentReaderSpan.Items.Add(span);
                    }
                }
            }

            if (cb_currentReaderSpan.Items.Count > 0)
            {
                cb_currentReaderSpan.SelectedIndex = 0;
            }

            if (cb_currentReaderSpan.Items.Count > 1)
            {
                cb_currentReaderSpan.Enabled = true;
            }
            else
            {
                cb_currentReaderSpan.Enabled = false;
            }
        }

        CancellationTokenSource CurrentCyclicReadingCTS { get; set; }
        private IUOM currentInstrumentUom { get; } = new mA();
        private void StartCurrentCyclicReading()
        {
            CurrentCyclicReadingCTS = new CancellationTokenSource();
            var token = CurrentCyclicReadingCTS.Token;
            Task.Run(() => CurrentCyclicReaderLoop(token));
        }

        private void StopCurrentCyclicReading()
        {
            CurrentCyclicReadingCTS?.Cancel();
            CurrentCyclicReadingCTS?.Dispose();
            CurrentCyclicReadingCTS = null;
        }

        private void CurrentCyclicReaderLoop(CancellationToken token)
        { 
                while (!token.IsCancellationRequested)
                {
                    var CurrentOneMeasure = currentReaderSpan.Read(currentInstrumentUom);
                    if (CurrentOneMeasure != null && !token.IsCancellationRequested)
                        OnOneCurrentMeasureReaded(null, CurrentOneMeasure);
                }
        }

        private void OnOneCurrentMeasureReaded(object sender, OneMeasure oneMeasure)
        {
            currentStabilityCalc.AddMeasure(oneMeasure);

            if (currentStabilityCalc.TrendStatus == TrendStatus.Unknown)
            {
                InvokeControlAction(
                    () => {
                        lbl_EKstability.Text = $"{currentStabilityCalc.MeasuresCount}/{currentStabilityCalc.MeasuringTimeSpan.TotalSeconds:N0}s";
                        lbl_ekmean.Text = "----";
                        lbl_EKstdev.Text = "----";
                        lbl_EKLRSlope.Text = "----";
                    });
                
            }
            else
            {
                InvokeControlAction(() => { 
                    lbl_ekmean.Text = currentStabilityCalc.MeanValue.ToWBFloatString();
                    lbl_EKstdev.Text = currentStabilityCalc.StdDeviation.ToWBFloatString();
                    lbl_EKLRSlope.Text = currentStabilityCalc.LRSlope.ToWBFloatString();
                    lbl_EKstability.Text = currentStabilityCalc.GetStatusTextRu();
                });

            }

            Color backColor = Color.Transparent;
            if (currentStabilityCalc.TrendStatus == TrendStatus.Stable) backColor = Color.Yellow;
            if (currentStabilityCalc.Ready) backColor = Color.GreenYellow;
            
            lbl_cnahValue.BackColor = backColor;

            switch (showCurrentInPressureUnits)
            {
                case false:
                    InvokeControlAction(() => lbl_cnahValue.Text = $"{oneMeasure.Value.ToWBFloatString()} {oneMeasure.UOM.Name}" );
                    break;
                case true:
                    var ma = oneMeasure.Value;
                    var press = (ma - 4) / 16 * (pressureScaleMax - pressureScaleMin) + pressureScaleMin;
                    InvokeControlAction(() => lbl_cnahValue.Text = $"{press:N2} {selectedPressureUOM.Name}");
                    break;
            }

            fillMeasuresChart();

        }

        private bool showCurrentInPressureUnits { get; set; }
        private void lbl_cnahValue_Click(object sender, EventArgs e)
        {
            switch (showCurrentInPressureUnits)
            {
                case true:
                    showCurrentInPressureUnits = false;
                    break;
                case false:
                    if (!string.IsNullOrEmpty(tbScaleMin.Text) && !string.IsNullOrEmpty(tbScaleMax.Text))
                    {
                        showCurrentInPressureUnits = true;
                    }
                    break;
            }
        }

        private async void SearchEKOnSerialPorts()
        {
            lbl_cnahValue.Text = "поиск....";

            foreach (var item in await Factory.serialPortNamesWithEK())
            {
                cb_CurrentMeasuringInstruments.Items.Add(item);
            }

            if (cb_CurrentMeasuringInstruments.Items.Count > 0)
            {
                cb_CurrentMeasuringInstruments.SelectedItem = cb_CurrentMeasuringInstruments.Items[0];

                lbl_cnahValue.Text = "---.----";

                if (cb_CurrentMeasuringInstruments.Items.Count == 1)
                {
                    cb_CurrentMeasuringInstruments.Enabled = false;
                }
            }
            else
            {
                lbl_cnahValue.Font = new Font(

                    lbl_cnahValue.Font.FontFamily,

                    (float)(lbl_cnahValue.Font.Size * 0.7),

                    lbl_cnahValue.Font.Style);

                lbl_cnahValue.Text = "не найдено :(";

                cb_CurrentInstrumentChannels.Enabled = false;

                cb_CurrentMeasuringInstruments.Enabled = false;
            }

        }

        #endregion
    }
}
