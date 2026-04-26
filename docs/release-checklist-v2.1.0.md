# ExchangeAPI v2.1.0 Release Checklist

最終更新: 2026-04-26
位置づけ: v2.1.0 release checklist

## 1. Scope Confirmation

- [ ] `ExchangeApi.Optional.Logging` を追加した
- [ ] safe redaction を追加した
- [ ] evidence directory helper を追加した
- [ ] JSONL writer を追加した
- [ ] MCP read-only inspection tools を追加した
  - [ ] `get_collateral_accounts`
  - [ ] `get_balance_history`
  - [ ] `get_collateral_history`
  - [ ] `get_child_orders`
- [ ] package / project consolidation は v2.1.0 に含めていない
- [ ] `Unified`、`Optional.Resilience`、credentials provider 拡張、samples、MCP client は v2.1.0 に含めていない
- [ ] CLI evidence option は v2.1.0 に含めていない
  - [ ] `--evidence-run` を追加していない
  - [ ] `--evidence-phase` を追加していない

## 2. Documentation

- [ ] `docs/plan-v2.1.0.md` が v2.1.0 の採用範囲と非対象を固定している
- [ ] `docs/release-notes/v2.1.0.md` が主な変更点を説明している
- [ ] `docs/spec.md` が optional logging の位置づけを説明している
- [ ] `docs/verification.md` が evidence helper 利用時の secret-free 原則を説明している
- [ ] `docs/mcp-server.md` が v2.1.0 MCP inspection tools と error boundary を説明している
- [ ] `docs/mcp-tool-catalog.md` が 10 tool の visible surface と response shape を説明している
- [ ] `docs/distribution.md` / package publish guide / local consumer guide が `ExchangeApi.Optional.Logging` を含めている

## 3. Optional.Logging Verification

- [ ] `ExchangeApi.Optional.Logging` が `ExchangeApi.slnx` に含まれている
- [ ] `ExchangeApi.Optional.Logging` が `ExchangeApi.Primitives` のみを参照している
- [ ] core / exchange project から `ExchangeApi.Optional.Logging` を参照していない
- [ ] redaction deterministic tests が通っている
- [ ] evidence helper deterministic tests が通っている
- [ ] JSONL writer deterministic tests が通っている
- [ ] `apiKey` / `apiSecret` / `signature` / `Authorization` / `ACCESS-*` が test fixture 上で redacted される

## 4. MCP Verification

- [ ] `tools/list` に private credentials 利用時の 4 inspection tools が出る
- [ ] private credentials がない場合、private inspection tools は advertise されない
- [ ] MCP inspection schemas と `docs/mcp-tool-catalog.md` が一致している
- [ ] response shape は現行形で固定する
  - [ ] `get_collateral_accounts`: `accounts` array
  - [ ] `get_balance_history`: `items` array
  - [ ] `get_collateral_history`: `items` array
  - [ ] `get_child_orders`: `orders` array
- [ ] inspection response に `venue` / `accountContext` を含めていない
- [ ] MCP state-changing operation を追加していない
- [ ] MCP result / error / stderr に API key、secret、signature、Authorization header が含まれない

## 5. Required Local Verification

```bash
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 2.1.0-local.checklist
bash scripts/smoke-local-nuget-consumer.sh 2.1.0-local.checklist
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- [ ] deterministic tests passed
- [ ] `ExchangeApi.Optional.Logging.2.1.0-local.checklist.nupkg` が生成される
- [ ] local consumer smoke passed
- [ ] live tests は opt-in なしで安全に skip する

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

証跡ルール:

- [ ] API key / secret / signature / Authorization header を含めない
- [ ] request / response summary は secret-free な structured JSON のみを残す
- [ ] raw credential profile をコピーしない
- [ ] stdout / stderr に secret がないことを確認する
- [ ] 実施できない場合は checklist 上で明示的に deferred とし、理由を残す

## 7. Release Gate

- [ ] Git working tree が release 前に clean である
- [ ] generated `.nupkg` は `local/nuget/` 配下にのみ存在し、git 管理対象に含めない
- [ ] `local/evidence/` の run directory は git 管理対象に含めない
- [ ] GitHub Packages publish 前に `2.1.0-local.*` で preflight している
- [ ] GitHub Packages publish 後、`ExchangeApi.Optional.Logging 2.1.0` が見える
