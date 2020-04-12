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
from buildingblocks.Decoraters import overrides
from workthreads.complianceCheckThread import ComplianceCheckThread
from workstates.crHealth.crTestState import CrTestState
from workstates.systemControlState.systemControlState import SystemControlState
from PowerControl.powerSwitcher import PowerSwitcher
from workstates.crPerf.combInfoDictionary import CombInfoDictionary
from buildingblocks.Definitions import Consts

from workstates.crPerf.changeBiosKnobSettingState import ChangeBiosKnobSettingsState
from workstates.systemControlState.initialParamicoClientState import InitialParamicoClientState
from workstates.systemControlState.powerCycleState import PowerCycleState
from workstates.aepCommand.aepCommand_CreateADGoal_State import aepCommand_CreateADGoal_State
from workstates.aepCommand.aepCommand_DeleteNameSpace_State import aepCommand_DeleteNameSpace_State
from AEPDatamodels.biosSettingsComb import BiosSettingsComb
from workstates.crPerf.perfTuning import PerfTuning
from workstates.SerialInvokeState.serial_waitFor_State import Serial_waitFor_State
from workstates.systemControlState.waitForOSBootState import WaitForOSBootState
from workstates.crPerf.crPerf_LatencyTest_State import crPerf_LatencyTest_State
from workstates.openHtmlFileState import OpenHtmlFileState
from workstates.canaryState import CanaryState

import shutil
import weakref

try:
    import queue
except ImportError:
    import Queue as queue

sys.setrecursionlimit(50000)

class PerformanceTuningThread(ComplianceCheckThread):
    @overrides(ComplianceCheckThread)
    def __init__(self, config, resource, itpInstanceTuple):
        super(PerformanceTuningThread, self).__init__(config, resource)
        self._serialInvoke = None
        self._powerSwitch = PowerSwitcher(config['PowerSwitcher'])
        self._queue = None
        self._itpInstanceTuple = itpInstanceTuple

    @overrides(ComplianceCheckThread)
    def StateFactory(self, workState = None):
        state = None
        try:
            if workState is None:
                state = self.IntialWork()
            elif isinstance(workState, SystemControlState) or isinstance(workState, CanaryState):
                if not workState._success:
                    state = workState
                else:
                    state = self._fetchOutState()
            elif isinstance(workState, CrTestState):
                state = self._fetchOutState()

            if state is None:
                if os.path.isdir(Consts.COMB_OUTPUT_FOLDER):
                    combinfos = os.listdir(Consts.COMB_OUTPUT_FOLDER)
                    if len(combinfos) == 0:
                        self.Stop()
                        #self._logger.info('Performance test completed.')
            else:
                state.SetParentWorkThread(self)
                if not isinstance(state, OpenHtmlFileState):
                    self._logger.info('Calling {}'.format(type(state).__name__))
                elif self._queue.qsize() > 0:
                    self.Stop()


            if self._config['simulation']:
                if state is not None and not isinstance(state, PerfTuning):
                    state = CanaryState()
                    #if not (isinstance(state, InitialParamicoClientState)):
                    #    state = CanaryState()
        except Exception as ex:
            self._logger.warning(str(ex))
            self.Stop()
        return state

    @overrides(ComplianceCheckThread)
    def IntialWork(self):
        state = None
        self._powerCycleRequestCount = 0
        q = queue.Queue()
        queueRef = weakref.ref(q)
        self._queue = queueRef()
        if os.path.isdir(Consts.COMB_OUTPUT_FOLDER):
            combinfos = os.listdir(Consts.COMB_OUTPUT_FOLDER)
            if len(combinfos) > 0:
                jsonfileCombInfo = os.path.realpath(r'{}/{}'.format(Consts.COMB_OUTPUT_FOLDER, combinfos[0]))
                combInfo = CombInfoDictionary(open(jsonfileCombInfo, 'r').read())
                comb = combInfo[Consts.COMB_DATA_KEY]

                # The system should be starting with the OS mode
                self._queue.put(InitialParamicoClientState())
                self._queue.put(ChangeBiosKnobSettingsState(BiosSettingsComb(self._config),
                                                            comb[1]))

                self._queue.put(WaitForOSBootState())
                self._queue.put(InitialParamicoClientState())

                latencyTestState = crPerf_LatencyTest_State()
                latencyTestState._configTest = self._config['PerfTuning']['tests'][0]
                latencyTestState.TestCaption = combInfo.__dict__[Consts.COMB_CAPTION_KEY]
                latencyTestState.SetParentWorkThread(self)
                self._queue.put(latencyTestState)

                if os.path.isfile(jsonfileCombInfo):
                    os.remove(jsonfileCombInfo)

        if self._queue.qsize() > 0:
            state = self._queue.get_nowait()
        return state

    @staticmethod
    def Launch(config, resource, itpInstanceTuple):
        try:
            thread = PerformanceTuningThread(config, resource, itpInstanceTuple)
            thread.Start()
            return thread
        except Exception as e:
            print (str(e))
            sys.exit(1)

    @staticmethod
    def GenerateCombJsons(config):
        if os.path.isdir(Consts.COMB_OUTPUT_FOLDER):
            shutil.rmtree(Consts.COMB_OUTPUT_FOLDER)
        settingcomb = BiosSettingsComb(config)
        combs = settingcomb.__dict__.items()
        idx = 0
        for comb in combs:
            combInfo = CombInfoDictionary()
            combInfo[Consts.COMB_DATA_KEY] = comb
            combInfo[Consts.COMB_CAPTION_KEY] = settingcomb.Caption(idx)
            combInfo.Save()
            idx += 1

