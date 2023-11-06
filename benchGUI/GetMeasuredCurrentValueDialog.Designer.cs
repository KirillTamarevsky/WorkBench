namespace benchGUI
{
    partial class GetMeasuredCurrentValueDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn_Cancel = new System.Windows.Forms.Button();
            btn_OK = new System.Windows.Forms.Button();
            tb_mA_value = new System.Windows.Forms.TextBox();
            SuspendLayout();
            // 
            // btn_Cancel
            // 
            btn_Cancel.Location = new System.Drawing.Point(120, 51);
            btn_Cancel.Name = "btn_Cancel";
            btn_Cancel.Size = new System.Drawing.Size(75, 23);
            btn_Cancel.TabIndex = 3;
            btn_Cancel.Text = "Отмена";
            btn_Cancel.UseVisualStyleBackColor = true;
            btn_Cancel.Click += btn_Cancel_Click;
            // 
            // btn_OK
            // 
            btn_OK.Location = new System.Drawing.Point(12, 51);
            btn_OK.Name = "btn_OK";
            btn_OK.Size = new System.Drawing.Size(75, 23);
            btn_OK.TabIndex = 2;
            btn_OK.Text = "OK";
            btn_OK.UseVisualStyleBackColor = true;
            btn_OK.Click += btn_OK_Click;
            // 
            // tb_mA_value
            // 
            tb_mA_value.Location = new System.Drawing.Point(12, 12);
            tb_mA_value.Name = "tb_mA_value";
            tb_mA_value.Size = new System.Drawing.Size(183, 23);
            tb_mA_value.TabIndex = 1;
            tb_mA_value.TextChanged += tb_mA_value_TextChanged;
            // 
            // GetMeasuredCurrentValueDialog
            // 
            AcceptButton = btn_OK;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btn_Cancel;
            ClientSize = new System.Drawing.Size(211, 92);
            Controls.Add(tb_mA_value);
            Controls.Add(btn_OK);
            Controls.Add(btn_Cancel);
            Name = "GetMeasuredCurrentValueDialog";
            Text = "Введите измеренное значение тока";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.TextBox tb_mA_value;
    }
}