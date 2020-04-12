import sys, os, json
from buildingblocks.Decoraters import overrides, initializer
from workthreads.complianceCheckThread import ComplianceCheckThread
from workstates.crHealth.crTestState import CrTestState
from workstates.systemControlState.waitKeyStrokeState import WaitKeyStrokeState
from workstates.systemControlState.systemControlState import SystemControlState
from workstates.systemControlState.archiveDataState import ArchiveDataState
from buildingblocks.Definitions import RESULTS
import buildingblocks.utils as util
try:
    import queue
except ImportError:
    import Queue as queue

class UEFI_SelfCheckThread(ComplianceCheckThread):
    @overrides(ComplianceCheckThread)
    def StateFactory(self, workState = None):
        state = None
        if workState is None:
            state = self.IntialWork()
        elif isinstance(workState, SystemControlState):
            if not workState._success:
                state = workState
            else:
                state = self._fetchOutState()
        elif isinstance(workState, CrTestState):
            state = self._fetchOutState()

        self.SetupState(workState, state)
        return state

    def _fetchOutState(self):
        if self._queue.qsize() > 0:
            return self._queue.get_nowait()
        else:
            return self.CloseReport()

    @initializer
    def Filters(self):
        return self._filters

    @staticmethod
    def Launch(config, filters):
        resourcePath = os.path.realpath(r'Json/Resource.json')
        try:
            resource = json.loads(open(resourcePath).read())
            t = UEFI_SelfCheckThread(config, resource)
            t._isUEFI = True
            t.Filters = filters
            t.Start()
        except Exception as e:
            print (str(e))
            sys.exit(0)

    @overrides(ComplianceCheckThread)
    def IntialWork(self):
        state = None
        self._powerCycleRequestCount = 0
        self._queue = queue.Queue()

        self.TestResultsDictionary = {}
        for k, v in self._config.iteritems():
            if type(v) is not dict:
                continue

            if ComplianceCheckThread.KEY_TESTS in v.keys():
                group = []
                skip = v[ComplianceCheckThread.KEY_SKIP]
                if self.Filters is not None:
                    if k.lower() in self.Filters:
                        skip = False
                    else:
                        skip = True

                if skip:
                    for i in range(0, len(v['tests'])):
                        group.append({v['tests'][i]['testid'] : RESULTS.SKIPPED})
                    self.TestResultsDictionary[k] = group
                else:
                    for t in v["tests"]:
                        if t[ComplianceCheckThread.KEY_SKIP]:
                            self._logger.info('Case {} skips'.format(t[ComplianceCheckThread.KEY_TEST_ID]))
                            group.append({t['testid'] : RESULTS.SKIPPED})
                            continue
                        instance = util.CreateInstance(t[ComplianceCheckThread.KEY_TEST_ID],
                                                       'crHealth')
                        if instance is not None:
                            instance._configTest = t
                            self._queue.put(instance)
                            group.append({t['testid'] : RESULTS.UNKNOWN})
                            if instance._configTest['waitkeystroke']:
                                self._queue.put(WaitKeyStrokeState(instance._configTest['testid']))
                            if instance._configTest['powercycle']:
                                self._powerCycleRequestCount += 1
                self.TestResultsDictionary[k] = group
            else:
                continue

        self._queue.put(ArchiveDataState())
        self.NotificationMessage(None)

        if self._queue.qsize() > 0:
            state =  self._queue.get_nowait()
        return state

if __name__ == '__main__':
    if not (sys.version_info > (2, 7, 0) and sys.version_info < (3, 0, 0)):
        sys.stderr.write("Python 2.7 is required to run this tool.\n")
        exit(1)

    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument('-j', action='store', dest='jsonfile', help="Config Json file path", default=None)
    parser.add_argument('-f', action='store', dest='filters', help="Test category", default=None)
    parser.add_argument('-i', action='store', dest='ip', help="SUT ip", default=None)
    parser.add_argument('-u', action='store', dest='user', help="SUT user", default=None)
    parser.add_argument('-p', action='store', dest='password', help="SUT password", default=None)
    parser.add_argument('-t', action='store', dest='timeout', help="Timeout", default=None)
    args = parser.parse_args()
    if args.jsonfile is not None:
        jsonpath = args.jsonfile
    else:
        jsonpath = os.path.realpath(r'Json/UEFI_AEPHealthCheck.json')
    config = json.loads(open(jsonpath).read())
    f = None
    if args.filters is not None:
        f = [x.strip().lower() for x in args.filters.split(',') if x != '']
    if args.ip is not None: config['Client']['ip'] = args.ip
    if args.user is not None: config['Client']['user'] = args.user
    if args.password is not None: config['Client']['password'] = args.password
    if args.timeout is not None: config['timeout'] = args.timeout
    UEFI_SelfCheckThread.Launch(config, f)
