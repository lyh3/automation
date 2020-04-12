from buildingblocks.Decoraters import overrides
from workstates.systemControlState.systemControlState import SystemControlState
import os, re, subprocess, time

class WaitForOSBootState(SystemControlState):
    @overrides(SystemControlState)
    def DoWork(self):
        try:
            self._logger.info("Waiting For OS Boot...")
            while(True):
                self._checkTimeout()
                if self._pingHost(self._config['Client']['ip']):
                    self._success = True
                    self._logger.info("Out of waiting for OS boot...")
                    break
            # After ping succeeded, wait for 60 secs for the OS to be up and running,
            # otherwise Paramico may not be able to establish the SSH connection.
            time.sleep(60)
        except Exception as e:
            self._logger.error(str(e))

    def _pingHost(self, ipaddress):
        hostUp = False
        resp = subprocess.Popen(['ping', ipaddress, '-n', '1'], stdout=subprocess.PIPE).stdout.read()
        if not (len(re.findall('100% loss', resp)) == 1):
            hostUp = True
        return hostUp