# Quick Start

ExchangeApi を最短で動かすための手順です。Raw-first のレイアウト（`Common.Core` + `Exchange.(Bitflyer|Bittrade)` + `ExchangeApi.Factory`）に移行済みです。REST-only で、信頼性パターン（Timeout/Retry/RateLimit/CircuitBreaker）と観測性フックを提供しています（WSは未実装、エラー分類はカテゴリ単位）。

## 前提
- .NET 10 以降
- bitFlyer の API キー（Private API を使う場合）
- プロジェクトに `Common.Core` / `Exchange.Bitflyer` / `Exchange.Bittrade` / `ExchangeApi.Factory` のソースがあること（ソリューションに含まれています）

## 1. インストール & ビルド
```bash
git clone <this-repo>
cd ExchangeApi
dotnet build
```

## 2. DI 登録（最小例）
```csharp
using Common.Core.Contracts.Contracts;
using Exchange.Bitflyer.Factory;
using Exchange.Bitflyer.Facade;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
// APIキー/シークレットを指定して bitFlyer クライアントを組み立て
var client = BitflyerClientFactory.Create("api-key", "api-secret");
services.AddSingleton<IMarketDataApi>(client);
services.AddSingleton<ITradingApi>(client);
services.AddSingleton<IAccountApi>(client);
services.AddSingleton<IMarginAccountApi>(client);
var provider = services.BuildServiceProvider();
```

## 3. Ticker を取得
```csharp
var market = provider.GetRequiredService<IMarketDataApi>();
var trading = provider.GetRequiredService<ITradingApi>();
var accounts = provider.GetRequiredService<IMarginAccountApi>();
var ticker = await market.GetTickerAsync("BTC/JPY");
Console.WriteLine($"Bid {ticker.BestBid} / Ask {ticker.BestAsk} / Last {ticker.LastTradedPrice}");
```

## 4. 注文を出す（MARKET/LIMIT/STOP）
```csharp
using ExchangeApi.Contracts.Dtos;

var order = new OrderRequest(
    ProductCode: "BTC_JPY",
    Side: OrderSide.Buy,
    OrderType: OrderType.Market,
    Size: 0.01m,
    Price: null);

var result = await trading.SendOrderAsync(order);
Console.WriteLine($"Accepted: {result.OrderId}");
```

## 5. 主要API（Stage6）
- REST: `IMarketDataApi` / `ITradingApi` / `IAccountApi` / `IMarginAccountApi` / `IExchangeInfoApi`
- WS: 未実装（Stage6以降に検討）

## エラーとハマりポイント
- サポート外シンボルは `SymbolNotSupportedException`（抽象層）で通知。
- HTTP/bitFlyer エラーは `ExchangeApiException`。`StatusCode` と `ExchangeErrorCode` を確認。
- Candles は REST未サポート（NotSupported）。

より詳しい説明は `docs/entry-guide.md` を参照してください。
