# ExchangeAPI

ExchangeAPI は、複数の暗号資産取引所 API を扱うための library / adapter 基盤です。現行の library 実装範囲では、bitFlyer を主対象とし、Binance は `GetKlines` のみをサポートします。
現行ブランチでは、`stage11.md` を入口文書、`docs/spec.md` を library 本体の設計正本として扱い、CLI と MCP Server は別文書で扱います。

## Quick Links

- Stage11 goals: [`stage11.md`](stage11.md)
- Stage10 goals: [`stage10.md`](stage10.md)
- Library spec: [`docs/spec.md`](docs/spec.md)
- Bitflyer endpoints: [`docs/endpoints-bitflyer.md`](docs/endpoints-bitflyer.md)
- Binance endpoints: [`docs/endpoints-binance.md`](docs/endpoints-binance.md)
- Local NuGet consumer guide: [`docs/local-nuget-consumer.md`](docs/local-nuget-consumer.md)
- CLI adapter: [`docs/cli.md`](docs/cli.md)
- MCP Server adapter: [`docs/mcp-server.md`](docs/mcp-server.md)

## Support Boundary

- bitFlyer が現行 Stage10 の主対象であり、最も広い実装済み surface を持つ
- Binance は public `GetKlines` のみをサポートする
- `Unified` は未実装
- CLI と MCP Server は Stage11 の実装対象であり、現時点では branch 上で整備中
- endpoint ごとの exact contract は `docs/endpoints-bitflyer.md` と `docs/endpoints-binance.md` を正本とする

## Distribution

- 現段階の正規導線は source checkout + `ProjectReference`
- 公開 feed 向けの NuGet package は現 phase では提供しない
- venue ごとの entry point には `Composition` project を参照する
  - bitFlyer: `src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj`
  - Binance: `src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj`

## Local NuGet Feed

- repo root の `NuGet.config` は local feed `local/nuget` を package source として追加する
- local feed へ pack するには `bash scripts/pack-local-nuget.sh` を使う
- package version を変えたい場合は第 1 引数で渡す
  - 例: `bash scripts/pack-local-nuget.sh 0.1.0-local.1`
- この feed は repo 内ローカル用途であり、生成された `.nupkg` は git 管理しない
- 別 repo から consume する手順は `docs/local-nuget-consumer.md` を参照する

## Surface Overview

- `Protocol`
  - venue-specific execution runtime
  - raw request / response を扱う
  - debug / inspection 向けの lower-level surface
- `Native`
  - exchange-native contract
  - request / response DTO、validation、decode を扱う
  - application code が通常使う main surface
- `Unified`
  - 将来追加予定の取引所横断層
  - 現時点では未実装

通常は `Native` を使い、status code・raw body・header を直接見たいときだけ `Protocol` を使う。

## Call Contract

- facade の主 API は `*CallAsync(...)`
- `Native` facade は `Task<Call<TRequest, TResponse>>` を返す
- `Protocol` facade は `Task<Call<ProtocolRequest, ProtocolResponse>>` を返す
- success は `IsSuccess = true`, `Response != null`, `Error = null`
- failure は `IsSuccess = false`, `Response = null`, `Error != null`
- `CallError.Kind` は `Transport`, `Http`, `Codec`, `Semantic`, `Mapping` の 5 種を使う
  - `Transport`: DNS / socket / TLS / timeout / cancellation など、HTTP response を受け取る前の失敗
  - `Http`: `Native` が expected status と実 status の不一致を判定した失敗
  - `Codec`: JSON parse、shape、required field、scalar parse の失敗
  - `Semantic`: request 不正、または raw shape は読めるが API contract rule として不正
  - `Mapping`: 明示的 mapping 導入時の reserved kind
- `Protocol` は HTTP response を受け取った時点で `ProtocolResponse` を返し、non-success status を自動で `Http` failure に変換しない
- `Native` は対応する `Protocol` call を child call として保持しつつ、status / decode / contract rule を評価する

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
- Stage11 では CLI / MCP Server adapter を実装対象とし、`Unified` は薄い共通 capability に限定する

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
