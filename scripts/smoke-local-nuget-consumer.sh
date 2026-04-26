#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "${script_dir}/.." && pwd)"
package_version="${1:-2.0.0-local.verify}"
smoke_dir="$(mktemp -d)"

cleanup() {
  rm -rf "${smoke_dir}"
}
trap cleanup EXIT

dotnet new console \
  --framework net10.0 \
  --name ExchangeApiConsumerSmoke \
  --output "${smoke_dir}/ExchangeApiConsumerSmoke" \
  >/dev/null

cd "${smoke_dir}/ExchangeApiConsumerSmoke"

cat > NuGet.config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="exchangeapi-local" value="${repo_root}/local/nuget" />
  </packageSources>
</configuration>
EOF

dotnet add package ExchangeApi.Exchanges.Bitflyer.Composition \
  --version "${package_version}" \
  >/dev/null

dotnet add package ExchangeApi.Optional.Credentials \
  --version "${package_version}" \
  >/dev/null

cat > Program.cs <<'EOF'
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.PlainText;

using var client = BitflyerClientFactory.CreateNativeClientBundle();
var request = new GetTickerRequest { ProductCode = ProductCodes.BtcJpy };
var provider = PlainTextApiCredentialProviderFactory.Create(ExchangeVenue.Bitflyer, "api-key", "api-secret");
await using var session = await provider.OpenSessionAsync();

Console.WriteLine(
    client.Public is not null &&
    request.ProductCode == ProductCodes.BtcJpy &&
    session.ApiKey == "api-key"
        ? "consumer-smoke-ok"
        : "consumer-smoke-ng");
EOF

dotnet restore --configfile NuGet.config >/dev/null
dotnet build --no-restore >/dev/null

output="$(dotnet run --no-build)"
if [[ "${output}" != "consumer-smoke-ok" ]]; then
  echo "Unexpected smoke output: ${output}" >&2
  exit 1
fi

echo "consumer smoke passed: ${package_version}"
