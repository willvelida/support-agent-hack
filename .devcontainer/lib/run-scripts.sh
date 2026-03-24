#!/usr/bin/env bash
set -euo pipefail

# ─────────────────────────────────────────────────────────────────────────────
# run-scripts.sh — Shared runner library for modular .d script execution.
#
# Usage (from an orchestrator script):
#   SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
#   source "${SCRIPT_DIR}/lib/run-scripts.sh"
#   run_scripts "${SCRIPT_DIR}/post-create.d"
# ─────────────────────────────────────────────────────────────────────────────

run_scripts() {
  local scripts_dir="${1:?Usage: run_scripts <directory>}"

  if [[ ! -d "${scripts_dir}" ]]; then
    echo "⚠️  Directory not found: ${scripts_dir} — skipping."
    return 0
  fi

  local scripts
  scripts=$(find "${scripts_dir}" -maxdepth 1 -name '*.sh' ! -name '*.skip.sh' | sort)

  if [[ -z "${scripts}" ]]; then
    echo "ℹ️  No scripts found in ${scripts_dir}."
    return 0
  fi

  for script in ${scripts}; do
    local name
    name="$(basename "${script}")"
    echo "▶ Running ${name}..."
    bash "${script}"
    echo "✅ ${name} completed."
  done
}
