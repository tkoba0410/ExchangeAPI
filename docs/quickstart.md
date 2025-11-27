# Quick Start

ExchangeApi を最短で動かすための手順です。Stage4 時点の機能（bitFlyer での Ticker/発注/キャンセル/ポジション・約定取得）を対象としています。

## 前提
- .NET 10 以降
- bitFlyer の API キー（Private API を使う場合）
- プロジェクトに `ExchangeApi` ソース一式があること

## 1. インストール & ビルド
```bash
git clone <this-repo>
cd ExchangeApi
dotnet build
```

## 2. DI 登録（最小例）
```csharp
using ExchangeApi.Abstractions.Contracts;
using ExchangeApi.Bitflyer;
using ExchangeApi.Infrastructure.Protocol;
using ExchangeApi.Infrastructure.Transport;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddSingleton<IHttpTransport, HttpTransport>();
services.AddSingleton<IRestClient>(sp =>
{
    var http = sp.GetRequiredService<IHttpTransport>();
    return new RestClient(new Uri("https://api.bitflyer.com"), http);
});
services.AddSingleton<IBitflyerPublicApi, BitflyerPublicApi>();
services.AddSingleton<IBitflyerPrivateApi, BitflyerPrivateApi>();
services.AddSingleton<IBitflyerPrivateTradingApi, BitflyerPrivateApi>();
services.AddSingleton<IExchangeClient, BitflyerExchangeClient>();
var provider = services.BuildServiceProvider();
```

## 3. Ticker を取得
```csharp
var client = provider.GetRequiredService<IExchangeClient>();
var ticker = await client.GetTickerAsync("BTC/JPY");
Console.WriteLine($"Bid {ticker.BestBid} / Ask {ticker.BestAsk} / Last {ticker.LastTradedPrice}");
```

## 4. 注文を出す（LIMIT/STOP 対応）
```csharp
using ExchangeApi.Abstractions.Dtos;

var order = new OrderRequest(
    ProductCode: "BTC_JPY",
    Side: OrderSide.Buy,
    OrderType: OrderType.Limit,
    Size: 0.01m,
    Price: 4000000m);

var result = await client.PlaceOrderAsync(order);
Console.WriteLine($"Accepted: {result.OrderId}");
```

## 5. キャンセルする
```csharp
await client.CancelOrderAsync("BTC_JPY", result.OrderId);
await client.CancelAllOrdersAsync("BTC_JPY");
```

## 6. ポジション・約定を取る
```csharp
var positions = await client.ListPositionsAsync("FX_BTC_JPY");
var executions = await client.ListExecutionsAsync("BTC_JPY");
```

## エラーとハマりポイント
- サポート外シンボルは `SymbolNotSupportedException`（抽象層）で通知。
- HTTP/bitFlyer エラーは `ExchangeApiException`。`StatusCode` と `ExchangeErrorCode` を確認。
- STOP 注文で指値付きにする場合は `Price` と `TriggerPrice` を両方指定（STOP_LIMIT）。`Price` なしは成行（STOP）。

より詳しい説明は `docs/entry-guide.md` を参照してください。
