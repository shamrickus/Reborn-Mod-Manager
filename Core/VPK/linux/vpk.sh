#!/bin/bash

#https://developer.valvesoftware.com/wiki/VPK#Linux_.2F_Unix
VPK_LINUX="$1"/vpk_linux32
VALVE_LIB_DIR=$(dirname "${VPK_LINUX}")
LD_LIBRARY_PATH="${VALVE_LIB_DIR}:${LD_LIBRARY_PATH}" "${VPK_LINUX}" "${2}"
