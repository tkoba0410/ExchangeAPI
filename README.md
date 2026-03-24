# ExchangeAPI

ExchangeAPI は、複数の暗号資産取引所 API を扱うための Stage10 実装基盤です。
現行ブランチでは、`stage10.md` を入口文書、`docs/spec.md` を library 本体の設計正本として扱い、CLI と MCP Server は別文書で扱います。

## Quick Links

- Stage10 goals: `stage10.md`
- Library spec: `docs/spec.md`
- Bitflyer endpoints: `docs/endpoints-bitflyer.md`
- Binance endpoints: `docs/endpoints-binance.md`
- CLI adapter: `docs/cli.md`
- MCP Server adapter: `docs/mcp-server.md`

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

var client = BitflyerClientFactory.CreateNativeClient();

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

var client = BinanceClientFactory.CreateNativeClient();

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

## Current State

- 実装済みの公開面は `Protocol` / `Native` の一部 endpoint
- bitFlyer は `GetMarkets`, `GetBoard`, `GetTicker`, `GetExecutionsPublic`, `GetBalance`, `GetCollateral`, `GetCollateralAccounts`, `GetChildOrders`, `GetExecutionsPrivate`, `GetPositions`, `GetCollateralHistory`, `GetTradingCommission`, `SendChildOrder`, `CancelChildOrder`, `CancelAllChildOrders`
- Binance は `GetKlines`
- 現行 phase では library を優先し、`Unified`, CLI, MCP Server は将来検討とする

## Development

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-build
```
