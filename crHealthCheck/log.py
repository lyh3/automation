from datetime import datetime as d
import os


class Log(object):
	__f = None
	ts = d.now().strftime('%Y-%m-%d-%H-%M-%S')

	def __init__(self):
		if self.__f is None:
			if not os.path.isdir('SATLog'):
				os.mkdir('SATLog')
			self.__f = file(r'SATLog\SAT_%s_debug.log' % Log.ts, 'a+')

	def i(self, str):
		stamp = d.now().strftime('%Y-%m-%d-%H-%M-%S')
		fmt_str = '[Info] [%s] %s\n' % (stamp, str)
		self.__f.write(fmt_str)
		self.__f.flush()
		print fmt_str

	def close(self):
		self.__f.close()
