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
from abc import abstractmethod
from workstates.crHealth.crTestState import CrTestState
from buildingblocks.Decoraters import overrides,timeElapseTimer


class SystemControlState(CrTestState):
    @overrides(CrTestState)
    def __init__(self, *args, **kwargs):
        super(SystemControlState, self).__init__(*args, **kwargs)
        self._timeoutTimer(0, True)

    @overrides(CrTestState)
    def DoWork(self):
        self.CallSystemFunction()

    @abstractmethod
    def CallSystemFunction(self):
        raise NotImplementedError("users must implement the CallSystemFunction method!")


    @timeElapseTimer
    def _timeoutTimer(self, timeout, reset):
        return self._timeoutTimer.isTimeout

    def _checkTimeout(self, timeout=None):
        self._success = False
        if timeout is not None:
            t = timeout
        else:
            t = self._config['timeout']
        if t is not None and t != '':
            t = t.lstrip().strip()
            if t != '':
                timeout = int(t, 0)
                if self._timeoutTimer(timeout, False):
                    self._success = True
                    if self._logger is not None:
                        self._logger.info('Timeout = {0}sec reached. Test aborted.'.format(timeout))
                    if not self._abort:
                        self._setGraceTermination()
                    self._abort = True
                    return