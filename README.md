# ExchangeApi

ExchangeApi は、複数の暗号資産取引所向けに統一インターフェースを提供する C#/.NET ライブラリです。  
Stage1 では **bitFlyer Public REST API /v1/ticker による Ticker 取得** のみに対応します。

- 統一 DTO（Ticker）
- 抽象クライアント（IExchangeClient）
- REST/Transport 分離
- 取引所ごとの Adapter 実装（Stage1 は bitFlyer）

この構造により、Stage2 以降の拡張（Private REST / 認証 / WebSocket / 複数取引所統合）が容易になります。

---

## 🏗 プロジェクト構成（Stage1）

```
ExchangeApi.Abstractions      ← 仕様（Boundary）
ExchangeApi.Infrastructure    ← REST / HTTP Transport（共通）
ExchangeApi.Bitflyer          ← bitFlyer Adapter（Raw → Ticker）
ExchangeApi.Orchestration     ← Stage2 で実装予定（現状は空）
```

依存方向は必ず以下を守ります：

```
Abstractions  ←  Infrastructure  ←  Bitflyer
```

---

## 📦 インストール（ローカル）

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
using ExchangeApi.Abstractions.Contracts;
using ExchangeApi.Bitflyer;
using ExchangeApi.Infrastructure.Protocol;
using ExchangeApi.Infrastructure.Transport;

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

// 統一 IExchangeClient（bitFlyer 実装）
services.AddSingleton<IExchangeClient, BitflyerExchangeClient>();

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<IExchangeClient>();

// BTC/JPY の最新 ticker
var ticker = await client.GetTickerAsync("BTC/JPY");

Console.WriteLine($"Bid : {ticker.BestBid}");
Console.WriteLine($"Ask : {ticker.BestAsk}");
Console.WriteLine($"Time: {ticker.Timestamp:O}");
```

---

## 🎯 Stage1 の仕様（概要）

- サポート取引所：bitFlyer（Public）
- サポート API：`GET /v1/ticker`
- サポートシンボル：`BTC/JPY`
- DTO：`Ticker`
  - `Symbol`  
  - `BestBid`  
  - `BestAsk`  
  - `LastTradedPrice`  
  - `Timestamp` (`DateTimeOffset`)
- 例外ポリシー  
  - 未対応シンボル → `SymbolNotSupportedException`  
  - REST/HTTP エラー → `ExchangeApiException`

詳細は `/docs/A020`（要求）および `/docs/A040`（実装仕様）を参照してください。

---

## 🧪 テスト構成

```
tests/
 ├─ ExchangeApi.Abstractions.Tests
 ├─ ExchangeApi.Infrastructure.Tests
 ├─ ExchangeApi.Bitflyer.Tests
```

特に Stage1 で重要なテスト：

- `TickerTests`（DTO）
- `BitflyerExchangeClientTests`（Raw → Ticker）
- `RestClientTests`（path + query → URI 構築）

---

## 🧱 Stage2 への拡張予定

Stage1 の構造は、次の拡張を前提に設計されています：

- 私設 REST API（認証）
- WebSocket Ticker / Board
- 複数取引所の Orchestration 層
- マーケットデータ統合
- リトライ・レート制限・メトリクス（OpenTelemetry）

これらは Stage2 文書（S2xx 系）で定義予定です。

---

## 📄 ライセンス

MIT License  
（必要に応じて修正）

---

> **Stage1 Status:**  
> ExchangeApi は v1.0.0-stage1 をもって Stage1 を完了しています。  
> Stage2（認証/API拡張/WS/複数取引所統合）は S2xx 文書および v2 系タグで管理されます。
