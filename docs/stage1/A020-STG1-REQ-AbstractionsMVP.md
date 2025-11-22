# 0210-REQ-STG1-AbstractionsMVP（完成版）
Stage 1 要求仕様書 / Exchange API Library

---

## 1. 目的（Purpose）

Stage 1 の目的は、Exchange API Library の基盤となる **最小限の Abstractions と bitFlyer Public REST（Ticker）の取得機能**を実現し、将来の Stage（認証 / WebSocket / Board / Multi-Exchange）へ拡張可能な構造を確立することである。

本 Stage は **MVP（Minimum Viable Product）** とし、利用者が「単一取引所・単一シンボルの現在価格を取得できる」状態を最終成果物とする。

---

## 2. スコープ（Scope）

### 2.1 Stage 1 で扱う機能（In Scope）
- bitFlyer Public REST `GET /v1/getticker` を利用した **現在価格（Ticker）の取得**
- Abstractions 層の最小定義：
  - `IExchangeClient`
  - `Ticker` DTO
  - `Symbols`（例えば `BTC/JPY`）
- bitFlyer 専用 Adapter（Raw モデル＋マッピング）の最小実装
- symbol → product_code の変換ロジック（`BTC/JPY` ↔ `BTC_JPY`）

### 2.2 Stage 1 の対象外（Out of Scope）
以下は Stage 2 以降で扱う。
- 認証 REST API（残高取得／注文系）
- WebSocket（リアルタイム価格・板情報）
- Board / OrderBook / Balance / Position / Order DTO
- 複数取引所対応（Binance, Bybit 等）
- Rate limit 制御 / Retry / Circuit breaker
- Result 型（`Result<T>` / `ErrorOr<T>` 等）によるエラー表現
- OpenTelemetry / 高度なログ／メトリクス

---

## 3. 用語定義（Terminology）

- **Exchange**：暗号資産取引所。本 Stage では bitFlyer のみ。
- **Abstractions**：取引所非依存のインターフェースと DTO を提供する層。
- **Adapter**：取引所固有 API と Abstractions の間をマッピングする層。
- **Raw モデル**：取引所の JSON レスポンスを可能な限り欠損なく保持するデータ構造。
- **Ticker**：現在価格情報（best bid / best ask / last traded price など）。

---

## 4. レイヤ構造と依存ルール（Stage1 縮退モデル）

- 正典は **4 層構造（Abstractions → Adapters → Protocol → Transport）** だが、Stage1 では **Protocol / Transport を使用しない縮退版として Abstractions / Adapter / Raw の 3 層のみを採用**する。bitFlyer Public REST `getticker` のみを扱うため、共通 Protocol や高機能 Transport を導入しない。
- レイヤ間の依存方向および禁止ルールの正典は `A030-STG1-ARC-MinimalArchitecture.md` に定義される。

---

## 5. 機能要求（Functional Requirements, FR）

### FR-1: Ticker 取得 API（Abstractions）
- **FR-1-1**: `IExchangeClient` を定義する。
- **FR-1-2**: 次のメソッドを必ず提供する：
  ```csharp
  Task<Ticker> GetTickerAsync(string symbol, CancellationToken cancellationToken = default);
  ```
- **FR-1-3**: `symbol` の形式は `"BASE/QUOTE"`（大文字＋スラッシュ）を正とする。
- **FR-1-4**: Stage 1 で必須対応は **BTC/JPY のみ** とする。

### FR-2: Ticker DTO（共通データ構造）
- **FR-2-1**: Ticker DTO は次のプロパティを持つ：
  - `string Symbol`
  - `decimal BestBid`
  - `decimal BestAsk`
  - `decimal LastTradedPrice`
  - `DateTime TimestampUtc`（UTC）
- **FR-2-2**: Ticker DTO は Abstractions プロジェクトに配置し、取引所固有フィールドを持たないこと。
- REQ補足（Spec と整合）：bitFlyer の Raw レスポンスに含まれる `state`, `tick_id`, `volume` などの取引所固有フィールドは Stage1 の Ticker DTO に含めない。
  これらは Raw モデル側に保持し、将来の拡張に備える。Abstractions 層は一般化された最小限の情報のみ公開する。

### FR-3: Symbols（共通定数）
- **FR-3-1**: `BTC/JPY` を示す定数を Abstractions に定義する。
  - 例：`public const string BtcJpy = "BTC/JPY";`

### FR-4: bitFlyer Raw モデル（Adapter 内部）
- **FR-4-1**: bitFlyer の `GET /v1/getticker` のレスポンスを漏れなく保持する Raw モデルを定義する。
- **FR-4-2**: フィールド名・型は bitFlyer の API に忠実であること。
- **FR-4-3**: Raw モデルは **Adapter 内部専用** とする。

### FR-5: bitFlyer 公開 API インターフェース（Raw API）
- **FR-5-1**: `IBitflyerPublicApi` を定義する。
- **FR-5-2**: 次のメソッドを提供する：
  ```csharp
  Task<BitflyerTickerRaw> GetTickerRawAsync(string productCode, CancellationToken cancellationToken = default);
  ```

### FR-6: bitFlyer ExchangeClient（一般化マッピング）
- **FR-6-1**: `BitflyerExchangeClient` は `IExchangeClient` を実装する。
- **FR-6-2**: `symbol` ↔ `product_code` の変換を行う。
- **FR-6-3**: Raw モデルから一般化 Ticker へのマッピングを行う。

---

## 6. 非機能要求（Non-functional Requirements, NFR）

### NFR-0: ログ／メトリクス（Stage1 の基本方針）
- **NFR-0-1:** Abstractions 層はログ・メトリクスの責務を持たない（MUST）。
- **NFR-0-2:** Adapter 層はログ出力フック（ILogger など）を後から注入可能な構造であるべきだが、Stage1 では実装を必須としない（MAY）。
- **NFR-0-3:** OpenTelemetry 等のメトリクス連携は Stage2 以降の対象とする（OUT OF SCOPE）。

### NFR-1: スレッドセーフティ
- **NFR-1-1**: `IExchangeClient` の実装は、同一インスタンスを複数スレッドから利用可能な設計とする（推奨）。

### NFR-2: HTTP / Transport 方針
- **NFR-2-1**: `HttpClient` は外部注入（DI）を推奨し、内部で使い捨てしない。
- **NFR-2-2**: `CancellationToken` は `SendAsync` に伝播すること。
- **NFR-2-3**: JSON パースエラー／Content-Type 不一致時は許容範囲で処理し、それ以外は例外とする。
- **NFR-2-4**: `User-Agent` を明示的に設定する。
- **NFR-2-5**: タイムアウトは適切な値（5〜10 秒）を推奨とする。

### NFR-3: 例外ポリシー
- **NFR-3-1**: Stage 1 は **例外ベース** のエラー通知とする。
- **NFR-3-2**: 入力エラーは `ArgumentException` 系を使用する。
- **NFR-3-3**: API エラーは `ExchangeApiException` を用いて通知する。

NFR-2/3 は OVR 8 章および SPC 6・7 章と整合し、Stage1 共通ポリシーを維持する。

---

## 7. 対象外の正式宣言（Non-goals）
以下は Stage 1 の対象外であり、本 REQ による実装義務を負わない。
- 認証付き API（Balance / Order など）
- WebSocket（Board / Executions）
- 複数シンボルの対応
- Multi-exchange（Binance / Bybit）
- 複雑なエラーモデル・Result 型導入
- Retry / Circuit breaker など高機能 Transport
- 高度なログ基盤（OpenTelemetry, Structured logging）

---

## 8. Stage 1 完了条件（Definition of Done, DoD）
Stage 1 は以下を満たしたとき完了とする：

### DoD-1: Abstractions 実装
- `IExchangeClient`
- `Ticker`
- `Symbols`

### DoD-2: bitFlyer Adapter 実装
- Raw モデル（BitflyerTickerRaw）
- Raw API（IBitflyerPublicApi）
- ExchangeClient マッピング（BitflyerExchangeClient）

### DoD-3: 動作確認
- 実 API または HTTP モックを利用し、`GetTickerAsync(Symbols.BtcJpy)` が成功すること。

### DoD-4: エラー検証
- 無効 symbol に対して例外が発生すること。

### DoD-5: ドキュメント
- 本 REQ
- Stage1 SPEC
- README に Stage1 の使用例が記載

---

## 9. 関連文書（Reference Documents）
- `0120-OVR-INTR-ProjectOverview.md`
- `Stage1-spec-full`（詳細仕様）
- `0800-STD-DOC0-DocumentPolicy.md`

---

## 改訂履歴

| 版 | 日付 | 内容 |
|----|------|------|
| v1.1.1 | 2025-05-05 | HTTP/例外ポリシーが OVR/SPC と整合することを明示し、改訂履歴を更新。 |
| v1.1.0 | 2025-05-05 | Stage1 縮退構造の明記、依存ルール参照の統一、章番号の更新を実施。 |
