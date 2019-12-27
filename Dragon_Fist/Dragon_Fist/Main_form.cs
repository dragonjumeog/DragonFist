using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Data;
using System.Threading;

namespace Dragon_Fist
{
    public partial class Main_form : Form
    {
        int is_mono = 0;
        int is_ok = 0; int is_functions_ok = 0;
        int is_correct_sig = 0;
        int is_meta_file = 0; int is_so_file = 0;
        int is_android_ARM64 = 0; int is_android_ARMv7 = 0; int is_emulator = 0;
        int is_meta_exist = 0; int is_md_ok = 0;
        int is_main_closed = 0;
        int is_click_md = 0;
        int is_hook_ok = 0;
        int is_ocr_runned = 0;
        int is_db_runned = 0;
        int is_mdf_ok = 0;
        int is_searched = 0;
        String search_result = null;
        String metadata_path = null;
        String apk_name = null;
        String changed_apk_name = null;
        String changed_path_name = null;
        String original_path_name = null;
        String dump_path = null;
        String level0 = null;
        String select_platform = null;
        String package_name = null;
        String manifest_path = null;
        String so_path = null;
        String so_path_armeabiv7a = "\\lib\\armeabi-v7a\\libil2cpp.so";
        String so_path_arm64v8a = "\\lib\\arm64-v8a\\libil2cpp.so";
        String so_path_x86 = "\\lib\\x86\\libil2cpp.so";
        String meta_path = "\\assets\\bin\\Data\\Managed\\Metadata\\global-metadata.dat";
        String level0_path = "\\assets\\bin\\Data\\level0";
        String level0_split0_path = "\\assets\\bin\\Data\\level0.split0";
        String unity_default_resources_path = "\\assets\\bin\\Data\\unity default resources";
        byte[] Signature = { 0xAF, 0x1B, 0xB1, 0xFA };
        List<List<String>> meta_f_list = new List<List<string>>();
        List<List<String>> h_list = new List<List<string>>();
        List<int> md_ok = new List<int>();
        List<int> T_status = new List<int>();
        List<int> R_status = new List<int>();
        List<String> items = new List<String>();
        List<String> table_list = new List<String>();
        List<DataTable> itemTables = new List<DataTable>();
        List<String> time_report = new List<String>();
        List<String> rand_report = new List<String>();
        List<List<String>> ocr_list = new List<List<String>>();

        public Main_form()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("Path", 450, HorizontalAlignment.Center);
            listView1.Columns.Add("APK Name", 280, HorizontalAlignment.Center);

            listView2.View = View.Details;
            listView2.FullRowSelect = true;
            listView2.GridLines = true;
            listView2.Columns.Add("Log", 1000, HorizontalAlignment.Center);

            listView3.View = View.Details;
            listView3.FullRowSelect = true;
            listView3.GridLines = true;
            listView3.Columns.Add("Items", 1000, HorizontalAlignment.Center);

            button1.TabStop = false; button1.FlatStyle = FlatStyle.Flat; button1.FlatAppearance.BorderSize = 0;
            button2.TabStop = false; button2.FlatStyle = FlatStyle.Flat; button2.FlatAppearance.BorderSize = 0;
            button3.TabStop = false; button3.FlatStyle = FlatStyle.Flat; button3.FlatAppearance.BorderSize = 0;
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;
            button5.TabStop = false; button5.FlatStyle = FlatStyle.Flat; button5.FlatAppearance.BorderSize = 0;
            button6.TabStop = false; button6.FlatStyle = FlatStyle.Flat; button6.FlatAppearance.BorderSize = 0;
            button7.TabStop = false; button7.FlatStyle = FlatStyle.Flat; button7.FlatAppearance.BorderSize = 0;
            button8.TabStop = false; button8.FlatStyle = FlatStyle.Flat; button8.FlatAppearance.BorderSize = 0;
            button9.TabStop = false; button9.FlatStyle = FlatStyle.Flat; button9.FlatAppearance.BorderSize = 0;
            button10.TabStop = false; button10.FlatStyle = FlatStyle.Flat; button10.FlatAppearance.BorderSize = 0;
            button11.TabStop = false; button11.FlatStyle = FlatStyle.Flat; button11.FlatAppearance.BorderSize = 0;
        }

        private void Main_form_Load(object sender, EventArgs e)
        {
            bool created_new = false;
            Mutex dup = new Mutex(true, "DragonFist", out created_new);
            if (!created_new)
            {
                MessageBox.Show(this, "This Program is already running\n\nPlease check again", "Error");
                Application.Exit();
            }
        }

        private void Main_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            is_main_closed = 1;
        }

        public int get_is_main_closed() { return is_main_closed; }

        private void FormEffect(Form fm)
        {
            double[] opacity = new double[] { 0.1d, 0.2d, 0.3d, 0.5d, 0.7d, 0.8d, 0.9d, 1.0d };
            int cnt = 0;
            System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
            {
                fm.RightToLeftLayout = false;
                fm.Opacity = 0d;
                tm.Interval = 10;   // 나타나는 속도(숫자가 작을수록 빨라짐)       
                tm.Tick += delegate (object obj, EventArgs e)
                {
                    if ((cnt + 1 > opacity.Length) || (fm == null))
                    {
                        tm.Stop();
                        tm.Dispose();
                        tm = null;
                        return;
                    }
                    else { fm.Opacity = opacity[cnt++]; }
                };
                tm.Start();
            }
        }

        private void Reset_Search()
        {
            is_searched = 0; search_result = null;
        }

        private void Find_Global_by_Sig(String dir)
        {
            if (is_searched == 1) { return; }
            bool isEqual = true;
            byte[] sig = new byte[4];
            String[] directories = Directory.GetDirectories(dir);
            {
                String[] files = Directory.GetFiles(dir);
                for (int i = 0; i < files.Length; i++)
                {
                    // Compare Signature
                    sig = File.ReadAllBytes(files[i]);
                    if(sig.Length >= 4)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (sig[j] != Signature[j])
                            {
                                isEqual = false;
                                break;
                            }
                        }
                        if (isEqual)
                        {
                            search_result = files[i];
                            is_searched = 1;
                            return;
                        }
                    }
                }
                for (int i = 0; i < directories.Length; i++)
                {
                    // Search again
                    Find_Global_by_Sig(directories[i]);
                }
            }
        }

        private void Directory_Search(String dir, String file_name)
        {
            if (is_searched == 1) { return; }
            String[] directories = Directory.GetDirectories(dir);
            {
                String[] files = Directory.GetFiles(dir);
                for (int i = 0; i < files.Length; i++)
                {
                    // Compare file name(global-metadata.dat)
                    if (files[i].Contains(file_name))
                    {
                        search_result = files[i];
                        is_searched = 1;
                        return;
                    }
                }
                for (int i = 0; i < directories.Length; i++)
                {
                    // Search again
                    Directory_Search(directories[i], file_name);
                }
            }
        }

        public int ADB_Check()
        {
            List<String> txt = new List<string>(); int is_using = 0;

            // adb check
            ProcessStartInfo proInfo = new ProcessStartInfo();
            Process current_pro = new Process();

            proInfo.FileName = @"cmd";
            proInfo.WorkingDirectory = @Application.StartupPath + "\\";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardOutput = true;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardError = true;
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
            return cnt;
        }

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

        private void decompile_apk(String apk, String path)
        {
            if (File.Exists(@Application.StartupPath + "\\Modules\\apktool\\apktool.bat"))
            {
                ProcessStartInfo proInfo = Set_Process("cmd", path, false);
                String cmd_str = "java -jar " + "\"" + @Application.StartupPath + "\\Modules\\apktool\\apktool.jar" + "\"" + " d ";
                cmd_str += "\"" + apk + "\""; cmd_str += " -f";
                Run_process(proInfo, cmd_str, false);
            }
            else
            {
                MessageBox.Show("[Error Code = 0x10]\n\napktool Not Found\n\nPlease check apktool.jar", "Error");
            }
        }

        private void il2cpp_dumper(String il2cpp, String metadata, String level0, String apk_name, String apk_path)
        {
            if (File.Exists(@Application.StartupPath + "\\Modules\\Il2CppDumper\\Il2CppDumper.exe"))
            {
                String dp = apk_path + apk_name + "_dump" + "_" + select_platform + "\\"; dump_path = dp;
                DirectoryInfo dir = new DirectoryInfo(@dp);
                if (dir.Exists)
                {
                    listView2.Items.Add("Already dumped");
                    if (MessageBox.Show(this, "Do you want to dump again?", "Check", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        dir.Delete(true);
                    }
                    else { return; }
                }
                ProcessStartInfo proInfo_mkdir = Set_Process("cmd", apk_path, false);
                String cmd_str_mkdir = "mkdir ";
                cmd_str_mkdir += "\"" + apk_name + "_dump" + "_" + select_platform + "\"";
                Run_process(proInfo_mkdir, cmd_str_mkdir, false);

                // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                listView2.Items.Add("Dumping...");
                String dump_direc = apk_path + "\\" + apk_name + "_dump" + "_" + select_platform;
                if (!Directory.Exists(dump_direc)) { MessageBox.Show(this, "[Error Code = 0x12]\n\nNot found directory", "Error"); return; }
                ProcessStartInfo proInfo_il2cpp = Set_Process("cmd", dump_direc, false);
                String cmd_il2cppdumper_str = "\"" + Application.StartupPath + "\\Modules\\Il2CppDumper\\Il2CppDumper.exe\" ";
                cmd_il2cppdumper_str += "\"" + metadata + "\" " + "\"" + il2cpp + "\" " + level0;
                Run_process(proInfo_il2cpp, cmd_il2cppdumper_str, false);
                if (!File.Exists(@dump_path + "\\dump.cs"))
                {
                    listView2.Items.Add("[Error Code = 0x00] Fail to Dump APK, the dumped file is not found");
                }
                else
                {
                    listView2.Items.Add("Sucess to Dump APK");
                }
            }
            else
            {
                MessageBox.Show(this, "[Error Code = 0x11]\n\nIl2CppDumper Not Found\n\nPlease check Il2", "Error");
            }
        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@ Buttons @@@@

        private void Button3_Click(object sender, EventArgs e) // Open file button
        {
            is_functions_ok = 0;
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Open an APK File";
            of.InitialDirectory = "C:\\";
            of.DefaultExt = "apk";
            of.Filter = "APK File(*.apk)|*.apk|모든 파일|*.*";
            String file_path = null;
            String libil2cpp_so_path = null;
            String lev0_path = null;
            int is_decompile = 0;
            if (of.ShowDialog() == DialogResult.OK)
            {
                if (of.FileName.Length > 0)
                {
                    is_mono = 0;
                    file_path = of.FileName;
                    apk_name = file_path.Split('\\')[file_path.Split('\\').Length - 1];
                    original_path_name = file_path.Replace(apk_name, "");

                    if (apk_name.Contains("xapk"))
                    {
                        MessageBox.Show(this, "XAPK is not supported\n\nPlease use APK", "Info");
                        return;
                    }

                    changed_apk_name = apk_name.Replace(".apk", "");
                    changed_path_name = file_path.Replace(".apk", "");

                    listView1.Items.Clear(); listView2.Items.Clear();
                    is_ok = 0;

                    // Select Platform: ARMv7 | ARM64 | x86
                    Select_platform Sf = new Select_platform();
                    if (Sf.ShowDialog() == DialogResult.OK) // package_name
                    {
                        is_android_ARM64 = Sf._is_android_ARM64; is_emulator = Sf._is_emulator;
                        is_android_ARMv7 = Sf._is_android_ARMv7;
                        if (is_android_ARM64 == 1) { select_platform = "arm64"; so_path = so_path_arm64v8a; listView2.Items.Add("ARM64 is selected"); }
                        else if (is_android_ARMv7 == 1) { select_platform = "armv7"; so_path = so_path_armeabiv7a; listView2.Items.Add("ARMv7 is selected"); }
                        else if (is_emulator == 1) { select_platform = "x86"; so_path = so_path_x86; listView2.Items.Add("x86 is selected"); }
                    }
                    else { return; }

                    ListViewItem newitem = new ListViewItem(new String[] { file_path, apk_name });
                    listView1.Items.Add(newitem);
                    manifest_path = changed_path_name + "\\AndroidManifest.xml";

                    package_label.Visible = false;
                    items.Clear();
                    listView3.Items.Clear();
                    String p = @changed_path_name + "\\lib\\";
                    DirectoryInfo dir2 = new DirectoryInfo(@changed_path_name);
                    DirectoryInfo d;
                    if (dir2.Exists)
                    {
                        listView2.Items.Add("Already decompiled");
                        if (MessageBox.Show(this, "Do you want to decompile again?", "Check", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        { is_decompile = 1; }
                        else { is_decompile = 0; }
                    }
                    else { is_decompile = 1; }
                    if (is_decompile == 1)
                    {
                        DirectoryInfo dir = new DirectoryInfo(@original_path_name + "\\" + changed_apk_name);
                        if (dir.Exists) { dir.Delete(true); }

                        listView2.Items.Add("Decompiling...");
                        try { decompile_apk(file_path, original_path_name); }
                        catch (Exception d1)
                        {
                            listView2.Items.Add("Fail to decompile apk");
                            MessageBox.Show(this, "Fail to decompile apk\n\n" + d1.ToString(), "Error");
                            return;
                        }
                        listView2.Items.Add("Success to Decompile this APK File");
                    }

                    DirectoryInfo cpn_dir = new DirectoryInfo(changed_path_name);
                    if (!cpn_dir.Exists)
                    {
                        MessageBox.Show(this, changed_path_name + "\n\nThe decompiled directory is not found or wrong\n\nPlease check your APK folder\n\nThe folder name must be equal to the APK name(except .apk)", "Error");
                        return;
                    }

                    // Il2CPP Check First
                    Directory_Search(changed_path_name + "\\", "libil2cpp.so");
                    if(search_result == null)
                    {
                        // MONO Check
                        Directory_Search(changed_path_name + "\\", "libmono.so");
                        if (search_result != null)
                        {
                            if (!File.Exists(manifest_path))
                            {
                                Reset_Search();
                                Directory_Search(changed_path_name + "\\", "AndroidManifest.xml");
                                if (search_result != null)
                                {
                                    manifest_path = search_result;
                                }
                                else
                                {
                                    MessageBox.Show(this, "[Error Code = 0x13]\n\nAndroidManifest.xml path ERROR\n\nPlease check AndroidManifest.xml", "Error");
                                    return;
                                }
                            }
                            byte[] inputs2 = new byte[1000];
                            inputs2 = File.ReadAllBytes(@manifest_path);
                            String str2 = Encoding.Default.GetString(inputs2);
                            String[] lines2 = str2.Split(' ');
                            foreach (var line in lines2)
                            {
                                if (line.Contains("package="))
                                {
                                    String temp = line.Substring(9);
                                    temp = temp.Replace("\"", "");
                                    temp = temp.Replace("<", ""); temp = temp.Replace(">", "");
                                    temp = temp.Replace("{", ""); temp = temp.Replace("}", "");
                                    temp = temp.Replace("(", ""); temp = temp.Replace(")", "");
                                    package_label.Text = temp; package_name = temp;
                                    break;
                                }
                            }
                            MessageBox.Show(this, "Open Mono APK\nYou can use\nTime\nData search\nReport", "Info");
                            Reset_Search();
                            package_label.Visible = true;
                            is_ok = 1;
                            is_mono = 1;
                            return;
                        }
                        else
                        {
                            MessageBox.Show(this, "This APK is not builded by Unity", "Error");
                            listView2.Items.Add("Fail to find so file(mono, il2cpp)\n\nThis APK is not builded by Unity"); return;
                        }
                    }
                    Reset_Search();

                    if (is_mono == 0)
                    {
                        if (is_android_ARM64 == 1)
                        {
                            p += "arm64-v8a";
                            d = new DirectoryInfo(p);
                            if (d.Exists == false)
                            {
                                listView2.Items.Add("Fail to dump apk");
                                MessageBox.Show(this, "Not found ARM64\n\nPlease select a correct platform", "Error"); return;
                            }
                        }
                        else if (is_android_ARMv7 == 1)
                        {
                            p += "armeabi-v7a";
                            d = new DirectoryInfo(p);
                            if (d.Exists == false)
                            {
                                listView2.Items.Add("Fail to dump apk");
                                MessageBox.Show(this, "Not found ARMv7\n\nPlease select a correct platform", "Error"); return;
                            }
                        }
                        else if (is_emulator == 1)
                        {
                            p += "x86";
                            d = new DirectoryInfo(p);
                            if (d.Exists == false)
                            {
                                listView2.Items.Add("Fail to dump apk");
                                MessageBox.Show(this, "Not found x86\n\nPlease select a correct platform", "Error"); return;
                            }
                        }

                        // Check libil2cpp.so
                        libil2cpp_so_path = changed_path_name + so_path;
                        if (File.Exists(libil2cpp_so_path)) { is_so_file = 1; }
                        else
                        {
                            Directory_Search(changed_path_name + "\\", "libil2cpp.so");
                            if (search_result != null)
                            {
                                libil2cpp_so_path = search_result;
                                is_so_file = 1;
                            }
                            else
                            {
                                MessageBox.Show(this, "Fail to Decompile this APK File, libil2cpp.so is not found", "Error");
                                listView2.Items.Add("Fail to Decompile this APK File, libil2cpp.so is not found"); return;
                            }
                            Reset_Search();
                        }
                        // if not exists, exit this function

                        // Check global-metadata.dat
                        metadata_path = changed_path_name + meta_path;
                        if (File.Exists(metadata_path)) { is_meta_file = 1; }
                        else
                        {
                            // 1. Find global-matadata.dat at the absolute path
                            Directory_Search(changed_path_name + "\\", "global-metadata.dat");
                            if (search_result != null)
                            {
                                metadata_path = search_result;
                                is_meta_file = 1;
                            }
                            else
                            {
                                // 2. Find global-matadata.dat in other directories
                                Directory_Search(changed_path_name + "\\", "*.dat");
                                if (search_result != null)
                                {
                                    metadata_path = search_result;
                                    is_meta_file = 1;
                                }
                                else
                                {
                                    // 3. Find metadata by signature
                                    Find_Global_by_Sig(changed_path_name + "\\");
                                    if(search_result != null)
                                    {
                                        metadata_path = search_result;
                                        is_meta_file = 1;
                                    }
                                    else
                                    {
                                        MessageBox.Show(this, "This APK is not builded by Unity", "Error");
                                        listView2.Items.Add("Fail to decompile this APK File, metadata file is not found"); return;
                                    }
                                }
                            }
                            Reset_Search();
                        }

                        // Check signature
                        bool isEqual = true;
                        byte[] sig = new byte[4];
                        sig = File.ReadAllBytes(metadata_path);
                        for (int i = 0; i < 4; i++)
                        {
                            if (sig[i] != Signature[i])
                            {
                                isEqual = false;
                                break;
                            }
                        }
                        if (isEqual) { is_correct_sig = 1; }
                        else { is_correct_sig = 0; }

                        // Check level0
                        lev0_path = changed_path_name + level0_path;
                        byte[] temp_lev0 = new byte[25]; byte[] lev0 = new byte[6];
                        if (File.Exists(lev0_path))
                        {
                            temp_lev0 = File.ReadAllBytes(lev0_path);
                        }
                        else
                        {
                            String lev0_split0_path = changed_path_name + level0_split0_path;
                            String udr_path = changed_path_name + unity_default_resources_path;
                            if (File.Exists(lev0_split0_path))
                            {
                                temp_lev0 = File.ReadAllBytes(lev0_split0_path);
                            }
                            else if (File.Exists(udr_path))
                            {
                                temp_lev0 = File.ReadAllBytes(udr_path);
                            }
                            else
                            {
                                Directory_Search(changed_path_name + "\\", "level0");
                                if (search_result != null)
                                {
                                    temp_lev0 = File.ReadAllBytes(search_result);
                                }
                                else
                                {
                                    Directory_Search(changed_path_name + "\\", "unity default resources");
                                    if (search_result != null)
                                    {
                                        temp_lev0 = File.ReadAllBytes(search_result);
                                    }
                                    else
                                    {
                                        MessageBox.Show(this, "[Error Code = 0x14]\n\nNot Found level0", "Error");
                                        return;
                                    }
                                }
                                Reset_Search();
                            }
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            lev0[i] = temp_lev0[20 + i];
                        }

                        level0 = Encoding.Default.GetString(lev0);
                        level0 = level0.Trim();
                        try { il2cpp_dumper(libil2cpp_so_path, metadata_path, level0, changed_apk_name, original_path_name); }
                        catch (Exception il2)
                        {
                            listView2.Items.Add("Fail to dump apk");
                            MessageBox.Show(this, "Fail to dump apk\n\n" + il2.ToString(), "Error");
                            return;
                        }

                        // Check AndroidManifes.xml
                        if (!File.Exists(manifest_path))
                        {
                            Directory_Search(changed_path_name + "\\", "AndroidManifest.xml");
                            if (search_result != null)
                            {
                                manifest_path = search_result;
                            }
                            else
                            {
                                MessageBox.Show(this, "[Error Code = 0x13]\n\nAndroidManifest.xml path ERROR\n\nPlease check AndroidManifest.xml", "Error");
                                return;
                            }
                            Reset_Search();
                        }
                        byte[] inputs = new byte[1000];
                        inputs = File.ReadAllBytes(@manifest_path);
                        String str = Encoding.Default.GetString(inputs);
                        String[] lines = str.Split(' ');
                        foreach (var line in lines)
                        {
                            if (line.Contains("package="))
                            {
                                String temp = line.Substring(9);
                                temp = temp.Replace("\"", "");
                                temp = temp.Replace("<", ""); temp = temp.Replace(">", "");
                                temp = temp.Replace("{", ""); temp = temp.Replace("}", "");
                                temp = temp.Replace("(", ""); temp = temp.Replace(")", "");
                                package_label.Text = temp; package_name = temp;
                                break;
                            }
                        }
                        package_label.Visible = true;
                        is_ok = 1;
                    }
                }
            }
        }

        private void Button4_Click(object sender, EventArgs e) // Report button
        {
            if (is_ok == 1)
            {
                this.Opacity = 0;
                this.Visible = false;
                Report_form Rf = new Report_form(apk_name, dump_path, package_name, level0, is_meta_exist, meta_f_list, h_list, changed_path_name, md_ok, T_status, R_status, items, table_list, itemTables, is_mono, select_platform, time_report, rand_report, metadata_path, is_click_md, is_hook_ok, ocr_list, is_ocr_runned, is_db_runned, is_mdf_ok, original_path_name);
                Rf.ShowDialog();
                List<String> temp_items = new List<String>();
                temp_items = Rf.get_items();
                for (int i = 0; i < temp_items.Count; i++)
                {
                    if (!items.Contains(temp_items[i]))
                    {
                        items.Add(temp_items[i]);
                    }
                }
                listView3.Items.Clear();
                int is_dup = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    is_dup = 0;
                    for (int j = 0; j < listView3.Items.Count; j++)
                    {
                        if (items[i].Equals(listView3.Items[j].SubItems[0].Text))
                        {
                            is_dup = 1;
                            break;
                        }
                    }
                    if (is_dup == 0) { listView3.Items.Add(items[i]); }
                }
                button4.BackColor = Color.FromArgb(64, 64, 64);
                pictureBox3.BackColor = Color.FromArgb(64, 64, 64);
                FormEffect(this);
                this.Visible = true;
            }
            else
            {
                MessageBox.Show(this, "APK is not decompiled", "Error");
            }
        }

        private void Button5_Click(object sender, EventArgs e) // Exit button
        {
            Application.Exit();
        }

        private void button2_Click_1(object sender, EventArgs e) // Meta data
        {
            if (is_mono == 1)
            {
                MessageBox.Show(this, "Mono can't use Meta data", "Info");
                return;
            }
            if (is_ok == 1)
            {
                this.Opacity = 0;
                this.Visible = false;
                Metadata_form Mdf = new Metadata_form(is_correct_sig, is_meta_file, package_name, select_platform);
                if (Mdf.ShowDialog() == DialogResult.OK)
                {
                    is_click_md = Mdf.get_is_click_md();
                    is_mdf_ok = Mdf.get_is_mdf_ok();
                    if (is_mdf_ok == 1)
                    {
                        try
                        {
                            is_meta_exist = Mdf.get_is_meta_exist();
                            is_md_ok = Mdf.get_is_md_ok();
                            md_ok.Clear();
                            md_ok.Add(is_correct_sig);
                            md_ok.Add(is_meta_file);
                            md_ok.Add(is_md_ok);
                            List<String> temp_items = new List<String>();
                            temp_items = Mdf.get_items();
                            for (int i = 0; i < temp_items.Count; i++)
                            {
                                if (!items.Contains(temp_items[i]))
                                {
                                    items.Add(temp_items[i]);
                                }
                            }
                            int is_dup = 0;
                            for (int i = 0; i < items.Count; i++)
                            {
                                is_dup = 0;
                                for (int j = 0; j < listView3.Items.Count; j++)
                                {
                                    if (items[i].Equals(listView3.Items[j].SubItems[0].Text))
                                    {
                                        is_dup = 1;
                                        break;
                                    }
                                }
                                if (is_dup == 0) { listView3.Items.Add(items[i]); }
                            }
                        }
                        catch (Exception mdf)
                        {
                            listView2.Items.Add("Metadata form Error - get_is_meta_exist()");
                            MessageBox.Show(this, "Metadata form Error\n\nget_is_meta_exist()\n\n" + mdf.ToString(), "Error");
                        }
                    }
                }
                button2.BackColor = Color.FromArgb(64, 64, 64);
                pictureBox5.BackColor = Color.FromArgb(64, 64, 64);
                FormEffect(this);
                this.Visible = true;
            }
            else { MessageBox.Show(this, "APK is not decompiled", "Error"); }
        }

        private void button6_Click_1(object sender, EventArgs e) // Dictionary
        {
            if (is_mono == 1)
            {
                MessageBox.Show(this, "Mono can't use Method check", "Info");
                return;
            }
            if (is_ok == 1)
            {
                if (!File.Exists(@dump_path + "\\dump.cs"))
                {
                    MessageBox.Show(this, "[Error Code = 0x00]\n\nThe dumped file is not found\n\nPlease dump again or check your APK", "Error");
                    return;
                }

                this.Opacity = 0;
                this.Visible = false;

                Method_check_form Df = new Method_check_form(dump_path, package_name, meta_f_list, h_list, select_platform);
                Df.ShowDialog();

                List<String> temp_items = new List<String>();
                temp_items = Df.get_items();
                for (int i = 0; i < temp_items.Count; i++)
                {
                    if (!items.Contains(temp_items[i]))
                    {
                        items.Add(temp_items[i]);
                    }
                }
                int is_dup = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    is_dup = 0;
                    for (int j = 0; j < listView3.Items.Count; j++)
                    {
                        if (items[i].Equals(listView3.Items[j].SubItems[0].Text))
                        {
                            is_dup = 1;
                            break;
                        }
                    }
                    if (is_dup == 0) { listView3.Items.Add(items[i]); }
                }

                try { meta_f_list = Df.get_meta_function_list(); }
                catch (Exception df)
                {
                    listView2.Items.Add("Method_check form Error - get_is_meta_function_list()");
                    MessageBox.Show(this, "Method_check form Error\n\nget_is_meta_function_list()\n\n" + df.ToString(), "Error");
                }
                try { is_hook_ok = Df.get_is_hook_ok(); }
                catch (Exception df)
                {
                    listView2.Items.Add("Method_check form Error - get_is_hook_ok()");
                    MessageBox.Show(this, "Method_check form Error\n\nget_is_hook_ok()\n\n" + df.ToString(), "Error");
                }
                try { h_list = Df.get_h_list(); }
                catch (Exception df)
                {
                    listView2.Items.Add("Method_check form Error - get_is_h_list()");
                    MessageBox.Show(this, "Method_check form Error\n\nget_is_h_list()\n\n" + df.ToString(), "Error");
                }
                button6.BackColor = Color.FromArgb(64, 64, 64);
                pictureBox7.BackColor = Color.FromArgb(64, 64, 64);
                FormEffect(this);
                this.Visible = true;
            }
            else { MessageBox.Show(this, "APK is not decompiled", "Error"); }
        }

        private void button7_Click_1(object sender, EventArgs e) // OCR
        {
            if (is_mono == 1)
            {
                MessageBox.Show(this, "Mono can't use OCR", "Info");
                return;
            }
            if (is_ok == 1)
            {
                int cnt = ADB_Check();
                if (cnt == 0) { MessageBox.Show(this, "Device or adb not found", "Error"); return; }
                else if (cnt > 0)
                {
                    this.Opacity = 0;
                    this.Visible = false;
                    OCR_form of = new OCR_form(dump_path, package_name);
                    of.ShowDialog();
                    List<String> temp_items = new List<String>();
                    temp_items = of.get_items();
                    ocr_list = of.get_items_list();
                    is_ocr_runned = of.get_is_ocr_runned();
                    for (int i = 0; i < temp_items.Count; i++)
                    {
                        if (!items.Contains(temp_items[i]))
                        {
                            items.Add(temp_items[i]);
                        }
                    }
                    int is_dup = 0;
                    for (int i = 0; i < items.Count; i++)
                    {
                        is_dup = 0;
                        for (int j = 0; j < listView3.Items.Count; j++)
                        {
                            if (items[i].Equals(listView3.Items[j].SubItems[0].Text))
                            {
                                is_dup = 1;
                                break;
                            }
                        }
                        if (is_dup == 0) { listView3.Items.Add(items[i]); }
                    }
                    button7.BackColor = Color.FromArgb(64, 64, 64);
                    pictureBox8.BackColor = Color.FromArgb(64, 64, 64);
                    FormEffect(this);
                    this.Visible = true;
                }
            }
            else { MessageBox.Show(this, "APK is not decompiled", "Error"); }
        }

        private void button8_Click_1(object sender, EventArgs e) // DB search
        {
            if (is_ok == 1)
            {
                int cnt = ADB_Check();
                if (cnt == 0) { MessageBox.Show(this, "Device or adb not found", "Error"); return; }
                else if (cnt > 0)
                {
                    this.Opacity = 0;
                    this.Visible = false;
                    Data_search_form DBf = new Data_search_form(changed_path_name, select_platform);
                    DBf.ShowDialog();
                    itemTables = DBf.get_data_table();
                    table_list = DBf.get_table_list();
                    is_db_runned = DBf.get_is_db_runned();

                    List<String> temp_items = new List<String>();
                    temp_items = DBf.get_items();
                    for (int i = 0; i < temp_items.Count; i++)
                    {
                        if (!items.Contains(temp_items[i]))
                        {
                            items.Add(temp_items[i]);
                        }
                    }
                    int is_dup = 0;
                    for (int i = 0; i < items.Count; i++)
                    {
                        is_dup = 0;
                        for (int j = 0; j < listView3.Items.Count; j++)
                        {
                            if (items[i].Equals(listView3.Items[j].SubItems[0].Text))
                            {
                                is_dup = 1;
                                break;
                            }
                        }
                        if (is_dup == 0) { listView3.Items.Add(items[i]); }
                    }
                    button8.BackColor = Color.FromArgb(64, 64, 64);
                    pictureBox6.BackColor = Color.FromArgb(64, 64, 64);
                    FormEffect(this);
                    this.Visible = true;
                }
            }
            else { MessageBox.Show(this, "APK is not decompiled", "Error"); }
        }

        private void button10_Click(object sender, EventArgs e) // Time / Random
        {
            int cnt = ADB_Check();
            if (cnt == 0) { MessageBox.Show(this, "Device or adb not found", "Error"); return; }
            else if (cnt > 0)
            {
                this.Opacity = 0;
                this.Visible = false;
                TR_form tf = new TR_form(dump_path, package_name, select_platform, is_mono);
                tf.ShowDialog();
                try
                {
                    time_report = tf.get_time_report();
                    rand_report = tf.get_rand_report();
                    T_status = tf.get_T_status();
                    R_status = tf.get_R_status();
                    List<String> temp_items = new List<String>();
                    temp_items = tf.get_items();
                    for (int i = 0; i < temp_items.Count; i++)
                    {
                        if (!items.Contains(temp_items[i]))
                        {
                            items.Add(temp_items[i]);
                        }
                    }
                    int is_dup = 0;
                    for (int i = 0; i < items.Count; i++)
                    {
                        is_dup = 0;
                        for (int j = 0; j < listView3.Items.Count; j++)
                        {
                            if (items[i].Equals(listView3.Items[j].SubItems[0].Text))
                            {
                                is_dup = 1;
                                break;
                            }
                        }
                        if (is_dup == 0) { listView3.Items.Add(items[i]); }
                    }
                }
                catch (Exception df)
                {
                    listView2.Items.Add("TR_form Error - get_TR_status()");
                    MessageBox.Show(this, "TR_form Error\n\nget_TR_status()\n\n" + df.ToString(), "Error");
                }
                button10.BackColor = Color.FromArgb(64, 64, 64);
                pictureBox12.BackColor = Color.FromArgb(64, 64, 64);
                FormEffect(this);
                this.Visible = true;
            }
        }

        private void button9_Click(object sender, EventArgs e) // Remove
        {
            if (listView3.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "This item will be removed\rContinue?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = listView3.FocusedItem.Index;
                    listView3.Items.RemoveAt(index);
                    items.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show(this, "Not selected\n\nPlease select an item", "Error");
            }
        }

        private void button1_Click(object sender, EventArgs e) // Reset
        {
            items.Clear();
            listView3.Items.Clear();
            MessageBox.Show(this, "Success to reset list", "Info");
        }


        private void button11_Click(object sender, EventArgs e) // Open APK folder
        {
            if (is_ok == 1)
            {
                if (Directory.Exists(@original_path_name))
                {
                    String file_path = @original_path_name;
                    System.Diagnostics.Process.Start(file_path);
                }
            }
            else
            {
                MessageBox.Show(this, "Please open APK first", "Error");
            }
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

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            button4.BackColor = Color.DodgerBlue;
            pictureBox3.BackColor = Color.DodgerBlue;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox3.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            button3.BackColor = Color.DodgerBlue;
            pictureBox4.BackColor = Color.DodgerBlue;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox4.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            button5.BackColor = Color.DodgerBlue;
            pictureBox9.BackColor = Color.DodgerBlue;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox9.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button10_MouseMove(object sender, MouseEventArgs e)
        {
            button10.BackColor = Color.DodgerBlue;
            pictureBox12.BackColor = Color.DodgerBlue;
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
            button10.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox12.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button2_MouseMove_1(object sender, MouseEventArgs e)
        {
            button2.BackColor = Color.DodgerBlue;
            pictureBox5.BackColor = Color.DodgerBlue;
        }

        private void button2_MouseLeave_1(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox5.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button6_MouseMove(object sender, MouseEventArgs e)
        {
            button6.BackColor = Color.DodgerBlue;
            pictureBox7.BackColor = Color.DodgerBlue;
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            button6.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox7.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button7_MouseMove(object sender, MouseEventArgs e)
        {
            button7.BackColor = Color.DodgerBlue;
            pictureBox8.BackColor = Color.DodgerBlue;
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            button7.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox8.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button8_MouseMove(object sender, MouseEventArgs e)
        {
            button8.BackColor = Color.DodgerBlue;
            pictureBox6.BackColor = Color.DodgerBlue;
        }

        private void button8_MouseLeave(object sender, EventArgs e)
        {
            button8.BackColor = Color.FromArgb(64, 64, 64);
            pictureBox6.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button9_MouseMove(object sender, MouseEventArgs e)
        {
            button9.BackColor = Color.DodgerBlue;
        }

        private void button9_MouseLeave(object sender, EventArgs e)
        {
            button9.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button11_MouseMove(object sender, MouseEventArgs e)
        {
            button11.BackColor = Color.DodgerBlue;
        }

        private void button11_MouseLeave(object sender, EventArgs e)
        {
            button11.BackColor = Color.FromArgb(64, 64, 64);
        }

        #endregion
    }
    // @@@@ Buttons @@@@
    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
}