# ExchangeAPI

ExchangeAPI は、複数の暗号資産取引所 API を扱うための Stage10 実装基盤です。現行の実装範囲では、bitFlyer を主対象とし、Binance は `GetKlines` のみをサポートします。
現行ブランチでは、`stage10.md` を入口文書、`docs/spec.md` を library 本体の設計正本として扱い、CLI と MCP Server は別文書で扱います。

## Quick Links

- Stage10 goals: [`stage10.md`](stage10.md)
- Library spec: [`docs/spec.md`](docs/spec.md)
- Bitflyer endpoints: [`docs/endpoints-bitflyer.md`](docs/endpoints-bitflyer.md)
- Binance endpoints: [`docs/endpoints-binance.md`](docs/endpoints-binance.md)
- CLI adapter: [`docs/cli.md`](docs/cli.md)
- MCP Server adapter: [`docs/mcp-server.md`](docs/mcp-server.md)

## Surface Overview

- `Protocol`
  - venue-specific execution runtime
  - raw request / response を扱う
- `Native`
  - exchange-native contract
  - request / response DTO、validation、decode を扱う
- `Unified`
  - 将来追加予定の取引所横断層
  - 現時点では未実装

## Quickstart

### 1) bitFlyer Public Ticker (`Native`)

```bash
# repo root で実行
dotnet new console -n ExchangeApi.Quickstart
cd ExchangeApi.Quickstart
dotnet add reference ../src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj
```

`Program.cs` を次で置き換え:

```csharp
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

using var client = BitflyerClientFactory.CreateNativeClient();

var call = await client.Public.GetTickerCallAsync(new GetTickerRequest
{
    ProductCode = ProductCodes.BtcJpy,
});

if (call.IsSuccess && call.Response is not null)
{
    Console.WriteLine($"{call.Response.ProductCode} ltp={call.Response.Ltp} at={call.Response.Timestamp:O}");
}
else
{
    Console.WriteLine($"error kind={call.Error?.Kind} message={call.Error?.Message}");
}
```

実行:

```bash
dotnet run
```

期待結果: `BTC_JPY ltp=...` のような出力が 1 行出る（ネットワーク要）。

### 2) Binance Klines (`Native`)

```bash
# 1) の console project で続けて実行
dotnet add reference ../src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj
```

`Program.cs` 例:

```csharp
using ExchangeApi.Exchanges.Binance.Composition.Factory;
using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Exchanges.Binance.Vocabulary;

using var client = BinanceClientFactory.CreateNativeClient();

var call = await client.Public.GetKlinesCallAsync(new GetKlinesRequest
{
    Symbol = BinanceSymbols.BtcJpy,
    Interval = "1h",
    Limit = 2,
});

if (call.IsSuccess && call.Response is not null)
{
    Console.WriteLine($"count={call.Response.Count} close={call.Response[^1].ClosePrice}");
}
else
{
    Console.WriteLine($"error kind={call.Error?.Kind} message={call.Error?.Message}");
}
```

## Configuration

- `BitflyerClientOptions`
  - `BaseUri`
  - `RequestTimeout`
  - `Credentials`
  - `UseTickerAliasPath`
  - `EnableProtocolDebugLogging`
  - `ProtocolDebugLogDirectory`
- `BinanceClientOptions`
  - `BaseUri`
  - `RequestTimeout`
  - `EnableProtocolDebugLogging`
  - `ProtocolDebugLogDirectory`

bitFlyer の private surface は `Credentials` を渡したときだけ有効になる。
credentials なしの bundle では `Private` は `null` になる。

### bitFlyer Private Permissions (`Native`)

```csharp
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPermissions;

using var client = BitflyerClientFactory.CreateNativeClient(new BitflyerClientOptions
{
    Credentials = new BitflyerApiCredentials
    {
        ApiKey = Environment.GetEnvironmentVariable("BITFLYER_API_KEY")!,
        ApiSecret = Environment.GetEnvironmentVariable("BITFLYER_API_SECRET")!,
    },
});

var call = await client.Private!.GetPermissionsCallAsync(new GetPermissionsRequest());
```

### bitFlyer Public Ticker (`Protocol`)

```csharp
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

using var client = BitflyerClientFactory.CreateProtocolClient();
var call = await client.Public.GetTickerCallAsync(ProductCodes.BtcJpy);

if (call.IsSuccess && call.Response is not null)
{
    Console.WriteLine(call.Response.BodyText);
}
```

## Client Lifetime

- `CreateProtocolClient(...)` / `CreateNativeClient(...)` が返す bundle は per-call object ではなく reuse 前提の long-lived object
- bundle は `IDisposable` を実装する
- `CreateProtocolClient(options)` / `CreateNativeClient(options)` は internal-owned mode で、library が `HttpClient` を生成して所有する
- `CreateProtocolClient(httpClient, options)` / `CreateNativeClient(httpClient, options)` は external-owned mode で、library は caller 提供 `HttpClient` を dispose しない
- bundle は process または application scope で使い回し、不要になった時点で dispose することを推奨する
- `RequestTimeout` は per-request timeout で、`HttpClient.Timeout` ではなく linked cancellation で適用する
- internal-owned mode では library 生成 `HttpClient.Timeout` を `Timeout.InfiniteTimeSpan` に固定する
- external-owned mode では library は caller 提供 `HttpClient` の `Timeout` / `BaseAddress` / `DefaultRequestHeaders` を変更しない
- caller が external-owned `HttpClient.Timeout` を短く設定している場合、それが `RequestTimeout` より先に失敗することがある

## Current State

- 実装済みの公開面は `Protocol` / `Native` の一部 endpoint
- bitFlyer は `GetMarkets`, `GetBoard`, `GetTicker`, `GetExecutionsPublic`, `GetBoardState`, `GetHealth`, `GetFundingRate`, `GetCorporateLeverage`, `GetChats`, `GetPermissions`, `GetBalance`, `GetCollateral`, `GetCollateralAccounts`, `GetAddresses`, `GetCoinIns`, `GetCoinOuts`, `GetBankAccounts`, `GetDeposits`, `Withdraw`, `GetWithdrawals`, `GetChildOrders`, `GetParentOrders`, `GetParentOrder`, `GetExecutionsPrivate`, `GetBalanceHistory`, `GetPositions`, `GetCollateralHistory`, `GetTradingCommission`, `SendChildOrder`, `SendParentOrder`, `CancelChildOrder`, `CancelParentOrder`, `CancelAllChildOrders`
- bitFlyer の `Withdraw` は fixed contract だが、live 検証は negative contract のみを持つ
- Binance は public `GetKlines` のみをサポートする
- 現行 phase では library を優先し、`Unified`, CLI, MCP Server は将来検討とする

## Live Tests

- 標準の `dotnet test ExchangeApi.slnx` には live test project を含めない
- live test は `dotnet test ExchangeApi.LiveTests.slnx` で明示実行する
- すべての live test は既定で skip され、次のいずれかで opt-in する
  - `EXCHANGEAPI_RUN_LIVE_TESTS=1`
  - `touch local/live-enabled`
- public live test も opt-in がない限り実行しない
- bitFlyer private/write live test は `age` で復号する credentials source を前提にする
  - `EXCHANGEAPI_BITFLYER_CREDENTIALS_AGE_FILE_PATH`
  - `EXCHANGEAPI_AGE_IDENTITY_FILE_PATH`
- 例: `source scripts/bitflyer-live-age-env.example.sh`
- write opt-in marker
  - `touch local/bitflyer-live-write-enabled`
- `CancelAllChildOrders` 専用 marker
  - `touch local/bitflyer-live-cancel-all-enabled`
- `Withdraw` negative live contract 専用 marker
  - `touch local/bitflyer-live-withdraw-negative-enabled`

## Development

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-build
dotnet test ExchangeApi.LiveTests.slnx --no-build
```
