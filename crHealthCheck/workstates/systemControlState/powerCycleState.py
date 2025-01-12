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
from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.Decoraters import callCount
import os

class PowerCycleState(SystemControlState):
    @overrides(SystemControlState)
    def CallSystemFunction(self):
        if self._powerCycleCount.count > self.ParentWorkThread.PowerCycleRequestCount:
            self._setGraceTermination('System power cycle count {0} is more than the count {1} of request in the workflow. '
                                      'Please verify if to increase the timeout is necessary.'
                                      .format(self._powerCycleCount.count, self.ParentWorkThread.PowerCycleRequestCount))
            return

        self._powerCycleCount()
        self._client.ExecuteCommand('reboot')
        ip = self._config['Client']['ip']
        while not self._checkTimeout():
            ret = os.system("ping " + ip)
            if ret == 0:
                self._success = True
                break
        pass

    @callCount
    def _powerCycleCount(self):
        pass