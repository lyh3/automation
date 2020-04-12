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
from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.Decoraters import initializer
import re

class Serial_waitFor_State(SerialInvokeState, SystemControlState):
    @overrides(SerialInvokeState)
    def SerialInvoke(self):
         while True:
            try:
                self._checkTimeout(self._timeout)
                if self._success:
                    print('Timeout for [{0}] (sec).'.format(self._timeout))
                    break;
                ret = InvokeSerial(self._port, self._baud).ReadMessage().strip()
                if ret is not None and ret != '':
                    for t in self._trims:
                        ret = ret.replace(t, '')
                    print(ret)
                    if len(re.findall(self.WaitFor, ret)) > 0:
                        if self.WaitFor == 'Press F2':
                            InvokeSerial(self._port, self._baud).WriteLines(r'\x1b2')
                        print('Find [{0}].'.format(self.WaitFor))
                        break

            except Exception as e:
                print(str(e))

    @initializer
    def WaitFor(self):
        return self._waitFor

if __name__ == '__main__':
    import os
    serialWaitFor = Serial_waitFor_State('COM3',
                                     115200,
                                     None,
                                     ['[', ']'],
                                     '400',
                                     1,
                                     os.path.realpath(r'../../Json/SerialInvoke.json'),)
    serialWaitFor.WaitFor = 'Press F2'
    serialWaitFor.SerialInvoke()

    pass