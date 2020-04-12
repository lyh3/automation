from buildingblocks.utils import overrides
from workstates.Template.crHealth_BiosLogDependencyCheck_State import CrHealth_BiosLogDependencyCheck_State
import re

class CrHealth_UEFI_004_State(CrHealth_BiosLogDependencyCheck_State):
    @overrides(CrHealth_BiosLogDependencyCheck_State)
    def _lookupConverter(self, dimm):
        return dimm.DimmId

    @overrides(CrHealth_BiosLogDependencyCheck_State)
    def _lookup(self, lookup, source):
        return len(re.findall(lookup, source)) > 0
