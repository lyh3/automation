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
import sys,os
sys.path.append(os.path.split(os.path.dirname(os.getcwd()))[0])
sys.path.append(os.path.dirname(__file__))
sys.path.append(r'..//')
from buildingblocks.Decoraters import overrides, callCount
from buildingblocks.workflow.workthread import WorkThread
from workstates.openHtmlFileState import OpenHtmlFileState
from buildingblocks.AutomationLog import AutomationLog
from workstates.systemControlState.archiveDataState import ArchiveDataState
from buildingblocks.Definitions import RESULTS
from buildingblocks.Decoraters import messageBuffer
import buildingblocks.utils as util
import socket


class HtmlReportWorkthread(WorkThread):
    @overrides(WorkThread)
    def __init__(self, config, resource, isDefaultTestType = True):
        super(HtmlReportWorkThread, self).__init__()
        self._isDefaultTestType = isDefaultTestType
        self._resource = resource
        logName = self.class_id
        automationlog = AutomationLog(logName)
        self._config = config
        self._logger = automationlog.GetLogger(logName)
        automationlog.TryAddConsole(logName)
        self._client = None
        self._reportHeader = []
        self._reportData = []
        self._powerCycleRequestCount = 0
        self._reportHeader.append('<html xmlns="http://www.w3.org/1999/xhtml">')
        self._reportHeader.append('<body style="background-color:{0};">'.format('rgb(225, 231, 242)'))
        self._reportHeader.append('<table cellspacing="0" cellpadding="0" border="0" style="height: 50px;width: 100%;'
                                  'background-color:steelblue; font-family:arial,helvetica; '
                                  'font-size:smaller; font-weight:bold; color:white;border-radius:5px;">')
        self._reportHeader.append('<tr style="border:none;">')
        s = self._config['Client']['ip']
        if s is None:
            s = socket.gethostname()
        self._reportHeader.append('<th align=left style="padding-left:3px;">{0}</th><th>'.format(self._resource['ICON']['INTEL_LOGO']))
        self._reportHeader.append('Apache Pass Health Check Report [ {0} ] on system @{1}'.format(util.GetCurrentTimestamp(), s))
        self._reportHeader.append('</th></tr></table>')

    @callCount
    def SkipCount(self):
        pass
    @callCount
    def FailCount(self):
        pass
    @callCount
    def PassCount(self):
        pass
    @property
    def IsDefaultTestType(self):
        return self._isDefaultTestType
    @IsDefaultTestType.setter
    def IsDefaultTestType(self, val):
        self._isDefaultTestType = val
    @property
    def PowerCycleRequestCount(self):
        return self._powerCycleRequestCount

    def SetupState(self, currentState, nextState):
        if nextState is None \
        or (currentState is not None and isinstance(currentState, ArchiveDataState)):#OpenHtmlFileState)):
            self.Stop()

        elif nextState is not None:
            if not type(nextState) is OpenHtmlFileState:
                nextState.SetParentWorkThread(self)
            if currentState is not None:
                nextState._abort = currentState._abort
                self._logger.info('{0} : completed.'.format(currentState.class_id.replace('_state', '')))
                self._logger.info('Next test : {0}'.format(nextState.class_id.replace('_state', '')))

    def CloseReport(self):
        if self._reportData is None or len(self._reportData) == 0:
            return
        self._reportData.append('</table>')
        self._reportData.append('</body>')
        self._reportData.append('</html>')
        state = OpenHtmlFileState()
        state.SetParentWorkThread(self)
        self._writeStatistics(state)
        try:
            fsummary = open(state.GetSummaryUrl().replace('file://', ''), 'w')
            for line in self._reportHeader:
                fsummary.write(line)
            fsummary.writelines('</table></body></html>')
            fsummary.flush()

            f = open(state.GetUrl().replace('file://', ''), 'w')
            for line in self._reportHeader:
                f.write(line)
            for line in self._reportData:
                f.write(line)
            f.flush()
        except Exception as e:
            print (str(e))

        return state

    def _writeStatistics(self, state):
        # ------------------ write header ----------------------------------------------
        v = ['', '']
        if self._client is not None:
            versionInfo = self._client.ExecuteCommand(self._config['Report']['versionCommand']).split('\n')
            v = versionInfo[1].split('Version')
        self._reportHeader.append('<table cellspacing="1" border="0" style="width: 100%; border-color:silver">')
        self._reportHeader.append('<tr style="height: 28px;background-color:%s;font-size:8pt;font-family: '
                                  'Verdana, Geneva, Arial, Helvetica, sans-serif;text-align:center;color: black">'
                                  % 'rgb(225, 231, 242)')
        self._reportHeader.append('<td>{0}: <span style="font-weight:bold;">{1}</span></td>'.format(v[0], v[1]))


        STATISTIC_TD_FORMAT = '<td style="font-size:8pt;background-color:{2};' \
                              'border-radius:5px;">&nbsp&nbsp{0}:' \
                              '&nbsp&nbsp<span style="font-weight:bold;">{1}</span>&nbsp&nbsp</td>'
        self._reportHeader.append('<td align=right>')
        self._reportHeader.append('<table cellspacing="1" border="0" style="width:250px; border-color:steelblue;'
                                  'font-size:9pt;text-align:center;">')
        self._reportHeader.append('<tr>')
        bkcolor = self._resource['COLOR']['SILVER']
        tooltipformat = STATISTIC_TD_FORMAT.replace('<td style="', '<td style="color:white;')

        total = 0
        for r in RESULTS:
            for k, v in self.TestResultsDictionary.iteritems():
                for i in range(0, len([x for x in v if x[x.keys()[0]] == r])):
                    if r == RESULTS.SKIPPED:
                        self.SkipCount()
                    elif r == RESULTS.PASSED:
                        self.PassCount()
                    elif r == RESULTS.FAILED:
                        self.FailCount()
                    total += 1

        if self.SkipCount.count > 0:
            bkcolor = self._resource['COLOR']['GOLD']
        persentage = 0
        if total > 0:
            persentage = '%.1f' % (self.SkipCount.count * 100.0 / total) + '%'
        self._reportHeader.append(tooltipformat.format('Skipped',
                                                     persentage,
                                                     bkcolor))
        bkcolor = self._resource['COLOR']['SILVER']
        if self.PassCount.count > 0:
            bkcolor = self._resource['COLOR']['DODGERBLUE']
        persentage = 0
        if total > 0:
            persentage = '%.1f' % (self.PassCount.count * 100.0 / total)+ '%'
        self._reportHeader.append(tooltipformat.format('Passed',
                                                       persentage,
                                                       bkcolor))
        bkcolor = self._resource['COLOR']['SILVER']
        if self.FailCount.count > 0:
            bkcolor = self._resource['COLOR']['PINK']
        persentage = 0
        if total > 0:
            persentage = '%.1f' % (self.FailCount.count * 100.0 / total) + '%'
        self._reportHeader.append(tooltipformat.format('Failed',
                                                       persentage,
                                                       bkcolor))
        self._reportHeader.append(tooltipformat.format('Total',
                                                     total,
                                                     'dimgrey'))
        self._reportHeader.append('<td>&nbsp;&nbsp;</td></tr></table></td>')
        self._reportHeader.append('</tr></table>')

        # ------ write statistics table ------------------------------------------------
        MOD = 2
        n = 1
        dic = {}
        staticeticsSource = []
        for k, v in self.TestResultsDictionary.iteritems():
            dic[k] = v
            if n % MOD is 0:
                staticeticsSource.append(dic)
                dic = {}
            n += 1
        if len(dic) > 0:
            staticeticsSource.append(dic)

        for lineitem in staticeticsSource:
            if len(lineitem) is 0:
                continue
            groupCount = len(lineitem)
            if groupCount < MOD:
                groupCount = MOD

            self._reportHeader.append('<table cellspacing="0" cellpadding="0" border="0" '
                                      'style="border-radius:5px;width: 100%;">')
            self._reportHeader.append('<tr>')
            width = 100 / groupCount
            if groupCount is 1:
                width = 100
            for k, v in lineitem.iteritems():
                self._reportHeader.append(
                    '<th colspan="8" style="height: 30px;width:{1}%;background-color:{2};color:dimgrey;'
                    'font-family:arial,helvetica; font-size:smaller;">{0}&nbsp;&nbsp;({3})'
                    .format(k, width, self._resource['COLOR']['GRAY'], len([x for x in v if True])))

                self._reportHeader.append('</th>')
                self._reportHeader.append('<th>&nbsp;</th>')
            self._reportHeader.append('</tr></table>')

            self._reportHeader.append('<table cellspacing="0" cellpadding="0" border="0" '
                                      'style="border-radius:5px;width:100%;">')
            self._reportHeader.append('<tr style="border:none;">')
            tooltipformat = STATISTIC_TD_FORMAT.replace('<td', '<td title={3}')
            for k, v in lineitem.iteritems():
                w = 100 / MOD
                if len(lineitem) % MOD is 0:
                    w = 100
                self._reportHeader.append('<td>')
                self._reportHeader.append(
                    '<table cellspacing="1" border="0" style="border-radius:5px;width:{0}%;'
                    'font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;'
                    'border-color:dimgray;font-size:10pt;text-align:center;"><tr style="height:35px;">'.format(w))
                for i in RESULTS:
                    bkcolor = self._resource['COLOR']['SILVER']
                    if i == RESULTS.UNKNOWN:
                        break
                    elif i == RESULTS.FAILED:
                        prefix = self._resource['ICON']['ERROR_BLACK'] + '&nbsp;'
                        failed = [x for x in v if x[x.keys()[0]] == RESULTS.FAILED]
                        count = len(failed)
                        if count > 0:
                            bkcolor = self._resource['COLOR']['RED']
                            tooltip = 'Failed&#32;ID&#32;of&#32;test&#32;cases&#58;&#32;'
                            for t in failed:
                                tooltip = r'{0}&#32;&#91;&#32;{1}&#32;&#93;&#32;'.format(tooltip, t.keys()[0])
                            self._reportHeader.append(tooltipformat.format( prefix + i.value,
                                                                         count,
                                                                         bkcolor,
                                                                         tooltip))
                            self._logger.error('Failed tests: {0}'.format([x.keys()[0] for x in failed if True]))
                        else:
                            self._reportHeader.append(STATISTIC_TD_FORMAT.format(prefix + i.value,
                                                                                 count,
                                                                                 bkcolor))
                    elif i == RESULTS.PASSED:
                        prefix = self._resource['ICON']['CHECK_MARK_BLACK'] + '&nbsp;'
                        count = len([x for x in v if x[x.keys()[0]] == RESULTS.PASSED])
                        if count > 0:
                            bkcolor = self._resource['COLOR']['LAWGREEN']
                        self._reportHeader.append(STATISTIC_TD_FORMAT.format(prefix + i.value,
                                                                             count,
                                                                             bkcolor))
                    elif i == RESULTS.SKIPPED:
                        prefix = self._resource['ICON']['NEXT_STEP_BLACK'] + '&nbsp;'
                        skipped = [x for x in v if x[x.keys()[0]] == RESULTS.SKIPPED]
                        count = len(skipped)
                        if count > 0:
                            tooltip = 'Skipped&#32;ID&#32;of&#32;test&#32;cases&#58;&#32;'
                            for t in skipped:
                                tooltip = r'{0}&#32;&#91;&#32;{1}&#32;&#93;&#32;'.format(tooltip, t.keys()[0])
                            bkcolor = self._resource['COLOR']['LIGHYELLOW']
                            self._reportHeader.append(tooltipformat.format(prefix + i.value,
                                                                             count,
                                                                             bkcolor,
                                                                             tooltip))
                        else:
                            self._reportHeader.append(STATISTIC_TD_FORMAT.format(prefix + i.value,
                                                                                 count,
                                                                                 bkcolor))
                self._reportHeader.append('</td>')
                self._reportHeader.append('</tr></table>')
            self._reportHeader.append('</tr></table>')

        self._reportHeader.append('<table cellspacing="0" cellpadding="0" border="0" '
                                  'style="border-radius:5px;background-color:{0};width: 100%;">'.format(self._resource['COLOR']['GRAY']))
        self._reportHeader.append('<tr style="border:none;height:40px;color:black;">')
        self._reportHeader.append('<th align=left style="color:grey;">&nbsp;&nbsp;'
                                  '<span style="color:black">Test results:</span>&nbsp;'
                                  '<span style="font-size:8pt;font-weight:normal;">'
                                  'If there are failed/skipped cases, please mouse hoover to the failed/skipped button '
                                  'above to view the ID list. To view the detailed report, click on the dropdown icon '
                                  'at the right.</span>{0}</th>'.format(self._resource['ICON']['ARROW_RIGHT_BLAK']))
        self._reportHeader.append('<th title="View detailed report" align=right><a href="{1}">{0}</a>&nbsp;&nbsp;</th>'
                                  .format(self._resource['ICON']['ARROW_DOWN_BLACK'], state.GetUrl()))
        self._reportHeader.append('</tr></table>')

        self._reportHeader.append('<br>')

        if len(self.NotificationMessage.content) > 0:
            self._reportHeader.append('<table cellspacing="1" border="0" '
                                      'style="width:100%; border-color:silver;">')
            self._reportHeader.append('<tr style="height:50px;background-color:%s;font-size:12pt;font-family: '
                                      'Verdana, Geneva, Arial, Helvetica, sans-serif;text-align:center;color:red">'
                                      % 'black')
            msg = ''
            for m in self.NotificationMessage.content:
                msg += '{0}\n'.format(m)
            self._reportHeader.append('<td style="border-radius:5px;font-weight:normal;">{0}<td/>'.format(msg))
            self._reportHeader.append('</tr></table>')

        if  self._isDefaultTestType and self._client is None:
            msg = 'Cannot connect to SUT @ {0}. The Python package [Paramiko] is required. ' \
                  'If this module has been installed, please verify the connection and the access privalege. ' \
                  'If there is a power cycle in theworkflow, value of the timeout set to 300 sec or above ' \
                  'is recommended.'\
                .format(self._config['Client']['ip'])
            self._logger.error(msg)
            self._reportHeader.append('<br><table cellspacing="1" border="0" style="width:100%; border-color:silver;">')
            self._reportHeader.append('<tr style="height: 100px;background-color:%s;font-size:14pt;font-family: '
                                      'Verdana, Geneva, Arial, Helvetica, sans-serif;text-align:center;color: white">'
                                      % 'rgb(229, 128, 145)')
            self._reportHeader.append('<th style="border-radius:10px;font-weight:normal;">{0}<th/>'.format(msg))
            self._reportHeader.append('</tr></table>')

    @messageBuffer
    def NotificationMessage(self, msg):
        pass
