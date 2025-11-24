# ExchangeApi Goal 1.0 (Vision)

本書は、**ExchangeApi が最終的に到達すべき Spec Version 1.0.0 のビジョン**を示す。
ここで定義する内容は「固定的な最終仕様」ではなく、**進行状況や学びに応じて調整され得る“生きたGoal”**である。
実装フェーズ（Stage1, Stage2, ...）は、すべてこの Goal を達成するためのステップとして位置づけられる。

最終的に本 Goal の要件をすべて満たした時点で、ExchangeApi 実装のバージョンを **v1.0.0** とする。

---

## 1. 目的

ExchangeApi は、複数の暗号資産取引所に対し、統一された API 抽象化を提供する C#/.NET ライブラリである。
Goal 1.0 では、**「複数取引所の Public / Private REST / WebSocket を統一的・拡張的に扱える API」** を完成形とする。

---

## 2. コアコンセプト

Goal 1.0 における ExchangeApi の中心思想は以下のとおり。

### ● 2.1 統一抽象化（Abstractions）

* Ticker / Board / Trades / Balance / Orders / Positions などを抽象化
* 各取引所の違いを隠蔽し、利用者は共通インターフェースで扱える

### ● 2.2 Transport と Adapter の完全分離

* REST / WebSocket 等の通信処理は **Infrastructure**
* 取引所固有の JSON → Abstraction 変換は **Adapter**
* 依存方向は常に Abstractions → Infrastructure → Adapter

### ● 2.3 Multi-Exchange 対応

* bitFlyer / Bybit / Binance など複数取引所を同時に扱える
* Orchestration 層で複数取引所を束ねる高レベル API を提供

### ● 2.4 Public / Private REST / WS の統一構造

* Public：Ticker / Board / Trades
* Private：Balance / Orders / Positions / User Stream
* WebSocket：マーケットデータ + 認証ストリーム

---

## 3. サポート取引所（到達イメージ）

Goal 1.0 では、以下の取引所を対象とする想定とする。

* bitFlyer（Public / Private / WS）
* Bybit（Public / Private / WS）
* Binance（Public / Private / WS）
* その他は Stage2 以降で拡張可能

---

## 4. サポート機能一覧（1.0 到達時点）

### 4.1 Public API

* Ticker（単一・複数）
* Board（Orderbook）
* Trades / Executions
* OHLC / Kline

### 4.2 Private API（認証）

* Balance / Collateral
* Wallet / Deposit / Withdraw
* Orders（新規 / 取消 / 一括取消）
* Positions（建玉管理）
* Executions（自己約定履歴）

### 4.3 Streaming API（WebSocket）

* Ticker Stream
* Board Stream
* Execution Stream
* User Stream（認証）
* 再接続 / 心拍監視 / サブスク管理

---

## 5. 非機能要件（Non-Functional Requirements）

Goal 1.0 における主要 NFR を以下に示す。

### 5.1 レイテンシ

* REST：200–500ms 程度の応答時間を想定
* WS：リアルタイム配信（数 ms〜数十 ms 遅延）

### 5.2 信頼性

* リトライ / ジッタ制御 / レート制限回避
* API 落ち時のフェイルオーバー（Stage3 以降）

### 5.3 ロギング・メトリクス

* HTTP 呼び出し成功率
* WS 再接続回数
* 遅延・タイムアウト計測
* OpenTelemetry 対応（Stage2 以降）

### 5.4 拡張性

* 新規取引所追加が Adapter のみで完結する構造
* Abstractions は安定し、破壊的変更を minimun に抑える

---

## 6. アーキテクチャ概要

Goal 1.0 の内部構造は以下とする。

```
Abstractions (Contracts, DTO)
        ↓
Infrastructure (REST, WS, Transport)
        ↓
Adapter (Exchange-specific)
        ↓
Orchestration (Multi-exchange integration)
```

* **Abstractions**：DTO / Interface / Errors
* **Infrastructure**：RestClient / WsClient / HttpTransport
* **Adapter**：bitFlyer / Bybit / Binance 実装
* **Orchestration**：ポートフォリオ統合・統一サブスク管理

---

## 7. バージョン到達条件（v1.0.0）

ExchangeApi の実装が以下を満たした時、**v1.0.0** とする：

* Public REST のすべてをサポート（Ticker〜OHLC）
* Private REST の主要機能（Balance / Orders / Positions）をサポート
* Public / Private の WebSocket を実装
* Orchestration 層で複数取引所を統合
* Abstraction が安定し破壊的変更が不要
* NFR（レイテンシ / リトライ / ロギング）を満たす

---

## 8. Stage とバージョンの対応

Goal と実装フェーズの関係は以下。

```
Goal 1.0 (Vision)
   ↑
Stage1 → v0.1.0 : Public Ticker のみ
Stage2 → v0.2.0 : Private REST / 部分実装
Stage3 → v0.3.0 : WS Ticker / Board Stream
Stage4 → v0.4.0 : Order / Position の強化
...
Goal 達成 → v1.0.0
```

---

## 9. まとめ

ExchangeApi Goal 1.0 は「複数取引所を統合的に扱える抽象 API」を完成形とする。
この Goal は進行に応じて改訂可能であり、実装は Stage1(v0.1.0) から段階的に近づいていく。
最終的に Goal の要件を満たした時点で、実装バージョンは v1.0.0 となる。
