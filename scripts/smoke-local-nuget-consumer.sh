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

dotnet add package ExchangeApi.Exchanges.Bitflyer \
  --version "${package_version}" \
  >/dev/null

dotnet add package ExchangeApi.Optional.Credentials \
  --version "${package_version}" \
  >/dev/null

dotnet add package ExchangeApi.Optional.Logging \
  --version "${package_version}" \
  >/dev/null

dotnet add package ExchangeApi.Optional.Testing \
  --version "${package_version}" \
  >/dev/null

cat > Program.cs <<'EOF'
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.PlainText;
using ExchangeApi.Optional.Logging.Redaction;
using ExchangeApi.Optional.Logging.Realtime;
using ExchangeApi.Optional.Testing.Realtime;

using var client = BitflyerClientFactory.CreateNativeClientBundle();
await using var realtimeClient = BitflyerRealtimeClientFactory.CreatePublicClient(new SmokeRealtimeTransport());
var request = new GetTickerRequest { ProductCode = ProductCodes.BtcJpy };
var provider = PlainTextApiCredentialProviderFactory.Create(ExchangeVenue.Bitflyer, "api-key", "api-secret");
await using var privateRealtimeClient = BitflyerRealtimeClientFactory.CreatePrivateClient(provider, new SmokeRealtimeTransport());
var options = new BitflyerRealtimeClientOptions
{
    Reconnect = new BitflyerRealtimeReconnectOptions
    {
        MaxAttempts = 0,
        InitialDelay = TimeSpan.Zero,
        MaxDelay = TimeSpan.Zero,
    },
    IdleTimeout = TimeSpan.FromSeconds(30),
};
await using var session = await provider.OpenSessionAsync();
var redactor = new Redactor(new RedactionOptions { SensitiveValues = ["secret-value"] });
var redacted = redactor.RedactText("apiSecret=api-secret payload=secret-value");
var diagnostic = new RealtimeDiagnosticEvent
{
    EventType = RealtimeDiagnosticEventTypes.MessageRejected,
    ObservedAt = DateTimeOffset.UtcNow,
    Venue = "bitFlyer",
    Channel = BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy),
    Severity = RealtimeDiagnosticSeverities.Warning,
    Reason = "Smoke",
};
var frameLogFactory = new RealtimeRawFrameLogRecordFactory(new RealtimeRawFrameLogOptions
{
    IncludeBody = true,
    MaxRawFrameBodyBytes = 65536,
});
var frameLog = frameLogFactory.Create(
    "bitFlyer",
    BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy),
    DateTimeOffset.UtcNow,
    """{"api_key":"api-key","message":{"ltp":100}}""");
var replayFrame = RealtimeReplayFrame.Create(
    BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy),
    """
    {"jsonrpc":"2.0","method":"channelMessage","params":{"channel":"lightning_ticker_BTC_JPY","message":{"product_code":"BTC_JPY","timestamp":"2026-04-27T12:34:56.789","tick_id":1,"best_bid":99,"best_ask":101,"best_bid_size":1,"best_ask_size":2,"total_bid_depth":3,"total_ask_depth":4,"ltp":100,"volume":5,"volume_by_product":6}}}
    """,
    DateTimeOffset.Parse("2026-04-27T12:34:56Z"));
var replay = await BitflyerRealtimeReplayRunner.ReplayTickerAsync(ProductCodes.BtcJpy, [replayFrame]);

Console.WriteLine(
    client.Public is not null &&
    realtimeClient is not null &&
    privateRealtimeClient is not null &&
    request.ProductCode == ProductCodes.BtcJpy &&
    BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy) == "lightning_ticker_BTC_JPY" &&
    BitflyerRealtimeChannels.ChildOrderEvents() == "child_order_events" &&
    BitflyerRealtimeChannels.ParentOrderEvents() == "parent_order_events" &&
    typeof(BitflyerRealtimeStreamEvent<>).Name == "BitflyerRealtimeStreamEvent`1" &&
    typeof(BitflyerRealtimeDiagnostic<>).Name == "BitflyerRealtimeDiagnostic`1" &&
    diagnostic.EventType == RealtimeDiagnosticEventTypes.MessageRejected &&
    diagnostic.Severity == RealtimeDiagnosticSeverities.Warning &&
    frameLog.BodySkipped == false &&
    frameLog.Body is not null &&
    !frameLog.Body.Contains("api-key", StringComparison.Ordinal) &&
    replay.IsSuccessful &&
    replay.Items.Count == 1 &&
    replay.Items[0].Ltp == 100m &&
    options.Reconnect.MaxAttempts == 0 &&
    options.IdleTimeout == TimeSpan.FromSeconds(30) &&
    session.ApiKey == "api-key" &&
    redacted == "apiSecret=[REDACTED] payload=[REDACTED]"
        ? "consumer-smoke-ok"
        : "consumer-smoke-ng");

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
if [[ "${output}" != "consumer-smoke-ok" ]]; then
  echo "Unexpected smoke output: ${output}" >&2
  exit 1
fi

if [[ "${output}" == *"api-key"* || "${output}" == *"api-secret"* || "${output}" == *"secret-value"* ]]; then
  echo "Smoke output contained a secret marker" >&2
  exit 1
fi

echo "consumer smoke passed: ${package_version}"
