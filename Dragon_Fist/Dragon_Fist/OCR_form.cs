using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenCvSharp;
using Tesseract;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;


namespace Dragon_Fist
{
    public partial class OCR_form : Form
    {
        class Fieldmanager
        {
            String Field_name;
            String Field_offset;
            List<String> Method_names;
            List<String> Method_offsets;
            String Class_name;

            public Fieldmanager()
            {
                this.Field_name = null;
                this.Field_offset = null;
                this.Method_names = new List<String>();
                this.Method_offsets = new List<String>();
                this.Class_name = null;
            }

            internal void set_name(String name)
            {
                this.Field_name = name;
            }
            internal void set_offset(String offset)
            {
                this.Field_offset = offset;
            }
            internal void set_Method_names(String M_name)
            {
                this.Method_names.Add(M_name);
            }
            internal void set_Method_offsets(String M_offset)
            {
                this.Method_offsets.Add(M_offset);
            }
            internal void set_Class_name(String C_name)
            {
                this.Class_name = C_name;
            }
            internal String get_name()
            {
                return this.Field_name;
            }
            internal String get_offset()
            {
                return this.Field_offset;
            }
            internal List<String> get_Method_names()
            {
                return this.Method_names;
            }
            internal List<String> get_Method_offsets()
            {
                return this.Method_offsets;
            }
            internal String get_Class_name()
            {
                return this.Class_name;
            }
        }
        public class Field_address
        {
            String Address;
            String name;
            int value;

            public Field_address(String add, String name, int value)
            {
                this.Address = add;
                this.name = name;
                this.value = value;
            }

            internal void set_address(String add)
            {
                this.Address = add;
            }
            internal void set_name(String name)
            {
                this.name = name;
            }
            internal void set_value(int val)
            {
                this.value = val;
            }
            internal String get_address()
            {
                return this.Address;
            }
            internal String get_name()
            {
                return this.name;
            }
            internal int get_value()
            {
                return this.value;
            }
        }

        private Image _originalImage;
        private bool _selecting;
        private Rectangle _selection;
        bool _selected = false;
        Process current_proc;
        List<Fieldmanager> Fieldman = new List<Fieldmanager>();
        public List<String> result = new List<String>();
        List<Field_address> Fieldadd = new List<Field_address>();
        List<String> Checked_Addresses = new List<String>();
        int OCR_Value = 0xdead;
        List<uint> Checked_number = new List<uint>();
        public AutoResetEvent memsearch_finish = new AutoResetEvent(false);
        public AutoResetEvent memcorrupt_finish = new AutoResetEvent(false);
        bool is_value_search = false;
        bool is_class_search = false;
        public AutoResetEvent getthis_finish = new AutoResetEvent(false);
        List<List<String>> items_list = new List<List<String>>();
        List<String> items = new List<String>();

        // Socket
        bool server_running = false;
        bool process_on = false;
        bool server_fin = false;
        Thread socket_server = null;
        UdpClient srv = null;

        // MemWrap
        bool MemWrap_process = false;
        bool MemWrap_fin = false;

        bool clicked = false;
        bool clicked_t = false;
        System.Windows.Forms.VisualStyles.CheckBoxState state;

        int is_captured = 0;
        int is_capture_button_fin= 0;
        String dump_path = null;
        String package_name = null;
        int is_ocr_runned = 0;

        public OCR_form(String _dump_path, String _package_name)
        {
            InitializeComponent();

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.HeaderStyle = ColumnHeaderStyle.Clickable;
            listView1.Columns.Add("Name", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("Address", 150, HorizontalAlignment.Center);
            listView1.Columns.Add("Value", 140, HorizontalAlignment.Center);

            listView2.View = View.Details;
            listView2.FullRowSelect = true;
            listView2.GridLines = true;
            listView2.HeaderStyle = ColumnHeaderStyle.Clickable;
            listView2.Columns.Add("Name", 210, HorizontalAlignment.Center);
            listView2.Columns.Add("Offset", 210, HorizontalAlignment.Center);

            listView3.View = View.Details;
            listView3.FullRowSelect = true;
            listView3.GridLines = true;
            listView3.HeaderStyle = ColumnHeaderStyle.Clickable;
            listView3.Columns.Add("Name", 150, HorizontalAlignment.Center);
            listView3.Columns.Add("Address", 150, HorizontalAlignment.Center);

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

            dump_path = _dump_path;
            package_name = _package_name;

            socket_server = new Thread(new ThreadStart(udpserverStart));
            socket_server.IsBackground = true;
            socket_server.Start();
        }

        public List<List<String>> get_items_list() { return items_list; }

        public List<String> get_items() { return items; }

        public int get_is_ocr_runned() { return is_ocr_runned; }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Starting point of the selection:
            if (e.Button == MouseButtons.Left && !_selected)
            {
                _selecting = true;
                _selection = new Rectangle(new System.Drawing.Point(e.X, e.Y), new System.Drawing.Size());
                is_capture_button_fin = 1;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Update the actual size of the selection:
            if (_selecting)
            {
                int _X = e.X;
                int _Y = e.Y;
                if (pictureBox1.Width < e.X)
                    _X = pictureBox1.Width;
                if (pictureBox1.Height < e.Y)
                    _Y = pictureBox1.Height;
                _selection.Width = _X - _selection.X;
                if (_selection.Width <= 0)
                    _selection.Width = 0;
                _selection.Height = _Y - _selection.Y;
                if (_selection.Height <= 0)
                    _selection.Height = 0;

                _selected = true;

                // Redraw the picturebox:
                pictureBox1.Refresh();
            }

        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if(is_captured == 1)
            {
                if (_selection.Height == 0 || _selection.Width == 0)
                {
                    _selected = false;
                    return;
                }
                if (e.Button == MouseButtons.Left && _selecting)
                {
                    // Create cropped image:
                    Image img = pictureBox1.Image.Crop(_selection);

                    // Fit image to the picturebox:
                    pictureBox1.Image = img.Fit2PictureBox(pictureBox1);

                    _selecting = false;
                }
            }
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_selecting)
            {
                // Draw a rectangle displaying the current selection
                Pen pen = Pens.Red;
                e.Graphics.DrawRectangle(pen, _selection);
            }
        }

        ////////////////////////////////////
        private void udpserverStart()
        {
            try
            {
                srv = new UdpClient(5572);
                IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

                server_running = true;
                MemWrap_process = true;

                while (server_running)
                {
                    byte[] dgram = srv.Receive(ref clientEP);
                    String tmp = Encoding.Default.GetString(dgram);
                    result.Add(tmp);
                        if (tmp.Contains("Memory Scan Finished!"))
                        {
                            memsearch_finish.Set();
                            MemWrap_process = false;
                        }
                        if (tmp.Contains("Memory Corrupt Finished!"))
                        {
                            memcorrupt_finish.Set();
                        }
                        if (tmp.Contains("this :"))
                        {
                            getthis_finish.Set();
                        }
                }
            }
            catch (Exception e)
            {
                srv.Close();
            }
            finally
            {

            }
        }

        private void frida_check()
        {
            ProcessStartInfo proInfo = new ProcessStartInfo();
            Process current_pro = new Process();

            proInfo.FileName = @"cmd";
            proInfo.WorkingDirectory = Application.StartupPath;
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardOutput = true;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardError = true;
            current_pro.StartInfo = proInfo;
            current_pro.Start();

            //capture current device screen.
            String cmd_str = "frida-ps -U";
            current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            String txt = current_pro.StandardOutput.ReadToEnd();
            current_pro.WaitForExit();
            current_pro.Close();

        }
        private void GenerateJSCode_memscan(String Searched_Value)
        {
            String code = "function memscan(pattern) {\n" +
                "    var ranges = Process.enumerateRangesSync({protection: 'rw-', });\n" +
                "    var range;\n" +
                "    function processNext(){\n" +
                "      range = ranges.pop();\n" +
                "       if(!range){\n" +
                "           send('Memory Scan Finished!');\n" +
                "           return;\n" +
                "       }\n" +
                "       Memory.scan(range.base, range.size, pattern, {\n" +
                "           onMatch: function(address, size){\n" +
                "               send('[+] Pattern found at: ' + address.toString());\n" +
                "           }, \n" +
                "           onError: function(reason){\n" +
                "               ;\n" +
                "           }, \n" +
                "           onComplete: function(){\n" +
                "               processNext();\n" +
                "           }\n" +
                "       });\n" +
                "   }\n" +
                "   processNext();\n" +
                "}\n" +

                "Java.perform(function(){\n" +
                "    memscan('" + Searched_Value + "');\n" +
                "});\n";

            using (StreamWriter outFile = new StreamWriter(@Application.StartupPath + @"\Modules\hook\MemSearch.js"))
            {
                outFile.Write(code);
            }
        }

        private void GenerateHookCode_memscan()
        {
            String source = File.ReadAllText(@Application.StartupPath + @"\Modules\hook\base_udp.py");
            String jscode = File.ReadAllText(@Application.StartupPath + @"\Modules\hook\MemSearch.js");

            int jscodeIndex = source.IndexOf("'''");

            String Payload = source.Substring(0, jscodeIndex + 4);
            Payload += jscode;
            Payload += source.Substring(jscodeIndex + 4);

            using (StreamWriter outFile = new StreamWriter(@Application.StartupPath + @"\Modules\hook\MemSearch.py"))
            {
                outFile.Write(Payload);
            }
        }

        private void GenerateJSCode_getthis(List<List <String>> target)
        {
            String code = "function check(){\n" +
                "    send(' - Process id : ' + Process.id);\n" +
                "    send(' - Process arch : ' + Process.arch);\n" +
                "    send(' - isDebuggerAttached : ' + Process.isDebuggerAttached());\n" +
                "}\n\n" +

                "function dumpAddr(info, addr, size) {\n" +
                "    if (addr.isNull())\n" +
                "        return;\n" +
                "    send('Data dump ' + info + ' :');\n" +
                "    var buf = Memory.readByteArray(addr, size);\n" +
                "    send(hexdump(buf, { offset: 0, length: size, header: true, ansi: false }));\n" +
                "}\n\n" +


                "Java.perform(function(){\n" +
                "    send('Hooking Start ...');\n" +
                "    check();\n\n" +

                "    var il2cpp = Module.getBaseAddress('libil2cpp.so');\n" +
                "    send('[*] libil2cpp.so @ ' + il2cpp.toString());\n\n";

            List<List<String>> arrays = new List<List<String>>();

            foreach (List<String> sub in target)
            {
                List<String> tmp = new List<String>();
                String plain = sub[1];
                var idx1 = plain.IndexOf('(');
                if (!plain.Contains('('))
                    idx1 = plain.IndexOf(']');

                String function = plain.Substring(0, idx1);

                tmp.Add(sub[0]); // add Class
                tmp.Add(function); // add Function name
                tmp.Add(sub[2]); // add Offset

                if (idx1 + 1 == plain.IndexOf(')'))
                {
                    arrays.Add(tmp);
                    continue;
                }

                String[] tt = plain.Substring(idx1 + 1, plain.Length - idx1 - 2).Split(',');
                foreach (String str in tt)
                {
                    tmp.Add(str); // add arguments
                }

                arrays.Add(tmp);
            }

            int i = 0;
            foreach (List<String> array in arrays)
            {
                String code2;
                code2 = "    var offset" + i.ToString() + " = " + array[2] + ";\n"
                    + "    var target" + i.ToString() + " = il2cpp.add(offset" + i.ToString() + ");\n"
                    + "    send('" + array[1] + "() @ ' + target" + i.ToString() + ".toString());\n"
                    + "    Interceptor.attach(target" + i.ToString() + ", {\n"
                    // onEnter()
                    + "        onEnter : function(args){\n"
                    + "            send('======');\n"
                    + "            send('[*] Class : " + array[0] + "');\n"
                    + "            send('[*] Function : ";

                code2 += array[1] + "(";
                for (int j = 3; j < array.Count; j++)
                {
                    code2 += " " + array[j];
                    if (j != array.Count - 1)
                        code2 += ",";
                }
                code2 += ") :: onEnter()');\n"
                    + "            send('[*] Offset : " + array[2] + "');\n";

                // static type이 아니면 args[0]가 this object임
                int idx1 = array[1].LastIndexOf(' ');
                String tmp1 = array[1].Substring(0, idx1);
                //MessageBox.Show(tmp1);
                var dumpSize = 0x100;

                if (tmp1.Contains("static")) // this 없음
                {
                }
                else // this가 args[0]에 추가됨
                {
                    code2 += "            send('this : ' + args[0]);\n";

                    /////////////////////////////////////////////////////////////////////////////////
                    // onLeave()
                    code2 += ""
                        + "        },\n"
                        + "        onLeave : function(retval){\n"
                        + "        }\n    })\n";
                    code += code2;
                    i++;
                }
            }

            code += "});\n";

            using (StreamWriter outFile = new StreamWriter(Application.StartupPath + @"\Modules\hook\get_this.js"))
            {
                outFile.Write(code);
            }

        }

        public static String OCR_proc(Bitmap image)
        {
            Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(image);
            Mat dst = new Mat(src.Size(), MatType.CV_8UC1);
            Cv2.CvtColor(src, dst, ColorConversionCodes.BGR2GRAY);

            Cv2.Threshold(dst, src, 127, 255, ThresholdTypes.Otsu);

            Bitmap img = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(src);
            var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractAndCube);
            var texts = ocr.Process(img);

            return texts.GetText();
        }

        public static void OCR_Capture(String path)
        {
            FileInfo fi = new FileInfo(path + "\\screen.png");
            if (fi.Exists)
            {
                File.Delete(path + "\\screen.png");
            }

            ProcessStartInfo proInfo = new ProcessStartInfo();
            Process current_pro = new Process();

            proInfo.FileName = @"cmd";
            proInfo.WorkingDirectory = path + "\\";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardOutput = true;
            proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardError = true;
            current_pro.StartInfo = proInfo;
            current_pro.Start();

            //capture current device screen.
            String cmd_str = "adb shell screencap -p /data/local/tmp/screen.png";
            current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            current_pro.StandardInput.Close();
            String txt = current_pro.StandardOutput.ReadToEnd();
            current_pro.WaitForExit();
            current_pro.Close();

            current_pro = new Process();
            current_pro.StartInfo = proInfo;
            current_pro.Start();

            cmd_str = "adb pull /data/local/tmp/screen.png";
            current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            current_pro.StandardInput.Close();
            txt = current_pro.StandardOutput.ReadToEnd();
            current_pro.WaitForExit();
            current_pro.Close();

            current_pro = new Process();
            current_pro.StartInfo = proInfo;
            current_pro.Start();

            cmd_str = "adb shell rm /sdcard/screen.png";
            current_pro.StandardInput.Write(@cmd_str + Environment.NewLine);
            current_pro.StandardInput.Close();
            txt = current_pro.StandardOutput.ReadToEnd();
            current_pro.WaitForExit();
            current_pro.Close();
        }

        public void MemWrap(String PackageName, String Address)
        {
            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = @"python.exe";
            proInfo.Arguments = String.Format("{0} {1} {2}", @Application.StartupPath + @"\Modules\hook\MemWrap.py", PackageName, Address);
            proInfo.WorkingDirectory = @Application.StartupPath + @"\Modules\hook\";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;

            current_proc = new Process();
            current_proc.StartInfo = proInfo;
            current_proc.Start();

            MemWrap_process = true;
        }
        static string LittleEndian(int num)
        {
            int number = num;
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
            {
                retval += b.ToString("X2");
                retval += ' ';
            }
            retval = retval.Replace(" 00", "");
            return retval;
        }


        public void MemSearch(int value)
        {
            String Str_Val = LittleEndian(value);
            ListViewItem itm;

            GenerateJSCode_memscan(Str_Val);
            GenerateHookCode_memscan();

            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = "python.exe";
            proInfo.Arguments = String.Format("{0} {1}", @Application.StartupPath + @"\Modules\hook\MemSearch.py", "com.Dragonfist.Where"); // 
            proInfo.WorkingDirectory = @Application.StartupPath + @"\Modules\hook\";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;

            current_proc = new Process();
            current_proc.StartInfo = proInfo;
            current_proc.Start();

            memsearch_finish.WaitOne();
            current_proc.Kill();

            MessageBox.Show(this, "Address Found!\n\n" + result.Count.ToString(), "Info");
            process_on = true;
            int i = 0;
            
            foreach (String line in result)
            {
                if(line.Contains("[+]"))
                {
                    Fieldadd.Add(new Field_address(line.Split(':')[1], "null", value));
                    itm = new ListViewItem(new String[] { Fieldadd[i].get_name(), Fieldadd[i].get_address(), Fieldadd[i].get_value().ToString() });
                    listView1.Items.Add(itm);
                    i++;
                }
            }
            memsearch_finish.Reset();
            result.Clear();
        }

        public void ClassSearch(String dump_path, String name)
        {
            //search var offset in dump.cs with its name.
            //asd
            if(!File.Exists(@dump_path + "dump.cs"))
            {
                MessageBox.Show(this, "[Error Code = 0x00]\n\nThe dumped file is not found\n\nPlease dump again or check your APK", "Error");
            }
            String[] dumpcs = File.ReadAllLines(@dump_path + "dump.cs");
            String line;
            String line_i;
            String class_pre = null;
            int count = 0;
            int p;
            long j;
            Fieldman.Clear();
            for (long i = 0; i < dumpcs.Length; i++)
            {
                line = dumpcs[i];
                line = line.Trim();
                if (line.Contains("class"))
                {
                    class_pre = line.Split('/')[0];
                }
                if (line.Contains(name) && !line.Contains("(") && !line.Contains("class"))
                {
                    Fieldman.Add(new Fieldmanager());
                    Fieldman[count].set_name(line.Split(';')[0]);
                    Fieldman[count].set_offset(line.Split('/')[2]);
                    Fieldman[count].set_Class_name(class_pre);
                    j = 0;
                    while(true)
                    {
                        line_i = dumpcs[i + j].Trim();
                        if (line_i.Contains("Methods"))
                        {
                            j++;
                            break;
                        }
                        j++;
                    }
                    while (dumpcs[i + j + 1].Contains("RVA"))
                    {
                        line_i = dumpcs[i + j];
                        if(line_i.Contains('['))
                        {
                            j++;
                            continue;
                        }
                        Fieldman[count].set_Method_names(line_i.Trim().Split('/')[0].Split(';')[0]);
                        Fieldman[count].set_Method_offsets(line_i.Trim().Split(':')[1].Split(' ')[1]);
                        j++;
                    }
                    count++;
                }
            }

            listView1.Items.Clear();
            ListViewItem itm;
            for(int i = 0; i< Fieldman.Count; i++)
            {
                itm = new ListViewItem(new String[] { Fieldman[i].get_Class_name(), Fieldman[i].get_name(), Fieldman[i].get_offset(), "NULL" });
                listView1.Items.Add(itm);
            }
            /*
            //choice function to hook in the vars class.
            //with user input thru UI.
            String[] input_func_RVA = { "###input function's RVA###", };
            //change to array of String.
            String[] method_offset = { "###method's offset###", };
            //change to array of String.

            for (int i = 0; i < Fieldman.Length; i++)
            {
                Method_address[i] = MethodSearch(input_func_RVA[i], method_offset[i]);
            }

            //input address to Addresses variable.
            //call MemCorrupt with mem addresses with a loop.
            for (int i = 0; i < Method_address.Length; i++)
            {
                //MemCorrupt(Method_address[i]);
                //check routine!!
            }
            //done.
            */
        }

        public void MemCorrupt(String Address)
        {
            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = @"python.exe";
            proInfo.Arguments = String.Format("{0} {1} {2}", @Application.StartupPath + @"\Modules\hook\MemCorrupt.py", "com.DragonFist.Where", Address);
            //python MemCorrupt.py @(Process_name) @(Address)
            proInfo.WorkingDirectory = @Application.StartupPath + @"\Modules\hook\";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            proInfo.RedirectStandardOutput = true;

            current_proc = new Process();
            current_proc.StartInfo = proInfo;
            current_proc.Start();
        }

        public String MethodSearch(String func_RVA, String method_offset)
        {
            //get this ptr.
            //with frida pycode.
            //calc exact address in memory with the variables offset.
            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = @"python.exe";
            proInfo.Arguments = String.Format("{0} {1} {2}", @Application.StartupPath + @"Modules\hook\ClassSearch.py", func_RVA, method_offset);
            proInfo.WorkingDirectory = @Application.StartupPath + @"\Modules\hook\";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;
            //proInfo.RedirectStandardInput = true;
            proInfo.RedirectStandardOutput = true;

            current_proc = new Process();
            current_proc.StartInfo = proInfo;
            current_proc.Start();
            //get output
            String Address = "###output Address###";
            return Address;
        }

        public void TraceSearch(String dump_path)
        {

            //get ClassSearch's result(Back Trace).
            //get target Class name through address which is result of Back Trace function.

            int address = int.Parse("0xdeadbeef");
            int re_address = 0;
            long count = 0xfffffffffffffff;
            String func_name = null;
            int flag = 0;
            String[] dumpcs = File.ReadAllLines(dump_path + "\\dump.cs");


            foreach (String line in dumpcs)
            {
                if (line.Contains("RVA"))
                {
                    flag = int.Parse(line.Split(':')[1].Remove(' ')) - address;
                    if (flag <= count)
                    {
                        count = flag;
                        func_name = line.Split(';')[0];
                        re_address = int.Parse(line.Split(':')[1].Remove(' '));
                    }
                }
            }

            //do ClassSearch for exact one.

            //classSearch UI!!!!
        }

        private void button6_Click(object sender, EventArgs e) // Capture Display
        {
            String tmp_path = @Application.StartupPath;
            //try
            //{
                OCR_Capture(tmp_path);
            //}
            //catch (Exception ocr_e1)
            //{
               // MessageBox.Show(this, "[Error Code = 0x60]\n\nFail to capture display", "Error");
               // return;
            //}
            String capture_path = tmp_path + "\\screen.png";
            FileInfo fi = new FileInfo(capture_path);
            if (!fi.Exists)
            {
                MessageBox.Show(this, "[Error Code = 0x61]\n\nscreen.png is not exist", "Error");
                return;
            }
            Bitmap pict = new Bitmap(tmp_path + "\\screen.png");
            pictureBox1.Image = new Bitmap(pict, new System.Drawing.Size(pict.Width / 3, pict.Height / 3));
            _originalImage = new Bitmap(pictureBox1.Image);
            pict.Dispose();
            is_captured = 1;
            _selected = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (is_captured == 1)
            {
                pictureBox1.Image = _originalImage.Clone() as Image;
                _selected = false;
            }
            else
            {
                MessageBox.Show(this, "Please capture first", "Error");
            }
        }

        private void button1_Click_Value_Search(object sender, EventArgs e)
        {
            if (is_captured == 1 && is_capture_button_fin == 1)
            {
                if (!CheckFrida("zygote"))
                {
                    MessageBox.Show("Cannot Find running frida-server", "Error");
                    return;
                }
                if(!CheckFrida(package_name))
                {
                    MessageBox.Show("Cannot Find "+package_name, "Error");
                    return;
                }
                is_class_search = false;
                is_value_search = true;
                listView2.Columns.Clear();
                listView2.Items.Clear();
                listView1.Columns.Clear();
                listView1.Items.Clear();
                listView1.Columns.Add("Name", 150, HorizontalAlignment.Center);
                listView1.Columns.Add("Address", 150, HorizontalAlignment.Center);
                listView1.Columns.Add("Value", 140, HorizontalAlignment.Center);
                try
                {
                    String OCR = OCR_proc(new Bitmap(pictureBox1.Image));
                    OCR = Regex.Replace(OCR, @"\D", "");
                    int int_result = int.Parse(OCR);
                    OCR_Value = int_result;
                    MessageBox.Show(this, "OCR Value is :\n" + OCR_Value, "Info");
                    MemSearch(int_result);
                }
                catch(Exception ocr)
                {
                    MessageBox.Show(this, "Please capture only numbers", "Error");
                }
            }
            else
            {
                MessageBox.Show(this, "Please capture first", "Error");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            String path = Application.StartupPath;
            FileInfo fi = new FileInfo(path + "\\screen.png");
            if (fi.Exists)
            {
                File.Delete(path + "\\screen.png");
            }
            server_running = false;
            srv.Close();
            socket_server.Abort();
            this.Close();
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            TextFormatFlags flags = TextFormatFlags.LeftAndRightPadding;
            e.DrawBackground();
            CheckBoxRenderer.DrawCheckBox(e.Graphics, ClientRectangle.Location, state);
            e.DrawText(flags);
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (!clicked)
            {
                clicked = true;
                state = System.Windows.Forms.VisualStyles.CheckBoxState.CheckedPressed;
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Checked = true;
                }
                Invalidate();
            }
            else
            {
                clicked = false;
                state = System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal;
                Invalidate();
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Checked = false;
                }
            }
        }

        private void GenerateHookCode_getthis()
        {
            String source = File.ReadAllText(Application.StartupPath + @"\Modules\hook\base_udp.py");
            String jscode = File.ReadAllText(Application.StartupPath + @"\Modules\hook\get_this.js");

            int jscodeIndex = source.IndexOf("'''");

            String Payload = source.Substring(0, jscodeIndex + 4);
            Payload += jscode;
            Payload += source.Substring(jscodeIndex + 4);

            using (StreamWriter outFile = new StreamWriter(Application.StartupPath + @"\Modules\hook\get_this.py"))
            {
                outFile.Write(Payload);
            }
        }

        private void Button4_Click_Exec(object sender, EventArgs e) // Run
        {
            if (is_captured == 1)
            {
                Checked_Addresses.Clear(); //변조 할 메모리 주소를 저장하는 Checked_Addresses를 초기화.

                foreach (ListViewItem itm in listView1.Items)
                {
                    if (itm.Checked)
                    {
                        if (itm.SubItems[1].Text.Contains("0x"))
                            Checked_Addresses.Add(itm.SubItems[1].Text);
                        else if (itm.SubItems[3].Text.Contains("0x"))
                        {
                            Checked_Addresses.Add(itm.SubItems[3].Text);
                        }
                    }
                }
                OCR_Value = int.Parse(OCR_proc((Bitmap)pictureBox1.Image));
                //MessageBox.Show(this, "Check done!", "Info");
                for (int i = 0; i < Checked_Addresses.Count; i++)
                {
                    if(Checked_Addresses[i].Contains("NULL"))
                    {
                        MessageBox.Show("Find Address First!");
                        return;
                    }
                    MemCorrupt(Checked_Addresses[i]);
                    memcorrupt_finish.WaitOne();
                    current_proc.Kill();
                    OCR_Capture(@Application.StartupPath);
                    Thread.Sleep(1000);
                    pictureBox1.Image = null;
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    Image new_cap = new Bitmap(Application.StartupPath + "\\screen.png");
                    new_cap = new Bitmap(new_cap, new System.Drawing.Size(new_cap.Width / 3, new_cap.Height / 3));
                    pictureBox1.Image = new_cap.Crop(_selection).Fit2PictureBox(pictureBox1);
                    new_cap.Dispose();
                    if (OCR_Value != int.Parse(OCR_proc((Bitmap)pictureBox1.Image)))
                    {
                        MessageBox.Show(this, "Address Found!\n\n" + Checked_Addresses[i], "Info");
                        foreach(ListViewItem itm in listView1.Items)
                        {
                            if (itm.Checked)
                            {
                                if (itm.SubItems[1].Text.Contains(Checked_Addresses[i]))
                                {
                                    items_list.Add(new List<String>());
                                    items_list[items_list.Count - 1].Add("Value");//어떤 기능의 결과인지 알려줌
                                    items_list[items_list.Count - 1].Add("NULL");//name
                                    items_list[items_list.Count - 1].Add(itm.SubItems[1].Text);//address
                                    items_list[items_list.Count - 1].Add(itm.SubItems[2].Text);//원래 가지고있던 값
                                    listView3.Items.Add(new ListViewItem(new String[] { "NULL", itm.SubItems[1].Text }));
                                    break;
                                }
                                else if (itm.SubItems[3].Text.Contains(Checked_Addresses[i]))
                                {
                                    items_list.Add(new List<String>());
                                    items_list[items_list.Count - 1].Add("Class");//어떤 기능의 결과인지 알려줌
                                    items_list[items_list.Count - 1].Add(itm.SubItems[0].Text);//속한 Class
                                    items_list[items_list.Count - 1].Add(itm.SubItems[1].Text);//name
                                    items_list[items_list.Count - 1].Add(itm.SubItems[3].Text);//Address
                                    listView3.Items.Add(new ListViewItem(new String[] { itm.SubItems[1].Text, itm.SubItems[3].Text }));
                                    break;
                                }
                            }
                        }
                        return;
                    }
                    memcorrupt_finish.Reset();
                    result.Clear();
                }
                MessageBox.Show(this, "Not Found :(", "Error");
                is_ocr_runned = 1;
            }
            else
            {
                MessageBox.Show(this, "Please capture first", "Error");
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength <= 1)
            {
                MessageBox.Show(this, "Please enter value name", "Error");
                return;
            }
            if (is_captured == 1 && is_capture_button_fin == 1)
            {
                if (!CheckFrida("zygote"))
                {
                    MessageBox.Show("Cannot Find running frida-server", "Error");
                    return;
                }
                if (!CheckFrida(package_name))
                {
                    MessageBox.Show("Cannot Find " + package_name, "Error");
                    return;
                }
                is_class_search = true;
                is_value_search = false;
                listView1.Columns.Clear();
                listView1.Items.Clear();
                listView1.Columns.Add("Class", 140, HorizontalAlignment.Center);
                listView1.Columns.Add("Name", 130, HorizontalAlignment.Center);
                listView1.Columns.Add("Offset", 85, HorizontalAlignment.Center);
                listView1.Columns.Add("Address", 85, HorizontalAlignment.Center);
                listView2.Columns.Add("Name", 210, HorizontalAlignment.Center);
                listView2.Columns.Add("Offset", 210, HorizontalAlignment.Center);
                if (textBox1.Text == null)
                {
                    MessageBox.Show(this, "Enter value name to search!", "Error");
                    return;
                }
                String name = textBox1.Text;
                ClassSearch(dump_path, name);
            }
            else
            {
                MessageBox.Show(this, "Please capture first", "Error");
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(is_value_search)
            {
                return;
            }
            else
            {
                foreach(ListViewItem itm in listView1.SelectedItems)
                {
                    List<String> names = Fieldman[itm.Index].get_Method_names();
                    List<String> offsets = Fieldman[itm.Index].get_Method_offsets();
                    listView2.Items.Clear();
                    for(int i = 0; i < names.Count; i++)
                    {
                        listView2.Items.Add(new ListViewItem(new string[] { names[i], offsets[i] }));
                    }
                }
            }
        }
        private void Button8_Click(object sender, EventArgs e)
        {
            if (!CheckFrida("zygote"))
            {
                MessageBox.Show("Cannot Find running frida-server", "Error");
                return;
            }
            if (!CheckFrida(package_name))
            {
                MessageBox.Show("Cannot Find " + package_name, "Error");
                return;
            }
            if (is_captured == 1)
            {
                if(listView1.Items.Count > 0)
                {
                    Checked_number.Clear();
                    foreach (ListViewItem itm in listView1.Items)
                    {
                        if (itm.Checked)
                        {
                            Checked_number.Add((uint)itm.Index);
                        }
                    }
                    if(Checked_number.Count > 0)
                    {
                        int i = (int)Checked_number[0];

                        List<String> names = Fieldman[i].get_Method_names();
                        List<String> offsets = Fieldman[i].get_Method_offsets();
                        String class_name = Fieldman[i].get_Class_name();

                        List<List<String>> list = new List<List<string>>();

                        for (int j = 0; j < names.Count; j++)
                        {
                            list.Add(new List<String>());
                            list[j].Add(class_name);
                            list[j].Add(names[j]);
                            list[j].Add(offsets[j]);
                        }
                        GenerateJSCode_getthis(list);
                        GenerateHookCode_getthis();


                        ProcessStartInfo proInfo = new ProcessStartInfo();
                        proInfo.FileName = @"python.exe";
                        proInfo.Arguments = String.Format("{0} {1}", @Application.StartupPath + @"\Modules\hook\get_this.py", package_name);
                        proInfo.WorkingDirectory = @Application.StartupPath + @"\Modules\hook\";
                        proInfo.CreateNoWindow = true;
                        proInfo.UseShellExecute = false;
                        //proInfo.RedirectStandardInput = true;
                        proInfo.RedirectStandardOutput = true;

                        current_proc = new Process();
                        current_proc.StartInfo = proInfo;
                        current_proc.Start();

                        getthis_finish.WaitOne();
                        current_proc.Kill();
                        String this_add = "0";
                        foreach(String ad in result)
                        {
                            if(ad.Contains("this"))
                                this_add = ad.Split(':')[1].Trim();

                        }
                        foreach (ListViewItem itm in listView1.Items)
                        {
                            if (itm.Checked)
                            {
                                long a = long.Parse(this_add.Replace("0x", "").Trim(), System.Globalization.NumberStyles.HexNumber);
                                long b = long.Parse(itm.SubItems[2].Text.Replace("0x", "").Trim(), System.Globalization.NumberStyles.HexNumber);
                                itm.SubItems[3].Text = String.Format("0x{0:x}", a + b);
                                break;
                            }
                        }
                        result.Clear();
                    }
                }
                else
                {
                    MessageBox.Show(this, "Not found", "Error");
                }
            }
            else
            {
                MessageBox.Show(this, "Please capture first", "Error");
            }
        }

        private void button5_Click(object sender, EventArgs e) // Add to report
        {
            if(items_list.Count > 0)
            {
                items.Add("OCR");
                MessageBox.Show(this, "Success to add to report", "Info");
            }
            else
            {
                MessageBox.Show(this, "Fail to add to report\n\nPlease click run button", "Error");
            }
        }

        #region Button_color

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.BackColor = Color.DodgerBlue;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(64, 64, 64);
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

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2.BackColor = Color.DodgerBlue;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.FromArgb(64, 64, 64);
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

        private void button8_MouseMove(object sender, MouseEventArgs e)
        {
            button8.BackColor = Color.DodgerBlue;
        }

        private void button8_MouseLeave(object sender, EventArgs e)
        {
            button8.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button9_MouseMove(object sender, MouseEventArgs e)
        {
            button9.BackColor = Color.DodgerBlue;
        }

        private void button9_MouseLeave(object sender, EventArgs e)
        {
            button9.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void button10_MouseMove(object sender, MouseEventArgs e)
        {
            button10.BackColor = Color.DodgerBlue;
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
            button10.BackColor = Color.FromArgb(64, 64, 64);
        }

        #endregion

        private void button9_Click(object sender, EventArgs e) // Remove
        {
            if (listView3.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(this, "This item will be removed\rContinue?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = listView3.FocusedItem.Index;
                    listView3.Items.RemoveAt(index);
                    items_list.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show(this, "Not selected\n\nPlease select an item", "Error");
            }
        }

        private void button10_Click(object sender, EventArgs e) // Reset
        {
            items_list.Clear();
            listView3.Items.Clear();
            MessageBox.Show(this, "Success to reset list", "Info");
        }
        private bool CheckFrida(String proc)
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

            if (output.Contains(proc))
                return true;
            else
                return false;
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}