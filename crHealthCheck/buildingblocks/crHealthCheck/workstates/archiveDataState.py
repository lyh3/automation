from buildingblocks.utils import overrides
from buildingblocks.paeAutomationLog import PaeAutomationLog
from workstates.CrTestState.crTestState import CrTestState
from buildingblocks.utils import RESULTS
import buildingblocks.utils as util
import os

class ArchiveDataState(CrTestState):
    def _movDataFile(self, source, target):
        try:
            os.rename(source, target)
        except Exception as e:
            print(str(e))
    @overrides(CrTestState)
    def DoWork(self):
        try:
            PaeAutomationLog().Close()
            for d in ['log', 'report']:
                self._createDirectory('{0}/{1}'.format(cwd, r'archive/'), d)

            for key in self._parentWorkThread.TestResultsDictionary.keys():
                resultFolder = r'{0}/{1}'.format(cwd, r'archive/results')
                for g in [RESULTS.FAILED, RESULTS.SKIPPED]:
                    if self._createDirectory(resultFolder, key):
                        for k, v in self._parentWorkThread.TestResultsDictionary.iteritems():
                            ids = [x for x in v if x[x.keys()[0]] == g]
                            if len(ids) > 0:
                                filename = '{0}/{1}/{2}_{3}.txt'.format(resultFolder,
                                                                    key,
                                                                    g.value,
                                                                    util.GetCurrentTimestamp('%Y-%m-%d-%H-%M-%S'))
                                outFile = open(filename, 'w')
                                for id in ids:
                                    outFile.writelines(id)
                                outFile.flush()
                                pass

            files = os.listdir(cwd)
            for l in [x for x in files if x.endswith('.log')]:
                source = os.path.join(cwd, l)
                target = os.path.join(cwd, r'archive/log/{0}_{1}.log'.format(l.split('.')[0], util.GetCurrentTimestamp('%Y-%m-%d-%H-%M-%S')))
                self._movDataFile(source, target)
            for l in [x for x in files if x.endswith('.html')]:
                source = os.path.join(cwd, l)
                target = os.path.join(cwd, r'archive/report/{0}'.format(l))
                self._movDataFile(source, target)

        except Exception as e:
            print(str(e))

    def _createDirectory(self, parentfolder, folder):
        try:
            path = os.path.join(parentfolder, folder)
            if not os.path.exists(path):
                os.makedirs(path)
            return True
        except IOError as e:
            self._logger.error(str(e))
            return False