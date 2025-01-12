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
from workstates.Template.crHealth_Template_State import crHealth_Template_State
import re

class crHealth_UEFI_103_State(crHealth_Template_State):
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




