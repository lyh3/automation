#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from collections import OrderedDict
from buildingblocks.Definitions import BiosKnobEnum
from itertools import combinations


class SocketKnob:
    def __init__(self):
        self.__dict__ = OrderedDict()
        for i in range(1, len(BiosKnobEnum) + 1):
            comb = list(combinations(BiosKnobEnum, i))
            self.__dict__.__setitem__(i, comb)

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

    def getCaption(self, key, index):
        comb = self[key]
        if comb is not None:
            ret = ''
            for x in comb:
                ret = '{}_{}'.format(ret, x)
            return 'Combination_{}_{} : {}'.format(key, index, ret.lstrip('_'))
        return None


