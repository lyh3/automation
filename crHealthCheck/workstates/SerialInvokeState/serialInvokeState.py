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
import sys,os,json
sys.path.append(os.path.split(os.path.dirname(os.getcwd()))[0])
sys.path.append(os.path.dirname(__file__))
from abc import abstractmethod
from workstates.crHealth.crTestState import CrTestState
from buildingblocks.Decoraters import overrides

class SerialInvokeState(CrTestState):
    @overrides(CrTestState)
    def __init__(self, port,
                       baud,
                       key=None,
                       trims = [],
                       timeout = '400',
                       recur = 1,
                       jsonPath = None):
        super(SerialInvokeState, self).__init__()
        self._port = port
        self._baud = baud
        jsonpath = os.path.realpath(r'../crHealthCheck/Json/SerialInvoke.json')
        if jsonPath is not None:
            jsonpath = jsonPath
        self._invokLookup = (json.loads(open(jsonpath).read()))
        self._keystroke = None
        self._recur = recur
        if key is not None and key != '':
            for k in self._invokLookup['serialInvoke']:
                if key == k.keys()[0]:
                    self._keystroke = k[k.keys()[0]]
        self._trims = trims
        self._timeout = timeout
        self._waitFor = None
        '''
            members for SystemControlState, even they are not been used
        '''
        self._logger = None
        self._abort = True

    @overrides(CrTestState)
    def DoWork(self):
        self.SerialInvoke()

    @abstractmethod
    def SerialInvoke(self):
        raise NotImplementedError("users must implement the SerialInvoke method!")
