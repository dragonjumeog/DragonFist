using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Dragon_Fist
{
    public partial class TR_form : Form
    {
        struct called_function
        {
            // time function
            public int clock_gettime;
            public int gettimeofday;

            // rand function
            // // namespace in System
            public int random_next; // Next()
            public int random_next_1; // Next(int maxValue)
            public int random_next_2; // Next (int  minValue, int maxValue)
            // // namespace in UnityEngine
            public int random_range_f; // float Range(float min, float max) 
            public int random_range_i; // int Range(int min, int max)
        }

        ////  Variables
        // socket transmission
        UdpClient srv = null;
        Thread server;
        bool server_running = true;

        // time hook process
        Process time_pro;

        // rand hook process
        Process rand_pro;

        // called function
        called_function hook = new called_function();

        // python process
        bool time_process_on = false;
        bool rand_process_on = false;

        String dump_path = null;
        String package_name = null;

        // rand and time value ( min -> average -> max )
        int rand_value = 0;
        int time_value = 0;

        String platform = null;
        List<int> T_status = new List<int>();
        List<int> R_status = new List<int>();
        int is_t_runned = 0;
        int is_r_runned = 0;
        int is_mono = 0;
        List<String> items = new List<String>();
        List<String> time_report = new List<String>();
        List<String> rand_report = new List<String>();
        //// Variables

        public TR_form(String _dump_path, String _package_name, String _platform, int _is_mono)
        {
            InitializeComponent();

            platform = _platform;

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("Log", 800, HorizontalAlignment.Center);

            button1.TabStop = false; button1.FlatStyle = FlatStyle.Flat; button1.FlatAppearance.BorderSize = 0;
            button2.TabStop = false; button2.FlatStyle = FlatStyle.Flat; button2.FlatAppearance.BorderSize = 0;
            button3.TabStop = false; button3.FlatStyle = FlatStyle.Flat; button3.FlatAppearance.BorderSize = 0;
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;
            button5.TabStop = false; button5.FlatStyle = FlatStyle.Flat; button5.FlatAppearance.BorderSize = 0;

            // hook initialize
            hook.clock_gettime = 0;
            hook.gettimeofday = 0;

            hook.random_next = 0;
            hook.random_next_1 = 0;
            hook.random_next_2 = 0;
            hook.random_range_f = 0;
            hook.random_range_i = 0;

            dump_path = _dump_path;
            package_name = _package_name;
            is_mono = _is_mono;

            server = new Thread(UdpServerStart);
            server.IsBackground = true;
            server.Start();
        }

        public List<String> get_items() { return items; }

        public List<String> get_time_report() { return time_report; }

        public List<String> get_rand_report() { return rand_report; }

        public List<int> get_T_status()
        {
            T_status.Clear();

            T_status.Add(hook.clock_gettime);
            T_status.Add(hook.gettimeofday);

            return T_status;
        }

        public List<int> get_R_status()
        {
            R_status.Clear();

            R_status.Add(hook.random_next);
            R_status.Add(hook.random_next_1);
            R_status.Add(hook.random_next_2);
            R_status.Add(hook.random_range_f);
            R_status.Add(hook.random_range_i);

            return R_status;
        }

        private void UdpServerStart()
        {
            server_running = true;

            try
            {
                srv = new UdpClient(5585);
                IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                String tmp;
                while (server_running)
                {
                    byte[] dgram = srv.Receive(ref clientEP);
                    if (dgram.Length != 0)
                    {
                        tmp = Encoding.Default.GetString(dgram);

                        if (tmp.Contains("[+]"))
                        {
                            if (hook.clock_gettime != 1)
                            {
                                if (tmp.Contains("[+] clock_gettime() Called!"))
                                {
                                    hook.clock_gettime = 1;
                                    clock_gettime_function.Text = "Called!";
                                    clock_gettime_function.ForeColor = Color.Red;
                                }
                            }

                            if (hook.gettimeofday != 1)
                            {
                                if (tmp.Contains("[+] gettimeofday() Called!"))
                                {
                                    hook.gettimeofday = 1;
                                    gettimeofday.Text = "Called!";
                                    gettimeofday.ForeColor = Color.Red;
                                }
                            }

                            if (hook.random_next != 1)
                            {
                                if (tmp.Contains("[+] Random.Next() Called!"))
                                {
                                    hook.random_next = 1;
                                    next_function.Text = "Called!";
                                    next_function.ForeColor = Color.Red;
                                }
                            }

                            if (hook.random_next_1 != 1)
                            {
                                if (tmp.Contains("[+] Random.Next(int maxValue) Called!"))
                                {
                                    hook.random_next_1 = 1;
                                    next_function.Text = "Called!";
                                    next_function.ForeColor = Color.Red;
                                }
                            }

                            if (hook.random_next_2 != 1)
                            {
                                if (tmp.Contains("[+] Random.Next(int minValue, int maxValue) Called!"))
                                {
                                    hook.random_next_2 = 1;
                                    next_function.Text = "Called!";
                                    next_function.ForeColor = Color.Red;
                                }
                            }

                            if (hook.random_range_f != 1)
                            {
                                if (tmp.Contains("[+] float Random.Range(float min, float max) Called!"))
                                {
                                    hook.random_range_f = 1;
                                    range_function.Text = "Called!";
                                    range_function.ForeColor = Color.Red;
                                }
                            }

                            if (hook.random_range_i != 1)
                            {
                                if (tmp.Contains("[+] int Random.Range(int min, int max) Called!"))
                                {
                                    hook.random_range_i = 1;
                                    range_function.Text = "Called!";
                                    range_function.ForeColor = Color.Red;
                                }
                            }

                        }

                        if (tmp.Contains(" - "))
                        {
                            // time
                            if (tmp.Contains("time"))
                            {
                                time_report.Add(tmp);
                            }
                            else // (tmp.Contains("Random"))
                            {
                                rand_report.Add(tmp);
                            }
                        }

                        listView1.Items.Add(tmp);
                        listView1.EnsureVisible(listView1.Items.Count - 1);
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(this, e.ToString());
                srv.Close();
            }
            finally
            {
                srv.Close();
            }
        }

        private List<String> GetRandomRVA()
        {
            // In this section, We can get the RVA value of below random function
            // The list of random function
            // 1. namespace System @ Random.Next()
            // 2. namespace UnityEngine @ Random.Range()

            List<String> function = new List<String>(new String[] { "Random@int Next()", "Random@int Next(int maxValue)", "Random@int Next(int minValue, int maxValue)", "Random@float Range(float min, float max)", "Random@int Range(int min, int max)" });
            List<String> offset = new List<string>();
            bool findClass = false;
            bool start = false;
            bool found = false;

            try
            {
                String[] lines = File.ReadAllLines(@dump_path + "\\dump.cs"); // <-= change directory!

                for (int i = 0; i < function.Count; i++)
                {
                    foreach (String line in lines)
                    {
                        if (line.Contains("class Random")) // find class
                        {
                            findClass = true;
                        }
                        else if (findClass && line.Contains("// Methods")) // Methods
                        {
                            start = true;
                        }
                        else if (findClass && start && line.Contains(function[i].Split('@')[1]))
                        {
                            //MessageBox.Show(this, line.Split(';')[1]);
                            String tmp_offset = line.Split('}')[1].Split(' ')[3];

                            offset.Add(tmp_offset);

                            found = true;
                            findClass = false;
                            start = false;
                            break;
                        }
                        else if (!found && (line == "}"))
                        {
                            findClass = false;
                            start = false;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(this, e.ToString(), "Error");
            }
            return offset;
        }

        private void GenerateRandJS(List<String> offset)
        {
            String code =
                "Java.perform(function(){\n" +
                "    send('[*] Running Rand Hook...');\n" +
                "    var il2cpp = Module.getBaseAddress('libil2cpp.so');\n" +
                //"    send(' -  libil2cpp.so @ ' + il2cpp.toString());\n\n" +
                "    var next_flag = false;\n" +
                "    var next_1_flag = false;\n" +
                "    var next_2_flag = false;\n" +
                "    var range_i_flag = false;\n" +
                "    var range_f_flag = false;\n\n";

            for (int i = 0; i < offset.Count; i++)
            {
                code += "    var offset" + i.ToString() + " = " + offset[i] + ";\n";
            }

            code += "\n    var target0 = il2cpp.add(offset0);\n"
                + "    send(' -  Random.Next() @ ' + target0);\n"
                + "    Interceptor.attach(target0, {\n"
                + "        onEnter : function(args){\n"
                + "            if(!next_flag){\n"
                + "                send('[+] Random.Next() Called!');\n"
                + "                next_flag = true;\n"
                + "            }\n"
                + "        },\n"
                + "        onLeave : function(retval){\n";

            if (rand_value == 1) // min
                code += "            retval.replace(0);\n";
            else if (rand_value == 2) // average
                code += "            retval.replace(0x3fffffff);\n";
            else if (rand_value == 3)// max
                code += "            retval.replace(0x7ffffffe);\n";

            code += "        }\n"
                + "    });\n"

                + "    var save1;\n"
                + "    var target1 = il2cpp.add(offset1);\n"
                + "    send(' -  Random.Next(int MaxValue) @ ' + target1);\n"
                + "    Interceptor.attach(target1, {\n"
                + "        onEnter : function(args){\n"
                + "            if(!next_1_flag){\n"
                + "                send('[+] Random.Next(int MaxValue) Called!');\n"
                + "                next_1_flag = true;\n"
                + "            }\n";

            if (rand_value == 1) // min
                code += "            save1 = 0;\n";
            else if (rand_value == 2) // average
                code += "            save1 = parseInt(( parseInt(args[1]) - 1 )/2);\n";
            else if (rand_value == 3)// max
                code += "            save1 = parseInt(args[1]) - 1;\n";

            code += "        },\n"
                + "        onLeave : function(retval){\n";
            if (rand_value != 0)
                code += "            retval.replace(save1);\n";
            code += "        }\n"
                + "    });\n"

                + "    var save2;\n"
                + "    var target2 = il2cpp.add(offset2);\n"
                + "    send(' -  Random.Next(int MinValue, int MaxValue) @ ' + target2);\n"
                + "    Interceptor.attach(target2, {\n"
                + "        onEnter : function(args){\n"
                + "            if(!next_2_flag){\n"
                + "                send('[+] Random.Next(int MinValue, int MaxValue) Called!');\n"
                + "                next_2_flag = true;\n"
                + "            }\n";

            if (rand_value == 1) // min
                code += "            save2 = args[1];\n";
            else if (rand_value == 2) // average
                code += "            save2 = parseInt((parseInt(args[1]) + parseInt(args[2]) - 1)/2);\n";
            else if (rand_value == 3)// max
                code += "            save2 = parseInt(args[2]) - 1;\n";

            code += "        },\n"
                + "        onLeave : function(retval){\n";
            if (rand_value != 0)
                code += "            retval.replace(save2);\n";
            code += "        }\n"
                + "    });\n"

                + "    var save3;\n"
                + "    var target3 = il2cpp.add(offset3);\n"
                + "    send(' -  float Random.Range(float min, float max) @ ' + target3);\n"
                + "    Interceptor.attach(target3, {\n"
                + "        onEnter : function(args){\n"
                + "            if(!range_f_flag){\n"
                + "                send('[+] float Random.Range(float min, float max) Called!');\n"
                + "                range_f_flag = true;\n"
                + "            }\n";

            if (rand_value == 1) // min
                code += "            save3 = args[0];\n";
            else if (rand_value == 2) // average
                code += "            save3 = (parseInt(args[0]) + parseInt(args[1])) / 2;\n";
            else if (rand_value == 3)// max
                code += "            save3 = args[1];\n";

            code += "        },\n"
                + "        onLeave : function(retval){\n";
            if (rand_value != 0)
                code += "            retval.replace(save3);\n";
            code += "        }\n"
                + "    });\n"

                + "    var save4;\n"
                + "    var target4 = il2cpp.add(offset4);\n"
                + "    send(' -  int Random.Range(int min, int max) @ ' + target4);\n"
                + "    Interceptor.attach(target4, {\n"
                + "        onEnter : function(args){\n"
                + "            if(!range_i_flag){\n"
                + "                send('[+] int Random.Range(int min, int max) Called!');\n"
                + "                range_i_flag = true;\n"
                + "            }\n";

            if (rand_value == 1) // min
                code += "            save4 = args[0];\n";
            else if (rand_value == 2) // average
                code += "            save4 = (parseInt(args[0]) + parseInt(args[1])) / 2;\n";
            else if (rand_value == 3)// max
                code += "            save4 = args[1];\n";

            code += "        },\n"
                + "        onLeave : function(retval){\n";
            if (rand_value != 0)
                code += "            retval.replace(save4);\n";
            code += "        }\n"
                + "    });\n"
                + "});\n";

            using (StreamWriter outFile = new StreamWriter(@Application.StartupPath + @"\Modules\rand\rand.js"))
            {
                outFile.Write(code);
            }
        }

        private void GenerateRandHookCode()
        {
            String source = File.ReadAllText(@Application.StartupPath + @"\Modules\rand\base.py");
            String jscode = File.ReadAllText(@Application.StartupPath + @"\Modules\rand\rand.js");

            int jscodeIndex = source.IndexOf("'''");

            String Payload = source.Substring(0, jscodeIndex + 4);
            Payload += jscode;
            Payload += source.Substring(jscodeIndex + 4);

            using (StreamWriter outFile = new StreamWriter(@Application.StartupPath + @"\Modules\rand\rand.py"))
            {
                outFile.Write(Payload);
            }
        }
        private void RandHook()
        {
            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = "python.exe";
            proInfo.Arguments = String.Format("-u {0} {1}", "\"" + @Application.StartupPath + "\"" + @"\Modules\rand\rand.py", package_name);
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;

            rand_pro = new Process();
            rand_pro.StartInfo = proInfo;
            rand_pro.Start();

            rand_process_on = true;
        }

        private void TimeHook()
        {
            String arch = null;
            String ratio = null;

            if (time_value == 0)
                ratio = "0.5";
            else if (time_value == 1)
                ratio = "1.0";
            else if (time_value == 2)
                ratio = "1.5";
            else if (time_value == 3)
                ratio = "2.0";
            else
                ratio = "3.0";

            if (platform == "arm64")
                arch = "0x8";
            else
                arch = "0x4";

            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = "python.exe";
            proInfo.Arguments = String.Format("-u {0} {1} {2} {3}", "\"" + @Application.StartupPath + "\"" + @"\Modules\time\time.py", package_name, ratio, arch);
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;

            time_pro = new Process();
            time_pro.StartInfo = proInfo;
            time_pro.Start();

            time_process_on = true;
        }

        private bool CheckFridaServer()
        {
            String output;

            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = "cmd";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardOutput = true;

            Process shell_pro = new Process();
            shell_pro.StartInfo = proInfo;
            shell_pro.Start();
            shell_pro.StandardInput.Write("frida-ps -U" + Environment.NewLine);
            shell_pro.StandardInput.Close();
            output = shell_pro.StandardOutput.ReadToEnd();
            shell_pro.WaitForExit();
            shell_pro.Close();

            if (output.Contains("zygote"))
                return true;
            else
                return false;
        }

        private bool CheckProcess()
        {
            String output;

            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = "cmd";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardOutput = true;

            Process shell_pro = new Process();
            shell_pro.StartInfo = proInfo;
            shell_pro.Start();
            shell_pro.StandardInput.Write("adb shell ps" + Environment.NewLine);
            shell_pro.StandardInput.Close();
            output = shell_pro.StandardOutput.ReadToEnd();
            shell_pro.WaitForExit();
            shell_pro.Close();

            if (output.Contains(package_name))
                return true;
            else
                return false;
        }

        private void button1_Click(object sender, EventArgs e) // Time
        {
            if (!CheckFridaServer())
            {
                MessageBox.Show(this, "[Error Code = 0x02]\n\nPlease Run Frida-Server on your device!", "Error");
                return;
            }

            if (!CheckProcess())
            {
                MessageBox.Show(this, package_name + " Not Found!", "Error");
                return;
            }

            if (time_process_on)
            {
                time_pro.Kill();
                time_process_on = false;
            }
            TimeHook();
            is_t_runned = 1;
        }

        private void button2_Click(object sender, EventArgs e) // Back
        {
            if (server_running)
            {
                server_running = false;
                srv.Close();
                server.Abort();
            }
            if (time_process_on)
            {
                time_pro.Kill();
                time_process_on = false;
            }
            if (rand_process_on)
            {
                rand_pro.Kill();
                rand_process_on = false;
            }
            this.Close();
        }

        private void button1_Click_1(object sender, EventArgs e) // Random
        {
            if (!CheckFridaServer())
            {
                MessageBox.Show(this, "[Error Code = 0x02]\n\nPlease Run Frida-Server on your device!", "Error");
                return;
            }

            if (!CheckProcess())
            {
                MessageBox.Show(this, package_name + " Not Found!", "Error");
                return;
            }

            if (is_mono == 1)
            {
                MessageBox.Show(this, "Mono can't use Random", "Info");
                return;
            }
            if (rand_process_on)
            {
                rand_pro.Kill();
                rand_process_on = false;
            }
            List<String> offset = GetRandomRVA();
            GenerateRandJS(offset);
            GenerateRandHookCode();
            RandHook();
            is_r_runned = 1;
        }

        #region Button Color

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.BackColor = Color.DodgerBlue;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2.BackColor = Color.DodgerBlue;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(64, 64, 64);
        }

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

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            button5.BackColor = Color.DodgerBlue;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.BackColor = Color.FromArgb(64, 64, 64);
        }

        #endregion

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value == 0)
                Bar1.Text = "0.5";
            else if (trackBar1.Value == 1)
                Bar1.Text = "1.0";
            else if (trackBar1.Value == 2)
                Bar1.Text = "1.5";
            else if (trackBar1.Value == 3)
                Bar1.Text = "2.0";
            else if (trackBar1.Value == 4)
                Bar1.Text = "3.0";

            time_value = trackBar1.Value;
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (trackBar2.Value == 0)
                Bar2.Text = "  Default";
            else if (trackBar2.Value == 1)
                Bar2.Text = "     Min";
            else if (trackBar2.Value == 2)
                Bar2.Text = "Average";
            else
                Bar2.Text = "     Max";

            rand_value = trackBar2.Value;
        }

        private void button3_Click(object sender, EventArgs e) // All Stop
        {
            if (time_process_on)
            {
                time_pro.Kill();
                time_process_on = false;
                listView1.Items.Add("[*] Closing Time Hook...");
            }
            if (rand_process_on)
            {
                rand_pro.Kill();
                rand_process_on = false;
                listView1.Items.Add("[*] Closing Rand Hook...");
                listView1.Items.Add("");
                listView1.EnsureVisible(listView1.Items.Count - 1);
            }
        }

        private void button5_Click(object sender, EventArgs e) // Add to Report
        {
            if (is_t_runned == 0 && is_r_runned == 0)
            {
                MessageBox.Show(this, "Please run one more function", "Error");
                return;
            }
            else if (is_t_runned == 0 && is_r_runned == 1)
            {
                items.Add("Random");
                MessageBox.Show(this, "Success to add to report", "Info");
            }
            else if (is_t_runned == 1 && is_r_runned == 0)
            {
                items.Add("Time");
                MessageBox.Show(this, "Success to add to report", "Info");
            }
            else if (is_t_runned == 1 && is_r_runned == 1)
            {
                Select_TR_form trf = new Select_TR_form();
                if (trf.ShowDialog() == DialogResult.OK)
                {
                    is_t_runned = trf.get_is_time;
                    is_r_runned = trf.get_is_random;
                    items.Clear();
                    if (is_r_runned == 1)
                    {
                        items.Add("Time");
                    }
                    if (is_t_runned == 1)
                    {
                        items.Add("Random");
                    }
                    MessageBox.Show(this, "Success to add to report", "Info");
                }
            }
        }
    }
}
