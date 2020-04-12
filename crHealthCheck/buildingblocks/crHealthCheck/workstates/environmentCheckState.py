from buildingblocks.workflow.workstate import WorkState
from buildingblocks.utils import overrides


class EnvironmentCheckState(WorkState):
    @overrides(WorkState)
    def DoWork(self):
        pass