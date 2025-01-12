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

class crHealth_UEFI_102_State(crHealth_Template_State):
	def _invoke(self, command):
		COLUMN = 'SocketID MappedMemoryLimit TotalMappedMemory'
		buffer = []
		buffer.append(COLUMN)
		output = ''
		self._serialInvoke.WriteLinesRaw('{}\r'.format(self._shellCommand[0]['command']))
		self._serialInvoke.WriteLinesRaw('{} \r'.format(self._shellCommand[1]['command']))
		while True:
			ret = self._serialInvoke.ReadMessage().strip()
			if ret is None or ret == '' or ';' in ret or '0' == ret \
					or 'S' == ret or 'appedMemory' == ret or 'SocketID Mappe' == ret or '2H' == ret\
					or ret.startswith('ocketID') or ret.startswith('yLimit') or ret.startswith('otalMapped')\
					or len(ret) == 1 or len(ret) == 0 or '[' in ret or ret == 'mory' or ret.startswith('dMemory'):
				continue
			buffer.append(ret.replace('\x1b', '').replace('Ti\nB', 'TiB').replace('T\niB', 'TiB')
						  .replace('Gi\nB', 'GiB').replace('G\niB', 'GiB'))
			if len(re.findall('In UefiMain', ret)) > 0:
				break
		temp = None
		for x in buffer:
			if x == '' or x == '-' or ';' in x or x.startswith('In UefiMain'):
				continue
			if x.startswith('x00'):
				split = [n for n in x.split(' ') if n != '']
				if len(split) < 5 or not (split[4] == 'GiB' or split[4] != 'TiB'):
					temp = x
					continue
			if temp is not None:
				temp += x
				output = '{0}{1}\n'.format(output, temp.replace('GiB', 'GiB ').replace('TiB', 'TiB '))
				temp = None
				continue
			if not (x == COLUMN or re.match('0x\d+\s+\d+.\d+\s+TiB|GiB\s+\d+.\d+\s+TiB|GiB', x) is not None):
				continue
			output = '{0}{1}\n'.format(output, x)
			re.sub('TiB\n', 'TiB ', output)
		return output.rstrip('\n')





