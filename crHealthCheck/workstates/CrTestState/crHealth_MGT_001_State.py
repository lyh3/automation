from buildingblocks.utils import overrides
from workstates.Template.crHealth_BiosLogDependencyCheck_State import CrHealth_BiosLogDependencyCheck_State

class CrHealth_MGT_001_State(CrHealth_BiosLogDependencyCheck_State):
    @overrides(CrHealth_BiosLogDependencyCheck_State)
    def _lookupConverter(self, dimm):
        return dimm.TopologyLocater()

    @overrides(CrHealth_BiosLogDependencyCheck_State)
    def _lookup(self, lookup, source):
        producedResults = [x.split('=')[1].strip()
                           for x in source.split('\n') if x.strip().split('=')[0] == 'DeviceLocator']
        return lookup in producedResults


