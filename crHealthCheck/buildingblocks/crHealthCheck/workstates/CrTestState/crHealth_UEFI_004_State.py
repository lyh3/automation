from buildingblocks.utils import overrides, callCount
from workstates.CrTestState.crTestState import CrTestState
import re

class CrHealth_UEFI_004_State(CrTestState):
    @overrides(CrTestState)
    def ReportHealthStatus(self):
        #self._reportInstalledEntities()
        expectedDictionary, reportData = self._initializeTemplate()
        command = self._shellCommand[0]['command']
        ret = self._client.ExecuteCommand(command)
        toggle = True
        if ret is not None:
            results = self._client.ExecuteCommand(command)
            for r in results:
                pass
            for expectedKey in expectedDictionary.keys():
                try:
                    if self._parentWorkThread.SocketTableDictionary is None or len(
                            self._parentWorkThread.SocketTableDictionary) == 0:
                        raise LookupError('To run UEFI_001 is required before to run this test')
                    for k, v in self._parentWorkThread.SocketTableDictionary.iteritems():
                        for d in v:
                            rowData = []
                            success = True
                            rowData.append(d)
                            if toggle:
                                bkcolor = self._resource['COLOR']['WHITE']
                            else:
                                bkcolor = self._resource['COLOR']['LIGHYELLOW']
                            lookup = d.DimmId
                            rowData.append(lookup)
                            found = len(re.findall(lookup, results)) > 0
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
                    rowData.append('{0} : {1}'.format(expectedKey, str(e)))
                    rowData.append(self._resource['ICON']['CROSS_RED'])
                    self._insertSubRow(rowData, reportData, self._resource['COLOR']['WHITE'])
                    print(str(e))

        self._flushSubTable('Apach pass MGT', self._resource['COLOR']['SILVER'], reportData)