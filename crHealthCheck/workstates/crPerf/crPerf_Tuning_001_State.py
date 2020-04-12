#-------------------------------------------------------------------------------
# Name:
# Purpose:
#
# Author:      liyingho
#
# Created:     07/17/2019
# Copyright:   (c) Intel Co. 2019
# Licence:     <your licence>
#-------------- -----------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from workstates.crHealth.crHealth_PERF_001_State import crHealth_PERF_001_State
from changeBiosKnobSettingState import ChangeBiosKnobSettingsState
from workstates.systemControlState.initialParamicoClientState import InitialParamicoClientState
from workstates.systemControlState.powerCycleState import PowerCycleState
from workstates.aepCommand.aepCommand_CreateADGoal_State import aepCommand_CreateADGoal_State
from workstates.aepCommand.aepCommand_DeleteNameSpace_State import aepCommand_DeleteNameSpace_State
from AEPDatamodels.biosSettingsComb import BiosSettingsComb
from perfTuning import PerfTuning
import os

try:
    import Queue as queue
except:
    import queue as queue


class crPerf_Tuning_001_State(PerfTuning):
    @overrides(PerfTuning)
    def __init__(self, *args, **kwargs):
        super(PerfTuning, self).__init__(*args, **kwargs)

    @overrides(PerfTuning)
    def DoWork(self):
        try:
            jsonpath = os.path.realpath(r'../CrAutoFarm/Json/PerfTuning.json')
            settingcomb = BiosSettingsComb(jsonpath)
            q = queue.Queue()
            combs = settingcomb.__dict__.items()
            for idx in range(0, len(combs)):
                comb = combs[idx]
                q.put(ChangeBiosKnobSettingsState(BiosSettingsComb(jsonpath), comb[1]))

                '''
                    Switch AD mode
                '''
                #q.put(InitialParamicoClientState())
                q.put(aepCommand_DeleteNameSpace_State())
                q.put(aepCommand_CreateADGoal_State())

                '''
                OS reboot
                '''
                q.put(PowerCycleState())
                self._parentWorkThread._powerCycleRequestCount += 1

                q.put(InitialParamicoClientState())
                latencyTestState = crHealth_PERF_001_State()
                latencyTestState.TestCaption = settingcomb.Caption(idx)
                latencyTestState.SetParentWorkThread(self._parentWorkThread)
                q.put(latencyTestState)

            currentQueue = self._parentWorkThread._queue
            while(True):
                if currentQueue.qsize() == 0:
                    break
                q.put(currentQueue.get_nowait())

            self._parentWorkThread._queue = queue.Queue()
            while(True):
                if q.qsize() == 0:
                    break
                self._parentWorkThread._queue.put(q.get_nowait())
            pass
        except Exception as e:
            self._logger.error(str(e))
            self._success = False

