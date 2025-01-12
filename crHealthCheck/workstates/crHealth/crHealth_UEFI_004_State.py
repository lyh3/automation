#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho,
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from workstates.Template.crHealth_BiosLogDependencyCheck_State import crHealth_BiosLogDependencyCheck_State
import re

class crHealth_UEFI_004_State(crHealth_BiosLogDependencyCheck_State):
    @overrides(crHealth_BiosLogDependencyCheck_State)
    def _lookupConverter(self, dimm):
        return dimm.DimmId

    @overrides(crHealth_BiosLogDependencyCheck_State)
    def _lookup(self, lookup, source):
        return len(re.findall(lookup, source)) > 0
