#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"
package_version="${1:-3.0.0-local.preflight.$(date -u +%Y%m%d%H%M%S)}"
rid="${2:-linux-x64}"

echo "preflight package version: ${package_version}"
echo "preflight runtime id: ${rid}"

dotnet build "${repo_root}/ExchangeApi.slnx"
dotnet test "${repo_root}/ExchangeApi.slnx" --no-build

bash "${repo_root}/scripts/pack-local-nuget.sh" "${package_version}"
bash "${repo_root}/scripts/smoke-local-nuget-consumer.sh" "${package_version}"

bash "${repo_root}/scripts/create-release-assets.sh" "${package_version}" "${rid}" Release

if [[ "${EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT:-}" == "1" ]]; then
  bash "${repo_root}/scripts/run-safe-live-tests.sh"
else
  echo "safe live preflight skipped: set EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1 to include it"
fi

echo "release preflight passed: ${package_version}"
