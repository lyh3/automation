from buildingblocks.utils import overrides
from workstates.CrTestState.crTestState import CrTestState
from AEPDatamodels.dimm import Dimm
import re

class CrHealth_Template_State(CrTestState):
    @overrides(CrTestState)
    def ReportHealthStatus(self):
        expectedDictionary, reportData = self._initializeTemplate()
        command = self._shellCommand[0]['command']
        ret = self._client.ExecuteCommand(command)

        if ret is not None:
            results = ret.split('\n')
            for expectedKey in expectedDictionary.keys():
                toggle = True
                success = True
                colIdx = None
                logicDictionary = {}
                rowData = []
                rowData.append(expectedKey)

                try:
                    foundCountValDic = expectedDictionary[expectedKey]
                    foundkey = foundCountValDic.keys()[0]

                    op = self._parseOperator(logicDictionary, foundkey)
                    if op is not None:
                        for i in range(0, len(results)):
                            if results[i] is None or results[i] == '':
                                continue
                            key = logicDictionary.keys()[0]
                            valArray = logicDictionary[key]
                            split = [x.lstrip().strip() for x in re.split(self._delimiterPattern(), results[i])
                                     if x.strip() != '' and not x in Dimm.SUPORTED_DIMM_SIZE_UNIT()]

                            if expectedKey in split:
                                for i in range(0, len(split)):
                                    if expectedKey == split[i]:
                                        colIdx = i
                                        break

                            if colIdx is None:
                                raise StandardError('Invalid expected data name')

                            if re.match(r'\d+.?\d+?', split[colIdx]) is None:
                                continue
                            if eval('{0} {1} {2}'.format(split[colIdx], key, valArray[0])):
                                foundCountValDic[foundkey] += 1
                    else:
                        for i in range(0, len(results)):
                            split = self._reproduce(foundkey, results[i])
                            if self._lookupValueConvertor(foundkey) in split:
                                foundCountValDic[foundkey] += 1
                    if toggle:
                        bkcolor = self._resource['COLOR']['WHITE']
                    else:
                        bkcolor = self._resource['COLOR']['LIGHYELLOW']
                    if foundCountValDic[foundkey] > 0:
                        icon = self._resource['ICON']['CHECK_MARK_BLUE']
                    else:
                        icon = self._resource['ICON']['CROSS_RED']
                        success = False

                    rowData.append('{0}({1})'.format(foundkey,
                                                     foundCountValDic[foundkey]))
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

    def _parseOperator(self, logicDictionary, foundkey):
        op = None
        logicOps = '(>=)|(>)|(<=)|(<)|(==)'
        ops = logicOps.replace('(', '').replace(')', '').split('|')
        expectedData = re.split(logicOps, str(foundkey))
        expectedData = [x for x in expectedData if x is not None and x != '']
        for e in expectedData:
            if e in ops:
                op = e
        if op is not None:
            for val in expectedData:
                if val != op:
                    val = Dimm.DimmSizeFormat(val)
                    logicDictionary[op] = [x for x in val.split(' ') if x.strip() != '']
        return op

    def _delimiterPattern(self):
        return r'\s+'

    def _lookupValueConvertor(self, key):
        return key

    def _reproduce(self, pattern, source):
        return source