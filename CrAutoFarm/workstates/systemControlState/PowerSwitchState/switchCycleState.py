#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho, henry.li@intel.com
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from powerSwitchState import PowerSwitchState

class SwitchCycleState(PowerSwitchState):
    @overrides(PowerSwitchState)
    def Switch(self):
        self._powerSwitch.power_cycle()