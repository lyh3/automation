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
from buildingblocks.invokeITP import InvokeITP


class InitialITPState(SystemControlState):
    @overrides(SystemControlState)
    def CallSystemFunction(self):
        try:
            self._checkTimeout()
            from buildingblocks.AutomationLog import AutomationLog
            invokeITP = InvokeITP(self._logger)
            if invokeITP is not None and not invokeITP.IsRunning():
                self._success = True
        except Exception as e:
            errorMsg = 'InitialITPState exception caught, error = {}\nPlease resolve the issue and try again.'.format(
                str(e))
            self._logger.error(errorMsg)
            self._setGraceTermination(errorMsg)