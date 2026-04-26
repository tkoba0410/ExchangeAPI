# ExchangeAPI v2.2.0 Plan

最終更新: 2026-04-26
位置づけ: v2.2.0 初期計画

本書は `v2.2.0` の候補を整理するための初期 plan である。
採用範囲はこの文書で固定するが、実装前に対象を小さく裁定する。

## 1. 目的

`v2.2.0` では、`v2.1.0` で追加した logging / evidence / MCP inspection surface を前提に、運用導線と release verification を薄く強化する。

破壊的変更は入れない。
package / project consolidation は引き続き `v3.0.0` 候補として扱い、`v2.2.0` では扱わない。

## 2. 推奨候補

### 2.1 Evidence Helper Integration

`ExchangeApi.Optional.Logging` の evidence directory helper を、manual / live verification の運用導線から使えるようにする。

候補:

- safe live test / MCP live verification 用の evidence run directory 作成 helper
- verification script から `local/evidence/<phase>/<yyyymmdd>-<label>/` を作成する薄い導線
- stdout / stderr / structured summary を secret-free に保存する最小 helper

制約:

- default では evidence / log を勝手に作らない
- opt-in のみ
- credentials、API key、API secret、signature、Authorization header は evidence / log / result に含めない
- raw credential profile はコピーしない

### 2.2 Release Verification Script 整理

`v2.1.0` release で実施した確認を、次回以降に再実行しやすい形へ整理する。

候補:

- local package smoke に `ExchangeApi.Optional.Logging` を含める
- GitHub Packages consumer smoke の手順を script 化または runbook 化する
- release asset 作成手順を script 化する
- release checklist の生成物確認項目を v2.2.0 向けに整理する

### 2.3 MCP Inspection Operational Runbook

MCP private read inspection tools の実運用確認を、手順として再実行しやすくする。

候補:

- `get_collateral_accounts`
- `get_balance_history`
- `get_collateral_history`
- `get_child_orders`

確認観点:

- `tools/list` に private read tool が出る
- credential 未設定時は private read tool が advertise されない
- response shape は `accounts` / `items` / `items` / `orders`
- state-changing operation を追加しない
- result / error / stderr に secret を含めない

## 3. 非対象

以下は `v2.2.0` では扱わない。

- package / project consolidation
- `Unified` 層の実装
- `ExchangeApi.Optional.Resilience`
- credentials provider 拡張
- full MCP client 実装
- write operation の MCP tool 追加
- order / cancel / withdraw / deposit など state-changing operation
- public API の破壊的変更

## 4. 裁定待ち

実装前に次を決める。

- evidence helper を接続する対象を script / live test / CLI のどこまでにするか
- GitHub Packages consumer smoke を script 化するか、runbook に留めるか
- release asset 作成を script 化するか、manual 手順に留めるか
- v2.2.0 を documentation / operations release として小さく切るか、追加機能を含めるか

## 5. 推奨順序

1. v2.2.0 の採用範囲を裁定する
2. `docs/release-checklist-v2.2.0.md` を追加する
3. verification / distribution / package publish docs を更新する
4. smoke / release helper script を必要最小限で追加または更新する
5. deterministic tests を追加する
6. local verification を実行する
7. package / release preflight を実行する

