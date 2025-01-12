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
from uefi_firmware import AutoParser

if __name__ == '__main__':
    with open('WLYDCRB.86B.BR.64.2019.19.3.03.1837_0011.D13_P80019_LBG_SPS.bin', 'rb') as biosimage:
        content = biosimage.read()
    parser = AutoParser(content)
    if parser.type() != 'unknown':
        fd = parser.parse()
        fd.dump('IFWIDump')
        fd.showinfo()