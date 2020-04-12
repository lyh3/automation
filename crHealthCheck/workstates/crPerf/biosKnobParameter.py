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

class BiosKnobParameter(object):
    def __init__(self, config):
        self._biosLocation = config['BiosLocation']
        self._value = config['value']
        self._selectedValue = None

    @property
    def BiosLocation(self):
        return self._biosLocation

    @property
    def Value(self):
        return self._value

    @property
    def SelectedValue(self):
        return self._selectedValue

    @SelectedValue.setter
    def SelectedValue(self, val):
        self._selectedValue = val

    def __str__(self):
        return 'Bios knob {} : {}'.format(self._biosLocation, self._selectedValue)
