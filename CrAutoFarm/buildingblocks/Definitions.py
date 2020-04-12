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
from enum import Enum
from buildingblocks.utils import InvalidArgumentException


class LengthType(Enum):
    def __str__(self):
        return self.value
    METER = 'm'
    MIL_METER = 'mm'
    CENT_METER = 'cm'
    KILO_METER = 'km'
    YARD = 'yd'
    FOOT = 'ft'
    INCH = 'in'
    MILE = 'mi'


class RESULTS(Enum):
    def __str__(self):
        return self.value
    PASSED = 'PASSED'
    FAILED = 'FAILED'
    SKIPPED = 'SKIPPED'
    UNKNOWN = 'UNKNOWN'


class BiosKnobEnum(Enum):
    def __str__(self):
        return self.value
    DCPMMQos = 'DCPMMQos'
    DDRvsDDRT = 'DDRvsDDRT'
    IODC = 'IODC'
    FastGO = 'FastGO'
    ADSnoopyMode = 'ADSnoopyMode'
    MMSnoopy = 'MMSnoopy'


class LengthMetric:
    def __getitem__(self, index):
        if index is None or not (type(index) is LengthType):
            raise InvalidArgumentException(index)

        return {LengthType.MIL_METER: 1000.0,
                LengthType.CENT_METER: 100.0,
                LengthType.KILO_METER: 0.001,
                LengthType.INCH: 39.3701,
                LengthType.FOOT: 3.28084,
                LengthType.YARD: 1.09361,
                LengthType.MILE: 0.000621371}[index]


class Consts:
    INTEL = 'Intel'
    BKC_AUTOMATION_PACKAGE = 'bkcAutomationState'
    BKC_STATE_OBJ_PREFIX = 'bkcAutomation_'
    BKC_STATE_OBJ_SUFFIX = '_State'
    ACTION_DATA = 'actionData'
    TRANSCTION_COMPLETE = 'transactionComplete'
    REPO_PATH = 'RepoPath'
    EXTRACTED_BIOS_IMAGE = 'ExtractedBiosImage'
    OUTPUT_BINARIES_COUNT = 'OutputBinariesCount'
    BACKUP = 'Backup'
    SKIP = 'skip'
    REGIONS_MAP = 'RegionsMap'
    RENAME_MAP = 'RenameMap'
    BKC_BINARIES = 'BKCBinaries'
    GUID = 'GUID'
    STITCHING_CONFIG = 'StitchingConfig'
    COMPLETED_MSG = '--- Completed ---'
    FAILED_MSG = '--- Failed ---'
    REGEX_GUILD_PATTERN = '(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}'
    STATE_COMPLETE_EVENT = 'StateComplete'
    COMB_DATA_KEY = 'CombData'
    COMB_CAPTION_KEY = 'CombCaption'
    COMB_OUTPUT_FOLDER = './CombInfo'



