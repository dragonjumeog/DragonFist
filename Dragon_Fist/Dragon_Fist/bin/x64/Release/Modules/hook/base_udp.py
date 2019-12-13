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

jscode = '''
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
