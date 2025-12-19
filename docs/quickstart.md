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
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
// APIキー/シークレットを指定して bitFlyer クライアントを組み立て
var client = BitflyerClientFactory.Create("api-key", "api-secret");
services.AddSingleton<IMarketDataApi>(client);
services.AddSingleton<ITradingApi>(client);
services.AddSingleton<IAccountApi>(client);
services.AddSingleton<IMarginAccountApi>(client);
services.AddSingleton<IExchangeInfoApi>(client);
var provider = services.BuildServiceProvider();
```

## 3. Ticker を取得
```csharp
using ExchangeApi.Common.Types;

var market = provider.GetRequiredService<IMarketDataApi>();
var trading = provider.GetRequiredService<ITradingApi>();
var accounts = provider.GetRequiredService<IMarginAccountApi>();
var ticker = await market.GetTickerAsync(new Symbol("BTC/JPY"));
Console.WriteLine($"Last {ticker.LastTradedPrice} @ {ticker.Timestamp:O}");
```

## 4. 注文を出す（MARKET/LIMIT）
```csharp
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

var result = await trading.PlaceMarketOrderAsync(new Symbol("BTC/JPY"), Side.Buy, 0.01m);
Console.WriteLine($"Accepted: {result.Key}");
```

## 5. 注文の完了を待つ（ポーリング）
```csharp
using ExchangeApi.Common.UseCases;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;

try
{
    var status = await OrderPolling.WaitForOrderAsync(
        trading,
        new Symbol("BTC/JPY"),
        result.Key,
        new PollingOptions(TimeSpan.FromSeconds(1), 30)
        {
            NotFoundPolicy = NotFoundPolicy.Continue
        });
    Console.WriteLine($"Order status: {status.Status}");
}
catch (ExchangeOrderNotFoundException)
{
    Console.WriteLine("Order not found (still propagating or invalid key).");
}
```

## 6. OpenOrder から照会/キャンセル
```csharp
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;

var orders = await trading.GetOrdersAsync(new Symbol("BTC/JPY"));
var first = orders[0];

var status = await trading.GetOrderAsync(new Symbol("BTC/JPY"), first.Key);
Console.WriteLine($"OpenOrder status: {status.Status}");

await trading.CancelOrderAsync(new Symbol("BTC/JPY"), first.Key);
```

## 7. 注文識別子の基本ルール
- `OrderKey` は `(OrderIdKind, Value)` の組です。
- `OrderResult.Key` / `OpenOrder.Key` をそのまま `GetOrderAsync` / `CancelOrderAsync` / `OrderPolling` に渡せます。

## 8. 主要API
- REST: `IMarketDataApi` / `ITradingApi` / `IAccountApi` / `IMarginAccountApi` / `IExchangeInfoApi`
- WS: 未実装（Stage6以降に検討）

## 9. 統合クライアントを使う場合（オプション）
Raw-first が基本ですが、複数取引所を束ねる薄いファサード `UnifiedClient`（Exchange.Factory 内）も用意できます。

```csharp
using ExchangeApi.Factory.Unified;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;
using ExchangeApi.Exchanges.Bittrade.Adapter.Factory;
using ExchangeApi.Common.Types;

var bitflyer = BitflyerClientFactory.Create("api-key", "api-secret");
var bittrade = BittradeClientFactory.CreatePublic();

var unified = new UnifiedClient(bitflyer, bittrade, primary: PrimaryExchange.Bitflyer);

var ticker = await unified.PrimaryMarket.GetTickerAsync(new Symbol("BTC/JPY")); // Primary=bitFlyer
var tickerBt = await unified.Bittrade.GetTickerAsync(new Symbol("BTC_USDT"));   // Bittrade を直接
```

## エラーとハマりポイント
- サポート外シンボルは `SymbolNotSupportedException`。
- 未対応機能は `ExchangeFeatureNotSupportedException`。
- 注文が見つからない場合は `ExchangeOrderNotFoundException`（`NotFoundPolicy` で挙動を選択）。
- HTTP/取引所エラーは `ExchangeApiException`。`StatusCode` と `ExchangeErrorCode` を確認。

より詳しい説明は `docs/entry-guide.md` を参照してください。
