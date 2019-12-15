using System;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using Word = Microsoft.Office.Interop.Word;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Dragon_Fist
{
    public partial class Report_form : Form
    {
        int is_make = 0;
        String apk_name = null;
        String dump_path = null;
        String package_name = null;
        String level0 = null;
        String xml_path = null;
        int is_meta = 0;
        int is_dic = 0;
        int is_memory = 0;
        int is_db = 0;
        int is_random = 0;
        int is_speed = 0;
        int is_meta_exist = 0;
        int hooking_list_size = 0;
        int h_list_size = 0;
        List<int> is_md_ok = new List<int>();
        List<int> T_status = new List<int>();
        List<int> R_status = new List<int>();
        List<List<String>> meta_f_list = new List<List<string>>();
        List<List<String>> h_list = new List<List<string>>();

        List<List<String>> Result = new List<List<String>>();
        List<String> manifest = new List<String>(); // package name
        List<String> application = new List<String>(); // debuggable
        List<String> permission = new List<String>(); // permission
        String sdk_version = null;
        String sdk_code_name = null;
        String platform_b_v_code = null;
        String platform_b_v_name = null;
        String debuggable = null;
        List<String> pm = new List<String>();
        List<String> items = new List<String>();
        StringBuilder sb_pm = new StringBuilder();

        List<String> table_list = new List<String>();
        List<DataTable> itemTables = new List<DataTable>();

        List<String> time_report = new List<String>();
        List<String> rand_report = new List<String>();

        List<List<String>> ocr_list = new List<List<String>>();

        String select_platform = null;
        String metadata_path = null;
        String original_path_name = null;

        int is_click_md = 0;
        int is_mono = 0;
        int is_hook_ok = 0;
        int is_ocr_runned = 0;
        int is_db_runned = 0;
        int is_mdf_ok = 0;

        public Report_form(String _apk_name, String _dump_path, String _package_name, String _level0, int _is_meta_exist, List<List<String>> _meta_f_list, List<List<String>> _h_list, String _xml_path, List<int> _is_md_ok, List<int> _T_status, List<int> _R_status, List<String> _items, List<String> _table_list, List<DataTable> _itemTables, int _is_mono, String _select_platform, List<String> _time_report, List<String> _rand_report, String _metadata_path, int _is_click_md, int _is_hook_ok, List<List<String>> _ocr_list, int _is_ocr_runned, int _is_db_runned, int _is_mdf_ok, String _original_path_name)
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("Details", 1000, HorizontalAlignment.Center);

            items = _items;
            for(int i = 0; i < items.Count; i++)
            {
                if (items[i].Equals("Meta data")) { is_meta = 1; }
                else if (items[i].Equals("Time")) { is_speed = 1; }
                else if (items[i].Equals("Random")) { is_random = 1; }
                else if (items[i].Equals("Method check")) { is_dic = 1; }
                else if (items[i].Equals("OCR")) { is_memory = 1; }
                else if (items[i].Equals("Data search")) { is_db = 1; }
            }
            if (is_meta == 1) { metadata_check.Checked = true; label6.BackColor = Color.Green; }
            else if(is_meta == 0) { metadata_check.Checked = false; label6.BackColor = Color.Red; }
            if (is_dic == 1) { dictionary_check.Checked = true; label1.BackColor = Color.Green; }
            else if (is_dic == 0) { dictionary_check.Checked = false; label1.BackColor = Color.Red; }
            if (is_memory == 1) { library_check.Checked = true; label4.BackColor = Color.Green; }
            else if (is_memory == 0) { library_check.Checked = false; label4.BackColor = Color.Red; }
            if (is_db == 1) { DBsearch_check.Checked = true; label7.BackColor = Color.Green; }
            else if (is_db == 0) { DBsearch_check.Checked = false; label7.BackColor = Color.Red; }
            if(is_random == 1) { random_checkbox.Checked = true; label8.BackColor = Color.Green; }
            else if (is_random == 0) { random_checkbox.Checked = false; label8.BackColor = Color.Red; }
            if(is_speed == 1) { speed_checkbox.Checked = true; label9.BackColor = Color.Green; }
            else if (is_speed == 0) { speed_checkbox.Checked = false; label9.BackColor = Color.Red; }
            apk_name = _apk_name;
            dump_path = _dump_path;
            package_name = _package_name;
            level0 = _level0;
            is_meta_exist = _is_meta_exist;
            meta_f_list = _meta_f_list;
            h_list = _h_list;
            xml_path = _xml_path; xml_path += "\\AndroidManifest.xml";
            hooking_list_size = meta_f_list.Count;
            h_list_size = h_list.Count;
            is_md_ok = _is_md_ok; // is_sig_correct, is_meta_file, is_md_ok
            T_status = _T_status;
            R_status = _R_status;
            table_list = _table_list;
            itemTables = _itemTables;
            is_mono = _is_mono;
            select_platform = _select_platform;
            time_report = _time_report;
            rand_report = _rand_report;
            metadata_path = _metadata_path;
            is_click_md = _is_click_md;
            is_hook_ok = _is_hook_ok;
            ocr_list = _ocr_list;
            is_ocr_runned = _is_ocr_runned;
            is_db_runned = _is_db_runned;
            is_mdf_ok = _is_mdf_ok;
            original_path_name = _original_path_name;

            if (is_mono == 0)
            {
                ParseManifest(@xml_path);
                for (int i = 0; i < Result.Count; i++)
                {
                    if (i == 0) // manifest(package name, sdk version, sdk code name, platform build version code, platform build version name)
                    {
                        sdk_version = Result[i][1];
                        sdk_code_name = Result[i][2];
                        platform_b_v_code = Result[i][3];
                        platform_b_v_name = Result[i][4];
                    }
                    else if (i == 1) // debuggable
                    {
                        debuggable = Result[i][0];
                    }
                    else if (i == 2) // permission
                    {
                        pm = Result[i];
                        for (int j = 0; j < pm.Count; j++)
                        {
                            if ((j + 1) != pm.Count) { sb_pm.Append(pm[j] + "\n"); }
                            else { sb_pm.Append(pm[j]); }
                        }
                    }
                }
            }

            button1.TabStop = false; button1.FlatStyle = FlatStyle.Flat; button1.FlatAppearance.BorderSize = 0;
            button3.TabStop = false; button3.FlatStyle = FlatStyle.Flat; button3.FlatAppearance.BorderSize = 0;
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;

            if (File.Exists(@original_path_name + "\\Report_" + apk_name + "_" + select_platform + ".docx"))
            {
                is_make = 1;
            }
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

        private void ParseManifest(String filePath)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(filePath);

            foreach (DataTable tb in ds.Tables)
            {
                DataTable dtCopy = tb.Copy();
                if (tb.TableName == "manifest")
                {
                    manifest.Add(tb.Rows[0]["package"].ToString());
                    manifest.Add(tb.Rows[0]["compileSdkVersion"].ToString());
                    manifest.Add(tb.Rows[0]["compileSdkVersionCodename"].ToString());
                    manifest.Add(tb.Rows[0]["platformBuildVersionCode"].ToString());
                    manifest.Add(tb.Rows[0]["platformBuildVersionName"].ToString());
                }
                else if (tb.TableName == "application")
                {
                    if (tb.Columns.Contains("debuggable"))
                    {
                        application.Add(tb.Rows[0]["debuggable"].ToString());
                    }
                    application.Add("false");
                }
                else if (tb.TableName == "uses-permission")
                {
                    foreach (DataRow tr in tb.Rows)
                    {
                        permission.Add(tr[0].ToString());
                    }
                }
            }
            Result.Add(manifest);
            Result.Add(application);
            Result.Add(permission);
        }

        public List<String> get_items() { return items; }

        public void Make_Word_Report()
        {
            int para_num = 1; int a = 0;
            listView1.Items.Add("making...");
            String t = DateTime.Now.ToString("yyyy/MM/dd  HH:mm:ss");
            object end_of_doc = "\\endofdoc";
            //object path = @Application.StartupPath + "\\Modules\\Basic_template.docx";
            object obj_miss = System.Reflection.Missing.Value;
            Word.Application word = new Word.Application();
            Word.Document word_doc = new Word.Document();
            word.Visible = false;
            word.WindowState = Word.WdWindowState.wdWindowStateNormal;

            // space
            Word.Paragraph space_first = word_doc.Paragraphs.Add(ref obj_miss);
            space_first.Range.Font.Bold = 0;
            space_first.Range.Font.Size = 10;
            space_first.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            space_first.Range.Text = "";
            space_first.Format.SpaceAfter = 30;
            space_first.Range.InsertParagraphAfter();

            //word_doc = word.Documents.Open(ref path, ref obj_miss, ref obj_miss, ref obj_miss);

            // apk name #1
            Word.Paragraph para_apk = word_doc.Paragraphs.Add(ref obj_miss);
            para_apk.Range.Font.Size = 26;
            para_apk.Range.Font.Color = Word.WdColor.wdColorGray50;
            para_apk.Range.Font.Bold = 1;
            para_apk.Range.Text = apk_name;
            para_apk.Format.SpaceAfter = 15;
            para_apk.Range.InsertParagraphAfter();

            Word.Paragraph para_apk_1_1 = word_doc.Paragraphs.Add(ref obj_miss);
            para_apk_1_1.Range.Font.Size = 36;
            para_apk_1_1.Range.Font.Color = Word.WdColor.wdColorBlueGray;
            para_apk_1_1.Range.Font.Bold = 1;
            para_apk_1_1.Range.Text = "Game Service";
            para_apk_1_1.Format.SpaceAfter = 10;
            para_apk_1_1.Range.InsertParagraphAfter();

            Word.Paragraph para_apk_1_2 = word_doc.Paragraphs.Add(ref obj_miss);
            para_apk_1_2.Range.Font.Size = 36;
            para_apk_1_2.Range.Font.Color = Word.WdColor.wdColorBlueGray;
            para_apk_1_2.Range.Font.Bold = 1;
            para_apk_1_2.Range.Text = "Vulnerability scanning";
            para_apk_1_2.Format.SpaceAfter = 10;
            para_apk_1_2.Range.InsertParagraphAfter();

            Word.Paragraph para_apk_1_3 = word_doc.Paragraphs.Add(ref obj_miss);
            para_apk_1_3.Range.Font.Size = 36;
            para_apk_1_3.Range.Font.Color = Word.WdColor.wdColorBlueGray;
            para_apk_1_3.Range.Font.Bold = 1;
            para_apk_1_3.Range.Text = "Report";
            para_apk_1_3.Format.SpaceAfter = 300;
            para_apk_1_3.Range.InsertParagraphAfter();

            Word.Paragraph para_apk_2 = word_doc.Paragraphs.Add(ref obj_miss);
            para_apk_2.Range.Font.Size = 10;
            para_apk_2.Range.Font.Bold = 1;
            para_apk_2.Range.Text = t + " 생성됨";
            para_apk_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            para_apk_2.Format.SpaceAfter = 150;
            para_apk_2.Range.InsertParagraphAfter();

            // title #2
            Word.Paragraph para_title_2 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_2.Range.Text = "Scanning report summary";
            para_title_2.Range.Font.Bold = 1;
            para_title_2.Range.Font.Size = 20;
            para_title_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            para_title_2.Format.SpaceAfter = 1;
            //para_title_2.Range.InsertParagraphAfter();

            // table 0-1 #2
            Word.Table table_0_1;
            Word.Range rng_0_1 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_0_1 = word_doc.Tables.Add(rng_0_1, 2, 2, ref obj_miss, ref obj_miss);
            table_0_1.Range.Font.Size = 10;
            table_0_1.Range.Font.Bold = 0;
            table_0_1.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_0_1.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_0_1.AllowAutoFit = false;

            table_0_1.Cell(1, 1).Width = 40;
            table_0_1.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_1.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_0_1.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_0_1.Cell(1, 1).Range.Font.Bold = 1;
            table_0_1.Cell(1, 1).Range.Text = "개요";

            table_0_1.Cell(2, 1).Width = 40;
            table_0_1.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_1.Cell(2, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_0_1.Cell(2, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_0_1.Cell(2, 1).Range.Font.Bold = 1;
            table_0_1.Cell(2, 1).Range.Text = "목적";

            table_0_1.Cell(1, 2).Width = 410;
            table_0_1.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            table_0_1.Cell(1, 2).Range.Text = "체크리스트 기반 모바일 게임 취약점 진단";

            table_0_1.Cell(2, 2).Width = 410;
            table_0_1.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            table_0_1.Cell(2, 2).Range.Text = "보안성 검토를 통해 취약한 부분을 발견하고 그에 대한 가이드라인 제공";

            rng_0_1.InsertParagraphAfter();

            // table 0-1 title #2
            Word.Paragraph para_table_0_1_title = word_doc.Paragraphs.Add(ref obj_miss);
            para_table_0_1_title.Range.Text = "[표 0-1] 개요 및 목적";
            para_table_0_1_title.Range.Font.Bold = 1;
            para_table_0_1_title.Range.Font.Size = 10;
            para_table_0_1_title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            para_table_0_1_title.Format.SpaceAfter = 40;
            para_table_0_1_title.Range.InsertParagraphAfter();

            // 진단 목록 및 결과 #2
            Word.Paragraph para_title2 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title2.Range.Text = "진단 목록 및 결과";
            para_title2.Range.Font.Bold = 1;
            para_title2.Range.Font.Size = 14;
            para_title2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_table_0_1_title.Format.SpaceAfter = 3;
            //para_table_0_1_title.Range.InsertParagraphAfter();

            // table 0-2 #2
            Word.Table table_0_2;
            Word.Range rng_0_2 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_0_2 = word_doc.Tables.Add(rng_0_2, 7, 4, ref obj_miss, ref obj_miss);
            table_0_2.Range.Font.Size = 10;
            table_0_2.Range.Font.Bold = 0;
            table_0_2.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_0_2.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_0_2.AllowAutoFit = false;

            table_0_2.Cell(1, 1).Width = 50;
            table_0_2.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_0_2.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_0_2.Cell(1, 1).Range.Text = "순번";

            table_0_2.Cell(1, 2).Width = 200;
            table_0_2.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_0_2.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_0_2.Cell(1, 2).Range.Text = "점검 항목";

            table_0_2.Cell(1, 3).Width = 100;
            table_0_2.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_0_2.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_0_2.Cell(1, 3).Range.Text = "위험도";

            table_0_2.Cell(1, 4).Width = 100;
            table_0_2.Cell(1, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(1, 4).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_0_2.Cell(1, 4).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_0_2.Cell(1, 4).Range.Text = "점검 결과";

            for (int i = 1; i <= 6; i++)
            {
                table_0_2.Cell(i + 1, 1).Width = 50;
                table_0_2.Cell(i + 1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_0_2.Cell(i + 1, 1).Range.Text = i.ToString();
            }

            table_0_2.Cell(2, 2).Width = 200;
            table_0_2.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(2, 2).Range.Text = "Metadata 노출 여부";
            table_0_2.Cell(3, 2).Width = 200;
            table_0_2.Cell(3, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(3, 2).Range.Text = "함수 후킹 공격 가능성";
            table_0_2.Cell(4, 2).Width = 200;
            table_0_2.Cell(4, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(4, 2).Range.Text = "메모리 변조 가능성";
            table_0_2.Cell(5, 2).Width = 200;
            table_0_2.Cell(5, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(5, 2).Range.Text = "저장 데이터 변조 가능성";
            table_0_2.Cell(6, 2).Width = 200;
            table_0_2.Cell(6, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(6, 2).Range.Text = "시간 정보 조작 가능성";
            table_0_2.Cell(7, 2).Width = 200;
            table_0_2.Cell(7, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_0_2.Cell(7, 2).Range.Text = "난수 생성 함수 조작 가능성";

            table_0_2.Cell(2, 3).Width = 100;
            table_0_2.Cell(2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int md_cnt = 0;
            int is_sig_correct = 0, is_meta_file = 0, md_ok = 0;
            if (is_md_ok.Count > 0 && is_meta == 1)
            {
                is_sig_correct = is_md_ok[0]; is_meta_file = is_md_ok[1]; md_ok = is_md_ok[2];
                if (is_sig_correct == 1) { md_cnt++; }
                if (is_meta_file == 1) { md_cnt++; }
                if (md_ok == 1) { md_cnt++; }
                if (md_cnt == 0) { table_0_2.Cell(2, 3).Range.Text = "L"; }
                else if (md_cnt == 1) { table_0_2.Cell(2, 3).Range.Text = "M"; }
                else if (md_cnt >= 2) { table_0_2.Cell(2, 3).Range.Text = "H"; }
            }
            else
            {
                table_0_2.Cell(2, 3).Range.Text = "-";
            }

            table_0_2.Cell(3, 3).Width = 100;
            table_0_2.Cell(3, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if(is_hook_ok == 1 && is_dic == 1)
            {
                if (h_list_size > 0) { table_0_2.Cell(3, 3).Range.Text = "H"; }
                else if (h_list_size == 0) { table_0_2.Cell(3, 3).Range.Text = "L"; }
            }
            else { table_0_2.Cell(3, 3).Range.Text = "-"; }

            table_0_2.Cell(4, 3).Width = 100;
            table_0_2.Cell(4, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if(is_ocr_runned == 1 && is_memory == 1)
            {
                if(ocr_list.Count > 0) { table_0_2.Cell(4, 3).Range.Text = "H"; }
                else if(ocr_list.Count == 0) { table_0_2.Cell(4, 3).Range.Text = "L"; }
            }
            else { table_0_2.Cell(4, 3).Range.Text = "-"; }

            table_0_2.Cell(5, 3).Width = 100;
            table_0_2.Cell(5, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if(is_db_runned == 1 && is_db == 1)
            {
                if (table_list.Count > 0) { table_0_2.Cell(5, 3).Range.Text = "H"; }
                else if (table_list.Count == 0) { table_0_2.Cell(5, 3).Range.Text = "L"; }
            }
            else
            {
                table_0_2.Cell(5, 3).Range.Text = "-";
            }

            // gettimeofday / clock_gettime_function
            table_0_2.Cell(6, 3).Width = 100;
            table_0_2.Cell(6, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int t_cnt = 0;
            if (T_status.Count > 0 && is_speed == 1)
            {
                if(T_status[0] == 1) { t_cnt++; }
                if(T_status[1] == 1) { t_cnt++; }
                if(t_cnt == 0) { table_0_2.Cell(6, 3).Range.Text = "L"; }
                else if (t_cnt == 1) { table_0_2.Cell(6, 3).Range.Text = "M"; }
                else if (t_cnt == 2) { table_0_2.Cell(6, 3).Range.Text = "M"; }
            }
            else
            {
                table_0_2.Cell(6, 3).Range.Text = "-";
            }

            // next_function / range_function
            table_0_2.Cell(7, 3).Width = 100;
            table_0_2.Cell(7, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int r_cnt = 0;
            if (R_status.Count > 0 && is_random == 1)
            {
                if (R_status[0] == 1) { r_cnt++; }
                if (R_status[1] == 1) { r_cnt++; }
                if (R_status[2] == 1) { r_cnt++; }
                if (R_status[3] == 1) { r_cnt++; }
                if (R_status[4] == 1) { r_cnt++; }
                if (r_cnt <= 1) { table_0_2.Cell(7, 3).Range.Text = "L"; }
                else if (r_cnt >= 2 && r_cnt <= 3) { table_0_2.Cell(7, 3).Range.Text = "M"; }
                else if (r_cnt >= 4) { table_0_2.Cell(7, 3).Range.Text = "H"; }
            }
            else
            {
                table_0_2.Cell(7, 3).Range.Text = "-";
            }

            table_0_2.Cell(2, 4).Width = 100;
            table_0_2.Cell(2, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if (is_md_ok.Count > 0 && is_meta == 1)
            {
                if (md_cnt == 0) { table_0_2.Cell(2, 4).Range.Font.Color = Word.WdColor.wdColorGreen; table_0_2.Cell(2, 4).Range.Text = "안전"; }
                else if (md_cnt >= 1) { table_0_2.Cell(2, 4).Range.Font.Color = Word.WdColor.wdColorRed; table_0_2.Cell(2, 4).Range.Text = "취약"; a++; }
            }
            else
            {
                table_0_2.Cell(2, 4).Range.Text = "알 수 없음";
            }

            table_0_2.Cell(3, 4).Width = 100;
            table_0_2.Cell(3, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if(is_hook_ok == 1 && is_dic == 1)
            {
                if (h_list.Count > 0) { table_0_2.Cell(3, 4).Range.Font.Color = Word.WdColor.wdColorRed; table_0_2.Cell(3, 4).Range.Text = "취약"; a++; }
                else if (h_list.Count == 0) { table_0_2.Cell(3, 4).Range.Font.Color = Word.WdColor.wdColorGreen; table_0_2.Cell(3, 4).Range.Text = "안전"; }
            }
            else { table_0_2.Cell(3, 4).Range.Text = "알 수 없음"; }

            table_0_2.Cell(4, 4).Width = 100;
            table_0_2.Cell(4, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if (is_ocr_runned == 1 && is_memory == 1)
            {
                if (ocr_list.Count > 0) { table_0_2.Cell(4, 4).Range.Font.Color = Word.WdColor.wdColorRed; table_0_2.Cell(4, 4).Range.Text = "취약"; a++; }
                else if (ocr_list.Count == 0) { table_0_2.Cell(4, 4).Range.Font.Color = Word.WdColor.wdColorGreen; table_0_2.Cell(4, 4).Range.Text = "안전"; }
            }
            else { table_0_2.Cell(4, 4).Range.Text = "알 수 없음"; }

            table_0_2.Cell(5, 4).Width = 100;
            table_0_2.Cell(5, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if (is_db_runned == 1 && is_db == 1)
            {
                if (table_list.Count > 0) { table_0_2.Cell(5, 4).Range.Font.Color = Word.WdColor.wdColorRed; table_0_2.Cell(5, 4).Range.Text = "취약"; a++; }
                else if (table_list.Count == 0) { table_0_2.Cell(5, 4).Range.Font.Color = Word.WdColor.wdColorGreen; table_0_2.Cell(5, 4).Range.Text = "안전"; }
            }
            else
            {
                table_0_2.Cell(5, 4).Range.Text = "알 수 없음";
            }

            table_0_2.Cell(6, 4).Width = 100;
            table_0_2.Cell(6, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if(T_status.Count > 0 && is_speed == 1)
            {
                if(t_cnt == 0)
                {
                    table_0_2.Cell(6, 4).Range.Font.Color = Word.WdColor.wdColorGreen;
                    table_0_2.Cell(6, 4).Range.Text = "안전";
                }
                else if(t_cnt >= 1)
                {
                    table_0_2.Cell(6, 4).Range.Font.Color = Word.WdColor.wdColorRed;
                    table_0_2.Cell(6, 4).Range.Text = "취약";
                    a++;
                }
            }
            else
            {
                table_0_2.Cell(6, 4).Range.Text = "알 수 없음";
            }

            table_0_2.Cell(7, 4).Width = 100;
            table_0_2.Cell(7, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            if (R_status.Count > 0 && is_random == 1)
            {
                if (r_cnt == 0)
                {
                    table_0_2.Cell(7, 4).Range.Font.Color = Word.WdColor.wdColorGreen;
                    table_0_2.Cell(7, 4).Range.Text = "안전";
                }
                else if (r_cnt >= 1)
                {
                    table_0_2.Cell(7, 4).Range.Font.Color = Word.WdColor.wdColorRed;
                    table_0_2.Cell(7, 4).Range.Text = "취약";
                    a++;
                }
            }
            else
            {
                table_0_2.Cell(7, 4).Range.Text = "알 수 없음";
            }

            rng_0_2.InsertParagraphAfter();

            // table 0-2 title #2
            Word.Paragraph para_table_0_2_title = word_doc.Paragraphs.Add(ref obj_miss);
            para_table_0_2_title.Range.Text = "[표 0-2] 진단 결과 요약";
            para_table_0_2_title.Range.Font.Bold = 1;
            para_table_0_2_title.Range.Font.Size = 10;
            para_table_0_2_title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            para_table_0_2_title.Format.SpaceAfter = 20;
            para_table_0_2_title.Range.InsertParagraphAfter();

            // comment #2
            Word.Paragraph comment_2 = word_doc.Paragraphs.Add(ref obj_miss);
            comment_2.Range.Text = "진단 결과 6개 항목 중 " + a.ToString() + "개 항목이 취약한 것으로 발견되었으며 해당 항목들에 대한 신속한 대응이 필요하다.";
            comment_2.Range.Font.Size = 10;
            comment_2.Range.Font.Bold = 0;
            comment_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            comment_2.Format.SpaceAfter = 1000;
            comment_2.Range.InsertParagraphAfter();

            // title #3
            Word.Paragraph para_title_3 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_3.Range.Text = "I. 진단 개요";
            para_title_3.Range.Font.Bold = 1;
            para_title_3.Range.Font.Size = 18;
            para_title_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_3.Format.SpaceAfter = 20;
            para_title_3.Range.InsertParagraphAfter();

            // title2 #3
            Word.Paragraph para_title_3_1 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_3_1.Range.Text = "1. 목적";
            para_title_3_1.Range.Font.Bold = 1;
            para_title_3_1.Range.Font.Size = 14;
            para_title_3_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_3_1.Format.SpaceAfter = 20;
            para_title_3_1.Range.InsertParagraphAfter();

            // title2 content #3
            Word.Paragraph para_title_3_1_content = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_3_1_content.Range.Text = " Unity Engine으로 개발한 게임에 대해 게임 보안성 검토를 통해 취약한 부분을 발견하고 이에 대해 보안 가이드라인을 제공함으로써 보안성 증진을 위한 의사결정을 돕는다.";
            para_title_3_1_content.Range.Font.Bold = 0;
            para_title_3_1_content.Range.Font.Size = 10;
            para_title_3_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_3_1_content.Format.SpaceAfter = 20;
            para_title_3_1_content.Range.InsertParagraphAfter();

            // title3 #3
            Word.Paragraph para_title_3_2 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_3_2.Range.Text = "2. 점검 대상";
            para_title_3_2.Range.Font.Bold = 1;
            para_title_3_2.Range.Font.Size = 14;
            para_title_3_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_3_2.Format.SpaceAfter = 1;
            //para_title_3_2.Range.InsertParagraphAfter();

            // table #3
            Word.Table table_1_1;
            Word.Range rng_1_1 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_1_1 = word_doc.Tables.Add(rng_1_1, 6, 2, ref obj_miss, ref obj_miss);
            table_1_1.Range.Font.Size = 10;
            table_1_1.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_1_1.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_1_1.AllowAutoFit = false;

            // 1
            table_1_1.Cell(1, 1).Width = 100;
            table_1_1.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_1.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_1.Cell(1, 1).Range.Font.Bold = 1;
            table_1_1.Cell(1, 1).Range.Text = "Package";
            table_1_1.Cell(1, 2).Width = 350;
            table_1_1.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(1, 2).Range.Font.Bold = 0;
            table_1_1.Cell(1, 2).Range.Text = package_name;

            // 2
            table_1_1.Cell(2, 1).Width = 100;
            table_1_1.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(2, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_1.Cell(2, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_1.Cell(2, 1).Range.Font.Bold = 1;
            table_1_1.Cell(2, 1).Range.Text = "SDK version";
            table_1_1.Cell(2, 2).Width = 350;
            table_1_1.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(2, 2).Range.Font.Bold = 0;
            table_1_1.Cell(2, 2).Range.Text = "(version) " + sdk_version + "\n(code name) " + sdk_code_name;

            // 3
            table_1_1.Cell(3, 1).Width = 100;
            table_1_1.Cell(3, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(3, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_1.Cell(3, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_1.Cell(3, 1).Range.Font.Bold = 1;
            table_1_1.Cell(3, 1).Range.Text = "Unity Version";
            table_1_1.Cell(3, 2).Width = 350;
            table_1_1.Cell(3, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(3, 2).Range.Font.Bold = 0;
            table_1_1.Cell(3, 2).Range.Text = level0.ToString();

            // 4
            table_1_1.Cell(4, 1).Width = 100;
            table_1_1.Cell(4, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(4, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_1.Cell(4, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_1.Cell(4, 1).Range.Font.Bold = 1;
            table_1_1.Cell(4, 1).Range.Text = "Debuggable";
            table_1_1.Cell(4, 2).Width = 350;
            table_1_1.Cell(4, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(4, 2).Range.Font.Bold = 0;
            table_1_1.Cell(4, 2).Range.Text = debuggable;

            // 5
            table_1_1.Cell(5, 1).Width = 100;
            table_1_1.Cell(5, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(5, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_1.Cell(5, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_1.Cell(5, 1).Range.Font.Bold = 1;
            table_1_1.Cell(5, 1).Range.Text = "Permission";
            table_1_1.Cell(5, 2).Width = 350;
            table_1_1.Cell(5, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(5, 2).Range.Font.Bold = 0;
            table_1_1.Cell(5, 2).Range.Text = sb_pm.ToString();

            // 6
            table_1_1.Cell(6, 1).Width = 100;
            table_1_1.Cell(6, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(6, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_1.Cell(6, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_1.Cell(6, 1).Range.Font.Bold = 1;
            table_1_1.Cell(6, 1).Range.Text = "Platform build version";
            table_1_1.Cell(6, 2).Width = 350;
            table_1_1.Cell(6, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_1.Cell(6, 2).Range.Font.Bold = 0;
            table_1_1.Cell(6, 2).Range.Text = "(version name) " + platform_b_v_name + "\n(code) " + platform_b_v_code;

            rng_1_1.InsertParagraphAfter();

            // space
            Word.Paragraph space3 = word_doc.Paragraphs.Add(ref obj_miss);
            space3.Range.Font.Bold = 0;
            space3.Range.Font.Size = 10;
            space3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            space3.Range.Text = "";
            space3.Format.SpaceAfter = 10;
            space3.Range.InsertParagraphAfter();

            // title4 #3
            Word.Paragraph para_title_3_3 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_3_3.Range.Text = "3. 점검 환경";
            para_title_3_3.Range.Font.Bold = 1;
            para_title_3_3.Range.Font.Size = 14;
            para_title_3_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_3_3.Format.SpaceAfter = 1;

            // table2 #3
            Word.Table table_1_2;
            Word.Range rng_1_2 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_1_2 = word_doc.Tables.Add(rng_1_2, 2, 2, ref obj_miss, ref obj_miss);
            table_1_2.Range.Font.Size = 10;
            table_1_2.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_1_2.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_1_2.AllowAutoFit = false;

            table_1_2.Cell(1, 1).Width = 70;
            table_1_2.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_2.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_2.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_2.Cell(1, 1).Range.Font.Bold = 1;
            table_1_2.Cell(1, 1).Range.Text = "점검 시간";

            table_1_2.Cell(2, 1).Width = 70;
            table_1_2.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_1_2.Cell(2, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_1_2.Cell(2, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_1_2.Cell(2, 1).Range.Font.Bold = 1;
            table_1_2.Cell(2, 1).Range.Text = "점검 환경";

            table_1_2.Cell(1, 2).Width = 380;
            table_1_2.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            table_1_2.Cell(1, 2).Range.Font.Bold = 0;
            table_1_2.Cell(1, 2).Range.Text = t;

            table_1_2.Cell(2, 2).Width = 380;
            table_1_2.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            table_1_2.Cell(2, 2).Range.Font.Bold = 0;

            // adb check
            ProcessStartInfo proInfo = Set_Process("cmd", @Application.StartupPath, true);
            Process current_pro = new Process();
            current_pro.EnableRaisingEvents = false;
            current_pro.StartInfo = proInfo;
            current_pro.Start();

            String cmd_str = "ver"; String result1 = null;
            current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            current_pro.StandardInput.Close();
            result1 = current_pro.StandardOutput.ReadLine();
            current_pro.WaitForExit();
            current_pro.Close();

            List<String> txt = new List<string>(); int is_using = 0;

            // adb check
            current_pro = new Process();
            current_pro.StartInfo = proInfo;
            current_pro.Start();

            cmd_str = "adb devices"; String tmp = null; int cnt = 0;
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

            if (cnt == 0) { table_1_2.Cell(2, 2).Range.Text = "(PC) " + result1; }
            else if (cnt > 0)
            {
                List<String> t_list = new List<String>();
                current_pro = new Process();
                current_pro.StartInfo = proInfo;
                current_pro.Start();
                cmd_str = "adb shell getprop ro.product.model"; String result2 = null;
                current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
                current_pro.StandardInput.Close();
                while (!current_pro.StandardOutput.EndOfStream)
                {
                    tmp = current_pro.StandardOutput.ReadLine();
                    if (!tmp.Equals(null) && !tmp.Equals("\n") && !tmp.Equals(" ")) { t_list.Add(tmp); }
                }
                current_pro.WaitForExit();
                current_pro.Close();
                for(int x = 0; x < t_list.Count; x++)
                {
                    if (t_list[x].Contains("adb shell getprop ro.product.model")) { result2 = t_list[x+1]; break; }
                }

                t_list = new List<String>();
                current_pro = new Process();
                current_pro.StartInfo = proInfo;
                current_pro.Start();
                cmd_str = "adb shell getprop ro.product.vendor.brand"; String result3 = null;
                current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
                current_pro.StandardInput.Close();
                while (!current_pro.StandardOutput.EndOfStream)
                {
                    tmp = current_pro.StandardOutput.ReadLine();
                    if (!tmp.Equals(null) && !tmp.Equals("\n") && !tmp.Equals(" ")) { t_list.Add(tmp); }
                }
                current_pro.WaitForExit();
                current_pro.Close();
                for (int x = 0; x < t_list.Count; x++)
                {
                    if (t_list[x].Contains("adb shell getprop ro.product.vendor.brand")) { result3 = t_list[x + 1]; break; }
                }
                if(result3 == null)
                {
                    t_list = new List<String>();
                    current_pro = new Process();
                    current_pro.StartInfo = proInfo;
                    current_pro.Start();
                    cmd_str = "adb shell getprop ro.product.manufacturer";
                    current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
                    current_pro.StandardInput.Close();
                    while (!current_pro.StandardOutput.EndOfStream)
                    {
                        tmp = current_pro.StandardOutput.ReadLine();
                        if (!tmp.Equals(null) && !tmp.Equals("\n") && !tmp.Equals(" ")) { t_list.Add(tmp); }
                    }
                    current_pro.WaitForExit();
                    current_pro.Close();

                    for (int x = 0; x < t_list.Count; x++)
                    {
                        if (t_list[x].Contains("adb shell getprop ro.product.manufacturer")) { result3 = t_list[x + 1]; break; }
                    }
                }

                t_list = new List<String>();
                current_pro = new Process();
                current_pro.EnableRaisingEvents = false;
                current_pro.StartInfo = proInfo;
                current_pro.Start();
                cmd_str = "adb shell getprop ro.product.cpu.abi"; String result4 = null;
                current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
                current_pro.StandardInput.Close();
                while (!current_pro.StandardOutput.EndOfStream)
                {
                    tmp = current_pro.StandardOutput.ReadLine();
                    if (!tmp.Equals(null) && !tmp.Equals("\n") && !tmp.Equals(" ")) { t_list.Add(tmp); }
                }
                current_pro.WaitForExit();
                current_pro.Close();
                for (int x = 0; x < t_list.Count; x++)
                {
                    if (t_list[x].Contains("adb shell getprop ro.product.cpu.abi")) { result4 = t_list[x + 1]; break; }
                }

                t_list = new List<String>();
                current_pro = new Process();
                current_pro.EnableRaisingEvents = false;
                current_pro.StartInfo = proInfo;
                current_pro.Start();
                cmd_str = "adb shell getprop ro.build.version.release"; String result5 = null;
                current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
                current_pro.StandardInput.Close();
                while (!current_pro.StandardOutput.EndOfStream)
                {
                    tmp = current_pro.StandardOutput.ReadLine();
                    if (!tmp.Equals(null) && !tmp.Equals("\n") && !tmp.Equals(" ")) { t_list.Add(tmp); }
                }
                current_pro.WaitForExit();
                current_pro.Close();
                for (int x = 0; x < t_list.Count; x++)
                {
                    if (t_list[x].Contains("adb shell getprop ro.build.version.release")) { result5 = t_list[x + 1]; break; }
                }

                t_list = new List<String>();
                current_pro = new Process();
                current_pro.EnableRaisingEvents = false;
                current_pro.StartInfo = proInfo;
                current_pro.Start();
                cmd_str = "adb shell getprop ro.build.version.sdk"; String result6 = null;
                current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
                current_pro.StandardInput.Close();
                while (!current_pro.StandardOutput.EndOfStream)
                {
                    tmp = current_pro.StandardOutput.ReadLine();
                    if (!tmp.Equals(null) && !tmp.Equals("\n") && !tmp.Equals(" ")) { t_list.Add(tmp); }
                }
                current_pro.WaitForExit();
                current_pro.Close();
                for (int x = 0; x < t_list.Count; x++)
                {
                    if (t_list[x].Contains("adb shell getprop ro.build.version.sdk")) { result6 = t_list[x + 1]; break; }
                }

                StringBuilder temp_sb = new StringBuilder();
                if (result1 != null) { temp_sb.Append("(PC) " + result1); }
                if (result3 != null) { temp_sb.Append("\n(Brand) " + result3); }
                if (result2 != null) { temp_sb.Append("\n(Device) " + result2); }
                if (result4 != null) { temp_sb.Append("\n(Architecture) " + result4); }
                if (result5 != null) { temp_sb.Append("\n(Release version) " + result5); }
                if (result6 != null) { temp_sb.Append("\n(SDK version) " + result6); }
                table_1_2.Cell(2, 2).Range.Text = temp_sb.ToString();
            }
            rng_1_2.InsertParagraphAfter();

            // space
            Word.Paragraph space4 = word_doc.Paragraphs.Add(ref obj_miss);
            space4.Range.Font.Bold = 0;
            space4.Range.Font.Size = 10;
            space4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            space4.Range.Text = "";
            space4.Format.SpaceAfter = 20;
            space4.Range.InsertParagraphAfter();

            // title #4
            Word.Paragraph para_title_4_1 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_4_1.Range.Text = "4. 점검 항목";
            para_title_4_1.Range.Font.Bold = 1;
            para_title_4_1.Range.Font.Size = 14;
            para_title_4_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_4_1.Format.SpaceAfter = 3;

            // table #4
            Word.Table table_2_1;
            Word.Range rng_2_1 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_2_1 = word_doc.Tables.Add(rng_2_1, 7, 3, ref obj_miss, ref obj_miss);
            table_2_1.Range.Font.Size = 10;
            table_2_1.Range.Font.Bold = 0;
            table_2_1.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_2_1.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_2_1.AllowAutoFit = false;

            table_2_1.Cell(1, 1).Width = 50;
            table_2_1.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_1.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_1.Cell(1, 1).Range.Font.Bold = 1;
            table_2_1.Cell(1, 1).Range.Text = "순번";

            table_2_1.Cell(1, 2).Width = 200;
            table_2_1.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_1.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_1.Cell(1, 2).Range.Font.Bold = 1;
            table_2_1.Cell(1, 2).Range.Text = "점검 항목";

            table_2_1.Cell(1, 3).Width = 200;
            table_2_1.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_1.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_1.Cell(1, 3).Range.Font.Bold = 1;
            table_2_1.Cell(1, 3).Range.Text = "세부 내용";

            for (int i = 1; i <= 6; i++)
            {
                table_2_1.Cell(i + 1, 1).Width = 50;
                table_2_1.Cell(i + 1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_2_1.Cell(i + 1, 1).Range.Text = i.ToString();
            }

            table_2_1.Cell(2, 2).Width = 200;
            table_2_1.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(2, 2).Range.Text = "Metadata 노출 여부";
            table_2_1.Cell(3, 2).Width = 200;
            table_2_1.Cell(3, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(3, 2).Range.Text = "함수 후킹 공격 가능성";
            table_2_1.Cell(4, 2).Width = 200;
            table_2_1.Cell(4, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(4, 2).Range.Text = "메모리 변조 가능성";
            table_2_1.Cell(5, 2).Width = 200;
            table_2_1.Cell(5, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(5, 2).Range.Text = "저장 데이터 변조 가능성";
            table_2_1.Cell(6, 2).Width = 200;
            table_2_1.Cell(6, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(6, 2).Range.Text = "시간 정보 조작 가능성";
            table_2_1.Cell(7, 2).Width = 200;
            table_2_1.Cell(7, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(7, 2).Range.Text = "난수 생성 함수 조작 가능성";

            table_2_1.Cell(2, 3).Width = 200;
            table_2_1.Cell(2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(2, 3).Range.Text = "Global-metadata.dat 파일을 통한 디버그 심볼 노출 확인";
            table_2_1.Cell(3, 3).Width = 200;
            table_2_1.Cell(3, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(3, 3).Range.Text = "함수명, 인자값, 반환값을 통해\n함수의 후킹 공격 취약성 진단";
            table_2_1.Cell(4, 3).Width = 200;
            table_2_1.Cell(4, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(4, 3).Range.Text = "게임 내 주요 정보의 메모리 노출 여부, 변조 가능성 진단";
            table_2_1.Cell(5, 3).Width = 200;
            table_2_1.Cell(5, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(5, 3).Range.Text = "게임 내부 DB나 저장 파일에서 주요 정보 노출 여부, 변조 가능성 진단";
            table_2_1.Cell(6, 3).Width = 200;
            table_2_1.Cell(6, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(6, 3).Range.Text = "시간 정보 조작을 통한 스피드핵\n이용 가능성 진단";
            table_2_1.Cell(7, 3).Width = 200;
            table_2_1.Cell(7, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_1.Cell(7, 3).Range.Text = "난수 생성 함수 후킹을 통한\n확률 조작 가능성 진단";

            rng_2_1.InsertParagraphAfter();

            // table 2-1 title #4
            Word.Paragraph para_table_2_1_title = word_doc.Paragraphs.Add(ref obj_miss);
            para_table_2_1_title.Range.Text = "[표 0-1] 점검 항목 목록";
            para_table_2_1_title.Range.Font.Bold = 1;
            para_table_2_1_title.Range.Font.Size = 10;
            para_table_2_1_title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            para_table_2_1_title.Format.SpaceAfter = 20;
            para_table_2_1_title.Range.InsertParagraphAfter();

            // title2 #4
            Word.Paragraph para_title_4_2 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_4_2.Range.Text = "5. 위험도 평가 기준";
            para_title_4_2.Range.Font.Bold = 1;
            para_title_4_2.Range.Font.Size = 14;
            para_title_4_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_4_2.Format.SpaceAfter = 3;

            // table2 #4
            Word.Table table_2_2;
            Word.Range rng_2_2 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_2_2 = word_doc.Tables.Add(rng_2_2, 4, 2, ref obj_miss, ref obj_miss);
            table_2_2.Range.Font.Size = 10;
            table_2_2.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_2_2.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_2_2.AllowAutoFit = false;

            table_2_2.Cell(1, 1).Width = 70;
            table_2_2.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_2.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_2.Cell(1, 1).Range.Font.Bold = 1;
            table_2_2.Cell(1, 1).Range.Text = "위험도";

            table_2_2.Cell(2, 1).Width = 70;
            table_2_2.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(2, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_2.Cell(2, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_2.Cell(2, 1).Range.Font.Bold = 1;
            table_2_2.Cell(2, 1).Range.Text = "High (H)";

            table_2_2.Cell(3, 1).Width = 70;
            table_2_2.Cell(3, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(3, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_2.Cell(3, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_2.Cell(3, 1).Range.Font.Bold = 1;
            table_2_2.Cell(3, 1).Range.Text = "Medium (M)";

            table_2_2.Cell(4, 1).Width = 70;
            table_2_2.Cell(4, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(4, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_2.Cell(4, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_2.Cell(4, 1).Range.Font.Bold = 1;
            table_2_2.Cell(4, 1).Range.Text = "Low (L)";

            table_2_2.Cell(1, 2).Width = 380;
            table_2_2.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_2_2.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_2_2.Cell(1, 2).Range.Font.Bold = 1;
            table_2_2.Cell(1, 2).Range.Text = "설명";

            table_2_2.Cell(2, 2).Width = 380;
            table_2_2.Cell(2, 2).Range.Font.Bold = 0;
            table_2_2.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(2, 2).Range.Text = "시스템에 심각한 영향을 발생시켜 비즈니스 영역에 피해를 미칠 위험성 존재";

            table_2_2.Cell(3, 2).Width = 380;
            table_2_2.Cell(3, 2).Range.Font.Bold = 0;
            table_2_2.Cell(3, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(3, 2).Range.Text = "제한된 환경에서만 주요 정보가 노출되며, 추가 공격이 가능한 정보가 노출된 상태";

            table_2_2.Cell(4, 2).Width = 380;
            table_2_2.Cell(4, 2).Range.Font.Bold = 0;
            table_2_2.Cell(4, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_2_2.Cell(4, 2).Range.Text = "서비스에 큰 영향을 미치지 않고 중요하지 않은 정보가 노출된 상태";

            rng_2_2.InsertParagraphAfter();

            // table 2-2 title #4
            Word.Paragraph para_table_2_2_title = word_doc.Paragraphs.Add(ref obj_miss);
            para_table_2_2_title.Range.Text = "[표 0-2] 위험도 평가 기준";
            para_table_2_2_title.Range.Font.Bold = 1;
            para_table_2_2_title.Range.Font.Size = 10;
            para_table_2_2_title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            para_table_2_2_title.Format.SpaceAfter = 20;
            para_table_2_2_title.Range.InsertParagraphAfter();

            // table 2-2 title content #4
            Word.Paragraph para_table_2_2_title_content = word_doc.Paragraphs.Add(ref obj_miss);
            para_table_2_2_title_content.Range.Text = "* 위험도 평가 기준의 경우, 기본값을 제공하나 환경에 따라 본인이 선택할 수 있도록 변경 가능하다.";
            para_table_2_2_title_content.Range.Font.Bold = 0;
            para_table_2_2_title_content.Range.Font.Size = 9;
            para_table_2_2_title_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_table_2_2_title_content.Format.SpaceAfter = 100;
            para_table_2_2_title_content.Range.InsertParagraphAfter();

            // title #5
            Word.Paragraph para_title_5 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_5.Range.Text = "II. 진단 결과 및 대응방안";
            para_title_5.Range.Font.Bold = 1;
            para_title_5.Range.Font.Size = 18;
            para_title_5.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_5.Format.SpaceAfter = 10;
            para_title_5.Range.InsertParagraphAfter();

            // ################################ META
            if (is_md_ok.Count > 0 && is_meta == 1)
            {
                // title2 #5
                Word.Paragraph para_title_5_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1.Range.Text = "1. Metadata 노출 여부";
                para_title_5_1.Range.Font.Bold = 1;
                para_title_5_1.Range.Font.Size = 14;
                para_title_5_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1.Format.SpaceAfter = 10;
                para_title_5_1.Range.InsertParagraphAfter();

                // title3 #5
                Word.Paragraph para_title_5_1_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_1.Range.Text = "1.1. 취약점 소개";
                para_title_5_1_1.Range.Font.Bold = 1;
                para_title_5_1_1.Range.Font.Size = 12;
                para_title_5_1_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_1.Format.SpaceAfter = 10;
                para_title_5_1_1.Range.InsertParagraphAfter();

                // title3 content #5
                Word.Paragraph para_title_5_1_1_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_1_content.Range.Text = " global-metadata.dat 파일이 아무런 보호조치 없이 노출된 경우, 디버그 심볼 복구를 통해 클래스 정보, 변수와 함수의 이름 및 위치 정보가 노출될 위험이 존재한다. 이는 후킹 공격 및 메모리 변조 공격 등 게임 해킹에 있어 중요한 정보를 해커에게 제공하는 것이므로 각별한 유의가 필요하다.";
                para_title_5_1_1_content.Range.Font.Bold = 0;
                para_title_5_1_1_content.Range.Font.Size = 10;
                para_title_5_1_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_1_content.Format.SpaceAfter = 10;
                para_title_5_1_1_content.Range.InsertParagraphAfter();

                // title4 #5
                Word.Paragraph para_title_5_1_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_2.Range.Text = "1.2. 취약점 진단 결과";
                para_title_5_1_2.Range.Font.Bold = 1;
                para_title_5_1_2.Range.Font.Size = 12;
                para_title_5_1_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_2.Format.SpaceAfter = 10;
                para_title_5_1_2.Range.InsertParagraphAfter();

                // title4 content #5
                Word.Paragraph para_title_5_1_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_2_content.Range.Font.Bold = 0;
                para_title_5_1_2_content.Range.Font.Size = 10;
                para_title_5_1_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                if (is_meta_exist == 1) { para_title_5_1_2_content.Range.Text = " 해당 게임에 대해 Metadata 노출 여부를 진단한 결과, global-metadata.dat 파일에 대해 아무런 보호조치가 적용되지 않았음을 확인하였다. 해당 파일로부터 디버그 심볼을 복구하여 이름과 위치에 대한 정보가 노출됨을 알 수 있다."; }
                else { para_title_5_1_2_content.Range.Text = " 보호조치가 적용되어있으나, 메모리 덤프로 쉽게 복구할 수 있는 취약한 보호조치임을 확인하였다."; }
                para_title_5_1_2_content.Format.SpaceAfter = 10;
                para_title_5_1_2_content.Range.InsertParagraphAfter();

                Word.Paragraph para_title_5_1_2_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_2_2.Range.Text = "① global-metadata.dat 파일 존재 유무";
                para_title_5_1_2_2.Range.Font.Bold = 1;
                para_title_5_1_2_2.Range.Font.Size = 10;
                para_title_5_1_2_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_2_2.Format.SpaceAfter = 10;
                para_title_5_1_2_2.Range.InsertParagraphAfter();

                Word.Paragraph para_title_5_1_2_2_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_2_2_1.Range.Font.Bold = 0;
                para_title_5_1_2_2_1.Range.Font.Size = 10;
                para_title_5_1_2_2_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                if (is_meta_file == 1) { para_title_5_1_2_2_1.Range.Text = "아래 경로에 global-metadata.dat 파일이 존재한다.\n" + "경로: " + metadata_path; }
                else { para_title_5_1_2_2_1.Range.Text = ""; }
                para_title_5_1_2_2_1.Format.SpaceAfter = 20;
                para_title_5_1_2_2_1.Range.InsertParagraphAfter();

                Word.Paragraph para_title_5_1_2_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_2_3.Range.Text = "② global-metadata.dat 파일 암호화 여부";
                para_title_5_1_2_3.Range.Font.Bold = 1;
                para_title_5_1_2_3.Range.Font.Size = 10;
                para_title_5_1_2_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_2_3.Format.SpaceAfter = 10;
                para_title_5_1_2_3.Range.InsertParagraphAfter();

                Word.Paragraph para_title_5_1_2_3_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_2_3_1.Range.Font.Bold = 0;
                para_title_5_1_2_3_1.Range.Font.Size = 10;
                para_title_5_1_2_3_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                if (is_sig_correct == 1) { para_title_5_1_2_3_1.Range.Text = "Signature 비교 결과 global-metadata.dat 파일은 암호화되지 않았다."; }
                else { para_title_5_1_2_3_1.Range.Text = "Signature 비교 결과 global-metadata.dat 파일은 암호화되었다."; }
                para_title_5_1_2_3_1.Format.SpaceAfter = 20;
                para_title_5_1_2_3_1.Range.InsertParagraphAfter();

                if(is_click_md == 1)
                {
                    Word.Paragraph para_title_5_1_2_4 = word_doc.Paragraphs.Add(ref obj_miss);
                    para_title_5_1_2_4.Range.Text = "③ global-metadata.dat 파일 복구 가능 여부";
                    para_title_5_1_2_4.Range.Font.Bold = 1;
                    para_title_5_1_2_4.Range.Font.Size = 10;
                    para_title_5_1_2_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    para_title_5_1_2_4.Format.SpaceAfter = 10;
                    para_title_5_1_2_4.Range.InsertParagraphAfter();

                    Word.Paragraph para_title_5_1_2_4_1 = word_doc.Paragraphs.Add(ref obj_miss);
                    para_title_5_1_2_4_1.Range.Font.Bold = 0;
                    para_title_5_1_2_4_1.Range.Font.Size = 10;
                    para_title_5_1_2_4_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    if (md_ok == 1 && is_click_md == 1) { para_title_5_1_2_4_1.Range.Text = "프로세스 메모리를 덤프하여 원본의 global-metadata.dat를 복구하였다."; }
                    else if (md_ok == 0 && is_click_md == 1) { para_title_5_1_2_4_1.Range.Text = "프로세스 메모리를 덤프하였으나, global-metadata.dat 파일에 대해 보호조치가 되어있어 원본의 global-metadata.dat를 복구할 수 없었다."; }
                    para_title_5_1_2_4_1.Format.SpaceAfter = 20;
                    para_title_5_1_2_4_1.Range.InsertParagraphAfter();
                }

                if(meta_f_list.Count > 0)
                {
                    List<StringBuilder> _sb = new List<StringBuilder>();
                    _sb = search_dumpcs();
                    if(_sb.Count > 0)
                    {
                        // space
                        Word.Paragraph _para_title_5_1_3_2_1 = word_doc.Paragraphs.Add(ref obj_miss);
                        _para_title_5_1_3_2_1.Range.Font.Bold = 1;
                        _para_title_5_1_3_2_1.Range.Font.Size = 10;
                        _para_title_5_1_3_2_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        if(is_click_md == 0) { _para_title_5_1_3_2_1.Range.Text = "③ global-metadata.dat 파일 복구 예시"; }
                        else { _para_title_5_1_3_2_1.Range.Text = "④ global-metadata.dat 파일 복구 예시"; }
                        _para_title_5_1_3_2_1.Format.SpaceAfter = 3;

                        Word.Table table_5_1_3_content;
                        Word.Range rng_table_5_1_3_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                        table_5_1_3_content = word_doc.Tables.Add(rng_table_5_1_3_content, 1, 1, ref obj_miss, ref obj_miss);
                        table_5_1_3_content.Range.Font.Size = 8;
                        table_5_1_3_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                        table_5_1_3_content.AllowAutoFit = false;
                        table_5_1_3_content.Cell(1, 1).Width = 450;
                        table_5_1_3_content.Cell(1, 1).Range.Font.Bold = 0;
                        table_5_1_3_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        table_5_1_3_content.Cell(1, 1).Range.Text = _sb[0].ToString();
                        rng_table_5_1_3_content.InsertParagraphAfter();
                    }

                    // space
                    Word.Paragraph para_title_5_1_3_3_1 = word_doc.Paragraphs.Add(ref obj_miss);
                    para_title_5_1_3_3_1.Range.Font.Bold = 0;
                    para_title_5_1_3_3_1.Range.Font.Size = 10;
                    para_title_5_1_3_3_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    para_title_5_1_3_3_1.Range.Text = "";
                    para_title_5_1_3_3_1.Format.SpaceAfter = 10;
                    para_title_5_1_3_3_1.Range.InsertParagraphAfter();
                }

                // title5 #5
                Word.Paragraph para_title_5_1_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3.Range.Text = "1.3. 대응방안";
                para_title_5_1_3.Range.Font.Bold = 1;
                para_title_5_1_3.Range.Font.Size = 12;
                para_title_5_1_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3.Format.SpaceAfter = 10;
                para_title_5_1_3.Range.InsertParagraphAfter();

                // title5 #6
                Word.Paragraph para_title_5_1_3_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_1.Range.Text = "Guide 1) 메타데이터 파일명 변경";
                para_title_5_1_3_1.Range.Font.Bold = 1;
                para_title_5_1_3_1.Range.Font.Size = 10;
                para_title_5_1_3_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_1.Format.SpaceAfter = 10;
                para_title_5_1_3_1.Range.InsertParagraphAfter();

                // title6 content #5
                Word.Paragraph para_title_5_1_3_1_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_1_content.Range.Font.Bold = 0;
                para_title_5_1_3_1_content.Range.Font.Size = 10;
                para_title_5_1_3_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_1_content.Range.Text = "LoadMetadataFile() 호출시 전달하는 인자값으로 메타데이터 파일의 이름을 전달할 수 있다.\n아래 예시에서는 파일명을 “resource1.dat”으로 변경하여 전달한다.";
                para_title_5_1_3_1_content.Format.SpaceAfter = 1;

                Word.Table table_5_1_3_1_content;
                Word.Range rng_table_5_1_3_1_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_5_1_3_1_content = word_doc.Tables.Add(rng_table_5_1_3_1_content, 1, 1, ref obj_miss, ref obj_miss);
                table_5_1_3_1_content.Range.Font.Size = 8;
                table_5_1_3_1_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_5_1_3_1_content.AllowAutoFit = false;
                table_5_1_3_1_content.Cell(1, 1).Width = 450;
                table_5_1_3_1_content.Cell(1, 1).Range.Font.Bold = 0;
                table_5_1_3_1_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                table_5_1_3_1_content.Cell(1, 1).Range.Text = "void MetadataCache::Initialize()\n{\n          s_GlobalMetadata = vm::MetadataLoader::LoadMetadataFile(\"resource1.dat\");\n          s_GlobalMetadataHeader = (const Il2cppGlobalMetadataHeader*)s_GlobalMetadata;\n          IL2CPP_ASSERT(s_GlobalMetadataHeader->sanity == 0xFAB11BAF);\n          IL2CPP_ASSERT(s_GlobalMetadataHeader->verson == 24);\n          //preallocate these arrays so we don't need to lock when reading Later.\n}";
                rng_table_5_1_3_1_content.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_5_1_3_1_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_1_1.Range.Font.Bold = 0;
                para_title_5_1_3_1_1.Range.Font.Size = 10;
                para_title_5_1_3_1_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_1_1.Range.Text = "";
                para_title_5_1_3_1_1.Format.SpaceAfter = 10;
                para_title_5_1_3_1_1.Range.InsertParagraphAfter();

                // title7 #5
                Word.Paragraph para_title_5_1_3_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_2.Range.Text = "Guide 2) 메타데이터 로드 위치 변경";
                para_title_5_1_3_2.Range.Font.Bold = 1;
                para_title_5_1_3_2.Range.Font.Size = 10;
                para_title_5_1_3_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_2.Format.SpaceAfter = 10;
                para_title_5_1_3_2.Range.InsertParagraphAfter();

                // title7 content #5
                Word.Paragraph para_title_5_1_3_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_2_content.Range.Font.Bold = 0;
                para_title_5_1_3_2_content.Range.Font.Size = 10;
                para_title_5_1_3_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_2_content.Range.Text = " LoadMetadataFile() 호출시 메타데이터 파일을 읽어올 디렉토리 주소를 지정할 수 있다.\nglobal - metadata.dat는 기본적으로 ~\\assets\\bin\\Data\\Managed\\Metadata 경로에 존재한다.\n아래 예시에서는 메타데이터 파일을 은닉시킨 후, 해당 디렉토리 주소를 인자로 전달한다.";
                para_title_5_1_3_2_content.Format.SpaceAfter = 1;

                Word.Table table_5_1_3_2_content;
                Word.Range rng_table_5_1_3_2_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_5_1_3_2_content = word_doc.Tables.Add(rng_table_5_1_3_2_content, 1, 1, ref obj_miss, ref obj_miss);
                table_5_1_3_2_content.Range.Font.Size = 8;
                table_5_1_3_2_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_5_1_3_2_content.AllowAutoFit = false;
                table_5_1_3_2_content.Cell(1, 1).Width = 450;
                table_5_1_3_2_content.Cell(1, 1).Range.Font.Bold = 0;
                table_5_1_3_2_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                table_5_1_3_2_content.Cell(1, 1).Range.Text = "std::string MetadataDirectory\n{\n          return utils:PathUtils::Combine(utils::Runtime::GetDataDir(), utils::StringView<char>(\"TEST\"));\n}\n\nvoid* il2cpp::vm::MetadataLoader::LoadMetadataFile(const char* fileName)\n{\n          std::string resourcesDirectory = MetadataDirectory();\n          std::string resourceFilePath = utils::PathUtils::Combine(resourcesDirectory, utils::StringView<char>(fileName,\n          strlen(fileName)));\n          int error = 0;\n}";
                rng_table_5_1_3_2_content.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_5_1_3_2_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_2_1.Range.Font.Bold = 0;
                para_title_5_1_3_2_1.Range.Font.Size = 10;
                para_title_5_1_3_2_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_2_1.Range.Text = "";
                para_title_5_1_3_2_1.Format.SpaceAfter = 10;
                para_title_5_1_3_2_1.Range.InsertParagraphAfter();

                // title8 #5
                Word.Paragraph para_title_5_1_3_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_3.Range.Text = "Guide 3) 메타데이터 암호화";
                para_title_5_1_3_3.Range.Font.Bold = 1;
                para_title_5_1_3_3.Range.Font.Size = 10;
                para_title_5_1_3_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_3.Format.SpaceAfter = 10;
                para_title_5_1_3_3.Range.InsertParagraphAfter();

                // title8 content #5
                Word.Paragraph para_title_5_1_3_3_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_1_3_3_content.Range.Font.Bold = 0;
                para_title_5_1_3_3_content.Range.Font.Size = 10;
                para_title_5_1_3_3_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_1_3_3_content.Range.Text = " 메타데이터 파일명을 변경하고, 로드 위치를 변경한 이후에도 추가적으로 암호화를 진행할 수 있다. Metadata는 게임 해킹에 가장 기본적인 정보를 제공하므로 강력한 보안 조치가 필요하다.\n";
                para_title_5_1_3_3_content.Format.SpaceAfter = 50;
                para_title_5_1_3_3_content.Range.InsertParagraphAfter();

                para_num++;
            }

            if(h_list_size > 0 && is_dic == 1)
            {
                // title9 #5
                Word.Paragraph para_title_5_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2.Range.Text = para_num.ToString() + ". 함수 후킹 공격 가능성";
                para_title_5_2.Range.Font.Bold = 1;
                para_title_5_2.Range.Font.Size = 14;
                para_title_5_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2.Format.SpaceAfter = 20;
                para_title_5_2.Range.InsertParagraphAfter();

                // title9 #5
                Word.Paragraph para_title_5_2_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_1.Range.Font.Bold = 1;
                para_title_5_2_1.Range.Font.Size = 12;
                para_title_5_2_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_1.Range.Text = para_num.ToString() + ".1. 취약점 소개";
                para_title_5_2_1.Format.SpaceAfter = 20;
                para_title_5_2_1.Range.InsertParagraphAfter();

                // title9 content #5
                Word.Paragraph para_title_5_2_1_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_1_content.Range.Font.Bold = 0;
                para_title_5_2_1_content.Range.Font.Size = 10;
                para_title_5_2_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_1_content.Range.Text = " 함수 후킹(Function Hooking)이란, 게임 동작 중 호출되는 함수를 가로채는 행위를 말한다. 함수 후킹 공격을 통해 인자값과 반환값을 공격자가 임의로 변경할 수 있다. 또한 함수의 실행을 무효화시키거나, 본래의 함수가 아닌 다른 함수를 실행시키는 것도 가능하다.\n 함수 후킹 공격의 대표적 예시로는 1) set_gold(int gold) 함수의 인자값을 후킹하여 gold 변수를 원하는 값으로 변경하거나, 2) get_Damage() 함수를 후킹하여 damage를 원하는 값으로 변경하는 것 등이 있다.";
                para_title_5_2_1_content.Format.SpaceAfter = 20;
                para_title_5_2_1_content.Range.InsertParagraphAfter();

                // title10 #5
                Word.Paragraph para_title_5_2_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_2.Range.Text = para_num.ToString() + ".2. 취약점 진단 결과";
                para_title_5_2_2.Range.Font.Bold = 1;
                para_title_5_2_2.Range.Font.Size = 12;
                para_title_5_2_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_2.Format.SpaceAfter = 20;
                para_title_5_2_2.Range.InsertParagraphAfter();

                // title10 content #5
                List<List<String>> temp_list2 = new List<List<String>>();
                if (h_list_size > 0)
                {
                    List<String> temp_list = new List<String>();
                    List<String> temp_list3;
                    String temp_str = null;
                    for (int i = 0; i < h_list_size; i++)
                    {
                        temp_str = h_list[i][0];
                        for (int z = 0; z < temp_list.Count; z++) { if (temp_list[z].Equals(temp_str)) { continue; } } // 중복 방지
                        temp_list.Add(temp_str);
                    }

                    for (int i = 0; i < hooking_list_size; i++)
                    {
                        for (int j = 0; j < temp_list.Count; j++)
                        {
                            if (meta_f_list[i][1].Contains(temp_list[j]))
                            {
                                temp_list3 = new List<String>();
                                temp_list3.Add(meta_f_list[i][0]); temp_list3.Add(meta_f_list[i][1]); temp_list3.Add(meta_f_list[i][2]);
                                temp_list2.Add(temp_list3);
                            }
                        }
                    }
                }

                Word.Paragraph para_title_5_2_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_2_content.Range.Font.Bold = 0;
                para_title_5_2_2_content.Range.Font.Size = 10;
                para_title_5_2_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_2_content.Range.Text = " 해당 게임에서 함수 후킹 공격 가능성을 진단한 결과, 총 " + h_list_size.ToString() + "개의 함수에 대해서 함수의 인자값과 반환값이 평문으로 노출됨을 확인하였다. 취약한 함수의 목록은 다음과 같다.";
                para_title_5_2_2_content.Format.SpaceAfter = 1;

                Word.Table table_5_2_2_content;
                Word.Range rng_table_5_2_2_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_5_2_2_content = word_doc.Tables.Add(rng_table_5_2_2_content, 2, 3, ref obj_miss, ref obj_miss);
                table_5_2_2_content.Range.Font.Size = 8;
                table_5_2_2_content.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_5_2_2_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_5_2_2_content.AllowAutoFit = false;

                table_5_2_2_content.Cell(1, 1).Width = 185;
                table_5_2_2_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_5_2_2_content.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_5_2_2_content.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_5_2_2_content.Cell(1, 1).Range.Font.Bold = 1;
                table_5_2_2_content.Cell(1, 1).Range.Text = "Class";

                table_5_2_2_content.Cell(1, 2).Width = 185;
                table_5_2_2_content.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_5_2_2_content.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_5_2_2_content.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_5_2_2_content.Cell(1, 2).Range.Font.Bold = 1;
                table_5_2_2_content.Cell(1, 2).Range.Text = "Method";

                table_5_2_2_content.Cell(1, 3).Width = 80;
                table_5_2_2_content.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_5_2_2_content.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_5_2_2_content.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_5_2_2_content.Cell(1, 3).Range.Font.Bold = 1;
                table_5_2_2_content.Cell(1, 3).Range.Text = "RVA";

                if (temp_list2.Count > 0)
                {
                    for (int i = 0; i < temp_list2.Count; i++)
                    {
                        if (i > 0)
                        {
                            table_5_2_2_content.Rows.Add();
                        }
                        table_5_2_2_content.Cell(i + 2, 1).Width = 185;
                        table_5_2_2_content.Cell(i + 2, 1).Range.Font.Bold = 0;
                        table_5_2_2_content.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        table_5_2_2_content.Cell(i + 2, 1).Range.Text = temp_list2[i][0];

                        table_5_2_2_content.Cell(i + 2, 2).Width = 185;
                        table_5_2_2_content.Cell(i + 2, 2).Range.Font.Bold = 0;
                        table_5_2_2_content.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        table_5_2_2_content.Cell(i + 2, 2).Range.Text = temp_list2[i][1].Trim();

                        table_5_2_2_content.Cell(i + 2, 3).Width = 80;
                        table_5_2_2_content.Cell(i + 2, 3).Range.Font.Bold = 0;
                        table_5_2_2_content.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        table_5_2_2_content.Cell(i + 2, 3).Range.Text = temp_list2[i][2].Trim();
                    }
                }
                else
                {
                    for (int i = 0; i < hooking_list_size; i++)
                    {
                        if (i > 0)
                        {
                            table_5_2_2_content.Rows.Add();
                        }
                        table_5_2_2_content.Cell(i + 2, 1).Width = 185;
                        table_5_2_2_content.Cell(i + 2, 1).Range.Font.Bold = 0;
                        table_5_2_2_content.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        table_5_2_2_content.Cell(i + 2, 1).Range.Text = meta_f_list[i][0].Trim();

                        table_5_2_2_content.Cell(i + 2, 2).Width = 185;
                        table_5_2_2_content.Cell(i + 2, 2).Range.Font.Bold = 0;
                        table_5_2_2_content.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        table_5_2_2_content.Cell(i + 2, 2).Range.Text = meta_f_list[i][1].Trim();

                        table_5_2_2_content.Cell(i + 2, 3).Width = 80;
                        table_5_2_2_content.Cell(i + 2, 3).Range.Font.Bold = 0;
                        table_5_2_2_content.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        table_5_2_2_content.Cell(i + 2, 3).Range.Text = meta_f_list[i][2].Trim();
                    }
                }
                rng_table_5_2_2_content.InsertParagraphAfter();

                Word.Paragraph para_title_5_2_2_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_2_1.Range.Font.Bold = 0;
                para_title_5_2_2_1.Range.Font.Size = 10;
                para_title_5_2_2_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_2_1.Range.Text = "\n아래 표는 사용자가 선택한 함수 " + h_list_size + "개를 후킹 및 분석한 내용이다.";
                para_title_5_2_2_1.Format.SpaceAfter = 1;

                Word.Table table_5_2_2_1_content;
                Word.Range rng_table_5_2_2_1_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_5_2_2_1_content = word_doc.Tables.Add(rng_table_5_2_2_1_content, 2, 2, ref obj_miss, ref obj_miss);
                table_5_2_2_1_content.Range.Font.Size = 8;
                table_5_2_2_1_content.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_5_2_2_1_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_5_2_2_1_content.AllowAutoFit = false;

                table_5_2_2_1_content.Cell(1, 1).Width = 150;
                table_5_2_2_1_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_5_2_2_1_content.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_5_2_2_1_content.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_5_2_2_1_content.Cell(1, 1).Range.Font.Bold = 1;
                table_5_2_2_1_content.Cell(1, 1).Range.Text = "Method";

                table_5_2_2_1_content.Cell(1, 2).Width = 300;
                table_5_2_2_1_content.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_5_2_2_1_content.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_5_2_2_1_content.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_5_2_2_1_content.Cell(1, 2).Range.Font.Bold = 1;
                table_5_2_2_1_content.Cell(1, 2).Range.Text = "Details";

                StringBuilder sb2; int temp_cnt = 0;
                for (int i = 0; i < h_list_size; i++)
                {
                    temp_cnt = h_list[i].Count;
                    sb2 = new StringBuilder();
                    if (i > 0)
                    {
                        table_5_2_2_1_content.Rows.Add();
                    }
                    table_5_2_2_1_content.Cell(i + 2, 1).Width = 150;
                    table_5_2_2_1_content.Cell(i + 2, 1).Range.Font.Bold = 0;
                    table_5_2_2_1_content.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_5_2_2_1_content.Cell(i + 2, 1).Range.Text = h_list[i][0];
                    for (int j = 1; j < temp_cnt; j++)
                    {
                        sb2.Append(h_list[i][j].Trim());
                        if (j + 1 != temp_cnt)
                        {
                            sb2.Append("\n");
                        }
                    }
                    table_5_2_2_1_content.Cell(i + 2, 2).Width = 300;
                    table_5_2_2_1_content.Cell(i + 2, 2).Range.Font.Bold = 0;
                    table_5_2_2_1_content.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    table_5_2_2_1_content.Cell(i + 2, 2).Range.Text = sb2.ToString();
                }
                rng_table_5_2_2_1_content.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_5_2_2_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_2_2.Range.Font.Bold = 0;
                para_title_5_2_2_2.Range.Font.Size = 10;
                para_title_5_2_2_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_2_2.Range.Text = "";
                para_title_5_2_2_2.Format.SpaceAfter = 10;
                para_title_5_2_2_2.Range.InsertParagraphAfter();

                // title11 #5
                Word.Paragraph para_title_5_2_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3.Range.Text = para_num.ToString() + ".3. 대응방안";
                para_title_5_2_3.Range.Font.Bold = 1;
                para_title_5_2_3.Range.Font.Size = 12;
                para_title_5_2_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3.Format.SpaceAfter = 10;
                para_title_5_2_3.Range.InsertParagraphAfter();

                // title12 #5
                Word.Paragraph para_title_5_2_3_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_1.Range.Text = "Guide 1) 함수 내부/외부에 검증 로직 추가";
                para_title_5_2_3_1.Range.Font.Bold = 1;
                para_title_5_2_3_1.Range.Font.Size = 10;
                para_title_5_2_3_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_1.Format.SpaceAfter = 10;
                para_title_5_2_3_1.Range.InsertParagraphAfter();

                // title12 content #5
                Word.Paragraph para_title_5_2_3_1_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_1_content.Range.Font.Bold = 0;
                para_title_5_2_3_1_content.Range.Font.Size = 10;
                para_title_5_2_3_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_1_content.Range.Text = " 함수 내부와 외부에서 함수가 후킹되었는지를 검증해주어야 한다. 함수로 전해지는 인자값을 신뢰할 수 없는 경우에는 중요 변수를 다른 곳에도 저장해두고 그 값과 인자값이 같은지를 검증하여야 한다. 또한 함수의 반환값을 신뢰할 수 없는 경우에는 함수 외부에서 그 값이 정상적인 범주에 있는 값인지를 검증하여야 한다.\n 한편, 함수가 정상적인 흐름에서 실행되었는지를 탐지해주는 것도 좋은 방법이다. 각각의 함수 내부에 플래그를 설정해두고, 어떤 함수가 실행되었을 때 플래그를 확인하여 정상적이지 않은 플래그를 가진다면 에러로 처리하는 것이다.\n";
                para_title_5_2_3_1_content.Format.SpaceAfter = 10;
                para_title_5_2_3_1_content.Range.InsertParagraphAfter();

                // title13 #5
                Word.Paragraph para_title_5_2_3_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_2.Range.Text = "Guide 2) 불필요한 함수 제거";
                para_title_5_2_3_2.Range.Font.Bold = 1;
                para_title_5_2_3_2.Range.Font.Size = 10;
                para_title_5_2_3_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_2.Format.SpaceAfter = 10;
                para_title_5_2_3_2.Range.InsertParagraphAfter();

                // title13 content #5
                Word.Paragraph para_title_5_2_3_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_2_content.Range.Font.Bold = 0;
                para_title_5_2_3_2_content.Range.Font.Size = 10;
                para_title_5_2_3_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_2_content.Range.Text = " 클래스 내부의 변수에 접근할 수 있는 함수 중 불필요한 함수는 제거해주어야 한다. 일반적으로 많이 사용되는 getter와 setter는 후킹 공격에 이용하기 좋은 함수이다. 이런 함수가 실제로 사용되고 있지 않다면, 제거해주는 것이 바람직하다.\n";
                para_title_5_2_3_2_content.Format.SpaceAfter = 10;
                para_title_5_2_3_2_content.Range.InsertParagraphAfter();

                // title14 #5
                Word.Paragraph para_title_5_2_3_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_3.Range.Text = "Guide 3) 서버 검증";
                para_title_5_2_3_3.Range.Font.Bold = 1;
                para_title_5_2_3_3.Range.Font.Size = 10;
                para_title_5_2_3_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_3.Format.SpaceAfter = 10;
                para_title_5_2_3_3.Range.InsertParagraphAfter();

                // title14 content #5
                Word.Paragraph para_title_5_2_3_3_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_3_content.Range.Font.Bold = 0;
                para_title_5_2_3_3_content.Range.Font.Size = 10;
                para_title_5_2_3_3_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_3_content.Range.Text = " 추가적인 보안성 향상을 위해서는 서버에서 중요 데이터 변조 여부를 검증하는 것이 바람직하다. 급격한 값의 변화를 탐지하거나 게임 플레이 로그와 플레이 시간 등을 고려하여 비정상적인 데이터를 탐지하는 방법 등이 있다.\n";
                para_title_5_2_3_3_content.Format.SpaceAfter = 10;
                para_title_5_2_3_3_content.Range.InsertParagraphAfter();

                // title15 #5
                Word.Paragraph para_title_5_2_3_4 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_4.Range.Text = "Guide 4) 보안 솔루션 적용";
                para_title_5_2_3_4.Range.Font.Bold = 1;
                para_title_5_2_3_4.Range.Font.Size = 10;
                para_title_5_2_3_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_4.Format.SpaceAfter = 10;
                para_title_5_2_3_4.Range.InsertParagraphAfter();

                // title15 content #5
                Word.Paragraph para_title_5_2_3_4_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_3_4_content.Range.Font.Bold = 0;
                para_title_5_2_3_4_content.Range.Font.Size = 10;
                para_title_5_2_3_4_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_3_4_content.Range.Text = " 시중에서 구할 수 있는 솔루션을 적용하는 것도 하나의 방법이다. 참고자료 IV.1, IV.2에서 “Debugging Detection”, “Hacking Tool Detection” 기능이 지원되는 앱 보안 솔루션 혹은 게임 보안 솔루션을 적용하면 함수 후킹을 방지할 수 있다. 그럼에도 불구하고 솔루션이 우회되는 경우가 존재하므로 Guide 1, 2, 3의 대응방안을 추가적으로 적용하는 것이 바람직하다.\n";
                para_title_5_2_3_4_content.Format.SpaceAfter = 50;
                para_title_5_2_3_4_content.Range.InsertParagraphAfter();

                para_num++;
            }

            if(is_memory == 1 && ocr_list.Count > 0)
            {
                // title #6
                Word.Paragraph para_title_6_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_1.Range.Text = para_num.ToString() + ". 메모리 변조 공격 가능성";
                para_title_6_1.Range.Font.Bold = 1;
                para_title_6_1.Range.Font.Size = 14;
                para_title_6_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_1.Format.SpaceAfter = 20;
                para_title_6_1.Range.InsertParagraphAfter();

                // title2 #6
                Word.Paragraph para_title_6_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_2.Range.Font.Bold = 1;
                para_title_6_2.Range.Font.Size = 12;
                para_title_6_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_2.Range.Text = para_num.ToString() + ".1. 취약점 소개";
                para_title_6_2.Format.SpaceAfter = 20;
                para_title_6_2.Range.InsertParagraphAfter();

                // title2 content #6
                Word.Paragraph para_title_6_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_2_content.Range.Font.Bold = 0;
                para_title_6_2_content.Range.Font.Size = 10;
                para_title_6_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_2_content.Range.Text = " 메모리 변조 공격이란, 게임 프로세스가 사용하는 메모리에 직접 접근하여 그 값을 수정하는 것을 말한다. 메모리 변조 공격을 통해 게임 내의 중요 데이터를 변조할 수 있으며, 게임의 정상적인 동작을 방해할 수도 있다.\n 메모리 변조 공격은 가장 일반적이고 자주 사용되는 공격 방법이다. “Cheat Engine”이나 “GameGuardian”을 이용하면 전문 지식 없이도 쉽게 메모리를 스캔하고 변조할 수 있다. 따라서 메모리 변조 공격에 대한 보안은 특히 중요하다.";
                para_title_6_2_content.Format.SpaceAfter = 20;
                para_title_6_2_content.Range.InsertParagraphAfter();

                // title3 #6
                Word.Paragraph para_title_6_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_3.Range.Text = para_num.ToString() + ".2. 취약점 진단 결과";
                para_title_6_3.Range.Font.Bold = 1;
                para_title_6_3.Range.Font.Size = 12;
                para_title_6_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_3.Format.SpaceAfter = 20;
                para_title_6_3.Range.InsertParagraphAfter();

                // title3 content #6
                Word.Paragraph para_title_6_3_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_3_content.Range.Font.Bold = 0;
                para_title_6_3_content.Range.Font.Size = 10;
                para_title_6_3_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_3_content.Range.Text = " 해당 게임에서 메모리 변조 공격 가능성을 진단한 결과, 총 " + ocr_list.Count.ToString() + "개의 지점에서 게임의 중요 데이터가 노출됨을 확인했다. 그 목록은 다음과 같다.";
                para_title_6_3_content.Format.SpaceAfter = 10;
                para_title_6_3_content.Range.InsertParagraphAfter();

                List<List<String>> value_list = new List<List<String>>();
                List<List<String>> class_list = new List<List<String>>();

                for(int i = 0; i < ocr_list.Count; i++)
                {
                    if(ocr_list[i][0] == "Value") { value_list.Add(ocr_list[i]); }
                    else if(ocr_list[i][0] == "Class") { class_list.Add(ocr_list[i]); }
                }

                Word.Paragraph para_title_6_3_1_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_3_1_content.Range.Font.Bold = 1;
                para_title_6_3_1_content.Range.Font.Size = 10;
                para_title_6_3_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_3_1_content.Range.Text = "Scan Result 1) Value 검색";
                para_title_6_3_1_content.Format.SpaceAfter = 3;

                // Value Table
                Word.Table table_6_3_1_content;
                Word.Range rng_table_6_3_1_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_6_3_1_content = word_doc.Tables.Add(rng_table_6_3_1_content, 2, 3, ref obj_miss, ref obj_miss);
                table_6_3_1_content.Range.Font.Size = 8;
                table_6_3_1_content.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_6_3_1_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_6_3_1_content.AllowAutoFit = false;

                table_6_3_1_content.Cell(1, 1).Width = 100;
                table_6_3_1_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_6_3_1_content.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_6_3_1_content.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_6_3_1_content.Cell(1, 1).Range.Font.Bold = 1;
                table_6_3_1_content.Cell(1, 1).Range.Text = "Name";

                table_6_3_1_content.Cell(1, 2).Width = 150;
                table_6_3_1_content.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_6_3_1_content.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_6_3_1_content.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_6_3_1_content.Cell(1, 2).Range.Font.Bold = 1;
                table_6_3_1_content.Cell(1, 2).Range.Text = "Address";

                table_6_3_1_content.Cell(1, 3).Width = 200;
                table_6_3_1_content.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_6_3_1_content.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_6_3_1_content.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_6_3_1_content.Cell(1, 3).Range.Font.Bold = 1;
                table_6_3_1_content.Cell(1, 3).Range.Text = "Original Value";

                for (int i = 0; i < value_list.Count; i++)
                {
                    if (i > 0)
                    {
                        table_6_3_1_content.Rows.Add();
                    }
                    table_6_3_1_content.Cell(i + 2, 1).Width = 100;
                    table_6_3_1_content.Cell(i + 2, 1).Range.Font.Bold = 0;
                    table_6_3_1_content.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_6_3_1_content.Cell(i + 2, 1).Range.Text = "알 수 없음";

                    value_list[i][2].Replace("\n", "");
                    table_6_3_1_content.Cell(i + 2, 2).Width = 150;
                    table_6_3_1_content.Cell(i + 2, 2).Range.Font.Bold = 0;
                    table_6_3_1_content.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_6_3_1_content.Cell(i + 2, 2).Range.Text = value_list[i][2].Trim();

                    value_list[i][3].Replace("\n", "");
                    table_6_3_1_content.Cell(i + 2, 3).Width = 200;
                    table_6_3_1_content.Cell(i + 2, 3).Range.Font.Bold = 0;
                    table_6_3_1_content.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_6_3_1_content.Cell(i + 2, 3).Range.Text = value_list[i][3].Trim();
                }
                rng_table_6_3_1_content.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_6_3_1_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_3_1_1.Range.Font.Bold = 0;
                para_title_6_3_1_1.Range.Font.Size = 10;
                para_title_6_3_1_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_3_1_1.Range.Text = "";
                para_title_6_3_1_1.Format.SpaceAfter = 10;
                para_title_6_3_1_1.Range.InsertParagraphAfter();

                Word.Paragraph para_title_6_3_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_3_2_content.Range.Font.Bold = 1;
                para_title_6_3_2_content.Range.Font.Size = 10;
                para_title_6_3_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_3_2_content.Range.Text = "Scan Result 2) Class 검색";
                para_title_6_3_2_content.Format.SpaceAfter = 3;

                // Class Table
                Word.Table table_6_3_2_content;
                Word.Range rng_table_6_3_2_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_6_3_2_content = word_doc.Tables.Add(rng_table_6_3_2_content, 2, 3, ref obj_miss, ref obj_miss);
                table_6_3_2_content.Range.Font.Size = 8;
                table_6_3_2_content.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_6_3_2_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_6_3_2_content.AllowAutoFit = false;

                table_6_3_2_content.Cell(1, 1).Width = 200;
                table_6_3_2_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_6_3_2_content.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_6_3_2_content.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_6_3_2_content.Cell(1, 1).Range.Font.Bold = 1;
                table_6_3_2_content.Cell(1, 1).Range.Text = "Class";

                table_6_3_2_content.Cell(1, 2).Width = 150;
                table_6_3_2_content.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_6_3_2_content.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_6_3_2_content.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_6_3_2_content.Cell(1, 2).Range.Font.Bold = 1;
                table_6_3_2_content.Cell(1, 2).Range.Text = "Name";

                table_6_3_2_content.Cell(1, 3).Width = 100;
                table_6_3_2_content.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_6_3_2_content.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_6_3_2_content.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_6_3_2_content.Cell(1, 3).Range.Font.Bold = 1;
                table_6_3_2_content.Cell(1, 3).Range.Text = "Address";

                for (int i = 0; i < class_list.Count; i++)
                {
                    if (i > 0)
                    {
                        table_6_3_2_content.Rows.Add();
                    }
                    class_list[i][1].Replace("\n", "");
                    table_6_3_2_content.Cell(i + 2, 1).Width = 200;
                    table_6_3_2_content.Cell(i + 2, 1).Range.Font.Bold = 0;
                    table_6_3_2_content.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_6_3_2_content.Cell(i + 2, 1).Range.Text = class_list[i][1].Trim();

                    class_list[i][2].Replace("\n", "");
                    table_6_3_2_content.Cell(i + 2, 2).Width = 150;
                    table_6_3_2_content.Cell(i + 2, 2).Range.Font.Bold = 0;
                    table_6_3_2_content.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_6_3_2_content.Cell(i + 2, 2).Range.Text = class_list[i][2].Trim();

                    class_list[i][3].Replace("\n", "");
                    table_6_3_2_content.Cell(i + 2, 3).Width = 100;
                    table_6_3_2_content.Cell(i + 2, 3).Range.Font.Bold = 0;
                    table_6_3_2_content.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_6_3_2_content.Cell(i + 2, 3).Range.Text = class_list[i][3].Trim();
                }
                rng_table_6_3_2_content.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_6_3_3_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_3_3_1.Range.Font.Bold = 0;
                para_title_6_3_3_1.Range.Font.Size = 10;
                para_title_6_3_3_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_3_3_1.Range.Text = "";
                para_title_6_3_3_1.Format.SpaceAfter = 10;
                para_title_6_3_3_1.Range.InsertParagraphAfter();

                // title4 #6
                Word.Paragraph para_title_6_4 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_4.Range.Text = para_num.ToString() + ".3. 대응방안";
                para_title_6_4.Range.Font.Bold = 1;
                para_title_6_4.Range.Font.Size = 12;
                para_title_6_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_4.Format.SpaceAfter = 10;
                para_title_6_4.Range.InsertParagraphAfter();

                // title5 #6
                Word.Paragraph para_title_6_5 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_5.Range.Text = "Guide 1) 변수 난독화";
                para_title_6_5.Range.Font.Bold = 1;
                para_title_6_5.Range.Font.Size = 10;
                para_title_6_5.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_5.Format.SpaceAfter = 10;
                para_title_6_5.Range.InsertParagraphAfter();

                // title5 content #6
                Word.Paragraph para_title_6_5_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_5_content.Range.Font.Bold = 0;
                para_title_6_5_content.Range.Font.Size = 10;
                para_title_6_5_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_5_content.Range.Text = " HP, Damage 등 게임 진행에 직접적으로 영향을 끼치는 주요 변수를 여러 변수로 나누어 저장하거나, 다른 주요 변수와 산술 연산을 거친 후 저장함으로써 사용자에게 노출되는 데이터가 프로세스 메모리에 직접적으로 노출되지 않도록 구현할 수 있다. 위 방법을 통해 메모리 스캐닝 공격으로부터 중요 변수의 위치 노출을 피할 수 있다.\n";
                para_title_6_5_content.Format.SpaceAfter = 10;
                para_title_6_5_content.Range.InsertParagraphAfter();
                
                // title6 #6
                Word.Paragraph para_title_6_6 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_6.Range.Text = "Guide 2) 중요 데이터 암호화";
                para_title_6_6.Range.Font.Bold = 1;
                para_title_6_6.Range.Font.Size = 10;
                para_title_6_6.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_6.Format.SpaceAfter = 10;
                para_title_6_6.Range.InsertParagraphAfter();

                // title6 content #6
                Word.Paragraph para_title_6_6_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_6_content.Range.Font.Bold = 0;
                para_title_6_6_content.Range.Font.Size = 10;
                para_title_6_6_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_6_content.Range.Text = " 게임 내의 중요 데이터를 암호화하는 것은 메모리 스캔 및 변조를 막기 위해 가장 기본적으로 해야 할 일이다. 암호화 방법으로 XOR만을 사용하는 것은 단순 리버싱만으로도 XOR 키를 찾아낼 수 있어 취약하다. 암호화 방법으로는 AES 알고리즘을 사용하는 것을 권장한다. 보다 심화된 보안을 원한다면 별도의 쓰레드를 생성하여 암호화 키를 주기적으로 변경하는 방법을 사용해도 좋다. 암호화 알고리즘에 대해서는 참고자료 IV.3을 참고하면 더 자세한 정보를 알 수 있다.\n";
                para_title_6_6_content.Format.SpaceAfter = 10;
                para_title_6_6_content.Range.InsertParagraphAfter();

                // title7 #6
                Word.Paragraph para_title_6_7 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_7.Range.Text = "Guide 3) 서버 유효성 검증";
                para_title_6_7.Range.Font.Bold = 1;
                para_title_6_7.Range.Font.Size = 10;
                para_title_6_7.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_7.Format.SpaceAfter = 10;
                para_title_6_7.Range.InsertParagraphAfter();

                // title7 content #6
                Word.Paragraph para_title_6_7_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_7_content.Range.Font.Bold = 0;
                para_title_6_7_content.Range.Font.Size = 10;
                para_title_6_7_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_7_content.Range.Text = " 클라이언트의 메모리 데이터를 신뢰할 수 없다면, 서버에서 데이터 유효성 검사를 진행하는 것이 바람직하다. 클라이언트 환경에서의 메모리는 사용자에 의해 변경 가능하기 때문에 신뢰해서는 안된다. 따라서 클라이언트에서의 중요한 정보일수록 서버를 경유하여 처리하는 것이 좋고, 다양한 타이밍에 보다 많은 정보를 서버가 검사할수록 더욱 안전하다.\n";
                para_title_6_7_content.Format.SpaceAfter = 10;
                para_title_6_7_content.Range.InsertParagraphAfter();

                // title8 #6
                Word.Paragraph para_title_6_8 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_8.Range.Text = "Guide 4) 로드된 DLL 주기적 검증";
                para_title_6_8.Range.Font.Bold = 1;
                para_title_6_8.Range.Font.Size = 10;
                para_title_6_8.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_8.Format.SpaceAfter = 10;
                para_title_6_8.Range.InsertParagraphAfter();

                // title8 content #6
                Word.Paragraph para_title_6_8_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_8_content.Range.Font.Bold = 0;
                para_title_6_8_content.Range.Font.Size = 10;
                para_title_6_8_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_8_content.Range.Text = " 주기적으로 게임 프로세스 메모리 내에 로드되어 있는 DLL을 체크하는 방법이다. 본래 사용되는DLL의 경우에는 해시값을 통해 무결성을 검증하도록 하고, 이외의 DLL 삽입에 대해서는 에러 처리를 하는 것이 바람직하다.\n";
                para_title_6_8_content.Format.SpaceAfter = 10;
                para_title_6_8_content.Range.InsertParagraphAfter();

                // title9 #6
                Word.Paragraph para_title_6_9 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_9.Range.Text = "Guide 5) 보안 솔루션 적용";
                para_title_6_9.Range.Font.Bold = 1;
                para_title_6_9.Range.Font.Size = 10;
                para_title_6_9.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_9.Format.SpaceAfter = 10;
                para_title_6_9.Range.InsertParagraphAfter();

                // title9 content #6
                Word.Paragraph para_title_6_9_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_6_9_content.Range.Font.Bold = 0;
                para_title_6_9_content.Range.Font.Size = 10;
                para_title_6_9_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_6_9_content.Range.Text = " 시중에서 구할 수 있는 솔루션을 적용하는 것도 하나의 방법이다. 참고자료 IV.1, IV.2에서 “Memory Protection” 기능이 지원되는 앱 보안 솔루션 혹은 게임 보안 솔루션을 적용하면 메모리 변조를 방지할 수 있다. 그럼에도 불구하고 솔루션이 우회되는 경우가 존재하므로 Guide 1, 2, 3의 대응방안을 추가적으로 적용하는 것이 바람직하다.\n";
                para_title_6_9_content.Format.SpaceAfter = 50;
                para_title_6_9_content.Range.InsertParagraphAfter();

                para_num++;
            }

            if(table_list.Count > 0 && itemTables.Count > 0 && is_db == 1)
            {
                // title #7
                Word.Paragraph para_title_7_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_1.Range.Text = para_num.ToString() + ". 저장 데이터 변조 가능성";
                para_title_7_1.Range.Font.Bold = 1;
                para_title_7_1.Range.Font.Size = 14;
                para_title_7_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_1.Format.SpaceAfter = 20;
                para_title_7_1.Range.InsertParagraphAfter();

                // title2 #7
                Word.Paragraph para_title_7_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_2.Range.Font.Bold = 1;
                para_title_7_2.Range.Font.Size = 12;
                para_title_7_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_2.Range.Text = para_num.ToString() + ".1. 취약점 소개";
                para_title_7_2.Format.SpaceAfter = 10;
                para_title_7_2.Range.InsertParagraphAfter();

                // title2 content #7
                Word.Paragraph para_title_7_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_2_content.Range.Font.Bold = 0;
                para_title_7_2_content.Range.Font.Size = 10;
                para_title_7_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_2_content.Range.Text = " 저장 데이터 변조 공격이란, 게임에서 사용하는 저장 파일(PlayerPrefs)이나 DB 파일(SQLite3)을 변조하는 것이다. 저장 파일에는 게임의 진행 상황 이외에도 유저의 로그인 토큰이나 서버에서의 ID 값이 저장되는 경우도 있으므로 이를 보호하는 것은 중요하다.";
                para_title_7_2_content.Format.SpaceAfter = 10;
                para_title_7_2_content.Range.InsertParagraphAfter();

                // title3 #7
                Word.Paragraph para_title_7_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_3.Range.Text = para_num.ToString() + ".2. 취약점 진단 결과";
                para_title_7_3.Range.Font.Bold = 1;
                para_title_7_3.Range.Font.Size = 12;
                para_title_7_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_3.Format.SpaceAfter = 10;
                para_title_7_3.Range.InsertParagraphAfter();

                // title3 content #7
                Word.Paragraph para_title_7_3_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_3_content.Range.Font.Bold = 0;
                para_title_7_3_content.Range.Font.Size = 10;
                para_title_7_3_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

                // table_list, itemTables
                int is_pp = 0, is_sql = 0;
                for(int i = 0; i < table_list.Count; i++)
                {
                    if (table_list[i].ToLower().Contains("playerprefs")) { is_pp = 1; }
                    else { is_sql = 1; }
                }

                if (is_pp == 0 && is_sql == 1) { para_title_7_3_content.Range.Text = " 취약점 진단 결과 Sqlite3에 대해서 중요 정보 노출 및 변조 가능성을 확인하였다."; }
                else if (is_pp == 1 && is_sql == 0) { para_title_7_3_content.Range.Text = " 취약점 진단 결과 PlayerPrefs에 대해서 중요 정보 노출 및 변조 가능성을 확인하였다."; }
                else if (is_pp == 1 && is_sql == 1) { para_title_7_3_content.Range.Text = " 취약점 진단 결과 Sqlite3와 PlayerPrefs에 대해서 중요 정보 노출 및 변조 가능성을 확인하였다."; }
                para_title_7_3_content.Format.SpaceAfter = 10;
                para_title_7_3_content.Range.InsertParagraphAfter();

                List<StringBuilder> db_sb_list = new List<StringBuilder>();
                StringBuilder _sb;
                int _idx1 = 0;
                for(int i = 0; i < table_list.Count; i++)
                {
                    _sb = new StringBuilder();
                    _idx1 = table_list[i].IndexOf("{");
                    String _temp_str = table_list[i].Substring(_idx1).Trim();
                    _temp_str = _temp_str.Substring(1, _temp_str.Length - 2);
                    String[] a_str = _temp_str.Split('@');
                    _sb.Append("[File: " + a_str[0] + ", Table: " + a_str[1] + "]");
                    db_sb_list.Add(_sb);
                }

                Word.Paragraph para_title_5_2_2_6 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_2_6.Range.Font.Bold = 0;
                para_title_5_2_2_6.Range.Font.Size = 10;
                para_title_5_2_2_6.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_2_6.Range.Text = "";
                para_title_5_2_2_6.Format.SpaceAfter = 1;

                int rc = 0, cc = 0, x = 1, y = 0;
                for (int i = 0; i < itemTables.Count; i++)
                {
                    if (i > 0) { 
                        y = 0;
                        Word.Paragraph para_title_5_2_2_4 = word_doc.Paragraphs.Add(ref obj_miss);
                        para_title_5_2_2_4.Range.Font.Bold = 0;
                        para_title_5_2_2_4.Range.Font.Size = 10;
                        para_title_5_2_2_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        para_title_5_2_2_4.Range.Text = "";
                        para_title_5_2_2_4.Format.SpaceAfter = 10;
                        para_title_5_2_2_4.Range.InsertParagraphAfter();

                        Word.Paragraph para_title_5_2_2_5 = word_doc.Paragraphs.Add(ref obj_miss);
                        para_title_5_2_2_5.Range.Font.Bold = 0;
                        para_title_5_2_2_5.Range.Font.Size = 10;
                        para_title_5_2_2_5.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                        para_title_5_2_2_5.Range.Text = "";
                        para_title_5_2_2_5.Format.SpaceAfter = 1;
                    }
                    // space

                    cc = itemTables[i].Columns.Count;
                    rc = itemTables[i].Rows.Count;
                    Word.Table table_7_3_2_content;
                    Word.Range rng_table_7_3_2_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                    table_7_3_2_content = word_doc.Tables.Add(rng_table_7_3_2_content, rc + 1, cc, ref obj_miss, ref obj_miss);
                    table_7_3_2_content.Range.Font.Size = 8;
                    table_7_3_2_content.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table_7_3_2_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    table_7_3_2_content.AllowAutoFit = false;

                    for(int j = 0; j < cc; j++)
                    {
                        table_7_3_2_content.Cell(1, j + 1).Width = 450 / cc;
                        table_7_3_2_content.Cell(1, j + 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                        table_7_3_2_content.Cell(1, j + 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                        table_7_3_2_content.Cell(1, j + 1).Range.Font.Color = Word.WdColor.wdColorWhite;
                        table_7_3_2_content.Cell(1, j + 1).Range.Font.Bold = 1;
                        table_7_3_2_content.Cell(1, j + 1).Range.Text = itemTables[i].Columns[j].ColumnName.Trim();
                    }

                    foreach (DataRow row in itemTables[i].Rows)
                    {
                        x = 1;
                        foreach (DataColumn column in itemTables[i].Columns)
                        {
                            table_7_3_2_content.Cell(y + 2, x).Width = 450 / cc;
                            table_7_3_2_content.Cell(y + 2, x).Range.Font.Bold = 0;
                            table_7_3_2_content.Cell(y + 2, x).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            table_7_3_2_content.Cell(y + 2, x).Range.Text = row[column].ToString().Trim();
                            x++;
                        }
                        y++;
                    }
                    //if((i + 1) == itemTables.Count) { rng_table_7_3_2_content.InsertParagraphAfter(); }

                    Word.Paragraph para_table_7_1_title = word_doc.Paragraphs.Add(ref obj_miss);
                    para_table_7_1_title.Range.Text = db_sb_list[i].ToString();
                    para_table_7_1_title.Range.Font.Bold = 1;
                    para_table_7_1_title.Range.Font.Size = 10;
                    para_table_7_1_title.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    para_table_7_1_title.Format.SpaceAfter = 10;
                    para_table_7_1_title.Range.InsertParagraphAfter();
                }

                // space
                Word.Paragraph para_title_5_2_2_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_5_2_2_3.Range.Font.Bold = 0;
                para_title_5_2_2_3.Range.Font.Size = 10;
                para_title_5_2_2_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_5_2_2_3.Range.Text = "";
                para_title_5_2_2_3.Format.SpaceAfter = 10;
                para_title_5_2_2_3.Range.InsertParagraphAfter();

                // title4 #7
                Word.Paragraph para_title_7_4 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_4.Range.Text = para_num.ToString() + ".3. 대응방안";
                para_title_7_4.Range.Font.Bold = 1;
                para_title_7_4.Range.Font.Size = 12;
                para_title_7_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_4.Format.SpaceAfter = 10;
                para_title_7_4.Range.InsertParagraphAfter();

                // title5 #7
                Word.Paragraph para_title_7_5 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_5.Range.Text = "Guide 1) 데이터 암호화";
                para_title_7_5.Range.Font.Bold = 1;
                para_title_7_5.Range.Font.Size = 10;
                para_title_7_5.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_5.Format.SpaceAfter = 10;
                para_title_7_5.Range.InsertParagraphAfter();

                // title5 content #7
                Word.Paragraph para_title_7_5_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_5_content.Range.Font.Bold = 0;
                para_title_7_5_content.Range.Font.Size = 10;
                para_title_7_5_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_5_content.Range.Text = " 저장 파일과 DB 파일은 클라이언트 환경에서 특정 폴더에 파일이 생성되기 때문에 사용자가 언제든 접근하여 수정할 수 있다는 특징을 가진다. 따라서 데이터를 암호화하지 않고 저장하는 경우에는 어떠한 전문 지식 없이도 쉽게 값을 변조할 수 있다. 특히 URL 인코딩이나 Base64 인코딩과 암호화를 착각하는 경우가 있는데, 인코딩은 가독성을 떨어뜨리는 역할을 할 뿐 보안성을 증가시키지는 않는다.\n 암호화 방법으로 XOR만을 사용하는 것은 단순 리버싱만으로도 XOR 키를 찾아낼 수 있어 취약하다. 암호화 방법으로는 AES 알고리즘을 사용하는 것을 권장한다.암호화 알고리즘에 대해서는 참고자료 IV.3을 참고하면 더 자세한 정보를 알 수 있다.\n";
                para_title_7_5_content.Format.SpaceAfter = 10;
                para_title_7_5_content.Range.InsertParagraphAfter();

                // title6 #7
                Word.Paragraph para_title_7_6 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_6.Range.Text = "Guide 2) 무결성 검증";
                para_title_7_6.Range.Font.Bold = 1;
                para_title_7_6.Range.Font.Size = 10;
                para_title_7_6.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_6.Format.SpaceAfter = 10;
                para_title_7_6.Range.InsertParagraphAfter();

                // title6 content #7
                Word.Paragraph para_title_7_6_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_6_content.Range.Font.Bold = 0;
                para_title_7_6_content.Range.Font.Size = 10;
                para_title_7_6_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_6_content.Range.Text = " 저장 데이터에 대해서 해시값을 통해 무결성을 검증하는 것도 변조를 막기 위한 좋은 방법이다. 해시 알고리즘으로 MD5 보다는 SHA-256이나 SHA-512를 쓰는 것이 바람직하다. 해시값을 생성할 때에도 랜덤 salt 값을 생성하여, 저장 데이터와 salt 값을 포함한 해시값을 생성하는 것이 보다 안전하다. 서버에서 진행하기를 권고한다.\n";
                para_title_7_6_content.Format.SpaceAfter = 10;
                para_title_7_6_content.Range.InsertParagraphAfter();

                // title7 #7
                Word.Paragraph para_title_7_7 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_7.Range.Text = "Guide 3) 보안 솔루션 적용";
                para_title_7_7.Range.Font.Bold = 1;
                para_title_7_7.Range.Font.Size = 10;
                para_title_7_7.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_7.Format.SpaceAfter = 10;
                para_title_7_7.Range.InsertParagraphAfter();

                // title7 content #7
                Word.Paragraph para_title_7_7_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_7_7_content.Range.Font.Bold = 0;
                para_title_7_7_content.Range.Font.Size = 10;
                para_title_7_7_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_7_7_content.Range.Text = " 시중에서 구할 수 있는 솔루션을 적용하는 것도 하나의 방법이다. 참고자료 IV.2에서 “Interity Validation” 기능이 지원되는 게임 보안 솔루션을 적용하면 저장 데이터 변조를 방지할 수 있다. 그럼에도 불구하고 솔루션이 우회되는 경우가 존재하므로 Guide 1, 2의 대응방안을 추가적으로 적용하는 것이 바람직하다.\n";
                para_title_7_7_content.Format.SpaceAfter = 50;
                para_title_7_7_content.Range.InsertParagraphAfter();

                para_num++;
            }

            int is_t_exist = 0;
            for(int i = 0; i < T_status.Count; i++)
            {
                if(T_status[i] == 1) { is_t_exist++; }
            }

            if (is_t_exist > 0 && is_speed == 1 && time_report.Count > 0)
            {
                // title #8
                Word.Paragraph para_title_8_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_1.Range.Text = para_num.ToString() + ". 시간 정보 조작 가능성";
                para_title_8_1.Range.Font.Bold = 1;
                para_title_8_1.Range.Font.Size = 14;
                para_title_8_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_1.Format.SpaceAfter = 20;
                para_title_8_1.Range.InsertParagraphAfter();

                // title2 #8
                Word.Paragraph para_title_8_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_2.Range.Font.Bold = 1;
                para_title_8_2.Range.Font.Size = 12;
                para_title_8_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_2.Range.Text = para_num.ToString() + ".1. 취약점 소개";
                para_title_8_2.Format.SpaceAfter =  10;
                para_title_8_2.Range.InsertParagraphAfter();

                // title2 content #8
                Word.Paragraph para_title_8_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_2_content.Range.Font.Bold = 0;
                para_title_8_2_content.Range.Font.Size = 10;
                para_title_8_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_2_content.Range.Text = " 시간 정보 조작이란, 시간 정보나 시간의 속도를 변경하여 시간에 관련된 게임의 흐름을 비정상적으로 바꾸는 것을 말한다. 시간 정보를 조작하는 방법은 시간에 관련된 함수를 후킹하는 방법과 시스템에서 사용하는 PIT(Programmable Interval Timer) 칩을 조작하는 방법이 있다. 공격자는 시간의 속도를 높여 게임을 빠르게 끝내거나, 시간의 속도를 늦춰 게임의 난이도를 낮출 수도 있다. 멀티플레이 게임에서 “스피드핵”이라고 불리는 것이 시간 정보 조작의 한 예시이다.";
                para_title_8_2_content.Format.SpaceAfter = 10;
                para_title_8_2_content.Range.InsertParagraphAfter();

                // title3 #8
                Word.Paragraph para_title_8_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_3.Range.Text = para_num.ToString() + ".2. 취약점 진단 결과";
                para_title_8_3.Range.Font.Bold = 1;
                para_title_8_3.Range.Font.Size = 12;
                para_title_8_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_3.Format.SpaceAfter = 10;
                para_title_8_3.Range.InsertParagraphAfter();

                // title3 content #8
                Word.Paragraph para_title_8_3_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_3_content.Range.Font.Bold = 0;
                para_title_8_3_content.Range.Font.Size = 10;
                para_title_8_3_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                if (T_status.Count > 0 && is_speed == 1) { para_title_8_3_content.Range.Text = " 해당 게임에서 시간 정보 조작 가능성을 진단한 결과, 아래와 같이 " + is_t_exist.ToString() + "개의 취약한 함수 사용이 발견되었다."; }
                else { para_title_8_3_content.Range.Text = " 시간 함수 목록이 비어있어서 진단 불가"; }
                para_title_8_3_content.Format.SpaceAfter = 1;

                // title3 table #8
                Word.Table table_8_3_content;
                Word.Range rng_table_8_3_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_8_3_content = word_doc.Tables.Add(rng_table_8_3_content, 2, 3, ref obj_miss, ref obj_miss);
                table_8_3_content.Range.Font.Size = 8;
                table_8_3_content.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_8_3_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_8_3_content.AllowAutoFit = false;

                table_8_3_content.Cell(1, 1).Width = 185;
                table_8_3_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_8_3_content.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_8_3_content.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_8_3_content.Cell(1, 1).Range.Font.Bold = 1;
                table_8_3_content.Cell(1, 1).Range.Text = "Function";

                table_8_3_content.Cell(1, 2).Width = 100;
                table_8_3_content.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_8_3_content.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_8_3_content.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_8_3_content.Cell(1, 2).Range.Font.Bold = 1;
                table_8_3_content.Cell(1, 2).Range.Text = "Detail";

                table_8_3_content.Cell(1, 3).Width = 165;
                table_8_3_content.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_8_3_content.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_8_3_content.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_8_3_content.Cell(1, 3).Range.Font.Bold = 1;
                table_8_3_content.Cell(1, 3).Range.Text = "Memory Address";

                List<String> t_f_list = new List<String>();
                for (int i = 0; i < 2; i++)
                {
                    if (T_status[i] == 1)
                    {
                        if (i == 0) { t_f_list.Add("gettimeofday"); }
                        else if (i == 1) { t_f_list.Add("clock_gettime"); }
                    }
                }

                String[] tr;
                String time_m_a;

                for (int i = 0; i < t_f_list.Count; i++)
                {
                    time_m_a = null;
                    if (i > 0)
                    {
                        table_8_3_content.Rows.Add();
                    }
                    for(int j = 0; j < time_report.Count; j++)
                    {
                        tr = time_report[j].Split('@');
                        if (tr[0].Contains(t_f_list[i])) { time_m_a = tr[1]; break; }
                    }
                    table_8_3_content.Cell(i + 2, 1).Width = 185;
                    table_8_3_content.Cell(i + 2, 1).Range.Font.Bold = 0;
                    table_8_3_content.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_8_3_content.Cell(i + 2, 1).Range.Text = t_f_list[i];

                    table_8_3_content.Cell(i + 2, 2).Width =  100;
                    table_8_3_content.Cell(i + 2, 2).Range.Font.Bold = 0;
                    table_8_3_content.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_8_3_content.Cell(i + 2, 2).Range.Text = "Called";

                    table_8_3_content.Cell(i + 2, 3).Width = 165;
                    table_8_3_content.Cell(i + 2, 3).Range.Font.Bold = 0;
                    table_8_3_content.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_8_3_content.Cell(i + 2, 3).Range.Text = time_m_a.Trim();
                }
                rng_table_8_3_content.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_8_3_2_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_3_2_2.Range.Font.Bold = 0;
                para_title_8_3_2_2.Range.Font.Size = 10;
                para_title_8_3_2_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_3_2_2.Range.Text = "";
                para_title_8_3_2_2.Format.SpaceAfter = 10;
                para_title_8_3_2_2.Range.InsertParagraphAfter();

                // title4 #8
                Word.Paragraph para_title_8_4 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_4.Range.Text = para_num.ToString() + ".3. 대응방안";
                para_title_8_4.Range.Font.Bold = 1;
                para_title_8_4.Range.Font.Size = 12;
                para_title_8_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_4.Format.SpaceAfter = 10;
                para_title_8_4.Range.InsertParagraphAfter();

                // title5 #8
                Word.Paragraph para_title_8_5 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_5.Range.Text = "Guide 1) 서버의 시간과 클라이언트의 시간 동기화";
                para_title_8_5.Range.Font.Bold = 1;
                para_title_8_5.Range.Font.Size = 10;
                para_title_8_5.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_5.Format.SpaceAfter = 10;
                para_title_8_5.Range.InsertParagraphAfter();

                // title5 content #8
                Word.Paragraph para_title_8_5_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_5_content.Range.Font.Bold = 0;
                para_title_8_5_content.Range.Font.Size = 10;
                para_title_8_5_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_5_content.Range.Text = " 단순히 클라이언트상의 비교만으로는 시간 정보를 조작하였는지 판단하기 어렵기 때문에, 서버와의 동기화를 진행하는 것이 바람직하다. 클라이언트 시간과 서버 시간을 동기화하는 주기는 게임의 특성과 서버 환경에 따라 적절히 선택해주는 것이 필요하다.\n";
                para_title_8_5_content.Format.SpaceAfter = 10;
                para_title_8_5_content.Range.InsertParagraphAfter();

                // title6 #8
                Word.Paragraph para_title_8_6 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_6.Range.Text = "Guide 2) 보안 솔루션 적용";
                para_title_8_6.Range.Font.Bold = 1;
                para_title_8_6.Range.Font.Size = 10;
                para_title_8_6.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_6.Format.SpaceAfter = 10;
                para_title_8_6.Range.InsertParagraphAfter();

                // title6 content #8
                Word.Paragraph para_title_8_6_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_8_6_content.Range.Font.Bold = 0;
                para_title_8_6_content.Range.Font.Size = 10;
                para_title_8_6_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_8_6_content.Range.Text = " 시중에서 구할 수 있는 솔루션을 적용하는 것도 하나의 방법이다. 참고자료 IV.2에서 “Speed Hack Detection”, “Hacking Tool Detection” 기능이 지원되는 게임 보안 솔루션을 적용하면 시간 정보 조작을 방지할 수 있다. 그럼에도 불구하고 솔루션이 우회되는 경우가 존재하므로 Guide 1의 대응방안을 추가적으로 적용하는 것이 바람직하다.\n";
                para_title_8_6_content.Format.SpaceAfter = 50;
                para_title_8_6_content.Range.InsertParagraphAfter();

                para_num++;
            }

            int is_r_exist = 0;
            for (int i = 0; i < R_status.Count; i++)
            {
                if (R_status[i] == 1) { is_r_exist++; }
            }

            if (is_r_exist > 0 && is_random == 1 && rand_report.Count > 0)
            {
                // title #9
                Word.Paragraph para_title_9_1 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_1.Range.Text = para_num.ToString() + ". 난수 생성 함수 조작 가능성";
                para_title_9_1.Range.Font.Bold = 1;
                para_title_9_1.Range.Font.Size = 14;
                para_title_9_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_1.Format.SpaceAfter = 20;
                para_title_9_1.Range.InsertParagraphAfter();

                // title2 #9
                Word.Paragraph para_title_9_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_2.Range.Font.Bold = 1;
                para_title_9_2.Range.Font.Size = 12;
                para_title_9_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_2.Range.Text = para_num.ToString() + ".1. 취약점 소개";
                para_title_9_2.Format.SpaceAfter = 10;
                para_title_9_2.Range.InsertParagraphAfter();

                // title2 content #9
                Word.Paragraph para_title_9_2_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_2_content.Range.Font.Bold = 0;
                para_title_9_2_content.Range.Font.Size = 10;
                para_title_9_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_2_content.Range.Text = " 난수 생성 함수 조작이란, 안드로이드 시스템 혹은 Unity에서 기본적으로 제공하는 랜덤 함수를 이용하는 경우에 관련 함수를 후킹하여 확률 관련 정보를 조작하는 것이다. 이를 이용하면 게임 내에서 치명타 확률이나, 확률형 뽑기 등 확률을 사용할 경우 결과를 조작할 수 있다.";
                para_title_9_2_content.Format.SpaceAfter = 10;
                para_title_9_2_content.Range.InsertParagraphAfter();

                // title3 #9
                Word.Paragraph para_title_9_3 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_3.Range.Text = para_num.ToString() + ".2. 취약점 진단 결과";
                para_title_9_3.Range.Font.Bold = 1;
                para_title_9_3.Range.Font.Size = 12;
                para_title_9_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_3.Format.SpaceAfter = 10;
                para_title_9_3.Range.InsertParagraphAfter();

                // title3 content #9
                Word.Paragraph para_title_9_3_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_3_content.Range.Font.Bold = 0;
                para_title_9_3_content.Range.Font.Size = 10;
                para_title_9_3_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                if (R_status.Count > 0 && is_random == 1) { para_title_9_3_content.Range.Text = " 해당 게임에서 난수 생성 함수 조작 가능성을 진단한 결과, 아래와 같이 " + is_r_exist.ToString() + "개의 취약한 함수의 사용이 발견되었다."; }
                else { para_title_9_3_content.Range.Text = " 랜덤 함수 목록이 비어있어서 진단 불가"; }
                para_title_9_3_content.Format.SpaceAfter = 1;

                // title3 table #9
                Word.Table table_9_3_content;
                Word.Range rng_table_9_3_content = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_9_3_content = word_doc.Tables.Add(rng_table_9_3_content, 2, 3, ref obj_miss, ref obj_miss);
                table_9_3_content.Range.Font.Size = 8;
                table_9_3_content.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_9_3_content.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_9_3_content.AllowAutoFit = false;

                table_9_3_content.Cell(1, 1).Width = 185;
                table_9_3_content.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_9_3_content.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_9_3_content.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_9_3_content.Cell(1, 1).Range.Font.Bold = 1;
                table_9_3_content.Cell(1, 1).Range.Text = "Function";
                table_9_3_content.Cell(1, 1).Range.Text = table_9_3_content.Cell(1, 1).Range.Text.Replace("\n", "").Trim();

                table_9_3_content.Cell(1, 2).Width = 100;
                table_9_3_content.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_9_3_content.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_9_3_content.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_9_3_content.Cell(1, 2).Range.Font.Bold = 1;
                table_9_3_content.Cell(1, 2).Range.Text = "Detail";

                table_9_3_content.Cell(1, 3).Width = 165;
                table_9_3_content.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                table_9_3_content.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
                table_9_3_content.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
                table_9_3_content.Cell(1, 3).Range.Font.Bold = 1;
                table_9_3_content.Cell(1, 3).Range.Text = "Memory Address";

                List<String> r_f_list = new List<String>();
                for(int i = 0; i < 5; i++)
                {
                    if(R_status[i] == 1) 
                    {
                        if (i == 0) { r_f_list.Add("Random.Next()"); }
                        else if (i == 1) { r_f_list.Add("Random.Next(int maxValue)"); }
                        else if (i == 2) { r_f_list.Add("Random.Next(int minValue, int maxValue)"); }
                        else if (i == 3) { r_f_list.Add("float Random.Range(float min, float max)"); }
                        else if (i == 4) { r_f_list.Add("int Random.Range(int min, int max)"); }
                    }
                }

                String[] rr;
                String rand_m_a;

                for (int i = 0; i < r_f_list.Count; i++)
                {
                    rand_m_a = null;
                    if (i > 0)
                    {
                        table_9_3_content.Rows.Add();
                    }
                    for (int j = 0; j < rand_report.Count; j++)
                    {
                        rr = rand_report[j].Split('@');
                        if (rr[0].Contains(r_f_list[i])) { rand_m_a = rr[1]; break; }
                    }
                    table_9_3_content.Cell(i + 2, 1).Width = 185;
                    table_9_3_content.Cell(i + 2, 1).Range.Font.Bold = 0;
                    table_9_3_content.Cell(i + 2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_9_3_content.Cell(i + 2, 1).Range.Text = r_f_list[i];

                    table_9_3_content.Cell(i + 2, 2).Width = 100;
                    table_9_3_content.Cell(i + 2, 2).Range.Font.Bold = 0;
                    table_9_3_content.Cell(i + 2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_9_3_content.Cell(i + 2, 2).Range.Text = "Called";

                    table_9_3_content.Cell(i + 2, 3).Width = 165;
                    table_9_3_content.Cell(i + 2, 3).Range.Font.Bold = 0;
                    table_9_3_content.Cell(i + 2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    table_9_3_content.Cell(i + 2, 3).Range.Text = rand_m_a.Trim();
                }

                rng_table_9_3_content.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_9_3_2_2 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_3_2_2.Range.Font.Bold = 0;
                para_title_9_3_2_2.Range.Font.Size = 10;
                para_title_9_3_2_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_3_2_2.Range.Text = "";
                para_title_9_3_2_2.Format.SpaceAfter = 10;
                para_title_9_3_2_2.Range.InsertParagraphAfter();

                // title4 #9
                Word.Paragraph para_title_9_4 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_4.Range.Text = para_num.ToString() + ".3. 대응방안";
                para_title_9_4.Range.Font.Bold = 1;
                para_title_9_4.Range.Font.Size = 12;
                para_title_9_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_4.Format.SpaceAfter = 10;
                para_title_9_4.Range.InsertParagraphAfter();

                // title5 #9
                Word.Paragraph para_title_9_5 = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_5.Range.Text = "Guide 1) 직접 생성한 함수 사용";
                para_title_9_5.Range.Font.Bold = 1;
                para_title_9_5.Range.Font.Size = 10;
                para_title_9_5.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_5.Format.SpaceAfter = 10;
                para_title_9_5.Range.InsertParagraphAfter();

                // title5 content #9
                Word.Paragraph para_title_9_5_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_5_content.Range.Font.Bold = 0;
                para_title_9_5_content.Range.Font.Size = 10;
                para_title_9_5_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_5_content.Range.Text = " 아래 예시는 rand 함수와 srand 함수를 직접 구현한 코드이다. 난수 생성하는 함수를 직접 구현하는 것은 쉽고 간단하게 적용 가능하지만 확실한 해결책이므로 적용을 권고한다.";
                para_title_9_5_content.Format.SpaceAfter = 1;

                Word.Table table_9_5_1;
                Word.Range rng_9_5_1 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
                table_9_5_1 = word_doc.Tables.Add(rng_9_5_1, 1, 1, ref obj_miss, ref obj_miss);
                table_9_5_1.Range.Font.Size = 8;
                table_9_5_1.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                table_9_5_1.AllowAutoFit = false;
                table_9_5_1.Cell(1, 1).Width = 450;
                table_9_5_1.Cell(1, 1).Range.Font.Bold = 0;
                table_9_5_1.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                table_9_5_1.Cell(1, 1).Range.Text = "static long holdrand = 1L;\n\nvoid __cdecl srand(unsigned int seed)\n{\n          _getptd()->_holdrand = (unsigned long)seed;\n}\n\nint __cdecl rand(void)\n{\n          _ptiddata ptd = _getptd();\n          return (((ptd->_holdrand = ptd->_holdrand * 214013L + 2531011L) >> 16) & 0x7fff);\n}";
                rng_9_5_1.InsertParagraphAfter();

                // space
                Word.Paragraph para_title_9_5_1_1_content = word_doc.Paragraphs.Add(ref obj_miss);
                para_title_9_5_1_1_content.Range.Font.Bold = 0;
                para_title_9_5_1_1_content.Range.Font.Size = 10;
                para_title_9_5_1_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                para_title_9_5_1_1_content.Range.Text = "";
                para_title_9_5_1_1_content.Format.SpaceAfter = 50;
                para_title_9_5_1_1_content.Range.InsertParagraphAfter();
            }

            // References
            // title #10
            Word.Paragraph para_title_10_1 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_1.Range.Text = "III. 참고 자료";
            para_title_10_1.Range.Font.Bold = 1;
            para_title_10_1.Range.Font.Size = 18;
            para_title_10_1.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_1.Format.SpaceAfter = 20;
            para_title_10_1.Range.InsertParagraphAfter();

            // title2 #10
            Word.Paragraph para_title_10_2 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_2.Range.Text = "1. 모바일 앱 보안 솔루션";
            para_title_10_2.Range.Font.Bold = 1;
            para_title_10_2.Range.Font.Size = 14;
            para_title_10_2.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_2.Format.SpaceAfter = 20;
            para_title_10_2.Range.InsertParagraphAfter();

            // title2 content #10
            Word.Paragraph para_title_10_2_content = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_2_content.Range.Font.Bold = 0;
            para_title_10_2_content.Range.Font.Size = 10;
            para_title_10_2_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_2_content.Range.Text = " 일반적인 모바일 앱들을 보호하기 위한 솔루션들을 기능별로 정리한 표이다. 일반적인 앱 보안 솔루션에서 제공하는 기능 중에서 게임 보안과 밀접한 관련이 있는 내용을 위주로 기능을 정리했다.";
            para_title_10_2_content.Format.SpaceAfter = 1;

            // table 10-1 #10
            Word.Table table_10_1;
            Word.Range rng_10_1 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_10_1 = word_doc.Tables.Add(rng_10_1, 7, 8, ref obj_miss, ref obj_miss);
            table_10_1.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_10_1.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_10_1.AllowAutoFit = false;

            table_10_1.Cell(1, 1).Width = 100;
            table_10_1.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 1).Range.Text = "";

            table_10_1.Cell(1, 2).Width = 50;
            table_10_1.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 2).Range.Font.Size = 9;
            table_10_1.Cell(1, 2).Range.Font.Bold = 1; 
            table_10_1.Cell(1, 2).Range.Text = "AppSuit";

            table_10_1.Cell(1, 3).Width = 50;
            table_10_1.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 3).Range.Font.Size = 9;
            table_10_1.Cell(1, 3).Range.Font.Bold = 1;
            table_10_1.Cell(1, 3).Range.Text = "LIAPP";

            table_10_1.Cell(1, 4).Width = 50;
            table_10_1.Cell(1, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 4).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 4).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 4).Range.Font.Size = 9;
            table_10_1.Cell(1, 4).Range.Font.Bold = 1;
            table_10_1.Cell(1, 4).Range.Text = "Applron";

            table_10_1.Cell(1, 5).Width = 50;
            table_10_1.Cell(1, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 5).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 5).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 5).Range.Font.Size = 9;
            table_10_1.Cell(1, 5).Range.Font.Bold = 1;
            table_10_1.Cell(1, 5).Range.Text = "FxShield";

            table_10_1.Cell(1, 6).Width = 50;
            table_10_1.Cell(1, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 6).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 6).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 6).Range.Font.Size = 9;
            table_10_1.Cell(1, 6).Range.Font.Bold = 1;
            table_10_1.Cell(1, 6).Range.Text = "EverSafe";

            table_10_1.Cell(1, 7).Width = 50;
            table_10_1.Cell(1, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 7).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 7).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 7).Range.Font.Size = 9;
            table_10_1.Cell(1, 7).Range.Font.Bold = 1;
            table_10_1.Cell(1, 7).Range.Text = "AppSolid";

            table_10_1.Cell(1, 8).Width = 50;
            table_10_1.Cell(1, 8).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(1, 8).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(1, 8).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(1, 8).Range.Font.Size = 9;
            table_10_1.Cell(1, 8).Range.Font.Bold = 1;
            table_10_1.Cell(1, 8).Range.Text = "Arxan";

            ////////////////////////////////////////////////////////////////////

            table_10_1.Cell(2, 1).Width = 100;
            table_10_1.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(2, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(2, 1).Range.Font.Size = 9;
            table_10_1.Cell(2, 1).Range.Font.Bold = 1;
            table_10_1.Cell(2, 1).Range.Text = "Rooting, VM Detection";

            table_10_1.Cell(3, 1).Width = 100;
            table_10_1.Cell(3, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(3, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(3, 1).Range.Font.Size = 9;
            table_10_1.Cell(3, 1).Range.Font.Bold = 1;
            table_10_1.Cell(3, 1).Range.Text = "Debugging Detection";

            table_10_1.Cell(4, 1).Width = 100;
            table_10_1.Cell(4, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(4, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(4, 1).Range.Font.Size = 9;
            table_10_1.Cell(4, 1).Range.Font.Bold = 1;
            table_10_1.Cell(4, 1).Range.Text = "Re-packaging Detection";

            table_10_1.Cell(5, 1).Width = 100;
            table_10_1.Cell(5, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(5, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(5, 1).Range.Font.Size = 9;
            table_10_1.Cell(5, 1).Range.Font.Bold = 1;
            table_10_1.Cell(5, 1).Range.Text = "Memory Protection";

            table_10_1.Cell(6, 1).Width = 100;
            table_10_1.Cell(6, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(6, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(6, 1).Range.Font.Size = 9;
            table_10_1.Cell(6, 1).Range.Font.Bold = 1;
            table_10_1.Cell(6, 1).Range.Text = "Native Binary Encryption";

            table_10_1.Cell(7, 1).Width = 100;
            table_10_1.Cell(7, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_1.Cell(7, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_1.Cell(7, 1).Range.Font.Size = 9;
            table_10_1.Cell(7, 1).Range.Font.Bold = 1;
            table_10_1.Cell(7, 1).Range.Text = "Unity Protection";

            ////////////////////////////////////////////////////////////////////

            table_10_1.Cell(2, 2).Width = 50;
            table_10_1.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 2).Range.Font.Size = 9;
            table_10_1.Cell(2, 2).Range.Font.Bold = 0;
            table_10_1.Cell(2, 2).Range.Text = "O";
            table_10_1.Cell(3, 2).Width = 50;
            table_10_1.Cell(3, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 2).Range.Font.Size = 9;
            table_10_1.Cell(3, 2).Range.Font.Bold = 0;
            table_10_1.Cell(3, 2).Range.Text = "O";
            table_10_1.Cell(4, 2).Width = 50;
            table_10_1.Cell(4, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 2).Range.Font.Size = 9;
            table_10_1.Cell(4, 2).Range.Font.Bold = 0;
            table_10_1.Cell(4, 2).Range.Text = "O";
            table_10_1.Cell(5, 2).Width = 50;
            table_10_1.Cell(5, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 2).Range.Font.Size = 9;
            table_10_1.Cell(5, 2).Range.Font.Bold = 0;
            table_10_1.Cell(5, 2).Range.Text = "O";
            table_10_1.Cell(6, 2).Width = 50;
            table_10_1.Cell(6, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 2).Range.Font.Size = 9;
            table_10_1.Cell(6, 2).Range.Font.Bold = 0;
            table_10_1.Cell(6, 2).Range.Text = "O";
            table_10_1.Cell(7, 2).Width = 50;
            table_10_1.Cell(7, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 2).Range.Font.Size = 9;
            table_10_1.Cell(7, 2).Range.Font.Bold = 0;
            table_10_1.Cell(7, 2).Range.Text = "";

            table_10_1.Cell(2, 3).Width = 50;
            table_10_1.Cell(2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 3).Range.Font.Size = 9;
            table_10_1.Cell(2, 3).Range.Font.Bold = 0;
            table_10_1.Cell(2, 3).Range.Text = "O";
            table_10_1.Cell(3, 3).Width = 50;
            table_10_1.Cell(3, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 3).Range.Font.Size = 9;
            table_10_1.Cell(3, 3).Range.Font.Bold = 0;
            table_10_1.Cell(3, 3).Range.Text = "O";
            table_10_1.Cell(4, 3).Width = 50;
            table_10_1.Cell(4, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 3).Range.Font.Size = 9;
            table_10_1.Cell(4, 3).Range.Font.Bold = 0;
            table_10_1.Cell(4, 3).Range.Text = "O";
            table_10_1.Cell(5, 3).Width = 50;
            table_10_1.Cell(5, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 3).Range.Font.Size = 9;
            table_10_1.Cell(5, 3).Range.Font.Bold = 0;
            table_10_1.Cell(5, 3).Range.Text = "O";
            table_10_1.Cell(6, 3).Width = 50;
            table_10_1.Cell(6, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 3).Range.Font.Size = 9;
            table_10_1.Cell(6, 3).Range.Font.Bold = 0;
            table_10_1.Cell(6, 3).Range.Text = "";
            table_10_1.Cell(7, 3).Width = 50;
            table_10_1.Cell(7, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 3).Range.Font.Size = 9;
            table_10_1.Cell(7, 3).Range.Font.Bold = 0;
            table_10_1.Cell(7, 3).Range.Text = "O";

            table_10_1.Cell(2, 4).Width = 50;
            table_10_1.Cell(2, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 4).Range.Font.Size = 9;
            table_10_1.Cell(2, 4).Range.Font.Bold = 0;
            table_10_1.Cell(2, 4).Range.Text = "O";
            table_10_1.Cell(3, 4).Width = 50;
            table_10_1.Cell(3, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 4).Range.Font.Size = 9;
            table_10_1.Cell(3, 4).Range.Font.Bold = 0;
            table_10_1.Cell(3, 4).Range.Text = "O";
            table_10_1.Cell(4, 4).Width = 50;
            table_10_1.Cell(4, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 4).Range.Font.Size = 9;
            table_10_1.Cell(4, 4).Range.Font.Bold = 0;
            table_10_1.Cell(4, 4).Range.Text = "O";
            table_10_1.Cell(5, 4).Width = 50;
            table_10_1.Cell(5, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 4).Range.Font.Size = 9;
            table_10_1.Cell(5, 4).Range.Font.Bold = 0;
            table_10_1.Cell(5, 4).Range.Text = "";
            table_10_1.Cell(6, 4).Width = 50;
            table_10_1.Cell(6, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 4).Range.Font.Size = 9;
            table_10_1.Cell(6, 4).Range.Font.Bold = 0;
            table_10_1.Cell(6, 4).Range.Text = "O";
            table_10_1.Cell(7, 4).Width = 50;
            table_10_1.Cell(7, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 4).Range.Font.Size = 9;
            table_10_1.Cell(7, 4).Range.Font.Bold = 0;
            table_10_1.Cell(7, 4).Range.Text = "";

            table_10_1.Cell(2, 5).Width = 50;
            table_10_1.Cell(2, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 5).Range.Font.Size = 9;
            table_10_1.Cell(2, 5).Range.Font.Bold = 0;
            table_10_1.Cell(2, 5).Range.Text = "O";
            table_10_1.Cell(3, 5).Width = 50;
            table_10_1.Cell(3, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 5).Range.Font.Size = 9;
            table_10_1.Cell(3, 5).Range.Font.Bold = 0;
            table_10_1.Cell(3, 5).Range.Text = "O";
            table_10_1.Cell(4, 5).Width = 50;
            table_10_1.Cell(4, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 5).Range.Font.Size = 9;
            table_10_1.Cell(4, 5).Range.Font.Bold = 0;
            table_10_1.Cell(4, 5).Range.Text = "O";
            table_10_1.Cell(5, 5).Width = 50;
            table_10_1.Cell(5, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 5).Range.Font.Size = 9;
            table_10_1.Cell(5, 5).Range.Font.Bold = 0;
            table_10_1.Cell(5, 5).Range.Text = "O";
            table_10_1.Cell(6, 5).Width = 50;
            table_10_1.Cell(6, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 5).Range.Font.Size = 9;
            table_10_1.Cell(6, 5).Range.Font.Bold = 0;
            table_10_1.Cell(6, 5).Range.Text = "";
            table_10_1.Cell(7, 5).Width = 50;
            table_10_1.Cell(7, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 5).Range.Font.Size = 9;
            table_10_1.Cell(7, 5).Range.Font.Bold = 0;
            table_10_1.Cell(7, 5).Range.Text = "";

            table_10_1.Cell(2, 6).Width = 50;
            table_10_1.Cell(2, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 6).Range.Font.Size = 9;
            table_10_1.Cell(2, 6).Range.Font.Bold = 0;
            table_10_1.Cell(2, 6).Range.Text = "O";
            table_10_1.Cell(3, 6).Width = 50;
            table_10_1.Cell(3, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 6).Range.Font.Size = 9;
            table_10_1.Cell(3, 6).Range.Font.Bold = 0;
            table_10_1.Cell(3, 6).Range.Text = "";
            table_10_1.Cell(4, 6).Width = 50;
            table_10_1.Cell(4, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 6).Range.Font.Size = 9;
            table_10_1.Cell(4, 6).Range.Font.Bold = 0;
            table_10_1.Cell(4, 6).Range.Text = "";
            table_10_1.Cell(5, 6).Width = 50;
            table_10_1.Cell(5, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 6).Range.Font.Size = 9;
            table_10_1.Cell(5, 6).Range.Font.Bold = 0;
            table_10_1.Cell(5, 6).Range.Text = "";
            table_10_1.Cell(6, 6).Width = 50;
            table_10_1.Cell(6, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 6).Range.Font.Size = 9;
            table_10_1.Cell(6, 6).Range.Font.Bold = 0;
            table_10_1.Cell(6, 6).Range.Text = "O";
            table_10_1.Cell(7, 6).Width = 50;
            table_10_1.Cell(7, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 6).Range.Font.Size = 9;
            table_10_1.Cell(7, 6).Range.Font.Bold = 0;
            table_10_1.Cell(7, 6).Range.Text = "";

            table_10_1.Cell(2, 7).Width = 50;
            table_10_1.Cell(2, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 7).Range.Font.Size = 9;
            table_10_1.Cell(2, 7).Range.Font.Bold = 0;
            table_10_1.Cell(2, 7).Range.Text = "O";
            table_10_1.Cell(3, 7).Width = 50;
            table_10_1.Cell(3, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 7).Range.Font.Size = 9;
            table_10_1.Cell(3, 7).Range.Font.Bold = 0;
            table_10_1.Cell(3, 7).Range.Text = "O";
            table_10_1.Cell(4, 7).Width = 50;
            table_10_1.Cell(4, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 7).Range.Font.Size = 9;
            table_10_1.Cell(4, 7).Range.Font.Bold = 0;
            table_10_1.Cell(4, 7).Range.Text = "O";
            table_10_1.Cell(5, 7).Width = 50;
            table_10_1.Cell(5, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 7).Range.Font.Size = 9;
            table_10_1.Cell(5, 7).Range.Font.Bold = 0;
            table_10_1.Cell(5, 7).Range.Text = "O";
            table_10_1.Cell(6, 7).Width = 50;
            table_10_1.Cell(6, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 7).Range.Font.Size = 9;
            table_10_1.Cell(6, 7).Range.Font.Bold = 0;
            table_10_1.Cell(6, 7).Range.Text = "O";
            table_10_1.Cell(7, 7).Width = 50;
            table_10_1.Cell(7, 7).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 7).Range.Font.Size = 9;
            table_10_1.Cell(7, 7).Range.Font.Bold = 0;
            table_10_1.Cell(7, 7).Range.Text = "O";

            table_10_1.Cell(2, 8).Width = 50;
            table_10_1.Cell(2, 8).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(2, 8).Range.Font.Size = 9;
            table_10_1.Cell(2, 8).Range.Font.Bold = 0;
            table_10_1.Cell(2, 8).Range.Text = "O";
            table_10_1.Cell(3, 8).Width = 50;
            table_10_1.Cell(3, 8).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(3, 8).Range.Font.Size = 9;
            table_10_1.Cell(3, 8).Range.Font.Bold = 0;
            table_10_1.Cell(3, 8).Range.Text = "O";
            table_10_1.Cell(4, 8).Width = 50;
            table_10_1.Cell(4, 8).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(4, 8).Range.Font.Size = 9;
            table_10_1.Cell(4, 8).Range.Font.Bold = 0;
            table_10_1.Cell(4, 8).Range.Text = "O";
            table_10_1.Cell(5, 8).Width = 50;
            table_10_1.Cell(5, 8).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(5, 8).Range.Font.Size = 9;
            table_10_1.Cell(5, 8).Range.Font.Bold = 0;
            table_10_1.Cell(5, 8).Range.Text = "";
            table_10_1.Cell(6, 8).Width = 50;
            table_10_1.Cell(6, 8).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(6, 8).Range.Font.Size = 9;
            table_10_1.Cell(6, 8).Range.Font.Bold = 0;
            table_10_1.Cell(6, 8).Range.Text = "O";
            table_10_1.Cell(7, 8).Width = 50;
            table_10_1.Cell(7, 8).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_1.Cell(7, 8).Range.Font.Size = 9;
            table_10_1.Cell(7, 8).Range.Font.Bold = 0;
            table_10_1.Cell(7, 8).Range.Text = "";

            rng_10_1.InsertParagraphAfter();

            // space
            Word.Paragraph table_10_1_content = word_doc.Paragraphs.Add(ref obj_miss);
            table_10_1_content.Range.Font.Bold = 0;
            table_10_1_content.Range.Font.Size = 10;
            table_10_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            table_10_1_content.Range.Text = "";
            table_10_1_content.Format.SpaceAfter = 10;
            table_10_1_content.Range.InsertParagraphAfter();

            // title3 #10
            Word.Paragraph para_title_10_3 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_3.Range.Text = "2. 안티 치트";
            para_title_10_3.Range.Font.Bold = 1;
            para_title_10_3.Range.Font.Size = 14;
            para_title_10_3.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_3.Format.SpaceAfter = 20;
            para_title_10_3.Range.InsertParagraphAfter();

            // title3 content #10
            Word.Paragraph para_title_10_3_content = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_3_content.Range.Font.Bold = 0;
            para_title_10_3_content.Range.Font.Size = 10;
            para_title_10_3_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_3_content.Range.Text = " 게임 해킹을 방지하기 위한 솔루션인 안티 치트를 기능별로 정리한 표이다. 기능은 각 제품 공식 홈페이지에 있는 내용을 참고하였으며, Dragonfist 도구에서 탐지하는 취약점에 대해 해결책을 줄 수 있는 기능 위주로 정리하였다.";
            para_title_10_3_content.Format.SpaceAfter = 1;

            // table 10-2 #10
            Word.Table table_10_2;
            Word.Range rng_10_2 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_10_2 = word_doc.Tables.Add(rng_10_2, 8, 6, ref obj_miss, ref obj_miss);
            table_10_2.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_10_2.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_10_2.AllowAutoFit = false;

            table_10_2.Cell(1, 1).Width = 150;
            table_10_2.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(1, 1).Range.Text = "";

            table_10_2.Cell(1, 2).Width = 60;
            table_10_2.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(1, 2).Range.Font.Size = 9;
            table_10_2.Cell(1, 2).Range.Font.Bold = 1;
            table_10_2.Cell(1, 2).Range.Text = "MTP";

            table_10_2.Cell(1, 3).Width = 60;
            table_10_2.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(1, 3).Range.Font.Size = 9;
            table_10_2.Cell(1, 3).Range.Font.Bold = 1;
            table_10_2.Cell(1, 3).Range.Text = "XIGNCODE";

            table_10_2.Cell(1, 4).Width = 60;
            table_10_2.Cell(1, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(1, 4).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(1, 4).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(1, 4).Range.Font.Size = 9;
            table_10_2.Cell(1, 4).Range.Font.Bold = 1;
            table_10_2.Cell(1, 4).Range.Text = "AppGuard";

            table_10_2.Cell(1, 5).Width = 60;
            table_10_2.Cell(1, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(1, 5).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(1, 5).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(1, 5).Range.Font.Size = 9;
            table_10_2.Cell(1, 5).Range.Font.Bold = 1;
            table_10_2.Cell(1, 5).Range.Text = "AIR";

            table_10_2.Cell(1, 6).Width = 60;
            table_10_2.Cell(1, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(1, 6).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(1, 6).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(1, 6).Range.Font.Size = 9;
            table_10_2.Cell(1, 6).Range.Font.Bold = 1;
            table_10_2.Cell(1, 6).Range.Text = "Gxshield";

            ////////////////////////////////////////////////////////////////////

            table_10_2.Cell(2, 1).Width = 150;
            table_10_2.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(2, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(2, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(2, 1).Range.Font.Size = 9;
            table_10_2.Cell(2, 1).Range.Font.Bold = 1;
            table_10_2.Cell(2, 1).Range.Text = "Native Library Encryption";

            table_10_2.Cell(3, 1).Width = 150;
            table_10_2.Cell(3, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(3, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(3, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(3, 1).Range.Font.Size = 9;
            table_10_2.Cell(3, 1).Range.Font.Bold = 1;
            table_10_2.Cell(3, 1).Range.Text = "Integrity Validation";

            table_10_2.Cell(4, 1).Width = 150;
            table_10_2.Cell(4, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(4, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(4, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(4, 1).Range.Font.Size = 9;
            table_10_2.Cell(4, 1).Range.Font.Bold = 1;
            table_10_2.Cell(4, 1).Range.Text = "Memory Protection";

            table_10_2.Cell(5, 1).Width = 150;
            table_10_2.Cell(5, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(5, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(5, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(5, 1).Range.Font.Size = 9;
            table_10_2.Cell(5, 1).Range.Font.Bold = 1;
            table_10_2.Cell(5, 1).Range.Text = "Speed Hack Detection";

            table_10_2.Cell(6, 1).Width = 150;
            table_10_2.Cell(6, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(6, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(6, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(6, 1).Range.Font.Size = 9;
            table_10_2.Cell(6, 1).Range.Font.Bold = 1;
            table_10_2.Cell(6, 1).Range.Text = "Debugging Detection";

            table_10_2.Cell(7, 1).Width = 150;
            table_10_2.Cell(7, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(7, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(7, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(7, 1).Range.Font.Size = 9;
            table_10_2.Cell(7, 1).Range.Font.Bold = 1;
            table_10_2.Cell(7, 1).Range.Text = "Haking Tool Detection";

            table_10_2.Cell(8, 1).Width = 150;
            table_10_2.Cell(8, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(8, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_2.Cell(8, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_2.Cell(8, 1).Range.Font.Size = 9;
            table_10_2.Cell(8, 1).Range.Font.Bold = 1;
            table_10_2.Cell(8, 1).Range.Text = "Rooting Detection";

            ////////////////////////////////////////////////////////////////////

            table_10_2.Cell(2, 2).Width = 60;
            table_10_2.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(2, 2).Range.Font.Size = 9;
            table_10_2.Cell(2, 2).Range.Font.Bold = 0;
            table_10_2.Cell(2, 2).Range.Text = "O";
            table_10_2.Cell(3, 2).Width = 60;
            table_10_2.Cell(3, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(3, 2).Range.Font.Size = 9;
            table_10_2.Cell(3, 2).Range.Font.Bold = 0;
            table_10_2.Cell(3, 2).Range.Text = "O";
            table_10_2.Cell(4, 2).Width = 60;
            table_10_2.Cell(4, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(4, 2).Range.Font.Size = 9;
            table_10_2.Cell(4, 2).Range.Font.Bold = 0;
            table_10_2.Cell(4, 2).Range.Text = "O";
            table_10_2.Cell(5, 2).Width = 60;
            table_10_2.Cell(5, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(5, 2).Range.Font.Size = 9;
            table_10_2.Cell(5, 2).Range.Font.Bold = 0;
            table_10_2.Cell(5, 2).Range.Text = "O";
            table_10_2.Cell(6, 2).Width = 60;
            table_10_2.Cell(6, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(6, 2).Range.Font.Size = 9;
            table_10_2.Cell(6, 2).Range.Font.Bold = 0;
            table_10_2.Cell(6, 2).Range.Text = "O";
            table_10_2.Cell(7, 2).Width = 60;
            table_10_2.Cell(7, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(7, 2).Range.Font.Size = 9;
            table_10_2.Cell(7, 2).Range.Font.Bold = 0;
            table_10_2.Cell(7, 2).Range.Text = "O";
            table_10_2.Cell(8, 2).Width = 60;
            table_10_2.Cell(8, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(8, 2).Range.Font.Size = 9;
            table_10_2.Cell(8, 2).Range.Font.Bold = 0;
            table_10_2.Cell(8, 2).Range.Text = "";

            table_10_2.Cell(2, 3).Width = 60;
            table_10_2.Cell(2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(2, 3).Range.Font.Size = 9;
            table_10_2.Cell(2, 3).Range.Font.Bold = 0;
            table_10_2.Cell(2, 3).Range.Text = "";
            table_10_2.Cell(3, 3).Width = 60;
            table_10_2.Cell(3, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(3, 3).Range.Font.Size = 9;
            table_10_2.Cell(3, 3).Range.Font.Bold = 0;
            table_10_2.Cell(3, 3).Range.Text = "O";
            table_10_2.Cell(4, 3).Width = 60;
            table_10_2.Cell(4, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(4, 3).Range.Font.Size = 9;
            table_10_2.Cell(4, 3).Range.Font.Bold = 0;
            table_10_2.Cell(4, 3).Range.Text = "";
            table_10_2.Cell(5, 3).Width = 60;
            table_10_2.Cell(5, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(5, 3).Range.Font.Size = 9;
            table_10_2.Cell(5, 3).Range.Font.Bold = 0;
            table_10_2.Cell(5, 3).Range.Text = "";
            table_10_2.Cell(6, 3).Width = 60;
            table_10_2.Cell(6, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(6, 3).Range.Font.Size = 9;
            table_10_2.Cell(6, 3).Range.Font.Bold = 0;
            table_10_2.Cell(6, 3).Range.Text = "O";
            table_10_2.Cell(7, 3).Width = 60;
            table_10_2.Cell(7, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(7, 3).Range.Font.Size = 9;
            table_10_2.Cell(7, 3).Range.Font.Bold = 0;
            table_10_2.Cell(7, 3).Range.Text = "O";
            table_10_2.Cell(8, 3).Width = 60;
            table_10_2.Cell(8, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(8, 3).Range.Font.Size = 9;
            table_10_2.Cell(8, 3).Range.Font.Bold = 0;
            table_10_2.Cell(8, 3).Range.Text = "O";

            table_10_2.Cell(2, 4).Width = 60;
            table_10_2.Cell(2, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(2, 4).Range.Font.Size = 9;
            table_10_2.Cell(2, 4).Range.Font.Bold = 0;
            table_10_2.Cell(2, 4).Range.Text = "O";
            table_10_2.Cell(3, 4).Width = 60;
            table_10_2.Cell(3, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(3, 4).Range.Font.Size = 9;
            table_10_2.Cell(3, 4).Range.Font.Bold = 0;
            table_10_2.Cell(3, 4).Range.Text = "O";
            table_10_2.Cell(4, 4).Width = 60;
            table_10_2.Cell(4, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(4, 4).Range.Font.Size = 9;
            table_10_2.Cell(4, 4).Range.Font.Bold = 0;
            table_10_2.Cell(4, 4).Range.Text = "O";
            table_10_2.Cell(5, 4).Width = 60;
            table_10_2.Cell(5, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(5, 4).Range.Font.Size = 9;
            table_10_2.Cell(5, 4).Range.Font.Bold = 0;
            table_10_2.Cell(5, 4).Range.Text = "O";
            table_10_2.Cell(6, 4).Width = 60;
            table_10_2.Cell(6, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(6, 4).Range.Font.Size = 9;
            table_10_2.Cell(6, 4).Range.Font.Bold = 0;
            table_10_2.Cell(6, 4).Range.Text = "O";
            table_10_2.Cell(7, 4).Width = 60;
            table_10_2.Cell(7, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(7, 4).Range.Font.Size = 9;
            table_10_2.Cell(7, 4).Range.Font.Bold = 0;
            table_10_2.Cell(7, 4).Range.Text = "O";
            table_10_2.Cell(8, 4).Width = 60;
            table_10_2.Cell(8, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(8, 4).Range.Font.Size = 9;
            table_10_2.Cell(8, 4).Range.Font.Bold = 0;
            table_10_2.Cell(8, 4).Range.Text = "O";

            table_10_2.Cell(2, 5).Width = 60;
            table_10_2.Cell(2, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(2, 5).Range.Font.Size = 9;
            table_10_2.Cell(2, 5).Range.Font.Bold = 0;
            table_10_2.Cell(2, 5).Range.Text = "O";
            table_10_2.Cell(3, 5).Width = 60;
            table_10_2.Cell(3, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(3, 5).Range.Font.Size = 9;
            table_10_2.Cell(3, 5).Range.Font.Bold = 0;
            table_10_2.Cell(3, 5).Range.Text = "O";
            table_10_2.Cell(4, 5).Width = 60;
            table_10_2.Cell(4, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(4, 5).Range.Font.Size = 9;
            table_10_2.Cell(4, 5).Range.Font.Bold = 0;
            table_10_2.Cell(4, 5).Range.Text = "O";
            table_10_2.Cell(5, 5).Width = 60;
            table_10_2.Cell(5, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(5, 5).Range.Font.Size = 9;
            table_10_2.Cell(5, 5).Range.Font.Bold = 0;
            table_10_2.Cell(5, 5).Range.Text = "";
            table_10_2.Cell(6, 5).Width = 60;
            table_10_2.Cell(6, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(6, 5).Range.Font.Size = 9;
            table_10_2.Cell(6, 5).Range.Font.Bold = 0;
            table_10_2.Cell(6, 5).Range.Text = "O";
            table_10_2.Cell(7, 5).Width = 60;
            table_10_2.Cell(7, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(7, 5).Range.Font.Size = 9;
            table_10_2.Cell(7, 5).Range.Font.Bold = 0;
            table_10_2.Cell(7, 5).Range.Text = "O";
            table_10_2.Cell(8, 5).Width = 60;
            table_10_2.Cell(8, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(8, 5).Range.Font.Size = 9;
            table_10_2.Cell(8, 5).Range.Font.Bold = 0;
            table_10_2.Cell(8, 5).Range.Text = "O";

            table_10_2.Cell(2, 6).Width = 60;
            table_10_2.Cell(2, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(2, 6).Range.Font.Size = 9;
            table_10_2.Cell(2, 6).Range.Font.Bold = 0;
            table_10_2.Cell(2, 6).Range.Text = "O";
            table_10_2.Cell(3, 6).Width = 60;
            table_10_2.Cell(3, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(3, 6).Range.Font.Size = 9;
            table_10_2.Cell(3, 6).Range.Font.Bold = 0;
            table_10_2.Cell(3, 6).Range.Text = "O";
            table_10_2.Cell(4, 6).Width = 60;
            table_10_2.Cell(4, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(4, 6).Range.Font.Size = 9;
            table_10_2.Cell(4, 6).Range.Font.Bold = 0;
            table_10_2.Cell(4, 6).Range.Text = "O";
            table_10_2.Cell(5, 6).Width = 60;
            table_10_2.Cell(5, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(5, 6).Range.Font.Size = 9;
            table_10_2.Cell(5, 6).Range.Font.Bold = 0;
            table_10_2.Cell(5, 6).Range.Text = "O";
            table_10_2.Cell(6, 6).Width = 60;
            table_10_2.Cell(6, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(6, 6).Range.Font.Size = 9;
            table_10_2.Cell(6, 6).Range.Font.Bold = 0;
            table_10_2.Cell(6, 6).Range.Text = "O";
            table_10_2.Cell(7, 6).Width = 60;
            table_10_2.Cell(7, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(7, 6).Range.Font.Size = 9;
            table_10_2.Cell(7, 6).Range.Font.Bold = 0;
            table_10_2.Cell(7, 6).Range.Text = "O";
            table_10_2.Cell(8, 6).Width = 60;
            table_10_2.Cell(8, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_2.Cell(8, 6).Range.Font.Size = 9;
            table_10_2.Cell(8, 6).Range.Font.Bold = 0;
            table_10_2.Cell(8, 6).Range.Text = "O";

            rng_10_2.InsertParagraphAfter();

            // space
            Word.Paragraph para_title_10_4_1_content = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_4_1_content.Range.Font.Bold = 0;
            para_title_10_4_1_content.Range.Font.Size = 10;
            para_title_10_4_1_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_4_1_content.Range.Text = "";
            para_title_10_4_1_content.Format.SpaceAfter = 10;
            para_title_10_4_1_content.Range.InsertParagraphAfter();

            // title4 #10
            Word.Paragraph para_title_10_4 = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_4.Range.Text = "3. 암호 알고리즘 관련";
            para_title_10_4.Range.Font.Bold = 1;
            para_title_10_4.Range.Font.Size = 14;
            para_title_10_4.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_4.Format.SpaceAfter = 20;
            para_title_10_4.Range.InsertParagraphAfter();

            // title3 content #10
            Word.Paragraph para_title_10_4_content = word_doc.Paragraphs.Add(ref obj_miss);
            para_title_10_4_content.Range.Font.Bold = 0;
            para_title_10_4_content.Range.Font.Size = 10;
            para_title_10_4_content.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            para_title_10_4_content.Range.Text = " 아래 표는 한국인터넷진흥원(KISA)에서 발행한 “암호 알고리즘 및 키 길이 이용 안내서”에서 일부를 발췌한 것이다. 암호 알고리즘을 적용하는 경우, 알고리즘의 종류나 키 길이 등은 해당 게임의 환경과 성능을 고려하여 안정성 수준을 만족할 수 있도록 선택해야한다. 암호화에 너무 많은 비용을 쏟기 보다는 암호화가 필수적으로 필요한 부분만 암호화하며, seed를 정기적으로 교체하는 방식을 권고한다.";
            para_title_10_4_content.Format.SpaceAfter = 1;

            // table 10-3 #10
            Word.Table table_10_3;
            Word.Range rng_10_3 = word_doc.Bookmarks.get_Item(ref end_of_doc).Range;
            table_10_3 = word_doc.Tables.Add(rng_10_3, 6, 5, ref obj_miss, ref obj_miss);
            table_10_3.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_10_3.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table_10_3.Range.Font.Size = 9;
            table_10_3.AllowAutoFit = false;

            table_10_3.Cell(1, 1).Width = 130;
            table_10_3.Cell(1, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(1, 1).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_3.Cell(1, 1).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_3.Cell(1, 1).Range.Text = "분류";
            table_10_3.Cell(1, 2).Width = 80;
            table_10_3.Cell(1, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(1, 2).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_3.Cell(1, 2).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_3.Cell(1, 2).Range.Font.Bold = 1;
            table_10_3.Cell(1, 2).Range.Text = "NIST(미국)";
            table_10_3.Cell(1, 3).Width = 80;
            table_10_3.Cell(1, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(1, 3).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_3.Cell(1, 3).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_3.Cell(1, 3).Range.Font.Bold = 1;
            table_10_3.Cell(1, 3).Range.Text = "CRYPTERC(일본)";
            table_10_3.Cell(1, 4).Width = 80;
            table_10_3.Cell(1, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(1, 4).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_3.Cell(1, 4).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_3.Cell(1, 4).Range.Font.Bold = 1;
            table_10_3.Cell(1, 4).Range.Text = "ECRYPT(유럽)";
            table_10_3.Cell(1, 5).Width = 80;
            table_10_3.Cell(1, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(1, 5).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorBlueGray;
            table_10_3.Cell(1, 5).Range.Font.Color = Word.WdColor.wdColorWhite;
            table_10_3.Cell(1, 5).Range.Font.Bold = 1;
            table_10_3.Cell(1, 5).Range.Text = "국내";

            table_10_3.Cell(2, 1).Width = 130;
            table_10_3.Cell(2, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(2, 1).Range.Font.Bold = 0;
            table_10_3.Cell(2, 1).Range.Text = "대칭키 암호 알고리즘";
            table_10_3.Cell(2, 2).Width = 80;
            table_10_3.Cell(2, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(2, 2).Range.Font.Bold = 0;
            table_10_3.Cell(2, 2).Range.Text = "AES";
            table_10_3.Cell(2, 3).Width = 80;
            table_10_3.Cell(2, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(2, 3).Range.Font.Bold = 0;
            table_10_3.Cell(2, 3).Range.Text = "AES\n3TDEA";
            table_10_3.Cell(2, 4).Width = 80;
            table_10_3.Cell(2, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(2, 4).Range.Font.Bold = 0;
            table_10_3.Cell(2, 4).Range.Text = "AES\n2TDA\n3TDA\nKASUMI";
            table_10_3.Cell(2, 5).Width = 80;
            table_10_3.Cell(2, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(2, 5).Range.Font.Bold = 0;
            table_10_3.Cell(2, 5).Range.Text = "SEED\nARIA\nHIGHT";

            table_10_3.Cell(3, 1).Width = 130;
            table_10_3.Cell(3, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(3, 1).Range.Font.Bold = 0;
            table_10_3.Cell(3, 1).Range.Text = "해시함수";
            table_10_3.Cell(3, 2).Width = 80;
            table_10_3.Cell(3, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(3, 2).Range.Font.Bold = 0;
            table_10_3.Cell(3, 2).Range.Text = "SHA-256\nSHA - 512";
            table_10_3.Cell(3, 3).Width = 80;
            table_10_3.Cell(3, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(3, 3).Range.Font.Bold = 0;
            table_10_3.Cell(3, 3).Range.Text = "SHA-256\nSHA - 512";
            table_10_3.Cell(3, 4).Width = 80;
            table_10_3.Cell(3, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(3, 4).Range.Font.Bold = 0;
            table_10_3.Cell(3, 4).Range.Text = "SHA-256\nSHA - 512";
            table_10_3.Cell(3, 5).Width = 80;
            table_10_3.Cell(3, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(3, 5).Range.Font.Bold = 0;
            table_10_3.Cell(3, 5).Range.Text = "SHA-256\nSHA - 512";

            table_10_3.Cell(4, 1).Split(1, 2);
            table_10_3.Cell(5, 1).Split(1, 2);
            table_10_3.Cell(6, 1).Split(1, 2);
            table_10_3.Cell(4, 1).Merge(table_10_3.Cell(5, 1)); table_10_3.Cell(4, 1).Merge(table_10_3.Cell(6, 1));

            table_10_3.Cell(4, 1).Width = 65;
            table_10_3.Cell(4, 1).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(4, 1).Range.Font.Bold = 0;
            table_10_3.Cell(4, 1).Range.Text = "공개키 암호 알고리즘";
            table_10_3.Cell(4, 2).Width = 65;
            table_10_3.Cell(4, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(4, 2).Range.Font.Bold = 0;
            table_10_3.Cell(4, 2).Range.Text = "키\n공유용";
            table_10_3.Cell(5, 2).Width = 65;
            table_10_3.Cell(5, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(5, 2).Range.Font.Bold = 0;
            table_10_3.Cell(5, 2).Range.Text = "암/복호화용";
            table_10_3.Cell(6, 2).Width = 65;
            table_10_3.Cell(6, 2).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(6, 2).Range.Font.Bold = 0;
            table_10_3.Cell(6, 2).Range.Text = "전자 서명용";

            table_10_3.Cell(4, 3).Width = 80;
            table_10_3.Cell(4, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(4, 3).Range.Font.Bold = 0;
            table_10_3.Cell(4, 3).Range.Text = "DH\nECDH\nMQV\nECMQV";
            table_10_3.Cell(4, 4).Width = 80;
            table_10_3.Cell(4, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(4, 4).Range.Font.Bold = 0;
            table_10_3.Cell(4, 4).Range.Text = "DH\nECDH\nPSEC - KEM";
            table_10_3.Cell(4, 5).Width = 80;
            table_10_3.Cell(4, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(4, 5).Range.Font.Bold = 0;
            table_10_3.Cell(4, 5).Range.Text = "ACS-KEM\nRSA - KEM";
            table_10_3.Cell(4, 6).Width = 80;
            table_10_3.Cell(4, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(4, 6).Range.Font.Bold = 0;
            table_10_3.Cell(4, 6).Range.Text = "DH\nECDH";

            table_10_3.Cell(5, 3).Width = 80;
            table_10_3.Cell(5, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(5, 3).Range.Font.Bold = 0;
            table_10_3.Cell(5, 3).Range.Text = "RSA";
            table_10_3.Cell(5, 4).Width = 80;
            table_10_3.Cell(5, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(5, 4).Range.Font.Bold = 0;
            table_10_3.Cell(5, 4).Range.Text = "RSAES-OAEP";
            table_10_3.Cell(5, 5).Width = 80;
            table_10_3.Cell(5, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(5, 5).Range.Font.Bold = 0;
            table_10_3.Cell(5, 5).Range.Text = "RSAES-OAEP";
            table_10_3.Cell(5, 6).Width = 80;
            table_10_3.Cell(5, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(5, 6).Range.Font.Bold = 0;
            table_10_3.Cell(5, 6).Range.Text = "RSAES-OAEP";

            table_10_3.Cell(6, 3).Width = 80;
            table_10_3.Cell(6, 3).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(6, 3).Range.Font.Bold = 0;
            table_10_3.Cell(6, 3).Range.Text = "RSA\nDSA\nECDSA";
            table_10_3.Cell(6, 4).Width = 80;
            table_10_3.Cell(6, 4).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(6, 4).Range.Font.Bold = 0;
            table_10_3.Cell(6, 4).Range.Text = "RSASSA-PSS\nDSA\nECDSA";
            table_10_3.Cell(6, 5).Width = 80;
            table_10_3.Cell(6, 5).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(6, 5).Range.Font.Bold = 0;
            table_10_3.Cell(6, 5).Range.Text = "RSASSA-PSS\nDSA\nECDSA";
            table_10_3.Cell(6, 6).Width = 80;
            table_10_3.Cell(6, 6).Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            table_10_3.Cell(6, 6).Range.Font.Bold = 0;
            table_10_3.Cell(6, 6).Range.Text = "KCDSA\nECDSA\nEC - KCDSA";

            rng_10_3.InsertParagraphAfter();

            // Save
            word_doc.SaveAs2(@original_path_name + "\\Report_" + apk_name + "_" + select_platform + ".docx");
            word_doc.Close();
            word.Quit();
            listView1.Items.Add("Success to make a report");
        }

        private List<StringBuilder> search_dumpcs()
        {
            int a = 0, cnt = 0;
            List<StringBuilder> result_sb = new List<StringBuilder>();
            StringBuilder SB = new StringBuilder();
            try
            {
                if (meta_f_list.Count == 0) { return null; }
                int m_cnt;
                for(int i = 0; i < meta_f_list.Count; i++)
                {
                    for(int j = 0; j < meta_f_list.Count; j++)
                    {
                        m_cnt = 0;
                        if (i != j)
                        {
                            if(meta_f_list[i][0] == meta_f_list[j][0]) { m_cnt++; }
                        }
                        if(m_cnt > 0) { meta_f_list.RemoveAt(j); }
                    }
                }
                String[] lines = File.ReadAllLines(@dump_path + "dump.cs");
                for (int i = 0; i < meta_f_list.Count; i++)
                {
                    cnt = 0;
                    foreach (String line in lines)
                    {
                        if (a == 1 && cnt > 20)
                        {
                            SB.Append("......");
                            a = 0;
                            result_sb.Add(SB);
                            SB = new StringBuilder();
                            break;
                        }
                        if (a == 1 && line.Contains("}") && line.Length == 1)
                        {
                            SB.Append(line);
                            a = 0;
                            result_sb.Add(SB);
                            SB = new StringBuilder();
                            break;
                        }
                        else if (a == 1)
                        {
                            SB.Append(line + "\n");
                            cnt++;
                        }
                        if (line.Contains(meta_f_list[i][0])) // find class
                        {
                            SB.Append(line + "\n");
                            a = 1;
                            cnt++;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(this, e.ToString(), "Error");
            }
            return result_sb;
        }


        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@ Buttons @@@@

        private void button1_Click_1(object sender, EventArgs e) // file save button
        {
            if (is_meta == 0 && is_dic == 0 && is_db == 0 && is_memory == 0 && is_random == 0 && is_speed == 0)
            {
                MessageBox.Show(this, "Please select at least one item", "Error");
            }
            else
            {
                try
                {
                    if (File.Exists(@original_path_name + "\\Report_" + apk_name + ".docx"))
                    {
                        listView1.Items.Add("The report is already made");
                        if (MessageBox.Show(this, "The report is already made\n\nDo you want to make again?", "Check", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Make_Word_Report();
                        }
                        else { return; }
                    }
                    else
                    {
                        Make_Word_Report();
                        is_make = 1;
                    }
                }
                catch (Exception e1)
                {
                    listView1.Items.Add("Fail to make a report");
                    MessageBox.Show(this, "[Error Code = 0x70]\n\nFail to make a report\n\n" + e1.ToString(), "Error");
                }
            }
        }

        private void button4_Click_1(object sender, EventArgs e) // back button
        {
            /*
            StringBuilder ssb = new StringBuilder();
            ssb.Append("is_meta: " + is_meta.ToString() + "\n");
            ssb.Append("is_speed: " + is_speed.ToString() + "\n");
            ssb.Append("is_random: " + is_random.ToString() + "\n");
            ssb.Append("is_dic: " + is_dic.ToString() + "\n");
            ssb.Append("is_memory: " + is_memory.ToString() + "\n");
            ssb.Append("is_db: " + is_db.ToString());
            MessageBox.Show(this, ssb.ToString(), "Info");
            */

            if (is_meta == 1) { if (!items.Contains("Meta data")) { items.Add("Meta data"); } }
            else if(is_meta == 0) { if(items.Contains("Meta data")) { items.Remove("Meta data"); } }

            if (is_speed == 1) { if (!items.Contains("Time")) { items.Add("Time"); } }
            else if (is_speed == 0) { if (items.Contains("Time")) { items.Remove("Time"); } }

            if (is_random == 1) { if (!items.Contains("Random")) { items.Add("Random"); } }
            else if (is_random == 0) { if (items.Contains("Random")) { items.Remove("Random"); } }

            if (is_dic == 1) { if (!items.Contains("Method check")) { items.Add("Method check"); } }
            else if (is_dic == 0) { if (items.Contains("Method check")) { items.Remove("Method check"); } }

            if (is_memory == 1) { if (!items.Contains("OCR")) { items.Add("OCR"); } }
            else if (is_memory == 0) { if (items.Contains("OCR")) { items.Remove("OCR"); } }

            if (is_db == 1) { if (!items.Contains("Data search")) { items.Add("Data search"); } }
            else if (is_db == 0) { if (items.Contains("Data search")) { items.Remove("Data search"); } }

            this.Close();
        }

        private void button5_Click_1(object sender, EventArgs e) // log save button
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show(this, "No Log here, Please Execute", "Error");
                return;
            }
            String item = null;
            using (StreamWriter outputFile = new StreamWriter(@"Log.txt", true))
            {
                outputFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    item = listView1.Items[i].ToString();
                    item = item.Replace("ListViewItem: ", "");
                    outputFile.WriteLine(item);
                }
                outputFile.WriteLine("");
            }
            MessageBox.Show(this, "Success to save log", "Info");
        }

        private void button3_Click_1(object sender, EventArgs e) // reset buton
        {
            listView1.Items.Clear();
            MessageBox.Show(this, "Success to reset log", "Info");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (is_make == 1)
            {
                ProcessStartInfo pro = new ProcessStartInfo();
                pro.FileName = @"cmd";
                pro.WorkingDirectory = @original_path_name + "\\";
                pro.CreateNoWindow = true;
                pro.UseShellExecute = false;
                pro.RedirectStandardOutput = false;
                pro.RedirectStandardInput = true;
                pro.RedirectStandardError = false;
                String cmd_str = "Report_" + apk_name + "_" + select_platform + ".docx";
                Run_process(pro, cmd_str, false);
            }
            else
            {
                MessageBox.Show(this, "Please click make button", "Error");
            }
        }

        // @@@@ Buttons @@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

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
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            button3.BackColor = Color.DodgerBlue;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(64, 64, 64);
        }

        #endregion

        private void metadata_check_CheckedChanged(object sender, EventArgs e)
        {
            if(is_mdf_ok == 1)
            {
                if (metadata_check.Checked == true) { is_meta = 1; label6.BackColor = Color.Green; }
                else if (metadata_check.Checked == false) { is_meta = 0; label6.BackColor = Color.Red; }
            }
        }

        private void dictionary_check_CheckedChanged(object sender, EventArgs e)
        {
            if(is_hook_ok == 1)
            {
                if (dictionary_check.Checked == true) { is_dic = 1; label1.BackColor = Color.Green; }
                else if (dictionary_check.Checked == false) { is_dic = 0; label1.BackColor = Color.Red; }
            }
        }

        private void library_check_CheckedChanged(object sender, EventArgs e)
        {
            if (is_ocr_runned == 1)
            {
                if (library_check.Checked == true) { is_memory = 1; label4.BackColor = Color.Green; }
                else if (library_check.Checked == false) { is_memory = 0; label4.BackColor = Color.Red; }
            }
        }

        private void DBsearch_check_CheckedChanged(object sender, EventArgs e)
        {
            if (is_db_runned == 1)
            {
                if (DBsearch_check.Checked == true) { is_db = 1; label7.BackColor = Color.Green; }
                else if (DBsearch_check.Checked == false) { is_db = 0; label7.BackColor = Color.Red; }
            }
        }

        private void random_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if (R_status.Count > 0)
            {
                if (random_checkbox.Checked == true) { is_random = 1; label8.BackColor = Color.Green; }
                else if (random_checkbox.Checked == false) { is_random = 0; label8.BackColor = Color.Red; }
            }
        }

        private void speed_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if(T_status.Count > 0)
            {
                if (speed_checkbox.Checked == true) { is_speed = 1; label9.BackColor = Color.Green; }
                else if (speed_checkbox.Checked == false) { is_speed = 0; label9.BackColor = Color.Red; }
            }
        }

        private void metadata_check_Click(object sender, EventArgs e)
        {
            if (is_mdf_ok == 0) { MessageBox.Show(this, "Please run MetaData first", "Error"); metadata_check.Checked = false; return; }
        }

        private void dictionary_check_Click(object sender, EventArgs e)
        {
            if (is_hook_ok == 0) { MessageBox.Show(this, "Please run Method Search & Hooking first", "Error"); dictionary_check.Checked = false; return; }
        }

        private void library_check_Click(object sender, EventArgs e)
        {
            if (is_ocr_runned == 0) { MessageBox.Show(this, "Please run Memory first", "Error"); library_check.Checked = false; return; }
        }

        private void speed_checkbox_Click(object sender, EventArgs e)
        {
            if (T_status.Count == 0) { MessageBox.Show(this, "Please run Time first", "Error"); speed_checkbox.Checked = false; return; }
        }

        private void random_checkbox_Click(object sender, EventArgs e)
        {
            if (R_status.Count == 0) { MessageBox.Show(this, "Please run Random first", "Error"); random_checkbox.Checked = false; return; }
        }

        private void DBsearch_check_Click(object sender, EventArgs e)
        {
            if (is_db_runned == 0) { MessageBox.Show(this, "Please run Data Search first", "Error"); DBsearch_check.Checked = false; return; }
        }
    }
}
