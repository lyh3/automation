#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho, henry.li@intel.com
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from abc import abstractmethod
from buildingblocks.Decoraters import overrides
from workstates.crHealth.crTestState import CrTestState


class crHealth_BiosLogDependencyCheck_State(CrTestState):
    @overrides(CrTestState)
    def ReportHealthStatus(self):
        expectedDictionary, reportData = self._initializeTemplate()
        command = self._shellCommand[0]['command']
        results = self._invoke(command)
        toggle = True
        if results is not None:
            try:
                if not isinstance(self._parentWorkThread.SocketTableDictionary, dict):
                    raise LookupError('To run UEFI_001 is required prior to run this test')
                for k, v in self._parentWorkThread.SocketTableDictionary.iteritems():
                    for d in v:
                        rowData = []
                        success = True
                        rowData.append(d)
                        if toggle:
                            bkcolor = self._resource['COLOR']['WHITE']
                        else:
                            bkcolor = self._resource['COLOR']['LIGHYELLOW']
                        lookup = self._lookupConverter(d)
                        rowData.append(lookup)
                        found = self._lookup(lookup, results)
                        if found:
                            icon = self._resource['ICON']['CHECK_MARK_BLUE']
                        else:
                            icon = self._resource['ICON']['CROSS_RED']
                            success = False
                        rowData.append(icon)
                        self._insertSubRow(rowData, reportData, bkcolor)
                        toggle = not toggle
                        self._success &= success
            except Exception as e:
                self._success = False
                rowData = []
                rowData.append('Error at {0}, error : {1}'.format(self._configTest['testid'], str(e)))
                rowData.append(self._resource['ICON']['CROSS_RED'])
                self._insertSubRow(rowData, reportData, self._resource['COLOR']['GOLD'])
                print(str(e))

        self._flushSubTable('Apach pass {}'.format(self.class_id.split('_')[1].upper()), self._resource['COLOR']['SILVER'], reportData)


    @abstractmethod
    def _lookupConverter(self, dimm):
        raise NotImplementedError("users must implement the _lookupConverter method!")

    @abstractmethod
    def _lookup(self, lookup, source):
        raise NotImplementedError("users must implement the _lookupSourceReproduce method!")

    def _invoke(self, command):
        return self._client.ExecuteCommand(command)