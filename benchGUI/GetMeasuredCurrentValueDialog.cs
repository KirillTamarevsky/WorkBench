using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace benchGUI
{
    public partial class GetMeasuredCurrentValueDialog : Form
    {
        public Single MeasuredCurrentValue { get; private set; }
        public GetMeasuredCurrentValueDialog()
        {
            InitializeComponent();
            btn_OK.Enabled = false;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (tb_mA_value.Text.TryParseToDouble(out double value))
            {
                MeasuredCurrentValue = (float)value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void tb_mA_value_TextChanged(object sender, EventArgs e)
        {
            if (tb_mA_value.Text.TryParseToDouble(out double _))
            {
                btn_OK.Enabled = true;
            }
            else
            {
                btn_OK.Enabled = false;
            }
        }
    }
}
