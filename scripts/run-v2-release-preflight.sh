#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "scripts/run-v2-release-preflight.sh is deprecated; use scripts/run-release-preflight.sh." >&2
exec "${script_dir}/run-release-preflight.sh" "$@"
