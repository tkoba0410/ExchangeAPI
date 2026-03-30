# MCP Server（Bot 向け adapter 設計補助文書）

最終更新: 2026-03-29  
対象ブランチ: `stage11`

## 1. 位置づけ

本書は、ExchangeAPI library の上に載る MCP Server adapter の設計補助文書である。  
library の設計正本は [`docs/spec.md`](./spec.md) に置き、  
本書では Bot 向け MCP Server の責務、依存、tool 契約、動作モードを扱う。

本MCP Server は、Codex / LLM を利用する Bot に対し、売買判断に必要な市場情報・口座情報・注文評価機能を、read / evaluate 専用 interface として提供する。

## 2. 目的

- 売買判断に必要な市場状態を構造化して返す
- 売買判断に必要な口座状態を最小集合で返す
- 指定注文が機械的に成立可能かを評価する
- 本番系 LLM に実行能力を与えず、判断材料だけを渡す

## 3. 非目的

- 本番実発注
- 注文取消
- 出金、設定変更、その他の副作用操作
- ExchangeAPI 全機能の完全ラッパー化
- 問いごとの専用 tool 量産
- 戦略そのものの実装
- Bot の execution / retry / idempotency / order tracking
- 市場予測
- 期待値計算
- リスク許容度評価
- 複数注文の最適化
- 他戦略との競合解決

## 4. 全体像

### 4.1 本番系

```text
Bot
  - LLM 呼び出し管理
  - MCP tool 提供
  - 判断採用 / 不採用
  - execution responsibility
    ↓
LLM / Codex
    ↓ tool call
MCP Server
  - market observation
  - account observation
  - order evaluation
    ↓
ExchangeAPI Library
    ↓
Exchange
```

### 4.2 デバッグ系

```text
Human / Debug LLM
    ↓
MCP Server
  - read / evaluate

Human / Debug LLM
    ↓
CLI
  - dry-run
  - sandbox execute
  - operational inspection
    ↓
ExchangeAPI Library
    ↓
Exchange / Sandbox
```

## 5. 役割分担

### 5.1 MCP Server

MCP Server は以下を所有する。

- tool schema
- tool input / output の adapter 契約
- 複数 library call を集約した Bot 向け read / evaluate tool
- 必要最小限の正規化
- warning の返却
- session / transport ごとの公開制御
- tool-level observability

MCP Server は以下を所有しない。

- 実発注
- 注文取消
- venue 固有 endpoint 実装
- concrete endpoint / runtime / signer / transport
- `Protocol` / `Native` の正本定義
- 戦略の最終決定
- 執行責任

### 5.2 Bot

Bot は以下を所有する。

- LLM 呼び出し管理
- MCP 利用
- 売買判断の採用 / 不採用
- 実発注責任
- 冪等性
- リトライ
- 注文追跡
- execution policy

Bot の制約:

- Bot は CLI を利用しない
- Bot は ExchangeAPI Library を直接利用する

### 5.3 CLI

CLI は以下を所有する。

- 人間向け実行面
- デバッグ補助
- dry-run
- sandbox execute
- 運用コマンド

CLI の制約:

- CLI は Bot から利用しない
- LLM に CLI を許可する場合は debug 系に限定する
- 本番口座への到達は技術的に不可にする

### 5.4 ExchangeAPI Library

ExchangeAPI Library は以下を所有する。

- 取引所差分吸収
- 状態取得
- 実発注 / 取消プリミティブ
- 制約情報取得
- exchange-native contract

## 6. 依存規約

- 現行 phase の依存は `McpServer -> Composition` を基本とする
- MCP Server は venue ごとの `Composition` project を経由して library を利用する
- MCP Server は必要に応じて複数の library call を集約して 1 tool response を構築してよい
- MCP Server から `Native` / `Protocol` / `Vocabulary` project を直接参照してはならない
- MCP Server は concrete endpoint / runtime / signer / transport を直接配線しない

### 6.1 物理配置

- MCP Server project は `src/Adapters/McpServer/ExchangeApi.Adapters.McpServer.csproj` に置く
- MCP Server test project は `tests/Adapters/McpServer.Tests/ExchangeApi.Adapters.McpServer.Tests.csproj` に置く
- MCP Server は external adapter であり、`src/Exchanges/<Venue>/` 配下に置いてはならない
- direct project reference は venue ごとの `Composition` project に限定する
  - `src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj`
  - `src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj`

推奨フォルダ構成:

```text
src/Adapters/McpServer/
  ExchangeApi.Adapters.McpServer.csproj
  Program.cs
  Tools/
    Market/
    Account/
    Evaluation/
  Schema/
  Permissions/
  Observability/
  Infrastructure/
  Mapping/
```

## 7. 設計原則

### 7.1 Read / Evaluate Only

本MCP Server は副作用を持たない。

### 7.2 Stable Responsibility

自然言語の問いごとではなく、安定した責務単位で tool を定義する。

### 7.3 Tool Minimalism

tool は最小限に保ち、意味の重複を避ける。

### 7.4 Bot-Oriented Abstraction

tool surface は library endpoint の 1:1 mirror を要求しない。  
Bot / LLM が安定して利用できる責務単位へ集約してよい。

### 7.5 Structured Response

返り値は、LLM と Bot が扱いやすい構造化 response とする。

### 7.6 Capability Separation

本番系 LLM は実行能力を持たない。  
debug 系の例外運用は、本番系とは別 capability / 別経路として扱う。

### 7.7 Numeric Consistency

価格、数量、金額に関する数値表現は JSON string で統一する。  
内部実装では decimal 相当で保持し、入出力時に文字列表現へ正規化する。

### 7.8 No Guessing

取引所側または ExchangeAPI 側で取得不能な項目は、推測や補完を行わず `null` または `unknown` として返す。

## 8. 動作モードと境界

### 8.1 Production

- LLM は MCP のみ利用可能
- MCP は read / evaluate only
- 実発注は Bot -> ExchangeAPI のみ
- Bot に CLI は許可しない
- 本番系 LLM に CLI は許可しない

### 8.2 Debug

- Debug LLM は MCP を利用可能
- 必要なら制限付き CLI を利用可能
- CLI の既定は dry-run
- execute は sandbox / test account に限定する
- 本番口座への到達は技術的に不可とする

## 9. 現行 phase

- Stage11 の初期実装は Bot 向け read / evaluate tool から始める
- 初期 tool inventory は `get_market_snapshot`、`get_account_snapshot`、`evaluate_order` の 3 つに固定する
- 初期 venue scope は bitFlyer を正本とする
- Binance など他 venue の展開は、market rule / account / evaluation の導出元が固定できてから行う
- 拡張候補は `list_markets` のみとし、他は明確な新責務が生じるまで追加しない

### 9.1 bitFlyer v1 support matrix

- `get_market_snapshot`: `BTC_JPY`、`FX_BTC_JPY`
- `get_account_snapshot`: symbol input なし。spot balance と `FX_BTC_JPY` position を返す
- `evaluate_order`: `BTC_JPY` の `LIMIT` / `MARKET` child order のみ

補足:

- `get_account_snapshot.positions` は bitFlyer `GetPositions` の制約に従い、`FX_BTC_JPY` のみを対象とする
- spot 保有は `positions` ではなく `balance` に表現する
- `evaluate_order` は margin product を初期 scope に含めない

## 10. 公開 tool 一覧

1. `get_market_snapshot`
2. `get_account_snapshot`
3. `evaluate_order`

補足:

- 上記 3 つが current phase の tool universe である
- MCP `tools/list` は current process が実際に実行可能な visible tool set を返す
- `get_account_snapshot` と `evaluate_order` は private credentials を解決できない場合、`tools/list` から advertise しない

追加しない例:

- `can_buy_now`
- `can_sell_now`
- `can_place_market_buy`
- `can_place_limit_buy`

これらは `evaluate_order` に吸収する。

## 11. Tool 契約

### 11.1 共通ルール

- tool input の数値は、価格、数量、金額に限り decimal string を使う
- count や boolean は JSON number / boolean を維持する
- timestamp は UTC の ISO 8601 string とする
- venue は初期 phase では server configuration に固定し、tool input には含めない
- 将来 multi-venue を扱う場合は、tool input へ `venue` または等価の account context を追加する
- supported symbol は tool ごとに固定する
- v1 では、supported symbol は library の市場存在確認と MCP adapter 側の明示 rule/config の両方を満たす集合とする
- market rule は runtime 推測で埋めてはならない

### 11.1.1 bitFlyer v1 共通導出ルール

- `get_market_snapshot` と `evaluate_order` の symbol support は adapter-owned の `BitflyerMarketRuleRegistry` を正本とする
- `BitflyerMarketRuleRegistry` は `minSize`、`sizeStep`、`priceStep` を symbol ごとに明示定義する
- `BitflyerMarketRuleRegistry` に entry がない symbol は、MCP として未サポートとみなし `invalid_symbol` とする
- `BitflyerMarketRuleRegistry` は venue 文書または運用上固定した設定値を source とする
- `BitflyerMarketRuleRegistry` の entry は version 管理対象とし、runtime observation から自動学習してはならない

### 11.1.2 `BitflyerMarketRuleRegistry` の source hierarchy

`BitflyerMarketRuleRegistry` は以下の source hierarchy で管理する。

1. 公式の公開文書に明示された定量値
2. 公式 API 文書の request / response contract
3. 上記で未公開の項目に限り、adapter-owned の明示設定値

bitFlyer v1 では次の source を正本とする。

- `minSize`
  - 公式 FAQ `注文数量について`
  - 公式手数料ページ `各暗号資産（仮想通貨）の売買単位・最小発注数量`
- `sizeStep`
  - 公式手数料ページ `各暗号資産（仮想通貨）の売買単位・最小発注数量` の「売買単位」
- `priceStep`
  - bitFlyer の公開文書に明示がないため、adapter-owned の明示設定値
  - この値は公式 API 文書の JPY market の example と live market observation を材料に maintain する
  - これは公開文書からの直接引用ではなく、運用上の推論である

参照 URL:

- `minSize`
  - <https://bitflyer.com/ja-jp/faq/4-27>
- `sizeStep`
  - <https://bitflyer.com/ja-jp/s/commission>
- `priceStep`
  - <https://lightning.bitflyer.com/docs/api>

### 11.1.3 bitFlyer v1 registry baseline

初期実装では、以下の registry entry を用いる。

| symbol | minSize | sizeStep | priceStep | source note |
| --- | --- | --- | --- | --- |
| `BTC_JPY` | `"0.001"` | `"0.00000001"` | `"1"` | `minSize` と `sizeStep` は公式公開値、`priceStep` は adapter-owned 推論値 |
| `FX_BTC_JPY` | `"0.001"` | `"0.00000001"` | `"1"` | `minSize` は 2024-10-21 以降の公式公開値、`sizeStep` は BTC 単位の公開値、`priceStep` は adapter-owned 推論値 |

補足:

- `FX_BTC_JPY.minSize = "0.001"` は bitFlyer Crypto CFD の最小発注数量変更後の値を正本とする
- `priceStep = "1"` は JPY market の例示価格が整数で示されていることと、運用上の観測に基づく保守的固定値である
- bitFlyer が価格単位を公式公開した場合は、その公開値を優先し、推論値を廃止する

### 11.1.4 `BitflyerMarketRuleRegistry` 更新手順

`BitflyerMarketRuleRegistry` の更新は以下の手順で行う。

1. 公式 FAQ `注文数量について` と公式手数料ページを確認し、公開日または更新日を記録する
2. `minSize` と `sizeStep` を公開値に合わせて更新する
3. `priceStep` 変更の必要がある場合は、公式 API 文書の example と live market observation を確認する
4. 推論値を変更する場合は、変更理由を commit message または関連文書に残す
5. MCP adapter test で `sizeRuleOk` / `priceRuleOk` / normalize の fixture を更新する

更新禁止事項:

- runtime で受け取った注文エラーを使って registry を自動更新してはならない
- 単一観測だけで `priceStep` を変更してはならない
- 公式 source 未確認のまま `minSize` / `sizeStep` を変更してはならない

### 11.2 `get_market_snapshot`

目的:

- 売買判断に必要な市場情報をまとめて取得する

入力:

```json
{
  "symbol": "BTC_JPY"
}
```

入力制約:

- `symbol` は必須
- サポート対象 symbol のみ受け付ける

出力:

```json
{
  "symbol": "BTC_JPY",
  "bid": "12345000",
  "ask": "12346000",
  "last": "12345500",
  "timestamp": "2026-03-29T10:00:00Z",
  "rules": {
    "minSize": "0.001",
    "sizeStep": "0.00000001",
    "priceStep": "1"
  },
  "status": "active"
}
```

出力項目:

- `symbol`: 正規化済み symbol
- `bid`: 現在買い気配
- `ask`: 現在売り気配
- `last`: 最新価格
- `timestamp`: 観測時刻
- `rules.minSize`: 最小数量
- `rules.sizeStep`: 数量刻み
- `rules.priceStep`: 価格刻み
- `status`: 市場状態

状態値:

- `active`
- `halted`
- `restricted`
- `unknown`

実装ルール:

- 市場状態は library の venue-specific state から上記 4 値へ写像する
- `rules.*` を導出できない venue / symbol では `null` を返す
- 詳細板情報は初期実装に含めない

bitFlyer v1 導出:

- `bid`、`ask`、`last`、`timestamp` は `GetTicker` を正本とする
- `status` は `GetBoardState.health` と `GetBoardState.state` の組み合わせで決定する
- `rules.*` は `BitflyerMarketRuleRegistry` を正本とする

bitFlyer v1 `status` mapping:

- `active`
  - `state = RUNNING`
  - `health = NORMAL` または `BUSY`
- `restricted`
  - `state = STARTING`、`PREOPEN`、`CIRCUIT BREAK`
  - `health = VERY BUSY` または `SUPER BUSY`
- `halted`
  - `state = CLOSED` または `MATURED`
  - `health = NO ORDER` または `STOP`
- `unknown`
  - 上記以外

bitFlyer v1 の補足:

- v1 support set に含まれる symbol では `rules.*` を non-null とする
- `GetBoardState` が取得できない場合は tool-level `upstream_error` とし、`GetHealth` 単独への silent fallback は行わない

### 11.3 `get_account_snapshot`

目的:

- 売買判断に必要な口座情報を MVP 項目に限定して返す

入力:

```json
{}
```

出力:

```json
{
  "balance": {
    "JPY": "5000000"
  },
  "positions": [
    {
      "symbol": "FX_BTC_JPY",
      "side": "buy",
      "size": "0.1",
      "avgPrice": "12000000"
    }
  ],
  "openOrdersSummary": {
    "count": 0
  },
  "margin": {
    "derivedAvailable": "4500000"
  },
  "accountReadiness": "ready"
}
```

出力項目:

- `balance`: 通貨別残高
- `positions`: 建玉一覧
- `positions[].side`: `buy` / `sell`
- `openOrdersSummary.count`: 未約定注文件数
- `margin.derivedAvailable`: `GetCollateral` 由来の導出余力
- `accountReadiness`: MCP が必要 read capability を観測できているか

状態値:

- `ready`
- `restricted`
- `unknown`

実装ルール:

- 取引所固有の詳細値は極力隠蔽する
- `openOrdersSummary.count` は trading 対象の active open orders の件数とする
- `accountReadiness` は venue 側の口座状態そのものではなく、MCP が必要 read capability を観測できているかを表す
- `margin.derivedAvailable` を導出できない場合は `null` を返す
- `GetPermissions` のみ取得不能な場合は、snapshot 自体は成功として返し、`accountReadiness = unknown` とする

bitFlyer v1 導出:

- `balance` は `GetBalance` の `available` を通貨別 map へ正規化したものを正本とする
- `positions` は `GetPositions(product_code = FX_BTC_JPY)` を `symbol`、`side`、`size`、`avgPrice` へ正規化したものを正本とする
- `openOrdersSummary.count` は `GetChildOrders(product_code = BTC_JPY, child_order_state = ACTIVE)` と `GetChildOrders(product_code = FX_BTC_JPY, child_order_state = ACTIVE)` の件数合計を正本とする
- `margin.derivedAvailable` は `GetCollateral` の `collateral + open_position_pnl - require_collateral` で算出する
- `accountReadiness` は `GetPermissions` による read capability 判定を正本とする

bitFlyer v1 `accountReadiness` mapping:

- `ready`
  - `GetPermissions` が成功し、以下の required read permissions をすべて含む
  - `/v1/me/getpermissions`
  - `/v1/me/getbalance`
  - `/v1/me/getcollateral`
  - `/v1/me/getchildorders`
  - `/v1/me/getpositions`
- `restricted`
  - `GetPermissions` が成功したが required read permissions の一部が欠ける
- `unknown`
  - `GetPermissions` を取得できない
  - venue から account status を断定できない

bitFlyer v1 の補足:

- `accountReadiness` は MCP が観測できる read capability の状態であり、execution 可否、KYC 状態、出金可否を意味しない
- `positions` は `FX_BTC_JPY` 固定であり、spot holdings は `balance` にのみ現れる
- `margin.derivedAvailable` は raw venue field ではなく導出値である

初期実装で除外する項目:

- 未約定注文の全件詳細
- 取引所固有の証拠金内訳
- 取引所固有の追加統計情報

### 11.4 `evaluate_order`

目的:

- 指定された注文要求が、現在の市場、口座、制約の下で機械的に成立可能かを評価する

入力:

```json
{
  "symbol": "BTC_JPY",
  "side": "buy",
  "orderType": "market",
  "size": "0.3",
  "price": null
}
```

入力制約:

- `symbol`: 必須
- `side`: 必須。`buy` / `sell`
- `orderType`: 必須。`market` / `limit`
- `size`: 必須。正の decimal string
- `price`: `limit` の場合必須、`market` の場合 `null`

bitFlyer v1 追加制約:

- `symbol` は `BTC_JPY` のみ
- `orderType` は bitFlyer child order としての `LIMIT` / `MARKET` のみ
- parent order、stop、trail、複合注文は v1 scope 外とする

評価対象:

- symbol 妥当性
- 市場状態
- size 最小数量 / 刻み適合
- price 妥当性
- 残高または証拠金余力
- 建玉 / 内部上限
- 想定 notional
- warning 抽出

非対象:

- 戦略としての妥当性
- 期待値
- 売買タイミングの最終判断
- 他戦略との競合解決
- 市場予測
- リスク許容度評価
- 複数注文の最適化

出力:

```json
{
  "canPlace": true,
  "checks": {
    "symbolOk": true,
    "marketStatusOk": true,
    "sizeRuleOk": true,
    "priceRuleOk": true,
    "balanceOk": true,
    "positionLimitOk": true
  },
  "normalizedRequest": {
    "symbol": "BTC_JPY",
    "side": "buy",
    "orderType": "market",
    "size": "0.300",
    "price": null
  },
  "estimate": {
    "referencePrice": "12345678",
    "estimatedNotional": "3703703.4"
  },
  "warnings": [
    "market_order_slippage_risk"
  ],
  "reasons": []
}
```

出力項目:

- `canPlace`: 総合判定
- `checks`: 個別検査結果
- `normalizedRequest`: 正規化済み注文要求
- `estimate.referencePrice`: 評価基準価格
- `estimate.estimatedNotional`: 想定約定金額
- `warnings`: 注意事項
- `reasons`: 不可時の理由一覧

実装ルール:

- `canPlace = false` は tool-level error ではなく正常 response として返す
- 残高不足や position limit 超過は、原則として `reasons` に積み、tool 自体は失敗させない
- 入力不正、upstream 取得失敗、想定外障害のみを tool-level error とする
- `canPlace = true` でも、最終発注判断は Bot が行う

bitFlyer v1 導出:

- 評価対象は `BTC_JPY` spot order に限定する
- `referencePrice` は `market buy -> ask`、`market sell -> bid`、`limit -> input price` とする
- `estimatedNotional` は `referencePrice * size` とする
- `balanceOk`
  - `buy`: `balance["JPY"] >= estimatedNotional`
  - `sell`: `balance["BTC"] >= size`
- `marketStatusOk` は `get_market_snapshot.status == active` のときのみ `true`
- `sizeRuleOk` は `BitflyerMarketRuleRegistry.minSize` と `sizeStep` に対する適合で判定する
- `priceRuleOk` は `orderType = limit` のとき `priceStep` 適合と正値条件で判定する
- `positionLimitOk` は adapter config の optional `MaxBaseSize` で判定し、未設定なら `true` とする
- `warnings` は v1 では `market` 注文時に `market_order_slippage_risk` を返し、それ以外は空配列とする

bitFlyer v1 の補足:

- v1 は fee を blocking check に含めない
- v1 は `FX_BTC_JPY` を評価対象に含めない
- v1 は exchange-side の hidden limit、rate limit、post-only 相当条件、将来追加される venue-specific reject rule を完全再現しない

## 12. エラー仕様

tool-level error は以下のカテゴリに分類する。

- `validation_error`
- `upstream_error`
- `domain_error`
- `internal_error`

代表 error code:

- `invalid_symbol`
- `invalid_side`
- `invalid_order_type`
- `invalid_size`
- `invalid_price`
- `market_unavailable`
- `account_unavailable`
- `internal_error`

error 返却形式:

```json
{
  "errorCategory": "validation_error",
  "errorCode": "invalid_symbol",
  "message": "Unsupported symbol.",
  "details": {},
  "retryable": false
}
```

ルール:

- 入力不正は `validation_error`
- ExchangeAPI / 取引所依存の取得失敗は `upstream_error`
- tool 契約外の domain invariant 崩れは `domain_error`
- 想定外障害は `internal_error`

補足:

- `evaluate_order` の機械的な不成立は、可能な限り正常 response の `canPlace = false` で表現する

MCP v1 error boundary:

- `validation_error`
  - unsupported symbol
  - invalid side / order type
  - malformed decimal string
  - `market` なのに `price != null`
  - `limit` なのに `price == null`
- `upstream_error`
  - `GetTicker` / `GetBoardState` / `GetBalance` / `GetCollateral` / `GetChildOrders` / `GetPositions` の transport, http, codec failure
  - `GetPermissions` failure は `get_account_snapshot` では degraded success とし、`accountReadiness = unknown` に写像する
- `domain_error`
  - tool 契約上は support 済みの symbol に対して、required mapping/config が壊れている
- `internal_error`
  - 上記に分類できない実装不整合

正常 response に残す不成立:

- `market_not_active`
- `size_rule_violation`
- `price_rule_violation`
- `insufficient_balance`
- `exposure_limit_exceeded`

## 13. ログ / 実装上の制約

- transport は初期実装で `stdio` を採用する
- stdio transport は latest MCP transport に従い、`stdin` / `stdout` で 1 行 1 JSON-RPC message を扱う
- `stdout` は MCP message 専用とし、ログを書かない
- ログは `stderr` または別ログ出力に限定する
- 初期 server surface は `initialize`、`ping`、`tools/list`、`tools/call` に限定する
- `tools/list` は各 tool の `inputSchema` と `outputSchema` を返す
- 返り値は安定した JSON 構造を維持する
- sensitive data をログに出してはならない

## 14. 完成条件

初期完成は以下を満たした時点とする。

- `get_market_snapshot` が実装されている
- `get_account_snapshot` が MVP 範囲で実装されている
- `evaluate_order` が機械的成立可否の範囲で実装されている
- 返り値が LLM / Bot に利用可能な構造化形式である
- 副作用を持たない
- bitFlyer venue で live / fixture の両方から検証可能である

### 14.1 最低検証項目

MCP v1 の検証は、fixture test と live test の両方で以下を満たす。

fixture test:

- `get_market_snapshot`
  - supported symbol に対して `rules.*` が registry baseline と一致する
  - `GetBoardState.health` / `state` の代表組み合わせが `active` / `restricted` / `halted` / `unknown` に写像される
  - `GetBoardState` failure は silent fallback せず `upstream_error` になる
- `get_account_snapshot`
  - `GetBalance.available` が通貨別 map に正規化される
  - `positions` は `FX_BTC_JPY` のみを返し、spot holdings は `balance` にのみ現れる
  - `openOrdersSummary.count` は `BTC_JPY` と `FX_BTC_JPY` の `ACTIVE` child order 件数合計である
  - `margin.derivedAvailable = collateral + open_position_pnl - require_collateral` で算出される
  - `accountReadiness` が required read permissions の有無に応じて `ready` / `restricted` / `unknown` に写像される
  - `GetPermissions` failure は tool error にせず、`accountReadiness = unknown` を返す
- `evaluate_order`
  - `validation_error` の境界が固定されている
  - `market_not_active`、`size_rule_violation`、`price_rule_violation`、`insufficient_balance`、`exposure_limit_exceeded` が正常 response の `reasons` に残る
  - `market buy -> ask`、`market sell -> bid`、`limit -> input price` の `referencePrice` が使われる
  - `sizeRuleOk` と `priceRuleOk` が registry baseline に対して判定される
  - `buy` は `JPY` 残高、`sell` は `BTC` 残高で `balanceOk` を判定する

live test:

- live test の opt-in と local marker の扱いは [`docs/spec.md`](./spec.md) の live test 契約を正本とする
- `get_market_snapshot` は public read live test として検証可能である
- `get_account_snapshot` と `evaluate_order` は private read live test として検証可能である
- private live test は read-only に限定し、write side effect を持たない
- live test は `BitflyerMarketRuleRegistry` baseline が drift していないことを検出できる構成にする
- adapter live test は `tests/Adapters/McpServer.LiveTests` に置き、transport は in-memory stdio で検証する

## 15. 今後詰める項目

- `BitflyerMarketRuleRegistry.priceStep` の公開 source が将来提供された場合の切替手順
- `accountReadiness` / `margin.derivedAvailable` を multi-venue でも維持するか、より venue-neutral な schema に寄せるか
- `evaluate_order` に fee を blocking check として含めるか
- `evaluate_order` を `FX_BTC_JPY` まで広げるための margin rule 正本
- warning taxonomy の固定
- multi-venue 化時の `venue` / account context 契約
- observability / tracing rule
- permission model の具体値
- `list_markets` を追加するかどうか
