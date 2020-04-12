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
from buildingblocks.Decoraters import overrides,initializer
from workthreads.htmlReportWorkthread import HtmlReportWorkThread
from buildingblocks.Definitions import RESULTS
from workstates.systemControlState.archiveDataState import ArchiveDataState
from workstates.systemControlState.waitKeyStrokeState import WaitKeyStrokeState
import buildingblocks.utils as util

class ComplianceCheckThread(HtmlReportWorkThread):
    KEY_TESTS = 'tests'
    KEY_SKIP = 'skip'
    KEY_TEST_ID = 'testid'
    @overrides(HtmlReportWorkThread)
    def __init__(self, config, resource, serialInvoke = None):
        super(ComplianceCheckThread, self).__init__(config, resource)
        self._filters = None
        self._testResultsDictionary = None
        self._socketTableDictionary = None
        self._serialInvoke = serialInvoke

    @initializer
    def SocketTableDictionary(self):
        return self._socketTableDictionary

    @initializer
    def Filters(self):
        return self._filters

    @initializer
    def TestResultsDictionary(self):
        return self._testResultsDictionary

    def UpdateResults(self, tId, result):
        if not isinstance(self.TestResultsDictionary, dict):
            return
        for k, v in self.TestResultsDictionary.iteritems():
            for i in v:
                if i.keys()[0] == tId:
                    i[tId] = result
                    break

    def _initialWork(self, package):
        state = None
        self.TestResultsDictionary = {}
        for k, v in self._config.items():#self._config.iteritems():
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
                                                       package,
                                                       t)
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
            state = self._queue.get_nowait()
        return state

    def _fetchOutState(self):
        if self._queue.qsize() > 0:
            return self._queue.get_nowait()
        else:
            return self.CloseReport()