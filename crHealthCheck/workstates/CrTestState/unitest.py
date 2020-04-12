from unittest import TestCase

class Unitest(TestCase):
    def testCreateInstance(self):
        from workstates.CrTestState.crTestState import CrTestState
        instance = CrTestState.CreateInstance('MGT_001')