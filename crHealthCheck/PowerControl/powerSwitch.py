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
from abc import abstractmethod
from buildingblocks.Decoraters import timeElapseTimer


class PowerSwitch(object):
	def __init__(self, config, logger):
		self._powerSwitch = None
		self._config = config
		self._logger = logger
		self._timeoutTimer(0, True)

	@timeElapseTimer
	def _timeoutTimer(self, timeout, reset):
		return self._timeoutTimer.isTimeout

	def _checkTimeout(self, timeout=None):
		if timeout is not None:
			t = timeout
		else:
			t = self._config['timeout']
		if t is not None and t != '':
			t = t.lstrip().strip()
			if t != '':
				timeout = int(t, 0)
				if self._timeoutTimer(timeout, False):
					if self._logger is not None:
						self._logger.info('Timeout = {0}sec reached. Test aborted.'.format(timeout))
					return

	def _getPort(self):
		return int(self._config['port'])

	@abstractmethod
	def power_on(self, switch_port=None):
		raise NotImplementedError("user must implemente the power_on.")


	@abstractmethod
	def power_off(self, switch_port=None):
		raise NotImplementedError("user must implemente the power_off.")


	@abstractmethod
	def power_cycle(self, switch_port=None):
		raise NotImplementedError("user must implemente the power_cycle.")
