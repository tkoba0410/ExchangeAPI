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
- tool-call-level observability via MCP result `_meta`

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

### 7.9 Extension Admission Rule

新しい tool は、次の条件をすべて満たす場合にのみ追加してよい。

1. 新しい責務単位があり、既存 tool へ自然吸収できない
2. Bot / LLM 実装に安定した価値があり、質問文ごとの convenience 問いではない
3. venue 固有の一時的 shortcut ではなく、current phase の support boundary と整合する

追加しない例:

- `can_buy_now`
- `can_sell_now`
- `can_place_market_buy`
- `can_place_limit_buy`

これらは `evaluate_order` に吸収する。

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
- bitFlyer v1 core tool は `get_market_snapshot`、`get_account_snapshot`、`evaluate_order`、`evaluate_margin_order` の 4 つとする
- 初期 venue scope は bitFlyer を正本とする
- Binance など他 venue の account / evaluation 展開は、market rule / account / evaluation の導出元が固定できてから行う
- ただし Binance public market data は例外とし、Kline 専用 tool を public read 拡張として先行追加してよい
- current phase では `list_markets` を market discovery tool として追加してよい

### 9.1 bitFlyer v1 support matrix

- `get_market_snapshot`: `BTC_JPY`、`FX_BTC_JPY`
- `get_account_snapshot`: symbol input なし。spot balance と `FX_BTC_JPY` position を返す
- `evaluate_order`: `BTC_JPY` の `LIMIT` / `MARKET` child order のみ
- `evaluate_margin_order`: `FX_BTC_JPY` の `LIMIT` / `MARKET` child order のみ

補足:

- `get_account_snapshot.positions` は bitFlyer `GetPositions` の制約に従い、`FX_BTC_JPY` のみを対象とする
- spot 保有は `positions` ではなく `balance` に表現する
- `evaluate_order` は margin product を初期 scope に含めない

### 9.2 Binance public kline extension support matrix

- `get_klines`: public read only
- account snapshot は提供しない
- order evaluation は提供しない
- private credentials は不要

初期 support set:

- `BTCJPY`
- `ETHJPY`
- `XRPJPY`
- `BNBJPY`
- `BTCUSDT`
- `ETHUSDT`
- `SOLUSDT`
- `XRPUSDT`

補足:

- Binance extension は Bot 向け account / evaluation tool ではなく、市場時系列の public read tool として扱う
- 初期 phase では Binance `GetKlines` のみを MCP へ露出する
- Binance private tool は初期 scope に含めない

## 10. 公開 tool 一覧

1. `get_market_snapshot`
2. `list_markets`
3. `get_klines`
4. `get_account_snapshot`
5. `evaluate_order`
6. `evaluate_margin_order`

補足:

- 上記 6 つが現行実装の current phase tool universe である
- bitFlyer v1 core は `get_market_snapshot`、`get_account_snapshot`、`evaluate_order`、`evaluate_margin_order` であり、`list_markets` は market discovery tool、`get_klines` は Binance public read extension として扱う
- MCP `tools/list` は current process が実際に実行可能な visible tool set を返す
- `get_account_snapshot`、`evaluate_order`、`evaluate_margin_order` は private credentials を解決できない場合、`tools/list` から advertise しない
- `list_markets` は current process の visible market capability set を返す
- `get_klines` は Binance public client が配線されている場合のみ advertise してよい
- current `CreateDefault` 実装では Binance public client を既定で配線するため、通常の server 起動では `get_klines` は visible tool set に含まれる
- Binance upstream の可用性までは `tools/list` で事前判定せず、`tools/call get_klines` 時の `upstream_error` として扱う
- v2 で multi-venue private tool を導入する場合、hidden venue を増やさず、tool input に `venue` と `accountContext` を first-class field として出す

追加しない例:

## 11. Tool 契約

### 11.1 共通ルール

- tool input の数値は、価格、数量、金額に限り decimal string を使う
- count や boolean は JSON number / boolean を維持する
- timestamp は UTC の ISO 8601 string とする
- bitFlyer v1 private tools は `venue` と `accountContext` を明示 input とする
- `get_market_snapshot` は current phase では bitFlyer v1 固定の public tool とする
- Binance public `get_klines` は venue-explicit とし、tool input の `venue` は v1 では `binance` のみを許可する
- multi-venue を扱う tool は、hidden venue を増やさず `venue` または等価の account context を contract で明示する
- supported symbol は tool ごとに固定する
- v1 では、supported symbol は library の市場存在確認と MCP adapter 側の明示 rule/config の両方を満たす集合とする
- market rule は runtime 推測で埋めてはならない

### 11.1.1 bitFlyer v1 共通導出ルール

- bitFlyer の公開文書と公開 API 文書を external source of truth とする
- `get_market_snapshot` と `evaluate_order` は adapter-owned の `BitflyerMarketRuleRegistry` を pinned operational config として利用する
- `BitflyerMarketRuleRegistry` の正本データは version 管理された data file とし、adapter code は loader / validator のみを持つ
- `BitflyerMarketRuleRegistry` は `minSize`、`sizeStep`、`priceStep` と各 field の source kind / source ref を symbol ごとに明示定義する
- `BitflyerMarketRuleRegistry` に entry がない symbol は、MCP として未サポートとみなし `invalid_symbol` とする
- `BitflyerMarketRuleRegistry` は venue 文書または運用上固定した設定値から構成される pinned config である
- `BitflyerMarketRuleRegistry` の entry は version 管理対象とし、runtime observation から自動学習してはならない

### 11.1.2 `BitflyerMarketRuleRegistry` の source hierarchy

`BitflyerMarketRuleRegistry` は以下の source hierarchy で管理する。

1. 公式の公開文書に明示された定量値
2. 公式 API 文書の request / response contract
3. 上記で未公開の項目に限り、adapter-owned の明示設定値

bitFlyer v1 では次の source hierarchy を使って pinned config を構成する。

- `minSize`
  - 公式 FAQ `注文数量について`
  - 公式手数料ページ `各暗号資産（仮想通貨）の売買単位・最小発注数量`
- `sizeStep`
  - 公式手数料ページ `各暗号資産（仮想通貨）の売買単位・最小発注数量` の「売買単位」
- `priceStep`
  - bitFlyer の公開文書に明示がないため、adapter-owned の明示設定値
  - この値は公式 API 文書の JPY market の example と live market observation を材料に maintain する
  - これは公開文書からの直接引用ではなく、運用上の推論である

versioned data file:

- current pinned file は `src/Adapters/McpServer/Data/bitflyer-market-rules.v1.json`
- code 側の registry は上記 file を load / validate して `Entries` を構成する

参照 URL:

- `minSize`
  - <https://bitflyer.com/ja-jp/faq/4-27>
- `sizeStep`
  - <https://bitflyer.com/ja-jp/s/commission>
- `priceStep`
  - <https://lightning.bitflyer.com/docs/api>

### 11.1.3 bitFlyer v1 registry baseline

初期実装では、以下の registry entry を用いる。

| symbol | minSize | sizeStep | priceStep | minSize source kind | minSize source ref | sizeStep source kind | sizeStep source ref | priceStep source kind | priceStep source ref | source note |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `BTC_JPY` | `"0.001"` | `"0.00000001"` | `"1"` | `official_documented` | `https://bitflyer.com/ja-jp/s/commission` | `official_documented` | `https://bitflyer.com/ja-jp/s/commission` | `adapter_inferred` | `adapter://bitflyer-jpy-price-step.v1` | `minSize` と `sizeStep` は公式公開値、`priceStep` は adapter-owned 推論値 |
| `FX_BTC_JPY` | `"0.001"` | `"0.00000001"` | `"1"` | `official_documented` | `https://bitflyer.com/pub/20241015-bitFlyerCryptoCFD-Minimum-Order-Change-en.pdf` | `official_documented` | `https://bitflyer.com/ja-jp/s/commission` | `adapter_inferred` | `adapter://bitflyer-jpy-price-step.v1` | `minSize` は 2024-10-21 以降の公式公開値、`sizeStep` は BTC 単位の公開値、`priceStep` は adapter-owned 推論値 |

補足:

- `FX_BTC_JPY.minSize = "0.001"` は bitFlyer Crypto CFD の最小発注数量変更後の値を正本とする
- `priceStep = "1"` は JPY market の例示価格が整数で示されていることと、運用上の観測に基づく保守的固定値である
- bitFlyer が価格単位を公式公開した場合は、その公開値を優先し、`priceStepSourceKind = official_documented` または `official_api_contract` へ切り替え、`adapter_inferred` を廃止する
- 公式 source が提供された symbol では `adapter_inferred` を残さない

### 11.1.4 `BitflyerMarketRuleRegistry` 更新手順

`BitflyerMarketRuleRegistry` の更新は以下の手順で行う。

1. 公式 FAQ `注文数量について` と公式手数料ページを確認し、公開日または更新日を記録する
2. `minSize` と `sizeStep` を公開値に合わせて更新する
3. `priceStep` 変更の必要がある場合は、公式 API 文書の example と live market observation を確認する
4. 推論値を変更する場合は、変更理由を commit message または関連文書に残す
5. MCP adapter test で `sizeRuleOk` / `priceRuleOk` / normalize の fixture を更新する
6. 公式 source が提供された場合は `*SourceRef` を公式参照へ更新し、`adapter_inferred` を除去する

更新禁止事項:

- runtime で受け取った注文エラーを使って registry を自動更新してはならない
- 単一観測だけで `priceStep` を変更してはならない
- 公式 source 未確認のまま `minSize` / `sizeStep` を変更してはならない

### 11.1.5 Binance public kline support set

- `get_klines` の symbol support は adapter-owned の `BinanceKlineSymbolSet` を正本とする
- `BinanceKlineSymbolSet` は初期 phase では以下の 8 symbol に固定する
  - `BTCJPY`
  - `ETHJPY`
  - `XRPJPY`
  - `BNBJPY`
  - `BTCUSDT`
  - `ETHUSDT`
  - `SOLUSDT`
  - `XRPUSDT`
- `BinanceSymbols` の known values 定数は convenience 用であり、MCP support set の正本ではない
- `get_klines.interval` は [`docs/endpoints-binance.md`](./endpoints-binance.md) の `GetKlines` fixed contract を正本とする
- `get_klines` は Binance の `timeZone` parameter を v1 では公開せず、UTC 固定で扱う

### 11.1.6 v2 multi-venue contract direction

bitFlyer v1 private tools は最小形としてすでに `venue + accountContext` を公開している。

v2 で multi-venue を扱う場合は、以下を採用する。

- public tool は `venue` を first-class input とする
- private tool は `venue` に加えて `accountContext` を first-class input とする
- server configuration は default venue / default accountContext を与えるだけで、意味論の本体にしない
- hidden venue / hidden account routing を増やさない
- 責務が同じ tool は venue ごとに rename せず、generic tool 名を維持する
- venue ごとの差分は input/output schema を `venue` 従属で表現する

`accountContext` の v2 初期形:

```json
{
  "venue": "bitflyer",
  "accountContext": "default"
}
```

補足:

- `accountContext` は free-form text にせず、tool ごとに許可された closed set とする
- 同じ server process が複数 venue を扱っても、contract 上は `venue` を省略しない
- `tools/list` は capability を返すものであり、runtime routing policy の暗黙共有手段として使わない

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
    "priceStep": "1",
    "minSizeSourceKind": "official_documented",
    "minSizeSourceRef": "https://bitflyer.com/ja-jp/s/commission",
    "sizeStepSourceKind": "official_documented",
    "sizeStepSourceRef": "https://bitflyer.com/ja-jp/s/commission",
    "priceStepSourceKind": "adapter_inferred",
    "priceStepSourceRef": "adapter://bitflyer-jpy-price-step.v1"
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
- `rules.minSizeSourceKind`: `minSize` の source kind
- `rules.minSizeSourceRef`: `minSize` の source ref
- `rules.sizeStepSourceKind`: `sizeStep` の source kind
- `rules.sizeStepSourceRef`: `sizeStep` の source ref
- `rules.priceStepSourceKind`: `priceStep` の source kind
- `rules.priceStepSourceRef`: `priceStep` の source ref
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
- `rules.*` の返却値は `BitflyerMarketRuleRegistry` pinned operational config から取る
- `rules.*SourceKind` は各 rule field が `official_documented`、`official_api_contract`、`adapter_inferred`、`pinned_operational` のどれに基づくかを返す
- `rules.*SourceRef` はその field の pinned source 参照を返す

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

### 11.3 `list_markets`

目的:

- current process が実際に提供できる market-specific capability を venue / symbol 単位で列挙する

入力:

```json
{}
```

出力:

```json
{
  "markets": [
    {
      "venue": "bitflyer",
      "symbol": "BTC_JPY",
      "capabilities": [
        "get_market_snapshot",
        "evaluate_order"
      ]
    },
    {
      "venue": "bitflyer",
      "symbol": "FX_BTC_JPY",
      "capabilities": [
        "get_market_snapshot",
        "evaluate_margin_order"
      ]
    },
    {
      "venue": "binance",
      "symbol": "BTCUSDT",
      "capabilities": [
        "get_klines"
      ]
    }
  ]
}
```

実装ルール:

- `list_markets` は current process の visible tool set を基準に構成する
- market-specific でない `get_account_snapshot` は `capabilities` に含めない
- `evaluate_order` は `BTC_JPY` に対してのみ列挙する
- `evaluate_margin_order` は `FX_BTC_JPY` に対してのみ列挙する
- private credentials が無い場合は `evaluate_order` と `evaluate_margin_order` capability を列挙しない

### 11.4 `get_account_snapshot`

目的:

- 売買判断に必要な口座情報を MVP 項目に限定して返す

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default"
}
```

出力:

```json
{
  "permissionModel": "bitflyer_private_read_v1",
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

- `permissionModel`: `accountReadiness` 判定に使う permission model identifier
- `balance`: 通貨別残高
- `positions`: 建玉一覧
- `positions[].side`: `buy` / `sell`
- `openOrdersSummary.count`: 未約定注文件数
- `margin.derivedAvailable`: `GetCollateral` 由来の導出余力
- `accountReadiness`: MCP が必要 read capability を観測できているか

入力制約:

- `venue`: 必須。v1 では `bitflyer`
- `accountContext`: 必須。v1 では `default`

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
- v1 では `accountReadiness` と `margin.derivedAvailable` を無理に venue-neutral 化しない

bitFlyer v1 導出:

- `balance` は `GetBalance` の `available` を通貨別 map へ正規化したものを正本とする
- `positions` は `GetPositions(product_code = FX_BTC_JPY)` を `symbol`、`side`、`size`、`avgPrice` へ正規化したものを正本とする
- `openOrdersSummary.count` は `GetChildOrders(product_code = BTC_JPY, child_order_state = ACTIVE)` と `GetChildOrders(product_code = FX_BTC_JPY, child_order_state = ACTIVE)` の件数合計を正本とする
- `margin.derivedAvailable` は `GetCollateral` の `collateral + open_position_pnl - require_collateral` で算出する
- `accountReadiness` は `GetPermissions` による read capability 判定を正本とする
- `permissionModel` は `bitflyer_private_read_v1` に固定する

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

### 11.5 `evaluate_order`

目的:

- 指定された注文要求が、現在の市場、口座、制約の下で機械的に成立可能かを評価する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "symbol": "BTC_JPY",
  "side": "buy",
  "orderType": "market",
  "size": "0.3",
  "price": null
}
```

入力制約:

- `venue`: 必須。v1 では `bitflyer`
- `accountContext`: 必須。v1 では `default`
- `symbol`: 必須
- `side`: 必須。`buy` / `sell`
- `orderType`: 必須。`market` / `limit`
- `size`: 必須。正の decimal string
- `price`: `limit` の場合必須、`market` の場合は省略または `null`

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
    "feeCoverageOk": null,
    "projectedExposureOk": true
  },
  "normalizedRequest": {
    "venue": "bitflyer",
    "accountContext": "default",
    "symbol": "BTC_JPY",
    "side": "buy",
    "orderType": "market",
    "size": "0.300",
    "price": null
  },
  "estimate": {
    "referencePrice": "12345678",
    "estimatedNotional": "3703703.4",
    "estimatedFee": null,
    "estimatedFeeSourceKind": null
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
- `normalizedRequest.venue`: 正規化済み venue
- `normalizedRequest.accountContext`: 正規化済み account context
- `estimate.referencePrice`: 評価基準価格
- `estimate.estimatedNotional`: 想定約定金額
- `estimate.estimatedFee`: optional fee estimate
- `estimate.estimatedFeeSourceKind`: fee estimate の source kind。v1 は optional `pinned_operational`
- `warnings`: 注意事項
- `reasons`: 不可時の理由一覧

warning taxonomy:

- v1 で返してよい warning code は `market_order_slippage_risk` と `estimated_fee_not_covered` のみ
- `warnings` は free-form text ではなく closed set として扱う

実装ルール:

- `canPlace = false` は tool-level error ではなく正常 response として返す
- 残高不足や position limit 超過は、原則として `reasons` に積み、tool 自体は失敗させない
- 入力不正、upstream 取得失敗、想定外障害のみを tool-level error とする
- `canPlace = true` でも、最終発注判断は Bot が行う
- `evaluate_order` は単一 venue / 単一 symbol / 単一 order request に対する局所 preflight evaluator として扱う
- `evaluate_order` は口座横断 risk engine、execution policy engine、strategy coordinator として扱わない
- v2 で margin product を扱う場合でも、spot evaluator と margin evaluator を 1 tool に混在させない

bitFlyer v1 導出:

- 評価対象は `BTC_JPY` spot order に限定する
- `referencePrice` は `market buy -> ask`、`market sell -> bid`、`limit -> input price` とする
- `estimatedNotional` は `referencePrice * size` とする
- `estimatedFee`
  - optional adapter config の fee rate がある場合のみ算出する
  - `market` は `MarketFeeRate`、`limit` は `LimitFeeRate` を使う
  - v1 では authoritative fee model ではなく operational estimate として扱う
- `balanceOk`
  - `buy`: `balance["JPY"] >= estimatedNotional`
  - `sell`: `balance["BTC"] >= size`
- `feeCoverageOk`
  - fee estimate がない場合は `null`
  - `buy`: `balance["JPY"] >= estimatedNotional + estimatedFee`
  - `sell`: v1 では fee settlement model を固定しないため `null`
- `marketStatusOk` は `get_market_snapshot.status == active` のときのみ `true`
- `sizeRuleOk` は `BitflyerMarketRuleRegistry.minSize` と `sizeStep` に対する適合で判定する
- `priceRuleOk` は `orderType = limit` のとき `priceStep` 適合と正値条件で判定する
- `projectedExposureOk` は adapter config の optional `MaxBaseSize` で判定し、`BTC_JPY` の同 side `ACTIVE` child order の `outstanding_size` 合計と今回 `size` の projected exposure が上限以下なら `true` とする
- `warnings`
  - `market` 注文時に `market_order_slippage_risk` を返す
  - `feeCoverageOk = false` の場合は `estimated_fee_not_covered` を返す

bitFlyer v1 の補足:

- v1 は fee を blocking check に含めない
- v1 は fee estimate があっても `canPlace` の blocking check に含めない
- v1 は `FX_BTC_JPY` を評価対象に含めない
- v1 の `projectedExposureOk` は active child order ベースの projected exposure 判定であり、既存 spot 保有残高そのものは含めない
- v1 は exchange-side の hidden limit、rate limit、post-only 相当条件、将来追加される venue-specific reject rule を完全再現しない

`evaluate_margin_order`:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "symbol": "FX_BTC_JPY",
  "side": "buy",
  "orderType": "market",
  "size": "0.1",
  "price": null
}
```

実装ルール:

- `evaluate_order` は spot preflight evaluator として維持する
- `evaluate_margin_order` は margin preflight evaluator として別 tool に維持する
- margin evaluator は spot evaluator と別の rule 正本を持つ
- bitFlyer margin evaluator の pinned file 名は `bitflyer-margin-rules.v1.json` とする

### 11.5.1 `evaluate_margin_order`

目的:

- 指定された bitFlyer margin 注文要求が、現在の市場、証拠金、維持率、制約の下で機械的に成立可能かを評価する

入力:

```json
{
  "venue": "bitflyer",
  "accountContext": "default",
  "symbol": "FX_BTC_JPY",
  "side": "buy",
  "orderType": "market",
  "size": "0.1",
  "price": null
}
```

入力制約:

- `venue`: 必須。v1 では `bitflyer`
- `accountContext`: 必須。v1 では `default`
- `symbol`: 必須。v1 では `FX_BTC_JPY`
- `side`: 必須。`buy` / `sell`
- `orderType`: 必須。`market` / `limit`
- `size`: 必須。正の decimal string
- `price`: `limit` の場合必須、`market` の場合は省略または `null`

出力:

```json
{
  "canPlace": true,
  "checks": {
    "symbolOk": true,
    "marketStatusOk": true,
    "sizeRuleOk": true,
    "priceRuleOk": true,
    "collateralCoverageOk": true,
    "feeCoverageOk": null,
    "projectedMarginExposureOk": true,
    "currentMaintenanceOk": true
  },
  "normalizedRequest": {
    "venue": "bitflyer",
    "accountContext": "default",
    "symbol": "FX_BTC_JPY",
    "side": "buy",
    "orderType": "market",
    "size": "0.1",
    "price": null
  },
  "estimate": {
    "referencePrice": "12345678",
    "estimatedNotional": "1234567.8",
    "estimatedRequiredCollateral": "493827.12",
    "currentMaxLeverage": "2.5",
    "currentKeepRate": "8",
    "minimumKeepRate": "1.5",
    "estimatedFee": null,
    "estimatedFeeSourceKind": null
  },
  "warnings": [
    "market_order_slippage_risk"
  ],
  "reasons": []
}
```

bitFlyer v1 導出:

- 評価対象は `FX_BTC_JPY` margin order に限定する
- `referencePrice` は `market buy -> ask`、`market sell -> bid`、`limit -> input price` とする
- `estimatedNotional` は `referencePrice * size` とする
- `estimatedRequiredCollateral` は `estimatedNotional / currentMaxLeverage` とする
- `currentMaxLeverage` は `GetCorporateLeverage.current_max` を正本とする
- `currentKeepRate` は `GetCollateral.keep_rate` を正本とする
- `minimumKeepRate` は `bitflyer-margin-rules.v1.json` を正本とする
- `collateralCoverageOk` は `derivedAvailable >= estimatedRequiredCollateral` で判定する
- `feeCoverageOk`
  - fee estimate がない場合は `null`
  - fee estimate がある場合は `derivedAvailable >= estimatedRequiredCollateral + estimatedFee`
- `projectedMarginExposureOk` は adapter config の optional `MaxBaseSize` で判定し、`FX_BTC_JPY` の同 side `ACTIVE` child order の `outstanding_size`、同 side open positions の `size`、今回 `size` の projected exposure が上限以下なら `true` とする
- `currentMaintenanceOk` は current keep rate が minimum keep rate 以上なら `true`
- `warnings`
  - `market` 注文時に `market_order_slippage_risk` を返す
  - `feeCoverageOk = false` の場合は `estimated_fee_not_covered` を返す

補足:

- `evaluate_margin_order` は current maintenance state を評価するが、post-trade liquidation proximity を完全再現しない
- v1 は fee を blocking check に含めない
- v1 は exchange-side の hidden leverage rule、future maintenance rule、将来追加される venue-specific reject rule を完全再現しない

### 11.5.2 MCP `tools/call` `_meta`

`tools/call` の result object には payload 本体と分離した `_meta` を含める。

```json
{
  "content": [
    {
      "type": "text",
      "text": "{\"symbol\":\"BTC_JPY\"}"
    }
  ],
  "structuredContent": {
    "symbol": "BTC_JPY"
  },
  "_meta": {
    "schemaVersion": "exchangeapi.mcp.get_market_snapshot.v1",
    "dataVersion": "bitflyer-market-rules.v1",
    "degraded": false
  },
  "isError": false
}
```

実装ルール:

- `_meta` は tool payload 本体に混ぜず、MCP call result envelope に置く
- `schemaVersion` は tool contract version を表す
- `dataVersion` は pinned config / support set / permission model の version を表す
- `degraded = true` は partial success で意味が弱まっている場合のみ返す
- v1 では `get_account_snapshot` で `accountReadiness = unknown` のときのみ `degraded = true`

### 11.6 `get_klines`

目的:

- 売買判断または market observation に必要な OHLCV 時系列を public read で取得する

入力:

```json
{
  "venue": "binance",
  "symbol": "BTCUSDT",
  "interval": "1h",
  "startTime": null,
  "endTime": null,
  "limit": 200
}
```

入力制約:

- `venue`: 必須。v1 では `binance`
- `symbol`: 必須
- `interval`: 必須。Binance の kline interval literal
- `startTime`: 任意。explicit `Z` または numeric offset を持つ RFC 3339 string または `null`
- `endTime`: 任意。explicit `Z` または numeric offset を持つ RFC 3339 string または `null`
- `limit`: 任意。`1..1000`

Binance public kline v1 追加制約:

- `venue` は `binance` のみ
- `symbol` は `BinanceKlineSymbolSet` のみ
- `interval` は [`docs/endpoints-binance.md`](./endpoints-binance.md) の `GetKlines` fixed contract に従う
- `timeZone` は公開しない
- `startTime` と `endTime` は strict RFC 3339 parse を行い、offset なし local time は受理しない
- `startTime` と `endTime` は server 側で UTC に正規化する
- `startTime` と `endTime` が両方ある場合は `startTime <= endTime`

multi-venue 方向:

- `get_klines` は generic tool 名を維持する
- venue ごとに `get_<venue>_klines` を増やさない
- multi-venue 時は `venue` を discriminator にして input schema を従属化する
- `symbol` の closed set は `venue` ごとに独立させる
- `interval` の closed set も `venue` ごとに独立させる

`venue` 従属 schema の考え方:

```json
{
  "oneOf": [
    {
      "type": "object",
      "properties": {
        "venue": { "const": "binance" },
        "symbol": { "enum": ["BTCJPY", "ETHJPY", "XRPJPY", "BNBJPY", "BTCUSDT", "ETHUSDT", "SOLUSDT", "XRPUSDT"] },
        "interval": { "enum": ["1m", "5m", "15m", "1h", "4h", "1d"] }
      },
      "required": ["venue", "symbol", "interval"]
    }
  ]
}
```

補足:

- second venue を追加するまでは v1 と同じ `venue = binance` の closed set を維持する
- second venue 追加時は new tool を増やさず、`oneOf` 分岐を追加して schema version を上げる
- output schema も `venue` を discriminator にして同じ方針で従属化する

出力:

```json
{
  "venue": "binance",
  "symbol": "BTCUSDT",
  "interval": "1h",
  "candles": [
    {
      "openTime": "2026-03-30T00:00:00Z",
      "closeTime": "2026-03-30T00:59:59.999Z",
      "open": "10700000",
      "high": "10750000",
      "low": "10680000",
      "close": "10720000",
      "volume": "123.45",
      "quoteVolume": "1323000000",
      "tradeCount": 12345,
      "takerBuyBaseVolume": "61.72",
      "takerBuyQuoteVolume": "662100000"
    }
  ]
}
```

出力項目:

- `venue`: 正規化済み venue
- `symbol`: 正規化済み symbol
- `interval`: 正規化済み interval literal
- `candles[].openTime`: UTC open time
- `candles[].closeTime`: UTC close time
- `candles[].open`: 始値
- `candles[].high`: 高値
- `candles[].low`: 安値
- `candles[].close`: 終値
- `candles[].volume`: base volume
- `candles[].quoteVolume`: quote volume
- `candles[].tradeCount`: trade count
- `candles[].takerBuyBaseVolume`: taker buy base volume
- `candles[].takerBuyQuoteVolume`: taker buy quote volume

実装ルール:

- `candles` は open time 昇順で返す
- `startTime` / `endTime` は tool input では explicit `Z` または numeric offset を持つ RFC 3339 string を受け、UTC に正規化した上で upstream へ epoch milliseconds として変換する
- `startTime` と `endTime` を省略した場合は upstream の most recent klines を返す
- v1 では `timeZone` は UTC 固定とし、Binance の `timeZone` parameter は使わない
- raw tuple array は MCP response に露出せず、named field object に正規化する

Binance public kline v1 導出:

- `candles` は `GetKlines` を正本とする
- `openTime` と `closeTime` は Binance の millisecond timestamp を UTC ISO 8601 string に正規化する
- `interval` は Binance の case-sensitive literal をそのまま返す

初期実装で除外する項目:

- `uiKlines`
- venue-side timezone variant
- symbol discovery
- technical indicator 計算

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
- `invalid_venue`
- `invalid_interval`
- `invalid_limit`
- `invalid_time_range`
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
  - unsupported venue
  - unsupported kline interval
  - kline `limit` out of range
  - kline `startTime > endTime`
- `upstream_error`
  - `GetTicker` / `GetBoardState` / `GetBalance` / `GetCollateral` / `GetChildOrders` / `GetPositions` の transport, http, codec failure
  - `GetKlines` の transport, http, codec failure
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
- `list_markets` が current visible market capability set を返せる
- `get_account_snapshot` が MVP 範囲で実装されている
- `evaluate_order` が機械的成立可否の範囲で実装されている
- 返り値が LLM / Bot に利用可能な構造化形式である
- 副作用を持たない
- bitFlyer venue で live / fixture の両方から検証可能である

### 14.2 Binance public kline extension completion

- `get_klines` が public read tool として実装されている
- `BinanceKlineSymbolSet` の support 済み symbol に対して fixture / live の両方で検証可能である
- `timeZone` を公開せず UTC 固定でも、`GetKlines` fixed contract と矛盾しない
- private credentials を要求しない
- raw tuple array を MCP の named field object へ安定変換できる

### 14.1 最低検証項目

MCP v1 の検証は、fixture test と live test の両方で以下を満たす。

fixture test:

- `get_market_snapshot`
  - supported symbol に対して `rules.*` が registry baseline と一致する
  - `GetBoardState.health` / `state` の代表組み合わせが `active` / `restricted` / `halted` / `unknown` に写像される
  - `GetBoardState` failure は silent fallback せず `upstream_error` になる
- `list_markets`
  - visible tool set に応じて venue / symbol / capabilities が変わる
  - `get_account_snapshot` のような market-specific でない tool は `capabilities` に含めない
- `get_account_snapshot`
  - `permissionModel = bitflyer_private_read_v1` を返す
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
  - `warnings` は closed set として扱い、v1 では `market_order_slippage_risk` と `estimated_fee_not_covered` のみを返す
  - `feeCoverageOk` と `estimatedFee` は optional fee config がないとき `null` を返す
  - `tools/call` result の `_meta` に `schemaVersion` / `dataVersion` / `degraded` が含まれる

live test:

- live test の opt-in と local marker の扱いは [`docs/spec.md`](./spec.md) の live test 契約を正本とする
- `get_market_snapshot` は public read live test として検証可能である
- `get_account_snapshot` と `evaluate_order` は private read live test として検証可能である
- `get_klines` は Binance public read live test として検証可能である
- private live test は read-only に限定し、write side effect を持たない
- live test は `BitflyerMarketRuleRegistry` baseline が drift していないことを検出できる構成にする
- adapter live test は `tests/Adapters/McpServer.LiveTests` に置き、transport は in-memory stdio で検証する

## 15. 実装 backlog

現時点で、採用済み方針の未実装項目はない。
