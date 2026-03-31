#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"
rid="${1:-linux-x64}"
configuration="${2:-Release}"
output_dir="${repo_root}/local/publish/mcp/${rid}"

mkdir -p "${output_dir}"

dotnet publish "${repo_root}/src/Adapters/McpServer/ExchangeApi.Adapters.McpServer.csproj" \
  --configuration "${configuration}" \
  --runtime "${rid}" \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:DebugType=None \
  -p:DebugSymbols=false \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  --output "${output_dir}"

echo "published: ${output_dir}/exchangeapi-mcp"
