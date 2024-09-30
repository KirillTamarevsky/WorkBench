
namespace benchGUI
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            cb_CurrentMeasuringInstruments = new System.Windows.Forms.ComboBox();
            cb_CurrentInstrumentChannels = new System.Windows.Forms.ComboBox();
            lbl_cnahValue = new System.Windows.Forms.Label();
            btn_openCurrentMeasureInstrument = new System.Windows.Forms.Button();
            cb_PressureGeneratorInstrument = new System.Windows.Forms.ComboBox();
            btn_openPressureInstrument = new System.Windows.Forms.Button();
            cb_cpcChannels = new System.Windows.Forms.ComboBox();
            lbl_cpc_read = new System.Windows.Forms.Label();
            tb_cpcStep = new System.Windows.Forms.TextBox();
            tb_PressureSetPoint = new System.Windows.Forms.TextBox();
            btn_StepUp = new System.Windows.Forms.Button();
            btn_StepDown = new System.Windows.Forms.Button();
            btn_setSetPoint = new System.Windows.Forms.Button();
            tb_newSetPoint = new System.Windows.Forms.TextBox();
            rb_StandBY = new System.Windows.Forms.RadioButton();
            rb_Measure = new System.Windows.Forms.RadioButton();
            rb_Control = new System.Windows.Forms.RadioButton();
            rb_Vent = new System.Windows.Forms.RadioButton();
            lbl_cpcStatus = new System.Windows.Forms.Label();
            dataGridView1 = new System.Windows.Forms.DataGridView();
            percent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            calcPressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            cpcPressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ekCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            tbScaleMin = new System.Windows.Forms.TextBox();
            tbScaleMax = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            btnStartAutoCal = new System.Windows.Forms.Button();
            cbPressureScaleUOM = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            cb_PressureReaderGeneratorSpan = new System.Windows.Forms.ComboBox();
            cb_currentReaderSpan = new System.Windows.Forms.ComboBox();
            btn_ZeroPressure = new System.Windows.Forms.Button();
            cb_HART_SerialPort = new System.Windows.Forms.ComboBox();
            btn_ReadHART_Scale = new System.Windows.Forms.Button();
            btn_HART_ZEROTRIM = new System.Windows.Forms.Button();
            gb_HART = new System.Windows.Forms.GroupBox();
            btn_HART_trim20mA = new System.Windows.Forms.Button();
            btn_HART_trim4mA = new System.Windows.Forms.Button();
            btn_HART_set_0mA = new System.Windows.Forms.Button();
            lbl_HART_Xfer_Function = new System.Windows.Forms.Label();
            cb_HART_Xfer_Function = new System.Windows.Forms.ComboBox();
            btn_HART_set_20mA = new System.Windows.Forms.Button();
            btn_HART_set_4mA = new System.Windows.Forms.Button();
            lbl_HART_Damping = new System.Windows.Forms.Label();
            btn_HART_open = new System.Windows.Forms.Button();
            tb_HART_Damping = new System.Windows.Forms.TextBox();
            btn_pressureMicroStepDown = new System.Windows.Forms.Button();
            btn_pressureMicrostepUP = new System.Windows.Forms.Button();
            tb_pressureMicroStep = new System.Windows.Forms.TextBox();
            nUD_CalibrationCyclesCount = new System.Windows.Forms.NumericUpDown();
            label4 = new System.Windows.Forms.Label();
            plot_result = new ScottPlot.FormsPlot();
            tb_HART_PV = new System.Windows.Forms.TextBox();
            tb_HART_PV_MA = new System.Windows.Forms.TextBox();
            tb_HART_TAG = new System.Windows.Forms.TextBox();
            tb_HART_SV = new System.Windows.Forms.TextBox();
            tb_HART_TV = new System.Windows.Forms.TextBox();
            tb_HART_QV = new System.Windows.Forms.TextBox();
            tb_longTag = new System.Windows.Forms.TextBox();
            btn_HART_BURST_OFF = new System.Windows.Forms.Button();
            btn_HART_BURST_ON = new System.Windows.Forms.Button();
            chkBx_AutoZeroAll = new System.Windows.Forms.CheckBox();
            nUD_PercentPoints = new System.Windows.Forms.NumericUpDown();
            label5 = new System.Windows.Forms.Label();
            chkBx_autoDATRIM = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            gb_HART.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nUD_CalibrationCyclesCount).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nUD_PercentPoints).BeginInit();
            SuspendLayout();
            // 
            // cb_CurrentMeasuringInstruments
            // 
            cb_CurrentMeasuringInstruments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_CurrentMeasuringInstruments.FormattingEnabled = true;
            cb_CurrentMeasuringInstruments.Location = new System.Drawing.Point(15, 15);
            cb_CurrentMeasuringInstruments.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_CurrentMeasuringInstruments.Name = "cb_CurrentMeasuringInstruments";
            cb_CurrentMeasuringInstruments.Size = new System.Drawing.Size(500, 23);
            cb_CurrentMeasuringInstruments.TabIndex = 0;
            // 
            // cb_CurrentInstrumentChannels
            // 
            cb_CurrentInstrumentChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_CurrentInstrumentChannels.FormattingEnabled = true;
            cb_CurrentInstrumentChannels.Location = new System.Drawing.Point(15, 43);
            cb_CurrentInstrumentChannels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_CurrentInstrumentChannels.Name = "cb_CurrentInstrumentChannels";
            cb_CurrentInstrumentChannels.Size = new System.Drawing.Size(500, 23);
            cb_CurrentInstrumentChannels.TabIndex = 2;
            cb_CurrentInstrumentChannels.SelectedIndexChanged += cb_chanNUM_SelectedIndexChanged;
            // 
            // lbl_cnahValue
            // 
            lbl_cnahValue.BackColor = System.Drawing.SystemColors.Control;
            lbl_cnahValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_cnahValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 65.25F, System.Drawing.FontStyle.Bold);
            lbl_cnahValue.ForeColor = System.Drawing.SystemColors.ControlText;
            lbl_cnahValue.Location = new System.Drawing.Point(15, 129);
            lbl_cnahValue.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_cnahValue.Name = "lbl_cnahValue";
            lbl_cnahValue.Size = new System.Drawing.Size(600, 124);
            lbl_cnahValue.TabIndex = 3;
            lbl_cnahValue.Text = "---.----";
            lbl_cnahValue.Click += lbl_cnahValue_Click;
            // 
            // btn_openCurrentMeasureInstrument
            // 
            btn_openCurrentMeasureInstrument.BackColor = System.Drawing.SystemColors.Control;
            btn_openCurrentMeasureInstrument.Location = new System.Drawing.Point(525, 15);
            btn_openCurrentMeasureInstrument.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_openCurrentMeasureInstrument.Name = "btn_openCurrentMeasureInstrument";
            btn_openCurrentMeasureInstrument.Size = new System.Drawing.Size(90, 50);
            btn_openCurrentMeasureInstrument.TabIndex = 4;
            btn_openCurrentMeasureInstrument.Text = "Установить связь";
            btn_openCurrentMeasureInstrument.UseVisualStyleBackColor = false;
            btn_openCurrentMeasureInstrument.Click += btn_openCurrentMeasureInstrument_Click;
            // 
            // cb_PressureGeneratorInstrument
            // 
            cb_PressureGeneratorInstrument.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_PressureGeneratorInstrument.FormattingEnabled = true;
            cb_PressureGeneratorInstrument.Location = new System.Drawing.Point(624, 15);
            cb_PressureGeneratorInstrument.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_PressureGeneratorInstrument.Name = "cb_PressureGeneratorInstrument";
            cb_PressureGeneratorInstrument.Size = new System.Drawing.Size(500, 23);
            cb_PressureGeneratorInstrument.TabIndex = 5;
            // 
            // btn_openPressureInstrument
            // 
            btn_openPressureInstrument.Location = new System.Drawing.Point(1134, 15);
            btn_openPressureInstrument.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_openPressureInstrument.Name = "btn_openPressureInstrument";
            btn_openPressureInstrument.Size = new System.Drawing.Size(90, 50);
            btn_openPressureInstrument.TabIndex = 6;
            btn_openPressureInstrument.Text = "Установить связь";
            btn_openPressureInstrument.UseVisualStyleBackColor = true;
            btn_openPressureInstrument.Click += btn_openPressureMeasureInstrument_Click;
            // 
            // cb_cpcChannels
            // 
            cb_cpcChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_cpcChannels.Enabled = false;
            cb_cpcChannels.FormattingEnabled = true;
            cb_cpcChannels.Location = new System.Drawing.Point(624, 43);
            cb_cpcChannels.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_cpcChannels.Name = "cb_cpcChannels";
            cb_cpcChannels.Size = new System.Drawing.Size(500, 23);
            cb_cpcChannels.TabIndex = 7;
            cb_cpcChannels.SelectedIndexChanged += cb_cpcChannels_SelectedIndexChanged;
            // 
            // lbl_cpc_read
            // 
            lbl_cpc_read.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_cpc_read.Font = new System.Drawing.Font("Microsoft Sans Serif", 65.25F, System.Drawing.FontStyle.Bold);
            lbl_cpc_read.Location = new System.Drawing.Point(624, 129);
            lbl_cpc_read.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_cpc_read.Name = "lbl_cpc_read";
            lbl_cpc_read.Size = new System.Drawing.Size(600, 124);
            lbl_cpc_read.TabIndex = 3;
            lbl_cpc_read.Text = "---.----";
            // 
            // tb_cpcStep
            // 
            tb_cpcStep.Enabled = false;
            tb_cpcStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            tb_cpcStep.Location = new System.Drawing.Point(761, 334);
            tb_cpcStep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_cpcStep.Name = "tb_cpcStep";
            tb_cpcStep.Size = new System.Drawing.Size(132, 44);
            tb_cpcStep.TabIndex = 9;
            tb_cpcStep.Text = "1.0";
            tb_cpcStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            tb_cpcStep.WordWrap = false;
            tb_cpcStep.TextChanged += tb_cpcStep_TextChanged;
            // 
            // tb_PressureSetPoint
            // 
            tb_PressureSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            tb_PressureSetPoint.Location = new System.Drawing.Point(901, 334);
            tb_PressureSetPoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_PressureSetPoint.Name = "tb_PressureSetPoint";
            tb_PressureSetPoint.ReadOnly = true;
            tb_PressureSetPoint.Size = new System.Drawing.Size(140, 44);
            tb_PressureSetPoint.TabIndex = 10;
            tb_PressureSetPoint.Text = "0.0";
            tb_PressureSetPoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            tb_PressureSetPoint.WordWrap = false;
            tb_PressureSetPoint.KeyDown += tb_PressureSetPoint_KeyDown;
            // 
            // btn_StepUp
            // 
            btn_StepUp.Enabled = false;
            btn_StepUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            btn_StepUp.Location = new System.Drawing.Point(761, 284);
            btn_StepUp.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_StepUp.Name = "btn_StepUp";
            btn_StepUp.Size = new System.Drawing.Size(132, 44);
            btn_StepUp.TabIndex = 11;
            btn_StepUp.Text = "+";
            btn_StepUp.UseVisualStyleBackColor = true;
            btn_StepUp.Click += btn_StepUp_Click;
            // 
            // btn_StepDown
            // 
            btn_StepDown.Enabled = false;
            btn_StepDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            btn_StepDown.Location = new System.Drawing.Point(761, 384);
            btn_StepDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_StepDown.Name = "btn_StepDown";
            btn_StepDown.Size = new System.Drawing.Size(132, 44);
            btn_StepDown.TabIndex = 12;
            btn_StepDown.Text = "-";
            btn_StepDown.UseVisualStyleBackColor = true;
            btn_StepDown.Click += btn_StepDown_Click;
            // 
            // btn_setSetPoint
            // 
            btn_setSetPoint.Enabled = false;
            btn_setSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F);
            btn_setSetPoint.Location = new System.Drawing.Point(1047, 333);
            btn_setSetPoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_setSetPoint.Name = "btn_setSetPoint";
            btn_setSetPoint.Size = new System.Drawing.Size(174, 44);
            btn_setSetPoint.TabIndex = 13;
            btn_setSetPoint.Text = "New Set Point";
            btn_setSetPoint.UseVisualStyleBackColor = true;
            btn_setSetPoint.Click += btn_setSetPoint_Click;
            // 
            // tb_newSetPoint
            // 
            tb_newSetPoint.Enabled = false;
            tb_newSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            tb_newSetPoint.Location = new System.Drawing.Point(1047, 284);
            tb_newSetPoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_newSetPoint.Name = "tb_newSetPoint";
            tb_newSetPoint.Size = new System.Drawing.Size(173, 44);
            tb_newSetPoint.TabIndex = 14;
            tb_newSetPoint.Text = "0";
            tb_newSetPoint.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            tb_newSetPoint.WordWrap = false;
            tb_newSetPoint.TextChanged += tb_newSetPoint_TextChanged;
            tb_newSetPoint.KeyPress += tb_newSetPoint_KeyPress;
            // 
            // rb_StandBY
            // 
            rb_StandBY.AutoSize = true;
            rb_StandBY.Location = new System.Drawing.Point(628, 258);
            rb_StandBY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rb_StandBY.Name = "rb_StandBY";
            rb_StandBY.Size = new System.Drawing.Size(75, 19);
            rb_StandBY.TabIndex = 15;
            rb_StandBY.TabStop = true;
            rb_StandBY.Text = "STANDBY";
            rb_StandBY.UseVisualStyleBackColor = true;
            rb_StandBY.CheckedChanged += rb_StandBY_CheckedChanged;
            // 
            // rb_Measure
            // 
            rb_Measure.AutoSize = true;
            rb_Measure.Location = new System.Drawing.Point(724, 258);
            rb_Measure.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rb_Measure.Name = "rb_Measure";
            rb_Measure.Size = new System.Drawing.Size(77, 19);
            rb_Measure.TabIndex = 16;
            rb_Measure.TabStop = true;
            rb_Measure.Text = "MEASURE";
            rb_Measure.UseVisualStyleBackColor = true;
            rb_Measure.CheckedChanged += rb_Measure_CheckedChanged;
            // 
            // rb_Control
            // 
            rb_Control.AutoSize = true;
            rb_Control.Location = new System.Drawing.Point(821, 258);
            rb_Control.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rb_Control.Name = "rb_Control";
            rb_Control.Size = new System.Drawing.Size(79, 19);
            rb_Control.TabIndex = 17;
            rb_Control.TabStop = true;
            rb_Control.Text = "CONTROL";
            rb_Control.UseVisualStyleBackColor = true;
            rb_Control.CheckedChanged += rb_Control_CheckedChanged;
            // 
            // rb_Vent
            // 
            rb_Vent.AutoSize = true;
            rb_Vent.Location = new System.Drawing.Point(918, 258);
            rb_Vent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            rb_Vent.Name = "rb_Vent";
            rb_Vent.Size = new System.Drawing.Size(53, 19);
            rb_Vent.TabIndex = 18;
            rb_Vent.TabStop = true;
            rb_Vent.Text = "VENT";
            rb_Vent.UseVisualStyleBackColor = true;
            rb_Vent.CheckedChanged += rb_Vent_CheckedChanged;
            // 
            // lbl_cpcStatus
            // 
            lbl_cpcStatus.Font = new System.Drawing.Font("Consolas", 12F);
            lbl_cpcStatus.Location = new System.Drawing.Point(624, 102);
            lbl_cpcStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_cpcStatus.Name = "lbl_cpcStatus";
            lbl_cpcStatus.Size = new System.Drawing.Size(602, 27);
            lbl_cpcStatus.TabIndex = 19;
            lbl_cpcStatus.Text = "нет связи";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { percent, calcPressure, cpcPressure, ekCurrent, error });
            dataGridView1.Location = new System.Drawing.Point(15, 459);
            dataGridView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.ShowEditingIcon = false;
            dataGridView1.Size = new System.Drawing.Size(514, 275);
            dataGridView1.TabIndex = 32;
            // 
            // percent
            // 
            percent.HeaderText = "%шкалы";
            percent.MinimumWidth = 6;
            percent.Name = "percent";
            percent.ReadOnly = true;
            percent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            percent.Width = 60;
            // 
            // calcPressure
            // 
            calcPressure.HeaderText = "Расчетное давление";
            calcPressure.MinimumWidth = 6;
            calcPressure.Name = "calcPressure";
            calcPressure.ReadOnly = true;
            calcPressure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // cpcPressure
            // 
            cpcPressure.HeaderText = "Действительное давление";
            cpcPressure.MinimumWidth = 6;
            cpcPressure.Name = "cpcPressure";
            cpcPressure.ReadOnly = true;
            cpcPressure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ekCurrent
            // 
            ekCurrent.HeaderText = "ток";
            ekCurrent.MinimumWidth = 6;
            ekCurrent.Name = "ekCurrent";
            ekCurrent.ReadOnly = true;
            ekCurrent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // error
            // 
            error.HeaderText = "погрешность, %";
            error.MinimumWidth = 6;
            error.Name = "error";
            error.ReadOnly = true;
            error.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // tbScaleMin
            // 
            tbScaleMin.Location = new System.Drawing.Point(15, 281);
            tbScaleMin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbScaleMin.Name = "tbScaleMin";
            tbScaleMin.Size = new System.Drawing.Size(69, 23);
            tbScaleMin.TabIndex = 29;
            tbScaleMin.TextChanged += tbScaleMin_TextChanged;
            tbScaleMin.KeyPress += tbScaleMax_KeyPress;
            // 
            // tbScaleMax
            // 
            tbScaleMax.Location = new System.Drawing.Point(92, 281);
            tbScaleMax.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbScaleMax.Name = "tbScaleMax";
            tbScaleMax.Size = new System.Drawing.Size(77, 23);
            tbScaleMax.TabIndex = 30;
            tbScaleMax.TextChanged += tbScaleMax_TextChanged;
            tbScaleMax.KeyPress += tbScaleMax_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(15, 261);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 15);
            label1.TabIndex = 33;
            label1.Text = "шкала мин.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(92, 261);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(76, 15);
            label2.TabIndex = 34;
            label2.Text = "шкала макс.";
            // 
            // btnStartAutoCal
            // 
            btnStartAutoCal.Location = new System.Drawing.Point(459, 739);
            btnStartAutoCal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStartAutoCal.Name = "btnStartAutoCal";
            btnStartAutoCal.Size = new System.Drawing.Size(69, 27);
            btnStartAutoCal.TabIndex = 31;
            btnStartAutoCal.Text = "Старт";
            btnStartAutoCal.UseVisualStyleBackColor = true;
            btnStartAutoCal.Click += btnStartAutoCal_Click;
            // 
            // cbPressureScaleUOM
            // 
            cbPressureScaleUOM.BackColor = System.Drawing.SystemColors.ControlLightLight;
            cbPressureScaleUOM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbPressureScaleUOM.FormattingEnabled = true;
            cbPressureScaleUOM.Location = new System.Drawing.Point(177, 280);
            cbPressureScaleUOM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbPressureScaleUOM.Name = "cbPressureScaleUOM";
            cbPressureScaleUOM.Size = new System.Drawing.Size(89, 23);
            cbPressureScaleUOM.TabIndex = 35;
            cbPressureScaleUOM.SelectedIndexChanged += cbScaleUOM_SelectedIndexChanged;
            cbPressureScaleUOM.KeyPress += tbScaleMax_KeyPress;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(177, 261);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(49, 15);
            label3.TabIndex = 34;
            label3.Text = "ед. изм.";
            // 
            // cb_PressureReaderGeneratorSpan
            // 
            cb_PressureReaderGeneratorSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_PressureReaderGeneratorSpan.Enabled = false;
            cb_PressureReaderGeneratorSpan.FormattingEnabled = true;
            cb_PressureReaderGeneratorSpan.Location = new System.Drawing.Point(624, 72);
            cb_PressureReaderGeneratorSpan.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_PressureReaderGeneratorSpan.Name = "cb_PressureReaderGeneratorSpan";
            cb_PressureReaderGeneratorSpan.Size = new System.Drawing.Size(500, 23);
            cb_PressureReaderGeneratorSpan.TabIndex = 36;
            cb_PressureReaderGeneratorSpan.SelectedIndexChanged += cb_PressureReaderGeneratorSpan_SelectedIndexChanged;
            // 
            // cb_currentReaderSpan
            // 
            cb_currentReaderSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_currentReaderSpan.Enabled = false;
            cb_currentReaderSpan.FormattingEnabled = true;
            cb_currentReaderSpan.Location = new System.Drawing.Point(15, 73);
            cb_currentReaderSpan.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_currentReaderSpan.Name = "cb_currentReaderSpan";
            cb_currentReaderSpan.Size = new System.Drawing.Size(500, 23);
            cb_currentReaderSpan.TabIndex = 37;
            cb_currentReaderSpan.SelectedIndexChanged += cb_currentReaderSpan_SelectedIndexChanged;
            // 
            // btn_ZeroPressure
            // 
            btn_ZeroPressure.Enabled = false;
            btn_ZeroPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            btn_ZeroPressure.Location = new System.Drawing.Point(1165, 256);
            btn_ZeroPressure.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_ZeroPressure.Name = "btn_ZeroPressure";
            btn_ZeroPressure.Size = new System.Drawing.Size(55, 23);
            btn_ZeroPressure.TabIndex = 38;
            btn_ZeroPressure.Text = ">0<";
            btn_ZeroPressure.UseVisualStyleBackColor = true;
            btn_ZeroPressure.Click += btn_ZeroPressure_Click;
            // 
            // cb_HART_SerialPort
            // 
            cb_HART_SerialPort.FormattingEnabled = true;
            cb_HART_SerialPort.Location = new System.Drawing.Point(7, 17);
            cb_HART_SerialPort.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_HART_SerialPort.Name = "cb_HART_SerialPort";
            cb_HART_SerialPort.Size = new System.Drawing.Size(99, 23);
            cb_HART_SerialPort.TabIndex = 39;
            // 
            // btn_ReadHART_Scale
            // 
            btn_ReadHART_Scale.Enabled = false;
            btn_ReadHART_Scale.Location = new System.Drawing.Point(433, 17);
            btn_ReadHART_Scale.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_ReadHART_Scale.Name = "btn_ReadHART_Scale";
            btn_ReadHART_Scale.Size = new System.Drawing.Size(57, 23);
            btn_ReadHART_Scale.TabIndex = 40;
            btn_ReadHART_Scale.Text = "шкала?";
            btn_ReadHART_Scale.UseVisualStyleBackColor = true;
            btn_ReadHART_Scale.Click += btn_ReadHART_Scale_Click;
            // 
            // btn_HART_ZEROTRIM
            // 
            btn_HART_ZEROTRIM.Enabled = false;
            btn_HART_ZEROTRIM.Location = new System.Drawing.Point(385, 17);
            btn_HART_ZEROTRIM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_HART_ZEROTRIM.Name = "btn_HART_ZEROTRIM";
            btn_HART_ZEROTRIM.Size = new System.Drawing.Size(40, 23);
            btn_HART_ZEROTRIM.TabIndex = 41;
            btn_HART_ZEROTRIM.Text = ">0<";
            btn_HART_ZEROTRIM.UseVisualStyleBackColor = true;
            btn_HART_ZEROTRIM.Click += btn_HART_ZEROTRIM_Click;
            // 
            // gb_HART
            // 
            gb_HART.Controls.Add(chkBx_autoDATRIM);
            gb_HART.Controls.Add(btn_HART_trim20mA);
            gb_HART.Controls.Add(btn_HART_trim4mA);
            gb_HART.Controls.Add(btn_HART_set_0mA);
            gb_HART.Controls.Add(lbl_HART_Xfer_Function);
            gb_HART.Controls.Add(cb_HART_Xfer_Function);
            gb_HART.Controls.Add(btn_HART_set_20mA);
            gb_HART.Controls.Add(btn_HART_set_4mA);
            gb_HART.Controls.Add(lbl_HART_Damping);
            gb_HART.Controls.Add(btn_HART_open);
            gb_HART.Controls.Add(tb_HART_Damping);
            gb_HART.Controls.Add(btn_HART_ZEROTRIM);
            gb_HART.Controls.Add(btn_ReadHART_Scale);
            gb_HART.Controls.Add(cb_HART_SerialPort);
            gb_HART.Location = new System.Drawing.Point(14, 311);
            gb_HART.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gb_HART.Name = "gb_HART";
            gb_HART.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gb_HART.Size = new System.Drawing.Size(586, 142);
            gb_HART.TabIndex = 42;
            gb_HART.TabStop = false;
            gb_HART.Text = "HART";
            // 
            // btn_HART_trim20mA
            // 
            btn_HART_trim20mA.Enabled = false;
            btn_HART_trim20mA.Location = new System.Drawing.Point(61, 115);
            btn_HART_trim20mA.Name = "btn_HART_trim20mA";
            btn_HART_trim20mA.Size = new System.Drawing.Size(51, 23);
            btn_HART_trim20mA.TabIndex = 47;
            btn_HART_trim20mA.Text = "trim20";
            btn_HART_trim20mA.UseVisualStyleBackColor = true;
            btn_HART_trim20mA.Click += btn_HART_trim20mA_Click;
            // 
            // btn_HART_trim4mA
            // 
            btn_HART_trim4mA.Enabled = false;
            btn_HART_trim4mA.Location = new System.Drawing.Point(7, 115);
            btn_HART_trim4mA.Name = "btn_HART_trim4mA";
            btn_HART_trim4mA.Size = new System.Drawing.Size(48, 23);
            btn_HART_trim4mA.TabIndex = 46;
            btn_HART_trim4mA.Text = "trim4";
            btn_HART_trim4mA.UseVisualStyleBackColor = true;
            btn_HART_trim4mA.Click += btn_HART_trim4mA_Click;
            // 
            // btn_HART_set_0mA
            // 
            btn_HART_set_0mA.Enabled = false;
            btn_HART_set_0mA.Location = new System.Drawing.Point(113, 86);
            btn_HART_set_0mA.Name = "btn_HART_set_0mA";
            btn_HART_set_0mA.Size = new System.Drawing.Size(43, 23);
            btn_HART_set_0mA.TabIndex = 45;
            btn_HART_set_0mA.Text = "0 mA";
            btn_HART_set_0mA.UseVisualStyleBackColor = true;
            btn_HART_set_0mA.Click += btn_HART_set_0mA_Click;
            // 
            // lbl_HART_Xfer_Function
            // 
            lbl_HART_Xfer_Function.AutoSize = true;
            lbl_HART_Xfer_Function.Location = new System.Drawing.Point(498, 66);
            lbl_HART_Xfer_Function.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_HART_Xfer_Function.Name = "lbl_HART_Xfer_Function";
            lbl_HART_Xfer_Function.Size = new System.Drawing.Size(80, 15);
            lbl_HART_Xfer_Function.TabIndex = 63;
            lbl_HART_Xfer_Function.Text = "Xfer_Function";
            // 
            // cb_HART_Xfer_Function
            // 
            cb_HART_Xfer_Function.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cb_HART_Xfer_Function.DropDownWidth = 200;
            cb_HART_Xfer_Function.FormattingEnabled = true;
            cb_HART_Xfer_Function.Location = new System.Drawing.Point(498, 87);
            cb_HART_Xfer_Function.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cb_HART_Xfer_Function.MaxDropDownItems = 20;
            cb_HART_Xfer_Function.Name = "cb_HART_Xfer_Function";
            cb_HART_Xfer_Function.Size = new System.Drawing.Size(89, 23);
            cb_HART_Xfer_Function.TabIndex = 62;
            cb_HART_Xfer_Function.SelectionChangeCommitted += cb_HART_Xfer_Function_SelectionChangeCommitted;
            // 
            // btn_HART_set_20mA
            // 
            btn_HART_set_20mA.Enabled = false;
            btn_HART_set_20mA.Location = new System.Drawing.Point(56, 86);
            btn_HART_set_20mA.Name = "btn_HART_set_20mA";
            btn_HART_set_20mA.Size = new System.Drawing.Size(50, 23);
            btn_HART_set_20mA.TabIndex = 44;
            btn_HART_set_20mA.Text = "20 mA";
            btn_HART_set_20mA.UseVisualStyleBackColor = true;
            btn_HART_set_20mA.Click += btn_HART_set_20mA_Click;
            // 
            // btn_HART_set_4mA
            // 
            btn_HART_set_4mA.Enabled = false;
            btn_HART_set_4mA.Location = new System.Drawing.Point(7, 86);
            btn_HART_set_4mA.Name = "btn_HART_set_4mA";
            btn_HART_set_4mA.Size = new System.Drawing.Size(43, 23);
            btn_HART_set_4mA.TabIndex = 43;
            btn_HART_set_4mA.Text = "4 mA";
            btn_HART_set_4mA.UseVisualStyleBackColor = true;
            btn_HART_set_4mA.Click += btn_HART_set_4mA_Click;
            // 
            // lbl_HART_Damping
            // 
            lbl_HART_Damping.AutoSize = true;
            lbl_HART_Damping.Location = new System.Drawing.Point(498, 20);
            lbl_HART_Damping.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_HART_Damping.Name = "lbl_HART_Damping";
            lbl_HART_Damping.Size = new System.Drawing.Size(55, 15);
            lbl_HART_Damping.TabIndex = 61;
            lbl_HART_Damping.Text = "damping";
            // 
            // btn_HART_open
            // 
            btn_HART_open.Location = new System.Drawing.Point(113, 16);
            btn_HART_open.Name = "btn_HART_open";
            btn_HART_open.Size = new System.Drawing.Size(68, 23);
            btn_HART_open.TabIndex = 42;
            btn_HART_open.Text = "Открыть";
            btn_HART_open.UseVisualStyleBackColor = true;
            btn_HART_open.Click += btn_HART_open_Click;
            // 
            // tb_HART_Damping
            // 
            tb_HART_Damping.Location = new System.Drawing.Point(498, 43);
            tb_HART_Damping.Name = "tb_HART_Damping";
            tb_HART_Damping.Size = new System.Drawing.Size(64, 23);
            tb_HART_Damping.TabIndex = 60;
            tb_HART_Damping.KeyPress += tb_HART_Damping_KeyPress;
            // 
            // btn_pressureMicroStepDown
            // 
            btn_pressureMicroStepDown.Enabled = false;
            btn_pressureMicroStepDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            btn_pressureMicroStepDown.Location = new System.Drawing.Point(625, 384);
            btn_pressureMicroStepDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_pressureMicroStepDown.Name = "btn_pressureMicroStepDown";
            btn_pressureMicroStepDown.Size = new System.Drawing.Size(132, 44);
            btn_pressureMicroStepDown.TabIndex = 45;
            btn_pressureMicroStepDown.Text = "-";
            btn_pressureMicroStepDown.UseVisualStyleBackColor = true;
            btn_pressureMicroStepDown.Click += btn_pressureMicroStepDown_Click;
            // 
            // btn_pressureMicrostepUP
            // 
            btn_pressureMicrostepUP.Enabled = false;
            btn_pressureMicrostepUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            btn_pressureMicrostepUP.Location = new System.Drawing.Point(624, 285);
            btn_pressureMicrostepUP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_pressureMicrostepUP.Name = "btn_pressureMicrostepUP";
            btn_pressureMicrostepUP.Size = new System.Drawing.Size(133, 44);
            btn_pressureMicrostepUP.TabIndex = 44;
            btn_pressureMicrostepUP.Text = "+";
            btn_pressureMicrostepUP.UseVisualStyleBackColor = true;
            btn_pressureMicrostepUP.Click += btn_pressureMicrostepUP_Click;
            // 
            // tb_pressureMicroStep
            // 
            tb_pressureMicroStep.Enabled = false;
            tb_pressureMicroStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
            tb_pressureMicroStep.Location = new System.Drawing.Point(625, 334);
            tb_pressureMicroStep.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tb_pressureMicroStep.Name = "tb_pressureMicroStep";
            tb_pressureMicroStep.Size = new System.Drawing.Size(132, 44);
            tb_pressureMicroStep.TabIndex = 43;
            tb_pressureMicroStep.Text = "1.0";
            tb_pressureMicroStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            tb_pressureMicroStep.WordWrap = false;
            tb_pressureMicroStep.TextChanged += tb_pressureMicroStep_TextChanged;
            // 
            // nUD_CalibrationCyclesCount
            // 
            nUD_CalibrationCyclesCount.Location = new System.Drawing.Point(537, 561);
            nUD_CalibrationCyclesCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nUD_CalibrationCyclesCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nUD_CalibrationCyclesCount.Name = "nUD_CalibrationCyclesCount";
            nUD_CalibrationCyclesCount.Size = new System.Drawing.Size(56, 23);
            nUD_CalibrationCyclesCount.TabIndex = 47;
            nUD_CalibrationCyclesCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            nUD_CalibrationCyclesCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(537, 538);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(63, 19);
            label4.TabIndex = 48;
            label4.Text = "циклы ->";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // plot_result
            // 
            plot_result.Location = new System.Drawing.Point(622, 434);
            plot_result.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            plot_result.Name = "plot_result";
            plot_result.Size = new System.Drawing.Size(602, 349);
            plot_result.TabIndex = 49;
            // 
            // tb_HART_PV
            // 
            tb_HART_PV.Enabled = false;
            tb_HART_PV.Location = new System.Drawing.Point(273, 279);
            tb_HART_PV.Name = "tb_HART_PV";
            tb_HART_PV.Size = new System.Drawing.Size(64, 23);
            tb_HART_PV.TabIndex = 51;
            // 
            // tb_HART_PV_MA
            // 
            tb_HART_PV_MA.Enabled = false;
            tb_HART_PV_MA.Location = new System.Drawing.Point(482, 255);
            tb_HART_PV_MA.Name = "tb_HART_PV_MA";
            tb_HART_PV_MA.Size = new System.Drawing.Size(64, 23);
            tb_HART_PV_MA.TabIndex = 52;
            // 
            // tb_HART_TAG
            // 
            tb_HART_TAG.Location = new System.Drawing.Point(273, 255);
            tb_HART_TAG.Name = "tb_HART_TAG";
            tb_HART_TAG.Size = new System.Drawing.Size(64, 23);
            tb_HART_TAG.TabIndex = 53;
            tb_HART_TAG.KeyPress += tbShortTag_KeyPress;
            // 
            // tb_HART_SV
            // 
            tb_HART_SV.Enabled = false;
            tb_HART_SV.Location = new System.Drawing.Point(343, 279);
            tb_HART_SV.Name = "tb_HART_SV";
            tb_HART_SV.Size = new System.Drawing.Size(64, 23);
            tb_HART_SV.TabIndex = 54;
            // 
            // tb_HART_TV
            // 
            tb_HART_TV.Enabled = false;
            tb_HART_TV.Location = new System.Drawing.Point(412, 279);
            tb_HART_TV.Name = "tb_HART_TV";
            tb_HART_TV.Size = new System.Drawing.Size(64, 23);
            tb_HART_TV.TabIndex = 55;
            // 
            // tb_HART_QV
            // 
            tb_HART_QV.Enabled = false;
            tb_HART_QV.Location = new System.Drawing.Point(482, 279);
            tb_HART_QV.Name = "tb_HART_QV";
            tb_HART_QV.Size = new System.Drawing.Size(64, 23);
            tb_HART_QV.TabIndex = 56;
            // 
            // tb_longTag
            // 
            tb_longTag.Location = new System.Drawing.Point(343, 255);
            tb_longTag.Name = "tb_longTag";
            tb_longTag.Size = new System.Drawing.Size(133, 23);
            tb_longTag.TabIndex = 58;
            tb_longTag.KeyPress += tb_longTag_KeyPress;
            // 
            // btn_HART_BURST_OFF
            // 
            btn_HART_BURST_OFF.Enabled = false;
            btn_HART_BURST_OFF.Location = new System.Drawing.Point(551, 255);
            btn_HART_BURST_OFF.Name = "btn_HART_BURST_OFF";
            btn_HART_BURST_OFF.Size = new System.Drawing.Size(64, 23);
            btn_HART_BURST_OFF.TabIndex = 48;
            btn_HART_BURST_OFF.Text = "BURSToff";
            btn_HART_BURST_OFF.UseVisualStyleBackColor = true;
            btn_HART_BURST_OFF.Click += btn_HART_BURST_OFF_Click;
            // 
            // btn_HART_BURST_ON
            // 
            btn_HART_BURST_ON.Enabled = false;
            btn_HART_BURST_ON.Location = new System.Drawing.Point(550, 280);
            btn_HART_BURST_ON.Name = "btn_HART_BURST_ON";
            btn_HART_BURST_ON.Size = new System.Drawing.Size(64, 23);
            btn_HART_BURST_ON.TabIndex = 59;
            btn_HART_BURST_ON.Text = "BURSTon";
            btn_HART_BURST_ON.UseVisualStyleBackColor = true;
            btn_HART_BURST_ON.Click += btn_HART_BURST_ON_Click;
            // 
            // chkBx_AutoZeroAll
            // 
            chkBx_AutoZeroAll.AutoSize = true;
            chkBx_AutoZeroAll.Location = new System.Drawing.Point(374, 744);
            chkBx_AutoZeroAll.Name = "chkBx_AutoZeroAll";
            chkBx_AutoZeroAll.Size = new System.Drawing.Size(78, 19);
            chkBx_AutoZeroAll.TabIndex = 64;
            chkBx_AutoZeroAll.Text = "обнулить";
            chkBx_AutoZeroAll.UseVisualStyleBackColor = true;
            // 
            // nUD_PercentPoints
            // 
            nUD_PercentPoints.Location = new System.Drawing.Point(537, 515);
            nUD_PercentPoints.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nUD_PercentPoints.Minimum = new decimal(new int[] { 2, 0, 0, 0 });
            nUD_PercentPoints.Name = "nUD_PercentPoints";
            nUD_PercentPoints.Size = new System.Drawing.Size(56, 23);
            nUD_PercentPoints.TabIndex = 65;
            nUD_PercentPoints.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            nUD_PercentPoints.Value = new decimal(new int[] { 4, 0, 0, 0 });
            nUD_PercentPoints.ValueChanged += nUD_PercentPoints_ValueChanged;
            // 
            // label5
            // 
            label5.Location = new System.Drawing.Point(537, 496);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(56, 15);
            label5.TabIndex = 66;
            label5.Text = "<- шаги";
            label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkBx_autoDATRIM
            // 
            chkBx_autoDATRIM.AutoSize = true;
            chkBx_autoDATRIM.Checked = true;
            chkBx_autoDATRIM.CheckState = System.Windows.Forms.CheckState.Checked;
            chkBx_autoDATRIM.Location = new System.Drawing.Point(187, 16);
            chkBx_autoDATRIM.Name = "chkBx_autoDATRIM";
            chkBx_autoDATRIM.Size = new System.Drawing.Size(100, 19);
            chkBx_autoDATRIM.TabIndex = 67;
            chkBx_autoDATRIM.Text = "авто D/A Trim";
            chkBx_autoDATRIM.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ClientSize = new System.Drawing.Size(1236, 781);
            Controls.Add(label5);
            Controls.Add(nUD_PercentPoints);
            Controls.Add(chkBx_AutoZeroAll);
            Controls.Add(btn_HART_BURST_ON);
            Controls.Add(btn_HART_BURST_OFF);
            Controls.Add(tb_longTag);
            Controls.Add(tb_HART_QV);
            Controls.Add(tb_HART_TV);
            Controls.Add(tb_HART_SV);
            Controls.Add(tb_HART_TAG);
            Controls.Add(tb_HART_PV_MA);
            Controls.Add(tb_HART_PV);
            Controls.Add(plot_result);
            Controls.Add(label4);
            Controls.Add(nUD_CalibrationCyclesCount);
            Controls.Add(btn_pressureMicroStepDown);
            Controls.Add(btn_pressureMicrostepUP);
            Controls.Add(tb_pressureMicroStep);
            Controls.Add(gb_HART);
            Controls.Add(btn_ZeroPressure);
            Controls.Add(cb_currentReaderSpan);
            Controls.Add(cb_PressureReaderGeneratorSpan);
            Controls.Add(cbPressureScaleUOM);
            Controls.Add(btnStartAutoCal);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tbScaleMax);
            Controls.Add(tbScaleMin);
            Controls.Add(dataGridView1);
            Controls.Add(lbl_cpcStatus);
            Controls.Add(rb_Vent);
            Controls.Add(rb_Control);
            Controls.Add(rb_Measure);
            Controls.Add(rb_StandBY);
            Controls.Add(btn_StepDown);
            Controls.Add(btn_setSetPoint);
            Controls.Add(btn_StepUp);
            Controls.Add(tb_PressureSetPoint);
            Controls.Add(tb_newSetPoint);
            Controls.Add(tb_cpcStep);
            Controls.Add(cb_cpcChannels);
            Controls.Add(btn_openPressureInstrument);
            Controls.Add(cb_PressureGeneratorInstrument);
            Controls.Add(btn_openCurrentMeasureInstrument);
            Controls.Add(lbl_cpc_read);
            Controls.Add(lbl_cnahValue);
            Controls.Add(cb_CurrentInstrumentChannels);
            Controls.Add(cb_CurrentMeasuringInstruments);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            Text = "Калибровка СИ давления";
            Load += Form1_Load;
            Shown += Form1_Shown;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            gb_HART.ResumeLayout(false);
            gb_HART.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nUD_CalibrationCyclesCount).EndInit();
            ((System.ComponentModel.ISupportInitialize)nUD_PercentPoints).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox cb_CurrentMeasuringInstruments;
        private System.Windows.Forms.ComboBox cb_CurrentInstrumentChannels;
        private System.Windows.Forms.Label lbl_cnahValue;
        private System.Windows.Forms.Button btn_openCurrentMeasureInstrument;
        private System.Windows.Forms.ComboBox cb_PressureGeneratorInstrument;
        private System.Windows.Forms.Button btn_openPressureInstrument;
        private System.Windows.Forms.ComboBox cb_cpcChannels;
        private System.Windows.Forms.Label lbl_cpc_read;
        private System.Windows.Forms.TextBox tb_cpcStep;
        private System.Windows.Forms.TextBox tb_PressureSetPoint;
        private System.Windows.Forms.Button btn_StepUp;
        private System.Windows.Forms.Button btn_StepDown;
        private System.Windows.Forms.Button btn_setSetPoint;
        private System.Windows.Forms.TextBox tb_newSetPoint;
        private System.Windows.Forms.RadioButton rb_StandBY;
        private System.Windows.Forms.RadioButton rb_Measure;
        private System.Windows.Forms.RadioButton rb_Control;
        private System.Windows.Forms.RadioButton rb_Vent;
        private System.Windows.Forms.Label lbl_cpcStatus;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox tbScaleMin;
        private System.Windows.Forms.TextBox tbScaleMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnStartAutoCal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_PressureReaderGeneratorSpan;
        private System.Windows.Forms.ComboBox cb_currentReaderSpan;
        private System.Windows.Forms.ComboBox cbPressureScaleUOM;
        private System.Windows.Forms.Button btn_ZeroPressure;
        private System.Windows.Forms.ComboBox cb_HART_SerialPort;
        private System.Windows.Forms.Button btn_ReadHART_Scale;
        private System.Windows.Forms.Button btn_HART_ZEROTRIM;
        private System.Windows.Forms.GroupBox gb_HART;
        private System.Windows.Forms.Button btn_pressureMicroStepDown;
        private System.Windows.Forms.Button btn_pressureMicrostepUP;
        private System.Windows.Forms.TextBox tb_pressureMicroStep;
        private System.Windows.Forms.NumericUpDown nUD_CalibrationCyclesCount;
        private System.Windows.Forms.Label label4;
        private ScottPlot.FormsPlot plot_result;
        private System.Windows.Forms.Button btn_HART_open;
        private System.Windows.Forms.Button btn_HART_set_0mA;
        private System.Windows.Forms.Button btn_HART_set_20mA;
        private System.Windows.Forms.Button btn_HART_set_4mA;
        private System.Windows.Forms.Button btn_HART_trim4mA;
        private System.Windows.Forms.Button btn_HART_trim20mA;
        private System.Windows.Forms.TextBox tb_HART_PV;
        private System.Windows.Forms.TextBox tb_HART_PV_MA;
        private System.Windows.Forms.TextBox tb_HART_TAG;
        private System.Windows.Forms.TextBox tb_HART_SV;
        private System.Windows.Forms.TextBox tb_HART_TV;
        private System.Windows.Forms.TextBox tb_HART_QV;
        private System.Windows.Forms.TextBox tb_longTag;
        private System.Windows.Forms.Button btn_HART_BURST_OFF;
        private System.Windows.Forms.Button btn_HART_BURST_ON;
        private System.Windows.Forms.TextBox tb_HART_Damping;
        private System.Windows.Forms.Label lbl_HART_Damping;
        private System.Windows.Forms.ComboBox cb_HART_Xfer_Function;
        private System.Windows.Forms.Label lbl_HART_Xfer_Function;
        private System.Windows.Forms.CheckBox chkBx_AutoZeroAll;
        private System.Windows.Forms.DataGridViewTextBoxColumn percent;
        private System.Windows.Forms.DataGridViewTextBoxColumn calcPressure;
        private System.Windows.Forms.DataGridViewTextBoxColumn cpcPressure;
        private System.Windows.Forms.DataGridViewTextBoxColumn ekCurrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
        private System.Windows.Forms.NumericUpDown nUD_PercentPoints;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkBx_autoDATRIM;
    }
}

