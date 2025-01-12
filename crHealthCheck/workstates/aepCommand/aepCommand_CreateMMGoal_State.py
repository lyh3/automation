#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from buildingblocks.Decoraters import overrides
from workstates.aepCommand.aepCommandState import AEPCommand_Type,IpmCtlCommandState

class aepCommand_CreateMMGoal_State(IpmCtlCommandState):
    @overrides(IpmCtlCommandState)
    def __init__(self):
        super(IpmCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.IPMCTL
        self._commandKey = AEPCommand_Type.CreateMMGoal
        self._percentage = 100

    @property
    def Percentage(self):
        return self._percentage
    @Percentage.setter
    def Percentage(self, val):
        self._percentage = val

    def _formatCommand(self):
        '{} {} {}'.format(self._foreword.value,
                       self._configAEPCommand[self._foreword.value][self._commandKey.value],
                       self._percentage)

    @overrides(IpmCtlCommandState)
    def CallAEPCommand(self):
        try:
            command = self._formatCommand()
            results = self._client.ExecuteCommand(command)
            self._logger.info(results)
            return results
        except Exception as e:
            self._success = False
            self._logger.error(str(e))