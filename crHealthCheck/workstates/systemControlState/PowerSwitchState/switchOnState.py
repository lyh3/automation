#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from powerSwitchState import PowerSwitchState

class SwitchOnState(PowerSwitchState):
    @overrides(PowerSwitchState)
    def Switch(self):
        self._success = self._powerSwitch.power_on()
