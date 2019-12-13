import sys, frida, socket

socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
socket.connect(('127.0.0.1', 5571))

def on_message(message, data):
    if message['type'] == 'send':
        try:
            payload = str(message['payload']) + '\n'
            socket.send(payload.encode())
            #f = open('Result\\result.txt', 'a')
            #f.write(str(message['payload']) + '\n')
            #f.close()
            print(str(message['payload']) + '\n')
        except:
            print('error');
    elif message['type'] == 'error':
        try:
            #f = open('Result\\result.txt', 'a')
            #f.write(str(message['stack']) + '\n')
            #f.close()
            socket.send(str(message['error']).encode())
            print(str(message['stack']) + '\n')
        except:
            print('error');
    else:
        print("something...")

jscode = '''function check(){
    send(' - Process id : ' + Process.id);
    send(' - Process arch : ' + Process.arch);
    send(' - isDebuggerAttached : ' + Process.isDebuggerAttached());
}

function dumpAddr(info, addr, size) {
    if (addr.isNull())
        return;
    send('Data dump ' + info + ' :');
    var buf = Memory.readByteArray(addr, size);
    send(hexdump(buf, { offset: 0, length: size, header: true, ansi: false }));
}

Java.perform(function(){
    send('Hooking Start ...');
    check();

    var il2cpp = Module.getBaseAddress('libil2cpp.so');
    send('[*] libil2cpp.so @ ' + il2cpp.toString());

    var offset0 = 0xAA515C;
    var target0 = il2cpp.add(offset0);
    send('	public int get_cnt() @ ' + target0.toString());
    Interceptor.attach(target0, {
        onEnter : function(args){
            console.log('======');
            send('======');
            send('[*] Class : public class gameDB.PlayerData ');
            send('[*] Function : 	public int get_cnt() :: onEnter()');
            send('[*] Offset : 0xAA515C');
            send('----------------[Registers]------------------');
            send('[+] pc : ' + this.context.pc);
            send('[+] sp : ' + this.context.sp);
            var tmp1 = this.context.sp;
            var result1 = tmp1;
            tmp1 = this.context.x0;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x0 : ' + result1);
            tmp1 = this.context.x1;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x1 : ' + result1);
            tmp1 = this.context.x2;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x2 : ' + result1);
            tmp1 = this.context.x3;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x3 : ' + result1);
            tmp1 = this.context.x4;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x4 : ' + result1);
            tmp1 = this.context.x5;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x5 : ' + result1);
            tmp1 = this.context.x6;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x6 : ' + result1);
            tmp1 = this.context.x7;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x7 : ' + result1);
            send('----------------[Arguments]------------------');
            send('args[0] : ' + args[0]);
            send('    type : pointer');
            var tmp = args[0];
            var result = args[0];
            while(true){
                try {
                    var data = Memory.readPointer(tmp);
                    result = result + '  -->  ' + data;
                    tmp = data;
                } catch (e) {
                    break;
                }
            }
            send('    data : ' + result);
            send('------------------[this Object]--------------------');
            send('public class gameDB.PlayerData // TypeDefIndex: 3541');
            send('{');
            send('      (0x8)   string   file_path   =   ' + Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(args[0]) + 0x8)).readPointer()) + 0x14)));
            send('      (0xC)   string   playerName   =   ' + Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(args[0]) + 0xC)).readPointer()) + 0x14)));
            send('      (0x10)   int   kill_score   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x10)).readInt());
            send('      (0x14)   int   dead_score   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x14)).readInt());
            send('      (0x18)   int   gold   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x18)).readInt());
            send('      (0x1C)   int   isUsing   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x1C)).readInt());
            send('      (0x20)   int   Result_Of_Match   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x20)).readInt());
            send('      (0x24)   int   start_sign   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x24)).readInt());
            send('      (0x28)   int   end_sign   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x28)).readInt());
            send('      (0x2C)   int   role   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x2C)).readInt());
            send('      (0x30)   int   is_selected   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x30)).readInt());
            send('      (0x34)   int   select   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x34)).readInt());
            send('      (0x38)   int   coin_cnt   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x38)).readInt());
            send('}');
            send('------------------[Stack]--------------------');
            dumpAddr('sp', this.context.sp, 0x100);
            send('------------------[BackTrace]--------------------');
            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\n') + ' ');
            send('######');
        },
        onLeave : function(retval){
            send('======');
            send('[*] Class : public class gameDB.PlayerData ');
            send('[*] Function : 	public int get_cnt() :: onLeave()');
            send('Offset : 0xAA515C');
            send('retval : ' + retval);
            send('----------------[Registers]------------------');
            send('[+] pc : ' + this.context.pc);
            send('[+] sp : ' + this.context.sp);
            var tmp1 = this.context.sp;
            var result1 = tmp1;
            tmp1 = this.context.x0;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x0 : ' + result1);
            tmp1 = this.context.x1;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x1 : ' + result1);
            tmp1 = this.context.x2;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x2 : ' + result1);
            tmp1 = this.context.x3;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x3 : ' + result1);
            tmp1 = this.context.x4;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x4 : ' + result1);
            tmp1 = this.context.x5;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x5 : ' + result1);
            tmp1 = this.context.x6;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x6 : ' + result1);
            tmp1 = this.context.x7;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x7 : ' + result1);
            send('------------------[Stack]--------------------');
            dumpAddr('sp', this.context.sp, 0x100);
            send('------------------[BackTrace]--------------------');
            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\n') + ' ');
            send('######');
        }
    })
    var offset1 = 0xAA9FE0;
    var target1 = il2cpp.add(offset1);
    send('	private void gold_spawn() @ ' + target1.toString());
    Interceptor.attach(target1, {
        onEnter : function(args){
            console.log('======');
            send('======');
            send('[*] Class : public class gold_manager : MonoBehaviour ');
            send('[*] Function : 	private void gold_spawn() :: onEnter()');
            send('[*] Offset : 0xAA9FE0');
            send('----------------[Registers]------------------');
            send('[+] pc : ' + this.context.pc);
            send('[+] sp : ' + this.context.sp);
            var tmp1 = this.context.sp;
            var result1 = tmp1;
            tmp1 = this.context.x0;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x0 : ' + result1);
            tmp1 = this.context.x1;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x1 : ' + result1);
            tmp1 = this.context.x2;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x2 : ' + result1);
            tmp1 = this.context.x3;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x3 : ' + result1);
            tmp1 = this.context.x4;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x4 : ' + result1);
            tmp1 = this.context.x5;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x5 : ' + result1);
            tmp1 = this.context.x6;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x6 : ' + result1);
            tmp1 = this.context.x7;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x7 : ' + result1);
            send('----------------[Arguments]------------------');
            send('args[0] : ' + args[0]);
            send('    type : pointer');
            var tmp = args[0];
            var result = args[0];
            while(true){
                try {
                    var data = Memory.readPointer(tmp);
                    result = result + '  -->  ' + data;
                    tmp = data;
                } catch (e) {
                    break;
                }
            }
            send('    data : ' + result);
            send('------------------[this Object]--------------------');
            send('public class gold_manager : MonoBehaviour // TypeDefIndex: 3543');
            send('{');
            send('      (0xC)   Text   gold   =   ' + 'undefined');
            send('      (0x10)   GameObject   item   =   ' + 'undefined');
            send('}');
            send('------------------[Stack]--------------------');
            dumpAddr('sp', this.context.sp, 0x100);
            send('------------------[BackTrace]--------------------');
            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\n') + ' ');
            send('######');
        },
        onLeave : function(retval){
            send('======');
            send('[*] Class : public class gold_manager : MonoBehaviour ');
            send('[*] Function : 	private void gold_spawn() :: onLeave()');
            send('Offset : 0xAA9FE0');
            send('retval : ' + retval);
            send('----------------[Registers]------------------');
            send('[+] pc : ' + this.context.pc);
            send('[+] sp : ' + this.context.sp);
            var tmp1 = this.context.sp;
            var result1 = tmp1;
            tmp1 = this.context.x0;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x0 : ' + result1);
            tmp1 = this.context.x1;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x1 : ' + result1);
            tmp1 = this.context.x2;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x2 : ' + result1);
            tmp1 = this.context.x3;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x3 : ' + result1);
            tmp1 = this.context.x4;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x4 : ' + result1);
            tmp1 = this.context.x5;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x5 : ' + result1);
            tmp1 = this.context.x6;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x6 : ' + result1);
            tmp1 = this.context.x7;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x7 : ' + result1);
            send('------------------[Stack]--------------------');
            dumpAddr('sp', this.context.sp, 0x100);
            send('------------------[BackTrace]--------------------');
            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\n') + ' ');
            send('######');
        }
    })
    var offset2 = 0xAA8F40;
    var target2 = il2cpp.add(offset2);
    send('	public void plus_gold() @ ' + target2.toString());
    Interceptor.attach(target2, {
        onEnter : function(args){
            console.log('======');
            send('======');
            send('[*] Class : public class gameDB.PlayerData ');
            send('[*] Function : 	public void plus_gold() :: onEnter()');
            send('[*] Offset : 0xAA8F40');
            send('----------------[Registers]------------------');
            send('[+] pc : ' + this.context.pc);
            send('[+] sp : ' + this.context.sp);
            var tmp1 = this.context.sp;
            var result1 = tmp1;
            tmp1 = this.context.x0;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x0 : ' + result1);
            tmp1 = this.context.x1;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x1 : ' + result1);
            tmp1 = this.context.x2;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x2 : ' + result1);
            tmp1 = this.context.x3;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x3 : ' + result1);
            tmp1 = this.context.x4;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x4 : ' + result1);
            tmp1 = this.context.x5;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x5 : ' + result1);
            tmp1 = this.context.x6;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x6 : ' + result1);
            tmp1 = this.context.x7;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x7 : ' + result1);
            send('----------------[Arguments]------------------');
            send('args[0] : ' + args[0]);
            send('    type : pointer');
            var tmp = args[0];
            var result = args[0];
            while(true){
                try {
                    var data = Memory.readPointer(tmp);
                    result = result + '  -->  ' + data;
                    tmp = data;
                } catch (e) {
                    break;
                }
            }
            send('    data : ' + result);
            send('------------------[this Object]--------------------');
            send('public class gameDB.PlayerData // TypeDefIndex: 3541');
            send('{');
            send('      (0x8)   string   file_path   =   ' + Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(args[0]) + 0x8)).readPointer()) + 0x14)));
            send('      (0xC)   string   playerName   =   ' + Memory.readUtf16String(ptr(parseInt(new NativePointer(ptr(parseInt(args[0]) + 0xC)).readPointer()) + 0x14)));
            send('      (0x10)   int   kill_score   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x10)).readInt());
            send('      (0x14)   int   dead_score   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x14)).readInt());
            send('      (0x18)   int   gold   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x18)).readInt());
            send('      (0x1C)   int   isUsing   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x1C)).readInt());
            send('      (0x20)   int   Result_Of_Match   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x20)).readInt());
            send('      (0x24)   int   start_sign   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x24)).readInt());
            send('      (0x28)   int   end_sign   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x28)).readInt());
            send('      (0x2C)   int   role   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x2C)).readInt());
            send('      (0x30)   int   is_selected   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x30)).readInt());
            send('      (0x34)   int   select   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x34)).readInt());
            send('      (0x38)   int   coin_cnt   =   ' + new NativePointer(ptr(parseInt(args[0]) + 0x38)).readInt());
            send('}');
            send('------------------[Stack]--------------------');
            dumpAddr('sp', this.context.sp, 0x100);
            send('------------------[BackTrace]--------------------');
            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\n') + ' ');
            send('######');
        },
        onLeave : function(retval){
            send('======');
            send('[*] Class : public class gameDB.PlayerData ');
            send('[*] Function : 	public void plus_gold() :: onLeave()');
            send('Offset : 0xAA8F40');
            send('retval : ' + retval);
            send('----------------[Registers]------------------');
            send('[+] pc : ' + this.context.pc);
            send('[+] sp : ' + this.context.sp);
            var tmp1 = this.context.sp;
            var result1 = tmp1;
            tmp1 = this.context.x0;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x0 : ' + result1);
            tmp1 = this.context.x1;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x1 : ' + result1);
            tmp1 = this.context.x2;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x2 : ' + result1);
            tmp1 = this.context.x3;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x3 : ' + result1);
            tmp1 = this.context.x4;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x4 : ' + result1);
            tmp1 = this.context.x5;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x5 : ' + result1);
            tmp1 = this.context.x6;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x6 : ' + result1);
            tmp1 = this.context.x7;
            result1 = tmp1;
            while(true){
                try {
                    var data = Memory.readPointer(tmp1);
                    result1 = result1 + '  -->  ' + data;
                    tmp1 = data;
                } catch (e) {
                    break;
                }
            }
            send('[-] x7 : ' + result1);
            send('------------------[Stack]--------------------');
            dumpAddr('sp', this.context.sp, 0x100);
            send('------------------[BackTrace]--------------------');
            send(Thread.backtrace(this.context, Backtracer.ACCURATE).map(DebugSymbol.fromAddress).join('\\n') + ' ');
            send('######');
        }
    })
});

'''

if __name__ == "__main__":
    print("[*] Start Process ...")
    PACKAGE_NAME = sys.argv[1]


    #f = open('Result\\result.txt', 'a')
    try:
        process = frida.get_usb_device().attach(PACKAGE_NAME)
        script = process.create_script(jscode)
        script.on('message', on_message)
        script.load()
        sys.stdin.read()
        # device = frida.get_usb_device(timeout=10)
        # print(device)
        # f.write(str(device) + '\n')
        # pid = device.spawn([PACKAGE_NAME])
        # print("[PID] : {}".format(pid))
        # f.write("[PID] : {}\n".format(pid))
        # f.close()
        # process = device.attach(pid)
        # device.resume(pid)
        # script = process.create_script(jscode)
        # script.on("message", on_message)
        # script.load()
        # sys.stdin.read()
    except Exception as error:
        print(error)
