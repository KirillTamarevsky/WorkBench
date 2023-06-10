using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

        private IInstrumentChannelSpanReader currentReaderSpan;

        private CyclicChannelSpanReader CurrentChannelSpanCyclicReader;

        private void btn_openCurrentMeasureInstrument_Click(object sender, EventArgs e)
        {
            switch (startedEK)
            {
                case false:
                    CurrentMeasuringInstrument = (IInstrument)cb_CurrentMeasuringInstruments.SelectedItem;
                    if (CurrentMeasuringInstrument.Open())
                    {
                        //для обновления информации, считанной из прибора, например, серийный номер
                        cb_CurrentMeasuringInstruments.Items.Remove(CurrentMeasuringInstrument);
                        cb_CurrentMeasuringInstruments.Items.Add(CurrentMeasuringInstrument);
                        cb_CurrentMeasuringInstruments.SelectedItem = CurrentMeasuringInstrument;

                        btn_openCurrentMeasureInstrument.Text = "Разорвать связь";
                        btn_openCurrentMeasureInstrument.BackColor = Color.LightYellow;
                        cb_CurrentInstrumentChannels.Items.Clear();
                        cb_CurrentInstrumentChannels.Items.AddRange( CurrentMeasuringInstrument.Channels );

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
                    break;
                case true:
                    StopCurrentCyclicReading();
                    CurrentMeasuringInstrument.Close();
                    btn_openCurrentMeasureInstrument.Text = "Установить связь";
                    btn_openCurrentMeasureInstrument.BackColor = Control.DefaultBackColor;
                    lbl_cnahValue.Text = "--.----";
                    cb_CurrentInstrumentChannels.Items.Clear();
                    cb_CurrentInstrumentChannels.Enabled=false;
                    cb_currentReaderSpan.Items.Clear();
                    cb_currentReaderSpan.Enabled=false;

                    currentStabilityCalc.Reset();
                    lbl_cnahValue.BackColor = Color.Transparent;

                    break;
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

            foreach (var span in (((IInstrumentChannel)((ComboBox)sender).SelectedItem)).AvailableSpans.Where(sp => sp.Scale.UOM.UOMType == WorkBench.Enums.UOMType.Current))
            {
                cb_currentReaderSpan.Items.Add(span);
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
        private void StartCurrentCyclicReading()
        {
            CurrentChannelSpanCyclicReader = new CyclicChannelSpanReader(currentReaderSpan, new mA());
            CurrentChannelSpanCyclicReader.OneMeasureReaded += OnOneCurrentMeasureReaded;
            CurrentChannelSpanCyclicReader.Start();
        }


        private void StopCurrentCyclicReading()
        {
            if (CurrentChannelSpanCyclicReader != null)
            {
                CurrentChannelSpanCyclicReader.OneMeasureReaded -= OnOneCurrentMeasureReaded;
                CurrentChannelSpanCyclicReader.Stop();
                CurrentChannelSpanCyclicReader=null;
            }

        }

        private void OnOneCurrentMeasureReaded(object sender, OneMeasure oneMeasure)
        {
            currentStabilityCalc.AddMeasure(oneMeasure);
            Color backColor = Color.Transparent;

            if (currentStabilityCalc.TrendStatus == TrendStatus.Unknown)
            {

                setLabelText($"{currentStabilityCalc.MeasuresCount}/{currentStabilityCalc.MeasuringTimeSpan.TotalSeconds:N0}s", lbl_EKstability);
                
                setLabelText("----", lbl_ekmean);
                setLabelText("----", lbl_EKstdev);
                setLabelText("----", lbl_EKLRSlope);
            }
            else
            {
                setLabelText(currentStabilityCalc.MeanValue.ToString("N4"), lbl_ekmean);
                setLabelText(currentStabilityCalc.StdDeviation.ToString("N4"), lbl_EKstdev);
                setLabelText(currentStabilityCalc.LRSlope.ToString("N4"), lbl_EKLRSlope);
                
                switch (currentStabilityCalc.TrendStatus)
                {
                    case TrendStatus.Unknown:
                        setLabelText("неизвестно", lbl_EKstability);
                        break;
                    case TrendStatus.GrowUP:
                        setLabelText("увеличивается", lbl_EKstability);
                        break;
                    case TrendStatus.FallDown:
                        setLabelText("уменьшается", lbl_EKstability);
                        break;
                    case TrendStatus.Stable:
                        setLabelText("стабильно", lbl_EKstability);
                        backColor = Color.Yellow;
                        break;
                    default:
                        break;
                }
            }
            if (currentStabilityCalc.Ready)
            {
                backColor = Color.GreenYellow;
            }
            
            lbl_cnahValue.BackColor = backColor;

            switch (showCurrentInPressureUnits)
            {
                case false:
                    setLabelText($"{oneMeasure.Value:N4} {oneMeasure.UOM.Name}", lbl_cnahValue);
                    break;
                case true:
                    var ma = oneMeasure.Value;
                    var press = (ma - 4) / 16 * (pressureScaleMax - pressureScaleMin) + pressureScaleMin;
                    setLabelText($"{press:N2} {selectedPressureUOM.Name}", lbl_cnahValue);
                    break;
            }

            fillMeasuresChart();

        }

        private bool showCurrentInPressureUnits;
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
