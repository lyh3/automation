
from buildingblocks.workflow.workstate import WorkState
from buildingblocks.utils import overrides

class TerminateState(WorkState):
    @overrides(WorkState)
    def DoWork(self):
        pass