#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho,
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from workstates.Template.crHealth_Template_State import crHealth_Template_State
from buildingblocks.Decoraters import callCount
import functools
import re
import time
import os

class crHealth_PERF_001_State(crHealth_Template_State):
    def __init__(self, *args, **kwargs):
        super(crHealth_Template_State, self).__init__(*args, **kwargs)
        self._latencyDictionary = {}
        self._testCption = None

    @property
    def TestCapton(self):
        return self._testCption

    @TestCapton.setter
    def TestCaption(self, val):
        self._testCption = val

    @callCount
    def _invoke(self, command):
        self._logger.info('------ Performance test invoke counter = {} -------'.format(self._invoke.count))
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
            outputFolder = self._config['ResultsOutputFolder']
            if not os.path.isdir(outputFolder):
                os.mkdir(outputFolder)
            '''
            # for Unit test
            ret = open(r'C:\PythonSV\crAutoFarm\archive\PerfOutput.log').read()
            '''
            start_time = time.time()
            cmd = command.replace('./', '{}/{}/'.format(self._config['sftp']['targetPath'], self._config['sftp']['OS']))
            #ret = self._client.ExecuteCommand(cmd)
            ret = '---Hello--'
            print ("*** Perf test execution time: %s seconds ***" %(time.time()-start_time))

            logfilename = os.path.realpath(r'{}/{}.log'.format(outputFolder, self._testCption))
            with open(logfilename, 'w') as logfile:
                logfile.write(self._testCption)
                logfile.write(ret)

            return None


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
            header = functools.reduce(lambda a, b : '{} {}'.format(a, b), columns) + ' \n'
            if results == header and count == 0:
                self._parentWorkThread.NotificationMessage(ret)
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
