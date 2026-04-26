# Adapter Status And History Notes

最終更新: 2026-04-22  
位置づけ: アーカイブ補助文書

本書は、adapter 文書から切り出した phase/status ベースの記述を履歴として残す。  
現行の adapter 契約は [`../cli.md`](../cli.md) と [`../mcp-server.md`](../mcp-server.md) を正本とする。

## 1. MCP Server の過去 status 記述

### 1.1 完成条件

初期完成は以下を満たした時点として扱っていた。

- `get_market_snapshot` が実装されている
- `list_markets` が current visible market capability set を返せる
- `get_account_snapshot` が MVP 範囲で実装されている
- `evaluate_order` が機械的成立可否の範囲で実装されている
- 返り値が LLM / Bot に利用可能な構造化形式である
- 副作用を持たない
- bitFlyer venue で live / fixture の両方から検証可能である

### 1.2 Binance public kline extension completion

- `get_klines` が public read tool として実装されている
- `BinanceKlineSymbolSet` の support 済み symbol に対して fixture / live の両方で検証可能である
- `timeZone` を公開せず UTC 固定でも、`GetKlines` fixed contract と矛盾しない
- private credentials を要求しない
- raw tuple array を MCP の named field object へ安定変換できる

### 1.3 最低検証項目

MCP v1 の検証は、fixture test と live test の両方で次を満たす前提で運用していた。

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

- live test の opt-in と local marker の扱いは [`../spec.md`](../spec.md) の live test 契約を正本とする
- MCP adapter の通常完了判定は `Engineering Complete` を基準とし、private live test の opt-in 未実行は未確認として扱う
- `Live Verified` は opt-in 条件を満たしたうえで adapter live test を明示実行して確認した状態を指す
- `get_market_snapshot` は public read live test として検証可能である
- `get_account_snapshot` と `evaluate_order` は private read live test として検証可能である
- `get_klines` は Binance public read live test として検証可能である
- private live test は read-only に限定し、write side effect を持たない
- live test は `BitflyerMarketRuleRegistry` baseline が drift していないことを検出できる構成にする
- adapter live test は `tests/Adapters/McpServer.LiveTests` に置き、transport は in-memory stdio で検証する
- adapter live test の official solution path は `ExchangeApi.LiveTests.slnx` とする

### 1.4 実装 backlog

現時点で、採用済み方針の未実装項目はない。

### 1.5 Verification Status Labels

- `Engineering Complete`
  - tool surface、schema、solution、CI、通常 test が整合している状態
  - live test は opt-in 未実行で skip されていてよい
- `Live Verified`
  - [`../spec.md`](../spec.md) の opt-in 条件を満たし、対象 live test を明示実行して確認済みの状態

MCP Server v1 の完了宣言は `Engineering Complete` を基準とし、`Live Verified` は追加確認として扱っていた。
