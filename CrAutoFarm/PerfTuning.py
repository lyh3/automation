#-------------------------------------------------------------------------------
# Name:        module1
# Purpose:
#
# Author:      liyingho, henry.li@intel.com
#
# Created:     07/13/2019
# Copyright:   (c) liyingho 2017
# Licence:     <your licence>
#-------------------------------------------------------------------------------
import sys, os, json
from buildingblocks.Definitions import Consts
from workthreads.performanceTuningThread import PerformanceTuningThread
import msvcrt
import time

if __name__ == '__main__':
    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument('-j', action='store', dest='jsonfile', help="Config Json file path", default=None)
    parser.add_argument('-f', action='store', dest='filters', help="Excluse bios knob list", default=None)
    parser.add_argument('-i', action='store', dest='ip', help="SUT ip", default=None)
    parser.add_argument('-u', action='store', dest='user', help="SUT user", default=None)
    parser.add_argument('-p', action='store', dest='password', help="SUT password", default=None)
    parser.add_argument('-t', action='store', dest='timeout', help="Timeout", default=None)
    parser.add_argument('-g', action='store', dest='generatecombs', help="Generate combs information", default='y')

    args = parser.parse_args()
    if args.jsonfile is not None:
        jsonpath = args.jsonfile
    else:
        jsonpath = os.path.realpath(r'Json/PerfTuning.json')
    config = json.loads(open(jsonpath).read())
    f = None
    if args.filters is not None:
        f = [x.strip() for x in args.filters.split(',') if x != '']
    if args.ip is not None: config['Client']['ip'] = args.ip
    if args.user is not None: config['Client']['user'] = args.user
    if args.password is not None: config['Client']['password'] = args.password
    if args.timeout is not None: config['timeout'] = args.timeout
    if f is not None:
        for knob in config['BiosKnobs']:
            if knob not in f:
                config['BiosKnobs'][knob]['skip'] = True
    filename, extension = os.path.splitext(os.path.basename(jsonpath))
    resourcePath = os.path.realpath(r'Json/Resource.json')
    resource = json.loads(open(resourcePath).read())

    if args.generatecombs.lower().startswith('y'):
        PerformanceTuningThread.GenerateCombJsons(config)

    if not os.path.isdir(Consts.COMB_OUTPUT_FOLDER)or len(os.listdir(Consts.COMB_OUTPUT_FOLDER)) == 0:
        print('There is no combination information been found, nothing to do.')
        sys.exit(-1)

    #TODO : instatiate iptInstance here
    itpInstance = None
    thread = PerformanceTuningThread.Launch(config, resource, itpInstance)
    try:
        while thread.IsRuning:
            if msvcrt.kbhit():
                char = msvcrt.getch()
                if str(char).lower() == 'q':
                    thread.Stop()
                    break
            import winsound

            #winsound.Beep(1440, 250)
            time.sleep(1)
    except KeyboardInterrupt as kbEx:
        thread.Stop()
    except Exception as e:
        print(str(e))
