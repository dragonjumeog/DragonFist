using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dragon_Fist
{
    public partial class Select_TR_form : Form
    {
        int is_time = 0;
        int is_random = 0;

        public Select_TR_form()
        {
            InitializeComponent();
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;
            button5.TabStop = false; button5.FlatStyle = FlatStyle.Flat; button5.FlatAppearance.BorderSize = 0;
        }

        public int get_is_time { get { return is_time; } }

        public int get_is_random { get { return is_random; } }

        private void button4_Click(object sender, EventArgs e) // Back
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e) // Apply
        {
            int cnt = 0;
            if (checkBox1.Checked == true) { is_time = 1; }
            if (checkBox3.Checked == true) { is_random = 1; }
            if (checkBox1.Checked == false && checkBox3.Checked == false) { MessageBox.Show(this, "Please select one more function", "Error"); return; }
            MessageBox.Show(this, "Success to apply!", "Info");
            DialogResult = DialogResult.OK;
            this.Close();
        }

        #region Button Color

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            button4.BackColor = Color.DodgerBlue;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            button5.BackColor = Color.DodgerBlue;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.BackColor = Color.FromArgb(64, 64, 64);
        }

        #endregion
    }
}
