#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
from workstates.Template.crHealth_Template_State import crHealth_Template_State
import re

class crHealth_MGT_005_State(crHealth_Template_State):
    def _reproduce(self, key, source):
        return [x for x in re.split('({0})'.format(key), source) if x != '']
