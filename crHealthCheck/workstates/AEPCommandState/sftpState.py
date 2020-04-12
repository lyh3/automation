from workstates.AEPCommandState.aepCommandState import AEPCommnadState
from buildingblocks.Decoraters import overrides
import os

class SftpState(AEPCommnadState):
    @overrides(AEPCommnadState)
    def DoWork(self):
        try:
            ftpClient = self._client.Ftp
            sourceDir = self._config['sftp']['sourcePath']
            targetDir = self._config['sftp']['targetPath']
            self._client.ExecuteCommand('yes | mkdir -p {}'.format(targetDir))
            if os.path.exists(sourceDir):
                for item in os.listdir(sourceDir):
                    targetFile = '{}/{}'.format(targetDir, item)
                    ftpClient.put(os.path.join(sourceDir, item), targetFile)
                    if item.endswith('sh') or item.startswith('mlc'):
                        command = 'chmod +x {}'.format(targetFile)
                        self._client.ExecuteCommand(command)
            else:
                self._success = False
                self._logger.error('Can not find the source path {}'.format(sourceDir))
            ftpClient.close()
        except Exception as e:
            self._logger.error(str(e))
            self._success = False