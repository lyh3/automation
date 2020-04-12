from collections import OrderedDict
from json import JSONDecoder
from buildingblocks.Definitions import Consts
from buildingblocks.utils import IdGenerator
import json, os


class CombInfoDictionary(object):
    def __init__(self, jsonString = None):
        self.__dict__ = OrderedDict()
        if jsonString is not None:
            decoder = JSONDecoder(object_hook=OrderedDict)
            self.__dict__ = decoder.decode(jsonString)

    def __getitem__(self, key):
        for k, v in self.__dict__.items():
            if k == key:
                return v
            if type(v) is dict:
                ret = self._getitem(key, v)
                if ret is not None:
                    return ret
            elif type(v) is list:
                for x in v:
                    ret = self._getitem(key, x)
                    if ret is not None:
                        return ret
        return None

    def __setitem__(self, key, val):
        if key not in self.__dict__.keys():
            self.__dict__[key] = val

    def __delitem__(self, key):
        if key in self.__dict__.keys():
            self.__dict__.remove(key)

    def __iter__(self):
        for key in self.__dict__.keys():
            node = self.__dict__[key]
            for x in node.keys():
                yield node[x]

    def __str__(self):
        return json.dumps(self, default=lambda x: x.__dict__, sort_keys=False, indent=4)

    def Save(self):
        if not os.path.isdir(Consts.COMB_OUTPUT_FOLDER):
            os.mkdir(Consts.COMB_OUTPUT_FOLDER)
        jsonpath = os.path.realpath(r'{}/CombInfo-{}.json'.format(Consts.COMB_OUTPUT_FOLDER, IdGenerator()))
        with open(jsonpath, 'w') as outputfile:
            outputfile.write(str(self))
