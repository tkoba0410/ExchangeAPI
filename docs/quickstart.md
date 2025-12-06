# Quick Start

ExchangeApi を最短で動かすための手順です。実装されているのは Stage3 相当（bitFlyer での Ticker / MARKET 発注 / 残高取得）で、Stage4 は「REST+WS 抽象 API を定義する」設計フェーズです（実装は Stage5 以降）。

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
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Adapter.Bitflyer;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Transport;
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
services.AddSingleton<BitflyerExchangeClient>();
services.AddSingleton<IMarketDataApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());
services.AddSingleton<ITradingApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());
services.AddSingleton<IAccountApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());
services.AddSingleton<IMarginAccountApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());
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

## 4. 注文を出す（Stage3: MARKET）
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

## 5. （参考）Stage4 で定義された抽象
- REST: `IMarketDataApi` / `ITradingApi` / `IAccountApi` / `IMarginAccountApi`
- WS: `IRealtimeMarketDataApi`
- ExchangeInfo: `IExchangeInfoApi`
※ これらは Stage4 で設計のみ。実装は Stage5 以降。

## エラーとハマりポイント
- サポート外シンボルは `SymbolNotSupportedException`（抽象層）で通知。
- HTTP/bitFlyer エラーは `ExchangeApiException`。`StatusCode` と `ExchangeErrorCode` を確認。
- STOP/LIMIT/キャンセル/ポジション取得などは Stage5 以降で実装される予定。

より詳しい説明は `docs/entry-guide.md` を参照してください。
