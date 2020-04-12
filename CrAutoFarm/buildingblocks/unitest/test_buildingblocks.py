#-------------------------------------------------------------------------------
# Name:
# Purpose:
#
# Author:      liyingho
#
# Created:     06/03/2019
# Copyright:   (c) Intel Co. 2019
# Licence:     <your licence>
#-------------- -----------------------------------------------------------------
from unittest import TestCase, expectedFailure
import os
from buildingblocks.Definitions import LengthType
from buildingblocks.Definitions import LengthMetric
from buildingblocks.utils import InvalidArgumentException, Position
from buildingblocks.AutomationLog import AutomationLog


class TestBuildingblocks(TestCase):
    def setUp(self):
        self._logname = 'automationTest'
        self._automationlogInstance = AutomationLog(self._logname)
        self._logFilename = self._automationlogInstance.GetFileName()
        self.assertTrue(os.path.isfile(self._logFilename))
        self._logger = self._automationlogInstance.GetLogger(self._logname)
        self._automationlogInstance.TryAddConsole(self._logname)
        self.assertFalse(self._logger is None)

    def tearDown(self):
        pass

    def testAutomationLog(self):
        self._logger.info('I am feeling lucky today.')
        if os.path.isfile(self._logFilename):
            try:
                self._automationlogInstance.Close()
                os.remove(self._logFilename)
            except IOError as ex:
                print('Error caugt at tearDown of text_buildingblocks : {}'.format(str(ex)))

    def testAutomationLogGetLogger(self):
        logger = self._automationlogInstance.GetLogger(self._logname)
        self.assertFalse(logger is None)
        self.assertEquals(logger, self._logger)

    def testAutomationCloseLog(self):
        self._automationlogInstance.Close()

    def testPosition(self):
        pos = Position()
        self.assertEquals(0, pos.x)
        self.assertEquals(0, pos.y)
        self.assertEquals(0, pos.z)
        pos.x = pos.y = pos.z = 5
        self.assertEquals(5, pos.x)
        self.assertEquals(5, pos.y)
        self.assertEquals(5, pos.z)

    @expectedFailure
    def testAutomationAddConsoleFail(self):
        self._automationlogInstance.TryAddConsole('')

    def testAutomationLogGetLoggerFail(self):
        logger = self._automationlogInstance.GetLogger(None)
        self.assertTrue(logger is None)
        logger = self._automationlogInstance.GetLogger('')
        self.assertTrue(logger is None)

    def testLengthMetricSucces(self):
        f = LengthMetric()[LengthType.FOOT]
        self.assertEquals(3.28084, f)

    @expectedFailure
    def testLengthMetricSFailedWithWrongIndex(self):
        self.assertRaises(LengthMetric()["Hello"], InvalidArgumentException)

    @expectedFailure
    def testLengthMetricSFailedWithNoneIndex(self):
        self.assertRaises(LengthMetric()[None], InvalidArgumentException)


