
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.cb_CurrentMeasuringInstruments = new System.Windows.Forms.ComboBox();
            this.cb_CurrentInstrumentChannels = new System.Windows.Forms.ComboBox();
            this.lbl_cnahValue = new System.Windows.Forms.Label();
            this.btn_openCurrentMeasureInstrument = new System.Windows.Forms.Button();
            this.cb_PressureGeneratorInstrument = new System.Windows.Forms.ComboBox();
            this.btn_openCPC = new System.Windows.Forms.Button();
            this.cb_cpcChannels = new System.Windows.Forms.ComboBox();
            this.lbl_cpc_read = new System.Windows.Forms.Label();
            this.tb_cpcStep = new System.Windows.Forms.TextBox();
            this.tb_PressureSetPoint = new System.Windows.Forms.TextBox();
            this.btn_StepUp = new System.Windows.Forms.Button();
            this.btn_StepDown = new System.Windows.Forms.Button();
            this.btn_setSetPoint = new System.Windows.Forms.Button();
            this.tb_newSetPoint = new System.Windows.Forms.TextBox();
            this.rb_StandBY = new System.Windows.Forms.RadioButton();
            this.rb_Measure = new System.Windows.Forms.RadioButton();
            this.rb_Control = new System.Windows.Forms.RadioButton();
            this.rb_Vent = new System.Windows.Forms.RadioButton();
            this.lbl_cpcStatus = new System.Windows.Forms.Label();
            this.lbl_cpcmean = new System.Windows.Forms.Label();
            this.lbl_ekmean = new System.Windows.Forms.Label();
            this.btn_copyMeansToClipboard = new System.Windows.Forms.Button();
            this.lbl_EKstdev = new System.Windows.Forms.Label();
            this.lbl_CPCstdev = new System.Windows.Forms.Label();
            this.lbl_EKLRSlope = new System.Windows.Forms.Label();
            this.lbl_CPCLRSlope = new System.Windows.Forms.Label();
            this.lbl_EKstability = new System.Windows.Forms.Label();
            this.lbl_CPCstability = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.percent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.calcPressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cpcPressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ekCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbScaleMin = new System.Windows.Forms.TextBox();
            this.tbScaleMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnStartAutoCal = new System.Windows.Forms.Button();
            this.cbPressureScaleUOM = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_PressureReaderGeneratorSpan = new System.Windows.Forms.ComboBox();
            this.cb_currentReaderSpan = new System.Windows.Forms.ComboBox();
            this.btn_ZeroPressure = new System.Windows.Forms.Button();
            this.cb_HART_SerialPort = new System.Windows.Forms.ComboBox();
            this.btn_ReadHART_Scale = new System.Windows.Forms.Button();
            this.btn_HART_ZEROTRIM = new System.Windows.Forms.Button();
            this.gb_HART = new System.Windows.Forms.GroupBox();
            this.btn_pressureMicroStepDown = new System.Windows.Forms.Button();
            this.btn_pressureMicrostepUP = new System.Windows.Forms.Button();
            this.tb_pressureMicroStep = new System.Windows.Forms.TextBox();
            this.chart_result = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.nUD_CalibrationCyclesCount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.chart_measures = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.gb_HART.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_result)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_CalibrationCyclesCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_measures)).BeginInit();
            this.SuspendLayout();
            // 
            // cb_CurrentMeasuringInstruments
            // 
            this.cb_CurrentMeasuringInstruments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_CurrentMeasuringInstruments.FormattingEnabled = true;
            this.cb_CurrentMeasuringInstruments.Location = new System.Drawing.Point(13, 13);
            this.cb_CurrentMeasuringInstruments.Name = "cb_CurrentMeasuringInstruments";
            this.cb_CurrentMeasuringInstruments.Size = new System.Drawing.Size(470, 21);
            this.cb_CurrentMeasuringInstruments.TabIndex = 0;
            // 
            // cb_CurrentInstrumentChannels
            // 
            this.cb_CurrentInstrumentChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_CurrentInstrumentChannels.FormattingEnabled = true;
            this.cb_CurrentInstrumentChannels.Location = new System.Drawing.Point(13, 40);
            this.cb_CurrentInstrumentChannels.Name = "cb_CurrentInstrumentChannels";
            this.cb_CurrentInstrumentChannels.Size = new System.Drawing.Size(470, 21);
            this.cb_CurrentInstrumentChannels.TabIndex = 2;
            this.cb_CurrentInstrumentChannels.SelectedIndexChanged += new System.EventHandler(this.cb_chanNUM_SelectedIndexChanged);
            // 
            // lbl_cnahValue
            // 
            this.lbl_cnahValue.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_cnahValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_cnahValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 65.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_cnahValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbl_cnahValue.Location = new System.Drawing.Point(12, 135);
            this.lbl_cnahValue.Name = "lbl_cnahValue";
            this.lbl_cnahValue.Size = new System.Drawing.Size(594, 108);
            this.lbl_cnahValue.TabIndex = 3;
            this.lbl_cnahValue.Text = "---.----";
            this.lbl_cnahValue.Click += new System.EventHandler(this.lbl_cnahValue_Click);
            // 
            // btn_openCurrentMeasureInstrument
            // 
            this.btn_openCurrentMeasureInstrument.Location = new System.Drawing.Point(491, 13);
            this.btn_openCurrentMeasureInstrument.Name = "btn_openCurrentMeasureInstrument";
            this.btn_openCurrentMeasureInstrument.Size = new System.Drawing.Size(115, 23);
            this.btn_openCurrentMeasureInstrument.TabIndex = 4;
            this.btn_openCurrentMeasureInstrument.Text = "Установить связь";
            this.btn_openCurrentMeasureInstrument.UseVisualStyleBackColor = true;
            this.btn_openCurrentMeasureInstrument.Click += new System.EventHandler(this.btn_openCurrentMeasureInstrument_Click);
            // 
            // cb_PressureGeneratorInstrument
            // 
            this.cb_PressureGeneratorInstrument.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_PressureGeneratorInstrument.FormattingEnabled = true;
            this.cb_PressureGeneratorInstrument.Location = new System.Drawing.Point(641, 13);
            this.cb_PressureGeneratorInstrument.Name = "cb_PressureGeneratorInstrument";
            this.cb_PressureGeneratorInstrument.Size = new System.Drawing.Size(468, 21);
            this.cb_PressureGeneratorInstrument.TabIndex = 5;
            // 
            // btn_openCPC
            // 
            this.btn_openCPC.Location = new System.Drawing.Point(1120, 13);
            this.btn_openCPC.Name = "btn_openCPC";
            this.btn_openCPC.Size = new System.Drawing.Size(115, 23);
            this.btn_openCPC.TabIndex = 6;
            this.btn_openCPC.Text = "Установить связь";
            this.btn_openCPC.UseVisualStyleBackColor = true;
            this.btn_openCPC.Click += new System.EventHandler(this.btn_openPressureMeasureInstrument_Click);
            // 
            // cb_cpcChannels
            // 
            this.cb_cpcChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_cpcChannels.Enabled = false;
            this.cb_cpcChannels.FormattingEnabled = true;
            this.cb_cpcChannels.Location = new System.Drawing.Point(641, 40);
            this.cb_cpcChannels.Name = "cb_cpcChannels";
            this.cb_cpcChannels.Size = new System.Drawing.Size(468, 21);
            this.cb_cpcChannels.TabIndex = 7;
            this.cb_cpcChannels.SelectedIndexChanged += new System.EventHandler(this.cb_cpcChannels_SelectedIndexChanged);
            // 
            // lbl_cpc_read
            // 
            this.lbl_cpc_read.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_cpc_read.Font = new System.Drawing.Font("Microsoft Sans Serif", 65.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_cpc_read.Location = new System.Drawing.Point(641, 135);
            this.lbl_cpc_read.Name = "lbl_cpc_read";
            this.lbl_cpc_read.Size = new System.Drawing.Size(594, 108);
            this.lbl_cpc_read.TabIndex = 3;
            this.lbl_cpc_read.Text = "---.----";
            // 
            // tb_cpcStep
            // 
            this.tb_cpcStep.Enabled = false;
            this.tb_cpcStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_cpcStep.Location = new System.Drawing.Point(771, 330);
            this.tb_cpcStep.Name = "tb_cpcStep";
            this.tb_cpcStep.Size = new System.Drawing.Size(120, 44);
            this.tb_cpcStep.TabIndex = 9;
            this.tb_cpcStep.Text = "1.0";
            this.tb_cpcStep.WordWrap = false;
            this.tb_cpcStep.TextChanged += new System.EventHandler(this.tb_cpcStep_TextChanged);
            // 
            // tb_PressureSetPoint
            // 
            this.tb_PressureSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_PressureSetPoint.Location = new System.Drawing.Point(898, 330);
            this.tb_PressureSetPoint.Name = "tb_PressureSetPoint";
            this.tb_PressureSetPoint.ReadOnly = true;
            this.tb_PressureSetPoint.Size = new System.Drawing.Size(158, 44);
            this.tb_PressureSetPoint.TabIndex = 10;
            this.tb_PressureSetPoint.Text = "0.0";
            this.tb_PressureSetPoint.WordWrap = false;
            this.tb_PressureSetPoint.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tb_PressureSetPoint_KeyDown);
            // 
            // btn_StepUp
            // 
            this.btn_StepUp.Enabled = false;
            this.btn_StepUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_StepUp.Location = new System.Drawing.Point(771, 280);
            this.btn_StepUp.Name = "btn_StepUp";
            this.btn_StepUp.Size = new System.Drawing.Size(120, 44);
            this.btn_StepUp.TabIndex = 11;
            this.btn_StepUp.Text = "+";
            this.btn_StepUp.UseVisualStyleBackColor = true;
            this.btn_StepUp.Click += new System.EventHandler(this.btn_StepUp_Click);
            // 
            // btn_StepDown
            // 
            this.btn_StepDown.Enabled = false;
            this.btn_StepDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_StepDown.Location = new System.Drawing.Point(771, 379);
            this.btn_StepDown.Name = "btn_StepDown";
            this.btn_StepDown.Size = new System.Drawing.Size(120, 44);
            this.btn_StepDown.TabIndex = 12;
            this.btn_StepDown.Text = "-";
            this.btn_StepDown.UseVisualStyleBackColor = true;
            this.btn_StepDown.Click += new System.EventHandler(this.btn_StepDown_Click);
            // 
            // btn_setSetPoint
            // 
            this.btn_setSetPoint.Enabled = false;
            this.btn_setSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_setSetPoint.Location = new System.Drawing.Point(1061, 330);
            this.btn_setSetPoint.Name = "btn_setSetPoint";
            this.btn_setSetPoint.Size = new System.Drawing.Size(174, 44);
            this.btn_setSetPoint.TabIndex = 13;
            this.btn_setSetPoint.Text = "New Set Point";
            this.btn_setSetPoint.UseVisualStyleBackColor = true;
            this.btn_setSetPoint.Click += new System.EventHandler(this.btn_setSetPoint_Click);
            // 
            // tb_newSetPoint
            // 
            this.tb_newSetPoint.Enabled = false;
            this.tb_newSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_newSetPoint.Location = new System.Drawing.Point(1061, 280);
            this.tb_newSetPoint.Name = "tb_newSetPoint";
            this.tb_newSetPoint.Size = new System.Drawing.Size(174, 44);
            this.tb_newSetPoint.TabIndex = 14;
            this.tb_newSetPoint.Text = "0";
            this.tb_newSetPoint.WordWrap = false;
            this.tb_newSetPoint.TextChanged += new System.EventHandler(this.tb_newSetPoint_TextChanged);
            this.tb_newSetPoint.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_newSetPoint_KeyPress);
            // 
            // rb_StandBY
            // 
            this.rb_StandBY.AutoSize = true;
            this.rb_StandBY.Location = new System.Drawing.Point(641, 246);
            this.rb_StandBY.Name = "rb_StandBY";
            this.rb_StandBY.Size = new System.Drawing.Size(76, 17);
            this.rb_StandBY.TabIndex = 15;
            this.rb_StandBY.TabStop = true;
            this.rb_StandBY.Text = "STANDBY";
            this.rb_StandBY.UseVisualStyleBackColor = true;
            this.rb_StandBY.CheckedChanged += new System.EventHandler(this.rb_StandBY_CheckedChanged);
            // 
            // rb_Measure
            // 
            this.rb_Measure.AutoSize = true;
            this.rb_Measure.Location = new System.Drawing.Point(723, 246);
            this.rb_Measure.Name = "rb_Measure";
            this.rb_Measure.Size = new System.Drawing.Size(78, 17);
            this.rb_Measure.TabIndex = 16;
            this.rb_Measure.TabStop = true;
            this.rb_Measure.Text = "MEASURE";
            this.rb_Measure.UseVisualStyleBackColor = true;
            this.rb_Measure.CheckedChanged += new System.EventHandler(this.rb_Measure_CheckedChanged);
            // 
            // rb_Control
            // 
            this.rb_Control.AutoSize = true;
            this.rb_Control.Location = new System.Drawing.Point(807, 246);
            this.rb_Control.Name = "rb_Control";
            this.rb_Control.Size = new System.Drawing.Size(77, 17);
            this.rb_Control.TabIndex = 17;
            this.rb_Control.TabStop = true;
            this.rb_Control.Text = "CONTROL";
            this.rb_Control.UseVisualStyleBackColor = true;
            this.rb_Control.CheckedChanged += new System.EventHandler(this.rb_Control_CheckedChanged);
            // 
            // rb_Vent
            // 
            this.rb_Vent.AutoSize = true;
            this.rb_Vent.Location = new System.Drawing.Point(890, 246);
            this.rb_Vent.Name = "rb_Vent";
            this.rb_Vent.Size = new System.Drawing.Size(54, 17);
            this.rb_Vent.TabIndex = 18;
            this.rb_Vent.TabStop = true;
            this.rb_Vent.Text = "VENT";
            this.rb_Vent.UseVisualStyleBackColor = true;
            this.rb_Vent.CheckedChanged += new System.EventHandler(this.rb_Vent_CheckedChanged);
            // 
            // lbl_cpcStatus
            // 
            this.lbl_cpcStatus.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_cpcStatus.Location = new System.Drawing.Point(637, 112);
            this.lbl_cpcStatus.Name = "lbl_cpcStatus";
            this.lbl_cpcStatus.Size = new System.Drawing.Size(598, 23);
            this.lbl_cpcStatus.TabIndex = 19;
            this.lbl_cpcStatus.Text = "нет связи";
            // 
            // lbl_cpcmean
            // 
            this.lbl_cpcmean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_cpcmean.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_cpcmean.Location = new System.Drawing.Point(111, 563);
            this.lbl_cpcmean.Name = "lbl_cpcmean";
            this.lbl_cpcmean.Size = new System.Drawing.Size(92, 23);
            this.lbl_cpcmean.TabIndex = 20;
            this.lbl_cpcmean.Text = "--------";
            // 
            // lbl_ekmean
            // 
            this.lbl_ekmean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ekmean.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_ekmean.Location = new System.Drawing.Point(13, 563);
            this.lbl_ekmean.Name = "lbl_ekmean";
            this.lbl_ekmean.Size = new System.Drawing.Size(92, 23);
            this.lbl_ekmean.TabIndex = 21;
            this.lbl_ekmean.Text = "--------";
            // 
            // btn_copyMeansToClipboard
            // 
            this.btn_copyMeansToClipboard.Location = new System.Drawing.Point(209, 563);
            this.btn_copyMeansToClipboard.Name = "btn_copyMeansToClipboard";
            this.btn_copyMeansToClipboard.Size = new System.Drawing.Size(45, 23);
            this.btn_copyMeansToClipboard.TabIndex = 22;
            this.btn_copyMeansToClipboard.Text = "copy";
            this.btn_copyMeansToClipboard.UseVisualStyleBackColor = true;
            this.btn_copyMeansToClipboard.Click += new System.EventHandler(this.btn_copyMeansToClipboard_Click);
            // 
            // lbl_EKstdev
            // 
            this.lbl_EKstdev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EKstdev.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_EKstdev.Location = new System.Drawing.Point(13, 592);
            this.lbl_EKstdev.Name = "lbl_EKstdev";
            this.lbl_EKstdev.Size = new System.Drawing.Size(92, 23);
            this.lbl_EKstdev.TabIndex = 23;
            this.lbl_EKstdev.Text = "--------";
            // 
            // lbl_CPCstdev
            // 
            this.lbl_CPCstdev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CPCstdev.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CPCstdev.Location = new System.Drawing.Point(111, 592);
            this.lbl_CPCstdev.Name = "lbl_CPCstdev";
            this.lbl_CPCstdev.Size = new System.Drawing.Size(92, 23);
            this.lbl_CPCstdev.TabIndex = 24;
            this.lbl_CPCstdev.Text = "--------";
            // 
            // lbl_EKLRSlope
            // 
            this.lbl_EKLRSlope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EKLRSlope.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_EKLRSlope.Location = new System.Drawing.Point(13, 621);
            this.lbl_EKLRSlope.Name = "lbl_EKLRSlope";
            this.lbl_EKLRSlope.Size = new System.Drawing.Size(92, 23);
            this.lbl_EKLRSlope.TabIndex = 25;
            this.lbl_EKLRSlope.Text = "--------";
            // 
            // lbl_CPCLRSlope
            // 
            this.lbl_CPCLRSlope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CPCLRSlope.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CPCLRSlope.Location = new System.Drawing.Point(111, 621);
            this.lbl_CPCLRSlope.Name = "lbl_CPCLRSlope";
            this.lbl_CPCLRSlope.Size = new System.Drawing.Size(92, 23);
            this.lbl_CPCLRSlope.TabIndex = 26;
            this.lbl_CPCLRSlope.Text = "--------";
            // 
            // lbl_EKstability
            // 
            this.lbl_EKstability.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EKstability.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_EKstability.Location = new System.Drawing.Point(13, 650);
            this.lbl_EKstability.Name = "lbl_EKstability";
            this.lbl_EKstability.Size = new System.Drawing.Size(92, 23);
            this.lbl_EKstability.TabIndex = 27;
            this.lbl_EKstability.Text = "--------";
            // 
            // lbl_CPCstability
            // 
            this.lbl_CPCstability.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CPCstability.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CPCstability.Location = new System.Drawing.Point(111, 650);
            this.lbl_CPCstability.Name = "lbl_CPCstability";
            this.lbl_CPCstability.Size = new System.Drawing.Size(92, 23);
            this.lbl_CPCstability.TabIndex = 28;
            this.lbl_CPCstability.Text = "--------";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.percent,
            this.calcPressure,
            this.cpcPressure,
            this.ekCurrent,
            this.error});
            this.dataGridView1.Location = new System.Drawing.Point(13, 349);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(593, 201);
            this.dataGridView1.TabIndex = 32;
            // 
            // percent
            // 
            this.percent.HeaderText = "%шкалы";
            this.percent.MinimumWidth = 6;
            this.percent.Name = "percent";
            this.percent.ReadOnly = true;
            this.percent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.percent.Width = 60;
            // 
            // calcPressure
            // 
            this.calcPressure.HeaderText = "Расчетное давление";
            this.calcPressure.MinimumWidth = 6;
            this.calcPressure.Name = "calcPressure";
            this.calcPressure.ReadOnly = true;
            // 
            // cpcPressure
            // 
            this.cpcPressure.HeaderText = "Действительное давление";
            this.cpcPressure.MinimumWidth = 6;
            this.cpcPressure.Name = "cpcPressure";
            this.cpcPressure.ReadOnly = true;
            this.cpcPressure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ekCurrent
            // 
            this.ekCurrent.HeaderText = "ток";
            this.ekCurrent.MinimumWidth = 6;
            this.ekCurrent.Name = "ekCurrent";
            this.ekCurrent.ReadOnly = true;
            this.ekCurrent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // error
            // 
            this.error.HeaderText = "погрешность, %";
            this.error.MinimumWidth = 6;
            this.error.Name = "error";
            this.error.ReadOnly = true;
            // 
            // tbScaleMin
            // 
            this.tbScaleMin.Location = new System.Drawing.Point(13, 272);
            this.tbScaleMin.Name = "tbScaleMin";
            this.tbScaleMin.Size = new System.Drawing.Size(100, 20);
            this.tbScaleMin.TabIndex = 29;
            this.tbScaleMin.TextChanged += new System.EventHandler(this.tbScaleMin_TextChanged);
            // 
            // tbScaleMax
            // 
            this.tbScaleMax.Location = new System.Drawing.Point(119, 272);
            this.tbScaleMax.Name = "tbScaleMax";
            this.tbScaleMax.Size = new System.Drawing.Size(100, 20);
            this.tbScaleMax.TabIndex = 30;
            this.tbScaleMax.TextChanged += new System.EventHandler(this.tbScaleMax_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 256);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "шкала мин.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(116, 256);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 34;
            this.label2.Text = "шкала макс.";
            // 
            // btnStartAutoCal
            // 
            this.btnStartAutoCal.Location = new System.Drawing.Point(359, 270);
            this.btnStartAutoCal.Name = "btnStartAutoCal";
            this.btnStartAutoCal.Size = new System.Drawing.Size(75, 23);
            this.btnStartAutoCal.TabIndex = 31;
            this.btnStartAutoCal.Text = "Старт";
            this.btnStartAutoCal.UseVisualStyleBackColor = true;
            this.btnStartAutoCal.Click += new System.EventHandler(this.btnStartAutoCal_Click);
            // 
            // cbPressureScaleUOM
            // 
            this.cbPressureScaleUOM.FormattingEnabled = true;
            this.cbPressureScaleUOM.Location = new System.Drawing.Point(226, 271);
            this.cbPressureScaleUOM.Name = "cbPressureScaleUOM";
            this.cbPressureScaleUOM.Size = new System.Drawing.Size(121, 21);
            this.cbPressureScaleUOM.TabIndex = 35;
            this.cbPressureScaleUOM.SelectedIndexChanged += new System.EventHandler(this.cbScaleUOM_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(223, 255);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "ед. изм.";
            // 
            // cb_PressureReaderGeneratorSpan
            // 
            this.cb_PressureReaderGeneratorSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_PressureReaderGeneratorSpan.Enabled = false;
            this.cb_PressureReaderGeneratorSpan.FormattingEnabled = true;
            this.cb_PressureReaderGeneratorSpan.Location = new System.Drawing.Point(640, 67);
            this.cb_PressureReaderGeneratorSpan.Name = "cb_PressureReaderGeneratorSpan";
            this.cb_PressureReaderGeneratorSpan.Size = new System.Drawing.Size(468, 21);
            this.cb_PressureReaderGeneratorSpan.TabIndex = 36;
            this.cb_PressureReaderGeneratorSpan.SelectedIndexChanged += new System.EventHandler(this.cb_PressureReaderGeneratorSpan_SelectedIndexChanged);
            // 
            // cb_currentReaderSpan
            // 
            this.cb_currentReaderSpan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_currentReaderSpan.Enabled = false;
            this.cb_currentReaderSpan.FormattingEnabled = true;
            this.cb_currentReaderSpan.Location = new System.Drawing.Point(13, 67);
            this.cb_currentReaderSpan.Name = "cb_currentReaderSpan";
            this.cb_currentReaderSpan.Size = new System.Drawing.Size(470, 21);
            this.cb_currentReaderSpan.TabIndex = 37;
            this.cb_currentReaderSpan.SelectedIndexChanged += new System.EventHandler(this.cb_currentReaderSpan_SelectedIndexChanged);
            // 
            // btn_ZeroPressure
            // 
            this.btn_ZeroPressure.Enabled = false;
            this.btn_ZeroPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.btn_ZeroPressure.Location = new System.Drawing.Point(1190, 247);
            this.btn_ZeroPressure.Name = "btn_ZeroPressure";
            this.btn_ZeroPressure.Size = new System.Drawing.Size(47, 27);
            this.btn_ZeroPressure.TabIndex = 38;
            this.btn_ZeroPressure.Text = ">0<";
            this.btn_ZeroPressure.UseVisualStyleBackColor = true;
            this.btn_ZeroPressure.Click += new System.EventHandler(this.btn_ZeroPressure_Click);
            // 
            // cb_HART_SerialPort
            // 
            this.cb_HART_SerialPort.FormattingEnabled = true;
            this.cb_HART_SerialPort.Location = new System.Drawing.Point(6, 15);
            this.cb_HART_SerialPort.Name = "cb_HART_SerialPort";
            this.cb_HART_SerialPort.Size = new System.Drawing.Size(121, 21);
            this.cb_HART_SerialPort.TabIndex = 39;
            // 
            // btn_ReadHART_Scale
            // 
            this.btn_ReadHART_Scale.Location = new System.Drawing.Point(154, 13);
            this.btn_ReadHART_Scale.Name = "btn_ReadHART_Scale";
            this.btn_ReadHART_Scale.Size = new System.Drawing.Size(90, 23);
            this.btn_ReadHART_Scale.TabIndex = 40;
            this.btn_ReadHART_Scale.Text = "считать шкалу";
            this.btn_ReadHART_Scale.UseVisualStyleBackColor = true;
            this.btn_ReadHART_Scale.Click += new System.EventHandler(this.btn_ReadHART_Scale_Click);
            // 
            // btn_HART_ZEROTRIM
            // 
            this.btn_HART_ZEROTRIM.Location = new System.Drawing.Point(273, 13);
            this.btn_HART_ZEROTRIM.Name = "btn_HART_ZEROTRIM";
            this.btn_HART_ZEROTRIM.Size = new System.Drawing.Size(34, 23);
            this.btn_HART_ZEROTRIM.TabIndex = 41;
            this.btn_HART_ZEROTRIM.Text = ">0<";
            this.btn_HART_ZEROTRIM.UseVisualStyleBackColor = true;
            this.btn_HART_ZEROTRIM.Click += new System.EventHandler(this.btn_HART_ZEROTRIM_Click);
            // 
            // gb_HART
            // 
            this.gb_HART.Controls.Add(this.btn_HART_ZEROTRIM);
            this.gb_HART.Controls.Add(this.btn_ReadHART_Scale);
            this.gb_HART.Controls.Add(this.cb_HART_SerialPort);
            this.gb_HART.Location = new System.Drawing.Point(12, 298);
            this.gb_HART.Name = "gb_HART";
            this.gb_HART.Size = new System.Drawing.Size(594, 43);
            this.gb_HART.TabIndex = 42;
            this.gb_HART.TabStop = false;
            this.gb_HART.Text = "HART";
            // 
            // btn_pressureMicroStepDown
            // 
            this.btn_pressureMicroStepDown.Enabled = false;
            this.btn_pressureMicroStepDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_pressureMicroStepDown.Location = new System.Drawing.Point(641, 379);
            this.btn_pressureMicroStepDown.Name = "btn_pressureMicroStepDown";
            this.btn_pressureMicroStepDown.Size = new System.Drawing.Size(120, 44);
            this.btn_pressureMicroStepDown.TabIndex = 45;
            this.btn_pressureMicroStepDown.Text = "-";
            this.btn_pressureMicroStepDown.UseVisualStyleBackColor = true;
            this.btn_pressureMicroStepDown.Click += new System.EventHandler(this.btn_pressureMicroStepDown_Click);
            // 
            // btn_pressureMicrostepUP
            // 
            this.btn_pressureMicrostepUP.Enabled = false;
            this.btn_pressureMicrostepUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_pressureMicrostepUP.Location = new System.Drawing.Point(641, 280);
            this.btn_pressureMicrostepUP.Name = "btn_pressureMicrostepUP";
            this.btn_pressureMicrostepUP.Size = new System.Drawing.Size(120, 44);
            this.btn_pressureMicrostepUP.TabIndex = 44;
            this.btn_pressureMicrostepUP.Text = "+";
            this.btn_pressureMicrostepUP.UseVisualStyleBackColor = true;
            this.btn_pressureMicrostepUP.Click += new System.EventHandler(this.btn_pressureMicrostepUP_Click);
            // 
            // tb_pressureMicroStep
            // 
            this.tb_pressureMicroStep.Enabled = false;
            this.tb_pressureMicroStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_pressureMicroStep.Location = new System.Drawing.Point(641, 330);
            this.tb_pressureMicroStep.Name = "tb_pressureMicroStep";
            this.tb_pressureMicroStep.Size = new System.Drawing.Size(120, 44);
            this.tb_pressureMicroStep.TabIndex = 43;
            this.tb_pressureMicroStep.Text = "1.0";
            this.tb_pressureMicroStep.WordWrap = false;
            this.tb_pressureMicroStep.TextChanged += new System.EventHandler(this.tb_pressureMicroStep_TextChanged);
            // 
            // chart_result
            // 
            this.chart_result.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.chart_result.ChartAreas.Add(chartArea1);
            this.chart_result.Location = new System.Drawing.Point(642, 444);
            this.chart_result.Name = "chart_result";
            series1.ChartArea = "ChartArea1";
            series1.Name = "Series1";
            this.chart_result.Series.Add(series1);
            this.chart_result.Size = new System.Drawing.Size(592, 228);
            this.chart_result.TabIndex = 46;
            this.chart_result.Text = "chart1";
            // 
            // nUD_CalibrationCyclesCount
            // 
            this.nUD_CalibrationCyclesCount.Location = new System.Drawing.Point(451, 271);
            this.nUD_CalibrationCyclesCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nUD_CalibrationCyclesCount.Name = "nUD_CalibrationCyclesCount";
            this.nUD_CalibrationCyclesCount.Size = new System.Drawing.Size(120, 20);
            this.nUD_CalibrationCyclesCount.TabIndex = 47;
            this.nUD_CalibrationCyclesCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(455, 257);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 48;
            this.label4.Text = "количество циклов";
            // 
            // chart_measures
            // 
            this.chart_measures.BackColor = System.Drawing.Color.Transparent;
            chartArea2.Name = "ChartArea1";
            this.chart_measures.ChartAreas.Add(chartArea2);
            this.chart_measures.Location = new System.Drawing.Point(260, 563);
            this.chart_measures.Name = "chart_measures";
            series2.ChartArea = "ChartArea1";
            series2.Name = "Series1";
            this.chart_measures.Series.Add(series2);
            this.chart_measures.Size = new System.Drawing.Size(346, 109);
            this.chart_measures.TabIndex = 49;
            this.chart_measures.Text = "chart1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(1252, 687);
            this.Controls.Add(this.chart_measures);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nUD_CalibrationCyclesCount);
            this.Controls.Add(this.chart_result);
            this.Controls.Add(this.btn_pressureMicroStepDown);
            this.Controls.Add(this.btn_pressureMicrostepUP);
            this.Controls.Add(this.tb_pressureMicroStep);
            this.Controls.Add(this.gb_HART);
            this.Controls.Add(this.btn_ZeroPressure);
            this.Controls.Add(this.cb_currentReaderSpan);
            this.Controls.Add(this.cb_PressureReaderGeneratorSpan);
            this.Controls.Add(this.cbPressureScaleUOM);
            this.Controls.Add(this.btnStartAutoCal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbScaleMax);
            this.Controls.Add(this.tbScaleMin);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btn_copyMeansToClipboard);
            this.Controls.Add(this.lbl_EKstability);
            this.Controls.Add(this.lbl_EKLRSlope);
            this.Controls.Add(this.lbl_EKstdev);
            this.Controls.Add(this.lbl_ekmean);
            this.Controls.Add(this.lbl_CPCstability);
            this.Controls.Add(this.lbl_CPCLRSlope);
            this.Controls.Add(this.lbl_CPCstdev);
            this.Controls.Add(this.lbl_cpcmean);
            this.Controls.Add(this.lbl_cpcStatus);
            this.Controls.Add(this.rb_Vent);
            this.Controls.Add(this.rb_Control);
            this.Controls.Add(this.rb_Measure);
            this.Controls.Add(this.rb_StandBY);
            this.Controls.Add(this.btn_StepDown);
            this.Controls.Add(this.btn_setSetPoint);
            this.Controls.Add(this.btn_StepUp);
            this.Controls.Add(this.tb_PressureSetPoint);
            this.Controls.Add(this.tb_newSetPoint);
            this.Controls.Add(this.tb_cpcStep);
            this.Controls.Add(this.cb_cpcChannels);
            this.Controls.Add(this.btn_openCPC);
            this.Controls.Add(this.cb_PressureGeneratorInstrument);
            this.Controls.Add(this.btn_openCurrentMeasureInstrument);
            this.Controls.Add(this.lbl_cpc_read);
            this.Controls.Add(this.lbl_cnahValue);
            this.Controls.Add(this.cb_CurrentInstrumentChannels);
            this.Controls.Add(this.cb_CurrentMeasuringInstruments);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Калибровка СИ давления";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.gb_HART.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_result)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nUD_CalibrationCyclesCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart_measures)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_CurrentMeasuringInstruments;
        private System.Windows.Forms.ComboBox cb_CurrentInstrumentChannels;
        private System.Windows.Forms.Label lbl_cnahValue;
        private System.Windows.Forms.Button btn_openCurrentMeasureInstrument;
        private System.Windows.Forms.ComboBox cb_PressureGeneratorInstrument;
        private System.Windows.Forms.Button btn_openCPC;
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
        private System.Windows.Forms.Label lbl_cpcmean;
        private System.Windows.Forms.Label lbl_ekmean;
        private System.Windows.Forms.Button btn_copyMeansToClipboard;
        private System.Windows.Forms.Label lbl_EKstdev;
        private System.Windows.Forms.Label lbl_CPCstdev;
        private System.Windows.Forms.Label lbl_EKLRSlope;
        private System.Windows.Forms.Label lbl_CPCLRSlope;
        private System.Windows.Forms.Label lbl_EKstability;
        private System.Windows.Forms.Label lbl_CPCstability;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn percent;
        private System.Windows.Forms.DataGridViewTextBoxColumn calcPressure;
        private System.Windows.Forms.DataGridViewTextBoxColumn cpcPressure;
        private System.Windows.Forms.DataGridViewTextBoxColumn ekCurrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_result;
        private System.Windows.Forms.NumericUpDown nUD_CalibrationCyclesCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_measures;
    }
}

