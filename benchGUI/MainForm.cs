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
using WorkBench;
using WorkBench.Interfaces;
using WorkBench.TestEquipment.EK;
using WorkBench.UOM;

namespace benchGUI
{
    public partial class MainForm : Form
    {

        List<WorkBench.Interfaces.OneMeasureResult> ekmeasures = new List<OneMeasureResult>();

        List<WorkBench.Interfaces.OneMeasureResult> cpcmeasures = new List<OneMeasureResult>();

        double ekmean;
        double cpcmean;


        IInstrument instrumentEK;

        IInstrument instrumentCPC6000;

        private bool startedEK;

        private bool startedCPC;

        private IReader readerEK;

        private IReader readerCPC;

        private IPressureGenerator generatorCPC;

        private TrendStatus ekTrendStatus;

        private TrendStatus cpcTrendStatus;

        private double scaleMin;

        private double scaleMax;

        public MainForm()
        {
            InitializeComponent();

            FormClosing += onFormClosing;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            startedEK = false;

            startedCPC = false;

            cb_chanNUM.Enabled = false;

            dataGridView1.Rows.Clear();

            var datagridviewrow = new DataGridViewRow();
            datagridviewrow.CreateCells(dataGridView1);
            datagridviewrow.Cells[0].Value = (double)0;
            dataGridView1.Rows.Add(datagridviewrow);

            datagridviewrow = new DataGridViewRow();
            datagridviewrow.CreateCells(dataGridView1);
            datagridviewrow.Cells[0].Value = (double)25;
            dataGridView1.Rows.Add(datagridviewrow);

            datagridviewrow = new DataGridViewRow();
            datagridviewrow.CreateCells(dataGridView1);
            datagridviewrow.Cells[0].Value = (double)50;
            dataGridView1.Rows.Add(datagridviewrow);

            datagridviewrow = new DataGridViewRow();
            datagridviewrow.CreateCells(dataGridView1);
            datagridviewrow.Cells[0].Value = (double)75;
            dataGridView1.Rows.Add(datagridviewrow);

            datagridviewrow = new DataGridViewRow();
            datagridviewrow.CreateCells(dataGridView1);
            datagridviewrow.Cells[0].Value = (double)100;
            dataGridView1.Rows.Add(datagridviewrow);

            cbScaleUOM.Items.Clear();
            cbScaleUOM.Items.Add(new bar());
            cbScaleUOM.Items.Add(new kPa());
            cbScaleUOM.Items.Add(new MPa());
            cbScaleUOM.Items.Add(new mbar());
            cbScaleUOM.Items.Add(new Pa());
            cbScaleUOM.SelectedIndex = 1;
        }

        private void fillComputedPressure()
        {


            if (double.TryParse(tbScaleMin.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures),
                    out scaleMin) &
                double.TryParse(tbScaleMax.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures),
                    out scaleMax)
                    )
            {
                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    item.Cells["calcPressure"].Value = (scaleMax - scaleMin) * ((double)(item.Cells["percent"].Value)) / (double)100 + scaleMin;
                }


            }
        }

        internal void onFormClosing(Object sender, FormClosingEventArgs e)
        {

            if (instrumentEK != null)
            {
                if (readerEK != null)
                {
                    readerEK.CyclicRead = false;

                    readerEK.NewValueReaded -= OnEKNewValueReaded;
                }

                instrumentEK.Close();

            }
            if (instrumentCPC6000 != null)
            {
                if (readerCPC != null)
                {

                    readerCPC.CyclicRead = false;

                    readerCPC.NewValueReaded -= OnCPCNewValueReaded;
                }

                instrumentCPC6000.Close();
            }

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            cb_SP.Items.Clear();

            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_SP.Items.Add(Factory.GetEK_on_SerialPort_with_default_Port_Settings(item));
                cb_SP.Items.Add(Factory.GetFakeEK(item));
            }

            if (cb_SP.Items.Count > 0)
            {
                cb_SP.SelectedItem = cb_SP.Items[0];
            }


            cb_cpc.Items.Clear();

            foreach (var item in Factory.GetSerialPortsNames())
            {
                cb_cpc.Items.Add(Factory.GetCPC6000_on_SerialPort_with_default_Port_Settings(item));
                cb_cpc.Items.Add(Factory.GetCPC6000_on_Fake_SerialPort());
            }

            if (cb_cpc.Items.Count > 0)
            {
                cb_cpc.SelectedItem = cb_cpc.Items[0];
            }

        }

        #region EK

        private void btn_openEK_Click(object sender, EventArgs e)
        {
            switch (startedEK)
            {
                case false:
                    instrumentEK = (IInstrument)cb_SP.SelectedItem;

                    if (instrumentEK.Open())
                    {
                        cb_chanNUM.Items.Clear();

                        foreach (var item in instrumentEK.Channels)
                        {
                            cb_chanNUM.Items.Add(item);
                        }

                        if (cb_chanNUM.Items.Count > 0)
                        {
                            cb_chanNUM.Enabled = true;

                            cb_chanNUM.SelectedIndex = 0;
                        }

                        startedEK = true;

                        btn_openEK.Text = "Разорвать связь";
                    }

                    break;
                case true:
                    readerEK.CyclicRead = false;
                
                    readerEK.NewValueReaded -= OnEKNewValueReaded;

                    lbl_cnahValue.Text = "---.----";

                    btn_openEK.Text = "Установить связь";

                    startedEK = !instrumentEK.Close();

                    cb_chanNUM.Items.Clear();

                    cb_chanNUM.Enabled = false;

                    readerEK = null;
                    
                    break;
                default:
                    break;
            }

        }

        private void OnEKNewValueReaded(IReader reader)
        {
            ekmeasures.Add(reader.lastValue);

            if (ekmeasures.Count > 20)
            {
                while (ekmeasures.Count > 20)
                {
                    ekmeasures.RemoveAt(0);
                }

                ekmean = ekmeasures.Average((om) => om.Value);

                double stdev = 0;

                foreach (var item in ekmeasures)
                {
                    stdev += Math.Pow(item.Value - ekmean, 2);
                }

                stdev = Math.Sqrt(stdev);

                setLabelText(ekmean.ToString("N4"), lbl_ekmean);

                setLabelText(stdev.ToString("N4"), lbl_EKstdev);
                double ekLRSlope = LRSlope(ekmeasures);
                setLabelText(ekLRSlope.ToString("N4"), lbl_EKLRSlope);
                if (Math.Round(ekLRSlope, 3) == 0)
                {
                    setLabelText("стабильно", lbl_EKstability);

                    ekTrendStatus = TrendStatus.Stable;
                }
                else
                {
                    if (ekLRSlope < 0)
                    {
                        setLabelText("уменьшается", lbl_EKstability);

                        ekTrendStatus = TrendStatus.GrowDown;
                    }
                    else
                    {
                        setLabelText("увеличивается", lbl_EKstability);

                        ekTrendStatus = TrendStatus.GrowUP;
                    }
                }

            }

            setLabelText
                (
                string.Format(
                    "{0} {1}",
                    reader.lastValue.Value.ToString("N4"),
                    reader.lastValue.UOM.Name
                    ),
                lbl_cnahValue
                );
        }

        private void cb_chanNUM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (readerEK != null)
            {
                readerEK.CyclicRead = false;

                readerEK.NewValueReaded -= OnEKNewValueReaded;
            }

            readerEK = ((IReader)((ComboBox)sender).SelectedItem);

            readerEK.NewValueReaded += OnEKNewValueReaded;

            readerEK.CyclicRead = true;

            readerEK.Read(WorkBench.Enums.UOMType.Current);

        }

        private void SearchEKOnSerialPorts()
        {
            lbl_cnahValue.Text = "поиск....";

            foreach (var item in Factory.serialPortNamesWithEK())
            {
                cb_SP.Items.Add(item);
            }

            if (cb_SP.Items.Count > 0)
            {
                cb_SP.SelectedItem = cb_SP.Items[0];

                lbl_cnahValue.Text = "---.----";

                if (cb_SP.Items.Count == 1)
                {
                    cb_SP.Enabled = false;
                }
            }
            else
            {
                lbl_cnahValue.Font = new Font(

                    lbl_cnahValue.Font.FontFamily,

                    (float)(lbl_cnahValue.Font.Size * 0.7),

                    lbl_cnahValue.Font.Style);

                lbl_cnahValue.Text = "не найдено :(";

                cb_chanNUM.Enabled = false;

                cb_SP.Enabled = false;
            }

        }

        #endregion

        #region CPC

        private void OnCPCNewValueReaded(IReader reader)
        {
            cpcmeasures.Add(reader.lastValue);

            if (cpcmeasures.Count > 1000)
            {
                while (cpcmeasures.Count > 1000)
                {
                    cpcmeasures.RemoveAt(0);
                }

                cpcmean = cpcmeasures.Average((om) => om.Value);
                
                double stdev = 0;

                foreach (var item in cpcmeasures)
                {
                    stdev += Math.Pow(item.Value - cpcmean, 2);
                }

                stdev = Math.Sqrt(stdev);
                setLabelText(cpcmean.ToString(), lbl_cpcmean);
                setLabelText(stdev.ToString(), lbl_CPCstdev);

                double cpcLRSlope = LRSlope(cpcmeasures);
                setLabelText(cpcLRSlope.ToString("N4"), lbl_CPCLRSlope);
                if (Math.Round(cpcLRSlope, 3) == 0)
                {
                    setLabelText("стабильно", lbl_CPCstability);

                    cpcTrendStatus = TrendStatus.Stable;
                }
                else
                {
                    if (cpcLRSlope < 0)
                    {
                        setLabelText("уменьшается", lbl_CPCstability);

                        cpcTrendStatus = TrendStatus.GrowDown;

                    }
                    else
                    {
                        setLabelText("увеличивается", lbl_CPCstability);

                        cpcTrendStatus = TrendStatus.GrowUP;

                    }
                }


            }

            setLabelText(
             string.Format("{0} {1}",
             reader.lastValue.Value.ToString("N4"),
             reader.lastValue.UOM.Name),
             lbl_cpc_read
             );


        }

        private delegate void SafeCallLabelDelegate(string txt, Label label);
        private void setLabelText(string txt, Label label)
        {
            if (label.InvokeRequired)
            {
                var d = new SafeCallLabelDelegate(setLabelText);
                label.BeginInvoke(d, new object[] { txt, label });
            }
            else
            {
                label.Text = txt;
            }
        }
        private delegate void SafeCallTextBoxDelegate(string txt, TextBox textbox);
        private void setTextBoxText(string txt, TextBox textbox)
        {
            if (textbox.InvokeRequired)
            {
                var d = new SafeCallTextBoxDelegate(setTextBoxText);
                textbox.BeginInvoke(d, new object[] { txt, textbox });
            }
            else
            {
                textbox.Text = txt;
            }
        }

        private void btn_openCPC_Click(object sender, EventArgs e)
        {
            switch (startedCPC)
            {
                case false:
                    instrumentCPC6000 = (IInstrument)cb_cpc.SelectedItem;

                    lbl_cpcStatus.Text = "поиск...";

                    if (instrumentCPC6000.Open() )
                    {
                        cpc_start();
                    }
                    else
                    {
                        lbl_cpcStatus.Text = "нет связи";
                    }
                    
                    break;
                case true:
                    cpc_stop();

                    instrumentCPC6000.Close();

                    break;
                default:
                    break;
            }


        }
        #region CPC start/stop routines

        private void cpc_start()
        {
            //*************************************************************
                startedCPC = true;

                lbl_cpcStatus.Text = "ок.";

                cb_cpcChannels.Items.Clear();

            
                foreach (var item in instrumentCPC6000.Channels)
                {

                    cb_cpcChannels.Items.Add(item);
                }

                if (cb_cpcChannels.Items.Count > 0)
                {
                    cb_cpcChannels.SelectedIndex = 0;


                }

                btn_openCPC.Text = "Разорвать связь";
            //*************************************************************


            btn_setSetPoint.Enabled = true;
            btn_StepUp.Enabled = true;
            btn_StepDown.Enabled = true;
            tb_cpcStep.Enabled = true;
            tb_newSetPoint.Enabled = true;
            cb_cpcChannels.Enabled = true;

            //readerCPC.CyclicRead = true;

            //readerCPC.NewValueReaded += OnCPCNewValueReaded;

            //readerCPC.Read(WorkBench.Enums.UOMType.Pressure);

        }

        private void cpc_stop()
        {
            //*************************************************************
                startedCPC = false;

                lbl_cpcStatus.Text = "нет связи.";

                cb_cpcChannels.Items.Clear();

                btn_openCPC.Text = "Установить связь";
            //*************************************************************

            readerCPC.CyclicRead = false;

            readerCPC.NewValueReaded -= OnCPCNewValueReaded;

            readerCPC = null;

            generatorCPC = null;

            lbl_cpc_read.Text = "---.----";
            btn_setSetPoint.Enabled = false;
            btn_StepUp.Enabled = false;
            btn_StepDown.Enabled = false;
            tb_cpcStep.Enabled = false;
            tb_newSetPoint.Enabled = false;
            cb_cpcChannels.Enabled = false;


        }

        #endregion

        #region CPC Step UP DOWN buttons

        private void btn_StepUp_Click(object sender, EventArgs e)
        {
            double steppoint = double.Parse(
                    tb_cpcStep.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures));
            double SETpoint = double.Parse(
                    tb_cpcSetPoint.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures));
            
            generatorCPC.SetPoint = new OneMeasureResult() { Value = SETpoint + steppoint, UOM = (IUOM)cbScaleUOM.SelectedItem, dateTimeOfMeasurement = DateTime.Now };
            
            tb_cpcSetPoint.Text = generatorCPC.SetPoint.ToString();
            //tb_cpcSetPoint.Text = (SETpoint + steppoint).ToString();

        }

        private void btn_StepDown_Click(object sender, EventArgs e)
        {
            double steppoint = double.Parse(
                    tb_cpcStep.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures));
            double SETpoint = double.Parse(
                    tb_cpcSetPoint.Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures));
            
            generatorCPC.SetPoint = new OneMeasureResult() { Value = SETpoint - steppoint, UOM = (IUOM)cbScaleUOM.SelectedItem, dateTimeOfMeasurement = DateTime.Now };

            tb_cpcSetPoint.Text = generatorCPC.SetPoint.ToString();
            //tb_cpcSetPoint.Text = (SETpoint - steppoint).ToString();
        }

        #endregion

        private void tb_cpcStep_TextChanged(object sender, EventArgs e)
        {
            double steppoint;

            if (double.TryParse(
                    ((TextBox)sender).Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures),
                    out steppoint))
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

        private void tb_newSetPoint_TextChanged(object sender, EventArgs e)
        {
            double newSETpoint;

            if (double.TryParse(
                    ((TextBox)sender).Text.Replace(',', '.'),
                    NumberStyles.Float,
                    new CultureInfo((int)CultureTypes.NeutralCultures),
                    out newSETpoint))
            {
                btn_setSetPoint.Enabled = true;
            }
            else
            {
                btn_setSetPoint.Enabled = false;
            }

        }

        private void btn_setSetPoint_Click(object sender, EventArgs e)
        {
            setSetPoint();
        }

        private void setSetPoint()
        {
            double SETpoint = double.Parse(
                tb_newSetPoint.Text.Replace(',', '.'),
                NumberStyles.Float,
                new CultureInfo((int)CultureTypes.NeutralCultures));

            generatorCPC.SetPoint = new OneMeasureResult() { Value = SETpoint, UOM = (IUOM)cbScaleUOM.SelectedItem, dateTimeOfMeasurement = DateTime.Now }; 

            tb_cpcSetPoint.Text = generatorCPC.SetPoint.ToString();
        }

        private void setCPCOperationMode(WorkBench.Enums.PressureControllerOperationMode operationMode)
        {
            if (generatorCPC != null)
            {
                generatorCPC.OperationMode = operationMode;
            }
        }

        #region CPC MODE Radio Buttons

        private void rb_Control_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setCPCOperationMode( WorkBench.Enums.PressureControllerOperationMode.CONTROL);
            }
        }

        private void rb_Vent_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setCPCOperationMode( WorkBench.Enums.PressureControllerOperationMode.VENT);
            }
        }

        private void rb_Measure_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setCPCOperationMode( WorkBench.Enums.PressureControllerOperationMode.MEASURE);
            }
        }

        private void rb_StandBY_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                setCPCOperationMode( WorkBench.Enums.PressureControllerOperationMode.STANDBY);
            }
        }
        #endregion

        private void cb_cpcChannels_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (readerCPC != null)
            {

                readerCPC.CyclicRead = false;

                readerCPC.NewValueReaded -= OnCPCNewValueReaded;

            }

            readerCPC = (IReader)((ComboBox)sender).SelectedItem;

            readerCPC.NewValueReaded += OnCPCNewValueReaded;

            readerCPC.CyclicRead = true;

            readerCPC.Read(WorkBench.Enums.UOMType.Pressure);


            generatorCPC = (IPressureGenerator)cb_cpcChannels.SelectedItem;

            tb_cpcSetPoint.Text = generatorCPC.SetPoint.Value.ToString("N4");

            ReadCPCOperationModeToRadioButtons();

        }

        private void ReadCPCOperationModeToRadioButtons()
        { 
            switch (generatorCPC.OperationMode)
            {
                case WorkBench.Enums.PressureControllerOperationMode.UNKNOWN:
                    break;
                case WorkBench.Enums.PressureControllerOperationMode.STANDBY:
                    rb_StandBY.Checked = true;
                    break;
                case WorkBench.Enums.PressureControllerOperationMode.MEASURE:
                    rb_Measure.Checked = true;
                    break;
                case WorkBench.Enums.PressureControllerOperationMode.CONTROL:
                    rb_Control.Checked = true;
                    break;
                case WorkBench.Enums.PressureControllerOperationMode.VENT:
                    rb_Vent.Checked = true;
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
                btn_setSetPoint_Click(this, new EventArgs());
            }
        }

        private void btn_copyMeansToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(string.Format("{0}\t{1}", lbl_cpcmean.Text, lbl_ekmean.Text));

        }

        private static double LRSlope ( List<OneMeasureResult> oneMeasureResults)
        {
            double x = 1;
            double xtotal = 0;
            double y = 0;
            double ytotal = 0;
            double XmultY = 0;
            double XmultYtotal = 0;
            double Xsqr = 0;
            double Xsqrtotal = 0;

            double b = 0;

            foreach (var item in oneMeasureResults)
            {
                xtotal += x;
                ytotal += item.Value;
                XmultY = item.Value * x;
                XmultYtotal += XmultY;
                Xsqr = Math.Pow( x, 2);
                Xsqrtotal += Xsqr;
                x++;
            }
            x--;
            b = (x * XmultYtotal - xtotal * ytotal) / ( x * Xsqrtotal - Math.Pow(xtotal, 2 ) );
            return b;
        }

        private void tbScaleMin_TextChanged(object sender, EventArgs e)
        {
            fillComputedPressure();
        }

        private void tbScaleMax_TextChanged(object sender, EventArgs e)
        {
            fillComputedPressure();
        }

        private void btnStartAutoCal_Click(object sender, EventArgs e)
        {
            if (startedEK && startedCPC)
            {

                generatorCPC.SetPoint = new OneMeasureResult() { Value = 0, UOM = (IUOM)cbScaleUOM.SelectedItem, dateTimeOfMeasurement = DateTime.Now };

                setTextBoxText(generatorCPC.SetPoint.ToString(), tb_cpcSetPoint);

                generatorCPC.OperationMode = WorkBench.Enums.PressureControllerOperationMode.CONTROL;
                ReadCPCOperationModeToRadioButtons();

                while (cpcTrendStatus != TrendStatus.Stable) { Application.DoEvents(); }

                foreach (DataGridViewRow item in dataGridView1.Rows)
                {
                    double setpoint = (double)item.Cells[calcPressure.Name].Value;

                    generatorCPC.SetPoint = new OneMeasureResult() { Value = setpoint, UOM = (IUOM)cbScaleUOM.SelectedItem, dateTimeOfMeasurement = DateTime.Now };

                    setTextBoxText(generatorCPC.SetPoint.ToString(), tb_cpcSetPoint);

                    Thread.Sleep(150);

                    while (ekTrendStatus != TrendStatus.Stable | cpcTrendStatus != TrendStatus.Stable)
                    { Application.DoEvents(); }

                    item.Cells[cpcPressure.Name].Value = cpcmean;
                    item.Cells[ekCurrent.Name].Value = ekmean;
                    item.Cells[error.Name].Value = ((ekmean  - 4) / 16 * (scaleMax - scaleMin) - cpcmean) / (scaleMax - scaleMin) * 100;
                }

                generatorCPC.SetPoint = new OneMeasureResult() { Value = 0, UOM = (IUOM)cbScaleUOM.SelectedItem, dateTimeOfMeasurement = DateTime.Now }; 

                while ( cpcTrendStatus != TrendStatus.Stable) { Application.DoEvents(); }

                generatorCPC.OperationMode = WorkBench.Enums.PressureControllerOperationMode.MEASURE;

                ReadCPCOperationModeToRadioButtons();

            }
        }

    }
}
