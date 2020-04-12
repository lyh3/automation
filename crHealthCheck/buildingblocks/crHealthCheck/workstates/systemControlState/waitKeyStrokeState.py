from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.utils import overrides
import msvcrt as ms

class WaitKeyStrokeState(SystemControlState):
    @overrides(SystemControlState)
    def __init__(self, who):
        self.__defaultExpectedkey = 13 #the ordinary value of <enter> key
        self._who = who

    @overrides(SystemControlState)
    def CallSystemFunction(self):
        self._success = False
        print('\n{0} - Waiting for key stroke from users: [Ctl-C] = exit, [Catrige Return] = continue ....\n'.format(self._who))
        while True:
            key = ord(ms.getwch())
            print('Received key stroke = {0}'.format(key))
            if key == self.__defaultExpectedkey:
                break
            elif key == 3: # ctl-c = 3, ctl-z = 26
                self._setGraceTermination('User terminated.')
                break
        self._success = True

def main():
    while True:
        key = ord(ms.getwch())
        if key == 27:  # ESC
            break
        else:
            print key

if __name__ == '__main__':
    main()