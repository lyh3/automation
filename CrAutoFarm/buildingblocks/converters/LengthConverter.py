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
from buildingblocks.Definitions import LengthType
from buildingblocks.Definitions import LengthMetric


class Meter(object):
    def __init__(self, value=0.0):
        self.value = value

    def __get__(self, instance, owner):
        return self.value

    def __set__(self, instance, value):
        self.value = value


class CentMeter(object):

    def __get__(self, instance, owner):
        return instance.meter * LengthMetric()[LengthType.CENT_METER]

    def __set__(self, instance, value):
        instance.meter = value / LengthMetric()[LengthType.CENT_METER]


class MilMeter(object):

    def __get__(self, instance, owner):
        return instance.meter * LengthMetric()[LengthType.MIL_METER]

    def __set__(self, instance, value):
        instance.meter = value / LengthMetric()[LengthType.MIL_METER]

class KiloMeter(object):

    def __get__(self, instance, owner):
        return instance.meter * LengthMetric()[LengthType.KILO_METER]

    def __set__(self, instance, value):
        instance.meter = value / LengthMetric()[LengthType.KILO_METER]

class Inch(object):

    def __get__(self, instance, owner):
        return instance.meter * LengthMetric()[LengthType.INCH]

    def __set__(self, instance, value):
        instance.meter = value / LengthMetric()[LengthType.INCH]


class Foot(object):
    def __get__(self, instance, owner):
        return instance.meter * LengthMetric()[LengthType.FOOT]

    def __set__(self, instance, value):
        instance.meter = value / LengthMetric()[LengthType.FOOT]


class Yard(object):

    def __get__(self, instance, owner):
        return instance.meter * LengthMetric()[LengthType.YARD]

    def __set__(self, instance, value):
        instance.meter = value / LengthMetric()[LengthType.YARD]


class Mile(object):

    def __get__(self, instance, owner):
        return instance.meter * LengthMetric()[LengthType.MILE]

    def __set__(self, instance, value):
        instance.meter = value / LengthMetric()[LengthType.MILE]


class LengthConverter(object):
    meter = Meter()
    centMeter = CentMeter()
    milMeter = MilMeter()
    kiloMeter = KiloMeter()
    inch = Inch()
    foot = Foot()
    yard = Yard()
    mile = Mile()
