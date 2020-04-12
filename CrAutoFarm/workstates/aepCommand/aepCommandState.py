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
from abc import abstractmethod
from workstates.crHealth.crTestState import CrTestState
from buildingblocks.Decoraters import overrides
from enum import Enum
import os,json,sys
try:
    import queue
except ImportError:
    import Queue as queue

class AEPCommnadState(CrTestState):
    @overrides(CrTestState)
    def __init__(self, *args, **kwargs):
        super(AEPCommnadState, self).__init__(*args, **kwargs)
        self._foreword = None
        self._commandKey = None
        jsonpath = os.path.dirname(__file__).replace('\\workstates', '').replace('aepCommand', '') + r'Json/AEPCommand.json'
        self._configAEPCommand = json.loads(open(jsonpath).read())
        pass

    @overrides(CrTestState)
    def SetParentWorkThread(self, val):
        self._powerSwitch = val._powerSwitch
        self._client = val._client
        super(AEPCommnadState, self).SetParentWorkThread(val)

    def _formatCommand(self):
        return '{} {}'.format(self._foreword.value, self._configAEPCommand[self._foreword.value][self._commandKey.value])

    @overrides(CrTestState)
    def DoWork(self):
        self.CallAEPCommand()

    @staticmethod
    def CreateInstance(testid):
        classname = 'CrHealth_{0}_State'.format(testid)
        namespaces = ['workstates',
                      'crHealth',
                      'crHealth_{0}_State'.format(testid)]
        q = queue.Queue()
        instance = None
        try:
            m = 'crHealthCheck.'
            for x in namespaces :
                m += '{0}.'.format(x)
                q.put(x)
            module = __import__(m)
            instance = getattr(CrTestState.ExtractAttr(module, q), classname)()
        except Exception as e:
            print('Exception caught at CreateInstance, error was :%s' % str(e))
        return instance

    @abstractmethod
    def CallAEPCommand(self):
        raise NotImplementedError("users must implement the CallSystemFunction method!")

    @staticmethod
    def CreateInstance(testid, subfolder = None):
        classname = 'CrHealth_{0}_State'.format(testid)
        namespaces = ['workstates']
        if subfolder is None:
            namespaces.append('crHealth')
        else:
            namespaces.append(subfolder)
        namespaces.append('crHealth_{0}_State'.format(testid))
        '''
        namespaces = ['workstates',
                      'crHealth',
                      'crHealth_{0}_State'.format(testid)]
        '''
        q = queue.Queue()
        instance = None
        try:
            m = 'crHealthCheck.'
            for x in namespaces :
                m += '{0}.'.format(x)
                q.put(x)
            module = __import__(m)
            instance = getattr(CrTestState.ExtractAttr(module, q), classname)()
        except Exception as e:
            print('Exception caught at CreateInstance, error was :%s' % str(e))
        return instance


class IpmCtlCommandState(AEPCommnadState):
    @overrides(AEPCommnadState)
    def __init__(self):
        super(IpmCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.IPMCTL
        pass

    @overrides(AEPCommnadState)
    def CallAEPCommand(self):
        command = self._formatCommand()
        self._client.ExecuteCommand(command)


class NdCtlCommandState(AEPCommnadState):
    @overrides(AEPCommnadState)
    def __init__(self):
        super(IpmCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.NDCTL

    @overrides(AEPCommnadState)
    def CallAEPCommand(self):
        command = self._formatCommand()
        self._client.ExecuteCommand(command)

class AEPCommand_Type(Enum):
    def __str__(self):
        return self.value
    IPMCTL = 'ipmctl'
    NDCTL = 'ndctl'
    CreateADGoal = 'CreateADGoal'
    CreateMMGoal = 'CreateMMGoal'
    CreateNameSpace = 'CreateNameSpace'
    DeleteGoal = 'DeleteGoal'
    DestroyNameSpace = 'DestroyNameSpace'
    GetMemoryResource = 'GetMemoryResource'
    GetNameSpace = 'GetNameSpace'
    GetRegion = 'GetRegion'
    QueryPool = 'QueryPool'
    ShowDimm = 'ShowDimm'


