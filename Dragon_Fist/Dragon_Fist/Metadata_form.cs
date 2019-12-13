using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;

namespace Dragon_Fist
{
    public partial class Metadata_form : Form
    {
        int is_click_md = 0;
        int is_mdf_ok = 0;
        int is_sig_correct = 0;
        int is_meta_exist = 0;
        int is_md_ok = 0;
        String package_name = null;
        String select_platform = null;
        String path = null;
        List<String> items = new List<String>();

        public Metadata_form(int sig, int meta, String _package_name, String _select_platform)
        {
            InitializeComponent();
            is_sig_correct = sig; is_meta_exist = meta;
            package_name = _package_name; select_platform = _select_platform;
            path = @Application.StartupPath + "\\Modules\\memorydump";
            if (is_sig_correct == 1) { sig_correct_label.Visible = true; sig_incorrect_label.Visible = false; }
            else { sig_correct_label.Visible = false; sig_incorrect_label.Visible = true; }
            if(is_meta_exist == 1) { file_yes_label.Visible = true; file_no_label.Visible = false; }
            else { file_yes_label.Visible = false; file_no_label.Visible = true; }
            button3.TabStop = false; button3.FlatStyle = FlatStyle.Flat; button3.FlatAppearance.BorderSize = 0;
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;
            button8.TabStop = false; button8.FlatStyle = FlatStyle.Flat; button8.FlatAppearance.BorderSize = 0;
        }

        private void Metadata_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            is_mdf_ok = 1;
        }

        public List<String> get_items() { return items; }

        public int get_is_meta_exist() { return is_meta_exist; }

        public int get_is_md_ok() { return is_md_ok; }

        public int get_is_mdf_ok() { return is_mdf_ok; }

        public int get_is_click_md() { return is_click_md; }

        public ProcessStartInfo Set_Process(String file_name, String working_path, bool is_output)
        {
            ProcessStartInfo pro_info = new ProcessStartInfo();
            pro_info.FileName = @file_name;
            pro_info.WorkingDirectory = working_path + "\\";
            pro_info.CreateNoWindow = true;
            pro_info.UseShellExecute = false;
            pro_info.RedirectStandardOutput = is_output;
            pro_info.RedirectStandardInput = true;
            pro_info.RedirectStandardError = false;
            return pro_info;
        }

        public string Run_process(ProcessStartInfo pro_info, String cmd_str, bool is_output)
        {
            String result = null;
            Process pro = new Process();
            pro.StartInfo = pro_info;
            pro.Start();
            pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            pro.StandardInput.Close();
            if (is_output == true) { result = pro.StandardOutput.ReadToEnd(); }
            pro.WaitForExit();
            pro.Close();
            return result;
        }

        // using Android Debug Bridge
        private void Memory_dump(String package_name, String platform)
        {
            string txt = null;
            ProcessStartInfo proInfo = Set_Process("cmd", path, true);

            // adb push memorydump /data/local/tmp
            String cmd_str = "adb push " + path + "\\memorydump_" + platform + " /data/local/tmp/memorydump";
            Run_process(proInfo, cmd_str, false);

            // adb shell chmod 755 /data/local/tmp/memorydump
            cmd_str = "adb shell chmod 755 /data/local/tmp/memorydump";
            Run_process(proInfo, cmd_str, false);

            // adb shell su -c chown root:root /data/local/tmp/memorydump
            cmd_str = "adb shell \"su -c chown root:root /data/local/tmp/memorydump\"";
            Run_process(proInfo, cmd_str, false);

            // adb shell "su -c ps -A | grep packagename"
            cmd_str = "adb shell \"su -c 'ps | grep ";
            cmd_str += package_name;
            cmd_str += "'\"";
            txt = Run_process(proInfo, cmd_str, true);
            string sentence = txt.Split('\n')[4];
            List<string> stringArray = sentence.Split(' ').ToList();
            stringArray.RemoveAll(str => String.IsNullOrEmpty(str));
            string pid = stringArray[1];

            // adb shell cp /proc/pid/maps /data/local/tmp/maps
            cmd_str = "adb shell \"su -c 'cp /proc/";
            cmd_str += pid;
            cmd_str += "/maps /data/local/tmp/maps' \"";
            Run_process(proInfo, cmd_str, false);

            proInfo = Set_Process("cmd", path, false);

            // adb shell ./memdump
            cmd_str = "adb shell \"su -c '/data/local/tmp/memorydump ";
            cmd_str += pid;
            cmd_str += "'\"";
            Run_process(proInfo, cmd_str, false);

            // adb shell chmod 755 /data/local/tmp/global-metadata.dat
            cmd_str = "adb shell \"su -c 'chmod 755 /data/local/tmp/global-metadata.dat '\"";
            Run_process(proInfo, cmd_str, false);

            DirectoryInfo dbDir = new DirectoryInfo(path + "\\" + package_name);
            if (!dbDir.Exists) dbDir.Create();

            // adb pull global-metadata.dat
            cmd_str = "adb pull /data/local/tmp/global-metadata.dat ";
            cmd_str += "\"" + path + "\\" + package_name + "\\global-metadata.dat\"";
            Run_process(proInfo, cmd_str, false);
        }

        private void button4_Click_1(object sender, EventArgs e) // back button
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e) // Memory dump button
        {
            is_click_md = 1;
            List<String> txt = new List<string>(); int is_using = 0;

            // adb check
            ProcessStartInfo proInfo = Set_Process("cmd", @Application.StartupPath, true);
            Process current_pro = new Process();
            current_pro.EnableRaisingEvents = false;
            current_pro.StartInfo = proInfo;
            current_pro.Start();

            String cmd_str = "adb devices"; String tmp = null; int cnt = 0;
            current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            current_pro.StandardInput.Close();
            while (!current_pro.StandardOutput.EndOfStream)
            {
                tmp = current_pro.StandardOutput.ReadLine();
                if (!tmp.Contains("devices")) { txt.Add(tmp); }
            }
            is_using = 1;
            current_pro.WaitForExit();
            current_pro.Close();

            if (is_using == 1)
            {
                for (int i = 0; i < txt.Count; i++)
                {
                    if (txt[i].Contains("device")) { cnt++; }
                }
            }

            if (cnt == 0) { MessageBox.Show("Device or adb not found", "Error"); return; }
            else if (cnt > 0) {
                try { Memory_dump(package_name, select_platform); }
                catch (Exception e1)
                {
                    MessageBox.Show("Fail to memory dump\n\n" + e1.ToString() + "\n\nCheck your adb or application", "Error");
                    return;
                }
                MessageBox.Show("Success to dump memory!", "Info");
                is_md_ok = 1;
            }
        }

        private void button3_Click(object sender, EventArgs e) // Add to Report
        {
            items.Clear();
            items.Add("Meta data");
            MessageBox.Show("Success to add to report", "Info");
        }

        #region Button Color

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            button3.BackColor = Color.DodgerBlue;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            button4.BackColor = Color.DodgerBlue;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button8_MouseMove(object sender, MouseEventArgs e)
        {
            button8.BackColor = Color.DodgerBlue;
        }

        private void button8_MouseLeave(object sender, EventArgs e)
        {
            button8.BackColor = Color.FromArgb(64, 64, 64);
        }

        #endregion
    }
}
