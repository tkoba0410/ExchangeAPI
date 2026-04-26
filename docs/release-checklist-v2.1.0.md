# ExchangeAPI v2.1.0 Release Checklist

最終更新: 2026-04-26
位置づけ: v2.1.0 release checklist

状態: `v2.1.0` released

release 完了日: 2026-04-26

完了済み:

- deterministic tests / local pack / local consumer smoke
- live tests safe skip without opt-in
- MCP private read live verification
- GitHub Packages publish: library / optional packages `2.1.0`
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Bitflyer.Composition`, `ExchangeApi.Optional.Credentials`, `ExchangeApi.Optional.Logging`
- tag: `v2.1.0`
- GitHub Release: `ExchangeAPI v2.1.0`
- release assets: `exchangeapi-linux-x64`, `exchangeapi-linux-x64.sha256`, `exchangeapi-mcp-linux-x64`, `exchangeapi-mcp-linux-x64.sha256`

## 1. Scope Confirmation

- [x] `ExchangeApi.Optional.Logging` を追加した
- [x] safe redaction を追加した
- [x] evidence directory helper を追加した
- [x] JSONL writer を追加した
- [x] MCP read-only inspection tools を追加した
  - [x] `get_collateral_accounts`
  - [x] `get_balance_history`
  - [x] `get_collateral_history`
  - [x] `get_child_orders`
- [x] package / project consolidation は v2.1.0 に含めていない
- [x] `Unified`、`Optional.Resilience`、credentials provider 拡張、samples、MCP client は v2.1.0 に含めていない
- [x] CLI evidence option は v2.1.0 に含めていない
  - [x] `--evidence-run` を追加していない
  - [x] `--evidence-phase` を追加していない

## 2. Documentation

- [x] `docs/plan-v2.1.0.md` が v2.1.0 の採用範囲と非対象を固定している
- [x] `docs/release-notes/v2.1.0.md` が主な変更点を説明している
- [x] `docs/spec.md` が optional logging の位置づけを説明している
- [x] `docs/verification.md` が evidence helper 利用時の secret-free 原則を説明している
- [x] `docs/mcp-server.md` が v2.1.0 MCP inspection tools と error boundary を説明している
- [x] `docs/mcp-tool-catalog.md` が 10 tool の visible surface と response shape を説明している
- [x] `docs/distribution.md` / package publish guide / local consumer guide が `ExchangeApi.Optional.Logging` を含めている

## 3. Optional.Logging Verification

- [x] `ExchangeApi.Optional.Logging` が `ExchangeApi.slnx` に含まれている
- [x] `ExchangeApi.Optional.Logging` が `ExchangeApi.Primitives` のみを参照している
- [x] core / exchange project から `ExchangeApi.Optional.Logging` を参照していない
- [x] redaction deterministic tests が通っている
- [x] evidence helper deterministic tests が通っている
- [x] JSONL writer deterministic tests が通っている
- [x] `apiKey` / `apiSecret` / `signature` / `Authorization` / `ACCESS-*` が test fixture 上で redacted される

## 4. MCP Verification

- [x] `tools/list` に private credentials 利用時の 4 inspection tools が出る
- [x] private credentials がない場合、private inspection tools は advertise されない
- [x] MCP inspection schemas と `docs/mcp-tool-catalog.md` が一致している
- [x] response shape は現行形で固定する
  - [x] `get_collateral_accounts`: `accounts` array
  - [x] `get_balance_history`: `items` array
  - [x] `get_collateral_history`: `items` array
  - [x] `get_child_orders`: `orders` array
- [x] inspection response に `venue` / `accountContext` を含めていない
- [x] MCP state-changing operation を追加していない
- [x] MCP result / error / stderr に API key、secret、signature、Authorization header が含まれない

## 5. Required Local Verification

```bash
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 2.1.0-local.checklist
bash scripts/smoke-local-nuget-consumer.sh 2.1.0-local.checklist
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- [x] deterministic tests passed
- [x] `ExchangeApi.Optional.Logging.2.1.0-local.checklist.nupkg` が生成される
- [x] local consumer smoke passed
- [x] live tests は opt-in なしで安全に skip する

## 6. MCP Private Read Live Verification

v2.1.0 release 前に 1 回実施することを推奨する。

実行条件:

```text
EXCHANGEAPI_RUN_LIVE_TESTS=1
credential profile configured at local/credentials/credential-profile.json
or --credential-profile <path> where supported
```

実行例:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test tests/Adapters/McpServer.LiveTests/ExchangeApi.Adapters.McpServer.LiveTests.csproj --no-restore
```

対象:

- `get_collateral_accounts`
- `get_balance_history`
- `get_collateral_history`
- `get_child_orders`

証跡先:

```text
local/evidence/local-live/<yyyymmdd>-v2.1.0-mcp-inspection/
  runtime/
    artifacts/
    logs/
  notes/
```

実施結果:

```text
date: 2026-04-26
command: EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test tests/Adapters/McpServer.LiveTests/ExchangeApi.Adapters.McpServer.LiveTests.csproj --no-restore
result: passed, 5 passed, 0 failed, 0 skipped
evidence: local/evidence/local-live/20260426-v2.1.0-mcp-inspection/
secret keyword scan: passed
```

証跡ルール:

- [x] API key / secret / signature / Authorization header を含めない
- [x] request / response summary は secret-free な structured JSON のみを残す
- [x] raw credential profile をコピーしない
- [x] stdout / stderr に secret がないことを確認する
- [x] live verification は実施済みのため deferred ではない

## 7. Release Gate

- [x] Git working tree が release 前に clean である
- [x] generated `.nupkg` は `local/nuget/` 配下にのみ存在し、git 管理対象に含めない
- [x] `local/evidence/` の run directory は git 管理対象に含めない
- [x] GitHub Packages publish 前に `2.1.0-local.*` で preflight している
- [x] GitHub Packages publish 後、`ExchangeApi.Optional.Logging 2.1.0` が見える

実施結果:

```text
date: 2026-04-26
local pack: bash scripts/pack-local-nuget.sh 2.1.0
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 2.1.0
publish: GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 2.1.0
GitHub Packages verification:
  ExchangeApi.Primitives 2.1.0 visible
  ExchangeApi.Optional.Logging 2.1.0 visible
GitHub Packages consumer smoke:
  ExchangeApi.Exchanges.Bitflyer.Composition 2.1.0 passed
  ExchangeApi.Optional.Credentials 2.1.0 passed
  ExchangeApi.Optional.Logging 2.1.0 passed
release assets:
  local/publish/release-assets/v2.1.0/exchangeapi-linux-x64
  local/publish/release-assets/v2.1.0/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v2.1.0/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v2.1.0/exchangeapi-mcp-linux-x64.sha256
```
