#-------------------------------------------------------------------------------
# Name:
# Purpose:
#
# Author:      liyingho
#
# Created:     06/03/2019
# Copyright:   (c) Intel Co. 2019
# Licence:     <your licence>
#-------------- -----------------------------------------------------------------
import logging
import os as os
from buildingblocks.utils import InvalidArgumentException

'''
    This class has bas been implemented using the Singleton designs pattern
'''


class AutomationLog(object):
    _instanceAutomationLog = None

    def __new__(cls, *args, **kwargs):
        if not cls._instanceAutomationLog:
            cls._instanceAutomationLog = super(AutomationLog, cls).__new__(cls)
            cwd = os.getcwd()
            fileName = r'automation.log'
            if args.__len__() > 0:
                fileName = args[0] + '.log'
            cls._automationLogfile = os.path.join(cwd, fileName)
            logging.basicConfig(level=logging.DEBUG,
                                format='%(asctime)s %(name)-12s %(levelname)-8s %(message)s',
                                datefmt='%m-%d %H:%M',
                                filename=cls._automationLogfile,
                                filemode='a')
            cls._logDictioanry = {}
        return cls._instanceAutomationLog


    @classmethod
    def GetFileName(cls):
        return cls._automationLogfile


    @classmethod
    def GetFormater(cls):
        return logging.Formatter('%(name)-12s: %(levelname)-8s %(message)s')


    @classmethod
    def GetLogger(cls, name):
        if name is None or name == '':
            return None

        log = cls._logDictioanry.get(name)
        if log is None:
            log = logging.getLogger(name)
            cls._logDictioanry[name] = log
        return log


    @classmethod
    def Close(cls):
        #for key, log in cls._logDictioanry.iteritems():
        for key in cls._logDictioanry:
            log = cls._logDictioanry[key]
            handlers = log.handlers[:]
            for handler in handlers:
                handler.close()
                log.removeHandler(handler)

    @classmethod
    def TryAddConsole(cls, logname):
        try:
            if logname is None or logname not in cls._logDictioanry.keys():
                raise InvalidArgumentException('Cannot find the log from dictionary.')
            console = logging.StreamHandler()
            console.setLevel(logging.INFO)
            formatter = cls.GetFormater()
            console.setFormatter(formatter)
            logging.getLogger(logname).addHandler(console)
        except Exception as e:
            print(str(e))
            raise e
        pass

