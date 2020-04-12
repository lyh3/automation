from buildingblocks.utils import overrides,initializer
from workthreads.htmlReportWorkthread import HtmlReportWorkThread

class ComplianceCheckThread(HtmlReportWorkThread):
    KEY_TESTS = 'tests'
    KEY_SKIP = 'skip'
    KEY_TEST_ID = 'testid'
    @overrides(HtmlReportWorkThread)
    def __init__(self, config, resource):
        super(ComplianceCheckThread, self).__init__(config, resource)
        self._filters = None
        self._testResultsDictionary = None
        self._socketTableDictionary = None

    @property
    def SocketTableDictionary(self):
        return self._socketTableDictionary

    @initializer
    def Filters(self):
        return self._filters

    @initializer
    def TestResultsDictionary(self):
        return self._testResultsDictionary

    def UpdateResults(self, tId, result):
        for k, v in self.TestResultsDictionary.iteritems():
            for i in v:
                if i.keys()[0] == tId:
                    i[tId] = result
                    break
