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
from workstates.aepCommand.aepCommandState import AEPCommand_Type,IpmCtlCommandState

class aepCommand_ShowDimm_State(IpmCtlCommandState):
    @overrides(IpmCtlCommandState)
    def __init__(self):
        super(IpmCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.IPMCTL
        self._commandKey = AEPCommand_Type.ShowDimm

    @overrides(IpmCtlCommandState)
    def CallAEPCommand(self):
        try:
            command = self._formatCommand()
            return self._client.ExecuteCommand(command)
        except Exception as e:
            self._success = False
            self._logger.error(str(e))