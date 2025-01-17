#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho
#
# Created:     26/08/2017
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------- -----------------------------------------------------------------

from abc import abstractmethod
from buildingblocks.eventHandler import EventHandler
import buildingblocks.utils as util

class WorkstateMetaClass(type):
    def __new__(cls, name, parents, dct):
        # create a class_id if it's not specified
        if 'class_id' not in dct:
            dct['class_id'] = name.lower()
        # we need to call type.__new__ to complete the initialization
        return super(WorkstateMetaClass, cls).__new__(cls, name, parents, dct)

class WorkState:# abstract base class
    __metaclass__ = WorkstateMetaClass
    file = __file__

    def __init__(self):
        self._success = True
        self._id = util.IdGenerator()
        self._parentWorkThread = None
        self._manufactureId = None
        self._configTest = None
        self._outfile = None
        self._invokeFactory = None

    def __str__(self):
        return repr("WorkState_" + self._id)

    def GetInvoke(self, device=None):
        if self._invokeFactory is None:
            return None
        invoke =  self._invokeFactory.DefaultDevice()
        if not device is None:
            invoke = self._invokeFactory.Get(device)
        return invoke

    @property
    def ParentWorkThread(self):
        return self._parentWorkThread

    def GetSuccess(self):
        self._success
    def SetSuccess(self, val):
        self._success = val
    def GetParentWorkThread(self):
        return self._parentWorkThread
    def SetParentWorkThread(self, val):
        if val is not None and val.class_id.lower().endswith('thread'):
            self._parentWorkThread = val
            #self._manufactureId = val._manufactureId
            self._invokeFactory = val.GetInvokeFactory()
            self._config = val._config

    def Excute(self):
        try:
            self.DoWork()
        except Exception as e:
            print "!!!! error at Excute, error %s" % str(e)
            self._success = False
        finally:
            EventHandler.callback(self)

    def LogMessage(self, msg):
        print (msg)
        if self._outfile is not None:
            self._outfile.write(msg)
            self._outfile.flush()

    @abstractmethod
    def DoWork(self):
        raise NotImplementedError("users must implement the DoWork method!")
