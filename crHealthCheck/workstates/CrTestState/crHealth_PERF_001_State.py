from workstates.Template.crHealth_Template_State import CrHealth_Template_State
import re

class CrHealth_PERF_001_State(CrHealth_Template_State):
    def __init__(self):
        super(CrHealth_Template_State, self).__init__()
        self._latencyDictionary = {}

    def _invoke(self, command):
        columns = ['DCPMMIdleSequentialLatency',
                   'DCPMMIdleRandomLatency',
                   'AverageOfMaxDCPMMBandwidth']
        datarow = ''
        results = ''
        header = ''
        isBandwidth = False
        bandwith = 0
        count = 0
        startSequencialTable = False
        startRandomTable = False
        try:
            '''
            # for Unit test
            ret = open(r'C:\PythonSV\crHealthCheck\archive\PerfOutput.log').read()
            '''
            ret = self._client.ExecuteCommand(command)

            self._latencyDictionary['sequential'] = []
            self._latencyDictionary['random'] = []
            if ret is not None and len(ret) > 0:
                for c in columns:
                    header = header + c + ' '
                results = header + '\n'
                for x in ret.split('\n'):
                    if re.match('\d+\s+core\s+DCPMM\s+sequential', x) is not None:
                        startRandomTable = False
                        startSequencialTable = True
                    if startSequencialTable:
                        self._latencyDictionary['sequential'].append(x)
                    if re.match('\d+\s+core\s+DCPMM\s+random', x) is not None:
                        startSequencialTable = False
                        startRandomTable = True
                    if startRandomTable:
                        self._latencyDictionary['random'].append(x)
                    if re.match('\s+?=====', x) is not None:
                        startSequencialTable = False
                        startRandomTable = False
                    if re.match('DCPMM idle sequential', x) is not None:
                        split = re.split('\(\t', x)
                        datarow += split[1].replace('\t', ' ').rstrip(')') + ' '
                    if re.match('DCPMM idle random', x) is not None:
                        split = re.split('\(\t', x)
                        datarow += split[1].replace('\t', ' ').rstrip(')') + ' '
                    if re.match('max DCPMM bandwidth', x) is not None:
                        isBandwidth = True
                    if isBandwidth:
                        split = re.split('\s+', x)
                        bandwith += eval(split[len(split) - 1])
                        count += 1
                    if count == 10:
                        results += '{0} {1}\n'.format(datarow, bandwith/10)
                        isBandwidth = False
            return results
        except Exception as e:
            raise e
        return None

    def _comprehensionData(self, data):
        return [x.lstrip().strip() for x in re.split(self._delimiterPattern(), data) \
                if x.strip() != '' and not x in ['ns']]

    def _insertAdditionalTable(self, reportData):
        for k,v in self._latencyDictionary.iteritems():
            toggle = True
            self._insertSubRow([v[0], '', ''], reportData, self._resource['COLOR']['GRAY'])
            self._insertSubRow(['Delay','ns', 'MBPS'], reportData, self._resource['COLOR']['SILVER'])

            for i in range(2, len(v) - 4):
                rowData = []
                l = v[i]
                for c in l.split('\t'):
                    rowData.append(c)
                if toggle:
                    bkcolor = self._resource['COLOR']['WHITE']
                else:
                    bkcolor = self._resource['COLOR']['LIGHYELLOW']
                toggle = not toggle
                self._insertSubRow(rowData, reportData, bkcolor)
