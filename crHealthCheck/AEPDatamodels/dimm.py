from enum import Enum
import re

class Dimm:
    def __init__(self,
                 cpu,
                 socket,
                 channel,
                 dimmtype,
                 dimmsize,
                 dimmid,
                 manufacture):
        self._cpu = cpu
        self._dimmtype = dimmtype
        self._socket = socket
        self._channel = channel
        self._verified = False
        if dimmsize is None:
            self._dimmsize = [0, Dimm_Size_Unit.GiB]
        else:
            self._dimmsize = dimmsize
        self._dimmid = dimmid
        self._manufacture = manufacture

    def __str__(self):
        return 'SOCKET_TABLE_{0}_SOCKET{1}_CHANNEL{2}_{3}_({4})_Manufacture({5})_ID({6})'\
            .format(self._cpu,
                    self._socket,
                    self._channel,
                    self._dimmtype,
                    self.DimmSize,
                    self._manufacture,
                    self._dimmid)

    @property
    def dimmtype(self):
        return self._dimmtype
    @dimmtype.setter
    def dimmtype(self, val):
        self._dimmtype = val
    @property
    def cpu(self):
        return self._cpu
    @cpu.setter
    def cpu(self, val):
        self._cpu = val
    @property
    def socket(self):
        return self._socket
    @socket.setter
    def socket(self, val):
        self._socket = val
    @property
    def channel(self):
        return self._channel
    @channel.setter
    def channel(self, val):
        self._channel = val
    @property
    def dimmsize(self):
        return self._dimmsize
    @dimmsize.setter
    def dimmsize(self, val):
        self._dimmsize = val
    @property
    def DimmSize(self):
        return '{0} {1}'.format(self._dimmsize[0], self._dimmsize[1])
    @property
    def DimmId(self):
        return self._dimmid
    @property
    def Manufacture(self):
        return self._manufacture

    def TopologyLocater(self):
        return 'CPU{0}_DIMM_{1}{2}'.format(eval('{0} + 1'.format(self._cpu)),
                                   self._channelTopologyMapping(),
                                   self._dimmtypeTopologyMapping())

    def _dimmtypeTopologyMapping(self):
        if self._dimmtype == Dimm_type.DDR4:
            return 1
        if self._dimmtype == Dimm_type.NVMDIMMLRDIMM\
            or self._dimmtype == Dimm_type.DDRT:
            return 2

    def _channelTopologyMapping(self):
        if self._channel == 0:
            return 'A'
        if self._channel == 1:
            return 'B'
        if self._channel == 2:
            return 'C'
        if self._channel == 3:
            return 'D'
        if self._channel == 4:
            return 'E'
        if self._channel == 5:
            return 'F'

    @staticmethod
    def DimmTypeRegexSearchPatterm():
        results = ''
        for p in ['{0}?|'.format(x) for x in Dimm_type if True]:
            results += p
        return '\s+{0}\s+'.format(results[:-1])

    @staticmethod
    def DimmSizeRegexSearchPatterm():
        results = ''
        for s in ['{0}?|'.format(x) for x in Dimm_Size_Unit if True]:
            results += s
        return '\s+\d+{0}\s+'.format(results[:-1])

    @staticmethod
    def DimmSizeFormat(dimmsize):
        val = dimmsize
        for u in Dimm.SUPORTED_DIMM_SIZE_UNIT():
            if val.endswith(u):
                if re.match(r'\d+\s+{0}'.format(u), val) is None:
                    val = val.replace(u, ' {0}'.format(u))
                break
        return val

    @staticmethod
    def SUPORTED_DIMM_SIZE_UNIT():
        return [Dimm_Size_Unit.GiB.value,
                Dimm_Size_Unit.TiB.value,
                Dimm_Size_Unit.GB.value]

class Dimm_type(Enum):
    def __str__(self):
        return self.value
    DDR4 = 'DDR4'
    DDRT = 'DDRT'
    NVMDIMMLRDIMM = 'NVMDIMMLRDIMM'

class Dimm_Size_Unit(Enum):
    def __str__(self):
        return self.value
    GiB = 'GiB'
    TiB = 'TiB'
    GB = 'GB'
