#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"

export EXCHANGEAPI_RUN_LIVE_TESTS=1

dotnet test "${repo_root}/tests/Exchanges/Binance/LiveTests/ExchangeApi.Exchanges.Binance.LiveTests.csproj"
dotnet test "${repo_root}/tests/Exchanges/Bitflyer/LiveTests/ExchangeApi.Exchanges.Bitflyer.LiveTests.csproj"
dotnet test "${repo_root}/tests/Adapters/McpServer.LiveTests/ExchangeApi.Adapters.McpServer.LiveTests.csproj"
