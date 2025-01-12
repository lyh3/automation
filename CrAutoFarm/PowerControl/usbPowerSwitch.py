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
import os,ctypes
import os.path as path
from buildingblocks.Decoraters import overrides
from PowerControl.powerSwitch import PowerSwitch
from time import sleep

class UsbPowerSwitch(PowerSwitch):
    def __init__(self, config, logger):
        super(UsbPowerSwitch, self).__init__(config, logger)
        try:
            splitter_path = config["power_splitter_path"]
            if not path.isfile(splitter_path):
                raise ResourceWarning('ERROR: could not find power splitter executable on host. Please fix path / install.')
            self._logger.info("Setting up the power splitter with path {}".format(splitter_path))
            libs = ctypes.WinDLL(str(os.path.realpath(splitter_path)))
            self._splitter_dll = libs['SRMLineOpenByIndex']
            self._logger.info("USB power splitter set.")
            self._powerSwitch = self
        except Exception as e:
            self._logger.error(str(e))

    @overrides(PowerSwitch)
    def power_on(self, switch_port):
        port = self._getPort()
        if switch_port is not None:
            port = switch_port
        self._logger("Turning on port {} of the usb power splitter".format(switch_port))
        if self._splitter_dll is not None:
            self._splitter_dll(0, int(port), 1)

    @overrides(PowerSwitch)
    def power_off(self, switch_port):
        port = self._getPort()
        if switch_port is not None:
            port = switch_port
        self._logger("Shutting down port {} of the usb power splitter".format(switch_port))
        if self._splitter_dll is not None:
            self._splitter_dll(0, int(port), 0)

    @overrides(PowerSwitch)
    def power_cycle(self, switch_port):
        port = self._getPort()
        if switch_port is not None:
            port = switch_port
        self.power_off(port)
        sleep(10)
        self.power_on(port)