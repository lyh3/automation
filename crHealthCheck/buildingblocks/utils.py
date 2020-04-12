#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho, henry.li@intel.com
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------

import string
import random
import time
import datetime, json
from subprocess import check_output
import os as os
import binascii
from enum import Enum

import imp
try:
    import Queue as queue
except:
    import queue as queue


try:
    if imp.find_module('Tkinter') and imp.find_module('ttk'):
        import Tkinter as tk
        import ttk as ttk
except Exception:pass


DefaultRecurringInterval = 1
DefaultTimeStampFormat = "%Y-%m-%d %H:%M:%S"

class InvalidArgumentException(Exception):
    def __init__(self, arg):
        super(InvalidArgumentException, self).__init__()
        self.msg = '"{}" is an invalid parameter.'.format(str(arg))

    def __str__(self):
        return self.msg


class Position(list):
    def __init__(self,x=0, y=0, z=0):
        super(Position, self).__init__((x,y,z))

    x = property(lambda self: self[0],
                 lambda self,value: self.__setitem__(0, value))
    y = property(lambda self: self[1],
                 lambda self,value: self.__setitem__(1, value))
    z = property(lambda self: self[2],
                 lambda self,value: self.__setitem__(2, value))


def IsTkinter():
    try:
        if imp.find_module('Tkinter') and imp.find_module('ttk'):
            return True
    except Exception:pass
    return False


def IdGenerator(size = 32, chars=string.ascii_uppercase + string.digits):
    return ''.join(random.choice(chars) for _ in range(size))


def LogMessage(msg, logFileName):
    if logFileName is not None:
        check_output("echo  {0} >> {1}".format(msg, logFileName), shell=True)
    else:
        print(msg)

def EscapedHexdecimalConverter(key):
    dic = {u'\\x0d':'\x0d',
           u'\\x1b': '\x1b',  #escape
           u'\\x0a':'\x0a',
           u'^[A': '\x1b[A',  #up arrow
           u'^[B': '\x1b[B',  #down arrow
           u'\\x20':'\x20',
           u'\\x7e':'\x7e',
           u'\\x40': '\x40',
           u'\\x1b2': '\x1b2',  #F2
           u'\\x1b0A': '\x1b0A',  #F10
           u'\\x1b\\x32':'\x1b\x32',
           u'\\x1b\\x01\\x00':'\x1b\x01\x00',
           u'\\x4E':'\x4E',
           u'\\x59':'\x59'}
    if key in dic.keys():
        return dic[key]
    return None

def Hex2BinConverter(hexString, delimiter = ''):
    if hexString is None or not len(hexString) > 0:
        return None
    if hexString.endswith('b') or hexString.endswith('B'):
        return hexString.replace('b', '').replace('B', '')
    hex2bin_map = {
        "0": "0000",
        "1": "0001",
        "2": "0010",
        "3": "0011",
        "4": "0100",
        "5": "0101",
        "6": "0110",
        "7": "0111",
        "8": "1000",
        "9": "1001",
        "a": "1010",
        "b": "1011",
        "c": "1100",
        "d": "1101",
        "e": "1110",
        "f": "1111",
    }
    return delimiter.join(hex2bin_map[i] for i in hexString.lower())


def TrimHesStringPreffix(hexstring):
    return hexstring.replace('0x', '').replace(' ', '')

def AssembleBytesInorder(source):
    results = source
    if len(source) == 2:
        results = '{1}{0}'.format(source[0], source[1])
    return results

def DecodeEventData(jsonpath, eventData, itemformat):
    results = None
    if jsonpath is not None and os.path.exists(jsonpath):
        decodingConfig = json.loads(open(jsonpath).read())
        results = DecodeFromJson(eventData, decodingConfig, itemformat=itemformat)
    return results

def DecodeFromJson(data, jsonpayload, itemformat):
    results = []
    dependencyDic = {}
    decodingConfig = jsonpayload
    response = Hex2BinConverter(data, ' ').split(' ')
    idxH = 0
    idxL = 2
    dependenceValue = None
    for data in decodingConfig["responsedata"]:
        buffer = response[idxH:idxL]
        ishex = data["hex"] == 'True'
        dependenceKey = data['dependence']
        values = None
        if not ishex:
            buffer = AssembleBytesInorder(response[idxH:idxL])
        for p in data["positions"]:
            position = p['position']
            if dependenceKey != '':
                dependenceValue = dependencyDic[dependenceKey]
                for px in p['values']:
                    if px['value'] == dependenceValue:
                        values = px['values']
                        break
            else:
                values = p['values']

            if values is None:
                continue

            for x in values:
                val = buffer[int(position[1], 0):int(position[3], 0) + 1]
                lookup = Hex2BinConverter(x['value'])
                match = False
                if ishex:
                    match = buffer[0] + buffer[1] == lookup
                else:
                    match = val == lookup
                if match:
                    v = x['value']
                    if dependenceValue is not None:
                        v = '{0} - {1}[{2}]'.format(dependenceValue, data['description'], v)
                    results.append(itemformat.format(v, x['description']))
                    if not data["description"] in dependencyDic:
                        dependencyDic[data["description"]] = v
        idxH += 2
        idxL += 2
    return results

def IpmiResponseConverter(line, limit = 0):
    if line is None or line == '':
        return None
    results = ''
    idx = 0
    lDx = 0
    hDx = 1
    a = line.split(' ')
    while hDx < len(a):
        if limit > 0 and idx >= limit:
            break
        results = results + str(int("0x{0}{1}".format(a[hDx], a[lDx]), 0)) + ','
        lDx += 2
        hDx += 2
        idx += 1
    return results[:len(results) - 1]

def Int2HexString(intVal, with0xprefix = True):
    if with0xprefix:
        return '0x{:02X}'.format(intVal)
    return '{:02X}',format(intVal)

def GetCurrentTimestamp(timestampformat = None):
    if timestampformat is None:
        timestampformat = DefaultTimeStampFormat
    timstamp = datetime.datetime.fromtimestamp(time.time())
    return timstamp.strftime(timestampformat)

def TryDeleteFile(filePath = None):
	try:
		if filePath is not None and os.path.exists(filePath):
			os.remove(filePath)
	except Exception as e:
		print(str(e))

def TryDeleteFolder(dir = None):
    try:
        if dir is not None and os.path.isdir(dir):
            os.rmdir(dir)
    except Exception as e:
        print(str(e))

def ShowSplashwindow(title, imagefile, delay, parent = None):
    if imp.find_module('Tkinter') and imp.find_module('ttk'):
        try:
            root = parent
            if parent is None:
                root = tk.Tk()
            root.title('{0} in progress ...'.format(title))
            root.overrideredirect(True)
            width = root.winfo_screenwidth()
            height = root.winfo_screenheight()
            progressbar = ttk.Progressbar(orient = tk.HORIZONTAL, length=width, mode='determinate')
            progressbar.pack()
            progressbar.start()
            root.geometry('%dx%d+%d+%d' % (width * 0.6, height * 0.6, width * 0.1, height * 0.1))
            image_file = imagefile
            image = tk.PhotoImage(file=image_file)
            canvas = tk.Canvas(root, height=height * 0.6, width=width * 0.6, bg="black")
            canvas.create_image(width * 0.6 / 2, height * 0.6 / 2, image=image)
            canvas.pack()
            root.after(delay, root.destroy)
            root.mainloop()
        except Exception as e:
            print(str(e))
            return None

def FormatRawbytes(manuID, split, params):
    str = TrimHesStringPreffix(split[1])
    if manuID is not None:
        str += TrimHesStringPreffix(manuID)
    if params is not None:
        str += TrimHesStringPreffix(params)
    return binascii.unhexlify(str)

def GetLocalHostIP():
    import socket, struct
    def _get_interface_ip(ifname):
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        return socket.inet_ntoa(_fcntl.ioctl(s.fileno(), 0x8915, struct.pack('256s', ifname[:15]))[20:24])
    def _fcntl():
        return 0
    ip = socket.gethostbyname(socket.gethostname())
    if ip.startswith("127.") and os.name != "nt":
        interfaces = ["eth0","eth1","eth2","wlan0","wlan1","wifi0","ath0","ath1","ppp0",]
        for ifname in interfaces:
            try:
                ip = _get_interface_ip(ifname)
                break
            except IOError:
                pass
    return ip

def CreateInstance(key, package, *args, **kwargs):
    classname = '{}_{}_State'.format(package.replace('State', ''), key)
    namespaces = ['workstates']
    namespaces.append(package)
    namespaces.append(classname)
    q = queue.Queue()
    instance = None
    try:
        m = ''  # "EDKAutomation."
        for x in namespaces:
            m += '{0}.'.format(x)
            q.put(x)

        module = __import__(m)
        q.get_nowait()
        instance = getattr(_extractAttr(module, q), classname)(*args, **kwargs)
    except Exception as e:
        print('Exception caught at CreateInstance, error was :%s' % str(e))
    return instance

def _extractAttr(module, q):
    if q is None or not q.qsize() > 0:
        return module
    n = q.get_nowait()
    m = getattr(module, n)
    return _extractAttr(m, q)
def test():
    b = Hex2BinConverter('a01301')
    c = AssembleBytesInorder(['1010', '0000'])
    id = IdGenerator()
    import msvcrt as ms
    while True:
        key = ord(ms.getwch())
        # case ESC
        if key == 27:
            break
        print(key)

if __name__ == '__main__':
    test()
