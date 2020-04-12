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
from buildingblocks.Decoraters import overrides
from workstates.systemControlState.PowerSwitchState.powerSwitchState import PowerSwitchState

class SwitchOffState(PowerSwitchState):
    @overrides(PowerSwitchState)
    def Switch(self):
        self._success = self._powerSwitch.power_off()
