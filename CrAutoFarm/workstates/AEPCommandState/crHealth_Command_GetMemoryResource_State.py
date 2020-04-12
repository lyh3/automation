from buildingblocks.Decoraters import overrides
from workstates.AEPCommandState.aepCommandState import AEPCommand_Type,IpmCtlCommandState

class CrHealth_Command_GetMemoryResource_State(IpmCtlCommandState):
    @overrides(IpmCtlCommandState)
    def __init__(self):
        super(IpmCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.IPMCTL
        self._commandKey = AEPCommand_Type.GetMemoryResource

    @overrides(IpmCtlCommandState)
    def CallAEPCommand(self):
        try:
            command = self._formatCommand()
            return self._client.ExecuteCommand(command)
        except Exception as e:
            self._success = False
            self._logger.error(str(e))