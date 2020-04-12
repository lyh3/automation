#-------------------------------------------------------------------------------
# Name:
# Purpose:
#
# Author:      liyingho
#
# Created:     06/03/2019
# Copyright:   (c) Intel Co. 2019
# Licence:     <your licence>
#-------------- -----------------------------------------------------------------
import sys, os
sys.path.append(os.path.split(os.path.dirname(os.getcwd()))[0])
import unittest
import buildingblocks.unitest.test_Converters as tsConvert
import buildingblocks.unitest.test_buildingblocks as tsBuildingblocks
import buildingblocks.unitest.test_decoders as tsDecoders
import buildingblocks.unitest.test_SocketKnob as tsSocketKnob

loader = unittest.TestLoader()
suite = unittest.TestSuite()
suite.addTest(loader.loadTestsFromModule(tsConvert))
suite.addTest(loader.loadTestsFromModule(tsBuildingblocks))
suite.addTest(loader.loadTestsFromModule(tsDecoders))
suite.addTest(loader.loadTestsFromModule(tsSocketKnob))

runner = unittest.TextTestRunner(verbosity=3)
result = runner.run(suite)
