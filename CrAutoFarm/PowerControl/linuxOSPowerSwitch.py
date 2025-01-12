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
from PowerControl.powerSwitch import PowerSwitch
from buildingblocks.Decoraters import overrides
from buildingblocks.paramikoClient import ParamikoClient

class LinuxOSPowerSwitch(PowerSwitch):
    def __init__(self, config, logger):
        super(LinuxOSPowerSwitch, self).__init__(config, logger)
        try:
            self._checkTimeout('300')
            client = ParamikoClient(config['ip'].lstrip(), config['user'], config['password'])
            client.Connect()
            self._powerSwitch = client
        except Exception as e:
            errorMsg = 'InitialParamikoClient exception caught, error = {}\nPlease resolve the issue and try again.'.format(str(e))
            self._logger.error(errorMsg)

    @overrides(PowerSwitch)
    def power_on(self, switch_port=None):
        #raise NotImplementedError("user cannot call this function.")
        self._logger.info('Call {} power_on, do nothing.'.format(type(self).__name__))

    @overrides(PowerSwitch)
    def power_off(self, switch_port=None):
        #raise NotImplementedError("user cannot call this function.")
        self._logger.info('Call {} power_on, do nothing.'.format(type(self).__name__))

    @overrides(PowerSwitch)
    def power_cycle(self, switch_port=None):
        if self._powerSwitch is not None:
            self._powerSwitch.ExecuteCommand('reboot')