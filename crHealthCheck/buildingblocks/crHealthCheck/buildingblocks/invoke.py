from abc import abstractmethod

class Invoke():
    @abstractmethod
    def InvokeIpmiTool(self, params):
        raise NotImplementedError("user must implemente the IntialWork.")

    @abstractmethod
    def InvokeIpmiRaw(self,  manuID, command, params, outFile = None):
        raise NotImplementedError("user must implemente the IntialWork.")
