
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
            this.cb_SP = new System.Windows.Forms.ComboBox();
            this.cb_chanNUM = new System.Windows.Forms.ComboBox();
            this.lbl_cnahValue = new System.Windows.Forms.Label();
            this.btn_openEK = new System.Windows.Forms.Button();
            this.cb_cpc = new System.Windows.Forms.ComboBox();
            this.btn_openCPC = new System.Windows.Forms.Button();
            this.cb_cpcChannels = new System.Windows.Forms.ComboBox();
            this.lbl_cpc_read = new System.Windows.Forms.Label();
            this.tb_cpcStep = new System.Windows.Forms.TextBox();
            this.tb_cpcSetPoint = new System.Windows.Forms.TextBox();
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
            this.cbScaleUOM = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // cb_SP
            // 
            this.cb_SP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SP.FormattingEnabled = true;
            this.cb_SP.Location = new System.Drawing.Point(13, 13);
            this.cb_SP.Name = "cb_SP";
            this.cb_SP.Size = new System.Drawing.Size(470, 21);
            this.cb_SP.Sorted = true;
            this.cb_SP.TabIndex = 0;
            // 
            // cb_chanNUM
            // 
            this.cb_chanNUM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_chanNUM.FormattingEnabled = true;
            this.cb_chanNUM.Location = new System.Drawing.Point(13, 40);
            this.cb_chanNUM.Name = "cb_chanNUM";
            this.cb_chanNUM.Size = new System.Drawing.Size(470, 21);
            this.cb_chanNUM.TabIndex = 2;
            this.cb_chanNUM.SelectedIndexChanged += new System.EventHandler(this.cb_chanNUM_SelectedIndexChanged);
            // 
            // lbl_cnahValue
            // 
            this.lbl_cnahValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_cnahValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_cnahValue.Location = new System.Drawing.Point(12, 135);
            this.lbl_cnahValue.Name = "lbl_cnahValue";
            this.lbl_cnahValue.Size = new System.Drawing.Size(594, 108);
            this.lbl_cnahValue.TabIndex = 3;
            this.lbl_cnahValue.Text = "---.----";
            // 
            // btn_openEK
            // 
            this.btn_openEK.Location = new System.Drawing.Point(491, 13);
            this.btn_openEK.Name = "btn_openEK";
            this.btn_openEK.Size = new System.Drawing.Size(115, 23);
            this.btn_openEK.TabIndex = 4;
            this.btn_openEK.Text = "Установить связь";
            this.btn_openEK.UseVisualStyleBackColor = true;
            this.btn_openEK.Click += new System.EventHandler(this.btn_openEK_Click);
            // 
            // cb_cpc
            // 
            this.cb_cpc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_cpc.FormattingEnabled = true;
            this.cb_cpc.Location = new System.Drawing.Point(641, 13);
            this.cb_cpc.Name = "cb_cpc";
            this.cb_cpc.Size = new System.Drawing.Size(468, 21);
            this.cb_cpc.TabIndex = 5;
            // 
            // btn_openCPC
            // 
            this.btn_openCPC.Location = new System.Drawing.Point(1120, 13);
            this.btn_openCPC.Name = "btn_openCPC";
            this.btn_openCPC.Size = new System.Drawing.Size(115, 23);
            this.btn_openCPC.TabIndex = 6;
            this.btn_openCPC.Text = "Установить связь";
            this.btn_openCPC.UseVisualStyleBackColor = true;
            this.btn_openCPC.Click += new System.EventHandler(this.btn_openCPC_Click);
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
            this.lbl_cpc_read.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
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
            this.tb_cpcStep.Location = new System.Drawing.Point(640, 322);
            this.tb_cpcStep.Name = "tb_cpcStep";
            this.tb_cpcStep.Size = new System.Drawing.Size(174, 44);
            this.tb_cpcStep.TabIndex = 9;
            this.tb_cpcStep.Text = "1.0";
            this.tb_cpcStep.WordWrap = false;
            this.tb_cpcStep.TextChanged += new System.EventHandler(this.tb_cpcStep_TextChanged);
            // 
            // tb_cpcSetPoint
            // 
            this.tb_cpcSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_cpcSetPoint.Location = new System.Drawing.Point(821, 322);
            this.tb_cpcSetPoint.Name = "tb_cpcSetPoint";
            this.tb_cpcSetPoint.ReadOnly = true;
            this.tb_cpcSetPoint.Size = new System.Drawing.Size(174, 44);
            this.tb_cpcSetPoint.TabIndex = 10;
            this.tb_cpcSetPoint.Text = "0.0";
            this.tb_cpcSetPoint.WordWrap = false;
            // 
            // btn_StepUp
            // 
            this.btn_StepUp.Enabled = false;
            this.btn_StepUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_StepUp.Location = new System.Drawing.Point(821, 272);
            this.btn_StepUp.Name = "btn_StepUp";
            this.btn_StepUp.Size = new System.Drawing.Size(174, 44);
            this.btn_StepUp.TabIndex = 11;
            this.btn_StepUp.Text = "+";
            this.btn_StepUp.UseVisualStyleBackColor = true;
            this.btn_StepUp.Click += new System.EventHandler(this.btn_StepUp_Click);
            // 
            // btn_StepDown
            // 
            this.btn_StepDown.Enabled = false;
            this.btn_StepDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_StepDown.Location = new System.Drawing.Point(821, 372);
            this.btn_StepDown.Name = "btn_StepDown";
            this.btn_StepDown.Size = new System.Drawing.Size(174, 44);
            this.btn_StepDown.TabIndex = 12;
            this.btn_StepDown.Text = "-";
            this.btn_StepDown.UseVisualStyleBackColor = true;
            this.btn_StepDown.Click += new System.EventHandler(this.btn_StepDown_Click);
            // 
            // btn_setSetPoint
            // 
            this.btn_setSetPoint.Enabled = false;
            this.btn_setSetPoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_setSetPoint.Location = new System.Drawing.Point(1001, 322);
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
            this.tb_newSetPoint.Location = new System.Drawing.Point(1001, 272);
            this.tb_newSetPoint.Name = "tb_newSetPoint";
            this.tb_newSetPoint.Size = new System.Drawing.Size(174, 44);
            this.tb_newSetPoint.TabIndex = 14;
            this.tb_newSetPoint.Text = "1.0";
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
            this.lbl_cpcmean.Location = new System.Drawing.Point(751, 461);
            this.lbl_cpcmean.Name = "lbl_cpcmean";
            this.lbl_cpcmean.Size = new System.Drawing.Size(100, 23);
            this.lbl_cpcmean.TabIndex = 20;
            this.lbl_cpcmean.Text = "--------";
            // 
            // lbl_ekmean
            // 
            this.lbl_ekmean.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_ekmean.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_ekmean.Location = new System.Drawing.Point(641, 461);
            this.lbl_ekmean.Name = "lbl_ekmean";
            this.lbl_ekmean.Size = new System.Drawing.Size(100, 23);
            this.lbl_ekmean.TabIndex = 21;
            this.lbl_ekmean.Text = "--------";
            // 
            // btn_copyMeansToClipboard
            // 
            this.btn_copyMeansToClipboard.Location = new System.Drawing.Point(873, 461);
            this.btn_copyMeansToClipboard.Name = "btn_copyMeansToClipboard";
            this.btn_copyMeansToClipboard.Size = new System.Drawing.Size(75, 23);
            this.btn_copyMeansToClipboard.TabIndex = 22;
            this.btn_copyMeansToClipboard.Text = "copy";
            this.btn_copyMeansToClipboard.UseVisualStyleBackColor = true;
            this.btn_copyMeansToClipboard.Click += new System.EventHandler(this.btn_copyMeansToClipboard_Click);
            // 
            // lbl_EKstdev
            // 
            this.lbl_EKstdev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EKstdev.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_EKstdev.Location = new System.Drawing.Point(641, 490);
            this.lbl_EKstdev.Name = "lbl_EKstdev";
            this.lbl_EKstdev.Size = new System.Drawing.Size(100, 23);
            this.lbl_EKstdev.TabIndex = 23;
            this.lbl_EKstdev.Text = "--------";
            // 
            // lbl_CPCstdev
            // 
            this.lbl_CPCstdev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CPCstdev.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CPCstdev.Location = new System.Drawing.Point(751, 490);
            this.lbl_CPCstdev.Name = "lbl_CPCstdev";
            this.lbl_CPCstdev.Size = new System.Drawing.Size(100, 23);
            this.lbl_CPCstdev.TabIndex = 24;
            this.lbl_CPCstdev.Text = "--------";
            // 
            // lbl_EKLRSlope
            // 
            this.lbl_EKLRSlope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EKLRSlope.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_EKLRSlope.Location = new System.Drawing.Point(641, 519);
            this.lbl_EKLRSlope.Name = "lbl_EKLRSlope";
            this.lbl_EKLRSlope.Size = new System.Drawing.Size(100, 23);
            this.lbl_EKLRSlope.TabIndex = 25;
            this.lbl_EKLRSlope.Text = "--------";
            // 
            // lbl_CPCLRSlope
            // 
            this.lbl_CPCLRSlope.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CPCLRSlope.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CPCLRSlope.Location = new System.Drawing.Point(751, 519);
            this.lbl_CPCLRSlope.Name = "lbl_CPCLRSlope";
            this.lbl_CPCLRSlope.Size = new System.Drawing.Size(100, 23);
            this.lbl_CPCLRSlope.TabIndex = 26;
            this.lbl_CPCLRSlope.Text = "--------";
            // 
            // lbl_EKstability
            // 
            this.lbl_EKstability.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_EKstability.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_EKstability.Location = new System.Drawing.Point(641, 548);
            this.lbl_EKstability.Name = "lbl_EKstability";
            this.lbl_EKstability.Size = new System.Drawing.Size(100, 23);
            this.lbl_EKstability.TabIndex = 27;
            this.lbl_EKstability.Text = "--------";
            // 
            // lbl_CPCstability
            // 
            this.lbl_CPCstability.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbl_CPCstability.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_CPCstability.Location = new System.Drawing.Point(751, 548);
            this.lbl_CPCstability.Name = "lbl_CPCstability";
            this.lbl_CPCstability.Size = new System.Drawing.Size(100, 23);
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
            this.dataGridView1.Size = new System.Drawing.Size(593, 222);
            this.dataGridView1.TabIndex = 32;
            // 
            // percent
            // 
            this.percent.HeaderText = "%шкалы";
            this.percent.Name = "percent";
            this.percent.ReadOnly = true;
            this.percent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // calcPressure
            // 
            this.calcPressure.HeaderText = "Расчетное давление";
            this.calcPressure.Name = "calcPressure";
            this.calcPressure.ReadOnly = true;
            // 
            // cpcPressure
            // 
            this.cpcPressure.HeaderText = "Действительное давление";
            this.cpcPressure.Name = "cpcPressure";
            this.cpcPressure.ReadOnly = true;
            this.cpcPressure.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ekCurrent
            // 
            this.ekCurrent.HeaderText = "ток";
            this.ekCurrent.Name = "ekCurrent";
            this.ekCurrent.ReadOnly = true;
            this.ekCurrent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // error
            // 
            this.error.HeaderText = "погрешность, %";
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
            // cbScaleUOM
            // 
            this.cbScaleUOM.FormattingEnabled = true;
            this.cbScaleUOM.Location = new System.Drawing.Point(226, 271);
            this.cbScaleUOM.Name = "cbScaleUOM";
            this.cbScaleUOM.Size = new System.Drawing.Size(121, 21);
            this.cbScaleUOM.TabIndex = 35;
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1249, 602);
            this.Controls.Add(this.cbScaleUOM);
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
            this.Controls.Add(this.tb_cpcSetPoint);
            this.Controls.Add(this.tb_newSetPoint);
            this.Controls.Add(this.tb_cpcStep);
            this.Controls.Add(this.cb_cpcChannels);
            this.Controls.Add(this.btn_openCPC);
            this.Controls.Add(this.cb_cpc);
            this.Controls.Add(this.btn_openEK);
            this.Controls.Add(this.lbl_cpc_read);
            this.Controls.Add(this.lbl_cnahValue);
            this.Controls.Add(this.cb_chanNUM);
            this.Controls.Add(this.cb_SP);
            this.Name = "MainForm";
            this.Text = "ШШтыекгьуте";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb_SP;
        private System.Windows.Forms.ComboBox cb_chanNUM;
        private System.Windows.Forms.Label lbl_cnahValue;
        private System.Windows.Forms.Button btn_openEK;
        private System.Windows.Forms.ComboBox cb_cpc;
        private System.Windows.Forms.Button btn_openCPC;
        private System.Windows.Forms.ComboBox cb_cpcChannels;
        private System.Windows.Forms.Label lbl_cpc_read;
        private System.Windows.Forms.TextBox tb_cpcStep;
        private System.Windows.Forms.TextBox tb_cpcSetPoint;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn percent;
        private System.Windows.Forms.DataGridViewTextBoxColumn calcPressure;
        private System.Windows.Forms.DataGridViewTextBoxColumn cpcPressure;
        private System.Windows.Forms.DataGridViewTextBoxColumn ekCurrent;
        private System.Windows.Forms.DataGridViewTextBoxColumn error;
        private System.Windows.Forms.ComboBox cbScaleUOM;
        private System.Windows.Forms.Label label3;
    }
}

