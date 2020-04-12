from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.utils import overrides
import os

class PingSUTState(SystemControlState):
    @overrides(SystemControlState)
    def CallSystemFunction(self):
        try:
            if os.system('ping -c 1 {0}'.format(self._config['Client']['ip'])) is 0:
                self._success = True

        except Exception as e:
            self._logger.error(str(e))
