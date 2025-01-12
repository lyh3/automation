#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.workflow.workstate import WorkState
from buildingblocks.Decoraters import overrides
import webbrowser, os
import buildingblocks.utils as util

class OpenHtmlFileState(WorkState):
    @overrides(WorkState)
    def SetParentWorkThread(self, val):
        super(OpenHtmlFileState, self).SetParentWorkThread(val)
        cwd = os.getcwd()
        path = '{0}/archive/report'.format(cwd)
        try:
            if not os.path.exists(path):
                os.makedirs(path)
        except IOError as e:
            self._logger.error(str(e))

        url = 'file://{0}'.format(os.path.join(path, self._config["reporthtmlfilename"]))
        self._urlSummary = '{0}_Summary_{1}.html'.format((url).replace('.html', ''),
                                                         util.GetCurrentTimestamp('%Y-%m-%d-%H-%M-%S'))
        self._url = self._urlSummary.replace('_Summary', '')
        pass

    def GetUrl(self):
        return self._url

    def GetSummaryUrl(self):
        return self._urlSummary

    @overrides(WorkState)
    def DoWork(self):
        if self._urlSummary is not None:
            try:
                webbrowser.open(self._urlSummary, 2)
            except:pass