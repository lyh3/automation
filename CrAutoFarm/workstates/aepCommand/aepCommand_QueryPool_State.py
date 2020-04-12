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
from workstates.aepCommand.aepCommandState import AEPCommand_Type,IpmCtlCommandState
from lxml import etree

class aepCommand_ShowDimm_State(IpmCtlCommandState):
    @overrides(IpmCtlCommandState)
    def __init__(self):
        super(IpmCtlCommandState, self).__init__()
        self._foreword = AEPCommand_Type.IPMCTL
        self._commandKey = AEPCommand_Type.QueryPool

    @overrides(IpmCtlCommandState)
    def CallAEPCommand(self):
        try:
            pools = []
            command = self._formatCommand()
            results = self._client.ExecuteCommand(command)
            root = etree.fromstring(results)
            for pool_tree in root.findall('Pool'):
                pl = {}
                for tag in ['PoolID', 'PersistentMemoryType', 'Capacity', 'FreeCapacity', 'SocketID']:
                    pl.setdefault(tag, pool_tree.find(tag).text)
                pools.append(pl)
            return pools
        except Exception as e:
            self._success = False
            self._logger.error(str(e))