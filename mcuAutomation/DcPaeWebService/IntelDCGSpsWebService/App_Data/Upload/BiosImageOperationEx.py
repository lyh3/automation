import sys
import os
import argparse
import struct


'''
    FLREG1 Flash Region 1 (BIOS) Register
    FRBA + 0x04
    Bit 30:16
    RegionLimit = 0x0000
    RegionBase = 0x7FFF
    Bit 14:0
'''

#-------------------------------------------------------------------------------
# Name:        imageOperationState
# Purpose:
#
# Author:      Henry Li
#
# Created:     26/08/2017
# Copyright:   (c) nathaniel.d.haller@intel.com 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------



'''
    FLREG1 Flash Region 1 (BIOS) Register
    FRBA + 0x04
    Bit 30:16
    RegionLimit = 0x0000
    RegionBase = 0x7FFF
    Bit 14:0
'''


class Flreg1:
    def __init__(self, FRBA, openfileIfwi, logger):
        logger.info("Parsing data at Flash Region Base Address: {}".format(FRBA))
        openfileIfwi.seek(FRBA + 0x04, 0)
        self.RegionBase = openfileIfwi.read(2)
        self.RegionBase = struct.unpack("<H", self.RegionBase)[0]
        logger.info("Flreg1.RegionBase located: {}".format(hex(self.RegionBase)))

        openfileIfwi.seek(FRBA + 0x06, 0)
        self.RegionLimit = openfileIfwi.read(2)
        self.RegionLimit = struct.unpack("<H", self.RegionLimit)[0]
        logger.info("Flreg1.RegionLimit located: {}".format(hex(self.RegionLimit)))


'''
    For information on Flmap0, refer to the SPI Programmer's Guide.
    FLMAP0 Flash Map 0 Register
    # FDBAR + 0x14
    # Bit 23:16.  Multiply by 16 for the actual base address.
    # 1 Byte of Data at FDBAR + 0x16
'''


class Flmap0:
    def __init__(self, FDBAR, openfileIfwi, logger):
        logger.info("Parsing data at Flash Descriptor Base Address: {}".format(hex(FDBAR)))
        openfileIfwi.seek(FDBAR + 0x16, 0)
        self.Frba = openfileIfwi.read(1)
        self.Frba = struct.unpack("<B", self.Frba)[0]
        logger.info("Flmap0.Frba located: {}".format(self.Frba))


'''
    For information on FlashDescriptorContent, refer to the SPI Programmer's Guide.
    Data structure of the Flash Descriptor
    FDBAR is address 0x0 of the SPI flash device on chip select 0.
'''


class FlashDescriptorContent:
    def __init__(self, openfileIfwi, logger):
        self.FDBAR = 0x0000
        logger.info("Parsing IFWI to initialize Flash Descriptor Content...")
        self.flmap0 = Flmap0(self.FDBAR, openfileIfwi, logger)
        self.flreg1 = Flreg1(self.flmap0.Frba * 16, openfileIfwi, logger)


class IfwiImage:
    def __init__(self, IFWI, logger):
        self._logger = logger
        self._isValidImage = False
        with open(IFWI, 'rb') as openfileIfwi:
            ifwi_descriptor = FlashDescriptorContent(openfileIfwi, logger)
            self._biosSize = ((ifwi_descriptor.flreg1.RegionLimit * 4096 + 0xFFF) - (
                        ifwi_descriptor.flreg1.RegionBase * 4096)) + 1
            self._biosStartAddress = (ifwi_descriptor.flreg1.RegionBase * 4096)
            fileinfo = os.stat(IFWI)
            self._ifwiSize = fileinfo.st_size
            self._biosEndAddress = self._biosStartAddress + self._biosSize
            logger.info("IFWI_SIZE: {}, BIOS_SIZE: {}, BIOS_START_ADDRESS: {}, BIOS_END_ADDRESS: {}"
                        .format(self._ifwiSize,
                                self._biosSize,
                                hex(self._biosStartAddress),
                                hex(self._biosEndAddress)))
    @property
    def ImageSize(self):
        return self._ifwiSize

    @property
    def BiosSize(self):
        return self._biosSize

    @property
    def BiosStartAddress(self):
        return self._biosStartAddress

    @property
    def BiosEndAddress(self):
        return self._biosEndAddress

    def IsValidImage(self):

        if not(self._ifwiSize <= 0 or self._biosSize <= 0 or
               self._ifwiSize <= self._biosSize or
               self._biosEndAddress <= self._biosStartAddress or
               self._ifwiSize <= self._biosStartAddress or
               self._ifwiSize < self._biosEndAddress):
            return True
        return False

def _fetchBytes(offset, size, source, target):
    byteCount = 0
    CHUNK = 4096
    source.seek(offset, 0)
    while byteCount < size:
        byte = source.read(CHUNK)
        if byte:
            if len(byte) == CHUNK and (byteCount + CHUNK) <= size:
                target.write(byte)
                byteCount += CHUNK
            else:
                for b in byte:
                    try:
                        target.write(b)  # Python 2.7
                    except TypeError:
                        target.write(bytes([b]))  # Python 3
                    byteCount += 1
                    if byteCount == size:
                        break
    return byteCount

def extractBios(settings, logger):
    logger.info("Extract_BIOS process start ...")
    bkcIfwi = IfwiImage(settings.IFWI, logger)
    with open(settings.output, 'wb') as outputfile:
        outputfile.truncate()
        with open(settings.IFWI, 'rb') as openfileIfwiTemplate:
            _fetchBytes(bkcIfwi.BiosStartAddress,bkcIfwi.BiosSize, openfileIfwiTemplate, outputfile)


def mergeIfwi(settings, logger):
    logger.info("Merge_IFWI proceedure start ...")
    bkcIfwi = IfwiImage(settings.IFWI, logger)
    externalIfwi = IfwiImage(settings.BIOS, logger)

    logger.info("bkcIfwi BIOS Size : {}, BIOS Start Address : {}, IFWI size : {}".format(bkcIfwi.BiosSize,
                                                                                         hex(bkcIfwi.BiosStartAddress),
                                                                                         bkcIfwi.ImageSize))
    logger.info("externalIfwi BIOS Size :{}, BIOS Start Address : {}, IFWI size: {}".format(externalIfwi.BiosSize,
                                                                                            hex(externalIfwi.BiosStartAddress),
                                                                                            externalIfwi.ImageSize))
    if bkcIfwi.BiosSize != externalIfwi.BiosSize:
        raise Exception("Input IFWI files have different BIOS sizes. Aborting merge IFWI attempt."
                     .format(settings.IFWI, settings.BIOS))

    bkcIfwi = IfwiImage(settings.IFWI, logger)
    externalIfwi = IfwiImage(settings.BIOS, logger)

    with open(settings.output, 'wb') as outputfile:
        outputfile.truncate()
        with open(settings.IFWI, 'rb') as openfileIfwiTemplate:
            bytes_written = _fetchBytes(0, bkcIfwi.BiosStartAddress, openfileIfwiTemplate, outputfile)
            logger.info("bytes_written: {}".format(bytes_written))
            with open(settings.BIOS, 'rb') as openfileWorkaround:
                bytes_written = _fetchBytes(externalIfwi.BiosStartAddress,
                                                              externalIfwi.BiosEndAddress - externalIfwi.BiosStartAddress,
                                                              openfileWorkaround, outputfile)
                logger.info("bytes_written: {}".format(bytes_written))

            bytes_written = _fetchBytes(bkcIfwi.BiosEndAddress, bkcIfwi.ImageSize - bkcIfwi.BiosEndAddress,
                                                          openfileIfwiTemplate, outputfile)
            logger.info("bytes_written: {}".format(bytes_written))


def biosSwap(settings, logger):
    logger.info("BIOS_Swap proceedure start ...")
    bkcIfwi = IfwiImage(settings.IFWI, logger)
    fileinfo = os.stat(settings.BIOS)
    biosSize = fileinfo.st_size

    if bkcIfwi.BiosSize != biosSize:
        raise Exception("Input IFWI [{}] and Input BIOS [{}]have different BIOS sizes. Aborting BIOS swap attempt.".format(settings.IFWI, settings.BIOS))

    with open(settings.output, 'wb') as outputfile:
        outputfile.truncate()
        with open(settings.IFWI, 'rb') as openfileIfwiTemplate:
            bytes_written = _fetchBytes(0, bkcIfwi.BiosStartAddress, openfileIfwiTemplate, outputfile)
            logger.info("bytes_written: {}".format(bytes_written))
            with  open(settings.BIOS, 'rb') as openfileWorkaround:
                bytes_written = _fetchBytes(0, biosSize, openfileWorkaround, outputfile)
                logger.info("bytes_written: {}".format( bytes_written))
            bytes_written = _fetchBytes(bkcIfwi.BiosEndAddress, bkcIfwi.ImageSize - bkcIfwi.BiosEndAddress,
                                                          openfileIfwiTemplate, outputfile)
            logger.info("bytes_written: {}".format(bytes_written))

import logging
'''
    This class has bas been implemented using the Singleton designs pattern
'''


class AutomationLog(object):
    _instancePaeAutomationLog = None

    def __new__(cls, *args, **kwargs):
        if not cls._instancePaeAutomationLog:
            cls._instancePaeAutomationLog = super(AutomationLog, cls).__new__(cls)
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
        return cls._instancePaeAutomationLog


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
                raise Exception('Cannot find the log from dictionary.')
            console = logging.StreamHandler()
            console.setLevel(logging.INFO)
            formatter = cls.GetFormater()
            console.setFormatter(formatter)
            logging.getLogger(logname).addHandler(console)
        except Exception as e:
            print(str(e))
            raise e
        pass

if __name__ == "__main__":

    if sys.version_info < (2, 6):
        sys.exit(1)
    logName = 'BiosImageOperation'
    automationlog = AutomationLog(logName)
    logger = automationlog.GetLogger(logName)
    automationlog.TryAddConsole(logName)

    _queue = None
    parser = argparse.ArgumentParser()

    parser.add_argument('-ifwi', action='store', dest='IFWI', help="IFWI image file path", default=None)
    parser.add_argument('-bios', action='store', dest='BIOS', help="bios update image file path", default=None)
    parser.add_argument('-o', action='store', dest='output', help="Output image file path", default='outimage.bin')
    parser.add_argument('-f', choices=['extractBios', 'mergeBios', 'swapBios'], action='store', dest='selection',
                        help="Function select", default='extractBios')

    try:
        args = parser.parse_args()
        if args.selection == 'extractBios':
            if args.IFWI is None:
                print('The IFWI must be provided.')
                sys.exit(1)
            extractBios(args, logger)
        elif args.selection == 'mergeBios':
            if args.IFWI is None or args.BIOS is None:
                print('The IFWI and BIOS must be provided.')
                sys.exit(1)
            mergeIfwi(args, logger)
        elif args.selection == 'swapBios':
            if args.IFWI is None or args.BIOS is None:
                print('The IFWI and BIOS must be provided.')
                sys.exit(1)
            biosSwap(args, logger)
    except Exception as e:
        logger.error('Error caught from BiosImageOperation, error = {}'.format(str(e)))



