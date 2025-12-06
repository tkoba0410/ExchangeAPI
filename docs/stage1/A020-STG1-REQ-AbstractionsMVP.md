---

doc_id: A020-STG1-REQ-AbstractionsMVP
title: Stage1 要求仕様書（Abstractions + bitFlyer Ticker）
version: 2.0.0
status: Draft
stage: Stage1
-------------

# A020-STG1-REQ-AbstractionsMVP

Stage1 要求仕様書 / Exchange API Library

本書は Exchange API Library における **Stage1（bitFlyer Public REST / Ticker）** の
要求仕様を定義する。A010-STG1-OVR-Overview で定義された目的・構造にもとづき、
実装およびテストが満たすべき条件を明文化する。

---

## 1. 目的（Purpose）

Stage1 の目的は、Exchange API Library の基盤となる **取引所非依存の Abstractions** と
**bitFlyer Public REST `GET /v1/getticker` による Ticker 取得機能** を、最小限の構成で
提供することである。

本 Stage の完成により、利用者は次を満たすことができる：

* 単一取引所（bitFlyer）・単一シンボル（BTC/JPY）の Ticker を取得できる
* 将来の Stage2（認証 API・WebSocket・複数取引所・Transport/Protocol 拡張）へ
  破綻なく拡張できる

---

## 2. スコープ（Scope）

### 2.1 対象（In Scope）

* Boundary / Abstractions の最小定義

  * `IExchangeClient`
  * `Ticker` DTO
  * `Symbols`（`BTC/JPY` 定数）
* bitFlyer Adapter

  * `BitflyerExchangeClient`（IExchangeClient 実装）
  * `IBitflyerPublicApi` + 実装
  * Raw モデル `BitflyerTickerRaw`
* REST 呼び出し用の最小技術モジュール

  * `IRestClient` / `RestClient`
  * `IHttpTransport` / `HttpTransport`
* symbol ↔ product_code（`BTC/JPY` ↔ `BTC_JPY`）の静的変換

### 2.2 対象外（Out of Scope）

以下は Stage1 の要求から明示的に除外し、Stage2 で扱う。

* 認証 REST API（Balance / Order / Position 等）
* WebSocket（Board / Executions / Realtime Ticker）
* 複数取引所（Binance / Bybit など）
* Transport 拡張（Retry / RateLimit / CircuitBreaker 等）
* Protocol 拡張（署名生成・timestamp/nonce 等）
* Result 型や高度なエラー表現
* OpenTelemetry など高度なロギング／メトリクス

---

## 3. 用語定義（Terminology）

* **Exchange**: 暗号資産取引所。本 Stage では bitFlyer のみ対象。
* **Boundary / Abstractions**: 取引所非依存のインターフェース・DTO・例外を定義する部分。
* **Adapter**: 取引所固有 API と Abstractions の間をマッピングする実装。
* **Technical Modules**: REST 通信や JSON シリアライズなどの技術的関心事を担うモジュール。
* **Raw モデル**: 取引所レスポンス JSON を欠損なく保持するデータ構造。
* **Ticker**: 現在価格情報（BestBid / BestAsk / LastTradedPrice / Timestamp）をまとめた DTO。

---

## 4. 構造と依存ルール（Structural Requirements）

### 4.1 プロジェクト構成

Stage1 におけるプロジェクトは最低限以下を含むものとする。

* `ExchangeApi.Contracts`  … Boundary（依存先なし）
* `ExchangeApi.Transport` … REST 向け Technical Modules
* `ExchangeApi.Adapter.Bitflyer`       … bitFlyer Adapter

### 4.2 依存方向（MUST）

* `ExchangeApi.Contracts` は他プロジェクトに依存してはならない（MUST NOT）。
* `ExchangeApi.Transport` は `ExchangeApi.Contracts` へ依存してよい（MUST）。
* `ExchangeApi.Adapter.Bitflyer` は `ExchangeApi.Contracts` と `ExchangeApi.Transport` に依存してよい（MUST）。
* Raw モデル（`BitflyerTickerRaw`）は `ExchangeApi.Adapter.Bitflyer` 内部の型であり、他プロジェクトから参照しない（MUST）。

---

## 5. 機能要求（Functional Requirements, FR）

### FR-1: `IExchangeClient` インターフェース

* **FR-1-1**: `IExchangeClient` インターフェースを Abstractions に定義しなければならない（MUST）。
* **FR-1-2**: `IExchangeClient` は次のメソッドを公開しなければならない（MUST）。

```csharp
Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default);
```

* **FR-1-3**: `symbol` は "BASE/QUOTE" 形式の大文字文字列で表現しなければならない（MUST）。

  * Stage1 で必須対応とするのは `"BTC/JPY"` のみとする。

### FR-2: Ticker DTO（共通データ構造）

* **FR-2-1**: `Ticker` DTO は Abstractions に属し、少なくとも次のプロパティを持たなければならない（MUST）。

  * `string Symbol`
  * `decimal BestBid`
  * `decimal BestAsk`
  * `decimal LastTradedPrice`
  * `DateTimeOffset Timestamp` （UTC）

* **FR-2-2**: Stage1 の `Ticker` は取引所固有フィールド（例: `tick_id`, `state`, `volume`）を含めてはならない（MUST NOT）。
  これらは Raw モデル側で保持し、将来必要に応じて Abstractions に追加する。

### FR-3: Symbols（共通定数）

* **FR-3-1**: Abstractions に `Symbols` などのクラスを定義し、`BTC/JPY` を表す定数を定義しなければならない（MUST）。

  * 例：`public const string BtcJpy = "BTC/JPY";`

### FR-4: bitFlyer Raw モデル

* **FR-4-1**: bitFlyer の `GET /v1/getticker` レスポンスを欠損なく保持する `BitflyerTickerRaw` を定義しなければならない（MUST）。
* **FR-4-2**: `BitflyerTickerRaw` のフィールド名・型は bitFlyer 公式仕様に合わせること（SHOULD）。
* **FR-4-3**: `BitflyerTickerRaw` は `ExchangeApi.Adapter.Bitflyer` プロジェクト内に配置し、外部に公開しない（MUST）。

### FR-5: bitFlyer 公開 API インターフェース

* **FR-5-1**: bitFlyer の Public REST を呼び出すためのインターフェース `IBitflyerPublicApi` を定義しなければならない（MUST）。
* **FR-5-2**: `IBitflyerPublicApi` は次のメソッドを公開しなければならない（MUST）。

```csharp
Task<BitflyerTickerRaw> GetTickerRawAsync(string productCode, CancellationToken cancellationToken = default);
```

* **FR-5-3**: `productCode` は "BTC_JPY" のような取引所仕様の表現形式とする（SHOULD）。

### FR-6: bitFlyer ExchangeClient 実装

* **FR-6-1**: `BitflyerExchangeClient` は `IExchangeClient` を実装しなければならない（MUST）。
* **FR-6-2**: `BitflyerExchangeClient.GetTickerAsync` は、`symbol` ↔ `productCode` の変換と `Raw → Ticker` のマッピングを担わなければならない（MUST）。
* **FR-6-3**: 対応していない `symbol` が渡された場合、`SymbolNotSupportedException` などの適切な例外をスローしなければならない（MUST）。

### FR-7: symbol ↔ product_code 変換

* **FR-7-1**: 少なくとも `BTC/JPY` と `BTC_JPY` を相互変換できるロジックを持たなければならない（MUST）。
* **FR-7-2**: Stage1 時点では `BTC/JPY` のみ必須とし、その他のシンボルは未対応でもよい（MAY）。

---

## 6. 非機能要求（Non-functional Requirements, NFR）

### NFR-1: スレッドセーフティ

* **NFR-1-1**: `IExchangeClient` の実装は、単一インスタンスを複数スレッドから利用可能であることが望ましい（SHOULD）。

### NFR-2: HTTP / Transport 方針

* **NFR-2-1**: REST 通信は `IRestClient` / `IHttpTransport` を通じて行い、直接 `HttpClient` に依存してはならない（MUST NOT）。
* **NFR-2-2**: `HttpClient` は DI から供給し、使い捨てしない構成にする（SHOULD）。
* **NFR-2-3**: `CancellationToken` は HTTP 送信まで確実に伝播しなければならない（MUST）。
* **NFR-2-4**: `User-Agent` は明示的に設定することが望ましい（SHOULD）。

### NFR-3: 例外ポリシー

* **NFR-3-1**: Stage1 は例外ベースのエラー通知とし、Result 型などの高度な表現は採用しない（MUST）。
* **NFR-3-2**: 入力エラー（無効な `symbol` 等）は `ArgumentException` 系または `SymbolNotSupportedException` で通知しなければならない（MUST）。
* **NFR-3-3**: HTTP ステータスエラー・JSON パースエラー等の API 失敗は `ExchangeApiException` で通知しなければならない（MUST）。

### NFR-4: ログ／メトリクス

* **NFR-4-1**: Stage1 では高度なログ／メトリクス基盤は要求しない（MAY）。
* **NFR-4-2**: 今後の拡張に備え、`ILogger` などを注入可能な形にしておくことが望ましい（SHOULD）。

---

## 7. Stage1 完了条件（Definition of Done, DoD）

Stage1 は次の条件を満たしたとき完了とみなす。

### DoD-1: Abstractions

* `IExchangeClient` / `Ticker` / `Symbols` が実装されている。

### DoD-2: bitFlyer Adapter

* `BitflyerTickerRaw` / `IBitflyerPublicApi` / `BitflyerExchangeClient` が実装されている。
* `GetTickerAsync("BTC/JPY")` が成功し、bitFlyer 実 API または HTTP モックを用いたテストが存在する。

### DoD-3: エラー動作

* 未対応 `symbol` に対して例外が発生するテストが存在する。

### DoD-4: ドキュメント

* A010（OVR）と本書（REQ）、ARC/SPC/DEV/PROC が Stage1 の内容に整合している。
* README に `GetTickerAsync("BTC/JPY")` の使用例が掲載されている。

---

## 8. 改訂履歴

| 版     | 日付         | 内容                                                                         |
| ----- | ---------- | -------------------------------------------------------------------------- |
| 2.0.0 | 2025-11-XX | Stage1 実装に合わせて全面改訂。Abstractions / bitFlyer / Infrastructure を前提とした要求仕様を定義。 |

---

> **Stage1 Freeze:**  
> 本ドキュメント群（A010〜A060）および関連コードは、本版をもって Stage1 の仕様・実装として確定とする。  
> 今後の機能追加・仕様変更は Stage2（S2xx 系）文書で管理する。
