#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
#
# Created:     07/20/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from perfTuning import PerfTuning

class ChangeBiosKnobSettingsState(PerfTuning):
    @overrides(PerfTuning)
    def __init__(self, *args, **kwargs):
        super(ChangeBiosKnobSettingsState, self).__init__(*args, **kwargs)
        self._knobsToSet = list()
        if args is not None and len(args) > 1:
            settingcomb = args[0]
            for item in args[1]:
                knobName = item[0]
                biosKnob = settingcomb.BiosKnob(knobName)
                biosKnob.SelectedValue = item[1]
                self._knobsToSet.append(biosKnob)

    @overrides(PerfTuning)
    def DoWork(self):
        ''' TODO: loop and set BIOS values'''
        for knob in self._knobsToSet:
            self._logger.info(knob)
        pass

    def __str__(self):
        return 'Change Bios Knob {} to {}'.format(self._biosKnob.BiosLocation,
                                                  self._biosKnob.SelectedValue)