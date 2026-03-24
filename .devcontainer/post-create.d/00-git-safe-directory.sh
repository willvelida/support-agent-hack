#!/usr/bin/env bash
set -euo pipefail

# Configure git safe directory for the workspace.
git config --global --add safe.directory "${containerWorkspaceFolder:-/workspaces}"
