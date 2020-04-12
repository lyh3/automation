import sys, os, json
from buildingblocks.utils import overrides
from buildingblocks.workflow.workthread import WorkThread
from workstates.SerialInvokeState.serialInvokeState import SerialInvokeState
from workstates.SerialInvokeState.serial_write_State import  Serial_write_State
from workstates.SerialInvokeState.serial_waitFor_State import Serial_waitFor_State
from buildingblocks.paramikoClient import ParamikoClient
import Queue as queue

class BIOSCheckThread(WorkThread):
    @overrides(WorkThread)
    def __init__(self, biossetting, config, reboot=False):
        super(BIOSCheckThread, self).__init__()
        self._queue = None
        self._config = config
        self._biosConfig = config[biossetting]
        self._reboot = reboot

    @overrides(WorkThread)
    def StateFactory(self, workState = None):
        state = None
        if workState is None:
            state = self.IntialWork()
            '''
            '''
            if isinstance(state, Serial_waitFor_State):
                client = ParamikoClient(self._config['Client']['ip'], self._config['Client']['user'], self._config['Client']['password'])
                try:
                    client.Connect()
                    client.ExecuteCommand('reboot')
                    pass
                except Exception as e:
                    print(str(e))
                    self.Stop()

        elif isinstance(workState, SerialInvokeState):
            if self._queue.qsize() > 0:
                state = self._queue.get_nowait()
            else:
                self.Stop()
        return state

    @staticmethod
    def Launch(biossetting, config, reboot):
        t = BIOSCheckThread(biossetting, config, reboot)
        t.Start()

    @overrides(WorkThread)
    def IntialWork(self):
        state = None
        self._queue = queue.Queue()
        port = self._config['serial']['port']
        baud = int(self._config['serial']['baud'], 0)
        timeout = self._config['timeout']
        if self._reboot:
            serialWaitFor = Serial_waitFor_State(port,
                                                 baud,
                                                 None,
                                                 ['[', ']'],
                                                 timeout
                                                 )
            serialWaitFor.WaitFor = 'Press F2'
            self._queue.put(serialWaitFor)

        for nav in self._biosConfig['navs']:
            writeState = Serial_write_State(port,
                                            baud,
                                            nav['navigation'],
                                            None,
                                            timeout,
                                            int(nav['recur'], 0))
            self._queue.put(writeState)
            invokeState = Serial_write_State(port,
                                             baud,
                                             nav['invoke'],
                                             None,
                                             timeout)
            self._queue.put(invokeState)
        if self._queue.qsize() > 0:
            state = self._queue.get_nowait()
        return state

if __name__ == '__main__':
    if not (sys.version_info > (2, 7, 0) and sys.version_info < (3, 0, 0)):
        sys.stderr.write("Python 2.7 is required to run this tool.\n")
        exit(1)

    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument('-j', action='store', dest='jsonfile', help="Config Json file path", default=None)
    parser.add_argument('-s', action='store', dest='biosSetting', help="BIOS Setting", default=None)
    parser.add_argument('-r', action='store', dest='reboot', help="Reboot system", default=None)

    args = parser.parse_args()
    if args.biosSetting is None:
        print('Plese provide the BIOS setting name.  The usage is -s [the setting name]')
        sys.exit(0)

    if args.jsonfile is not None:
        jsonpath = args.jsonfile
    else:
        jsonpath = os.path.realpath(r'./Json/HealthCheckBIOSConfig.json')
    reboot = False
    if args.reboot is not None and args.reboot == 'True':
        reboot = True
    try:
        config = json.loads(open(jsonpath).read())
        BIOSCheckThread.Launch(args.biosSetting, config, reboot)
    except Exception as e:
        print(str(e))