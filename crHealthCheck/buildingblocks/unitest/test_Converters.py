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
from unittest import TestCase
from buildingblocks.converters.LengthConverter import LengthConverter


class TestConverters(TestCase):
    def test_LengthConverter(self):
        l = LengthConverter()
        l.meter = 1.0
        self.assertEquals(1.0, round(l.meter, 1))
        self.assertEquals(100.0, round(l.centMeter, 1))
        self.assertEquals(1000.0, round(l.milMeter, 1))
        self.assertEquals(0.001, round(l.kiloMeter, 3))
        self.assertEquals(3.2808, round(l.foot, 4))
        self.assertEquals(1.0936, round(l.yard, 4))
        self.assertEquals(39.37, round(l.inch, 2))
        self.assertEquals(0.0006214, round(l.mile, 7))
        l.centMeter = 10
        l.milMeter = 10
        l.kiloMeter = 10

        l.foot = 1.0

        self.assertEquals(0.3048, round(l.meter, 4))
        self.assertEquals(30.48, round(l.centMeter, 2))
        self.assertEquals(304.8, round(l.milMeter, 1))
        self.assertEquals(0.0003048, round(l.kiloMeter, 7))
        self.assertEquals(1.0, round(l.foot, 1))
        self.assertEquals(0.3333, round(l.yard, 4))
        self.assertEquals(12.0, round(l.inch, 1))
        self.assertEquals(0.0001894, round(l.mile, 7))
        l.yard = 10
        l.inch = 10
        l.mile = 10
