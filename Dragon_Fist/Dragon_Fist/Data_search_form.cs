using System;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Diagnostics;
using System.Drawing;

namespace Dragon_Fist
{
    public partial class Data_search_form : Form
    {
        int is_runned = 0;
        int cnt;
        int cnt2;
        int treeIndex;
        String package_name = null;
        String manifest_path = null;
        String platform = null;
        DataSet superData_db = null;
        DataSet superData_pp = null;
        List<String> items = new List<String>();
        DataTable tmp = null;
        List<DataTable> itemsTables = new List<DataTable>();

        public Data_search_form(String _path, String _platform)
        {
            InitializeComponent();
            manifest_path = _path + "\\AndroidManifest.xml";
            platform = _platform;

            button1.TabStop = false; button1.FlatStyle = FlatStyle.Flat; button1.FlatAppearance.BorderSize = 0;
            button2.TabStop = false; button2.FlatStyle = FlatStyle.Flat; button2.FlatAppearance.BorderSize = 0;
            button3.TabStop = false; button3.FlatStyle = FlatStyle.Flat; button3.FlatAppearance.BorderSize = 0;
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;
            button6.TabStop = false; button6.FlatStyle = FlatStyle.Flat; button6.FlatAppearance.BorderSize = 0;
            button7.TabStop = false; button7.FlatStyle = FlatStyle.Flat; button7.FlatAppearance.BorderSize = 0;

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoResizeColumns();
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoResizeColumns();
            dataGridView2.ReadOnly = true;
            dataGridView2.AllowUserToAddRows = false;

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.MultiSelect = false;
            listView1.Columns.Add("Name", 800, HorizontalAlignment.Center);
        }

        public List<String> get_items() { return items; }

        public List<DataTable> get_data_table() { return itemsTables; }

        public List<String> get_table_list()
        {
            List<String> t_list = new List<String>();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                t_list.Add(listView1.Items[i].SubItems[0].ToString());
            }
            return t_list;
        }

        public int get_is_db_runned() { return is_runned; }

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

        public void Run_process(ProcessStartInfo pro_info, String cmd_str)
        {
            Process pro = new Process();
            pro.StartInfo = pro_info;
            pro.Start();
            pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            pro.StandardInput.Close();
            pro.WaitForExit();
            pro.Close();
        }

        private void ShowData1(DataTable selectedData)
        {
            if (selectedData.Rows.Count > 0)
            {
                dataGridView1.DataSource = selectedData;
            }
            else
            {
                MessageBox.Show(this, "No Tables");
            }
        }

        private void ShowData2(DataTable selectedData)
        {
            if (selectedData.Rows.Count > 0)
            {
                dataGridView2.DataSource = selectedData;
            }
            else
            {
                MessageBox.Show(this, "No Tables");
            }
        }

        private void parsePlayerPref(String dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                foreach (var file in directoryInfo.GetFiles())
                {
                    String filePath = dirPath + "\\" + file.Name;
                    DataSet ds = new DataSet();
                    ds.ReadXml(filePath);

                    TreeNode filename = new TreeNode(file.Name);

                    foreach (DataTable tb in ds.Tables)
                    {
                        //MessageBox.Show(tb.Columns[1].DataType.ToString());
                        TreeNode child = new TreeNode(tb.TableName);
                        child.Name = cnt2.ToString();
                        filename.Nodes.Add(child);

                        tb.TableName = file.Name + "@" + tb.TableName;
                        DataTable dtCopy = tb.Copy();
                        superData_pp.Tables.Add(dtCopy);
                        cnt2 += 1;
                    }
                    filename.Name = null;
                    treeView2.Nodes.Add(filename);
                }
            }
            else
            {
                MessageBox.Show(this, "Directory Not Exists!", "Error");
            }
        }

        private void getPlayerPref(String Path)
        {
            // goals adb pull /data/data/package_name/shared_prefs/* .
            ProcessStartInfo proInfo = Set_Process("cmd", Path, false);

            // remove old directory
            String cmd_str = "adb shell \"su -c 'rm -rf /data/local/tmp/playerpref'\"";
            Run_process(proInfo, cmd_str);

            // adb shell mkdir /data/local/tmp/playerpref"
            cmd_str = "adb shell mkdir /data/local/tmp/playerpref";
            Run_process(proInfo, cmd_str);

            // adb shell chmod 777 /data/local/tmp/playerpref"
            cmd_str = "adb shell chmod 777 /data/local/tmp/playerpref";
            Run_process(proInfo, cmd_str);

            // adb shell "su -c cp /data/data/package_name/shared_prefs/* /data/local/tmp/playerpref/"
            cmd_str = "adb shell \"su -c 'cp /data/data/" + package_name + "/shared_prefs/* /data/local/tmp/playerpref/'\"";
            Run_process(proInfo, cmd_str);

            // adb shell mkdir /data/local/tmp/playerpref
            cmd_str = "adb shell \"su -c 'chmod 744 /data/local/tmp/playerpref/*'\"";
            Run_process(proInfo, cmd_str);

            // adb shell mkdir /data/local/tmp/playerpref
            cmd_str = "adb pull /data/local/tmp/playerpref .";
            Run_process(proInfo, cmd_str);
        }

        private void readdbfile(String fileName, String dbPath)
        {

            List<String> list_table = new List<String>();

            // Get Table List
            String conn_str = "Data Source=" + dbPath + ";Version=3;";
            DataSet ds = new DataSet();

            String sql = "SELECT * FROM sqlite_master WHERE type='table';";

            var adpt = new SQLiteDataAdapter(sql, conn_str);
            adpt.Fill(ds);

            if (ds.Tables.Count > 0)
            {
                TreeNode filename = new TreeNode(fileName);
                filename.Name = null;
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    TreeNode child = new TreeNode(r["tbl_name"].ToString());
                    child.Name = treeIndex.ToString();
                    filename.Nodes.Add(child);
                    list_table.Add(r["tbl_name"].ToString());
                    treeIndex += 1;
                }
                treeView1.Nodes.Add(filename);
            }

            // Get Table
            foreach (String TableName in list_table)
            {
                sql = "SELECT * FROM " + TableName;
                adpt = new SQLiteDataAdapter(sql, conn_str);
                ds.Reset();
                adpt.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataTable table in ds.Tables)
                    {
                        table.TableName = fileName + "@" + TableName;
                        DataTable dtCopy = table.Copy();
                        superData_db.Tables.Add(dtCopy);
                        cnt += 1;
                    }
                }
            }
        }

        private void searchdb(String dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                foreach (var file in directoryInfo.GetFiles())
                {
                    String dbPath = dirPath + "\\" + file.Name;
                    readdbfile(file.Name, dbPath);
                }
            }
            else
            {
                MessageBox.Show(this, "Directory Not Exists!", "Error");
            }
        }

        private void pulldbfile(String PakageName, String Path)
        {
            ProcessStartInfo proInfo = Set_Process("cmd", Path, false);

            // adb push searchdb_arm64 /data/local/tmp
            String cmd_str = "adb push " + "\"" + Path + "\\searchdb_" + platform + "\"" + " /data/local/tmp/searchdb";
            Run_process(proInfo, cmd_str);

            // adb shell chmod 755 /data/local/tmp/searchdb
            cmd_str = "adb shell chmod 755 /data/local/tmp/searchdb";
            Run_process(proInfo, cmd_str);

            // adb shell \"su -c chown root:root /data/local/tmp/searchdb_arm64\"
            cmd_str = "adb shell \"su -c 'chown root:root /data/local/tmp/searchdb'\"";
            Run_process(proInfo, cmd_str);

            // adb shell rm -rf /data/local/tmp/db
            cmd_str = "adb shell \"su -c 'rm -rf /data/local/tmp/db'\"";
            Run_process(proInfo, cmd_str);

            // adb shell mkdir /data/local/tmp/db
            cmd_str = "adb shell mkdir /data/local/tmp/db";
            Run_process(proInfo, cmd_str);

            // adb shell chmod 777 /data/local/tmp/db
            cmd_str = "adb shell chmod 777 /data/local/tmp/db";
            Run_process(proInfo, cmd_str);

            // adb shell su -c /data/local/tmp/searchdb_arm64 /data/data/{PakageName}
            cmd_str = "adb shell \"su -c '/data/local/tmp/searchdb /data/data/" + PakageName + "/'\"";
            Run_process(proInfo, cmd_str);

            // adb shell su -c /data/local/tmp/searchdb_arm64 /data/data/{PakageName}
            cmd_str = "adb shell \"su -c '/data/local/tmp/searchdb /mnt/sdcard/Android/data/" + PakageName + "/'\"";
            Run_process(proInfo, cmd_str);

            // adb shell su -c /data/local/tmp/searchdb_arm64 /data/data/{PakageName}
            cmd_str = "adb shell \"su -c '/data/local/tmp/searchdb /storage/emulated/0/Android/data/" + PakageName + "/'\"";
            Run_process(proInfo, cmd_str);

            // adb pull /data/local/tmp/db.
            cmd_str = "adb pull /data/local/tmp/db " + "\"" + Path + "\\" + PakageName + "\\db\"";
            Run_process(proInfo, cmd_str);
        }

        private void Initializing()
        {
            superData_db = new DataSet();
            superData_pp = new DataSet();

            cnt = 0;
            cnt2 = 0;
            treeIndex = 0;

            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();

            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
        }

        private void DBsearch_form_Load(object sender, EventArgs e) // Bring package info
        {
            if (manifest_path == null)
            {
                MessageBox.Show(this, "AndroidManifest.xml path ERROR", "Error");
                return;
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
                    package_name = temp;
                    break;
                }
            }
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

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2.BackColor = Color.DodgerBlue;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.BackColor = Color.DodgerBlue;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button6_MouseMove(object sender, MouseEventArgs e)
        {
            button6.BackColor = Color.DodgerBlue;
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            button6.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button7_MouseMove(object sender, MouseEventArgs e)
        {
            button7.BackColor = Color.DodgerBlue;
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            button7.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            button4.BackColor = Color.DodgerBlue;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(64, 64, 64);
        }

        #endregion

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Node.Name))
            {
                ShowData1(superData_db.Tables[int.Parse(e.Node.Name)]);
                tmp = superData_db.Tables[int.Parse(e.Node.Name)];
            }
        }

        private void treeView2_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Node.Name))
            {
                ShowData2(superData_pp.Tables[int.Parse(e.Node.Name)]);
                tmp = superData_pp.Tables[int.Parse(e.Node.Name)];
            }
        }


        private void DBsearch_form_Shown(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            ;
        }

        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            ;
        }

        // Buttons

        private void button4_Click(object sender, EventArgs e) // Reset
        {
            listView1.Clear();
            itemsTables.Clear();
            MessageBox.Show(this, "Succes to reset the list", "Info");
        }

        private void button1_Click(object sender, EventArgs e) // Back
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
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

            if (cnt == 0) { MessageBox.Show(this, "Device or adb not found", "Error"); return; }
            else if (cnt > 0)
            {
                Initializing();

                DirectoryInfo dbDir = new DirectoryInfo(@Application.StartupPath + "\\Modules\\searchDB\\" + package_name);
                if (!dbDir.Exists) dbDir.Create();
                else
                {
                    Directory.Delete(dbDir.FullName, true);
                    dbDir.Create();
                }

                pulldbfile(package_name, @Application.StartupPath + "\\Modules\\searchDB");
                searchdb(@Application.StartupPath + "\\Modules\\searchDB\\" + package_name + "\\db");

                getPlayerPref(@Application.StartupPath + "\\Modules\\searchDB\\" + package_name);
                parsePlayerPref(@Application.StartupPath + "\\Modules\\searchDB\\" + package_name + "\\playerpref");

                is_runned = 1;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (is_runned == 0)
            {
                MessageBox.Show(this, "Please click run button", "Error");
                return;
            }
            if(itemsTables.Count == 0)
            {
                MessageBox.Show(this, "Please add one more tables", "Error");
                return;
            }
            items.Clear();
            items.Add("Data search");

            MessageBox.Show(this, "Success to add to report", "Info");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tmp != null)
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    if (tmp.TableName.Equals(listView1.Items[i].SubItems[0].Text))
                    {
                        MessageBox.Show(this, "Duplicate Error", "Error");
                        return;
                    }
                }
                listView1.Items.Add(tmp.TableName);
                itemsTables.Add(tmp);
                tmp = null;
            }
            else
            {
                MessageBox.Show(this, "Please select a table", "Error");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "This item will be removed\rContinue?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = listView1.FocusedItem.Index;
                    listView1.Items.RemoveAt(index);
                    itemsTables.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show(this, "Please select a table", "Error");
            }
        }
    }
}