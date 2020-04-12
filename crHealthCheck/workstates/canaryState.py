#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho, henry.li@intel.com
#
# Created:     07/19/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from buildingblocks.workflow.workstate import  WorkState


class CanaryState(WorkState):
    @overrides(WorkState)
    def DoWork(self):
        pass
