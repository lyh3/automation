from buildingblocks.Decoraters import overrides
from workstates.AEPCommandState.aepCommandState import AEPCommand_Type,NdCtlCommandState
from crHealth_Command_GetRegion_State import CrHealth_Command_GetRegion_State

class CrHealth_Command_CreateNameSpace_State(NdCtlCommandState):
    @overrides(NdCtlCommandState)
    def __init__(self):
        super(NdCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.NDCTL
        self._commandKey = AEPCommand_Type.CreateNameSpace
        self._namespace = 'fsdax'

    @property
    def NameSpace(self):
        return self._namespace
    @NameSpace.setter
    def NameSpace(self, val):
        self._namespace = val

    @overrides(NdCtlCommandState)
    def CallAEPCommand(self):
        try:
            getregion = CrHealth_Command_GetRegion_State()
            c = self._config[self._foreword.value][self._commandKey.value]
            for region in getregion.CallAEPCommand():
                command = '{} {}'.format(self._foreword.value, c.format(region))
                self._client.ExecuteCommand(command)
        except Exception as e:
            self._success = False
            self._logger.error(str(e))
