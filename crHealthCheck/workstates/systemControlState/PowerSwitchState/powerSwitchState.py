#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from abc import abstractmethod
from buildingblocks.workflow.workstate import WorkState
from buildingblocks.Decoraters import overrides
from workstates.crHealth.crTestState import CrTestState

class PowerSwitchState(CrTestState):
    @overrides(CrTestState)
    def SetParentWorkThread(self, val):
        self._powerSwitch = val._powerSwitch
        self._resource = val._resource
        super(CrTestState, self).SetParentWorkThread(val)

    @overrides(WorkState)
    def DoWork(self):
        self.Switch()

    @abstractmethod
    def Switch(self):
        raise NotImplementedError("users must implement the Switch method!")
