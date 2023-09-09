using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
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

        private bool startedCPC => PressureInstrument != null && PressureInstrument.IsOpen;

        private IInstrumentChannelSpanReader pressureReaderSpan { get; set; }

        private IUOM selectedPressureUOM { get; set; }

        private IInstrumentChannelSpanPressureGenerator pressureGeneratorSpan { get; set; }

        private double pressureScaleMin;

        private double pressureScaleMax;

        #region PressureInstrument

        private void btn_openPressureMeasureInstrument_Click(object sender, EventArgs e)
        {
            btn_openPressureInstrument.Enabled = false;
            switch (startedCPC)
            {
                case false:
                    PressureInstrument = (IInstrument)cb_PressureGeneratorInstrument.SelectedItem;

                    lbl_cpcStatus.Text = "поиск...";
                    Task.Factory.StartNew(() =>
                    { 
                        if ( PressureInstrument.Open())
                        {
                            InvokeControlAction( () =>
                            {
                                PressureInstrument_start();
                            });
                        }
                        else
                        {
                            InvokeControlAction( () =>
                            {
                                lbl_cpcStatus.Text = "нет связи";
                            });
                        }
                        InvokeControlAction( () => btn_openPressureInstrument.Enabled = true);
                    });

                    break;
                case true:
                    log4net.LogManager.GetLogger("").Debug("cpc_stop();");
                    PressureInstrument_stop();

                    log4net.LogManager.GetLogger("").Debug("PressureInstrument.Close();");
                    PressureInstrument.Close();

                    log4net.LogManager.GetLogger("").Debug("pressureStabilityCalc.Reset();");
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

            cb_PressureReaderGeneratorSpan.Items.Clear();

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
        private void PutOneMeasureToTextBox(OneMeasure oneMeasure, TextBox textBox)
        {
            setTextBoxText(oneMeasure.Value.ToWBFloatString(), textBox);
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

        private void btn_setSetPoint_Click(object sender, EventArgs e)
        {
            setSetPoint(newSetPoint);
        }

        double newSetPoint { get; set; }

        private void setSetPoint(double newSetPoint)
        {
            if (pressureGeneratorSpan == null ) return;

            var maxPressureSetValue = pressureGeneratorSpan.Scale.Max * (pressureGeneratorSpan.Scale.UOM.Factor / selectedPressureUOM.Factor );

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

            pressureGeneratorSpan.SetPoint = new OneMeasure(newSetPoint, selectedPressureUOM, DateTime.Now);

            PutOneMeasureToTextBox(pressureGeneratorSpan.SetPoint, tb_PressureSetPoint);

        }

        private void setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode operationMode)
        {
            if (pressureGeneratorSpan == null) return;
            Task.Factory.StartNew(() => pressureGeneratorSpan.PressureOperationMode = operationMode);

        }

        #region CPC MODE Radio Buttons

        private void rb_Control_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.CONTROL);
            }
        }

        private void rb_Vent_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.VENT);
            }
        }

        private void rb_Measure_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.MEASURE);
            }
        }

        private void rb_StandBY_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.STANDBY);
            }
        }
        #endregion

        private void cb_cpcChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopPressureCyclicRead();

            cb_PressureReaderGeneratorSpan.Items.Clear();

            foreach (var span in (((IInstrumentChannel)((ComboBox)sender).SelectedItem)).AvailableSpans)
            {
                cb_PressureReaderGeneratorSpan.Items.Add(span);
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

        }
        private void cb_PressureReaderGeneratorSpan_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopPressureCyclicRead();
            pressureStabilityCalc.Reset();

            pressureReaderSpan = (IInstrumentChannelSpanReader)((ComboBox)sender).SelectedItem;

            pressureGeneratorSpan = (IInstrumentChannelSpanPressureGenerator)pressureReaderSpan;

            tb_PressureSetPoint.Text = pressureGeneratorSpan.SetPoint.Value.ToWBFloatString();

            ReadPressureInstrumentOperationModeToRadioButtons();
            
            StartPressureCyclicRead();
        }

        CancellationTokenSource PressureCyclicReadingCTS { get; set; }
        private void StartPressureCyclicRead()
        {
            if (pressureReaderSpan!=null && selectedPressureUOM.UOMType == UOMType.Pressure)
            {
                PressureCyclicReadingCTS = new CancellationTokenSource();
                var token = PressureCyclicReadingCTS.Token;
                Task.Run(
                    () =>
                    {
                        while (!token.IsCancellationRequested)
                        {
                            var PressureOneMeasure = pressureReaderSpan.Read(selectedPressureUOM);
                            if (PressureOneMeasure != null && !token.IsCancellationRequested)
                                OnOnePressureMeasureReaded(null, PressureOneMeasure);
                        }
                    });
            }
        }

        private void StopPressureCyclicRead()
        {
            PressureCyclicReadingCTS?.Cancel();
            PressureCyclicReadingCTS?.Dispose();
            PressureCyclicReadingCTS = null;
        }
        
        private void OnOnePressureMeasureReaded(object sender, OneMeasure onemeasure)
        {
            pressureStabilityCalc.AddMeasure(onemeasure);
            
            Color backColor = Color.Transparent;

            if (pressureStabilityCalc.TrendStatus == TrendStatus.Unknown)
            {
                setLabelText($"{pressureStabilityCalc.MeasuresCount}/{pressureStabilityCalc.MeasuringTimeSpan.TotalSeconds:N0}s", lbl_CPCstability);

                setLabelText("----", lbl_cpcmean);
                setLabelText("----", lbl_CPCstdev);
                setLabelText("----", lbl_CPCLRSlope);
            }
            else
            {
                setLabelText(pressureStabilityCalc.MeanValue.ToWBFloatString(), lbl_cpcmean);
                setLabelText(pressureStabilityCalc.StdDeviation.ToWBFloatString(), lbl_CPCstdev);
                setLabelText(pressureStabilityCalc.LRSlope.ToWBFloatString(), lbl_CPCLRSlope);

                var trendStatusText = pressureStabilityCalc.GetStatusTextRu();
                setLabelText(trendStatusText, lbl_CPCstability);

                backColor = pressureStabilityCalc.TrendStatus switch
                {
                    TrendStatus.Stable => Color.Yellow,
                    _ => Color.Transparent
                };
            }
            if (pressureStabilityCalc.Ready)
            {
                backColor = Color.GreenYellow;
            }

            lbl_cpc_read.BackColor = backColor;

            setLabelText($"{onemeasure.Value.ToWBFloatString()} {onemeasure.UOM.Name}", lbl_cpc_read);

            fillMeasuresChart();

        }
        private void ReadPressureInstrumentOperationModeToRadioButtons()
        {
            var mode = pressureGeneratorSpan.PressureOperationMode;
            FillPressureGenerationModeRadioButtons(mode);
        }

        private void FillPressureGenerationModeRadioButtons(PressureControllerOperationMode  pressureControllerOperationMode)
        {
            switch (pressureControllerOperationMode)
            {
                case PressureControllerOperationMode.UNKNOWN:
                    break;
                case PressureControllerOperationMode.STANDBY:
                    setRadioButtonChecked(rb_StandBY, true);
                    break;
                case PressureControllerOperationMode.MEASURE:
                    setRadioButtonChecked(rb_Measure, true);
                    break;
                case PressureControllerOperationMode.CONTROL:
                    setRadioButtonChecked(rb_Control, true); 
                    break;
                case PressureControllerOperationMode.VENT:
                    setRadioButtonChecked(rb_Vent, true); 
                    break;
                default:
                    break;
            }
        }


        #endregion

        private void tb_newSetPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && btn_setSetPoint.Enabled == true)
            {
                 setSetPoint(newSetPoint);
            }
        }

        private void btn_ZeroPressure_Click(object sender, EventArgs e)
        {
            Task.Run(() => pressureGeneratorSpan.Zero());
        }


    }
}
