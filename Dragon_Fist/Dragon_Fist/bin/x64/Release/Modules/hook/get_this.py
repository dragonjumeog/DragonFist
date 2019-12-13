import sys, frida, socket

sc = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

def on_message(message, data):
    if message['type'] == 'send':
        try:
            payload = str(message['payload']) + '\n'
            sc.sendto(payload.encode(), ('127.0.0.1', 5572))
            print(str(message['payload']) + '\n')
        except:
            print('error');
    elif message['type'] == 'error':
        try:
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

    var offset0 = 0x416F1C;
    var target0 = il2cpp.add(offset0);
    send('private void Start() @ ' + target0.toString());
    Interceptor.attach(target0, {
        onEnter : function(args){
            send('======');
            send('[*] Class : public class Game_manager : MonoBehaviour ');
            send('[*] Function : private void Start() :: onEnter()');
            send('[*] Offset : 0x416F1C');
            send('this : ' + args[0]);
        },
        onLeave : function(retval){
        }
    })
});

'''

if __name__ == "__main__":
    print("[*] Start Process ...")
    PACKAGE_NAME = sys.argv[1]

    try:
        process = frida.get_usb_device().attach(PACKAGE_NAME)
        script = process.create_script(jscode)
        script.on('message', on_message)
        script.load()
        sys.stdin.read()

    except Exception as error:
        print(error)
