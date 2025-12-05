---

doc_id: A030-STG1-ARC-MinimalArchitecture
title: Stage1 最小アーキテクチャ構成（ARC）
version: 2.0.0
status: Draft
stage: Stage1
-------------

# A030-STG1-ARC-MinimalArchitecture

Stage1 最小アーキテクチャ構成（Architecture Specification）

本書は Exchange API Library における **Stage1（bitFlyer Public REST / Ticker）** の
アーキテクチャ構成を定義する。A010（OVR）および A020（REQ）で定めた目的・要求に従い、
ソリューション構成・依存方向・責務分割を明文化する。

Stage1 の対象は「bitFlyer Public REST `GET /v1/getticker` による Ticker 取得」のみであり、
ここで定義する構造は **今後 Stage2 での拡張（認証 / WebSocket / 複数取引所 / Transport/Protocol 強化）を前提にした最小構成**である。
※ Stage4 以降の命名: ExchangeApi.Core（旧 Abstractions）、ExchangeApi.Transport（旧 Infrastructure/Protocol/Transport）、ExchangeApi.Adapter.Bitflyer（旧 Adapter/Bitflyer）、ExchangeApi.Factory（旧 Orchestration）。本書の記述は旧名称ベースだが、現在はこの対応で整理する。

---

## 1. 目的（Purpose）

* Stage1 に必要な **最小限のアーキテクチャ構造**を定義する。
* コードとドキュメントの間で **依存方向と責務が常に一致するようにする**。
* 将来の Stage2 において、Stage1 の構造を壊さずに拡張できるようにする。

本書は「どのプロジェクトが何を担当し、どこに依存できるか」を定める。

---

## 2. スコープ（Scope）

### 2.1 対象

* Stage1 のプロジェクト構成

  * `ExchangeApi.Core`
  * `ExchangeApi.Transport`
  * `ExchangeApi.Adapter.Bitflyer`
* これらの依存関係および責務
* Raw モデル（bitFlyer 固有モデル）の扱い

### 2.2 対象外（Stage2 以降で扱う）

* 認証 REST（Balance / Order / Position）
* WebSocket（Board / Executions / Streaming Ticker）
* 複数取引所（Binance / Bybit 等）の具体的アーキテクチャ
* 高度な Transport / Protocol（Retry / RateLimit / CircuitBreaker / 署名生成 等）
* Orchestration 層（複数取引所・複数アカウントの束ね層）の詳細設計

---

## 3. 基本構造（Boundary + Technical Modules）

Stage1 のアーキテクチャは、**契約境界（Boundary）** と **技術モジュール（Technical Modules）** の
2 種類のコンポーネントで構成される。

### 3.1 全体構造

```text
                ┌─────────────────────────┐
                │  Boundary / Abstractions │
                │  (Interfaces + DTOs)      │
                └────────────▲──────────────┘
                             │
              ┌──────────────┼───────────────┐
              │              │               │
┌──────────────┴──────┐ ┌──────┴────────┐ ┌───────────────┴───────┐
│ Adapter (bitFlyer)   │ │ Protocol (REST) │ │ Transport (HTTP Client) │
└──────────────────────┘ └────────────────┘ └──────────────────────────┘
                        （ExchangeApi.Transport 内）
```

* **Boundary / Abstractions**

  * 取引所非依存のインターフェース・DTO・例外を定義する。
  * 他プロジェクトに依存しない。
* **Adapter (bitFlyer)**

  * Abstractions を実装し、bitFlyer API とやりとりする。
  * Raw モデルから Ticker へのマッピングを行う。
* **Protocol (REST)** / **Transport (HTTP)**

  * REST 呼び出しと HTTP 通信を担う技術モジュール。
  * Stage1 では GET + JSON デシリアライズの最小機能のみを提供する。

Raw モデル（`BitflyerTickerRaw`）は bitFlyer Adapter 内部の構造として扱い、外部には公開しない。

---

## 4. プロジェクト構成（Project Structure）

Stage1 では、ソリューションは少なくとも次のプロジェクトを含むものとする。

```text
src/
  ExchangeApi.Core/
  ExchangeApi.Transport/
  ExchangeApi.Adapter.Bitflyer/

tests/
  ExchangeApi.Core.Tests/
  ExchangeApi.Transport.Tests/
  ExchangeApi.Adapter.Bitflyer.Tests/
```

### 4.1 ExchangeApi.Core（Boundary）

* 役割：取引所非依存の契約境界を定義する。
* 主な内容：

  * `IExchangeClient`（インターフェース）
  * `Ticker` DTO
  * `Symbols` 定数（`BTC/JPY` 等）
  * 共通例外型（`ExchangeApiException` / `SymbolNotSupportedException` 等）
* 特徴：

  * 他のプロジェクトに依存しない（依存先ゼロ）。
  * HTTP / JSON / 認証などの技術的関心事を含まない。

### 4.2 ExchangeApi.Transport（Protocol + Transport）

* 役割：REST 通信および HTTP 通信の技術モジュールを提供する。
* 主な内容：

  * Protocol

    * `IRestClient` / `RestClient`
  * Transport

    * `IHttpTransport` / `HttpTransport`
* 特徴：

  * Abstractions に依存してもよい（例：例外型の再利用）。
  * Adapter 側から利用されるが、取引所固有ロジックは持たない。
  * Stage1 では GET + JSON デシリアライズの最小機能に限定する。

### 4.3 ExchangeApi.Adapter.Bitflyer（Adapter）

* 役割：bitFlyer Public REST API を呼び出し、Raw モデルを Ticker に変換する。
* 主な内容：

  * `BitflyerExchangeClient`（`IExchangeClient` 実装）
  * `IBitflyerPublicApi` / `BitflyerPublicApi`
  * Raw モデル `BitflyerTickerRaw`
  * symbol ↔ product_code 変換ロジック
* 特徴：

  * Abstractions / Infrastructure の両方に依存してよい。
  * Raw モデルはこのプロジェクト内部専用。

---

## 5. 依存関係ルール（Dependency Rules）

Stage1〜Stage2 を通じて、次の依存方向ルールを **不変の正典** とする。

### 5.1 プロジェクト間依存

```text
ExchangeApi.Core      ←  ExchangeApi.Transport
            ▲                 ←  ExchangeApi.Adapter.Bitflyer
            │
        （上位）
```

* `ExchangeApi.Core`

  * 他プロジェクトに依存してはならない（MUST NOT）。
* `ExchangeApi.Transport`

  * `ExchangeApi.Core` に依存してよい（MUST）。
  * `ExchangeApi.Adapter.Bitflyer` に依存してはならない（MUST NOT）。
* `ExchangeApi.Adapter.Bitflyer`

  * `ExchangeApi.Core` および `ExchangeApi.Transport` に依存してよい（MUST）。

### 5.2 Raw モデルの依存

* Raw モデル（`BitflyerTickerRaw`）は `ExchangeApi.Adapter.Bitflyer` 内部の型とし、
  他プロジェクトから参照されてはならない（MUST NOT）。
* Raw モデルは Abstractions への依存を持ってはならない（MUST NOT）。

### 5.3 コードレベルの依存

* `IExchangeClient` と `Ticker` は `ExchangeApi.Core` にのみ定義する（MUST）。
* Adapter 実装（`BitflyerExchangeClient`）は、ビジネスロジック層やアプリケーション層に依存しない（SHOULD）。

---

## 6. レイヤ構造（論理レイヤ）

Stage1 の論理レイヤは、次の 3 段階で理解できる。

```text
+---------------------------+
| Boundary / Abstractions   |
| (IExchangeClient, Ticker) |
+---------------------------+
            ▲
            │
+---------------------------+
| Adapter (bitFlyer)        |
| - BitflyerExchangeClient  |
| - IBitflyerPublicApi      |
+---------------------------+
            ▲
            │
+---------------------------+
| Technical Modules         |
| - RestClient              |
| - HttpTransport           |
+---------------------------+
```

* 「層」というよりも、上位に Boundary があり、
  下位に Adapter / Technical Modules がぶら下がる構造である。
* Adapter は bitFlyer 固有の API 仕様を扱い、Technical Modules は HTTP/REST を汎用的に扱う。

---

## 7. 責務分割（Responsibilities）

### 7.1 Abstractions

* 取引所非依存の IExchangeClient 契約を提供する。
* 共通 DTO（Ticker）および共通例外を提供する。
* 取引所固有の仕様は一切含まない。

### 7.2 Infrastructure

* HTTP 通信（HttpClient ラップ）を提供する。
* REST 通信の共通パターン（リクエスト生成・レスポンス処理）を提供する。
* Adapter から利用されるが、取引所固有ロジックを持たない。

### 7.3 Bitflyer Adapter

* `IBitflyerPublicApi` を通じて bitFlyer Public REST `getticker` を呼び出す。
* `BitflyerTickerRaw` にレスポンスをマッピングする。
* `BitflyerTickerRaw` から Abstractions の `Ticker` へマッピングする。
* `"BTC/JPY"` ↔ `"BTC_JPY"` の変換を行う。

---

## 8. Stage2 への拡張ポイント（参考）

Stage2 以降での拡張は、次の方針で行う。

* `ExchangeApi.Transport` において Transport / Protocol を強化する。

  * Retry / RateLimit / CircuitBreaker
  * 認証付き REST（署名 / timestamp / nonce 等）
* `ExchangeApi.Adapter.Bitflyer` において Private API / WebSocket / Board などの Adapter を段階的に追加する。
* 新規取引所（`ExchangeApi.Binance` 等）は、Bitflyer と同じパターンで Adapter プロジェクトを追加する。
* Boundary（Abstractions）は、必要に応じて DTO / インターフェースを拡張するが、
  Stage1 の基本構造（IExchangeClient / Ticker）の互換性を保つ。

---

## 9. アーキテクチャ DoD（Architecture Definition of Done）

Stage1 におけるアーキテクチャが完了したとみなす条件：

1. プロジェクト構成が `Abstractions` / `Infrastructure` / `Bitflyer` の 3 つに分離されている。
2. 依存方向が本書のルールに従っている（逆依存が存在しない）。
3. `IExchangeClient` / `Ticker` / `Symbols` が Abstractions に定義されている。
4. `BitflyerExchangeClient` / `IBitflyerPublicApi` / `BitflyerTickerRaw` が Bitflyer に存在する。
5. REST 通信が `IRestClient` / `IHttpTransport` を介して行われている。
6. REQ / SPC / DEV / PROC との間でアーキテクチャ上の矛盾がない。

---

## 10. 改訂履歴

| 版     | 日付         | 内容                                                                           |
| ----- | ---------- | ---------------------------------------------------------------------------- |
| 2.0.0 | 2025-11-XX | Stage1 実装と Stage2 方針に合わせて全面改訂。Boundary + Technical Modules に基づく最小アーキテクチャを定義。 |
