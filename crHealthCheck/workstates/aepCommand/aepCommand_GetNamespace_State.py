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
from workstates.aepCommand.aepCommandState import AEPCommand_Type,NdCtlCommandState

class aepCommand_GetNameSpace_State(NdCtlCommandState):
    @overrides(NdCtlCommandState)
    def __init__(self):
        super(NdCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.NDCTL
        self._commandKey = AEPCommand_Type.GetNameSpace
        pass

    @overrides(NdCtlCommandState)
    def CallAEPCommand(self):
        try:
            command = self._formatCommand()
            return self._client.ExecuteCommand(command)
        except Exception as e:
            self._success = False
            self._logger.error(str(e))