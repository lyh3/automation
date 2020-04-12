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
from itertools import combinations
from buildingblocks.Definitions import BiosKnobEnum
from collections import OrderedDict
from buildingblocks.automationConfig import AutomationConfig
from biosKnob import BiosKnob
from collections import Counter
from functools import reduce
from itertools import product
import os


class BiosSettingsComb:
    def __init__(self, configJsonPath=None):
        self.__dict__ = OrderedDict()
        self._biosknobDict = OrderedDict()
        pairs = list()
        jsonpath = configJsonPath
        if jsonpath is None:
            jsonpath = os.path.realpath(r'../../Json/PerfTuning.json')
        self._config = AutomationConfig(jsonpath)
        for item in self._config.BiosKnobs.items():
            biosKnob = BiosKnob(item[0], item[1]['knobParameters'])
            self._biosknobDict[biosKnob.KnobName] = biosKnob
            elem = list()
            for val in biosKnob.Values:
                tup = (biosKnob.KnobName, val)
                #if tup not in pairs:
                    #pairs.append(tup)
                elem.append(tup)
            pairs.append(elem)


        #combs = list(combinations(pairs, len(self._config.BiosKnobs.items())))

        #knobcombs = [x for x in combs
        #             if not len([n for n in Counter([n[0] for n in x]).items() if n[1] > 1]) > 0]
        knobcombs = list(product(*pairs))
        for i in range(0, len(knobcombs)):
            self.__dict__.__setitem__(i, knobcombs[i])

    def __getitem__(self, key):
        for k, v in self.__dict__.items():
            if k == key:
                return v
        return None

    def __iter__(self):
        for key in self.__dict__.keys():
            node = self.__dict__[key]
            for x in node.keys():
                yield node[x]

    @property
    def Config(self):
        return self._config

    def BiosKnob(self, knobName):
        if knobName in self._biosknobDict.keys():
            return self._biosknobDict[knobName]
        return None

    def Caption(self, index):
        comb = self[index]
        if comb is not None:
            return reduce(lambda x, y:'{}_{}'.format(x, y), map(lambda x:'{}[{}]'.format(x[0], x[1]), comb))
        return ''