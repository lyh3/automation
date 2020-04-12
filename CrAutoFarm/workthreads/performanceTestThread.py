import os, json
from buildingblocks.Decoraters import overrides
from workthreads.complianceCheckThread import ComplianceCheckThread
from workstates.crHealth.crTestState import CrTestState
from workstates.systemControlState.initialParamicoClientState import InitialParamicoClientState
from PowerControl.powerSwitcher import PowerSwitcher
from workstates.systemControlState.PowerSwitchState.switchOnState import SwitchOnState
from workstates.systemControlState.PowerSwitchState.switchOffState import SwitchOffState
from workstates.systemControlState.powerCycleState import PowerCycleState
from workstates.aepCommand.aepCommand_CreateADGoal_State import aepCommand_CreateADGoal_State
from workstates.aepCommand.aepCommand_DeleteNameSpace_State import aepCommand_DeleteNameSpace_State
from workstates.aepCommand.aepCommandState import AEPCommnadState
from workstates.systemControlState.PowerSwitchState.powerSwitchState import PowerSwitchState
from workstates.aepCommand.sftpState import SftpState
try:
    import queue
except ImportError:
    import Queue as queue

class PerformanceTestThread(ComplianceCheckThread):
    @overrides(ComplianceCheckThread)
    def __init__(self, config, resource):
        super(ComplianceCheckThread, self).__init__(config, resource)
        self._serialInvoke = None
        jsonpath = os.path.realpath(r'Json/PERF_Test.json')
        config = json.loads(open(jsonpath).read())
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
        elif isinstance(workState, CrTestState):
            state = self._fetchOutState()

        if state is None:
            self.Stop()
            self._logger.info('Performance test completed.')
        else:
            state.SetParentWorkThread(self)
        return state

    @overrides(ComplianceCheckThread)
    def IntialWork(self):
        self._powerCycleRequestCount = 0
        self._queue = queue.Queue()
        if not self._config['skipAcCycle']:
            self._queue.put(SwitchOffState())
            self._queue.put(SwitchOnState())
        '''
            Switch AD mode
        '''
        self._queue.put(InitialParamicoClientState())
        self._queue.put(aepCommand_DeleteNameSpace_State())
        self._queue.put(aepCommand_CreateADGoal_State())

        '''
        OS reboot
        '''

        self._queue.put(PowerCycleState())
        self._powerCycleRequestCount += 1

        self._queue.put(InitialParamicoClientState())

        self._queue.put(SftpState())

        return self._initialWork('crHealth')


