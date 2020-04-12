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
from buildingblocks.workflow.workstate import WorkState
from buildingblocks.Decoraters import overrides


class EnvironmentCheckState(WorkState):
    @overrides(WorkState)
    def DoWork(self):
        pass