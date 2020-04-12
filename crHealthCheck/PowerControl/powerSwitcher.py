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
from dLiPowerSwitch import DLiPowerSwitch
from usbPowerSwitch import UsbPowerSwitch
from linuxOSPowerSwitch import LinuxOSPowerSwitch
from buildingblocks.AutomationLog import AutomationLog


class PowerSwitcher(object):
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
                        if powerSwitch is not None:
                            cls._powerSwitch = powerSwitch
                            break
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
        jsonpath = os.path.realpath(r'../Json/PERF_Test.json')
    config = json.loads(open(jsonpath).read())
    switcher = PowerSwitcher(config['PowerSwitcher'])
    #switcher.power_off()
    #switcher.power_on(3)
    pass
