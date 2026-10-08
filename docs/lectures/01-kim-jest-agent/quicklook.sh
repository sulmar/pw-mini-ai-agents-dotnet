#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/slides"
qlmanage -p *.png
