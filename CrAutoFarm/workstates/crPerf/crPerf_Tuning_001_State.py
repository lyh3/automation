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
from workstates.crPerf.changeBiosKnobSettingState import ChangeBiosKnobSettingsState
from workstates.systemControlState.initialParamicoClientState import InitialParamicoClientState
from workstates.systemControlState.powerCycleState import PowerCycleState
from workstates.aepCommand.aepCommand_CreateADGoal_State import aepCommand_CreateADGoal_State
from workstates.aepCommand.aepCommand_DeleteNameSpace_State import aepCommand_DeleteNameSpace_State
from AEPDatamodels.biosSettingsComb import BiosSettingsComb
from workstates.crPerf.perfTuning import PerfTuning
import os

try:
    import queue
except ImportError:
    import Queue as queue


class crPerf_Tuning_001_State(PerfTuning):
    @overrides(PerfTuning)
    def __init__(self, *args, **kwargs):
        super(PerfTuning, self).__init__(*args, **kwargs)

    @overrides(PerfTuning)
    def DoWork(self):
        try:
            #jsonpath = os.path.realpath(r'../CrAutoFarm/Json/PerfTuning1.json')
            #settingcomb = BiosSettingsComb(jsonpath)
            settingcomb = BiosSettingsComb(self._config)
            q = queue.Queue()
            combs = settingcomb.__dict__.items()
            #for idx in range(0, len(combs)):  # python 2.7 not support this
                #comb = combs[idx]  # this is not supported in python 3
            idx = 0
            for comb in combs:
                q.put(ChangeBiosKnobSettingsState(BiosSettingsComb(self._config), comb[1]))

                '''
                    Switch AD mode
                '''
                q.put(InitialParamicoClientState())
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

                idx += 1

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

