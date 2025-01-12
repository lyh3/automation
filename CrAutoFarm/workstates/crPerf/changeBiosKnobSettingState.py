#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho
#
# Created:     07/20/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from workstates.crPerf.perfTuning import PerfTuning
from buildingblocks.Decoraters import callCount
import time


class ChangeBiosKnobSettingsState(PerfTuning):
    @overrides(PerfTuning)
    def __init__(self, *args, **kwargs):
        super(ChangeBiosKnobSettingsState, self).__init__(*args, **kwargs)
        self._knobsToSet = list()
        if args is not None:
            if len(args) > 1:
                settingcomb = args[0]
                for item in args[1]:
                    knobName = item[0]
                    biosKnob = settingcomb.BiosKnob(knobName)
                    biosKnob.SelectedValue = item[1]
                    self._knobsToSet.append(biosKnob)
            if len(args) > 2:
                self._itpInstance = args[2][0]
                self._cliInstance = args[2][1]



    @callCount
    @overrides(PerfTuning)
    def DoWork(self):
        ''' TODO: loop and set BIOS values'''
        strParam = ""
        for knob in self._knobsToSet:
            self._logger.info(knob)
            strParam += "{0}={1},".format(knob.KnobName, knob.SelectedValue)
        self._logger.info('------ ChangeBiosKnobSettingsState counter = {} -------'.format(self.DoWork.count))
        #self._cliInstance.CvReadKnobs(strParam)
        #self._itpInstance.resettarget()
        self._parentWorkThread._client.ExecuteCommand('python //root//SvBios//tweakbiosTarget.py -p {}'.format(strParam))
        time.sleep(150)  # give extra 150 secs for the system to reboot

    def __str__(self):
        return 'Change Bios Knob {} to {}'.format(self._biosKnob.BiosLocation,
                                                  self._biosKnob.SelectedValue)