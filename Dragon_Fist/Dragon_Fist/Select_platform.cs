using System;
using System.Windows.Forms;
using System.Drawing;

namespace Dragon_Fist
{
    public partial class Select_platform : Form
    {
        int is_android_ARM64 = 0;
        int is_android_ARMv7 = 0;
        int is_emulator = 0;

        public Select_platform()
        {
            InitializeComponent();
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;
            button5.TabStop = false; button5.FlatStyle = FlatStyle.Flat; button5.FlatAppearance.BorderSize = 0;
        }

        private void button4_Click(object sender, EventArgs e) // back button
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public int _is_android_ARM64 { get { return is_android_ARM64; } }

        public int _is_android_ARMv7 { get { return is_android_ARMv7; } }

        public int _is_emulator { get { return is_emulator; } }

        private void button5_Click(object sender, EventArgs e) // apply button
        {
            int cnt = 0;
            if (checkBox1.Checked == true)  { is_android_ARMv7 = 1; } // ARMv7
            if (checkBox2.Checked == true) {  is_emulator = 1; } // x86
            if (checkBox3.Checked == true) {  is_android_ARM64 = 1; } // ARM64
            if (checkBox1.Checked == false && checkBox2.Checked == false && checkBox3.Checked == false) { MessageBox.Show(this, "Please select a platfrom", "Error"); return; }
            if(is_android_ARM64 == 1) { cnt++; }
            if (is_android_ARMv7 == 1) { cnt++; }
            if (is_emulator == 1) { cnt++; }
            if (cnt > 1) {
                is_android_ARM64 = 0; is_emulator = 0; is_android_ARMv7 = 0;
                MessageBox.Show(this, "Please select only one platfrom", "Error");
                return;
            }
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
