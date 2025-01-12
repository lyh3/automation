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
from workstates.crHealth.crTestState import CrTestState

class crHealth_UEFI_002_State(CrTestState):
    @overrides(CrTestState)
    def ReportHealthStatus(self):
        pass