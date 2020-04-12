from workstates.Template.crHealth_Template_State import CrHealth_Template_State
import re

class CrHealth_MGT_005_State(CrHealth_Template_State):
    def _reproduce(self, key, source):
        return [x for x in re.split('({0})'.format(key), source) if x != '']
