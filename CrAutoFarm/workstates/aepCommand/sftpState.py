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
from workstates.aepCommand.aepCommandState import AEPCommnadState
from buildingblocks.Decoraters import overrides
import os

class SftpState(AEPCommnadState):
    @overrides(AEPCommnadState)
    def DoWork(self):
        try:
            ftpClient = self._client.Ftp
            sourceDir = self._config['sftp']['sourcePath']
            targetDir = self._config['sftp']['targetPath']
            workingspace = '{}/{}'.format(targetDir, self._config['sftp']['OS'])
            self._client.ExecuteCommand('yes | mkdir -p {}'.format(workingspace))
            ftpClient.chdir(workingspace)
            # cwd = ftpClient.getcwd()

            if os.path.exists(sourceDir):
                self._copyContens(sourceDir, targetDir, ftpClient)
            else:
                self._success = False
                self._logger.error('Can not find the source path {}'.format(sourceDir))
            ftpClient.close()
        except Exception as e:
            self._logger.error(str(e))
            self._success = False

    def _copyContens(self, source, targetDir, ftpClient):
        if source is None or not os.path.exists(source):
            return
        if os.path.isdir(source):
            for item in os.listdir(source):
                p = os.path.join(source, item)
                if os.path.isdir(p):
                    self._copyContens(p, '{}/{}'.format(targetDir, item), ftpClient)
                else:
                    self._client.ExecuteCommand('yes | mkdir -p {}'.format(targetDir))
                    targetFile = '{}/{}'.format(targetDir, item)
                    ftpClient.put(p, targetFile)
                    if item.endswith('sh') or item.startswith('mlc'):
                        command = 'chmod +x {}'.format(targetFile)
                    self._client.ExecuteCommand(command)

