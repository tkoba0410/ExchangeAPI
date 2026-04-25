# MCP Server Specification

最終更新: 2026-04-25  
位置づけ: MCP Server adapter 正本

## 1. 位置づけ

本書は、ExchangeAPI library の上に載る MCP Server adapter の現行契約を定義する正本である。  
library の設計正本は [`docs/spec.md`](./spec.md) に置き、  
本書では Bot 向け MCP Server の責務、依存、tool 契約、動作モードを扱う。

本MCP Server は、Codex / LLM を利用する Bot に対し、売買判断に必要な市場情報・口座情報・注文評価機能を、read / evaluate 専用 interface として提供する。
また、開発中確認と運用 inspection のために、副作用を持たない read-only 情報を段階的に公開してよい。
過去 phase の計画履歴は [`docs/archive/`](./archive/README.md) を参照する。

注記:

- 本文中に残る `Stage11` は履歴ラベルであり、現行契約の優先順位は stage 名ではなく文書体系ガイドに従う
- 個別 tool の surface と schema 詳細は [`docs/mcp-tool-catalog.md`](./mcp-tool-catalog.md) に分離して管理する

### 1.1 Version Notes

- `v2.0.0` で採用した MCP 変更は [`docs/breaking-changes-v2.0.0.md`](./breaking-changes-v2.0.0.md) と [`docs/migration-v2.0.0.md`](./migration-v2.0.0.md) を参照する
- MCP 関連の主な変更は、`upstream_error.details` への additive detail key 追加と、CLI との shared vocabulary 整合である
- MCP tool surface 自体は `v2.0.0` でも bot-oriented abstraction を維持し、CLI command や library endpoint の 1:1 mirror へは戻さない
- 時刻表示については、structured response は UTC / structured contract を維持しつつ、human-facing log や CLI 表示とは役割を分ける方針である
- private credentials については、`v2.0.0` で core 正本から特定の storage / encryption recipe を外し、auth provider 契約へ責務を寄せる
- auth provider の具体 shape は `IApiCredentialProvider.OpenSessionAsync(...)` 型を採用するが、MCP adapter は通常その session 境界を内部で扱う想定である
- MCP Server の private credentials は `--credential-profile <path>` または `local/credentials/credential-profile.json` から解決し、API key 読み込みに環境変数を使わない
- credential failure は MCP adapter が通知する。通知は `tools/list` の公開制御、`tools/call` の structured error、stderr diagnostic に分けて扱う

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

- 現行契約の依存は `McpServer -> Composition` を基本とする
- MCP Server は venue ごとの `Composition` project を経由して library を利用する
- MCP Server は必要に応じて複数の library call を集約して 1 tool response を構築してよい
- MCP Server から `Native` / `Protocol` / `Vocabulary` project を直接参照してはならない
- MCP Server は concrete endpoint / runtime / signer / transport を直接配線しない

### 6.1 物理配置

- MCP Server project は `src/Adapters/McpServer/ExchangeApi.Adapters.McpServer.csproj` に置く
- MCP Server test project は `tests/Adapters/McpServer.Tests/ExchangeApi.Adapters.McpServer.Tests.csproj` に置く
- MCP Server は external adapter であり、`src/Exchanges/<Venue>/` 配下に置いてはならない
- direct project reference は venue ごとの `Composition` project と optional credentials project に限定する
  - `src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj`
  - `src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj`
  - `src/Optional/Credentials/ExchangeApi.Optional.Credentials.csproj`

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

### 7.3.1 Two-Tier Tool Surface

MCP tool surface は次の 2 層で管理してよい。

- `Core Bot Tools`
  - bot / LLM 本番導線で安定利用する責務単位 tool
- `Inspection Read Tools`
  - 開発中確認、運用 inspection、manual diagnosis のための read-only tool

`Inspection Read Tools` を追加しても、副作用禁止と structured response の原則は維持しなければならない。

### 7.4 Bot-Oriented Abstraction

tool surface は library endpoint の 1:1 mirror を要求しない。  
Bot / LLM が安定して利用できる責務単位へ集約してよい。
ただし、`Inspection Read Tools` については library の read-only capability に近い粒度を許容してよい。

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
3. venue 固有の一時的 shortcut ではなく、現行 support boundary と整合する

`Inspection Read Tools` の追加条件:

1. read-only である
2. 資産状態を変更しない
3. 開発中確認または運用 inspection に継続的価値がある
4. 既存 aggregate tool に無理なく吸収できない

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

## 9. Tool Ledger

- current tool surface と tool schema 詳細は [`docs/mcp-tool-catalog.md`](./mcp-tool-catalog.md) を参照する

## 10. エラー仕様

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
- private credentials が未設定または解決不能な場合、private tool は `tools/list` から advertise してはならない
- advertise 済み tool の call 時点で credential failure が発生した場合、`upstream_error` / `account_unavailable` として返す
- credential failure の `details` には secret-safe な `credentialErrorKind`、`venue`、`provider`、`reason`、`requiredCredentialProfile` を含めてよい
- API key / secret / 署名値 / 認証 header は `message`、`details`、`_meta`、stderr に出してはならない
- stderr diagnostic は operator 向け通知であり、stdout の MCP JSON-RPC stream にログを混ぜてはならない

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

## 11. ログ / 実装上の制約

- transport は初期実装で `stdio` を採用する
- stdio transport は latest MCP transport に従い、`stdin` / `stdout` で 1 行 1 JSON-RPC message を扱う
- `stdout` は MCP message 専用とし、ログを書かない
- ログは `stderr` または別ログ出力に限定する
- 初期 server surface は `initialize`、`ping`、`tools/list`、`tools/call` に限定する
- `tools/list` は各 tool の `inputSchema` と `outputSchema` を返す
- 返り値は安定した JSON 構造を維持する
- sensitive data をログに出してはならない

## 12. Verification Notes

- live test の opt-in と local marker の扱いは [`docs/spec.md`](./spec.md) の live test 契約を正本とする
- adapter 固有の completion / verification 履歴は [`docs/archive/adapter-status-and-history.md`](./archive/adapter-status-and-history.md) を参照する
