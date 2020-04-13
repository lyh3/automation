import os,sys,re
from enum import Enum

class Bcolors(Enum):
    def __str__(self):
        return self.value
    RED = "\033[1;31m"
    BLUE = "\033[1;34m"
    CYAN = "\033[1;36m"
    GREEN = "\033[0;32m"
    RESET = "\033[0;0m"
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

class ImageREgionUpdate:
    def __init__(self, source, replace, map, regionname, outimage):
        try:
            if os.path.isfile(source) and os.path.isfile(replace) and os.path.isfile(map):
                with open(map) as fMap:
                    for line in fMap.readlines():
                        s = line.rstrip('\n')
                        if s.endswith(regionname):
                            offset = s.split(' ')[0]
                            break

                self._update(source,
                             offset,
                             replace,
                             outimage)
            else:
                ImageREgionUpdate.PrintMessage('Cannot open file, please valid the access to file <{0}> and <{1}>'
                                               .format(source, replace), Bcolors.RED)
        except Exception as e:
            ImageREgionUpdate.PrintMessage(str(e), Bcolors.RED)

    def _update(self, source, offset, replace, outimage):
        try:
            with open(source, 'rb') as fSource:
                fSource.seek(0,0)
                size = int(self._formatHexString(offset), 16)
                bytes = fSource.read(size)
                with open(outimage, 'wb') as fOutImage:
                    fOutImage.write(bytes)
                    with open(replace, 'rb') as fUpdate:
                        size += self._fetchBytes(fUpdate, fOutImage)
                    fSource.seek(size, 0)
                    self._fetchBytes(fSource, fOutImage)
        except Exception as e:
            ImageREgionUpdate.PrintMessage(str(e), Bcolors.RED)

    def _fetchBytes(self, source, target):
        byteCount = 0
        CHUNK = 4096
        while True:
            byte = source.read(CHUNK)
            if byte:
                if len(byte) == CHUNK:
                    target.write(byte)
                    byteCount += CHUNK
                else:
                    for b in byte:
                        target.write(b)
                        byteCount += 1
            else:
                break
        return byteCount

    def _formatHexString(self, input):
        results = input
        if not (input.startswith('0x') or input.startswith('0X')):
            results = '0x{0}'.format(input)
        if re.match('^(0x|0X)?[a-fA-F0-9]+$', results):
            return results
        else:
            raise ValueError('The {} is not a valid hex string'.format(input))

    @staticmethod
    def PrintMessage(msg, clr=None):
        if not(msg is None or len(msg) is 0):
            if clr is not None:
                sys.stdout.write(clr.value)
            print(msg)
            sys.stdout.write(Bcolors.RESET.value)

def _printUsage():
    usage = []
    usage.append('\n============================================================\n')
    usage.append('Usage:\n')
    usage.append(
        'python imageRegionUpdate.py -s <Source imagefile path> -u <Region update image file path> '
        '-m <Map file for regions layout> -r <Region name> -o <out image file, OPTIONAl>\n')
    usage.append('\n============================================================')

    msg = ''
    for x in usage:
        msg += x
    ImageREgionUpdate.PrintMessage(msg, Bcolors.GREEN)

if __name__ == '__main__':
    argsCount = len(sys.argv)
    if not argsCount >= 9:
        _printUsage()
        sys.exit(1)

    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument('-s', action='store', dest='source', help="Source image file path", default=None)
    parser.add_argument('-u', action='store', dest='update', help="Region update image file path", default=None)
    parser.add_argument('-o', action='store', dest='outimage', help="Output image file path", default='outimage.bin')
    parser.add_argument('-m', action='store', dest='map', help="Map file for region layout", default=None)
    parser.add_argument('-r', action='store', dest='region', help="Region to update", default=None)

    try:
        args = parser.parse_args()
        regionUpdate = ImageREgionUpdate(args.source,
                                         args.update,
                                         args.map,
                                         args.region,
                                         args.outimage)

    except Exception as e:
        ImageREgionUpdate.PrintMessage(str(e), Bcolors.GREEN)
        _printUsage()

