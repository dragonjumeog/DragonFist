function check(){
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
