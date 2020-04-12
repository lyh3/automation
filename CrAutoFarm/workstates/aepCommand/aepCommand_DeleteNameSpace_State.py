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

class aepCommand_DeleteNameSpace_State(NdCtlCommandState):
    @overrides(NdCtlCommandState)
    def __init__(self):
        super(NdCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.NDCTL
        self._commandKey = AEPCommand_Type.DestroyNameSpace
        self._nameSpace = 'all'#'fsdax'

    @property
    def NameSpace(self):
        return self._nameSpace
    @NameSpace.setter
    def NameSpace(self, val):
        self._nameSpace = val

    def _formatCommand(self):
        c = self._configAEPCommand[self._foreword.value][self._commandKey.value]
        return '{} {}'.format(self._foreword.value, c.format(self._nameSpace))

    @overrides(NdCtlCommandState)
    def CallAEPCommand(self):
        try:
            if not isinstance(self._nameSpace, list):
                command = self._formatCommand()
                ret = self._client.ExecuteCommand(command)
            else:
                c = self._configAEPCommand[self._foreword.value][self._commandKey.value]
                for s in self._nameSpace:
                    command = '{} {}'.format(self._foreword.value,
                           c.format(s))
                    self._client.ExecuteCommand(command)
        except Exception as e:
            self._success = False
            self._logger.error(str(e))

