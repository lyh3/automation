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
from workstates.Template.crHealth_BiosLogDependencyCheck_State import crHealth_BiosLogDependencyCheck_State
import re

class crHealth_UEFI_101_State(crHealth_BiosLogDependencyCheck_State):
	def _invoke(self, command):
		buffer = []
		output = ''
		self._serialInvoke.WriteLinesRaw('{}\r'.format(self._shellCommand[0]['command']))
		self._serialInvoke.WriteLinesRaw('{} \r'.format(self._shellCommand[1]['command']))
		while True:
			ret = self._serialInvoke.ReadMessage().strip()
			if ret is not None and ret != '':
				buffer.append(ret.replace('\x1b', ''))
				if len(re.findall('In UefiMain', ret)) > 0:
					break
		for x in buffer:
			if x == '' or x == '-' or ';' in x:
				continue
			output = '{0}{1}\n'.format(output, x)
		return output

	def _lookupConverter(self, dimm):
		return dimm.TopologyLocater()


	@overrides(crHealth_BiosLogDependencyCheck_State)
	def _lookup(self, lookup, source):
		return len(re.split(lookup, source.replace('\n', ''))) > 1





