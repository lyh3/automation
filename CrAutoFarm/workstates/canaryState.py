#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho,
#
# Created:     07/19/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from buildingblocks.workflow.workstate import  WorkState

mycount=0
class CanaryState(WorkState):
    @overrides(WorkState)
    def DoWork(self):
        global mycount
        # print ("--- CanaryState.DoWork() -- counter: {}".format(mycount))
        mycount += 1
