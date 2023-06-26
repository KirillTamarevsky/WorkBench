using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Data;
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
            cb_CurrentInstrumentChannels.Enabled = false;

            dataGridView1.Rows.Clear();

            var points = new List<double>() { 0, 25, 50, 75, 100, 75, 50, 25, 0 };

            foreach (var item in points)
            {
                var datagridviewrow = new DataGridViewRow();
                datagridviewrow.CreateCells(dataGridView1);
                datagridviewrow.Cells[0].Value = (double)item;
                dataGridView1.Rows.Add(datagridviewrow);
            }

#if DEBUG
            tbScaleMin.Text = "0";
            tbScaleMax.Text = "40";
            fillComputedPressure();
#endif
            cbPressureScaleUOM.Items.Clear();
            cbPressureScaleUOM.Items.Add(new kPa());
            cbPressureScaleUOM.Items.Add(new mbar());
            cbPressureScaleUOM.Items.Add(new bar());
            cbPressureScaleUOM.Items.Add(new MPa());
            cbPressureScaleUOM.Items.Add(new Kgfcmsq());
            cbPressureScaleUOM.Items.Add(new Pa());
            cbPressureScaleUOM.Items.Add(new mmH2OAt4DegreesCelsius());
            cbPressureScaleUOM.Items.Add(new DegreeCelcius());
            cbPressureScaleUOM.SelectedIndex = 0;

            // init Measurung Instrument Error Deviation Plot
            plot_result.Plot.Clear();

            var random = new Random();
            Random rand = new Random(1234);
            for (int i = 0; i < 20; i++)
            {
                double[] xs = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                double[] ys = DataGen.Random(rand, 10);
                plot_result.Plot.AddScatter(xs, ys);
            }
            plot_result.Refresh();

            // inti Standard Measuring Instruments Readings Plot
            currentMeasuresScatterPlot = plot_measures.Plot.AddScatter(new double[] { 0 }, new double[] { 0 });
            pressureMeasuresScatterPlot = plot_measures.Plot.AddScatter(new double[] { 0 }, new double[] { 0 });
            plot_measures.Refresh();
        }
        ScatterPlot currentMeasuresScatterPlot;
        ScatterPlot pressureMeasuresScatterPlot;
        private void fillComputedPressure()
        {
            if (double.TryParse(tbScaleMin.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out pressureScaleMin) &
                double.TryParse(tbScaleMax.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out pressureScaleMax)
                    )
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    item.Cells["calcPressure"].Value = ((pressureScaleMax - pressureScaleMin) * ((double)(item.Cells["percent"].Value)) / (double)100 + pressureScaleMin).ToString("N4");
                }


            }
        }

        internal void onFormClosing(Object sender, FormClosingEventArgs e)
        {
            StopCurrentCyclicReading();
            if (CurrentMeasuringInstrument != null && CurrentMeasuringInstrument.IsOpen)
            {
                CurrentMeasuringInstrument.Close();
            }

            StopPressureCyclicRead();
            PressureInstrument?.Close();
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

            cb_HART_SerialPort.Items.Clear();
            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_HART_SerialPort.Items.Add(item);
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
        private async void btnStartAutoCal_Click(object sender, EventArgs e)
        {
            if (AutoCalibrationTask == null)
            {
                AutoCalCancellationTokenSource = new CancellationTokenSource();
                btnStartAutoCal.Text = "Отмена";
                StartAutoCalibrationSequenceTask(AutoCalCancellationTokenSource.Token);
                await AutoCalibrationTask;
                AutoCalibrationTask = null;
                AutoCalCancellationTokenSource.Dispose();
            }
            else
            {
                InvokeControlAction(btnStartAutoCal, () => btnStartAutoCal.Enabled = false);
                AutoCalCancellationTokenSource.Cancel();
            }
        }
        private double[] chart_result_Xs { get; set; }
        private double[] chart_result_Ys { get; set; }
        private double currentChartDiscrepancy { get; set; }
        private ScatterPlot resultScatter { get; set; }
        void StartAutoCalibrationSequenceTask(CancellationToken cancellationToken)
        {
            AutoCalibrationTask = Task.Run(() =>
            {
                if (
                ((double.TryParse(tbScaleMin.Text.Replace(',', '.'),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out pressureScaleMin)
                    &
                double.TryParse(tbScaleMax.Text.Replace(',', '.'),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out pressureScaleMax)))
                && startedEK && startedCPC
                )
                {
                    InvokeControlAction(plot_result, () =>
                                                                {
                                                                    currentChartDiscrepancy = 0.1;
                                                                    plot_result.Plot.Clear();
                                                                    plot_result.Plot.SetAxisLimitsY(-currentChartDiscrepancy, currentChartDiscrepancy);
                                                                }
                    );

                    do
                    {
                        // add new scatter to plot
                        InvokeControlAction(this, () =>
                        {
                            chart_result_Xs = new double[dataGridView1.Rows.Count];
                            for (int i = 1; i < dataGridView1.Rows.Count + 1; i++)
                            {
                                chart_result_Xs[i - 1] = i;
                            }
                            chart_result_Ys = new double[dataGridView1.Rows.Count];
                            for (int i = 1; i < dataGridView1.Rows.Count + 1; i++)
                            {
                                chart_result_Ys[i - 1] = 0;
                            }
                            resultScatter = plot_result.Plot.AddScatter(chart_result_Xs, chart_result_Ys);
                            plot_result.Plot.AxisAutoX();
                            plot_result.Refresh();
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

                        while (pressureStabilityCalc.TrendStatus != TrendStatus.Stable & !cancellationToken.IsCancellationRequested) { } // { Application.DoEvents(); }

                        foreach (DataGridViewRow item in dataGridView1.Rows)
                        {

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                double setpoint = double.Parse(item.Cells[calcPressure.Name].Value.ToString());

                                pressureGeneratorSpan.SetPoint = new OneMeasure(setpoint, selectedPressureUOM, DateTime.Now);

                                //setTextBoxText(pressureGeneratorSpan.GetSetPoint().ToString(), tb_cpcSetPoint);
                                //pressureGeneratorSpan.GetSetPoint(om => PutOneMeasureToTextBox(om, tb_PressureSetPoint));


                                currentStabilityCalc.Reset();
                                pressureStabilityCalc.Reset();

                                while (!(currentStabilityCalc.Ready & pressureStabilityCalc.Ready) & !cancellationToken.IsCancellationRequested) { } //{ Application.DoEvents(); }
                                if (!cancellationToken.IsCancellationRequested)
                                {

                                    item.Cells[cpcPressure.Name].Value = pressureStabilityCalc.StableMeanValue.ToString("N4");
                                    item.Cells[ekCurrent.Name].Value = currentStabilityCalc.StableMeanValue.ToString("N4");
                                    var discrepancy = (((currentStabilityCalc.StableMeanValue - 4) / 16 * (pressureScaleMax - pressureScaleMin) + pressureScaleMin - pressureStabilityCalc.StableMeanValue) / (pressureScaleMax - pressureScaleMin) * 100);
                                    item.Cells[error.Name].Value = discrepancy.ToString("N4");
                                    InvokeControlAction(plot_result, () =>
                                    {
                                        chart_result_Xs[item.Index] = item.Index + 1;
                                        chart_result_Ys[item.Index] = discrepancy;
                                        var absDiscrepancy = Math.Round(Math.Abs(discrepancy));
                                        if (absDiscrepancy > Math.Abs(currentChartDiscrepancy))
                                        {
                                            currentChartDiscrepancy = absDiscrepancy * 1.1;
                                            plot_result.Plot.SetAxisLimitsY(-currentChartDiscrepancy, currentChartDiscrepancy);
                                        }
                                        plot_result.Refresh();
                                    });
                                }
                            }
                        }
                        if (nUD_CalibrationCyclesCount.Value > nUD_CalibrationCyclesCount.Minimum)
                        {
                            InvokeControlAction(nUD_CalibrationCyclesCount, () => nUD_CalibrationCyclesCount.Value--);

                        }
                    } while (nUD_CalibrationCyclesCount.Value - 1 >= nUD_CalibrationCyclesCount.Minimum);

                    pressureGeneratorSpan.SetPoint = new OneMeasure(0, selectedPressureUOM, DateTime.Now);
                    //pressureGeneratorSpan.GetSetPoint(om => PutOneMeasureToTextBox(om, tb_PressureSetPoint));

                    while (pressureStabilityCalc.TrendStatus != TrendStatus.Stable & !cancellationToken.IsCancellationRequested) { } // { Application.DoEvents(); }

                    pressureGeneratorSpan.PressureOperationMode = WorkBench.Enums.PressureControllerOperationMode.VENT;

                    ReadPressureInstrumentOperationModeToRadioButtons();

                }
                InvokeControlAction(btnStartAutoCal, () => btnStartAutoCal.Text = "Старт");
                InvokeControlAction(btnStartAutoCal, () => btnStartAutoCal.Enabled = true);


            });

        }

        void fillMeasuresChart()
        {

            InvokeControlAction(this, () =>
            {
                try
                {
                    double xAxisMinLimit = DateTime.Now.AddSeconds(-TIMETOSTABLE).ToOADate();
                    double xAxisMaxLimit = DateTime.Now.ToOADate();
                    plot_measures.Plot.SetAxisLimitsX(xAxisMinLimit, xAxisMaxLimit);

                    //chart_measures.Series.Clear();
                    if (currentStabilityCalc != null && currentStabilityCalc.MeasuresCount > 0)
                    {
                        plot_measures.Plot.XAxis.DateTimeFormat(true);
                        var currentMeasurePointsToPlot = currentStabilityCalc.Measures;
                        double[] xs = currentMeasurePointsToPlot.Select(m => m.TimeStamp.ToOADate()).ToArray();
                        double[] ys = currentMeasurePointsToPlot.Select(m => m.Value).ToArray();
                        currentMeasuresScatterPlot.Update(xs, ys);
                        currentMeasuresScatterPlot.MarkerShape = MarkerShape.none;
                        plot_measures.Plot.SetAxisLimitsY(0, 24);
                        currentMeasuresScatterPlot.YAxisIndex = 0;
                    }

                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    if (pressureStabilityCalc != null && pressureStabilityCalc.MeasuresCount > 0)
                    {
                        plot_measures.Plot.XAxis.DateTimeFormat(true);
                        var pressureMeasurePointsToPlot = pressureStabilityCalc.Measures;
                        double[] xs = pressureMeasurePointsToPlot.Select(m => m.TimeStamp.ToOADate()).ToArray();
                        double[] ys = pressureMeasurePointsToPlot.Select(m => m.Value).ToArray();
                        pressureMeasuresScatterPlot.Update(xs, ys);
                        pressureMeasuresScatterPlot.MarkerShape = MarkerShape.none;
                        pressureMeasuresScatterPlot.YAxisIndex = 1;

                        if (
                            ((double.TryParse(tbScaleMin.Text.Replace(',', '.'),
                            NumberStyles.Float,
                            CultureInfo.InvariantCulture,
                            out pressureScaleMin)
                            &
                            double.TryParse(tbScaleMax.Text.Replace(',', '.'),
                            NumberStyles.Float,
                            CultureInfo.InvariantCulture,
                            out pressureScaleMax))))
                        {
                            var fullscale = pressureScaleMax - pressureScaleMin;
                            plot_measures.Plot.SetAxisLimits(yMin: pressureScaleMin - fullscale * 0.05, yMax: pressureScaleMax + fullscale * 0.05, yAxisIndex: 1);
                            plot_measures.Plot.RightAxis.Hide(false);
                        }
                    }
                    plot_measures.Plot.XAxis.Layout(padding: 0);
                    plot_measures.Plot.XAxis2.Layout(padding: 0);
                    plot_measures.Plot.YAxis.Layout(padding: 0);
                    plot_measures.Plot.YAxis2.Layout(padding: 0);
                    plot_measures.Refresh();
                }
                catch (Exception)
                {

                }
            });


        }
        private void setLabelText(string txt, Label label) => InvokeControlAction(label, () => label.Text = txt);
        private void setTextBoxText(string txt, TextBox textbox) => InvokeControlAction(textbox, () => textbox.Text = txt);
        private void setComboboxSelectedItemIndex(ComboBox cb, int index) => InvokeControlAction(cb, () => cb.SelectedIndex = index);
        private void setComboboxSelectedItem(ComboBox cb, object item) => InvokeControlAction(cb, () => cb.SelectedItem = item);
        private void setRadioButtonChecked(RadioButton rb, bool _checked) => InvokeControlAction(rb, () => rb.Checked = _checked);

        private delegate void SafeCallInvokeControlActionDelegate(Control control, Action action);
        private void InvokeControlAction(Control control, Action action)
        {
            if (this.InvokeRequired)// control.InvokeRequired)
            {
                var SafecallDelegate = new SafeCallInvokeControlActionDelegate(InvokeControlAction);
                //control.BeginInvoke(SafecallDelegate, new object[] { control, action});
                this.BeginInvoke(SafecallDelegate, new object[] { control, action });
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


    }
}
