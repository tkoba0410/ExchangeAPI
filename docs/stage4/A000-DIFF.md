# A090-STG4-SUMM（Rev2）
# Stage4 旧版 → 新版（Rev2）差分サマリ

本書は、既存 Stage4 文書（A010〜A070）と今回再構成した **Stage4（Rev2）** の内容差分を整理したものである。  
ExchangeAPI の Stage4 の目的を「REST+WS 抽象層 API の確定」に再定義し、旧版との設計思想・スコープの差異を文書化する。

---

# 1. Stage4 の目的（Purpose）の違い

## ■ 旧 Stage4
- Private GET/POST の横展開が主目的。
- Positions / Executions / Collateral / OpenOrders / Cancel の対応。
- 注文モデル（OrderRequest）の拡張（LIMIT, STOP, TIF等）。
- bitFlyer Private API の網羅と安定化を進めるステージ。
- REST の強化が中心、抽象 API の体系化は未完成。

## ■ 新 Stage4（Rev2）
- **ExchangeAPI の抽象 API（REST+WS）を正式に確定し凍結するステージに再定義。**
- Market / Trading / Account(SPOT) / Account(Margin) / WS / ExchangeInfo の 6 区分へ明確分割。
- 「薄い API」として責務境界を最小化し、実装は Stage5 以降へ。
- Margin は「建玉取得」と「証拠金サマリ」のみに限定し、深い抽象化を避ける方針へ変更。

---

# 2. インターフェース構造の差分

## ■ 旧 Stage4 のインターフェース傾向
- TradingClient（統合的なインターフェース）に多くの責務が集約。
- MarketData / Account / Trading の役割境界が曖昧。
- WS 抽象は Stage4 では扱われない。
- Margin の扱いは Private GET の拡張として整理されていた。

## ■ 新 Stage4（Rev2）で採用した構造
以下のように **機能カテゴリで完全分割** し、「拡張インターフェース」で差分を吸収。

### REST（Market Data）
- `IMarketDataApi`
  - GetTicker
  - GetOrderBook
  - GetExecutions

### REST（Trading）
- `ITradingApi`
  - SendOrder
  - CancelOrder
  - GetOpenOrders

### REST（Account - Spot）
- `IAccountApi`
  - GetBalances

### REST（Account - Margin：Spot の拡張能力）
- `IMarginAccountApi : IAccountApi`
  - GetOpenPositions
  - GetCollateral

### WebSocket 抽象（新規）
- `IRealtimeMarketDataApi`
  - SubscribeTicker
  - SubscribeOrderBook
  - SubscribeExecutions

### Exchange Info（将来用入口）
- `IExchangeInfoApi`

**差分の要点**  
- API を役割別にモジュール化  
- 継承（拡張能力）による Margin 対応  
- WS を Stage4 の抽象に含めた（旧版には無い重要拡張）

---

# 3. Stage4 のスコープ差分

## ■ 旧版のスコープ
- Private API 横展開（Positions / Executions / Collateral / OpenOrders）。
- 注文モデルの強化（LIMIT / STOP / 時間条件）。
- E2 エラー分類の適用。
- 署名・Cancel 処理・DTO/Domains の整備。

## ■ 新版のスコープ
- **抽象設計の確定にフォーカス。実装は Stage5 に送る。**
- Core（Abstractions）は以下を Stage4 完了条件とする：
  - IMarketDataApi / ITradingApi / IAccountApi / IMarginAccountApi / IRealtimeMarketDataApi / IExchangeInfoApi
  - Domain 型：Ticker / OrderBook / Execution / Position / Collateral / OrderRequest / OrderResult 等
  - Margin は「最低限（建玉・証拠金）」に限定
- Raw API の明確化（抽象化不能領域の切り出し）

**差分の要点：**  
旧：REST 実装寄り  
新：**REST+WS 抽象 API の体系化と凍結**

---

# 4. アーキテクチャ差分

## ■ 旧版
- 横展開構造を前提に、Abstractions → Adapter → RestClient を中心とした構造。
- 抽象と実装の境界はあるが、役割分解は粗め。

## ■ 新版（Rev2）
- Abstractions 層を明確に 6 種へ分割。
- Margin を継承構造で表現し、抽象の肥大化を防止。
- WS を正式に抽象として取り込み、REST と責務分離。
- ExchangeInfo は skeleton のみ配置し、責務拡大を避ける方針を明示。

**差分の本質：抽象設計が“REST横展開 → レイヤ構造の正式化”へ進化した**。

---

# 5. API マッピングの差分

## ■ 旧版
- Private GET/POST の mapping 中心。
- Ticker / Board / Executions は REST 側 mapping の一部に留まる。
- WS は未抽象。

## ■ 新版
- REST と WS の mapping 入口を完全分離。
- MarketData / Trading / Account / MarginAccount の REST mapping を明確化。
- WS は `IRealtimeMarketDataApi` に統合し、REST と混在させない設計へ変更。

**差分の本質：REST/WS の混在から、完全な責務分離へ。**

---

# 6. 実装方針（Impl）の違い

## ■ 旧版
- Stage4 でほぼ実装も進める前提（Cancel・DTO変換・E2分類など）。

## ■ 新版
- Stage4 は「設計フェーズ」へ役割変更。  
  → 実装は Stage5 に送る。
- OrderRequest などの Domain 設計は踏襲しつつ、抽象IF との整合性を優先。
- Margin の深い仕様は抽象化しない：Raw API で扱う方針に変更。

**差分：Stage4 の負荷を削減し、後続 Stage の実装安定性を高めた。**

---

# 7. テスト観点の差分

## ■ 旧版
- DTO ↔ Domain の mapping テスト
- OrderRequest バリデーション
- cancelchildorder の RestClient テスト
- E2 エラー分類確認

## ■ 新版
- Stage4 は「設計整合性」テストが中心：
  - 全インターフェースの整合チェック
  - Domain 型の欠落/API 対応関係の確認
  - 責務境界が正しく分離されているか
- REST/WS の実テストは Stage5/6 へ移動

**差分：設計テスト中心にし、実装テストを Stage5 以降へ移譲。**

---

# 8. OPS（運用）観点の差分

## ■ 旧版
- LIMIT/STOP の送信 → キャンセル → Private GET の確認を主に扱う。

## ■ 新版
- REST/WS の使い分けを運用ガイドに含める必要が出た：
  - REST：スナップショット
  - WS：リアルタイム更新
- 旧版では WS 非対象だったため生じなかった差分。

---

# 9. 総括：Stage4 は「REST+WS 抽象確定ステージ」へ進化した

| 項目 | 旧 Stage4 | 新 Stage4（Rev2） |
|------|------------|---------------------|
| 中心テーマ | Private API の横展開 | **REST+WS 抽象 API の体系化** |
| REST | 拡張中心 | 抽象層の凍結 |
| WS | 非対象 | **抽象層に追加** |
| Margin | API 拡張扱い | **能力追加（継承）として整理** |
| 実装 | Stage4で進める | **Stage5に移動（設計専念）** |
| Raw API | 位置づけ曖昧 | **非抽象領域として明確化** |

**結果として、新 Stage4 は ExchangeAPI の中核となる抽象 API を定義し、  
Stage5〜7 の開発を迷いなく進めるための設計基盤として整理された。**

---

以上。

