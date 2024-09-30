using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
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
        StabilityCalculator pressureStabilityCalc;

        IInstrument PressureInstrument;

        private bool startedPressureInstrument => PressureInstrument != null && PressureInstrument.IsOpen;

        private IInstrumentChannelSpanReader pressureReaderSpan { get; set; }

        private IUOM selectedPressureUOM { get; set; }

        private IInstrumentChannelSpanPressureGenerator pressureGeneratorSpan { get; set; }

        private double pressureScaleMin;

        private double pressureScaleMax;

        #region PressureInstrument

        private void btn_openPressureMeasureInstrument_Click(object sender, EventArgs e)
        {
            btn_openPressureInstrument.Enabled = false;
            switch (startedPressureInstrument)
            {
                case false:
                    PressureInstrument = (IInstrument)cb_PressureGeneratorInstrument.SelectedItem;

                    lbl_cpcStatus.Text = "поиск...";
                    Task.Factory.StartNew(() =>
                    { 
                        if ( PressureInstrument.Open())
                        {
                            InvokeControlAction( () =>PressureInstrument_start());
                        }
                        else
                        {
                            InvokeControlAction( () => lbl_cpcStatus.Text = "нет связи");
                        }
                        InvokeControlAction( () => btn_openPressureInstrument.Enabled = true);
                    });

                    break;
                case true:
                    PressureInstrument_stop();
                    PressureInstrument.Close();
                    pressureStabilityCalc.Reset();
                    lbl_cpc_read.BackColor = Color.Transparent;
                    btn_openPressureInstrument.Enabled = true;
                    break;
            }

        }
        #region PressureInstrument start/stop routines

        private void PressureInstrument_start()
        {
            //*************************************************************

            lbl_cpcStatus.Text = "ок.";

            cb_cpcChannels.Items.Clear();
            foreach (var item in PressureInstrument.Channels)
            {
                cb_cpcChannels.Items.Add(item);
            }

            if (cb_cpcChannels.Items.Count > 0)
            {
                cb_cpcChannels.SelectedIndex = 0;
            }

            btn_openPressureInstrument.Text = "Разорвать связь";
            //*************************************************************

            btn_setSetPoint.Enabled = true;
            btn_StepUp.Enabled = true;
            btn_StepDown.Enabled = true;
            btn_pressureMicrostepUP.Enabled = true;
            btn_pressureMicroStepDown.Enabled = true;
            btn_ZeroPressure.Enabled = true;
            tb_cpcStep.Enabled = true;
            tb_pressureMicroStep.Enabled = true;
            tb_newSetPoint.Enabled = true;
            cb_cpcChannels.Enabled = true;
        }

        private void PressureInstrument_stop()
        {
            //*************************************************************

            lbl_cpcStatus.Text = "нет связи.";

            cb_cpcChannels.Items.Clear();

            //cb_PressureReaderGeneratorSpan.Items.Clear();
            PressureGeneratorSpansBL.Clear();

            btn_openPressureInstrument.Text = "Установить связь";
            //*************************************************************

            StopPressureCyclicRead();

            pressureReaderSpan = null;

            pressureGeneratorSpan = null;

            lbl_cpc_read.Text = "---.----";
            btn_setSetPoint.Enabled = false;
            btn_StepUp.Enabled = false;
            btn_StepDown.Enabled = false;
            btn_pressureMicrostepUP.Enabled = false;
            btn_pressureMicroStepDown.Enabled = false;
            btn_ZeroPressure.Enabled = false;

            tb_cpcStep.Enabled = false;
            tb_pressureMicroStep.Enabled = false;
            tb_newSetPoint.Enabled = false;
            cb_cpcChannels.Enabled = false;
            cb_PressureReaderGeneratorSpan.Enabled = false;


        }

        #endregion

        #region CPC Step UP DOWN buttons

        private void btn_StepUp_Click(object sender, EventArgs e)
        {
            double CoarsePressureStep = getPressureCoarseStepValue();
            double SetPoint = getPressureSetPointValue();
            setSetPoint(SetPoint + CoarsePressureStep);

        }
        private void btn_StepDown_Click(object sender, EventArgs e)
        {
            double CoarsePressureStep = getPressureCoarseStepValue();
            double SetPoint = getPressureSetPointValue();

            setSetPoint(SetPoint - CoarsePressureStep);
        }
        private void btn_pressureMicrostepUP_Click(object sender, EventArgs e)
        {
            double FinePressureStep = getPressureFineStepValue();
            double SetPoint = getPressureSetPointValue();

            setSetPoint(SetPoint + FinePressureStep);
        }
        private void btn_pressureMicroStepDown_Click(object sender, EventArgs e)
        {
            double FinePressureStep = getPressureFineStepValue();
            double SetPoint = getPressureSetPointValue();

            setSetPoint(SetPoint - FinePressureStep);

        }
        double getPressureCoarseStepValue()
        {
            return tb_cpcStep.Text.ParseToDouble();
        }
        double getPressureSetPointValue()
        {
            return tb_PressureSetPoint.Text.ParseToDouble();
        }

        double getPressureFineStepValue()
        {
            return tb_pressureMicroStep.Text.ParseToDouble();
        }


        #endregion

        private void tb_cpcStep_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text.IsFloatString())
            {
                btn_StepUp.Enabled = true;
                btn_StepDown.Enabled = true;
            }
            else
            {
                btn_StepUp.Enabled = false;
                btn_StepDown.Enabled = false;
            }
        }
        private void tb_pressureMicroStep_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text.IsFloatString())
            {
                btn_pressureMicrostepUP.Enabled = true;
                btn_pressureMicroStepDown.Enabled = true;
            }
            else
            {
                btn_pressureMicrostepUP.Enabled = false;
                btn_pressureMicroStepDown.Enabled = false;
            }
        }

        private void tb_newSetPoint_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Text.TryParseToDouble(out double parsedSP))
            {
                newSetPoint = parsedSP;
                btn_setSetPoint.Enabled = true;
            }
            else
            {
                btn_setSetPoint.Enabled = false;
            }

        }
        private void tb_newSetPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && btn_setSetPoint.Enabled == true)
            {
                setSetPoint(newSetPoint);
            }
        }

        private void btn_setSetPoint_Click(object sender, EventArgs e)
        {
            setSetPoint(newSetPoint);
        }


        double newSetPoint { get; set; }

        private void setSetPoint(double newSetPoint)
        {
            if (pressureGeneratorSpan == null ) return;

            //var maxPressureSetValue = pressureGeneratorSpan.Scale.Max * (pressureGeneratorSpan.Scale.UOM.Factor / selectedPressureUOM.Factor );

            if (tbScaleMin.Text.TryParseToDouble(out pressureScaleMin) 
                && 
                tbScaleMax.Text.TryParseToDouble(out pressureScaleMax)
                )
            {
                var fullscale = pressureScaleMax - pressureScaleMin;
                
                var minminsetpoint = pressureScaleMin - fullscale * 0.1;
                var maxmaxsetpoint = fullscale * 1.1 + pressureScaleMin;
                
                if (newSetPoint < minminsetpoint) newSetPoint = minminsetpoint;
                
                if (newSetPoint > maxmaxsetpoint) newSetPoint = maxmaxsetpoint;
            }

            pressureGeneratorSpan.SetPoint = new OneMeasure(newSetPoint, selectedPressureUOM);

            InvokeControlAction(() => tb_PressureSetPoint.Text = pressureGeneratorSpan.SetPoint.Value.ToWBFloatString());

        }

        private void setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode operationMode)
        {
            if (pressureGeneratorSpan == null) return;
            Task.Run(() => pressureGeneratorSpan.PressureOperationMode = operationMode);

        }

        #region Pressure Instrument MODE Radio Buttons

        private void rb_Control_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                btn_ZeroPressure.Enabled = false;
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.CONTROL);
            }
        }

        private void rb_Vent_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                btn_ZeroPressure.Enabled = true;
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.VENT);
            }
        }

        private void rb_Measure_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                btn_ZeroPressure.Enabled = false;
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.MEASURE);
            }
        }

        private void rb_StandBY_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                btn_ZeroPressure.Enabled = false;
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.STANDBY);
            }
        }
        #endregion

        private BindingList<object> PressureGeneratorSpansBL = new BindingList<object>();
        private void cb_cpcChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopPressureCyclicRead();

            PressureGeneratorSpansBL.Clear();
            var bindingSource = new BindingSource();
            bindingSource.DataSource = PressureGeneratorSpansBL;
            bindingSource.RaiseListChangedEvents = true;
            cb_PressureReaderGeneratorSpan.DataSource = bindingSource;
            cb_PressureReaderGeneratorSpan.DisplayMember = "Scale";
            foreach (var span in (((IInstrumentChannel)((ComboBox)sender).SelectedItem)).AvailableSpans)
            {
                PressureGeneratorSpansBL.Add(span);
            }

            if (cb_PressureReaderGeneratorSpan.Items.Count > 0)
            {
                cb_PressureReaderGeneratorSpan.SelectedIndex = 0;
            }

            cb_PressureReaderGeneratorSpan.Enabled = false;
            if (cb_PressureReaderGeneratorSpan.Items.Count > 1)
            {
                cb_PressureReaderGeneratorSpan.Enabled = true;
            }
            cb_PressureReaderGeneratorSpan_SelectedIndexChanged(cb_PressureReaderGeneratorSpan, new EventArgs());
        }
        private void cb_PressureReaderGeneratorSpan_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopPressureCyclicRead();
            pressureStabilityCalc.Reset();

            if (pressureReaderSpan is INotifyPropertyChanged prs)
            {
                prs.PropertyChanged -= updatepressureCBX;
            }
            if (((ComboBox)sender).SelectedItem is IInstrumentChannelSpanReader presReaderSpanSelected)
            {
                pressureReaderSpan = presReaderSpanSelected;


                if (pressureReaderSpan is INotifyPropertyChanged prs1)
                {
                    prs1.PropertyChanged += updatepressureCBX;
                }

                pressureGeneratorSpan = (IInstrumentChannelSpanPressureGenerator)pressureReaderSpan;

                tb_PressureSetPoint.Text = pressureGeneratorSpan.SetPoint.Value.ToWBFloatString();

                ReadPressureInstrumentOperationModeToRadioButtons();

                StartPressureCyclicRead();
            }
        }

        private void updatepressureCBX(object sender, PropertyChangedEventArgs e)
        {
                InvokeControlAction(() =>
                {
                    PressureGeneratorSpansBL.ResetBindings();
                });
        }

        CancellationTokenSource PressureCyclicReadingCTS { get; set; }
        private void StartPressureCyclicRead()
        {
            if (pressureReaderSpan != null && selectedPressureUOM.UOMType == UOMType.Pressure)
            {
                PressureCyclicReadingCTS = new CancellationTokenSource();
                var token = PressureCyclicReadingCTS.Token;
                Task.Run(() =>
                                {
                                    while (!token.IsCancellationRequested)
                                    {
                                        var PressureOneMeasure = pressureReaderSpan.Read(selectedPressureUOM);
                                        if (PressureOneMeasure != null && !token.IsCancellationRequested)
                                            OnOnePressureMeasureReaded(null, PressureOneMeasure);
                                    }
                                }
                        );

            }
        }

        private void OnOnePressureMeasureReaded(object sender, OneMeasure onemeasure)
        {
            pressureStabilityCalc.AddMeasure(onemeasure);
            pressureMeasures.Add(onemeasure);
            var currentTime = DateTime.Now;
            var startTime = currentTime.AddSeconds(-TIMETOSTABLE);

            pressureMeasures.RemoveAll(m => m.TimeStamp < startTime);
            InvokeControlAction(() =>
            {
                try
                {
                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    if (!plot_result.Plot.GetPlottables().Contains(pressureMeasuresScatterPlot))
                    {
                        plot_result.Plot.Add(pressureMeasuresScatterPlot);
                    }
                    if (pressureMeasures != null && pressureMeasures.Count > 0)
                    {
                        //plot_measures.Plot.XAxis.DateTimeFormat(true);
                        var pressureMeasurePointsToPlot = pressureMeasures;
                        double[] xs = pressureMeasurePointsToPlot.Select(m => m.TimeStamp.ToOADate()).ToArray();
                        double[] ys = pressureMeasurePointsToPlot.Select(m => m.Value).ToArray();
                        pressureMeasuresScatterPlot.Update(xs, ys);
                        pressureMeasuresScatterPlot.MarkerShape = MarkerShape.none;
                        if (
                            tbScaleMin.Text.TryParseToDouble(out pressureScaleMin)
                            &
                            tbScaleMax.Text.TryParseToDouble(out pressureScaleMax)
                            )
                        {
                            var fullscale = pressureScaleMax - pressureScaleMin;
                            plot_result.Plot.SetAxisLimits(yMin: pressureScaleMin - fullscale * 0.05, yMax: pressureScaleMax + fullscale * 0.05, yAxisIndex: YPressureAxis.AxisIndex);
                        }
                    }
                    YPressureAxis.Layout(padding: 0);
                    XTimeAxis.Layout(padding: 0);

                }
                catch (Exception)
                {

                }

                Color backColor = Color.Transparent;
                if (pressureStabilityCalc.TrendStatus == TrendStatus.Stable) backColor = Color.Yellow;
                if (pressureStabilityCalc.Ready) backColor = Color.GreenYellow;

                lbl_cpc_read.BackColor = backColor;

                lbl_cpc_read.Text = $"{onemeasure.Value.ToWBFloatString()} {onemeasure.UOM.Name}";
            });

        }

        private void StopPressureCyclicRead()
        {
            PressureCyclicReadingCTS?.Cancel();
            PressureCyclicReadingCTS?.Dispose();
            PressureCyclicReadingCTS = null;
        }
        
        private void ReadPressureInstrumentOperationModeToRadioButtons()
        {
            var mode = pressureGeneratorSpan.PressureOperationMode;
            InvokeControlAction(() => {
                switch (mode)
                {
                    case PressureControllerOperationMode.UNKNOWN:
                        break;
                    case PressureControllerOperationMode.STANDBY:
                        rb_StandBY.Checked = true;
                        break;
                    case PressureControllerOperationMode.MEASURE:
                        rb_Measure.Checked = true;
                        break;
                    case PressureControllerOperationMode.CONTROL:
                        rb_Control.Checked = true;
                        break;
                    case PressureControllerOperationMode.VENT:
                        rb_Vent.Checked = true;
                        break;
                    default:
                        break;
                }
            });
        }

        #endregion

        private void btn_ZeroPressure_Click(object sender, EventArgs e)
        {
            PressureAutoZero();
        }
        private Task PressureAutoZero()
        {
            return Task.Run(() =>
            {
                var formCaption = this.Text;
                InvokeControlAction(() =>
                {
                    this.Text = $"{formCaption} PressureController AutoZero ...";
                    rb_Control.Enabled = false;
                    rb_Measure.Enabled = false;
                    rb_StandBY.Enabled = false;
                    rb_Vent.Enabled = false;
                    btnStartAutoCal.Enabled = false;
                    btn_ZeroPressure.Enabled = false;
                });
                pressureGeneratorSpan.Zero();
                InvokeControlAction(() =>
                {
                    this.Text = formCaption;
                    rb_Control.Enabled = true;
                    rb_Measure.Enabled = true;
                    rb_StandBY.Enabled = true;
                    rb_Vent.Enabled = true;
                    btnStartAutoCal.Enabled = true;
                    btn_ZeroPressure.Enabled = true;
                });
            });
        }

    }
}
