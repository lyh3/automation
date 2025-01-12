#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho,
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from workstates.SerialInvokeState.serialInvokeState import SerialInvokeState
from buildingblocks.invokeSerial import InvokeSerial

class Serial_write_State(SerialInvokeState):
    @overrides(SerialInvokeState)
    def SerialInvoke(self):
        data = None
        if self._keystroke is not None:
            for i in range(0, self._recur):
                if self._keystroke['invoke'] == 'write':
                    InvokeSerial(self._port, self._baud).Write(self._keystroke['data'])
                else:
                    data = self._keystroke['data']
                if data is not None:
                    InvokeSerial(self._port, self._baud).WriteLines(data)

if __name__ == '__main__':
    serialWrite = Serial_write_State('COM', 115200, 'N')
    serialWrite.SrialInvoke()