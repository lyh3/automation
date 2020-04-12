#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho
#
# Created:     26/08/2017
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
import sys, os
sys.path.append(os.path.split(os.path.dirname(os.getcwd()))[0])
from abc import abstractmethod
from threading import Thread
from time import sleep
import os as os
import sys
import time
import buildingblocks.utils as util
from buildingblocks.eventHandler import EventHandler
from buildingblocks.Definitions import Consts


class WorkthreadMetaClass(type):
    def __new__(cls, name, parents, dct):
        if 'class_id' not in dct:
            dct['class_id'] = name.lower()
        return super(WorkthreadMetaClass, cls).__new__(cls, name, parents, dct)

    def __get__(self, obj, objtype):
        """Support instance methods."""
        import functools
        return functools.partial(self.__call__, obj)


class WorkThread:#abstract base class
    __metaclass__ = WorkthreadMetaClass
    file = __file__

    def __init__(self):
        self._thread = None#Thread(target = self.WorkerProcess, args = [None])
        self._isRunning = False
        self._recurringInterval = util.DefaultRecurringInterval
        self._manufactureId = "0x57 0x01 0x00"  # the manufactual ID for Intel Inc.
        '''
            NOTE: if the timeout value < 0, the life time for a work thread is infinit
        '''
        self._timeout = -1
        self._biosConfig = None
        self._invokeFactory = None

    def GetInvokeFactory(self):
        return self._invokeFactory

    def GetNanufactureId(self):
        return self._manufactureId
    def SetManufactureId(self, val):
        self._manufactureId = val
    def GetRecurringInterval(self):
        return self._recurringInterval
    def SetRecurringInterval(self, val):
        self._recurringInterval = val
    def GetTimeout(self):
        return self._timeout
    def SetTimeout(self, val):
        self._timeout = val

    @abstractmethod
    def IntialWork(self):
        raise NotImplementedError("user must implemente the IntialWork.")
    @abstractmethod
    def StateFactory(self, workState = None):
        raise NotImplementedError("user must implement the StateFatory.")

    @property
    def IsRuning(self):
        return self._isRunning

    def Setup(self, config):
        try:
            if config is not None:
                s = config["timeout"]
                if s is not None and s.strip() != '':
                    self.SetTimeout(int(s))
                clsname = [type(self).__name__.replace('thread', '')]
                if clsname in config["log"].keys():
                    logfileName = config["log"][clsname]
                    if logfileName is not None and logfileName.strip() != '':
                        logpath = config["log"]["path"]
                        if logpath is not None and logpath != "current":
                            logfileName = os.path.join(logpath, logfileName)
                        self.SetLogFileName(logfileName)
                if config["recurring"] is not None:
                        recurring = config["recurring"].strip()
                        if recurring != '':
                            self._recurringInterval = int(recurring, 16)
                if config["manufactureid"] is not None:
                    self._manufactureId = config["manufactureid"].strip()
        except:pass

    def Start(self):
        try:
            self._thread = Thread(target = self.WorkerProcess)
            if self._thread is not None:
                self._isRunning = True
                self._thread.start()
                #self._thread.join()
        except:
            type_, value_, traceback_ = sys.exc_info()
            print("type: {0}, value: {1}, traceback: {2}".format(type_, value_, traceback_))

    def Stop(self):
        if self._thread is not None:
            self._isRunning = False
            self._thread.do_run = False

    def WorkerProcess(self):
        startTime = time.time()
        while (self._isRunning):
            state = self.StateFactory()
            if state is not None:
                self.ExecuteState(state)
            sleep(self._recurringInterval)
            elapsedTime = time.time() - startTime
            if self._timeout > 0 and elapsedTime > self._timeout:
                print("timeout {0} (sec) reached, stop now.".format(self._timeout))
                self.Stop()
                break


    def ExecuteState(self, state):
        if state is None:
            return
        try:
            EventHandler().addEvent(Consts.STATE_COMPLETE_EVENT, self.onStateComplete)
            state.Excute()
        except:
            type_, value_, traceback_ = sys.exc_info()
            print("type: {0}, value: {1}, traceback: {2}".format(type_, value_, traceback_))
            pass
        finally:
            pass

    def onStateComplete(self, sender):
        try:
            state = self.StateFactory(sender)
            if state is not None:
                self.ExecuteState(state)
        finally:
            EventHandler().removeEvent(Consts.STATE_COMPLETE_EVENT)
            pass
'''
def threadTestFunction(arg):
    for i in range(arg):
        print ("{0} : running".format(i))
        sleep(1)

if __name__ == '__main__':
    thread = Thread(target = threadTestFunction, args = (10,))
    thread.start()
    thread.join()
    print "thread finished...exiting"
'''
