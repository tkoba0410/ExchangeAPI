# ExchangeApi

ExchangeApi は、複数の暗号資産取引所向けに統一インターフェースを提供する C#/.NET ライブラリです。  
Stage6 では **bitFlyer の Public/Private REST に特化した REST-only クライアント** として、以下を提供します。

- Ticker/Board/MarketExecutions（歩み値, Public）
- 残高・証拠金・AccountExecutions（自口座の約定履歴）
- 発注（MARKET/LIMIT/STOP。STOP_LIMIT は Stop + Price 指定で送信）、キャンセル
- オープン注文・約定・ポジション一覧
- Candlestick は未サポート（NotSupported）
- WebSocket/Realtime は正式に廃止（REST のみ）
- HTTP 呼び出しには Timeout/Retry/RateLimit/CircuitBreaker を含むポリシー層を用意（エラー分類はカテゴリ単位）

詳しい使い方は Quick Start / Entry Guide を参照してください。

---

## 🏗 プロジェクト構成（Raw-first 移行後）

```
Common.Core                  ← 契約/共通DTO/エラー + HTTP基盤/ポリシー
Exchange.Bitflyer            ← bitFlyer 実装（REST マッピング/Factory）
Exchange.Bittrade            ← Bittrade 実装
Exchange.Common              ← 取引所共通ヘルパ（プレースホルダ）
ExchangeApi.Factory          ← 共通の組み立てヘルパ（現状の Factory を継続利用）
```

依存方向（Raw-first/最小プロジェクト構成）：

```
Common.Core  ←  Exchange.(各取引所)  ←  Factory
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
using Exchange.Bitflyer;
using Exchange.Bitflyer.Factory;

var services = new ServiceCollection();

// Factory で Http/Signer/Policy をまとめて組み立てる
var client = BitflyerClientFactory.Create("api-key", "api-secret");

services.AddSingleton<IMarketDataApi>(client);
services.AddSingleton<ITradingApi>(client);
services.AddSingleton<IAccountApi>(client);
services.AddSingleton<IMarginAccountApi>(client);

var provider = services.BuildServiceProvider();
var market = provider.GetRequiredService<IMarketDataApi>();

// BTC/JPY の最新 ticker
var ticker = await market.GetTickerAsync("BTC/JPY");

Console.WriteLine($"Bid : {ticker.BestBid}");
Console.WriteLine($"Ask : {ticker.BestAsk}");
Console.WriteLine($"Time: {ticker.Timestamp:O}");
```

---

## 🎯 Stage6 の概要

- 取引所: bitFlyer（REST-only、WS 廃止）
- Public: `GET /v1/getticker`, `GET /v1/getboard`, `GET /v1/getexecutions`（MarketExecutions）
- Private: 残高/証拠金/ポジション/口座約定/オープン注文、`sendchildorder`, `cancelchildorder`, `cancelallchildorders`
- DTO: `Ticker`, `Board`, `MarketExecution`, `AccountExecution`, `Balance`, `Collateral`, `Position`, `OpenOrder`, `OrderRequest/Result`
- 例外: `SymbolNotSupportedException`（シンボル未対応）、`ExchangeApiException`（HTTP/取引所エラー）※エラー分類はカテゴリ単位
- 信頼性: Timeout/Retry/RateLimit/CircuitBreaker のデフォルトポリシーと観測性フックを用意
- Realtime/WS: 非対応（REST のみ）

---

## 🧪 テスト構成

```
tests/
 ├─ ExchangeApi.Contracts.Tests
 ├─ ExchangeApi.Transport.Tests
 ├─ Exchange.Bitflyer.Tests
```

代表的なテスト：

- `RestClientTests`（path + query → URI 構築）
- `BitflyerExchangeClientTests`（Ticker マッピング）
- `BitflyerExchangeClient_SendOrder_Tests` / `PollOrderStatus_Tests`（発注・ポーリングフロー）
- `HttpPolicyTests`（Timeout/Retry/RateLimit/CircuitBreaker）

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
