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
from workstates.SerialInvokeState.serialInvokeState import SerialInvokeState
from buildingblocks.invokeSerial import InvokeSerial

class Serial_read_State(SerialInvokeState):
    @overrides(SerialInvokeState)
    def SerialInvoke(self):
        return InvokeSerial(self._port, self._baud).ReadMessage()