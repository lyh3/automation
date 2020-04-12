from workstates.Template.crHealth_Template_State import CrHealth_Template_State
import re

SENSOR_DATA_UNITS = ['C', 'c', '%']
class CrHealth_MGT_013_State(CrHealth_Template_State):
	def _invoke(self, command):
		results = ''
		ret = self._client.ExecuteCommand(command)
		if ret is not None:
			dic = {}
			expectedDictionary, reportData = self._initializeTemplate()

			for expectedKey in expectedDictionary.keys():
				if not expectedKey in dic.keys():
					dic[expectedKey] = []
				for line in ret.split('\n'):
					if line == '':
						continue
					split = re.split('\s+', line)
					pos = [i for i, x in enumerate(split) if re.match(expectedKey, x) is not None]
					if len(pos) == 0:
						continue
					dic[expectedKey].append(self._sensorDataFormat(split[pos[0] + 1]))
		for key in dic.keys():
			results += '{} '.format(key)
		results = '{}\n'.format(results)
		for i in range(0, len(dic[dic.keys()[0]])):
			for k in dic.keys():
				results += '{} '.format(dic[k][i])
			results = '{}\n'.format(results)
		return results

	def _comprehensionData(self, data):
		return [x.lstrip().strip() for x in re.split(self._delimiterPattern(), data) \
				if x.strip() != '' and not x in SENSOR_DATA_UNITS]

	def _sensorDataFormat(self, data):
		val = data
		for u in SENSOR_DATA_UNITS:
			if val.endswith(u):
				if re.match(r'\d+\s+{0}'.format(u), val) is None:
					val = val.replace(u, ' {0}'.format(u))
				break
		return val
