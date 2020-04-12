#!/bin/sh
# This script uses MLC to measure bandwidth and latency for Intel® Optane™ DC Persistent Memory
# memory using App Direct mode.
# It will try to auomatically detect the number of DCPMM mapped
# to the specified pmem mount path (should be socket 0)
# Before running this, you need to make a filesystem on the namespace and
# mount it as dax
# example:
#   sudo mkfs.ext4 /dev/pmem
#   sudo mount -o dax /dev/pmem /mnt/pmem
# See help output for list of optional arguments
pushd $PWD &> /dev/null

#################################################################################################
# useful parameters to edit
#################################################################################################

MLC=./mlc                                # default, -m option to overrid
PMEM_PATH=/mnt/pmem0                           # default, -p option to override
BUF_SZ=400000                                   # used in MLC perthread files
OUTPUT_PATH="./outputs.`date +"%m%d-%H%M"`"     # created by the script
SAMPLE_TIME=10                                  # -t argument to MLC
socket=0					# -s argument default socket

# Index these by number of DIMMS to get optimal thread count for the address pattern
#                   0  1  2  3  4  5  6
THREADS_DCPMM_RD_SEQ=(0  6  12 18 24 30 36)
THREADS_DCPMM_RD_RND=(0  2  4  6  8  10 12)
THREADS_DCPMM_WR_SEQ=(0  1  2  2  2  2  4)
THREADS_DCPMM_WR_RND=(0  1  1  1  1  1  1)
THREADS_DCPMM_MX_SEQ=(0  2  4  6  8  10 12)
THREADS_DCPMM_MX_RND=(0  2  4  4  4  4  4)

# injection delays used for loaded latency (to vary demand bitrate)
DELAYS=(0 50 100 200 300 400 500 700 850 1000 1150 1300 1500 1700 2500 3500 5000 20000 40000 80000)

#################################################################################################
# helper functions
#################################################################################################

trap ctrl_c INT
function ctrl_c() {
   echo "Got contr+c - aborting"
   popd &> /dev/null
   exit 2
}

function display_help() {
   echo "Usage: $0 [optional args]"
   echo "Runs bandwidth and latency tests on DCPMM backed PMEM memory using MLC"
   echo "Run with root privilege (MLC needs it)"
   echo "Optional args:"
   echo "   -m <Path to MLC executable>"
   echo "      By default, The MLC binary is expected to be in $MLC"
   echo "   -p <Path to mounted PMEM directory>"
   echo "      By default, The pmem memory is expected to be mounted to $PMEM_PATH"
   echo "   -s <Socket>"
   echo "      By default, Socket 0 is used for load the traffic"
   exit 3
}

function handle_args() {
   if [ "$1" = "--h" ] || [ "$1" = "-help" ] || [ "$1" = "--help" ] || [ "$1" = "help" ]; then
      display_help $0
   fi

   if [[ $EUID -ne 0 ]]; then
      echo "Please run this script with root privilege" 
      exit 1
   fi
   
   while getopts "h?m:p:s:" opt; do
      case "$opt" in
         h|\?)
         display_help $0
         ;;
      m)  MLC=$OPTARG
         ;;
      p)  PMEM_PATH=$OPTARG	
	 ;;
      s)  socket=$OPTARG
         ;;
      esac
   done
   
   echo "Using pmem memory mounted to:     $PMEM_PATH"
   echo "Using MLC command:                $MLC"
   if [ ! -f $MLC ]; then
       echo "Couldn't find MLC at the indicated location"
       display_help $0
   fi
   TOKENS=( $($MLC --version 2>&1 | head -n 1) )
   MLC_VER=${TOKENS[5]}
   echo "MLC version:                      $MLC_VER"
   echo "MLC output files stored in:       $OUTPUT_PATH"

   if mountpoint -q -- "$PMEM_PATH"; then
      DAX_SUPPORT=$(mount | grep -w $PMEM_PATH | grep dax | wc -l)
      if (($DAX_SUPPORT <= 0)); then
         echo "Mounted filesystem doesn't support DAX"
         display_help $0
      fi
      TOKENS=( $(mount | grep -w $PMEM_PATH) )
      FS_TYPE=${TOKENS[4]}
      echo "mount found, FS type:             $FS_TYPE"
   else
      echo "Couldn't find pmem mounted at the path in this script"
      echo "If you haven't already, create a dax supporting filesystem on the namespace and mount it"
      display_help $0
   fi

   echo -n "Kernel version:                   "
   uname -r
}

function init_outputs() {
   rm -rf $OUTPUT_PATH 2> /dev/null
   mkdir $OUTPUT_PATH

   DELAYS_FILE=$OUTPUT_PATH/delays.txt
   for DELAY in "${DELAYS[@]}"; do 
      echo $DELAY >> $DELAYS_FILE
   done
   DRAM_PERTHREAD=$OUTPUT_PATH/dram_perthread.txt
   DCPMM_PERTHREAD=$OUTPUT_PATH/DCPMM_perthread.txt
}

function check_cpus() {
   TOKENS=( $(lscpu | grep "Core(s) per socket:") )
   CORES_PER_SOCKET=${TOKENS[3]}
   
   # only using the CPUs on this NUMA node
   CPUS=$CORES_PER_SOCKET
   
   echo "CPUs detected:                    $CPUS"
   
   if (($CPUS < 28 )); then
      echo "WARNING: Using less than $CPUS processors per node makes it harder to saturate memory"
   fi
   
   # One CPU used to measure latency, so the rest can be for bandwidth generation
   BW_CPUS=$(($CPUS-1))
}

function check_rage_per_socket(){
	#echo $socket
	if (( $socket==0 )); then
		TOKENS=( $(lscpu | grep "NUMA node0 CPU(s):") )
		X=0
	elif (( $socket==1)); then
		TOKENS=( $(lscpu | grep "NUMA node1 CPU(s):") )
      X=$CPUS
	fi
	
	RANGES_PER_SOCKET=${TOKENS[3]}
	#echo $RANGES_PER_SOCKET
	
	RANGE=${RANGES_PER_SOCKET/,*/}
	#echo $RANGE

	FIRST_P=$((${RANGE/-*/}))
	#echo $FIRST_P
	END_P=$((${RANGE/*-/}))
	#echo $END_P
	
	FIRST_P1=$(($FIRST_P+1))
	#echo $FIRST_P1

	echo "Rage of Socket$socket : $RANGE"
	
	if [[ $(($END_P-$FIRST_P)) < 27 ]]; then
		echo "WARNING: Using less than $CPUS processors per node makes it harder to saturate memory"
	fi
}

function check_dimms() {
   # Find the number of DCPMM in the namespace
   
   NUM_DIMMS=0
   
   # Check FW version if we can with ipmctl
   command -v ipmctl &> /dev/null 
   IPMCTL_NOT_PRESENT=$?
   if (($IPMCTL_NOT_PRESENT == 0)); then
      echo "Management software version:"
      ipmctl version
      echo "DCPMM Firmware versions:"
      ipmctl show -firmware -dimm
   else
      echo "ipmctl not found, cannot use it to detect DCPMM FW version"
   fi

   # First try with ndctl as we can follow the namespace to the DIMMs
   command -v ndctl &> /dev/null 
   NDCTL_NOT_PRESENT=$?
   if (($NDCTL_NOT_PRESENT == 0)); then
      echo -n "NDCTL version:                    "
      ndctl -v

      DEV_PATH=$(mount | grep -w $PMEM_PATH | awk '{print $1;}')
      if [[ $DEV_PATH == /dev/pmem* ]]; then
         DEV=$(echo $DEV_PATH | cut -c10-)
         NUM_DIMMS=$(ndctl list -DR -r $DEV | grep '"dimm":"' | wc -l)
      else 
         echo "Don't understand dev path $DEV_PATH"
      fi
   else
      echo "ndctl not found, cannot use it to detect number of DIMMS"
   fi

   if (($NUM_DIMMS <= 0)); then
      # Assuming namespace is on socket 0, so just looking at DIMMS there
      if (($IPMCTL_NOT_PRESENT == 0)); then
         echo "as secondary approach to DCPMM count detection, using ipmctl"
         echo "ASSUMING NAMESPACE IS ON SOCKET 0!"
         NUM_DIMMS=$( show -topology | grep "DCPMM DIMM" | grep CPU1 | wc -l)
      else
         echo "ipmctl not found, cannot use it to detect number of DIMMS"
      fi
   fi
   
   # if still 0, ask the caller
   if (($NUM_DIMMS <= 0)); then
      echo "Unable to automatically determine the number of DIMMS in the namespace"
      echo -n "please enter the DCPMM count: "
      read NUM_DIMMS
      if (($NUM_DIMMS < 1 )); then
         echo "Cannot have < 1 DCPMM in the namespace, exiting"
         exit 1
      fi
      if (($NUM_DIMMS > 6 )); then
         echo "Cannot have > 6 DCPMM in the namespace, exiting"
         exit 1
      fi
   fi
   
   echo "DCPMM count in namespace:      $NUM_DIMMS"
   
   # Calculate how many threads to use for each traffic type
   RD_SEQ_CPUS=$((${THREADS_DCPMM_RD_SEQ[$NUM_DIMMS]}+$FIRST_P))
   if (($RD_SEQ_CPUS > $END_P));then
      RD_SEQ_CPUS=$END_P
   fi
  # echo $RD_SEQ_CPUS
   RD_RND_CPUS=$((${THREADS_DCPMM_RD_RND[$NUM_DIMMS]}+$FIRST_P))
   if (($RD_RND_CPUS > $END_P));then
      RD_RND_CPUS=$END_P
   fi
  # echo $RD_RND_CPUS
   WR_SEQ_CPUS=$((${THREADS_DCPMM_WR_SEQ[$NUM_DIMMS]}+$FIRST_P))
   if (($WR_SEQ_CPUS > $END_P));then
      WR_SEQ_CPUS=$END_P
   fi
  # echo $WR_SEQ_CPUS
   WR_RND_CPUS=$((${THREADS_DCPMM_WR_RND[$NUM_DIMMS]}+$FIRST_P))
   if (($WR_RND_CPUS > $END_P));then
      WR_RND_CPUS=$END_P
   fi
  # echo $WR_RND_CPUS
   MX_SEQ_CPUS=$((${THREADS_DCPMM_MX_SEQ[$NUM_DIMMS]}+$FIRST_P))
   if (($MX_SEQ_CPUS > $END_P));then
      MX_SEQ_CPUS=$END_P
   fi
  # echo $MX_SEQ_CPUS
   MX_RND_CPUS=$((${THREADS_DCPMM_MX_RND[$NUM_DIMMS]}+$FIRST_P))
   if (($MX_RND_CPUS > $END_P));then
      MX_RND_CPUS=$END_P
   fi
  # echo $MX_RND_CPUS
}

#################################################################################################
# Metric measuring functions
#################################################################################################

function idle_latency() {
   echo ""
   echo -n "DCPMM idle sequential latency: "
   $MLC --idle_latency -J$PMEM_PATH > $OUTPUT_PATH/idle_seq.txt
   cat $OUTPUT_PATH/idle_seq.txt | grep "Each iteration took" 
   echo -n "DCPMM idle random     latency: "
   $MLC --idle_latency -l256 -J$PMEM_PATH > $OUTPUT_PATH/idle_rnd.txt
   cat $OUTPUT_PATH/idle_rnd.txt | grep "Each iteration took"
}
   
function bandwidth() {
   # if socket = 0 then X = 0, if socket = 1 then X = 28
   echo ""
   echo "DCPMM bandwidth: using $(( $RD_SEQ_CPUS - $X )) for seq read, $(( $RD_RND_CPUS - $X )) for rnd read,"
   echo "                     $(( $WR_SEQ_CPUS - $X )) for seq write, $(( $WR_RND_CPUS - $X )) for rnd write,"
   echo "                     $(( $MX_SEQ_CPUS - $X )) for seq mixed, $(( $MX_RND_CPUS -$X )) for rnd mixed"
   BW_ARRAY=(
   #  CPUs            Traffic type   seq or rand  buffer size   pmem or dram   pmem path     output filename
      "$FIRST_P1-$RD_SEQ_CPUS R              seq          $BUF_SZ       pmem           $PMEM_PATH    bw_seq_READ_$RD_SEQ_CPUS.txt"
      "$FIRST_P1-$RD_RND_CPUS R              rand         $BUF_SZ       pmem           $PMEM_PATH    bw_rnd_READ_$RD_RND_CPUS.txt"
      "$FIRST_P1-$WR_SEQ_CPUS W6             seq          $BUF_SZ       pmem           $PMEM_PATH    bw_seq_WRNT_$WR_SEQ_CPUS.txt"
      "$FIRST_P1-$WR_RND_CPUS W6             rand         $BUF_SZ       pmem           $PMEM_PATH    bw_rnd_WRNT_$WR_RND_CPUS.txt"
      "$FIRST_P1-$MX_SEQ_CPUS W7             seq          $BUF_SZ       pmem           $PMEM_PATH    bw_seq_21NT_$MX_SEQ_CPUS.txt"
      "$FIRST_P1-$MX_RND_CPUS W7             rand         $BUF_SZ       pmem           $PMEM_PATH    bw_rnd_21NT_$MX_RND_CPUS.txt"
      "$FIRST_P1-$MX_SEQ_CPUS W5             seq          $BUF_SZ       pmem           $PMEM_PATH    bw_seq_11RFO_$MX_SEQ_CPUS.txt"
      "$FIRST_P1-$MX_RND_CPUS W5             rand         $BUF_SZ       pmem           $PMEM_PATH    bw_rnd_11RFO_$MX_RND_CPUS.txt"
      "$FIRST_P1-$MX_SEQ_CPUS W2             seq          $BUF_SZ       pmem           $PMEM_PATH    bw_seq_21RFO_$MX_SEQ_CPUS.txt"
      "$FIRST_P1-$MX_RND_CPUS W2             rand         $BUF_SZ       pmem           $PMEM_PATH    bw_rnd_21RFO_$MX_RND_CPUS.txt"
   )
   for LN in "${BW_ARRAY[@]}"
   do
      TOK=( $LN )
      echo ${TOK[0]} ${TOK[1]} ${TOK[2]} ${TOK[3]} ${TOK[4]} ${TOK[5]} > $DCPMM_PERTHREAD
      TMP=($(echo ${TOK[6]} | sed 's/[_,.]/ /g'))
      TMP[3]=$(( ${TMP[3]} - $X ))
      FILE=$(echo ${TMP[*]}|sed 's/ /_/g'|sed 's/_txt/.txt/g')
      echo -n "max DCPMM bandwidth for $FILE - Delay, nS, MBPS: "
      $MLC --loaded_latency -d0 -o$DCPMM_PERTHREAD -t$SAMPLE_TIME -T > $OUTPUT_PATH/${TOK[6]}
      cat $OUTPUT_PATH/${TOK[6]} | sed -n -e '/==========================/,$p' | tail -n+2
   done
}

function loaded_latency() {
   echo ""
   echo "0  R seq  $BUF_SZ pmem $PMEM_PATH" >  $DCPMM_PERTHREAD
   echo "$FIRST_P1-$RD_SEQ_CPUS R seq  $BUF_SZ pmem $PMEM_PATH" >> $DCPMM_PERTHREAD
   echo $(( ($RD_SEQ_CPUS - $FIRST_P1)+1 )) core DCPMM sequential read loaded latency sweep:
   echo " Delay nS         MBPS"
   $MLC --loaded_latency -g$DELAYS_FILE -o$DCPMM_PERTHREAD -t$SAMPLE_TIME > $OUTPUT_PATH/out_llat_seq_READ_$RD_SEQ_CPUS.txt
   cat $OUTPUT_PATH/out_llat_seq_READ_$RD_SEQ_CPUS.txt | sed -n -e '/==========================/,$p' | tail -n+2

   echo "0  R rand $BUF_SZ pmem $PMEM_PATH" >  $DCPMM_PERTHREAD
   echo "$FIRST_P1-$RD_RND_CPUS R rand $BUF_SZ pmem $PMEM_PATH" >> $DCPMM_PERTHREAD
   echo $(( ($RD_RND_CPUS - $FIRST_P1)+1 )) core DCPMM random read loaded latency sweep:
   echo " Delay nS         MBPS"
   $MLC --loaded_latency -g$DELAYS_FILE -o$DCPMM_PERTHREAD -t$SAMPLE_TIME -r > $OUTPUT_PATH/out_llat_rnd_READ_$RD_RND_CPUS.txt
   cat $OUTPUT_PATH/out_llat_rnd_READ_$RD_RND_CPUS.txt | sed -n -e '/==========================/,$p' | tail -n+2
}


#################################################################################################
# Main program flow
#################################################################################################

echo "======================================================================="
echo "Starting DCPMM bandwidth and latency measurements using MLC"
echo "Version 1.2.1E"
echo "======================================================================="

handle_args $@
check_cpus
check_rage_per_socket
check_dimms
init_outputs

idle_latency
bandwidth
loaded_latency

echo "======================================================================="
echo "DCPMM bandwidth and latency measurements Complete"
echo "======================================================================="

popd &> /dev/null
exit 0
