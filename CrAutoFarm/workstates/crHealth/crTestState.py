#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho, henry.li@intel.com
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
import re
from abc import abstractmethod
from buildingblocks.Decoraters import overrides,initializer
from buildingblocks.workflow.workstate import WorkState
from buildingblocks.Definitions import RESULTS
try:
    import queue
except ImportError:
    import Queue as queue


class CrTestState(WorkState):
    INSTALL_ENTITY_FORMAT = 'Installed {} = {}, found = {}.'
    @overrides(WorkState)
    def SetParentWorkThread(self, val):
        super(CrTestState, self).SetParentWorkThread(val)
        self._reportData = val._reportData
        self._logger = val._logger
        self._client = val._client
        self._config = val._config
        self._resource = val._resource
        self._parentWorkThread.SocketTableDictionary = val.SocketTableDictionary
        self._shellCommand = None
        self._abort = None
        self._serialInvoke = val._serialInvoke

    def InsertReportHeader(self, testid):
        self._reportData.append('<table cellspacing="0" cellpadding="0" border="0" style="width: 100%;">')
        self._reportData.append('<tr style="border:none;">')
        self._reportData.append(
            '<th colspan="8" style="border:1px; width:100%; height: 40px;background-color:dimgrey; font-family:arial,helvetica; font-size:smaller; font-weight:bold; color:white;">')
        self._reportData.append('Test ID : %s' % testid)
        self._reportData.append('</th>')
        self._reportData.append('</tr>')
        self._reportData.append('</table>')
        self._reportData.append('<table cellspacing="1" border="0" style="width: 100%; border-color:silver">')

        toggle = True
        for name in ['objective', 'testProcedure', 'pass/failCreterial']:
            if toggle:
                bkcolor = self._resource['COLOR']['WHITE']
            else:
                bkcolor = self._resource['COLOR']['SILVER']
            self.InsertRow(name, bkcolor)
            toggle = not toggle

    def InsertRow(self, name, bkcolor, val = None, color = None):
        if color is None:
            color = 'transparent'
        self._reportData.append('<tr style="height: 28px;background-color:%s;font-size:8pt;font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;text-align:center;color: black">' % bkcolor)
        self._reportData.append('<td style="width:10%;font-weight:bold;color:navy">{0}</td>'.format(name))
        try:
            if val is None:
                self._reportData.append('<td style="width:60%;background-color:{1}">{0}</td>'.format(self._configTest[name], color))
            else:
                self._reportData.append('<td style="width:60%;background-color:{1}">{0}</td>'.format(val, color))
        except Exception as e:
            self._reportData.append('<td style="width:60%;background-color:{1}">{0}</td>'.format(str(e), color))

        self._reportData.append('</tr>')
        pass

    def CloseReportRow(self):
        self._reportData.append('</table>')
        self._reportData.append('<br/>')

    def _reportInstalledEntities(self):
        results = self._client.ExecuteCommand(self._shellCommand[0]['command'])
        reportData = []
        rowData = []
        self._insertReportSubHeader('{0} Results'.format(self._configTest['testid']),
                                    self._reportColumns(),
                                    reportData)
        expectedData = self._configTest['expecteddata']
        count = len(re.findall(expectedData[0]['name'], results))
        expectedVal = int(expectedData[0]['value'])
        m = count == expectedVal
        backColor = self._resource['COLOR']['WHITE']
        if m:
            rowData.append(count)
            icon = self._resource['ICON']['CHECK_MARK_BLUE']
        else:
            self._success = False
            id = self._configTest['testid']
            self._parentWorkThread.UpdateResults(id, RESULTS.FAILED)
            icon = self._resource['ICON']['CROSS_RED']
            backColor = self._resource['COLOR']['PINK']
            rowData.append('actual({0}) ----> expect({1})'.format(count, expectedVal))
        rowData.append(icon)
        self._insertSubRow(rowData, reportData, backColor)
        self._flushSubTable('Apach pass UEFI', self._resource['COLOR']['SILVER'], reportData)

    def _insertReportSubHeader(self, title, columns, reportData, appendIcon = True):
        reportData.append('<table cellspacing="0" border="0" cellpadding="0" border="0" style="width: 100%;;text-align:center">')
        reportData.append('<tr style="border:none;">')
        reportData.append(
            '<th colspan="8" style="border:1px; width:100%; height:25px;background-color:rgb(87, 133, 224); font-family:arial,helvetica; font-size:smaller; font-weight:bold; color:white;text-align:center;">')
        reportData.append(title)
        reportData.append('</th>')
        reportData.append('</tr>')
        reportData.append('</table>')

        reportData.append('<table cellspacing="1" border="0" style="width: 100%; border-color:white;font-size:8pt;background-color:{0};text-align:center;">'.format(self._resource['COLOR']['LIGHYELLOW']))
        reportData.append('<tr>')
        if appendIcon:
            width = 98 / len(columns)
            for i in range(0, len(columns) - 1):
                reportData.append('<td style="width:{0}%;font-weight:bold;">{1}</td>'.format(width, columns[i]))
            reportData.append('<td style="width:{0}%;font-weight:bold;">{1}</td>'.format(2, columns[len(columns) - 1]))
        else:
            width = 100 / len(columns)
            for i in range(0, len(columns) - 1):
                reportData.append('<td style="width:{0}%;font-weight:bold;">{1}</td>'.format(width, columns[i]))

        reportData.append('</tr>')
        reportData.append('</table>')
        reportData.append('<table cellspacing="1" border="0" style="width:100%; border-color:silver;font-size:8pt;text-align:center">')

    def _insertSubRow(self, rowdata, reportData, bkcolor, appendIcon = True):
        reportData.append(
            '<tr style="height: 20px;background-color:{0};font-size:8pt;font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;text-align:center;color:black;font-size:8pt">'.format(bkcolor))
        if appendIcon:
            width = 98/len(rowdata)
            for i in range(0, len(rowdata) - 1):
                reportData.append('<td style="width:{1}%;background-color{2};">{0}</td>'.format(rowdata[i], width, bkcolor))
            reportData.append('<td style="width:{0}%;font-weight:bold;">{1}</td>'.format(2, rowdata[len(rowdata) - 1]))
        else:
            width = 100 / len(rowdata)
            for i in range(0, len(rowdata) - 1):
                reportData.append('<td style="width:{1}%;background-color{2};">{0}</td>'.format(rowdata[i], width, bkcolor))

    def _flushSubTable(self, tag, tagColor, reportData):
        reportData.append('</table>')
        reportVal = ''
        for item in reportData:
            reportVal += item
        self.InsertRow(tag, tagColor, reportVal)

    def _insertStatesToWorkflow(self, states):
        q = queue.Queue()
        for state in states:
            q.put(state)

        while True:
            if self._parentWorkThread._queue.qsize() > 0:
                q.put(self._parentWorkThread._queue.get_nowait())
            else:
                break
        self._parentWorkThread._queue = q


    def _setGraceTermination(self, message = None):
        from workstates.systemControlState.systemControlState import SystemControlState
        from workstates.aepCommand.aepCommandState import AEPCommnadState
        from workstates.systemControlState.PowerSwitchState.powerSwitchState import PowerSwitchState
        ids = []
        q = queue.Queue()
        while True:
            if self._parentWorkThread._queue.qsize() > 0:
                qitem = self._parentWorkThread._queue.get_nowait()
            else:
                break
            if not (isinstance(qitem, SystemControlState)
                    or isinstance(qitem, PowerSwitchState)
                    or isinstance(qitem, AEPCommnadState)):
                q.put(qitem)
                qitem._configTest['skip'] = True
                ids.append(qitem._configTest['testid'])
        self._parentWorkThread._queue = q
        for id in ids:
            self._parentWorkThread.UpdateResults(id, RESULTS.SKIPPED)

        if message is not None:
            self.ParentWorkThread.NotificationMessage(message)
        pass

    def _reportColumns(self):
        columns = [x['name'] for x in self._configTest['expecteddata'] if True]
        columns.append('Mached')
        return columns

    def _initializeTemplate(self):
        expectedDictionary = {}
        reportData = []

        for e in self._configTest['expecteddata']:
            dic = {}
            dic[e['value']] = 0
            expectedDictionary[e['name']] = dic

        colunms = ['Expected', 'Value', 'Pass/Fail']
        self._insertReportSubHeader('{0} Results'.format(self._configTest['testid']),
                                    colunms,
                                    reportData)
        return expectedDictionary, reportData

    @abstractmethod
    def ReportHealthStatus(self):
        raise NotImplementedError("users must implement the DoWork method!")

    @overrides(WorkState)
    def DoWork(self):
        if self._abort or (self._configTest is not None and self._configTest['skip']):
            return
        self._shellCommand = self._configTest['shellCommand']
        self.InsertReportHeader(self._configTest['testid'])
        self.ReportHealthStatus()
        self.CloseReportRow()
        result = RESULTS.FAILED
        if self._success:
            result = RESULTS.PASSED

        self._parentWorkThread.UpdateResults(self._configTest['testid'], result)
        self._logger.info('Execute {0} result = {1}'.format(self._configTest['testid'], result.value))

        if self._configTest['powercycle']:
            from workstates.systemControlState.initialParamicoClientState import InitialParamicoClientState
            from workstates.systemControlState.powerCycleState import PowerCycleState
            self._insertStatesToWorkflow([PowerCycleState(),InitialParamicoClientState()])

    def __call__(self):
        if self._parentWorkThread.SocketTableDictionary is None or len(
                self._parentWorkThread.SocketTableDictionary) == 0:
            raise LookupError('To run UEFI_001 is required before to run this test')