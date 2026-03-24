#!/usr/bin/env bash
set -euo pipefail

# Post-start orchestrator — runs each time the container starts.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/lib/run-scripts.sh"
run_scripts "${SCRIPT_DIR}/post-start.d"
