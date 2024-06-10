using Microsoft.Win32;
using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Renderable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using WorkBench.TestEquipment.Calys150;
using WorkBench.UOMS;
using WorkBench.UOMS.Pressure;

namespace benchGUI
{
    public partial class MainForm : Form
    {
        int TIMETOSTABLE = 15;
        int COUNTTOSTABLE = 80;

        public MainForm()
        {
            InitializeComponent();

            FormClosing += onFormClosing;

            pressureStabilityCalc = new StabilityCalculator(COUNTTOSTABLE, TimeSpan.FromSeconds(TIMETOSTABLE));
            currentStabilityCalc = new StabilityCalculator(COUNTTOSTABLE, TimeSpan.FromSeconds(TIMETOSTABLE));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitPlot();


            cb_CurrentInstrumentChannels.Enabled = false;


#if DEBUG
            tbScaleMin.Text = "0";
            tbScaleMax.Text = "40";
#endif
            FillDatagridRows();
            //fillComputedPressure();
            cbPressureScaleUOM.Items.Clear();
            cbPressureScaleUOM.Items.Add(new kPa());
            cbPressureScaleUOM.Items.Add(new mbar());
            cbPressureScaleUOM.Items.Add(new bar());
            cbPressureScaleUOM.Items.Add(new MPa());
            cbPressureScaleUOM.Items.Add(new Kgfcmsq());
            cbPressureScaleUOM.Items.Add(new Pa());
            cbPressureScaleUOM.Items.Add(new mmH2OAt4DegreesCelsius());
            cbPressureScaleUOM.Items.Add(new psi());
            cbPressureScaleUOM.Items.Add(new DegreeCelcius());
            cbPressureScaleUOM.SelectedIndex = 0;


            var random = new Random();
            var percents = new List<double>();
            foreach (DataGridViewRow item in dataGridView1.Rows)
            {
                percents.Add(double.Parse(item.Cells[nameof(percent)].Value.ToString()));
                //scatterAxisXPositions[item.Index] = item.Index + 1;
                //scatterAxisXLabels[item.Index] = $"{item.Cells["percent"].Value}%";
            }
            double scatterAxisXMinPosition = percents.Min();
            double scatterAxisXMaxPosition = percents.Max();
            plot_result.Plot.SetAxisLimitsX(scatterAxisXMinPosition, scatterAxisXMaxPosition);

        }
        private void FillDatagridRows()
        {
            dataGridView1.Rows.Clear();

            int percentPoints = (int)nUD_PercentPoints.Value;

            double percentStep = 100f / percentPoints;

            var points = new List<double>(); // { 0, 25, 50, 75, 100, 75, 50, 25, 0 };
            
            // 0, 25, 50, 75, 100
            for (int i = 0; i <= percentPoints; i++)
            {
                points.Add(i * percentStep);
            }
            // 75, 50, 25, 0
            for (int i = percentPoints - 1; i >= 0; i--)
            {
                points.Add(i * percentStep);
            }

            foreach (var item in points)
            {
                var datagridviewrow = new DataGridViewRow();
                datagridviewrow.CreateCells(dataGridView1, item);
                dataGridView1.Rows.Add(datagridviewrow);
            }
            fillComputedPressure();
        }

        private void fillComputedPressure()
        {
            if (tbScaleMin.Text.TryParseToDouble(out pressureScaleMin) 
                &&
                tbScaleMax.Text.TryParseToDouble(out pressureScaleMax)
                )
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    var perc = (double)(item.Cells[nameof(percent)].Value);
                    item.Cells[nameof(calcPressure)].Value = 
                        (
                            (pressureScaleMax - pressureScaleMin) * (perc) / 100 + pressureScaleMin
                        ).ToString("0.0000");
                }
            }
        }

        internal void onFormClosing(Object sender, FormClosingEventArgs e)
        {
            PlotRefresherCTS.Cancel();
            PlotRefresherTask.Wait();
            PlotRefresherCTS.Dispose();

            StopCurrentCyclicReading();
            if (CurrentMeasuringInstrument != null && CurrentMeasuringInstrument.IsOpen)
            {
                CurrentMeasuringInstrument.Close();
            }

            StopPressureCyclicRead();
            PressureInstrument?.Close();
            HARTCommunicator_Close();
            if (cb_HART_SerialPort.SelectedItem is string hartPort)
            {
                Properties.Settings.Default.HartPort = hartPort;
                Properties.Settings.Default.Save();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            cb_CurrentMeasuringInstruments.Items.Clear();
            //===========================ЭЛМЕТРО - КЕЛЬВИН==============================================================================
            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_CurrentMeasuringInstruments.Items.Add(Factory.GetEK_on_SerialPort_with_default_Port_Settings(item));
            }
#if DEBUG
            cb_CurrentMeasuringInstruments.Items.Add(Factory.GetFakeEK("COM222"));
#endif


            //=========================== Calys 150 ==============================================================================
            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_CurrentMeasuringInstruments.Items.Add(new Calys150(item));
            }
#if DEBUG
            cb_CurrentMeasuringInstruments.Items.Add(new Calys150(Calys150.GetSimulationCalys150SerialPortCommunicatorWithDefaultSettings("150")));

#endif

            //===========================ЭЛМЕТРО - ВОЛЬТА===============================================================================

            foreach (var item in Factory.GetSerialPortsNames())
            {
                //cb_CurrentMeasuringInstruments.Items.Add(Factory.GetEVolta_on_SerialPort_with_default_Port_Settings(item));
            }
#if DEBUG
            //cb_CurrentMeasuringInstruments.Items.Add(Factory.GetFakeEVolta("COM222"));
#endif

            if (cb_CurrentMeasuringInstruments.Items.Count > 0)
            {
                cb_CurrentMeasuringInstruments.SelectedItem = cb_CurrentMeasuringInstruments.Items[0];
            }

            //===========================CPC - 6000======================================================================================

            cb_PressureGeneratorInstrument.Items.Clear();

            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_PressureGeneratorInstrument.Items.Add(Factory.GetCPC6000_on_SerialPort_with_default_Port_Settings(item));
            }
#if DEBUG
            cb_PressureGeneratorInstrument.Items.Add(Factory.GetCPC6000_on_Fake_SerialPort());
#endif

            //===========================ЭЛМЕТРО - ПАСКАЛЬ===============================================================================

            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_PressureGeneratorInstrument.Items.Add(Factory.GetEPascal_on_SerialPort_with_default_Port_Settings(item));
            }
#if DEBUG
            cb_PressureGeneratorInstrument.Items.Add(Factory.GetFakeEPascal("COM333"));
#endif


            if (cb_PressureGeneratorInstrument.Items.Count > 0)
            {
                cb_PressureGeneratorInstrument.SelectedItem = cb_PressureGeneratorInstrument.Items[0];
            }

            string HART_COM_Port;
            HART_COM_Port = Properties.Settings.Default.HartPort;

            cb_HART_SerialPort.Items.Clear();
            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_HART_SerialPort.Items.Add(item);
                if (!string.IsNullOrEmpty(HART_COM_Port) && HART_COM_Port == item) cb_HART_SerialPort.SelectedItem = item;
            }

        }

        private void btn_copyMeansToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText($"{lbl_cpcmean.Text}\t{lbl_ekmean.Text}");
        }

        private void tbScaleMin_TextChanged(object sender, EventArgs e)
        {
            fillComputedPressure();
        }

        private void tbScaleMax_TextChanged(object sender, EventArgs e)
        {
            fillComputedPressure();
        }
        private void cbScaleUOM_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopPressureCyclicRead();
            selectedPressureUOM = (IUOM)((ComboBox)sender).SelectedItem;
            StartPressureCyclicRead();

        }
        CancellationTokenSource AutoCalCancellationTokenSource;
        Task AutoCalibrationTask;
        private void btnStartAutoCal_Click(object sender, EventArgs e)
        {
            if (
                tbScaleMin.Text.TryParseToDouble(out pressureScaleMin)
                && tbScaleMax.Text.TryParseToDouble(out pressureScaleMax)
                && startedEK
                && startedPressureInstrument
                && pressureGeneratorSpan != null
                )
            {
                btnStartAutoCal.Enabled = false;
                Task.Run( () =>
                {
                    if (OnStartDemoTask != null)
                    {
                        OnStartDemoCTS.Cancel();
                        OnStartDemoTask.Wait();
                        OnStartDemoCTS.Dispose();
                        OnStartDemoTask.Dispose();
                        OnStartDemoTask = null;
                    }
                    if (AutoCalibrationTask == null)
                    {
                        AutoCalCancellationTokenSource = new CancellationTokenSource();
                        InvokeControlAction(() => btnStartAutoCal.Text = "Отмена");
                        InvokeControlAction(() => btnStartAutoCal.Enabled = true);
                        //-----Setup Plot Chart----------------------------------------------
                        currentChartDiscrepancy = 0.1;
                        plot_result.Plot.Clear();
                        plot_result.Plot.SetAxisLimitsY(-currentChartDiscrepancy, currentChartDiscrepancy);
                        //-------------------------------------------------------------------
                        InvokeControlAction(() => nUD_PercentPoints.Enabled = false);
                        AutoCalibrationTask = AutoCalibrationSequenceTask(AutoCalCancellationTokenSource.Token);
                        AutoCalibrationTask.Wait();
                        InvokeControlAction(() => nUD_PercentPoints.Enabled = true);
                        AutoCalibrationTask = null;
                        AutoCalCancellationTokenSource.Dispose();
                        InvokeControlAction(() => btnStartAutoCal.Text = "Старт");
                        InvokeControlAction(() => btnStartAutoCal.Enabled = true);
                    }
                    else
                    {
                        InvokeControlAction(() => btnStartAutoCal.Enabled = false);
                        AutoCalCancellationTokenSource.Cancel();
                    }
                });
            }
        }
        private double currentChartDiscrepancy { get; set; }
        async Task AutoCalibrationSequenceTask(CancellationToken cancellationToken)
        {
            if (chkBx_AutoZeroAll.Checked)
            {
                var formCaption = string.Empty;
                InvokeControlAction(() => formCaption = this.Text);

                pressureGeneratorSpan.PressureOperationMode = PressureControllerOperationMode.VENT;
                pressureStabilityCalc.Reset();
                currentStabilityCalc.Reset();
                while (!pressureStabilityCalc.Ready
                        && !currentStabilityCalc.Ready
                        && !cancellationToken.IsCancellationRequested) { await Task.Delay(100); }

                var hartAutoZeroTask = hart_communicator != null ? HART_ZEROTRIM() : Task.FromResult(false);
                var pressureAutoZeroTask = PressureAutoZero();
                Task.WaitAll(hartAutoZeroTask, pressureAutoZeroTask);
                InvokeControlAction(() => this.Text = formCaption);
            }
            do
            {
                //// add new scatter to plot
                InvokeControlAction(() =>
                {
                    var randomMarkerShape = (MarkerShape)Enum.GetValues(typeof(MarkerShape)).GetValue(new Random().Next(0, Enum.GetValues(typeof(MarkerShape)).Length - 1));
                    resultScatter = plot_result.Plot.AddScatterList(markerShape: randomMarkerShape);
                });

                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    item.Cells[cpcPressure.Name].Value = "";
                    item.Cells[ekCurrent.Name].Value = "";
                    item.Cells[error.Name].Value = "";
                }

                pressureGeneratorSpan.SetPoint = new OneMeasure(0, selectedPressureUOM, DateTime.Now);

                //setTextBoxText(pressureGeneratorSpan.GetSetPoint().ToString(), tb_cpcSetPoint);
                //pressureGeneratorSpan.GetSetPoint(om => PutOneMeasureToTextBox(om, tb_PressureSetPoint));

                pressureGeneratorSpan.PressureOperationMode = WorkBench.Enums.PressureControllerOperationMode.CONTROL;

                ReadPressureInstrumentOperationModeToRadioButtons();

                while (pressureStabilityCalc.TrendStatus != TrendStatus.Stable & !cancellationToken.IsCancellationRequested) { await Task.Delay(50); }

                foreach (DataGridViewRow currentDataGridRow in dataGridView1.Rows)
                {
                    InvokeControlAction(() =>
                    {
                        var percents = new List<double>();
                        foreach (DataGridViewRow item in dataGridView1.Rows)
                        {
                            percents.Add(double.Parse(item.Cells[nameof(percent)].Value.ToString()));
                        }
                        double scatterAxisXMinPosition = percents.Min();
                        double scatterAxisXMaxPosition = percents.Max();
                        plot_result.Plot.SetAxisLimitsX(scatterAxisXMinPosition, scatterAxisXMaxPosition);
                        plot_result.Plot.XAxis.TickLabelFormat((d) => $"{d}%");

                        //plot_result.Refresh(skipIfCurrentlyRendering: true);
                    });
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        currentDataGridRow.Selected = true;
                        double setpoint = double.Parse(currentDataGridRow.Cells[calcPressure.Name].Value.ToString());

                        pressureGeneratorSpan.SetPoint = new OneMeasure(setpoint, selectedPressureUOM, DateTime.Now);
                        InvokeControlAction(() => tb_PressureSetPoint.Text = $"{pressureGeneratorSpan.SetPoint.Value.ToWBFloatString()}");

                        currentStabilityCalc.Reset();
                        pressureStabilityCalc.Reset();

                        while (!(currentStabilityCalc.Ready & pressureStabilityCalc.Ready) & !cancellationToken.IsCancellationRequested) { Thread.Sleep(100); } //{ Application.DoEvents(); }
                        if (!cancellationToken.IsCancellationRequested)
                        {

                            currentDataGridRow.Cells[cpcPressure.Name].Value = pressureStabilityCalc.StableMeanValue.ToWBFloatString();
                            currentDataGridRow.Cells[ekCurrent.Name].Value = currentStabilityCalc.StableMeanValue.ToWBFloatString();
                            var discrepancy = (((currentStabilityCalc.StableMeanValue - 4) / 16 * (pressureScaleMax - pressureScaleMin) + pressureScaleMin - pressureStabilityCalc.StableMeanValue) / (pressureScaleMax - pressureScaleMin) * 100);
                            currentDataGridRow.Cells[error.Name].Value = discrepancy.ToWBFloatString();
                            InvokeControlAction(() =>
                            {
                                //chart_result_Xs[item.Index] = item.Index + 1;
                                //chart_result_Ys[item.Index] = discrepancy;
                                resultScatter.Add(double.Parse(currentDataGridRow.Cells[nameof(percent)].Value.ToString()), discrepancy);
                                var absDiscrepancy = Math.Round(Math.Abs(discrepancy), 2);
                                if (absDiscrepancy > Math.Abs(currentChartDiscrepancy))
                                {
                                    currentChartDiscrepancy = absDiscrepancy * 1.1;
                                    plot_result.Plot.SetAxisLimitsY(-currentChartDiscrepancy, currentChartDiscrepancy);
                                }
                                //plot_result.Refresh(skipIfCurrentlyRendering: true);
                            });
                        }
                        currentDataGridRow.Selected = false;
                    }
                }
                if (nUD_CalibrationCyclesCount.Value > nUD_CalibrationCyclesCount.Minimum)
                {
                    InvokeControlAction(() => nUD_CalibrationCyclesCount.Value--);

                }
            } while (!cancellationToken.IsCancellationRequested && nUD_CalibrationCyclesCount.Value - 1 >= nUD_CalibrationCyclesCount.Minimum);

            if (pressureGeneratorSpan != null)
            {
                pressureGeneratorSpan.SetPoint = new OneMeasure(0, selectedPressureUOM, DateTime.Now);

                while (pressureStabilityCalc.TrendStatus != TrendStatus.Stable & !cancellationToken.IsCancellationRequested) { Thread.Sleep(100); }

                pressureGeneratorSpan.PressureOperationMode = WorkBench.Enums.PressureControllerOperationMode.VENT;

                ReadPressureInstrumentOperationModeToRadioButtons();
            }
        }

        private delegate void SafeCallInvokeControlActionDelegate(Action action);
        private void InvokeControlAction(Action action)
        {
            if (this.InvokeRequired)// control.InvokeRequired)
            {
                var SafecallDelegate = new SafeCallInvokeControlActionDelegate(InvokeControlAction);
                //control.BeginInvoke(SafecallDelegate, new object[] { control, action});
                this.BeginInvoke(SafecallDelegate, new object[] { action });
            }
            else
            {
                action();
            }
        }

        private void tb_PressureSetPoint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                btn_StepUp_Click(this, new EventArgs());
            }
            if (e.KeyCode == Keys.Down)
            {
                btn_StepDown_Click(this, new EventArgs());
            }
            if (e.KeyCode == Keys.Left)
            {
                btn_pressureMicroStepDown_Click(this, new EventArgs());
            }
            if (e.KeyCode == Keys.Right)
            {
                btn_pressureMicrostepUP_Click(this, new EventArgs());
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (pressureGeneratorSpan == null) return;

                var mode = pressureGeneratorSpan.PressureOperationMode;

                if (mode == WorkBench.Enums.PressureControllerOperationMode.CONTROL)
                {
                    setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.VENT);
                    ReadPressureInstrumentOperationModeToRadioButtons();
                }
                if (mode == WorkBench.Enums.PressureControllerOperationMode.VENT || mode == WorkBench.Enums.PressureControllerOperationMode.UNKNOWN)
                {
                    setPressureGeneratorOperationMode(WorkBench.Enums.PressureControllerOperationMode.CONTROL);
                    ReadPressureInstrumentOperationModeToRadioButtons();
                }
            }
        }

        private void nUD_PercentPoints_ValueChanged(object sender, EventArgs e)
        {
            if (AutoCalibrationTask == null && sender is NumericUpDown ud)
            {
                FillDatagridRows();
            }
        }
    }
}
