#-------------------------------------------------------------------------------
# This file contains 'Framework Code' and is licensed as such
# under the terms of your license agreement with Intel or your
# vendor. This file may not be modified, except as allowed by
# additional terms of your license agreement.
#
## @file
#

# This software and associated documentation (if any) is furnished
# under a license and may only be used or copied in accordance
# with the terms of the license. Except as permitted by such
# license, no part of this software or documentation may be
# reproduced, stored in a retrieval system, or transmitted in any
# form or by any means without the express written consent of
# Intel Corporation.
#-------------- -----------------------------------------------------------------

import struct
import os

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




        print("  BIOS_SIZE: ", self.BIOS_SIZE)
        print("  BIOS_START_ADDRESS: ", self.BIOS_START_ADDRESS)
        print("  BIOS_END_ADDRESS: ", self.BIOS_END_ADDRESS)

        if not self.IsValid:
            print ("NOTICE: Unsupportable IFWI file.  Aborting process due to invalid input file.")

    @property
    def IsValid(self):
        if self.IFWI_SIZE <= 0 or self.BIOS_SIZE <= 0 or \
           self.IFWI_SIZE <= self.BIOS_SIZE or \
           self.BIOS_END_ADDRESS <= self.BIOS_START_ADDRESS or \
           self.IFWI_SIZE <= self.BIOS_START_ADDRESS or \
           self.IFWI_SIZE < self.BIOS_END_ADDRESS:
            self._isValid = False
