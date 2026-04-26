#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"
package_version="${1:-1.0.0}"
source_url="${2:-https://nuget.pkg.github.com/tkoba0410/index.json}"

if [[ -z "${GITHUB_TOKEN:-}" ]]; then
  echo "GITHUB_TOKEN is required" >&2
  exit 1
fi

shopt -s nullglob
packages=(
  "${repo_root}/local/nuget/ExchangeApi.Primitives.${package_version}.nupkg"
  "${repo_root}/local/nuget/ExchangeApi.Exchanges.Bitflyer."*.${package_version}.nupkg
  "${repo_root}/local/nuget/ExchangeApi.Exchanges.Binance."*.${package_version}.nupkg
  "${repo_root}/local/nuget/ExchangeApi.Optional."*.${package_version}.nupkg
)

if [[ ${#packages[@]} -eq 0 ]]; then
  echo "No packages found for version ${package_version} under local/nuget" >&2
  exit 1
fi

for package in "${packages[@]}"; do
  dotnet nuget push "${package}" \
    --source "${source_url}" \
    --api-key "${GITHUB_TOKEN}" \
    --skip-duplicate
done
