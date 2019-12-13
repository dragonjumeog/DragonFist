using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Drawing;

namespace Dragon_Fist
{
    public partial class Hook_form : Form
    {
        List<List<String>> target = new List<List<String>>();
        List<List<String>> Fields = new List<List<String>>();
        List<List<String>> result = new List<List<String>>();

        Process current_pro;
        bool process_on = false;

        // socket server
        int port = 5571;
        int number;
        int is_hook_ok = 0;
        UdpClient srv = null;
        Thread socket_server;
        bool server_running = false;

        String package_name = null;
        String dump_path = null;
        String platform = null;
        String arch = null;
        List<List<String>> h_list = new List<List<String>>();

        public Hook_form(List<List<String>> tg, String _package_name, List<List<String>> _h_list, String _dump_path, String _platform)
        {
            InitializeComponent();

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.Columns.Add("Timeline", 800, HorizontalAlignment.Center);

            listView2.View = View.Details;
            listView2.FullRowSelect = true;
            listView2.GridLines = true;
            listView2.Columns.Add("Details", 800, HorizontalAlignment.Center);

            listView3.View = View.Details;
            listView3.FullRowSelect = true;
            listView3.GridLines = true;
            listView3.Columns.Add("Method", 800, HorizontalAlignment.Center);

            button1.TabStop = false; button1.FlatStyle = FlatStyle.Flat; button1.FlatAppearance.BorderSize = 0;
            button2.TabStop = false; button2.FlatStyle = FlatStyle.Flat; button2.FlatAppearance.BorderSize = 0;
            button3.TabStop = false; button3.FlatStyle = FlatStyle.Flat; button3.FlatAppearance.BorderSize = 0;
            button4.TabStop = false; button4.FlatStyle = FlatStyle.Flat; button4.FlatAppearance.BorderSize = 0;

            if (_h_list.Count > 0)
            {
                h_list = _h_list;
                for (int i = 0; i < h_list.Count; i++)
                {
                    listView3.Items.Add(h_list[i][0]);
                }
            }

            target = tg;
            dump_path = _dump_path;
            package_name = _package_name;
            platform = _platform;

            socket_server = new Thread(new ThreadStart(UdpServerStart));
            socket_server.IsBackground = true;
            socket_server.Start();
        }

        public List<List<String>> get_h_list() { return h_list; }

        public int get_is_hook_ok() { return is_hook_ok; }

        private void UdpServerStart()
        {
            bool attached = false;
            server_running = true;

            try
            {
                srv = new UdpClient(port);
                IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                String msg;
                List<String> tmp = new List<String>();
                while (server_running)
                {
                    byte[] dgram = srv.Receive(ref clientEP);
                    if (dgram.Length != 0)
                    {
                        msg = Encoding.Default.GetString(dgram);
                        // consturct attched data
                        if (!attached)
                        {
                            if (msg.Contains("- Process id :"))
                            {
                                int idx = msg.IndexOf(": ");
                                PID.Text = msg.Substring(idx + 2);
                            }
                            else if (msg.Contains("- Process arch :"))
                            {
                                int idx = msg.IndexOf(": ");
                                Arch.Text = msg.Substring(idx + 2);
                            }
                            else if (msg.Contains("- isDebuggerAttached :"))
                            {
                                int idx = msg.IndexOf(": ");
                                Debuggable.Text = msg.Substring(idx + 2);
                                attached = true;
                            }

                        }

                        // Parsing message
                        if (msg.Contains("======")) // first
                        {
                            tmp = new List<String>();
                        }
                        else if (msg.Contains("######")) // last
                        {
                            listView1.Items.Add(number.ToString().PadLeft(6) + "  " + tmp[1]);
                            result.Add(tmp);
                            number += 1;
                        }
                        else // content
                        {
                            tmp.Add(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.ToString());
            }
            finally
            {
                srv.Close();
            }
        }

        private List<String> ParseFields(String line)
        {
            String[] tmp = line.Split(' ');
            int TypeIdx = 0, NameIdx = 0, OffsetIdx = 0;

            if (line.Contains("=")) // 초기화하는 경우
            {
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i] == "=")
                    {
                        NameIdx = i - 1;
                        TypeIdx = NameIdx - 1;
                        OffsetIdx = i + 3;
                    }
                }
            }
            else if (line.Contains("] // RVA")) // ex) [SerializeField]
            {
                List<String> tmp_result = new List<String>();
                tmp_result.Add(tmp[0]);
                tmp_result.Add(" ");
                tmp_result.Add("0x0");
                return tmp_result;
            }
            else // 초기화하지 않는 경우
            {
                for (int i = 0; i < tmp.Length; i++)
                {
                    if (tmp[i].Contains(";"))
                    {
                        NameIdx = i;
                        TypeIdx = NameIdx - 1;
                        OffsetIdx = i + 2;
                    }
                }
            }

            // ex) [ "string", "PlayerName", "0x10" ]
            List<String> result = new List<String>();

            if (tmp[TypeIdx - 1].Contains("unsigned"))
            {
                result.Add("unsigned " + tmp[TypeIdx]);
            }
            else
            {
                result.Add(tmp[TypeIdx]);
            }
            result.Add(tmp[NameIdx].Substring(0, tmp[NameIdx].Length - 1)); // ; 지우려고
            result.Add(tmp[OffsetIdx]);

            return result;
        }

        private void GetClassFields()
        {
            bool findClass = false;
            bool start = false;

            try
            {
                if (target.Count == 0) { return; }
                String[] lines = File.ReadAllLines(@dump_path + "\\dump.cs");
                List<String> tmp = null;
                for (int i = 0; i < target.Count; i++)
                {
                    start = false;
                    foreach (String line in lines)
                    {
                        if (line.Contains(target[i][0])) // find class
                        {
                            findClass = true;
                            tmp = new List<string>();
                            tmp.Add(line);
                        }
                        else if (findClass && line.Contains("// Fields")) // until Fields
                        {
                            start = true;
                        }
                        else if (findClass && line.Length == 0)
                        {
                            start = false;
                            break;
                        }
                        else if (findClass && start)
                        {
                            tmp.AddRange(ParseFields(line));
                        }
                    }
                    findClass = false;
                    Fields.Add(tmp);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("1.Check dump.cs file\n\n2.Please update the program", "Error");
            }
        }

        private void GenerateJSCode()
        {
            if (platform == "arm64")
                arch = "0x8";
            else
                arch = "0x4";

            String code = "function check(){\n" +
                "    send(' - Process id : ' + Process.id);\n" +
                "    send(' - Process arch : ' + Process.arch);\n" +
                "    send(' - isDebuggerAttached : ' + Process.isDebuggerAttached());\n" +
                "}\n\n" +

                "function buf2hex(buffer) {\n" +
                "   var bytearray = new Uint8Array(buffer);\n" +
                "   var hexParts = [];\n" +
                "   for (var i = 0; i < bytearray.length; i++){\n" +
                "       var hex = bytearray[i].toString(16);\n" +
                "       var padded = ('00' + hex).slice(-2);\n" +
                "       hexParts.push(padded);\n" +
                "   }\n" +
                "   return hexParts;\n" +
                "}\n\n" +

                "function endian(array, arch) {\n" +
                "   var result = '';\n" +
                "   for (var i = 0; i < 0x10 / arch; i++){\n" +
                "       var tmp = array.slice(i * arch, (i + 1) * arch);\n" +
                "       var p = '';\n" +
                "       for (var j = 0; j < tmp.length; j++){\n" +
                "           p += tmp[arch - j - 1];\n" +
                "       }\n" +
                "       result += ' 0x' + p + '  ';\n" +
                "   }\n" +
                "   return result;\n" +
                "}\n\n" +

                "function dumpAddr(info, addr, size) {\n" +
                "    if (addr.isNull())\n" +
                "        return;\n" +
                "    send('Data dump ' + info + ' :');\n" +
                "    var buf = Memory.readByteArray(addr, size);\n" +
                "    var array = buf2hex(buf);\n" +
                "    for (var i = 0; i < size/16; i++){\n" +
                "        var memAddr = (parseInt(addr) + 0x10 * i).toString(16);\n" +
                "        var sub = array.slice(i*0x10, (i+1)*0x10);\n" +
                "        send(' 0x' + memAddr + '  ' + endian(sub, " + arch + "));\n" +
                "    }\n" +
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
                    + "    var save_this" + i.ToString() + ";\n"
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
                    + "            send('[*] Offset : " + array[2] + "');\n"
                    + "            send('----------------[Registers]------------------');\n";

                code2 += "            send('[+] pc : ' + this.context.pc);\n"
                    + "            send('[+] sp : ' + this.context.sp);\n"
                    + "            var tmp1 = this.context.sp;\n"
                    + "            var result1 = tmp1;\n";

                if (platform == "arm64")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        code2 += "            tmp1 = this.context.x" + j.ToString() + ";\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] x" + j.ToString() + " : ' + result1);\n";
                    }
                }
                else if (platform == "armv7")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        code2 += "            tmp1 = this.context.r" + j.ToString() + ";\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] r" + j.ToString() + " : ' + result1);\n";
                    }
                }
                else // platform == "x86"
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int tmp = j + 97;
                        code2 += "            tmp1 = this.context.e" + (char)tmp + "x;\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] e" + (char)tmp + "x : ' + result1);\n";
                    }
                    code2 += "            tmp1 = this.context.edi;\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] edi : ' + result1);\n";

                    code2 += "            tmp1 = this.context.esi;\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] esi : ' + result1);\n";
                }

                // check arguments
                code2 += "            send('----------------[Arguments]------------------');\n";

                // static type이 아니면 args[0]가 this object임
                int idx1 = array[1].LastIndexOf(' ');
                String tmp1 = array[1].Substring(0, idx1);
                //MessageBox.Show(tmp1);
                var dumpSize = 0x100;

                if (tmp1.Contains("static")) // this 없음
                {
                    for (int j = 0; j < array.Count - 3; j++)
                    {
                        code2 += "            send('args[" + j.ToString() + "] : ' + args[" + j.ToString() + "]);\n";

                        int idx = array[j + 3].LastIndexOf(' ');
                        String tmp = array[j + 3].Substring(0, idx);
                        //MessageBox.Show(tmp);
                        if (tmp.Contains("unsigned int"))
                        {
                            code2 += "            send('    type : unsigned int');\n"
                            + "            send('    value : ' + parseInt(args[" + j.ToString() + "] ,16));\n";
                        }
                        else if (tmp.Contains("int"))
                        {
                            code2 += "            send('    type : int');\n"
                            + "            send('    value : ' + args[" + j.ToString() + "].toInt32());\n";
                        }
                        else if (tmp.Contains("string"))
                        {
                            code2 += "            send('    type : string');\n"
                                + "            send('    value : ' + Memory.readCString(args[" + j.ToString() + "]));\n";
                        }
                        else if (tmp.Contains("byte[]"))
                        {
                            int length = 0x30;
                            code2 += "            send('    type : byte[]');\n"
                                + "            send('    value : ' + Memory.readByteArray(args[" + j.ToString() + "], " + length + "));\n"; // length에 대한 개선방안 고려
                        }
                        else
                        {
                            // pointer ?
                            //code2 += "            dumpAddr('args[" + (j + 2).ToString() + "]', args[" + (j + 2).ToString() + "], " + dumpSize + ");\n";
                            code2 += "            send('    type : pointer');\n"
                                + "            var tmp = args[" + j.ToString() + "];\n"
                                + "            var result = args[" + j.ToString() + "];\n"
                                + "            while(true){\n"
                                + "                try {\n"
                                + "                    var data = Memory.readPointer(tmp);\n"
                                + "                    result = result + '  -->  ' + data;\n"
                                + "                    tmp = data;\n"
                                + "                } catch (e) {\n"
                                + "                    break;\n"
                                + "                }\n"
                                + "            }\n"
                                + "            send('    data : ' + result);\n";
                        }
                    }
                }
                else // this가 args[0]에 추가됨
                {
                    code2 += "            send('args[0] : ' + args[0]);\n"
                        + "            send('    type : pointer');\n"
                        + "            var tmp = args[0];\n"
                        + "            var result = args[0];\n"
                        + "            while(true){\n"
                        + "                try {\n"
                        + "                    var data = Memory.readPointer(tmp);\n"
                        + "                    result = result + '  -->  ' + data;\n"
                        + "                    tmp = data;\n"
                        + "                } catch (e) {\n"
                        + "                    break;\n"
                        + "                }\n"
                        + "            }\n"
                        + "            send('    data : ' + result);\n";



                    for (int j = 1; j <= array.Count - 3; j++)
                    {

                        code2 += "            send('args[" + j.ToString() + "] : ' + args[" + j.ToString() + "]);\n";
                        int idx = array[j + 2].LastIndexOf(' ');
                        String tmp = array[j + 2].Substring(0, idx);
                        // tmp has argument's data type
                        if (tmp.Contains("unsigned int"))
                        {
                            code2 += "            send('    type : unsigned int');\n"
                            + "            send('    value : ' + parseInt(args[" + j.ToString() + "] ,16));\n";
                        }
                        else if (tmp.Contains("int"))
                        {
                            code2 += "            send('    type : int');\n"
                            + "            send('    value : ' + args[" + j.ToString() + "].toInt32());\n";
                        }
                        else if (tmp.Contains("string"))
                        {
                            code2 += "            send('    type : string');\n"
                                + "            send('    value : ' + Memory.readCString(args[" + j.ToString() + "]));\n";
                        }
                        else if (tmp.Contains("byte[]"))
                        {
                            int length = 0x30;
                            code2 += "            send('    type : byte[]');\n"
                                + "            send('    value : ' + Memory.readByteArray(args[" + j.ToString() + "], " + length + "));\n"; // length에 대한 개선방안 고려
                        }
                        else
                        {
                            code2 += "            send('    type : pointer');\n"
                                + "            var tmp = args[" + j.ToString() + "];\n"
                                + "            var result = args[" + j.ToString() + "];\n"
                                + "            while(true){\n"
                                + "                try {\n"
                                + "                    var data = Memory.readPointer(tmp);\n"
                                + "                    result = result + '  -->  ' + data;\n"
                                + "                    tmp = data;\n"
                                + "                } catch (e) {\n"
                                + "                    break;\n"
                                + "                }\n"
                                + "            }\n"
                                + "            send('    data : ' + result);\n";
                        }
                    }
                    // Class view
                    code2 += "            send('------------------[this Object]--------------------');\n"
                        + "            save_this" + i.ToString() + " = args[0];\n"
                        + "            send('" + Fields[i][0] + "');\n"
                        + "            send('{');\n";

                    for (int j = 1; j < (Fields[i].Count - 1); j += 3)
                    {
                        if (Fields[i][j + 2] != "0x0")
                        {

                            code2 += "            send('      (" + Fields[i][j + 2] + ")   " + Fields[i][j] + "   " + Fields[i][j + 1] + "   =   '";
                            if (Fields[i][j].Contains("unsigned int"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readUInt());\n";
                            }
                            else if (Fields[i][j].Contains("int"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readInt());\n";
                            }
                            else if (Fields[i][j].Contains("string"))
                            {
                                if (platform == "arm64")
                                    code2 += " + ((new NativePointer(args[0]).add(" + Fields[i][j + 2] + ").readPointer().isNull()) ? 'null' : Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readPointer()) + 0x14))));\n";
                                else
                                    code2 += " + ((new NativePointer(args[0]).add(" + Fields[i][j + 2] + ").readPointer().isNull()) ? 'null' : Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readPointer()) + 0x0c))));\n";
                            }
                            else if (Fields[i][j].Contains("byte"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readU8());\n";
                            }
                            else if (Fields[i][j].Contains("float"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readFloat());\n";
                            }
                            else if (Fields[i][j].Contains("Double"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readDouble());\n";
                            }
                            else if (Fields[i][j].Contains("unsigned short"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readUShort());\n";
                            }
                            else if (Fields[i][j].Contains("short"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readShort());\n";
                            }
                            else if (Fields[i][j].Contains("bool"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(args[0]) + " + Fields[i][j + 2] + ")).readU8());\n";
                            }
                            else
                            {
                                code2 += " + 'undefined');\n";
                            }
                        }
                        else
                        {
                            code2 += "            send('      (0x0)   " + Fields[i][j] + "   " + Fields[i][j + 1] + "   =   undefined');\n";
                        }
                    }
                    code2 += "            send('}');\n";
                }

                code2 += "            send('------------------[Stack]--------------------');\n"
                    + "            dumpAddr('sp', this.context.sp, 0x100);\n"
                    + "            send('------------------[BackTrace]--------------------');\n"
                    + "            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\\\n') + ' ');\n"
                    + "            send('######');\n";


                /////////////////////////////////////////////////////////////////////////////////
                // onLeave()
                code2 += ""
                    + "        },\n"
                    + "        onLeave : function(retval){\n"
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
                code2 += ") :: onLeave()');\n"
                    + "            send('Offset : " + array[2] + "');\n"
                    + "            send('retval : ' + retval);\n"
                    + "            send('----------------[Registers]------------------');\n";

                code2 += "            send('[+] pc : ' + this.context.pc);\n"
                    + "            send('[+] sp : ' + this.context.sp);\n"
                    + "            var tmp1 = this.context.sp;\n"
                    + "            var result1 = tmp1;\n";

                if (platform == "arm64")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        code2 += "            tmp1 = this.context.x" + j.ToString() + ";\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] x" + j.ToString() + " : ' + result1);\n";
                    }
                }
                else if (platform == "armv7")
                {
                    for (int j = 0; j < 8; j++)
                    {
                        code2 += "            tmp1 = this.context.r" + j.ToString() + ";\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] r" + j.ToString() + " : ' + result1);\n";
                    }
                }
                else // platform == "x86"
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int tmp = j + 97;
                        code2 += "            tmp1 = this.context.e" + (char)tmp + "x;\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] e" + (char)tmp + "x : ' + result1);\n";
                    }
                    code2 += "            tmp1 = this.context.edi;\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] edi : ' + result1);\n";

                    code2 += "            tmp1 = this.context.esi;\n"
                            + "            result1 = tmp1;\n"
                            + "            while(true){\n"
                            + "                try {\n"
                            + "                    var data = Memory.readPointer(tmp1);\n"
                            + "                    result1 = result1 + '  -->  ' + data;\n"
                            + "                    tmp1 = data;\n"
                            + "                } catch (e) {\n"
                            + "                    break;\n"
                            + "                }\n"
                            + "            }\n"
                            + "            send('[-] esi : ' + result1);\n";
                }

                if (tmp1.Contains("static")) // this 없음
                {

                }
                else
                {   // no args in onLeave....
                    code2 += "            send('------------------[this Object]--------------------');\n"
                        + "            send('" + Fields[i][0] + "');\n"
                        + "            send('{');\n";

                    for (int j = 1; j < (Fields[i].Count - 1); j += 3)
                    {
                        if (Fields[i][j + 2] != "0x0")
                        {

                            code2 += "            send('      (" + Fields[i][j + 2] + ")   " + Fields[i][j] + "   " + Fields[i][j + 1] + "   =   '";
                            if (Fields[i][j].Contains("unsigned int"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readUInt());\n";
                            }
                            else if (Fields[i][j].Contains("int"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readInt());\n";
                            }
                            else if (Fields[i][j].Contains("string"))
                            {
                                if (platform == "arm64")
                                    code2 += " + ((new NativePointer(save_this" + i.ToString() + ").add(" + Fields[i][j + 2] + ").readPointer().isNull()) ? 'null' : Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readPointer()) + 0x14))));\n";
                                else
                                    code2 += " + ((new NativePointer(save_this" + i.ToString() + ").add(" + Fields[i][j + 2] + ").readPointer().isNull()) ? 'null' : Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readPointer()) + 0x0c))));\n";
                            }
                            else if (Fields[i][j].Contains("byte"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readU8());\n";
                            }
                            else if (Fields[i][j].Contains("float"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readFloat());\n";
                            }
                            else if (Fields[i][j].Contains("Double"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readDouble());\n";
                            }
                            else if (Fields[i][j].Contains("unsigned short"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readUShort());\n";
                            }
                            else if (Fields[i][j].Contains("short"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readShort());\n";
                            }
                            else if (Fields[i][j].Contains("bool"))
                            {
                                code2 += " + new NativePointer(ptr(parseInt(save_this" + i.ToString() + ") + " + Fields[i][j + 2] + ")).readU8());\n";
                            }
                            else
                            {
                                code2 += " + 'undefined');\n";
                            }
                        }
                        else
                        {
                            code2 += "            send('      (0x0)   " + Fields[i][j] + "   " + Fields[i][j + 1] + "   =   undefined');\n";
                        }
                    }
                    code2 += "            send('}');\n";
                }

                code2 += "            send('------------------[Stack]--------------------');\n"
                    + "            dumpAddr('sp', this.context.sp, 0x100);\n"
                    + "            send('------------------[BackTrace]--------------------');\n"
                    + "            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\\\n') + ' ');\n"
                    + "            send('######');\n"
                    + "        }\n    })\n";
                code += code2;
                i++;
            }


            code += "});\n";

            using (StreamWriter outFile = new StreamWriter(Application.StartupPath + @"\Modules\hook\hook.js"))
            {
                outFile.Write(code);
            }
        }

        private void GenerateHookCode()
        {
            String source = File.ReadAllText(Application.StartupPath + @"\Modules\hook\base.py");
            String jscode = File.ReadAllText(Application.StartupPath + @"\Modules\hook\hook.js");

            int jscodeIndex = source.IndexOf("'''");

            String Payload = source.Substring(0, jscodeIndex + 4);
            Payload += jscode;
            Payload += source.Substring(jscodeIndex + 4);

            using (StreamWriter outFile = new StreamWriter(Application.StartupPath + @"\Modules\hook\output.py"))
            {
                outFile.Write(Payload);
            }
        }

        private void RunHook()
        {
            ProcessStartInfo proInfo = new ProcessStartInfo();
            proInfo.FileName = "python.exe";
            proInfo.Arguments = String.Format(" {0} {1}", Application.StartupPath + @"\Modules\hook\output.py", package_name);
            proInfo.WorkingDirectory = Application.StartupPath + @"\Modules\hook\";
            proInfo.CreateNoWindow = true;
            proInfo.UseShellExecute = false;

            current_pro = new Process();
            current_pro.StartInfo = proInfo;
            current_pro.Start();
            current_pro.Exited += (sender, e) =>
            {
                MessageBox.Show("Hook Process exited!");
            };
        }

        private void AutoHook(String PackageName)
        {
            process_on = true;
            // GetClassFields
            GetClassFields();

            // Generate hook.js
            GenerateJSCode();

            // Source 코드를 베이스로 최종 후킹 코드 생성
            GenerateHookCode();

            // 후킹 코드 실행
            RunHook();
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
            shell_pro.StandardInput.Write("adb shell ps" + Environment.NewLine);
            shell_pro.StandardInput.Close();
            output = shell_pro.StandardOutput.ReadToEnd();
            shell_pro.WaitForExit();
            shell_pro.Close();

            if (output.Contains("frida"))
                return true;
            else
                return false;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                int index = listView1.SelectedIndices[0];
                listView2.Items.Clear();
                List<String> resultstr = result[index];
                foreach (String str in resultstr)
                {
                    listView2.Items.Add(str);
                }
            }
        }

        private void Hook_form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (process_on == true)
            {
                process_on = false;
                try { current_pro.Kill(); }
                catch { }
            }

            if (server_running == true)
            {
                server_running = false;
                srv.Close();
                socket_server.Abort();
            }
        }

        public String Splicing_h_method(String h_str)
        {
            int idx_str = 0;
            String temp_str = h_str;
            idx_str = temp_str.IndexOf(":");
            temp_str = temp_str.Substring(idx_str + 1).Trim();
            idx_str = temp_str.IndexOf("::");
            temp_str = temp_str.Substring(0, idx_str).Trim();
            return temp_str;
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

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@ Buttons

        private void button1_Click(object sender, EventArgs e) // Add
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                int index = listView1.SelectedIndices[0];
                List<String> t_list = new List<String>();
                String temp_str = Splicing_h_method(listView1.Items[index].SubItems[0].Text);
                t_list.Add(temp_str);
                listView3.Items.Add("(" + (index + 1).ToString() + ") " + temp_str);
                for (int i = 0; i < listView2.Items.Count; i++)
                {
                    t_list.Add(listView2.Items[i].SubItems[0].Text);
                }
                h_list.Add(t_list);
            }
            else
            {
                MessageBox.Show("Not selected", "Error");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e) // Remove
        {

            if (listView3.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("This item will be removed\rContinue?", "Remove", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    int index = listView3.FocusedItem.Index;
                    listView3.Items.RemoveAt(index);
                    h_list.RemoveAt(index);
                }
            }
            else
            {
                MessageBox.Show("Not selected", "Error");
            }
        }

        private void button4_Click(object sender, EventArgs e) // Exit button
        {
            if (process_on == true)
            {
                process_on = false;
                current_pro.Kill();
            }
            if (server_running == true)
            {
                server_running = false;
                srv.Close();
                socket_server.Abort();
            }
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e) // Run
        {
            if (!CheckFridaServer())
            {
                MessageBox.Show("Please Run Frida-Server on your device!");
                return;
            }

            if (!CheckProcess())
            {
                MessageBox.Show(this, package_name + " Not Found!", "Error");
                return;
            }

            if (process_on)
            {
                process_on = false;
                try { current_pro.Kill(); }
                catch { }
            }
            number = 1;
            listView1.Items.Clear();
            AutoHook(package_name);
            is_hook_ok = 1;
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

        #endregion
    }
}