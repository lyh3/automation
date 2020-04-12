#-------------------------------------------------------------------------------
# This file contains 'Framework Code' and is licensed as such
# under the terms of your license agreement with Intel or your
# vendor. This file may not be modified, except as allowed by
# additional terms of your license agreement.
#
## @file
#
# Copyright (c) 2019, Intel Corporation. All rights reserved.
# This software and associated documentation (if any) is furnished
# under a license and may only be used or copied in accordance
# with the terms of the license. Except as permitted by such
# license, no part of this software or documentation may be
# reproduced, stored in a retrieval system, or transmitted in any
# form or by any means without the express written consent of
# Intel Corporation.
#-------------- -----------------------------------------------------------------
from unittest import TestCase
from uefi_firmware import AutoParser


class test_uefi_firmware(TestCase):
    def test_parseimage(self):
        with open('IfwiSample.bin', 'rb') as biosimage:
            content = biosimage.read()
        parser = AutoParser(content)
        if parser.type() != 'unknown':
            fd = parser.parse()
            #fd.dump('SpiOemDump')
            fd.showinfo()
