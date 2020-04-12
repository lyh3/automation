from workstates.crPerf.perfTuning import PerfTuning
from buildingblocks.Decoraters import callCount
import time
import os

class crPerf_LatencyTest_State(PerfTuning):
    def __init__(self, *args, **kwargs):
        super(crPerf_LatencyTest_State, self).__init__(*args, **kwargs)
        self._latencyDictionary = {}
        self._testCption = None

    @property
    def TestCapton(self):
        return self._testCption

    @TestCapton.setter
    def TestCaption(self, val):
        self._testCption = val

    @callCount
    def DoWork(self):
        self._logger.info('------ Latency test invoke counter = {} -------'.format(self.DoWork.count))
        outputFolder = self._config['ResultsOutputFolder']
        if not os.path.isdir(outputFolder):
            os.mkdir(outputFolder)
        '''
        # for Unit test
        ret = open(r'C:\PythonSV\crAutoFarm\archive\PerfOutput.log').read()
        "command": "/usr/bin/ipmctl show -topology"
        "sh ./dcpmm_perf_sweep.sh -m ./mlc -p /mnt/pmem0 -s 0"
        '''
        start_time = time.time()
        command = self._configTest['shellCommand'][0]['command']
        cmd = command.replace('./', '{}/{}/'.format(self._config['sftp']['targetPath'], self._config['sftp']['OS']))
        if self._client is None:
            raise Exception('The dependency Paramiko must be initialized successfully. ')
        ret = self._client.ExecuteCommand(cmd)
        self._logger.info("*** Perf test execution time elapsed: %s seconds ***" % (time.time() - start_time))

        logfilename = os.path.realpath(r'{}/{}.log'.format(outputFolder, self._testCption))
        with open(logfilename, 'w') as logfile:
            logfile.write('{}\n\n'.format(self._testCption))
            logfile.write(ret)
            logfile.close()
