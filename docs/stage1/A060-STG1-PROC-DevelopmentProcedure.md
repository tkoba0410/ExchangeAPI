---

doc_id: A060-STG1-PROC-DevelopmentProcedure
title: Stage1 開発手順書（PROC）
version: 2.0.0
status: Draft
stage: Stage1
-------------

# A060-STG1-PROC-DevelopmentProcedure

Stage1 開発手順書（Development Procedure）

本書は Exchange API Library の **Stage1（bitFlyer Public REST / Ticker）** を開発する際の
具体的な作業手順を定義する。A010（OVR）・A020（REQ）・A030（ARC）・A040（SPC）・A050（DEV）で定義された
目的・要求・構造・仕様・ガイドラインに基づき、開発者が迷わず着手できるよう、
必要なステップを明確に記述する。

Stage1 は最小実装フェーズであり、**軽量・最短で動作まで到達すること**を目的とする。

---

# 1. 目的（Purpose）

* Stage1 の開発を、誰が行っても同じ結果になるよう標準化する。
* Boundary（Abstractions）→ Infrastructure（REST/HTTP）→ Adapter（bitFlyer）
  の順に構築し、依存方向と責務が必ず正しくなるよう導く。
* 実装・テスト・ドキュメント整合のタイミングを揃え、Stage2 移行前に破綻をなくす。

---

# 2. 前提（Prerequisites）

* .NET 10.0 SDK がインストールされていること。
* git リポジトリが初期化済みであること。
* ソリューションは以下の構造を持つ：

```text
src/
  ExchangeApi.Abstractions/
  ExchangeApi.Infrastructure/
  ExchangeApi.Bitflyer/

tests/
  ExchangeApi.Abstractions.Tests/
  ExchangeApi.Infrastructure.Tests/
  ExchangeApi.Bitflyer.Tests/
```

* Stage1 の設計仕様（A010〜A050）が最新状態で整合していること。

---

# 3. 開発全体の流れ（Overview）

Stage1 は次の 5 ステップで進める。

1. Abstractions の実装
2. Raw モデルの実装（bitFlyer JSON 写像）
3. REST / HTTP 技術モジュール（Infrastructure）の実装
4. bitFlyer Adapter の実装（Raw → Ticker）
5. テスト作成と確認

この順番は **依存方向（ARC 正典）に対応している** ため、常にこの流れを遵守する。

---

# 4. 詳細手順（Procedures）

## Step 1: Abstractions を実装する（Boundary）

### 1-1. `IExchangeClient` を作成

* `GetTickerAsync(string symbol, CancellationToken)` を定義。
* symbol 形式は "BASE/QUOTE"（大文字・スラッシュ区切り）。
* Stage1 では `BTC/JPY` のみ必須。

### 1-2. `Ticker` DTO を作成

```csharp
public sealed record Ticker(
    string Symbol,
    decimal BestBid,
    decimal BestAsk,
    decimal LastTradedPrice,
    DateTime TimestampUtc);
```

### 1-3. `Symbols` を作成

* `public const string BtcJpy = "BTC/JPY";` を定義。

### 1-4. 共通例外を追加

* `ExchangeApiException`
* `SymbolNotSupportedException`

**完了条件（DoD）**

* Abstractions プロジェクトが依存先ゼロでビルド可能になっている。
* DTO とインターフェースが A020（REQ）および A040（SPC）と一致している。

---

## Step 2: Raw モデル（BitflyerTickerRaw）を実装する

### 2-1. bitFlyer JSON を写した Raw モデルを作成

* `BitflyerTickerRaw` クラスに、`GET /v1/getticker` の全フィールドを定義。
* 例：`ProductCode`, `Timestamp`, `BestBid`, `BestAsk`, `LastTradedPrice`, `Volume` など。

### 2-2. Raw モデルの配置

* `ExchangeApi.Bitflyer` プロジェクト内 `Models/` ディレクトリに配置。
* Abstractions・Infrastructure から参照されないようにする。

**完了条件（DoD）**

* Raw モデルが bitFlyer 仕様と一致している。
* Adapter 以外から参照されていないことを確認。

---

## Step 3: REST / HTTP 技術モジュール（Infrastructure）を実装する

### 3-1. `IRestClient` を作成

```csharp
Task<TResponse> GetAsync<TResponse>(
    string path,
    IReadOnlyDictionary<string, string?>? query = null,
    CancellationToken cancellationToken = default);
```

* GET リクエストを送信し、JSON を `TResponse` にデシリアライズして返す。
* Content-Type が json でない場合は例外。

### 3-2. `RestClient` の実装

* Path + Query から URL を生成。
* `IHttpTransport` 経由で HTTP 通信を行う。
* HTTP ステータスが 2xx 以外の場合は例外を投げる。

### 3-3. `IHttpTransport` / `HttpTransport` の実装

* `HttpClient.SendAsync` をラップする。
* `CancellationToken` を確実に伝播する。
* 本番では DI（非破棄）で提供する前提とする。

**完了条件（DoD）**

* Infrastructure 全体がビルド成功し、Abstractions のみに依存している。

---

## Step 4: bitFlyer Adapter を実装する

### 4-1. `IBitflyerPublicApi` を定義

```csharp
Task<BitflyerTickerRaw> GetTickerRawAsync(string productCode, CancellationToken ct);
```

### 4-2. `BitflyerPublicApi` の実装

* REST パス `/v1/getticker` を使用。
* Query パラメータ `product_code=BTC_JPY` を付与。
* JSON → `BitflyerTickerRaw` にデシリアライズ。

### 4-3. `BitflyerExchangeClient` の実装

1. symbol 検証（null/空白 → ArgumentException）
2. 対応シンボル以外は `SymbolNotSupportedException`
3. `"BTC/JPY"` → `"BTC_JPY"` に変換
4. `IBitflyerPublicApi.GetTickerRawAsync` を呼び出す
5. Raw → Ticker にマッピング
6. 例外（HTTP/JSON）があれば `ExchangeApiException` として通知

**完了条件（DoD）**

* Adapter が Abstractions / Infrastructure の両方を参照し、逆依存が存在しない。
* Ticker 取得がローカル実装ではなく bitFlyer API を実際に通して行われる（実通信 or モック）。

---

## Step 5: テストを作成・実行する

### 5-1. Abstractions.Tests

* `Ticker` DTO の生成
* `symbol` 検証（`ArgumentException`）

### 5-2. Infrastructure.Tests

* `RestClient` の JSON デシリアライズ
* HTTP 例外の検証（ステータスコード ≠ 2xx）

### 5-3. Bitflyer.Tests

* Raw → Ticker のマッピング
* `GetTickerAsync("BTC/JPY")` の正常系
* 未対応 symbol の例外テスト
* モック or 実通信

**完了条件（DoD）**

* すべてのテストが green である。
* REQ / ARC / SPC の対応表が作成できる状態になっている。

---

# 5. 作業完了後の確認（Checklist）

* [ ] Abstractions が完全に独立（ゼロ依存）である
* [ ] Infrastructure が Abstractions のみを参照している
* [ ] Bitflyer が Abstractions / Infrastructure を参照している
* [ ] Raw モデルが Bitflyer 内に閉じている
* [ ] Ticker が A020 / A040 と一致している
* [ ] Adapter の処理が A040 のマッピング仕様どおり
* [ ] README に使用例がある
* [ ] docs/ に A010〜A060 が揃っている

---

# 6. Stage2 への引き継ぎ

Stage1 完了後、次の作業を Stage2 として進める。

* Transport の強化（Retry / RateLimit / CircuitBreaker）
* Protocol の導入（署名生成 / timestamp / nonce）
* 認証 REST（Balance / Order / Position）
* WebSocket（Board / Executions / Streaming Ticker）
* 複数取引所 Adapter の追加（`ExchangeApi.Binance` 等）
* Orchestration 層の本格実装

Stage1 の構造はそのまま Stage2 の成長点となる。

---

# 7. 改訂履歴

| 版     | 日付         | 内容                                                                     |
| ----- | ---------- | ---------------------------------------------------------------------- |
| 2.0.0 | 2025-11-XX | Stage1 の設計方針に合わせ全面改訂。Boundary → Infrastructure → Adapter の順による標準手順を確立。 |
