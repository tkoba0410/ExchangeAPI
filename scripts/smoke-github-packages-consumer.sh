#!/usr/bin/env bash

set -euo pipefail

package_version="${1:?usage: bash scripts/smoke-github-packages-consumer.sh <version>}"
feed_url="${EXCHANGEAPI_GITHUB_PACKAGES_FEED:-https://nuget.pkg.github.com/tkoba0410/index.json}"
feed_user="${EXCHANGEAPI_GITHUB_PACKAGES_USER:-tkoba0410}"
smoke_dir="$(mktemp -d)"

cleanup() {
  rm -rf "${smoke_dir}"
}
trap cleanup EXIT

token="${GITHUB_TOKEN:-${GH_TOKEN:-}}"
if [[ -z "${token}" ]]; then
  if command -v gh >/dev/null 2>&1; then
    token="$(gh auth token 2>/dev/null || true)"
  fi
fi

if [[ -z "${token}" ]]; then
  echo "GitHub Packages token is required. Set GITHUB_TOKEN/GH_TOKEN or authenticate gh." >&2
  exit 1
fi

dotnet new console \
  --framework net10.0 \
  --name ExchangeApiGitHubPackagesSmoke \
  --output "${smoke_dir}/ExchangeApiGitHubPackagesSmoke" \
  >/dev/null

cd "${smoke_dir}/ExchangeApiGitHubPackagesSmoke"

cat > NuGet.config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github" value="${feed_url}" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="${feed_user}" />
      <add key="ClearTextPassword" value="${token}" />
    </github>
  </packageSourceCredentials>
</configuration>
EOF

dotnet add package ExchangeApi.Exchanges.Bitflyer \
  --version "${package_version}" \
  >/dev/null

dotnet add package ExchangeApi.Optional.Credentials \
  --version "${package_version}" \
  >/dev/null

dotnet add package ExchangeApi.Optional.Logging \
  --version "${package_version}" \
  >/dev/null

cat > Program.cs <<'EOF'
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.PlainText;
using ExchangeApi.Optional.Logging.Redaction;

using var client = BitflyerClientFactory.CreateNativeClientBundle();
await using var realtimeClient = BitflyerRealtimeClientFactory.CreatePublicClient(new SmokeRealtimeTransport());
var request = new GetTickerRequest { ProductCode = ProductCodes.BtcJpy };
var provider = PlainTextApiCredentialProviderFactory.Create(ExchangeVenue.Bitflyer, "api-key", "api-secret");
await using var session = await provider.OpenSessionAsync();
var redactor = new Redactor(new RedactionOptions { SensitiveValues = ["secret-value"] });
var redacted = redactor.RedactText("apiSecret=api-secret payload=secret-value");

Console.WriteLine(
    client.Public is not null &&
    realtimeClient is not null &&
    request.ProductCode == ProductCodes.BtcJpy &&
    BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy) == "lightning_ticker_BTC_JPY" &&
    session.ApiKey == "api-key" &&
    redacted == "apiSecret=[REDACTED] payload=[REDACTED]"
        ? "github-packages-smoke-ok"
        : "github-packages-smoke-ng");

internal sealed class SmokeRealtimeTransport : IBitflyerRealtimeTransport
{
    public ValueTask ConnectAsync(Uri endpointUri, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask SendTextAsync(string text, CancellationToken cancellationToken = default)
    {
        return ValueTask.CompletedTask;
    }

    public IAsyncEnumerable<string> ReadTextAsync(CancellationToken cancellationToken = default)
    {
        return Empty();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private static async IAsyncEnumerable<string> Empty()
    {
        await Task.CompletedTask;
        yield break;
    }
}
EOF

dotnet restore --configfile NuGet.config >/dev/null
dotnet build --no-restore >/dev/null

output="$(dotnet run --no-build)"
if [[ "${output}" != "github-packages-smoke-ok" ]]; then
  echo "Unexpected smoke output: ${output}" >&2
  exit 1
fi

if [[ "${output}" == *"api-key"* || "${output}" == *"api-secret"* || "${output}" == *"secret-value"* || "${output}" == *"${token}"* ]]; then
  echo "Smoke output contained a secret marker" >&2
  exit 1
fi

echo "GitHub Packages consumer smoke passed: ${package_version}"
