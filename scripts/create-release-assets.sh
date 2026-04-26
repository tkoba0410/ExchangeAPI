#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"
version="${1:?usage: bash scripts/create-release-assets.sh <version> [rid] [configuration]}"
rid="${2:-linux-x64}"
configuration="${3:-Release}"
asset_dir="${repo_root}/local/publish/release-assets/v${version}"
cli_binary="${repo_root}/local/publish/cli/${rid}/exchangeapi"
mcp_binary="${repo_root}/local/publish/mcp/${rid}/exchangeapi-mcp"
cli_asset="${asset_dir}/exchangeapi-${rid}"
mcp_asset="${asset_dir}/exchangeapi-mcp-${rid}"

mkdir -p "${asset_dir}"

bash "${script_dir}/publish-cli-local.sh" "${rid}" "${configuration}"
bash "${script_dir}/publish-mcp-local.sh" "${rid}" "${configuration}"

if [[ ! -f "${cli_binary}" ]]; then
  echo "CLI binary not found: ${cli_binary}" >&2
  exit 1
fi

if [[ ! -f "${mcp_binary}" ]]; then
  echo "MCP binary not found: ${mcp_binary}" >&2
  exit 1
fi

cp "${cli_binary}" "${cli_asset}"
cp "${mcp_binary}" "${mcp_asset}"
chmod +x "${cli_asset}" "${mcp_asset}"

(
  cd "${asset_dir}"
  sha256sum "exchangeapi-${rid}" > "exchangeapi-${rid}.sha256"
  sha256sum "exchangeapi-mcp-${rid}" > "exchangeapi-mcp-${rid}.sha256"
)

echo "release assets created: ${asset_dir}"
