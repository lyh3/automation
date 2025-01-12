#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from powerSwitch import PowerSwitch
from time import sleep
import os
try:
	import dlipower
	print "Loaded DLI Power"
except ImportError:
	print "dlipower missing - calling setup.py"
	from buildingblocks.setup import setup as setup
	import dlipower

class DLiPowerSwitch(PowerSwitch):
    def __init__(self, config, logger):
        super(DLiPowerSwitch, self).__init__(config, logger)
        ip = config["power_switch_ip"]
        user = config["power_switch_user"]
        password = config["power_switch_password"]
        self._switch_access = {"hostname": ip,
                               "userid": user,
                               "password": password}
        self._logger = logger
        try:
            self._checkTimeout('10')
            response = os.system("ping -c 1 " + ip)
            powerSwitch = dlipower.PowerSwitch(hostname = ip,
                                               userid = user,
                                               password = password)
        except Exception as e:
            self._logger.error(str(e))

    @overrides(PowerSwitch)
    def power_on(self, port = None):
        success = False
        p = port
        if port is None:
            p = self._getPort()
        switch = self._getSwitch()
        if switch is not None:
            if not self.IsPortOn(p):
                switch.on(p)
            if not self.IsPortOn(p):
                self._logger("Failed to turn on port {}".format(p))
            else:
                success = True
        return success

    @overrides(PowerSwitch)
    def power_off(self, port = None):
        success = False
        p = port
        if port is None:
            p = self._getPort()
        switch = self._getSwitch()
        if switch is not None:
            if self.IsPortOn(p):
                switch.off(p)
            if self.IsPortOn(p):
                self._logger("Failed to turn off port {}".format(p))
            else:
                success = True

        return success

    def IsPortOn(self, port):
        switch = self._getSwitch()
        if switch is not None:
            #status = [x for x in str(self.switch).split('\n') if x != '' and x[0].isdigit()]
            #return status[port].split('\t')[2] == 'ON'
            return switch[port - 1].state == 'ON'
        else:
            return 'UNKNOWN'

    @overrides(PowerSwitch)
    def power_cycle(self, port=None):
        p = port
        if port is None:
            p = self._getPort()
        switch = self._getSwitch()
        if switch is not None:
            switch.off(p)
            sleep(5)
            switch.on(p)


    def _getSwitch(self):
        switch = dlipower.PowerSwitch(**self._switch_access)
        if not switch.verify():
            self._logger.info("Could not connect to the power switch.")
            return None
        return switch