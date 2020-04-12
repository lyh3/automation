#-------------------------------------------------------------------------------
# Name:
# Purpose:
#
# Author:      liyingho
#
# Created:     07/17/2019
# Copyright:   (c) Intel Co. 2019
# Licence:     <your licence>
#-------------- -----------------------------------------------------------------


class BiosKnob(object):
    def __init__(self, knobName, config):
        self._knobName = knobName
        self._biosLocation = config['BiosLocation']
        self._values =config['value']
        self._selectedValue = None

    @property
    def KnobName(self):
        return self._knobName

    @property
    def BiosLocation(self):
        return self._biosLocation

    @property
    def Values(self):
        return self._values

    @property
    def SelectedValue(self):
        return self._selectedValue

    @SelectedValue.setter
    def SelectedValue(self, val):
        self._selectedValue = val

    def __str__(self):
        return 'Bios knob {} : {}'.format(self._biosLocation, self._selectedValue)
