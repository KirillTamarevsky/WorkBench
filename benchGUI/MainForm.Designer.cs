
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
            lbl_cpcmean = new System.Windows.Forms.Label();
            lbl_ekmean = new System.Windows.Forms.Label();
            btn_copyMeansToClipboard = new System.Windows.Forms.Button();
            lbl_EKstdev = new System.Windows.Forms.Label();
            lbl_CPCstdev = new System.Windows.Forms.Label();
            lbl_EKLRSlope = new System.Windows.Forms.Label();
            lbl_CPCLRSlope = new System.Windows.Forms.Label();
            lbl_EKstability = new System.Windows.Forms.Label();
            lbl_CPCstability = new System.Windows.Forms.Label();
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
            btn_pressureMicroStepDown = new System.Windows.Forms.Button();
            btn_pressureMicrostepUP = new System.Windows.Forms.Button();
            tb_pressureMicroStep = new System.Windows.Forms.TextBox();
            nUD_CalibrationCyclesCount = new System.Windows.Forms.NumericUpDown();
            label4 = new System.Windows.Forms.Label();
            plot_result = new ScottPlot.FormsPlot();
            plot_measures = new ScottPlot.FormsPlot();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            gb_HART.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nUD_CalibrationCyclesCount).BeginInit();
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
            lbl_cnahValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 65.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            lbl_cpc_read.Font = new System.Drawing.Font("Microsoft Sans Serif", 65.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
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
            tb_cpcStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            tb_PressureSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            btn_StepUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            btn_StepDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            btn_setSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            tb_newSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            lbl_cpcStatus.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_cpcStatus.Location = new System.Drawing.Point(624, 102);
            lbl_cpcStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_cpcStatus.Name = "lbl_cpcStatus";
            lbl_cpcStatus.Size = new System.Drawing.Size(602, 27);
            lbl_cpcStatus.TabIndex = 19;
            lbl_cpcStatus.Text = "нет связи";
            // 
            // lbl_cpcmean
            // 
            lbl_cpcmean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_cpcmean.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_cpcmean.Location = new System.Drawing.Point(102, 650);
            lbl_cpcmean.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_cpcmean.Name = "lbl_cpcmean";
            lbl_cpcmean.Size = new System.Drawing.Size(80, 26);
            lbl_cpcmean.TabIndex = 20;
            lbl_cpcmean.Text = "--------";
            // 
            // lbl_ekmean
            // 
            lbl_ekmean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_ekmean.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_ekmean.Location = new System.Drawing.Point(15, 650);
            lbl_ekmean.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_ekmean.Name = "lbl_ekmean";
            lbl_ekmean.Size = new System.Drawing.Size(80, 26);
            lbl_ekmean.TabIndex = 21;
            lbl_ekmean.Text = "--------";
            // 
            // btn_copyMeansToClipboard
            // 
            btn_copyMeansToClipboard.Location = new System.Drawing.Point(187, 650);
            btn_copyMeansToClipboard.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_copyMeansToClipboard.Name = "btn_copyMeansToClipboard";
            btn_copyMeansToClipboard.Size = new System.Drawing.Size(52, 27);
            btn_copyMeansToClipboard.TabIndex = 22;
            btn_copyMeansToClipboard.Text = "copy";
            btn_copyMeansToClipboard.UseVisualStyleBackColor = true;
            btn_copyMeansToClipboard.Click += btn_copyMeansToClipboard_Click;
            // 
            // lbl_EKstdev
            // 
            lbl_EKstdev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_EKstdev.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_EKstdev.Location = new System.Drawing.Point(15, 683);
            lbl_EKstdev.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_EKstdev.Name = "lbl_EKstdev";
            lbl_EKstdev.Size = new System.Drawing.Size(80, 26);
            lbl_EKstdev.TabIndex = 23;
            lbl_EKstdev.Text = "--------";
            // 
            // lbl_CPCstdev
            // 
            lbl_CPCstdev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_CPCstdev.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_CPCstdev.Location = new System.Drawing.Point(102, 683);
            lbl_CPCstdev.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_CPCstdev.Name = "lbl_CPCstdev";
            lbl_CPCstdev.Size = new System.Drawing.Size(80, 26);
            lbl_CPCstdev.TabIndex = 24;
            lbl_CPCstdev.Text = "--------";
            // 
            // lbl_EKLRSlope
            // 
            lbl_EKLRSlope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_EKLRSlope.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_EKLRSlope.Location = new System.Drawing.Point(15, 717);
            lbl_EKLRSlope.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_EKLRSlope.Name = "lbl_EKLRSlope";
            lbl_EKLRSlope.Size = new System.Drawing.Size(80, 26);
            lbl_EKLRSlope.TabIndex = 25;
            lbl_EKLRSlope.Text = "--------";
            // 
            // lbl_CPCLRSlope
            // 
            lbl_CPCLRSlope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_CPCLRSlope.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_CPCLRSlope.Location = new System.Drawing.Point(102, 717);
            lbl_CPCLRSlope.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_CPCLRSlope.Name = "lbl_CPCLRSlope";
            lbl_CPCLRSlope.Size = new System.Drawing.Size(80, 26);
            lbl_CPCLRSlope.TabIndex = 26;
            lbl_CPCLRSlope.Text = "--------";
            // 
            // lbl_EKstability
            // 
            lbl_EKstability.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_EKstability.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_EKstability.Location = new System.Drawing.Point(15, 750);
            lbl_EKstability.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_EKstability.Name = "lbl_EKstability";
            lbl_EKstability.Size = new System.Drawing.Size(80, 26);
            lbl_EKstability.TabIndex = 27;
            lbl_EKstability.Text = "--------";
            // 
            // lbl_CPCstability
            // 
            lbl_CPCstability.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lbl_CPCstability.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lbl_CPCstability.Location = new System.Drawing.Point(102, 750);
            lbl_CPCstability.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lbl_CPCstability.Name = "lbl_CPCstability";
            lbl_CPCstability.Size = new System.Drawing.Size(80, 26);
            lbl_CPCstability.TabIndex = 28;
            lbl_CPCstability.Text = "--------";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { percent, calcPressure, cpcPressure, ekCurrent, error });
            dataGridView1.Location = new System.Drawing.Point(15, 366);
            dataGridView1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new System.Drawing.Size(599, 275);
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
            // 
            // tbScaleMin
            // 
            tbScaleMin.Location = new System.Drawing.Point(15, 281);
            tbScaleMin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbScaleMin.Name = "tbScaleMin";
            tbScaleMin.Size = new System.Drawing.Size(107, 23);
            tbScaleMin.TabIndex = 29;
            tbScaleMin.TextChanged += tbScaleMin_TextChanged;
            // 
            // tbScaleMax
            // 
            tbScaleMax.Location = new System.Drawing.Point(130, 280);
            tbScaleMax.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            tbScaleMax.Name = "tbScaleMax";
            tbScaleMax.Size = new System.Drawing.Size(98, 23);
            tbScaleMax.TabIndex = 30;
            tbScaleMax.TextChanged += tbScaleMax_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 262);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(72, 15);
            label1.TabIndex = 33;
            label1.Text = "шкала мин.";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(132, 262);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(76, 15);
            label2.TabIndex = 34;
            label2.Text = "шкала макс.";
            // 
            // btnStartAutoCal
            // 
            btnStartAutoCal.Location = new System.Drawing.Point(469, 309);
            btnStartAutoCal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStartAutoCal.Name = "btnStartAutoCal";
            btnStartAutoCal.Size = new System.Drawing.Size(135, 27);
            btnStartAutoCal.TabIndex = 31;
            btnStartAutoCal.Text = "Старт";
            btnStartAutoCal.UseVisualStyleBackColor = true;
            btnStartAutoCal.Click += btnStartAutoCal_Click;
            // 
            // cbPressureScaleUOM
            // 
            cbPressureScaleUOM.FormattingEnabled = true;
            cbPressureScaleUOM.Location = new System.Drawing.Point(236, 280);
            cbPressureScaleUOM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            cbPressureScaleUOM.Name = "cbPressureScaleUOM";
            cbPressureScaleUOM.Size = new System.Drawing.Size(119, 23);
            cbPressureScaleUOM.TabIndex = 35;
            cbPressureScaleUOM.SelectedIndexChanged += cbScaleUOM_SelectedIndexChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(236, 261);
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
            btn_ZeroPressure.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btn_ZeroPressure.Location = new System.Drawing.Point(1388, 285);
            btn_ZeroPressure.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_ZeroPressure.Name = "btn_ZeroPressure";
            btn_ZeroPressure.Size = new System.Drawing.Size(55, 31);
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
            btn_ReadHART_Scale.Location = new System.Drawing.Point(112, 15);
            btn_ReadHART_Scale.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_ReadHART_Scale.Name = "btn_ReadHART_Scale";
            btn_ReadHART_Scale.Size = new System.Drawing.Size(105, 27);
            btn_ReadHART_Scale.TabIndex = 40;
            btn_ReadHART_Scale.Text = "считать шкалу";
            btn_ReadHART_Scale.UseVisualStyleBackColor = true;
            btn_ReadHART_Scale.Click += btn_ReadHART_Scale_Click;
            // 
            // btn_HART_ZEROTRIM
            // 
            btn_HART_ZEROTRIM.Location = new System.Drawing.Point(226, 15);
            btn_HART_ZEROTRIM.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btn_HART_ZEROTRIM.Name = "btn_HART_ZEROTRIM";
            btn_HART_ZEROTRIM.Size = new System.Drawing.Size(40, 27);
            btn_HART_ZEROTRIM.TabIndex = 41;
            btn_HART_ZEROTRIM.Text = ">0<";
            btn_HART_ZEROTRIM.UseVisualStyleBackColor = true;
            btn_HART_ZEROTRIM.Click += btn_HART_ZEROTRIM_Click;
            // 
            // gb_HART
            // 
            gb_HART.Controls.Add(btn_HART_ZEROTRIM);
            gb_HART.Controls.Add(btn_ReadHART_Scale);
            gb_HART.Controls.Add(cb_HART_SerialPort);
            gb_HART.Location = new System.Drawing.Point(14, 311);
            gb_HART.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gb_HART.Name = "gb_HART";
            gb_HART.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            gb_HART.Size = new System.Drawing.Size(447, 50);
            gb_HART.TabIndex = 42;
            gb_HART.TabStop = false;
            gb_HART.Text = "HART";
            // 
            // btn_pressureMicroStepDown
            // 
            btn_pressureMicroStepDown.Enabled = false;
            btn_pressureMicroStepDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            btn_pressureMicrostepUP.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            tb_pressureMicroStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
            nUD_CalibrationCyclesCount.Location = new System.Drawing.Point(469, 280);
            nUD_CalibrationCyclesCount.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            nUD_CalibrationCyclesCount.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nUD_CalibrationCyclesCount.Name = "nUD_CalibrationCyclesCount";
            nUD_CalibrationCyclesCount.Size = new System.Drawing.Size(135, 23);
            nUD_CalibrationCyclesCount.TabIndex = 47;
            nUD_CalibrationCyclesCount.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(479, 264);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(114, 15);
            label4.TabIndex = 48;
            label4.Text = "количество циклов";
            // 
            // plot_result
            // 
            plot_result.Location = new System.Drawing.Point(622, 434);
            plot_result.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            plot_result.Name = "plot_result";
            plot_result.Size = new System.Drawing.Size(602, 349);
            plot_result.TabIndex = 49;
            // 
            // plot_measures
            // 
            plot_measures.Location = new System.Drawing.Point(246, 643);
            plot_measures.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            plot_measures.Name = "plot_measures";
            plot_measures.Size = new System.Drawing.Size(368, 133);
            plot_measures.TabIndex = 50;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            ClientSize = new System.Drawing.Size(1236, 781);
            Controls.Add(plot_measures);
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
            Controls.Add(btn_copyMeansToClipboard);
            Controls.Add(lbl_EKstability);
            Controls.Add(lbl_EKLRSlope);
            Controls.Add(lbl_EKstdev);
            Controls.Add(lbl_ekmean);
            Controls.Add(lbl_CPCstability);
            Controls.Add(lbl_CPCLRSlope);
            Controls.Add(lbl_CPCstdev);
            Controls.Add(lbl_cpcmean);
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
            ((System.ComponentModel.ISupportInitialize)nUD_CalibrationCyclesCount).EndInit();
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
        private System.Windows.Forms.NumericUpDown nUD_CalibrationCyclesCount;
        private System.Windows.Forms.Label label4;
        private ScottPlot.FormsPlot plot_result;
        private ScottPlot.FormsPlot plot_measures;
    }
}

