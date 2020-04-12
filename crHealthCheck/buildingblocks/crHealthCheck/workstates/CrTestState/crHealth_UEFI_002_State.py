from buildingblocks.utils import overrides
from workstates.CrTestState.crTestState import CrTestState

class CrHealth_UEFI_002_State(CrTestState):
    @overrides(CrTestState)
    def ReportHealthStatus(self):
        pass