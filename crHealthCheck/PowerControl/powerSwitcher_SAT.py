

from dLiPowerSwitch import DLiPowerSwitch
from usbPowerSwitch import UsbPowerSwitch
from linuxOSPowerSwitch import LinuxOSPowerSwitch
from buildingblocks.AutomationLog import AutomationLog



import paramiko
import subprocess
import os
import sys
import time
import ctypes
import json
import re
from lxml import etree
from log import Log

###########################################################
#simin copied from SAT_test_v0.5

import paramiko
import subprocess
import os
import sys
import time
import ctypes
import json
import re
from lxml import etree
from log import Log
IP = '10.54.0.235'
USERNAME = 'root'
PASSWORD = 'intel123'
'''
POWER_SWITCH_IP = '10.54.0.234'
POWER_SWITCH_USER = 'hct1'
POWER_SWITCH_PASSWORD = 'donotchange'
POWER_SWITCH_PORT = 1
'''
try:
    import dlipower
except ImportError:
    print "dlipower missing - calling setup.py"
    import setup as setup
    import dlipower

##################################################

log = Log()

class PowerSwitcher(object):
    def __init__(self, power_switch_ip, power_switch_user, power_switch_password):
        self.switch_access = {"hostname": power_switch_ip,
                              "userid": power_switch_user,
                              "password": power_switch_password}
        log.i('Connecting to a DLI PowerSwitch at {}'.format(power_switch_ip))
        self.switch = dlipower.PowerSwitch(hostname=power_switch_ip, userid=power_switch_user,
                                           password=power_switch_password)
        self.get_switch()
        log.i('Connected.')

    def get_switch(self):
        if not self.switch:
            self.switch = dlipower.PowerSwitch(**self.switch_access)
        if not self.switch.verify():
            log.i("Could not connect to the power switch.")
        return self.switch

    def power_on(self, power_switch_port=1):
        # TODO: Why is this a print() and not a log()
        log.i('Turning on outlet {}'.format(power_switch_port))
        self.get_switch().on(power_switch_port)
        if self.get_switch()[int(power_switch_port) - 1].state != "ON":
            log.i("Power outlet state is {}".format(self.get_switch()[power_switch_port].state))

    def power_off(self, power_switch_port=1):
        # TODO: Why is this a print() and not a log()
        log.i('Turning off outlet {}'.format(power_switch_port))
        self.get_switch().off(power_switch_port)
        if self.get_switch()[int(power_switch_port) - 1].state != "OFF":
            log.i("Power outlet state is {}".format(self.get_switch()[power_switch_port].state))



#######################################
#Simin comment out
'''
    _instancePowerSwitcher = None
    def __new__(cls, *args, **kwargs):
        if not cls._instancePowerSwitcher:
            cls._instancePowerSwitcher = super(PowerSwitcher, cls).__new__(cls)
            cls._powerSwitch = None
            if args.__len__() > 0:
                logName = 'PowerSwitcher'
                automationlog = AutomationLog(logName)
                logger = automationlog.GetLogger(logName)
                automationlog.TryAddConsole(logName)
                powerSwitch = None
                for key in args[0].keys():
                    try:
                        config = args[0][key]
                        if not config['active']:
                            continue
                        if key == 'DLIPowerSwitch':
                            powerSwitch = DLiPowerSwitch(config, logger)
                        elif key == 'usbPowerSwitch':
                            powerSwitch = UsbPowerSwitch(config, logger)
                        elif key == 'sshClient':
                            powerSwitch = LinuxOSPowerSwitch(config, logger)
                        if powerSwitch._powerSwitch is not None:
                            cls._powerSwitch = powerSwitch._powerSwitch
                    except Exception as e:
                        logger.error(str(e))
                        #raise Exception('Failed to initialize PowerSwitcher.\n')

        return cls._instancePowerSwitcher._powerSwitch

if __name__ == '__main__':
    import os,json
    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument('-j', action='store', dest='jsonfile', help="Config Json file path", default=None)
    args = parser.parse_args()
    if args.jsonfile is not None:
        jsonpath = args.jsonfile
    else:
        jsonpath = os.path.realpath(r'./Json/Henry.json')
    config = json.loads(open(jsonpath).read())
    switcher = PowerSwitcher(config['PowerSwitcher'])
    switcher.power_off()
    switcher.power_on()
    pass
'''

class CentOS(object):

    def __init__(self, ip, username, password):
        self._ip = ip
        self._username = username
        self._password = password
        self._ssh = None
        self._memory_resources = {}
        self._pools = []
        self._namespaces = {}
        self._mode = 'unknown'

    def connect(self):
        self._ssh = paramiko.SSHClient()
        self._ssh.set_missing_host_key_policy(paramiko.AutoAddPolicy())
        try:
            self._ssh.connect(self._ip, 22, self._username, self._password, timeout=None, auth_timeout=None)
            log.i("Accessed to server: {}".format(self._ip))
        except Exception, e:
            log.i("Failed to access server: {}".format(e))
            sys.exit(-1)

    def disconnect(self):
        try:
            self._ssh.close()
        finally:
            self._ssh = None
        log.i("Closed ssh access to server: {}".format(self._ip))

    def wait_for_ssh_connection(self, timeout=None):
        log.i('Waiting for ssh access')
        start_time = time.time()
        fnull = open(os.devnull, 'w')
        while 1:
            log.i('attempt to access server...')
            stdout, stderr = subprocess.Popen('ping -n 3 {}'.format(self._ip), stdout=subprocess.PIPE,
                                              stderr=subprocess.PIPE).communicate()
            if 'TTL' in stdout:
                break
            time.sleep(30)
            if timeout is not None and time.time() - start_time > timeout:
                log.i('waiting for ssh access timeout: {}'.format(timeout))
                sys.exit(-1)
        time.sleep(120)
        log.i('Ready for ssh connection.')
        return True

    def os_reboot(self):
        log.i('Start to reboot OS...')
        self.run_cmd('reboot')
        start_time = time.time()
        time.sleep(60)
        self.disconnect()
        self.wait_for_ssh_connection()
        time.sleep(120)
        self.connect()
        end_time = round(time.time() - start_time)
        log.i('Reboot completed, took {} seconds.'.format(end_time))

    def run_cmd(self, cmdline):
        if self._ssh is None:
            self.connect()
        try:
            stdin, stdout, stderr = self._ssh.exec_command(cmdline)
            output = stdout.read().rstrip('\n')
            errput = stderr.read().rstrip('\n')
            if re.search(r'failed|error', output, re.I):
                log.i('Failed exec cmd: {}.\n{}'.format(cmdline, output))
                sys.exit(-1)
            if re.search(r'failed|error', errput, re.I):
                log.i('Failed exec cmd: {}.\n{}'.format(cmdline, errput))
                sys.exit(-1)
            return output
        except paramiko.SSHException, e:
            log.i('run cmd: {} failed. Error: {}'.format(cmdline, e))
            sys.exit(-1)

    def query_pool(self):
        self._pools = []
        result = self.run_cmd('ipmctl show -a -o nvmxml -pool')
        root = etree.fromstring(result)
        for pool_tree in root.findall('Pool'):
            pool = {}
            for tag in ['PoolID', 'PersistentMemoryType', 'Capacity', 'FreeCapacity', 'SocketID']:
                pool.setdefault(tag, pool_tree.find(tag).text)
            self._pools.append(pool)
        return self._pools

    def verify_dimm(self):
        output = self.run_cmd('ipmctl show -dimm')
        for line in output.split('\n'):
            if line and 'DimmID' not in line:
                if 'Healthy' not in line:
                    log.i('Dimm not healthy!')
                    return False

        return True

    def create_goal(self, mode):
        if mode == 'AD':
            self.run_cmd('ipmctl create -f -goal persistentmemorytype=appdirect')
            log.i('Created AD goal')
        elif mode == 'MM':
            self.run_cmd('ipmctl create -f -goal memorymode=100')
            log.i('Created MM goal')
        else:
            log.i('Does not supporting create {} goals'.format(mode))
            sys.exit()

    def delete_goal(self):
        self.run_cmd('ipmctl delete -goal')
        log.i('Delete goals.')

    def get_region(self):
        regions = []
        result = self.run_cmd('ndctl list -R -u')
        region_list = json.loads(result)
        for region in region_list:
            regions.append(region['dev'])
        return regions

    def ndctl_create_namespace(self, regions=None):
        log.i('Start to create namespace...')
        regions = self.get_region()
        for region in regions:
            output = self.run_cmd('ndctl create-namespace -r {} -m fsdax -f --verbose'.format(region))
            namespace = json.loads(output)
            log.i('Created namespace: {}'.format(namespace['dev']))

    def ndctl_delete_namespace(self, namespaces=None):
        if namespaces is None:
            self.run_cmd('ndctl destroy-namespace all -f')
            log.i('Deleted all namespaces')
        else:
            for namespace in namespaces:
                self.run_cmd('ndctl destroy-namespace {} -f'.format(namespace))
                log.i('Deleted namespace: {}'.format(namespace))

    def ndctl_get_namespace(self):
        self._namespaces = {}
        ndctl_info = self.run_cmd('ndctl list --namespaces -u')
        namespace_json_data = '[' + ndctl_info.lstrip('[').rstrip(']') + ']'
        namespace_list = json.loads(namespace_json_data)
        for i in namespace_list:
            self._namespaces.update({i['dev']: i})
        return self._namespaces

    def get_current_mode(self):
        self.get_memory_resource()
        if self._memory_resources['AppDirectCapacity'] == '0 B':
            self._mode = 'MM'
        elif self._memory_resources['MemoryCapacity'] == '0 B':
            self._mode = 'AD'
        return self._mode

    def get_memory_resource(self):
        self._memory_resources.clear()
        result = self.run_cmd('ipmctl show -o nvmxml -memoryresources')
        memory_resources_tree = etree.fromstring(result)
        for tag in ['Capacity', 'MemoryCapacity', 'AppDirectCapacity', 'UnconfiguredCapacity']:
            self._memory_resources.setdefault(tag, memory_resources_tree.find(tag).text.strip())
        return self._memory_resources

    def switch_mode(self, mode_to):
        log.i('Start to change to {} mode'.format(mode_to))
        if self.ndctl_get_namespace():
            self.ndctl_delete_namespace()
        if mode_to == 'AD':
            self.create_goal('AD')
            self.os_reboot()
            self.ndctl_create_namespace()
        elif mode_to == 'MM':
            self.create_goal('MM')
            self.os_reboot()
        else:
            log.i('Unsupported mode: {}'.format(mode_to))
            sys.exit()
        log.i('Changed to {} mode successfully'.format(mode_to))

    def verify_file_md5_on_aep(self, namespaces=None, init_fs=False):
        if not namespaces:
            namespaces = self.ndctl_get_namespace()
        if init_fs:
            self.run_cmd('dd if=/dev/zero of=/1M_file bs=1M count=1')
            for namespace in namespaces.values():
                pmem = namespace['blockdev']
                self.run_cmd('umount -f /dev/{}'.format(pmem))
                self.run_cmd('rm -rf /mnt/{}'.format(pmem))
                self.run_cmd('mkfs.xfs -f /dev/{}'.format(pmem))
                self.run_cmd('mkdir -p /mnt/{}'.format(pmem))
                self.run_cmd('mount -o dax /dev/{} /mnt/{}'.format(pmem, pmem))
                self.run_cmd('for i in {{1..10}};do cp -f /1M_file /mnt/{}/1M_file_$i;done'.format(pmem))
                log.i('Inited files on aep pmem device: {}'.format(pmem))
        md5 = self.run_cmd('md5sum /1M_file | awk \'{print $1}\'')
        log.i('md5 code is: {}'.format(md5))
        for namespace in namespaces.values():
            pmem = namespace['blockdev']
            self.run_cmd('umount -f /dev/{}'.format(pmem))
            self.run_cmd('mount -o dax /dev/{} /mnt/{}'.format(pmem, pmem))
            output = self.run_cmd('md5sum /mnt/{}/1M_file* | awk \'{{print $1}}\''.format(pmem))
            for line in output.split('\n'):
                if line != md5:
                    return False
        return True


###flag####

SKIP_INIT_AC = False

### test functions ###


def warm_reboot_test(sut, loops):
    log.i('warm reboot test start...')
    start_time = time.time()
    i = 1
    if not SKIP_INIT_AC:
        log.i('Doing a init AC cycle')
        power_splitter.power_off()
        power_splitter.power_on()
    sut.wait_for_ssh_connection()
    sut.connect()
    namespaces = sut.ndctl_get_namespace()
    if len(namespaces) == 0:
        sut.switch_mode('AD')
        namespaces = sut.ndctl_get_namespace()
    sut.verify_file_md5_on_aep(namespaces, init_fs=True)
    while True:
        log.i('warm reboot test loop: {}'.format(i))
        sut.os_reboot()
        sut.connect()
        namespaces_current = sut.ndctl_get_namespace()
        if cmp(namespaces, namespaces_current) != 0:
            log.i('Namespace info changed after reboot')
            sys.exit()
        if not sut.verify_file_md5_on_aep(namespaces_current):
            log.i('File md5 changed after reboot')
            sys.exit()
        i += 1
        if i > loops:
            break
    elapsed_time = round(time.time() - start_time, 1)
    log.i('warm reboot test finished, took {}s!'.format(elapsed_time))


def ac_cycle_test(sut, loops):
    log.i('AC cycle test start...')
    start_time = time.time()
    i = 1
    if not SKIP_INIT_AC:
        power_splitter.power_off()
        power_splitter.power_on()
    sut.wait_for_ssh_connection()
    sut.connect()
    namespaces = sut.ndctl_get_namespace()
    if len(namespaces) == 0:
        sut.switch_mode('AD')
        namespaces = sut.ndctl_get_namespace()
    log.i(namespaces)
    sut.verify_file_md5_on_aep(namespaces, init_fs=True)
    while True:
        log.i('AC cycle test loop: {}'.format(i))
        power_splitter.power_off()
        power_splitter.power_on()
        sut.wait_for_ssh_connection()
        sut.connect()
        namespaces_current = sut.ndctl_get_namespace()
        log.i('loop: {} {}'.format(i, namespaces_current))
        if cmp(namespaces, namespaces_current) != 0:
            log.i('Namespace info changed after AC cycle')
            sys.exit()
        if not sut.verify_file_md5_on_aep(namespaces_current):
            log.i('File md5 changed after AC cycle')
            sys.exit()
        i += 1
        if i > loops:
            break
    elapsed_time = round(time.time() - start_time, 1)
    log.i('AC cycle test finished, took {}s!'.format(elapsed_time))


def provision_test(sut, mode, loops):
    log.i('{} provision test start...'.format(mode))
    if not SKIP_INIT_AC:
        power_splitter.power_off()
        power_splitter.power_on()
    i = 1
    while True:
        log.i('{} provision test loop: {}'.format(mode, i))
        sut.wait_for_ssh_connection()
        sut.connect()
        sut.switch_mode(mode)
        log.i('Current memory resources info:{}'.format(sut.get_memory_resource()))
        i += 1
        if loops < i:
            break
    log.i('{} provision test finished.'.format(mode))


if __name__ == '__main__':
    sut = CentOS(IP, USERNAME, PASSWORD)
    jsonpath = os.path.realpath(r'../Json/Henry.json')
    config = json.loads(open(jsonpath).read())['PowerSwitcher']['DLIPowerSwitch']
    POWER_SWITCH_IP = config['power_switch_ip']
    POWER_SWITCH_USER = config['power_switch_user']
    POWER_SWITCH_PASSWORD = config['power_switch_password']
    power_splitter = PowerSwitcher(POWER_SWITCH_IP, POWER_SWITCH_USER, POWER_SWITCH_PASSWORD)
    ac_cycle_test(sut, 1)
# provision_test(sut, 'AD', 50)
# provision_test(sut, 'MM', 50)
# warm_reboot_test(sut, 50)