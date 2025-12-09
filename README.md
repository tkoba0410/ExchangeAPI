# ExchangeApi

ExchangeApi は、複数の暗号資産取引所向けに統一インターフェースを提供する C#/.NET ライブラリです。  
Stage6 では **bitFlyer の Public/Private REST に特化した REST-only クライアント** として、以下を提供します。

- Ticker/Board/MarketExecutions（歩み値, Public）
- 残高・証拠金・AccountExecutions（自口座の約定履歴）
- 発注（MARKET/LIMIT/STOP/STOP_LIMIT）、キャンセル
- オープン注文・約定・ポジション一覧
- Candlestick は未サポート（NotSupported）
- WebSocket/Realtime は正式に廃止（REST のみ）
- HTTP 呼び出しには Timeout/Retry/RateLimit/CircuitBreaker を含むポリシー層を用意

詳しい使い方は Quick Start / Entry Guide を参照してください。

---

## 🏗 プロジェクト構成（Stage6 時点）

```
ExchangeApi.Contracts             ← 契約/共通DTO/エラー（旧: ExchangeApi.Contracts）
ExchangeApi.Transport        ← HTTP 基盤 + ポリシー（RestClient/Signer/Policy 等）
ExchangeApi.Adapter.Bitflyer ← bitFlyer 実装（REST マッピング）
ExchangeApi.Factory          ← DI 組み立て（機能ごとの登録を選択）
```

依存方向は必ず以下を守ります：

```
Core  ←  Transport  ←  Adapter.Bitflyer  ←  Factory
```

---

## 📦 インストール（ローカル）

前提: .NET 10 以降がインストールされていること。

リポジトリを clone します：

```bash
git clone <your-repo-url>
cd ExchangeApi
```

ビルド：

```bash
dotnet build
```

テスト：

```bash
dotnet test
```

---

## 🚀 使い方：bitFlyer の Ticker を取得する（最小例）

以下は、bitFlyer BTC/JPY の Ticker を取得する最小サンプルです。

```csharp
using Microsoft.Extensions.DependencyInjection;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Adapter.Bitflyer;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Transport;

// DI コンテナ構築
var services = new ServiceCollection();

// HttpTransport
services.AddSingleton<IHttpTransport, HttpTransport>();

// REST Client（baseUri を指定）
services.AddSingleton<IRestClient>(sp =>
{
    var http = sp.GetRequiredService<IHttpTransport>();
    return new RestClient(
        new Uri("https://api.bitflyer.com"),
        http
    );
});

// bitFlyer Public API
services.AddSingleton<IBitflyerPublicApi, BitflyerPublicApi>();

// REST 抽象インターフェースで解決（BitflyerExchangeClient を共有）
services.AddSingleton<BitflyerExchangeClient>();
services.AddSingleton<IMarketDataApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());
services.AddSingleton<ITradingApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());
services.AddSingleton<IAccountApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());
services.AddSingleton<IMarginAccountApi>(sp => sp.GetRequiredService<BitflyerExchangeClient>());

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IMarketDataApi>();

// BTC/JPY の最新 ticker
var ticker = await client.GetTickerAsync("BTC/JPY");

Console.WriteLine($"Bid : {ticker.BestBid}");
Console.WriteLine($"Ask : {ticker.BestAsk}");
Console.WriteLine($"Time: {ticker.Timestamp:O}");
```

---

## 🎯 Stage4 の概要

- 取引所: bitFlyer（REST-only）
- Public: `GET /v1/getticker`, `GET /v1/getboard`, `GET /v1/getexecutions`（MarketExecutions）
- Private: 残高/証拠金/ポジション/口座約定/オープン注文、`sendchildorder`, `cancelchildorder`, `cancelallchildorders`
- DTO: `Ticker`, `Board`, `MarketExecution`, `AccountExecution`, `Balance`, `Collateral`, `Position`, `OpenOrder`, `OrderRequest/Result`
- 例外: `SymbolNotSupportedException`（シンボル未対応）、`ExchangeApiException`（HTTP/取引所エラー）
- 信頼性: Timeout/Retry/RateLimit/CircuitBreaker のデフォルトポリシーを組み込み
- Realtime/WS: 非対応（REST のみ）

---

## 🧪 テスト構成

```
tests/
 ├─ ExchangeApi.Contracts.Tests
 ├─ ExchangeApi.Transport.Tests
 ├─ ExchangeApi.Adapter.Bitflyer.Tests
```

特に Stage1 で重要なテスト：

- `TickerTests`（DTO）
- `BitflyerExchangeClientTests`（Raw → Ticker）
- `RestClientTests`（path + query → URI 構築）

---

## 🔗 参考ドキュメント
- Quick Start: `docs/quickstart.md`
- Entry Guide: `docs/entry-guide.md`
- 抽象 API 対応表: `docs/stage4/A042-STG4-ABSTRACT-MAP.md`
- DTO マッピング（Ticker 例）: `docs/stage4/DTO-Ticker-MAP.md`
- Stage 概要: `docs/STAGES-OVERVIEW.md`

---

## 📄 ライセンス

MIT License  
（必要に応じて修正）

---

> **Stage6 Status:**  
> REST-only 方針で信頼性・運用強化中です（Timeout/Retry/RateLimit/CircuitBreaker / 観測性フック）。  
> WebSocket/Realtime は廃止し、bitFlyer REST 縦スライスにフォーカスしています。
