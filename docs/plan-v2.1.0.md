# ExchangeAPI v2.1.0 Plan

最終更新: 2026-04-26
位置づけ: v2.1.0 実施計画

## 1. 目的

`v2.1.0` では、運用証跡と安全なログ出力を強化し、MCP Server の read-only inspection surface を拡張する。

採用範囲:

- `ExchangeApi.Optional.Logging`
- safe redaction
- evidence directory helper
- MCP read-only inspection tools
  - `get_collateral_accounts`
  - `get_balance_history`
  - `get_collateral_history`
  - `get_child_orders`

## 2. 非対象

以下は `v2.1.0` では扱わない。

- package / project consolidation
  - venue 単位 project / package への統合は `v3.0.0` 候補とする
- `Unified` 層の実装
- `ExchangeApi.Optional.Resilience`
- credentials provider 拡張
- `samples/` directory
- MCP client / human trial CLI

## 3. Logging / Evidence 方針

- logging / evidence helper は core ではなく `ExchangeApi.Optional.Logging` に置く
- core / exchange project から optional package を参照しない
- `ExchangeApi.Optional.Logging` は `ExchangeApi.Primitives` のみ参照してよい
- CLI / MCP / live test は必要に応じて optional logging を参照してよい
- credentials、API key、API secret、signature、Authorization header は log / evidence / exception / result に含めない
- redaction は logging の後付け安全策ではなく、logging / evidence の前提部品として扱う

## 4. MCP 方針

- v2.1.0 の MCP 拡張は read-only inspection tool に限定する
- 注文、キャンセル、入金、出金などの state-changing operation は追加しない
- private read tool は credential profile 方針に従う
- environment variable で API key / secret を読まない
- private credentials を解決できない場合、private inspection read tool は `tools/list` に advertise しない

## 5. 完了条件

- `ExchangeApi.Optional.Logging` が solution と pack 対象に含まれる
- redaction / evidence directory helper / JSONL writer に deterministic tests がある
- MCP read-only 4 tools が実装され、tool catalog と tests が揃っている
- `dotnet test ExchangeApi.slnx --no-restore` が成功する
- package generation が成功する
- log / evidence / MCP result に credentials / signature が残らないことを test で確認する
