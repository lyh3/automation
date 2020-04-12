from buildingblocks.utils import overrides
from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.utils import callCount
from time import sleep

class PowerCycleState(SystemControlState):
    @overrides(SystemControlState)
    def CallSystemFunction(self):
        if self._powerCycleCount.count > self.ParentWorkThread.PowerCycleRequestCount:
            self._setGraceTermination('System power cycle count {0} is more than the count {1} of request in the workflow. '
                                      'Please verify if to increase the timeout is necessary.'
                                      .format(self._powerCycleCount.count, self.ParentWorkThread.PowerCycleRequestCount))
            return

        self._powerCycleCount()
        self._client.ExecuteCommand('reboot')
        sleep(self._config['timeout'])
        pass

    @callCount
    def _powerCycleCount(self):
        pass