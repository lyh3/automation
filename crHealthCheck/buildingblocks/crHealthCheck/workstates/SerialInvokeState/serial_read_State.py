from buildingblocks.utils import overrides
from workstates.SerialInvokeState.serialInvokeState import SerialInvokeState
from buildingblocks.invokeSerial import InvokeSerial

class Serial_read_State(SerialInvokeState):
    @overrides(SerialInvokeState)
    def SerialInvoke(self):
        return InvokeSerial(self._port, self._baud).ReadMessage()