from buildingblocks.utils import overrides
from workstates.CrTestState.crTestState import CrTestState

class CrHealth_MGT_001_State(CrTestState):
    @overrides(CrTestState)
    def ReportHealthStatus(self):
        command = self._shellCommand[0]['command']
        results = self._client.ExecuteCommand(command)
        self._logger.debug('Call command {} results = {}'.format(command, results))
        topologyResults = results.split('\n')

        colunms = ['BIOS Training']
        reportData = []
        for e in self._configTest['expecteddata']:
            expectedKey = e['name']
            colunms.append(expectedKey)
        colunms.append('Pass/Fail')
        self._insertReportSubHeader('{0} Results'.format(self._configTest['testid']),
                                    colunms,
                                    reportData)
        toggle = True
        for e in self._configTest['expecteddata']:
            for k, v in self._parentWorkThread.SocketTableDictionary.iteritems():
                try:
                    if e['name'] == 'DeviceLocator':
                        producedResults = [x.split('=')[1].strip()
                                           for x in topologyResults if x.strip().split('=')[0] == e['name']]
                        for d in v:
                            rowData = []
                            success = True
                            rowData.append(d)
                            if toggle:
                                bkcolor = self._resource['COLOR']['WHITE']
                            else:
                                bkcolor = self._resource['COLOR']['LIGHYELLOW']
                            lookup = d.TopologyLocater()
                            rowData.append(lookup)
                            found = lookup in producedResults
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
        pass
