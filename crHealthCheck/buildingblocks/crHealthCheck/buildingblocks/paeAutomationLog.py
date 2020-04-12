import logging
import os as os

'''
    This class has bas been implemented using the Singleton designs pattern
'''

class PaeAutomationLog(object):
    _instancePaeAutomationLog = None
    def __new__(cls, *args, **kwargs):
        if not cls._instancePaeAutomationLog:
            cls._instancePaeAutomationLog = super(PaeAutomationLog, cls).__new__(cls)
            cwd = os.getcwd()
            fileName = r'paeautomation.log'
            if args.__len__() > 0:
                fileName = args[0] + '.log'
            file = os.path.join(cwd, fileName)
            logging.basicConfig(level=logging.DEBUG,
                                format='%(asctime)s %(name)-12s %(levelname)-8s %(message)s',
                                datefmt='%m-%d %H:%M',
                                filename=file,
                                filemode='a')
            cls._logDictioanry = {}
        return cls._instancePaeAutomationLog


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
            #log.TryAddConsole()
            cls._logDictioanry[name] = log
        return log

    @classmethod
    def Close(cls):
        for key, log in cls._logDictioanry.iteritems():
            handlers = log.handlers[:]
            for handler in handlers:
                handler.close()
                log.removeHandler(handler)

    @classmethod
    def TryAddConsole(cls):
        try:
            console = logging.StreamHandler()
            console.setLevel(logging.INFO)
            formatter = cls.GetFormater()#logging.Formatter('%(name)-12s: %(levelname)-8s %(message)s')
            console.setFormatter(formatter)
            logging.getLogger('').addHandler(console)
        except Exception as e:
            print(str(e))