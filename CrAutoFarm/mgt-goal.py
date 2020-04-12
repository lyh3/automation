#!/usr/bin/env python

import re, os, subprocess


# This file checks MGT-006, MGT-007 and MGT-008, which are checking,
# creating and deleting goals.


# fail()-- Come here if the test fails.

def fail(msg):
    print ('Status: FAIL')
    print msg
    raise SystemExit


# success()-- Declare success.

def success():
    print ('Status: PASS')
    raise SystemExit



# run()-- Run a test command, collecting the output.  This is like
# subprocess.check_output() except that we don't care if the
# underlying command returns an error or not.

def run(cmd):
    process = subprocess.Popen(cmd, stdout=subprocess.PIPE)

    output, unused = process.communicate()
    process.poll()    # Ignore return code

    return output.split('\n')


# check_goal()-- Run the ipmctl show goal command.  Returns True if a
# goal is defined, False otherwise.

def check_goal():
    cmd = [ 'ipmctl', 'show', '-goal' ]

    for line in run(cmd):
        if 'There are no goal configs defined in the system' in line:
            return False

        if 'A reboot is required to process new memory allocation goals.' in line:
            return True
        
        pass

    fail('FAILURE: Unable to check goals')
    pass


# create_goal()-- Run the ipmctl create goal command.  Returns True if
# we created a goal, False if we did not.

def create_goal():
    cmd = [ 'ipmctl', 'create', '-f', '-goal' ]

    success = 'Created following region configuration goal'
    fail = 'Create region configuration goal failed: Error'

    for line in run(cmd):
        if success in line:
            return True

        if fail in line:
            return False

        pass
                                                   
    fail('Unable to create goal')
    pass


# delete_goal()-- Run the ipmctl delete goal command.  Returns True if
# we deleted a goal, False if we did not.

def delete_goal():
    cmd = [ 'ipmctl', 'delete', '-goal' ]

    success_re = re.compile('Delete memory allocation goal from DIMM .*: Success')
    fail_re = re.compile('Delete memory allocation goal from DIMM .*: Error')
    
    for line in run(cmd):
        if success_re.match(line):
            return True

        if fail_re.match(line):
            return False
        
        pass

    fail('Unable to delete goal')
    pass


# test_create()-- Test creating a goal.  Assumes that there is no goal
# defined.

def test_create():
    if not create_goal():
        fail('Unable to create goal')

    if not check_goal():
        fail('Goal was not created')

    if create_goal():
        fail('Created multiple goals')

    print ('MGT-007 Success creating goal')
    return


# test_delete()-- Test deleting a goal.  Assumes that there is a
# goal defined.

def test_delete():
    if not delete_goal():
        fail('FAILURE: Unable to delete goal')

    if check_goal():
        fail('Goal was not deleted')

    if delete_goal():
        fail('Can delete multiple goals')
    
    print ('MGT-008 Success deleting goal')
    return


if os.getuid() != 0:
    raise SystemExit, 'FAILURE: must be root to run this test'


# Ordering the tests this way leaves the system the way we found it.

if check_goal():
    test_delete()
    test_create()

else:
    test_create()
    test_delete()
    pass

print ('MGT-006 Success checking goal')
success()

