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
import sys, os, json
from buildingblocks.Decoraters import overrides, initializer
from workthreads.complianceCheckThread import ComplianceCheckThread
from workstates.crHealth.crTestState import CrTestState
from workstates.systemControlState.initialParamicoClientState import InitialParamicoClientState
from workstates.systemControlState.systemControlState import SystemControlState
from buildingblocks.invokeSerial import InvokeSerial
from performanceTest import PerformanceTestThread
from workthreads.performanceTuningThread import PerformanceTuningThread
from enum import Enum
try:
    import queue
except ImportError:
    import Queue as queue

class TEST_TYPE(Enum):
    def __str__(self):
        return self.value
    UEFI = 'UEFI'
    PERF = 'PERF'
    DEFAULT = 'DEFAULT'
    PerfTuning = 'PerfTuning'

class SelfCheckThread(ComplianceCheckThread):
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

    @initializer
    def Filters(self):
        return self._filters

    @staticmethod
    def Launch(config, filters, testType):
        resourcePath = os.path.realpath(r'Json/Resource.json')
        try:
            resource = json.loads(open(resourcePath).read())
            serialInvoke = None
            if 'serialport' in config.keys():
                InvokeSerial(config['serialport'], int(config['serialportbaudrate']))
            if testType is TEST_TYPE.PERF:
                t = PerformanceTestThread(config, resource)
            elif testType is TEST_TYPE.PerfTuning:
                t = PerformanceTuningThread(config, resource)
            else:
                t = SelfCheckThread(config, resource, serialInvoke)
            t.Filters = filters
            if testType is TEST_TYPE.UEFI or testType is TEST_TYPE.PERF or testType is TEST_TYPE.PerfTuning:
                t.IsDefaultTestType = False
            t.Start()
        except Exception as e:
            print (str(e))
            sys.exit(1)

    @overrides(ComplianceCheckThread)
    def IntialWork(self):
        self._powerCycleRequestCount = 0
        self._queue = queue.Queue()
        # self._queue.put(InitialITPState())
        if self.IsDefaultTestType:
            self._queue.put(InitialParamicoClientState())

        return self._initialWork('crHealth')


if __name__ == '__main__':
    '''
    if not (sys.version_info > (2, 7, 0) and sys.version_info < (3, 0, 0)):
        sys.stderr.write("Python 2.7 is required to run this tool.\n")
        exit(1)
    '''
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
        jsonpath = os.path.realpath(r'Json/AEPHealthCheck.json')
    config = json.loads(open(jsonpath).read())
    f = None
    if args.filters is not None:
        f = [x.strip().lower() for x in args.filters.split(',') if x != '']
    if args.ip is not None: config['Client']['ip'] = args.ip
    if args.user is not None: config['Client']['user'] = args.user
    if args.password is not None: config['Client']['password'] = args.password
    if args.timeout is not None: config['timeout'] = args.timeout
    filename, extension = os.path.splitext(os.path.basename(jsonpath))
    testType = TEST_TYPE.DEFAULT
    if filename.startswith(TEST_TYPE.UEFI.value):
        testType = TEST_TYPE.UEFI
    elif filename.startswith(TEST_TYPE.PERF.value):
        testType = TEST_TYPE.PERF
    elif filename.startswith(TEST_TYPE.PerfTuning.value):
        testType = TEST_TYPE.PerfTuning

    SelfCheckThread.Launch(config, f, testType)

