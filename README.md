# ExchangeApi

ExchangeApi は、複数の暗号資産取引所向けに統一インターフェースを提供する C#/.NET ライブラリです。  
Stage4 時点で **bitFlyer の Public/Private REST** に対応し、以下を提供します。

- Ticker/Board
- 残高・証拠金
- 発注（MARKET/LIMIT/STOP/STOP_LIMIT）、キャンセル/全キャンセル
- オープン注文・約定・ポジション一覧

詳しい使い方は Quick Start / Entry Guide を参照してください。

---

## 🏗 プロジェクト構成（Stage4 時点）

```
ExchangeApi.Abstractions      ← 仕様（Boundary）
ExchangeApi.Infrastructure    ← REST / HTTP Transport（共通）
ExchangeApi.Bitflyer          ← bitFlyer Adapter（Raw → DTO マッピング）
ExchangeApi.Orchestration     ← 将来の複数取引所統合用（最小構成）
```

依存方向は必ず以下を守ります：

```
Abstractions  ←  Infrastructure  ←  Bitflyer
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

## 🎯 Stage4 の概要

- 取引所: bitFlyer
- Public: `GET /v1/getticker`, `GET /v1/getboard`
- Private: 残高/証拠金/ポジション/約定/オープン注文、`sendchildorder`, `cancelchildorder`, `cancelallchildorders`
- DTO: `Ticker`, `Board`, `Balance`, `Collateral`, `Position`, `Execution`, `OpenOrder`, `OrderRequest/Result`
- 例外: `SymbolNotSupportedException`（シンボル未対応）、`ExchangeApiException`（HTTP/取引所エラー）

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

> **Stage1 Status:**  
> ExchangeApi は v1.0.0-stage1 をもって Stage1 を完了しています。  
> Stage2（認証/API拡張/WS/複数取引所統合）は S2xx 文書および v2 系タグで管理されます。
