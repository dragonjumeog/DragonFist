using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Diagnostics;

namespace Dragon_Fist
{
    public partial class Method_check_form : Form
    {
        int is_runned = 0;
        int cont = 0;
        int is_hook_ok = 0;
        List<String> user_dic = new List<String>();
        List<String> filter_list = new List<String>();
        List<List<String>> meta_function_list = new List<List<String>>();
        List<List<String>> hooking_list = new List<List<String>>();
        List<List<String>> h_list = new List<List<String>>();
        String dump_path = null; String package_name = null; String platform = null;
        List<String> items = new List<String>();

        private void Dictionary_form_Load(object sender, EventArgs e)
        {
            String[] tmp = File.ReadAllLines(@Application.StartupPath + "\\Dictionary.txt");
            for (int i = 0; i < tmp.Length; i++)
            {
                user_dic.Add(tmp[i]);
                listView2.Items.Add(tmp[i]);
            }
        }

        public List<String> get_items() { return items; }

        public List<List<String>> get_meta_function_list() { return meta_function_list; }
        public List<List<String>> get_h_list() { return h_list; }

        public int get_is_hook_ok() { return is_hook_ok; }

        struct result_dict
        {
            public String classname;
            public String name;
            public String RVA;
            public String offset;

            public result_dict(string v1, string v2, string v3, string v4) : this()
            {
                this.classname = v1;
                this.name = v2;
                this.RVA = v3;
                this.offset = v4;
            }
        }

        List<result_dict> result = new List<result_dict>();

        public Method_check_form(String _dump_path, String _package_name, List<List<String>> _meta_function_list, List<List<String>> _h_list, String _platform)
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.MultiSelect = false;
            listView1.Columns.Add("Class", 220, HorizontalAlignment.Center);
            listView1.Columns.Add("Method", 220, HorizontalAlignment.Center);
            listView1.Columns.Add("RVA", 170, HorizontalAlignment.Center);
            listView1.Columns.Add("Offset", 170, HorizontalAlignment.Center);

            listView2.View = View.Details;
            listView2.FullRowSelect = true;
            listView2.GridLines = true;
            listView2.MultiSelect = false;
            listView2.Columns.Add("Name", 500, HorizontalAlignment.Center);

            listView3.View = View.Details;
            listView3.FullRowSelect = true;
            listView3.GridLines = true;
            listView3.MultiSelect = false;
            listView3.Columns.Add("Name", 500, HorizontalAlignment.Center);

            listView4.View = View.Details;
            listView4.FullRowSelect = true;
            listView4.GridLines = true;
            listView4.MultiSelect = false;
            listView4.Columns.Add("Class", 390, HorizontalAlignment.Center);
            listView4.Columns.Add("Method", 390, HorizontalAlignment.Center);

            dump_path = _dump_path;
            package_name = _package_name;
            platform = _platform;

            if(_meta_function_list.Count > 0) {
                meta_function_list = _meta_function_list;
                hooking_list = meta_function_list;
                ListViewItem newitem;
                for(int i = 0; i < hooking_list.Count; i++)
                {
                    newitem = new ListViewItem(new String[] { hooking_list[i][0], hooking_list[i][1] });
                    listView4.Items.Add(newitem);
                }
            }

            if(_h_list.Count > 0)
            {
                h_list = _h_list;
            }

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
            button12.TabStop = false; button12.FlatStyle = FlatStyle.Flat; button12.FlatAppearance.BorderSize = 0;
            button13.TabStop = false; button13.FlatStyle = FlatStyle.Flat; button13.FlatAppearance.BorderSize = 0;
            button14.TabStop = false; button14.FlatStyle = FlatStyle.Flat; button14.FlatAppearance.BorderSize = 0;
            button16.TabStop = false; button16.FlatStyle = FlatStyle.Flat; button16.FlatAppearance.BorderSize = 0;
        }

        private void FormEffect(Form fm)
        {
            double[] opacity = new double[] { 0.1d, 0.2d, 0.3d, 0.5d, 0.7d, 0.8d, 0.9d, 1.0d };
            int cnt = 0;
            Timer tm = new Timer();
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

        private void search_dumpcs(List<String> dictionary, List<result_dict> result)
        {
            String[] lines = File.ReadAllLines(@dump_path + "dump.cs");
            String[] ing;
            String _class = "NULL";
            int is_dup = 0;

            foreach (String line in lines)
            {
                if (line.Contains("TypeDefIndex"))
                {
                    _class = line.Split('/')[0];
                }
                if (!line.Contains("RVA") || !line.Contains('{'))
                    continue;
                ing = line.Split('/');
                for (int i = 0; i < dictionary.Count; i++)
                {
                    is_dup = 0;
                    for (int j = 0; j < result.Count; j++)
                    {
                        if (result[j].name.Contains(ing[0].Split(')')[0]))
                        {
                            is_dup = 1;
                            break;
                        }
                    }
                    if (is_dup == 0)
                    {
                        if (ing[0].Contains(dictionary[i].ToLower()) || ing[0].Contains(dictionary[i]))
                        {
                            result_dict _res = new result_dict(_class, ing[0].Split(')')[0], ing[2].Split(':')[1].Split(' ')[1], ing[2].Split(':')[2].Trim());
                            result.Add(_res);
                        }
                    }
                }
            }
        }

        // [ String name | int RVA | int offset | long line_num ]
        private void Add_items_listView1() // Add items listView2 function
        {
            listView1.Items.Clear();
            ListViewItem newitem;
            for (int i = 0; i < result.Count; i++)
            {
                if (!result[i].name.Contains(")")) { newitem = new ListViewItem(new String[] { result[i].classname, result[i].name + ")", result[i].RVA, result[i].offset, }); }
                else { newitem = new ListViewItem(new String[] { result[i].classname, result[i].name, result[i].RVA, result[i].offset }); }
                listView1.Items.Add(newitem);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(@dump_path, "Info");
            if (!File.Exists(@dump_path + "\\dump.cs"))
            {
                MessageBox.Show(this, "[Error Code = 0x00]\n\nThe dumped file is not found\n\nPlease dump again or check your APK", "Error");
                return;
            }
            if (user_dic.Count == 0)
            {
                MessageBox.Show(this, "No Data\n\nPlease add one more items", "Error");
                return;
            }
            try
            {
                result.Clear();
                search_dumpcs(user_dic, result);
                Add_items_listView1();
            }
            catch (Exception button2)
            {
                MessageBox.Show(button2.ToString(), "Error");
            }
        }

        private void button8_Click_1(object sender, EventArgs e) // Filter button
        {
            if (filter_list.Count == 0)
            {
                MessageBox.Show(this, "No Filter Data, Please make a list", "Error");
                return;
            }
            result.Clear();
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                for (int j = 0; j < filter_list.Count; j++)
                {
                    if (listView1.Items[i].SubItems[1].Text.Contains(filter_list[j]))
                    {
                        result_dict _res = new result_dict(listView1.Items[i].SubItems[0].Text, listView1.Items[i].SubItems[1].Text, listView1.Items[i].SubItems[2].Text, listView1.Items[i].SubItems[3].Text);
                        result.Add(_res);
                    }
                }
            }
            result = result.Distinct().ToList();
            Add_items_listView1();
        }

        private void button5_Click_1(object sender, EventArgs e) // Save button
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show(this, "No Log here, Please Execute", "Error");
                return;
            }

            String item = null;
            using (StreamWriter outputFile = new StreamWriter(@"Searched Dictionary list.txt", true))
            {
                outputFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                foreach (ListViewItem it in listView1.Items)
                {
                    for (int i = 0; i < it.SubItems.Count; i++)
                    {
                        item = it.SubItems[i].Text.Trim();
                        item = item.Replace("ListViewItem: ", "");
                        item = item.Replace("{", ""); item = item.Replace("}", "");
                        outputFile.Write(item);
                        outputFile.Write("; ");
                    }
                    outputFile.WriteLine("");
                }
            }
            MessageBox.Show(this, "Success to save log", "Info");
        }

        private void button1_Click_1(object sender, EventArgs e) // listView2 Enter button
        {
            String input = textBox1.Text;
            if (input.Length <= 0) { MessageBox.Show(this, "No input, Please enter items", "Error"); return; }
            for (int i = 0; i < user_dic.Count; i++) { if (user_dic[i].Equals(input)) { MessageBox.Show(this, "Duplicate Error", "Error"); return; } }
            user_dic.Add(input);
            listView2.Items.Clear();
            for (int i = 0; i < user_dic.Count; i++) { listView2.Items.Add(user_dic[i]); }
            textBox1.Text = "";
        }

        private void button6_Click_1(object sender, EventArgs e) // listView2 Select Remove button
        {
            if (listView2.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "This item will be removed\rContinue?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = listView2.FocusedItem.Index;
                    listView2.Items.RemoveAt(index);
                    user_dic.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show(this, "Not selected", "Error");
            }
        }

        private void button7_Click_1(object sender, EventArgs e) // listView2 Reset button
        {
            user_dic.Clear();
            listView2.Items.Clear();
            MessageBox.Show(this, "Success to reset list", "Info");
        }

        private void button4_Click_1(object sender, EventArgs e) // Back button
        {
            this.Close();
        }

        private void button9_Click(object sender, EventArgs e) // Filter enter button
        {
            String input = textBox2.Text;
            if (input.Length <= 0) { MessageBox.Show(this, "No input, Please enter items", "Error"); return; }
            for (int i = 0; i < filter_list.Count; i++) { if (filter_list[i].Equals(input)) { MessageBox.Show(this, "Duplicate Error", "Error"); return; } }
            filter_list.Add(input);
            listView3.Items.Clear();
            for (int i = 0; i < filter_list.Count; i++) { listView3.Items.Add(filter_list[i]); }
            textBox2.Text = "";
        }

        private void button11_Click(object sender, EventArgs e) // Filter remove button
        {
            if (listView3.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "This item will be removed\rContinue?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = listView3.FocusedItem.Index;
                    listView3.Items.RemoveAt(index);
                    filter_list.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show(this, "Not selected\n\nPlease select an item", "Error");
            }
        }

        private void button10_Click(object sender, EventArgs e) // Filter reset button
        {
            filter_list.Clear();
            listView3.Items.Clear();
            MessageBox.Show(this, "Success to reset list", "Info");
        }

        private void button14_Click(object sender, EventArgs e) // listview4 Exec
        {
            try
            {
                if (hooking_list.Count == 0) { MessageBox.Show(this, "Please select more than one method", "Error"); return; }

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

                if (cnt == 0) { MessageBox.Show(this, "Device or adb not found", "Error"); return; }
                else if (cnt > 0)
                {
                    this.Opacity = 0;
                    this.Visible = false;
                    Hook_form Hf = new Hook_form(hooking_list, package_name, h_list, dump_path, platform);
                    Hf.ShowDialog();
                    h_list = Hf.get_h_list();
                    is_hook_ok = Hf.get_is_hook_ok();
                    FormEffect(this);
                    this.Visible = true;
                }
                is_runned = 1;
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.ToString(), "Error");
            }
        }

        private void button13_Click(object sender, EventArgs e) // listview4 reset
        {
            hooking_list.Clear();
            listView4.Items.Clear();
            MessageBox.Show(this, "Success to reset list", "Info");
        }

        private void button12_Click(object sender, EventArgs e) // listview4 remove
        {
            if (listView4.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "This function will be removed\rContinue?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = listView4.FocusedItem.Index;
                    listView4.Items.RemoveAt(index);
                    hooking_list.RemoveAt(index);
                    meta_function_list.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show(this, "Not selected\n\nPlease select a function", "Error");
            }
        }

        private void button16_Click(object sender, EventArgs e) // listview4 add
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int index = listView1.FocusedItem.Index;
                ListViewItem newitem;
                for (int i = 0; i < hooking_list.Count; i++)
                {
                    if (hooking_list[i][0].Equals(listView1.Items[index].SubItems[0].Text) &&
                        hooking_list[i][1].Equals(listView1.Items[index].SubItems[1].Text))
                    {
                        MessageBox.Show(this, "Duplicate Error", "Error"); return;
                    }
                }
                List<String> temp = new List<String>();
                for (int j = 0; j < 4; j++) { temp.Add(listView1.Items[index].SubItems[j].Text); }
                hooking_list.Add(temp);
                newitem = new ListViewItem(new String[] { temp[0], temp[1] });
                listView4.Items.Add(newitem);
                List<String> temp2 = new List<String>();
                temp2.Add(temp[0]); temp2.Add(temp[1]); temp2.Add(temp[2]); temp2.Add(temp[3]);
                meta_function_list.Add(temp2); // save class, method
            }
            else
            {
                MessageBox.Show(this, "Not selected\n\nPlease select a function", "Error");
            }
        }

        #region Button Color

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

        private void button8_MouseMove(object sender, MouseEventArgs e)
        {
            button8.BackColor = Color.DodgerBlue;
        }

        private void button8_MouseLeave(object sender, EventArgs e)
        {
            button8.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button16_MouseMove(object sender, MouseEventArgs e)
        {
            button16.BackColor = Color.DodgerBlue;
        }

        private void button16_MouseLeave(object sender, EventArgs e)
        {
            button16.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            button5.BackColor = Color.DodgerBlue;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.BackColor = Color.FromArgb(64, 64, 64);
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

        private void button14_MouseMove(object sender, MouseEventArgs e)
        {
            button14.BackColor = Color.DodgerBlue;
        }

        private void button14_MouseLeave(object sender, EventArgs e)
        {
            button14.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button12_MouseMove(object sender, MouseEventArgs e)
        {
            button12.BackColor = Color.DodgerBlue;
        }

        private void button12_MouseLeave(object sender, EventArgs e)
        {
            button12.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button13_MouseMove(object sender, MouseEventArgs e)
        {
            button13.BackColor = Color.DodgerBlue;
        }

        private void button13_MouseLeave(object sender, EventArgs e)
        {
            button13.BackColor = Color.FromArgb(64, 64, 64);
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

        private void button10_MouseMove(object sender, MouseEventArgs e)
        {
            button10.BackColor = Color.DodgerBlue;
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
            button10.BackColor = Color.FromArgb(64, 64, 64);
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

        private void button3_Click(object sender, EventArgs e) // Add to Report
        {
            if (is_runned == 0)
            {
                MessageBox.Show(this, "Please click hooking run button", "Error");
                return;
            }
            items.Clear();
            items.Add("Method check");
            MessageBox.Show(this, "Success to add to report", "Info");
        }
    }
}
