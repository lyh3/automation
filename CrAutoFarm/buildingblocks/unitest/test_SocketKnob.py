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
from unittest import TestCase
from AEPDatamodels.socketKnob import SocketKnob
from AEPDatamodels.biosSettingsComb import BiosSettingsComb
from math import factorial

class TestSocketKnob(TestCase):
    def test_socketKnob(self):
        socketKnob = SocketKnob()
        self.assertFalse(socketKnob is None)
        combs = socketKnob.__dict__.items()
        count = len(combs)
        for k, v in combs:
            self.assertTrue(factorial(count) / factorial(count - k) / factorial(k), len(v))

    def test_getCaptionSuccess(self):
        socketKnob = SocketKnob()
        self.assertTrue(socketKnob.getCaption(2, 0).startswith('Comb_'))
        self.assertTrue(socketKnob.getCaption(2, 1).startswith('Comb_'))

        self.assertTrue(socketKnob.getCaption(1, 0).startswith('Comb_'))

    def test_getCaptionFailed(self):
        socketKnob = SocketKnob()
        combs = socketKnob.__dict__.items()
        self.assertTrue(socketKnob.getCaption(len(combs) + 1, 0) is None)
        self.assertTrue(socketKnob.getCaption(len(combs), 1) is None)

    def test_biosSettingComb(self):
        settingcomb = BiosSettingsComb()
        item = settingcomb[0]
        cpation = settingcomb.Caption(0)
        pass
