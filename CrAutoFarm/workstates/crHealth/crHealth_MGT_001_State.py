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
from buildingblocks.Decoraters import overrides
from workstates.Template.crHealth_BiosLogDependencyCheck_State import crHealth_BiosLogDependencyCheck_State

class crHealth_MGT_001_State(crHealth_BiosLogDependencyCheck_State):
    @overrides(crHealth_BiosLogDependencyCheck_State)
    def _lookupConverter(self, dimm):
        return dimm.TopologyLocater()

    @overrides(crHealth_BiosLogDependencyCheck_State)
    def _lookup(self, lookup, source):
        producedResults = [x.split('=')[1].strip()
                           for x in source.split('\n') if x.strip().split('=')[0] == 'DeviceLocator']
        return lookup in producedResults


