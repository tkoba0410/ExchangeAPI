# ExchangeAPI v2.2.0 Plan

最終更新: 2026-04-26
位置づけ: v2.2.0 実施計画

本書は `v2.2.0` の実施範囲を固定する。
`v2.2.0` は operational / verification release として扱い、新しい大規模機能や破壊的変更は入れない。

## 1. 目的

`v2.2.0` では、`v2.1.0` で追加した `ExchangeApi.Optional.Logging`、evidence helper、MCP inspection surface を前提に、release・verification・evidence 運用の再現性を上げる。

重点:

- release verification script 整理
- local / GitHub Packages consumer smoke 強化
- release asset helper
- MCP inspection live verification runbook
- scripts / verification に限定した evidence helper integration

制約:

- default では evidence / log を作らない
- evidence helper integration は opt-in の scripts / verification に限定する
- credentials、API key、API secret、signature、Authorization header は evidence / log / result / exception / stdout / stderr に含めない
- raw credential profile を evidence へコピーしない

## 2. 対象スコープ

### 2.1 Release Verification Script 整理

- local package smoke に `ExchangeApi.Optional.Logging` を含める
- local package smoke で `ExchangeApi.Optional.Credentials` と `ExchangeApi.Optional.Logging` の併用を確認する
- GitHub Packages consumer smoke を script 化する
- release asset 作成を script 化する

### 2.2 Consumer Smoke

local consumer smoke:

- local NuGet feed から `ExchangeApi.Exchanges.Bitflyer.Composition` を restore できること
- local NuGet feed から `ExchangeApi.Optional.Credentials` を restore できること
- local NuGet feed から `ExchangeApi.Optional.Logging` を restore できること
- `BitflyerClientFactory`、`PlainTextApiCredentialProviderFactory`、`Redactor` を参照できること
- secret value が `[REDACTED]` になること
- smoke output が secret-free であること

GitHub Packages consumer smoke:

- version を引数で受け取る
- GitHub Packages feed から restore / build / run する
- token は環境変数または `gh auth token` から取得する
- token を stdout / stderr に出さない
- temp directory を使い、終了時に削除する

### 2.3 Release Asset Helper

`scripts/create-release-assets.sh` を追加し、次の layout を生成する。

```text
local/publish/release-assets/v2.2.0/
  exchangeapi-linux-x64
  exchangeapi-linux-x64.sha256
  exchangeapi-mcp-linux-x64
  exchangeapi-mcp-linux-x64.sha256
```

規則:

- `scripts/publish-cli-local.sh` を呼ぶ
- `scripts/publish-mcp-local.sh` を呼ぶ
- CLI / MCP publish は共有 `bin/obj` を使うため並列実行しない
- asset 名を固定する
- SHA-256 checksum を生成する
- `local/publish/` 配下のみを触る
- 生成物は git 管理対象にしない

### 2.4 MCP Inspection Live Verification Runbook

`verification/mcp-inspection-live.md` を追加し、次の private read inspection tools の live verification 手順を固定する。

- `get_collateral_accounts`
- `get_balance_history`
- `get_collateral_history`
- `get_child_orders`

実行条件:

```text
EXCHANGEAPI_RUN_LIVE_TESTS=1
credential profile configured at local/credentials/credential-profile.json
```

確認観点:

- private credentials がある場合 `tools/list` に 4 tools が出る
- credential 未設定時は advertise されない
- response shape は `accounts` / `items` / `items` / `orders`
- state-changing operation は増えていない
- result / error / stdout / stderr に secret がない
- evidence は `local/evidence/local-live/<yyyymmdd>-v2.2.0-mcp-inspection/` に残す

## 3. 非対象

以下は `v2.2.0` では扱わない。

- package / project consolidation
- `Unified` 層の実装
- `ExchangeApi.Optional.Resilience`
- credentials provider 拡張
- full MCP client
- MCP write tool
- order / cancel / withdraw / deposit など state-changing operation
- public API の破壊的変更
- CLI evidence option の追加
- v3.0.0 詳細設計

## 4. v3.0.0 方針

`v2.2.0` の次は `v3.0.0` を想定する。

`v3.0.0` の主題候補:

- package / project consolidation
- venue 単位の package 導線整理
- 破壊的変更を許容した論理性・合理性・可読性の優先

`v2.2.0` では v3 詳細設計までは行わず、候補を roadmap に残す。

## 5. 完了条件

- `docs/release-checklist-v2.2.0.md` が追加されている
- local consumer smoke が `ExchangeApi.Optional.Logging` を確認している
- GitHub Packages consumer smoke script がある
- release asset helper がある
- MCP inspection live verification runbook がある
- evidence helper integration は scripts / verification に限定されている
- default では evidence / log を作らない
- secret-free rule が docs / scripts / checklist に反映されている
- deterministic tests が通る
- package generation が通る
- release asset generation が通る
- live tests は opt-in なしで skip する
- package / project consolidation が含まれていない
- v3.0.0 方針が roadmap に残っている

## 6. Verification

最低限の実行:

```bash
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 2.2.0-local.checklist
bash scripts/smoke-local-nuget-consumer.sh 2.2.0-local.checklist
bash scripts/create-release-assets.sh 2.2.0-local.checklist linux-x64 Release
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

GitHub Packages publish 後:

```bash
bash scripts/smoke-github-packages-consumer.sh 2.2.0
```
