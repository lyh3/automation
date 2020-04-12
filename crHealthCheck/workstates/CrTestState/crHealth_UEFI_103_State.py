from workstates.Template.crHealth_Template_State import CrHealth_Template_State
import re

class CrHealth_UEFI_103_State(CrHealth_Template_State):
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




