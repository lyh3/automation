import sys
from buildingblocks.Decoraters import overrides
from workthreads.complianceCheckThread import ComplianceCheckThread
from workstates.crHealth.crTestState import CrTestState
from workstates.systemControlState.initialParamicoClientState import InitialParamicoClientState
from PowerControl.powerSwitcher import PowerSwitcher
from workstates.aepCommand.aepCommandState import AEPCommnadState
from workstates.systemControlState.PowerSwitchState.powerSwitchState import PowerSwitchState
from workstates.aepCommand.sftpState import SftpState
from workstates.crPerf.perfTuning import PerfTuning
import weakref
try:
    import Queue as queue
except:
    import queue as queue

sys.setrecursionlimit(50000)

class PerformanceTuningThread(ComplianceCheckThread):
    @overrides(ComplianceCheckThread)
    def __init__(self, config, resource):
        super(PerformanceTuningThread, self).__init__(config, resource)
        self._serialInvoke = None
        self._powerSwitch = PowerSwitcher(config['PowerSwitcher'])


    @overrides(ComplianceCheckThread)
    def StateFactory(self, workState = None):
        state = None
        if workState is None:
            state = self.IntialWork()
        elif (isinstance(workState, PowerSwitchState) or isinstance(workState, AEPCommnadState)) and not workState._success:
            errormessage = 'Setup pre-required for PERF test failed on {}. Please review the log file to find out the details.'.format(workState.class_id)
            self._logger.error(errormessage)
            workState._setGraceTermination(errormessage)
            state = self.CloseReport()
        elif isinstance(workState, InitialParamicoClientState):
            if not workState._success:
                state = workState
            else:
                state = self._fetchOutState()
        elif isinstance(workState, CrTestState) or self._queue.qsize() > 0:
            state = self._fetchOutState()

        if state is None:
            self.Stop()
            self._logger.info('Performance test completed.')
        else:
            state.SetParentWorkThread(self)
            self._logger.info('Calling {}'.format(type(state).__name__))

        if self._config['simulation']:
            from workstates.canaryState import CanaryState
            if state is not None and not isinstance(state, PerfTuning):
                state = CanaryState()

        return state

    @overrides(ComplianceCheckThread)
    def IntialWork(self):
        q = queue.Queue()
        queueRef = weakref.ref(q)
        self._queue = queueRef()  # queue.Queue()
        if not self._config['simulation']:
            self._queue.put(InitialParamicoClientState())
            self._queue.put(SftpState())
        return self._initialWork('crPerf')


