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
from aepCommand_GetRegion_State import aepCommand_GetRegion_State

class aepCommand_CreateNameSpace_State(NdCtlCommandState):
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
            getregion = aepCommand_GetRegion_State()
            c = self._config[self._foreword.value][self._commandKey.value]
            for region in getregion.CallAEPCommand():
                command = '{} {}'.format(self._foreword.value, c.format(region))
                self._client.ExecuteCommand(command)
        except Exception as e:
            self._success = False
            self._logger.error(str(e))
