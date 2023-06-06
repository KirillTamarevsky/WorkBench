using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WorkBench;
using WorkBench.Enums;
using WorkBench.Interfaces;
using WorkBench.Interfaces.InstrumentChannel;
using WorkBench.TestEquipment.EK;
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

            chart_result.Series.Clear();

            var random = new Random();
            for (int i = 0; i < 20; i++)
            {

                var chartSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                chartSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                chartSeries.XValueType = ChartValueType.Double;
                chart_result.Series.Add(chartSeries);
                for (int x = 0; x < 110; x = x + 10)
                {
                    chartSeries.Points.AddXY(x, random.NextDouble());
                }
            }
        }

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
            CurrentMeasuringInstrument?.Close();

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
                    InvokeControlAction(chart_result, () =>
                                                                {
                                                                    chart_result.Series.Clear();
                                                                    chart_result.ChartAreas[0].AxisX.Minimum = 1;
                                                                    chart_result.ChartAreas[0].AxisX.Maximum = dataGridView1.Rows.Count;
                                                                    chart_result.ChartAreas[0].AxisY.Minimum = -0.1;
                                                                    chart_result.ChartAreas[0].AxisY.Maximum = 0.1;
                                                                }
                    );

                    do
                    {
                        foreach (DataGridViewRow item in dataGridView1.Rows)
                        {
                            item.Cells[cpcPressure.Name].Value = "";
                            item.Cells[ekCurrent.Name].Value = "";
                            item.Cells[error.Name].Value = "";
                        }

                        var chartSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                        chartSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                        chartSeries.XValueType = ChartValueType.Int32;
                        chartSeries.YAxisType = AxisType.Primary;


                        InvokeControlAction(chart_result, () => chart_result.Series.Add(chartSeries));

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
                                    InvokeControlAction(chart_result, () =>
                                    {
                                        chartSeries.Points.AddXY(item.Index + 1, discrepancy);
                                        var absDiscrepancy = Math.Round(Math.Abs(discrepancy));
                                        if (absDiscrepancy > Math.Abs(chart_result.ChartAreas[0].AxisY.Minimum))
                                        {
                                            chart_result.ChartAreas[0].AxisY.Minimum = absDiscrepancy * -1.1;
                                            chart_result.ChartAreas[0].AxisY.Maximum = absDiscrepancy * 1.1;
                                        }
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

            InvokeControlAction(chart_measures, () =>
            {
                try
                {

                    chart_measures.Series.Clear();
                    if (currentStabilityCalc != null && currentStabilityCalc.MeasuresCount > 0)
                    {

                        var chartSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                        chartSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                        chartSeries.MarkerSize = 2;
                        chartSeries.YAxisType = AxisType.Primary;
                        chartSeries.YValueType = ChartValueType.Double;
                        chartSeries.XValueType = ChartValueType.Time;
                        DateTime minDate = currentStabilityCalc.Measures.OrderBy(m => m.TimeStamp).Select(m => m.TimeStamp).First();
                        DateTime maxDate = currentStabilityCalc.Measures.OrderByDescending(m => m.TimeStamp).Select(m => m.TimeStamp).First();
                        chart_measures.ChartAreas[0].AxisX.Minimum = maxDate.AddSeconds(-TIMETOSTABLE).ToOADate(); // minDate.ToOADate());
                        chart_measures.ChartAreas[0].AxisX.Maximum = maxDate.ToOADate();
                        chart_measures.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                        chart_measures.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
                        chart_measures.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                        chart_measures.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
                        chart_measures.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
                        chart_measures.ChartAreas[0].AxisY2.MinorGrid.Enabled = false;
                        chart_measures.ChartAreas[0].AxisY.Minimum = 4 - 16 * 0.05;
                        chart_measures.ChartAreas[0].AxisY.Maximum = 20 + 16 * 0.05;


                        InvokeControlAction(chart_measures, () => chart_measures.Series.Add(chartSeries));

                        foreach (var item in currentStabilityCalc.Measures)
                        {
                            chartSeries.Points.AddXY(item.TimeStamp.ToOADate(), item.Value);
                        }

                    }

                    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    if (pressureStabilityCalc != null && pressureStabilityCalc.MeasuresCount > 0)
                    {

                        var chartSeries = new System.Windows.Forms.DataVisualization.Charting.Series();
                        chartSeries.YAxisType = AxisType.Secondary;

                        chartSeries.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                        chartSeries.MarkerSize = 2;


                        chartSeries.XValueType = ChartValueType.Time;

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
                            chart_measures.ChartAreas[0].AxisY2.Minimum = pressureScaleMin - fullscale * 0.05;
                            chart_measures.ChartAreas[0].AxisY2.Maximum = pressureScaleMax + fullscale * 0.05;
                        }

                        chart_measures.Series.Add(chartSeries);

                        foreach (var item in pressureStabilityCalc.Measures)
                        {
                            chartSeries.Points.AddXY(item.TimeStamp.ToOADate(), item.Value);
                        }
                    }
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
