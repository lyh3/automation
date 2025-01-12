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
from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.Decoraters import overrides
import os

class PingSUTState(SystemControlState):
    @overrides(SystemControlState)
    def CallSystemFunction(self):
        try:
            if os.system('ping -c 1 {0}'.format(self._config['Client']['ip'])) is 0:
                self._success = True

        except Exception as e:
            self._logger.error(str(e))
