#!/usr/bin/env bash
set -euo pipefail

# Post-create orchestrator — runs once after the container is created.
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/lib/run-scripts.sh"
run_scripts "${SCRIPT_DIR}/post-create.d"
