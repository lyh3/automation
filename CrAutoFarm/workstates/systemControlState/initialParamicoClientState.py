#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho,
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.paramikoClient import ParamikoClient
import buildingblocks.utils as util


class InitialParamicoClientState(SystemControlState):
    @overrides(SystemControlState)
    def CallSystemFunction(self):
        try:
            self._checkTimeout()

            configClient = self._config['Client']
            ipAddress = configClient['ip'].lstrip()
            if ipAddress is '':
                ipAddress = util.GetLocalHostIP()
            client = ParamikoClient(ipAddress, configClient['user'], configClient['password'])
            client.Connect()
            self._parentWorkThread._client = client
            self._success = True
        except Exception as e:
            errorMsg = 'InitialParamikoClient exception caught, error = {}\nPlease resolve the issue and try again.'.format(str(e))
            self._logger.error(errorMsg)
