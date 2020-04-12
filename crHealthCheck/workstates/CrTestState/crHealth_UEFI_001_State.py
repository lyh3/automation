from buildingblocks.utils import overrides
from workstates.CrTestState.crTestState import CrTestState
from buildingblocks.paramikoClient import ParamikoClient
from AEPDatamodels.dimm import Dimm, Dimm_type, Dimm_Size_Unit
import buildingblocks.utils as util
import re

class CrHealth_UEFI_001_State(CrTestState):
    @overrides(CrTestState)
    def ReportHealthStatus(self):
        expectedDictionary, reportData = self._initializeTemplate()
        DDR4dimmSearchPattern = None
        NVDimmSearchPattern = None
        for k,v in expectedDictionary.iteritems():
            if k == 'DDR4IdSample':
                #pos = [pos for pos, char in enumerate(v.keys()[0]) if char == '-']
                DDR4dimmSearchPattern = eval(v.keys()[0])#[0:4]
            if k == 'IntelDDRTIdSample':
                NVDimmSearchPattern = eval(v.keys()[0])#[0:4]

        try:
            if DDR4dimmSearchPattern is None or NVDimmSearchPattern is None:
                raise IndentationError(
                    'The expected [DDR4IdSample] and Intel [IntelDDRTIdSample] ids sample is required.')

            configHost = self._config['Host']
            ipAddress = configHost['ip'].lstrip()
            if ipAddress is '':
                ipAddress = util.GetLocalHostIP()
            paramikoclient = ParamikoClient(ipAddress, configHost['user'], configHost['password'])
            paramikoclient.Connect()
        except Exception as e:
            paramikoclient = None
            errorMsg = 'Exception caught, error = {}\nAssuming local.'.format(str(e))
            print(errorMsg)
        tableSource = []
        try:
            logfile = self._shellCommand[0]['command']
            if paramikoclient is None:
                lines = open(logfile).read().split('\n')
            else:
                lines = self._client.ExecuteCommand('open({}).readTable()'.format(logfile)).split('\n')

            readTable = False
            collectError = False
            errorMRC = []
            mrcErrorCount = 0
            for line in lines:
                if line.strip().startswith('START_MRC_RUN'):
                    collectError = True
                elif line.strip().startswith('STOP_MRC_RUN'):
                    collectError = False
                if line.strip().startswith('STOP_DIMMINFO_TABLE'):
                    readTable = False
                elif line.strip().startswith('START_DIMMINFO_TABLE'):
                    readTable = True
                elif line.startswith('===') or line.startswith('^^^')or line.startswith('---'):
                    continue
                if readTable:
                    tableSource.append(line)
                if collectError:
                    if len(re.split('ERROR', line)) > 1:
                        mrcErrorCount += 1
                        if not line in errorMRC:
                            errorMRC.append(line)
            if len(errorMRC) > 0:
                self._success = False
                self._insertReportSubHeader('{0} - MRC Error'.format(self._configTest['testid']),
                                            ['({0}) MRC Errors have been captured.'.format(mrcErrorCount), ' '],
                                            reportData)

                self._insertSubRow([errorMRC[0], self._resource['ICON']['CROSS_RED']], reportData, self._resource['COLOR']['PINK'])
                self._flushSubTable('Apach pass UEFI', self._resource['COLOR']['SILVER'], reportData)

            readTable = False
            tableName = ''
            socket = ''
            toggle = True
            idx = 0
            socketTableDictionary = {}
            for line in tableSource:
                line = line.lstrip()
                if line == '':
                    idx += 1
                    continue
                if line.startswith('START_SOCKET_'):
                    reportData = []
                    readTable = True
                    toggle = True
                    idx += 1
                    tableName = line
                    socket = tableSource[idx]
                    continue
                elif line.startswith('STOP_SOCKET_'):
                    self._flushSubTable('Apach pass UEFI', self._resource['COLOR']['SILVER'], reportData)
                    socket = ''
                    readTable = False

                if readTable:
                    if line == socket:
                        idx += 1
                        continue
                    tableName = tableName.replace('START_', '')

                    columns = line.split('|')
                    if len(re.findall('Channel', line)) > 0:
                        self._insertReportSubHeader('{0} - {1}'.format(tableName, socket),
                                                    columns,
                                                    reportData,
                                                    False)
                        if not tableName in socketTableDictionary.keys():
                            socketTableDictionary[tableName] = []
                            socketTableDictionary[tableName].append(columns)
                    else:
                        if toggle:
                            bkcolor = self._resource['COLOR']['WHITE']
                        else:
                            bkcolor = self._resource['COLOR']['LIGHYELLOW']
                        self._insertSubRow(columns, reportData, bkcolor, False)
                        if re.match('\d+\s+DIMM', line.replace('|', '')):
                            socketTableDictionary[tableName].append(columns)
                        if line.replace('|', '').strip()[0:4] in DDR4dimmSearchPattern:
                            socketTableDictionary[tableName].append(columns)
                        if line.replace('|', '').strip()[0:4] in NVDimmSearchPattern:
                            socketTableDictionary[tableName].append(columns)
                        if len([x for x in columns if
                                x is not None and x.strip() != '' and re.match(Dimm.DimmSizeRegexSearchPatterm(), x) is not None]) > 0:
                            socketTableDictionary[tableName].append(columns)
                        if len([x for x in columns if
                                x is not None and x.strip() != '' and re.match(Dimm.DimmTypeRegexSearchPatterm(), x) is not None]) > 0:
                            socketTableDictionary[tableName].append(columns)

                        toggle = not toggle
                idx += 1
            self._populateDimmsDictionary(socketTableDictionary)
        except Exception as e:
            errorMsg = 'Exception caught, error = {}'.format(str(e))
            self._logger.error(errorMsg)
            self._insertReportSubHeader('{0} - Results'.format(self._configTest['testid']),
                                        ['Alert', ' '],
                                        reportData)
            self._insertSubRow([errorMsg, self._resource['ICON']['CROSS_RED']], reportData, self._resource['COLOR']['GOLD'])
            self._flushSubTable('Apach pass UEFI', self._resource['COLOR']['SILVER'], reportData)
            self._success = False
        pass

    def _populateDimmsDictionary(self, sourceDic):
        self._parentWorkThread.SocketTableDictionary = {}
        for k, v in sourceDic.iteritems():
            s = 0
            idx = 1

            self._parentWorkThread.SocketTableDictionary[k] = []
            for i in range(1, len(v)):
                if idx % 4 == 0:
                    c = 0
                    d_idx = 0

                    ds = [x.strip() for x in v[idx - 2]if x != '']
                    manufacture = [x.strip() for x in v[idx - 3]if x != '']
                    dimmType = [x.strip() for x in v[idx - 1] if x != '']

                    for d in [x for x in v[i] if x != '']:
                        dimmsize = []
                        for u in Dimm_Size_Unit:
                            split = [x.strip() for x in ds[d_idx].split(u.value) if True]
                            if len(split) > 0 and re.match('\d+', split[0]) is not None:
                                dimmsize.append(split[0])
                                dimmsize.append(u)
                                break
                        self._parentWorkThread.SocketTableDictionary[k] \
                            .append(Dimm(k.split('_')[1],
                                         s,
                                         c,
                                         Dimm_type([x.strip() for x in re.split(r'\s+', dimmType[d_idx]) if x != ''][0]),
                                         dimmsize,
                                         d.strip(),
                                         manufacture[d_idx + 1].split(':')[1].strip()
                                        ))
                        d_idx += 1
                        c += 1
                    s += 1
                idx += 1
