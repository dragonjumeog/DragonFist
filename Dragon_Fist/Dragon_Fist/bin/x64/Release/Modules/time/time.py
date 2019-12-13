import sys, frida, socket

sc = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

def on_message(message, data):
    if message['type'] == 'send':
        try:
            payload = str(message['payload']) + '\n'
            sc.sendto(payload.encode(), ('127.0.0.1', 5585))
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


jscode = """
var ratio = CHANGE_VALUE1; // 0.5, 1.5, 2.0, 3.0
var architecture = CHANGE_VALUE2; // 32bit 64bit

var max_clockgettime = 1000000000;
var max_gettimeofday = 1000000;

var clock_gettimeptr = Module.findExportByName("libc.so", "clock_gettime");
var gettimeofdayptr = Module.findExportByName("libc.so", "gettimeofday");

var clock_buffer; // temp
var clock_view; // temp
var clock_init_flag = new Array(0,0,0,0,0,0,0,0); // split to array depend on this.clock_type (type = 0x0 ~ 0x7)
var clock_game_sec = new Array(8); 
var clock_game_microsec = new Array(8);
var clock_old_sec = new Array(8);
var clock_old_microsec = new Array(8);
var clock_new_sec = new Array(8);
var clock_new_microsec = new Array(8);

send('[*] Running Time Hook...');
send(' -  clock_gettime() @ ' + clock_gettimeptr);
send(' -  gettimeofday() @ ' + gettimeofdayptr);

var clock_flag = false;
var time_flag = false;

Interceptor.attach(clock_gettimeptr, {
    onEnter : function(args){
        this.clock_type = parseInt(args[0]);
        this.struct_timespec = args[1];
        //for checking clock_type
        //console.log("clock_type : " + this.clock_type);
        if (!clock_flag){
            send('[+] clock_gettime() Called!');
            clock_flag = true;
        }
    },
    onLeave : function(retval){
        clock_buffer = Memory.readByteArray(this.struct_timespec, architecture*2);
        clock_view = new DataView(clock_buffer);
        if(clock_init_flag[this.clock_type] == 0){
            clock_game_sec[this.clock_type] = clock_view.getUint32(0, true);
            clock_game_microsec[this.clock_type] = clock_view.getUint32(architecture, true);
            clock_old_sec[this.clock_type] = clock_game_sec[this.clock_type];
            clock_old_microsec[this.clock_type] = clock_game_microsec[this.clock_type];
            clock_init_flag[this.clock_type] = 1;
        }else{
            clock_new_sec[this.clock_type] = clock_view.getUint32(0, true);
            clock_new_microsec[this.clock_type] = clock_view.getUint32(architecture, true);

            if(clock_new_sec[this.clock_type] == clock_old_sec[this.clock_type]){
                clock_game_microsec[this.clock_type] += (clock_new_microsec[this.clock_type] - clock_old_microsec[this.clock_type])*ratio;
            }else{
                clock_game_microsec[this.clock_type] += (clock_new_microsec[this.clock_type] + max_clockgettime - clock_old_microsec[this.clock_type])*ratio;
            }

            clock_old_sec[this.clock_type] = clock_new_sec[this.clock_type];
            clock_old_microsec[this.clock_type] = clock_new_microsec[this.clock_type];

            if(clock_game_microsec[this.clock_type] >= max_clockgettime){
                clock_game_sec[this.clock_type] += 1;
                clock_game_microsec[this.clock_type] -= max_clockgettime;
            } 
        }
        
        clock_view.setUint32(0, clock_game_sec[this.clock_type], true);
        clock_view.setUint32(architecture, clock_game_microsec[this.clock_type], true);
        Memory.writeByteArray(this.struct_timespec, clock_buffer);

        //logging
        //console.log("clock_game_time : " + clock_game_sec[this.clock_type] + " , " + clock_game_microsec[this.clock_type]);
    }
});

var gettime_buffer; // temp
var gettime_view; // temp
var gettime_init_flag = 0;
var gettime_game_sec = 0; 
var gettime_game_microsec = 0;
var gettime_old_sec = 0;
var gettime_old_microsec = 0;
var gettime_new_sec = 0;
var gettime_new_microsec = 0;

Interceptor.attach(gettimeofdayptr, {
    onEnter : function(args){
        this.tv = args[0];
        if (!time_flag){
            send('[+] gettimeofday() Called!');
            time_flag = true;
        }
    },
    onLeave : function(retval){
        gettime_buffer = Memory.readByteArray(this.tv, architecture*2);
        gettime_view = new DataView(gettime_buffer);
        if(gettime_init_flag == 0){
            gettime_game_sec = gettime_view.getUint32(0, true);
            gettime_game_microsec = gettime_view.getUint32(architecture, true);
            gettime_old_sec = gettime_game_sec;
            gettime_old_microsec = gettime_game_microsec;
            gettime_init_flag = 1;
        }else{
            gettime_new_sec = gettime_view.getUint32(0, true);
            gettime_new_microsec = gettime_view.getUint32(architecture, true);

            if(gettime_new_sec == gettime_old_sec){
                gettime_game_microsec += (gettime_new_microsec - gettime_old_microsec)*ratio;
            }else{
                gettime_game_microsec += (gettime_new_microsec + max_gettimeofday - gettime_old_microsec)*ratio;
            }

            gettime_old_sec = gettime_new_sec;
            gettime_old_microsec = gettime_new_microsec;

            if(gettime_game_microsec >= max_gettimeofday){
                gettime_game_sec += 1;
                gettime_game_microsec -= max_gettimeofday;
            } 
        }
        
        gettime_view.setUint32(0, gettime_game_sec, true);
        gettime_view.setUint32(architecture, gettime_game_microsec, true);
        Memory.writeByteArray(this.tv, gettime_buffer);

        //logging
        //console.log("gettime_game_time : " + gettime_game_sec + " , " + gettime_game_microsec);
    }
});

"""


if __name__ == "__main__":
    print("[*] Start Process ...")
    print("[Usage] time.py <Package Name> <Time Ratio> <Architecture>")
    PACKAGE_NAME = sys.argv[1]
    RATIO = sys.argv[2]
    ARCH = sys.argv[3]

    jscode = jscode.replace("CHANGE_VALUE1", RATIO)
    jscode = jscode.replace("CHANGE_VALUE2", ARCH)

    try:
        process = frida.get_usb_device().attach(PACKAGE_NAME)
        script = process.create_script(jscode)
        script.on('message', on_message)
        script.load()
        sys.stdin.read()

    except Exception as error:
        print(error)
