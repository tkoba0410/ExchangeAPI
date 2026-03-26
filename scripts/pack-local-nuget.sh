#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"
output_dir="${repo_root}/local/nuget"
package_version="${1:-0.1.0-local}"

mkdir -p "${output_dir}"

dotnet pack "${repo_root}/ExchangeApi.slnx" \
  --configuration Release \
  --output "${output_dir}" \
  -p:PackageVersion="${package_version}"
