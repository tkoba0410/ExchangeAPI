# ExchangeAPI v2.2.0 Release Checklist

最終更新: 2026-04-26
位置づけ: v2.2.0 release checklist

状態: `v2.2.0` released

release 完了日: 2026-04-26

完了済み:

- deterministic tests / local pack / local consumer smoke
- live tests safe skip without opt-in
- GitHub Packages publish: library / optional packages `2.2.0`
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Bitflyer.Composition`, `ExchangeApi.Optional.Credentials`, `ExchangeApi.Optional.Logging`
- tag: `v2.2.0`
- GitHub Release: `ExchangeAPI v2.2.0`
- release assets: `exchangeapi-linux-x64`, `exchangeapi-linux-x64.sha256`, `exchangeapi-mcp-linux-x64`, `exchangeapi-mcp-linux-x64.sha256`

## 1. Scope Confirmation

- [x] `v2.2.0` は operational / verification release として扱っている
- [x] package / project consolidation は含めていない
- [x] public API breaking change は含めていない
- [x] `Unified` 層、`Optional.Resilience`、credentials provider 拡張は含めていない
- [x] MCP write tool は含めていない
- [x] order / cancel / withdraw / deposit など state-changing operation は含めていない
- [x] CLI evidence option は追加していない
- [x] evidence helper integration は scripts / verification に限定している
- [x] default では evidence / log を作らない

## 2. Documentation

- [x] `docs/plan-v2.2.0.md` が scope / non-scope を固定している
- [x] `docs/roadmap-post-v2.md` が `v2.2.0` の次を `v3.0.0` として扱っている
- [x] `docs/verification.md` が v2.2.0 の evidence / MCP inspection 運用を説明している
- [x] `docs/distribution.md` が release asset helper を説明している
- [x] `docs/guides/package-publish.md` が GitHub Packages consumer smoke を説明している
- [x] `docs/local-nuget-consumer.md` が local smoke の Optional.Logging 確認を説明している
- [x] `verification/mcp-inspection-live.md` が MCP inspection live verification runbook を固定している

## 3. Consumer Smoke

- [x] local consumer smoke passed
- [x] local consumer smoke が `ExchangeApi.Optional.Logging` を package reference に追加している
- [x] local consumer smoke が `ExchangeApi.Optional.Credentials` と `ExchangeApi.Optional.Logging` の併用を確認している
- [x] local consumer smoke が `Redactor` の最小利用を確認している
- [x] local consumer smoke output は secret-free である
- [x] GitHub Packages consumer smoke script がある
- [x] GitHub Packages consumer smoke 手順がある
- [x] GitHub Packages consumer smoke は token を stdout / stderr に出さない

## 4. Release Assets

- [x] release asset helper passed
- [x] `local/publish/release-assets/v<version>/exchangeapi-linux-x64` が生成される
- [x] `local/publish/release-assets/v<version>/exchangeapi-linux-x64.sha256` が生成される
- [x] `local/publish/release-assets/v<version>/exchangeapi-mcp-linux-x64` が生成される
- [x] `local/publish/release-assets/v<version>/exchangeapi-mcp-linux-x64.sha256` が生成される
- [x] release asset helper は `local/publish/` 配下のみを触る
- [x] CLI / MCP publish は並列実行していない

## 5. Secret-Free Evidence

- [ ] credentials は evidence / logs / stdout / stderr / result / exception に含まれていない
- [ ] API key は evidence / logs / stdout / stderr / result / exception に含まれていない
- [ ] API secret は evidence / logs / stdout / stderr / result / exception に含まれていない
- [ ] signature は evidence / logs / stdout / stderr / result / exception に含まれていない
- [ ] Authorization header は evidence / logs / stdout / stderr / result / exception に含まれていない
- [ ] raw credential profile は evidence にコピーしていない

## 6. MCP Inspection Live Verification

実行条件:

```text
EXCHANGEAPI_RUN_LIVE_TESTS=1
credential profile configured at local/credentials/credential-profile.json
```

対象:

- [ ] `get_collateral_accounts`
- [ ] `get_balance_history`
- [ ] `get_collateral_history`
- [ ] `get_child_orders`

確認項目:

- [ ] private credentials がある場合 `tools/list` に 4 tools が出る
- [ ] credential 未設定時は advertise されない
- [ ] response shape は `accounts` / `items` / `items` / `orders`
- [ ] state-changing operation は増えていない
- [ ] result / error / stdout / stderr に secret がない
- [ ] evidence は `local/evidence/local-live/<yyyymmdd>-v2.2.0-mcp-inspection/` に残している

## 7. Required Local Verification

```bash
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 2.2.0-local.checklist
bash scripts/smoke-local-nuget-consumer.sh 2.2.0-local.checklist
bash scripts/create-release-assets.sh 2.2.0-local.checklist linux-x64 Release
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- [x] deterministic tests passed
- [x] package generation passed
- [x] `ExchangeApi.Optional.Logging.2.2.0-local.checklist.nupkg` が生成される
- [x] local consumer smoke passed
- [x] release asset generation passed
- [x] live tests は opt-in なしで安全に skip する

## 8. GitHub Packages Verification

publish 後に実行する。

```bash
bash scripts/smoke-github-packages-consumer.sh 2.2.0
```

確認項目:

- [x] `ExchangeApi.Exchanges.Bitflyer.Composition 2.2.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Credentials 2.2.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Logging 2.2.0` を restore / build / run できる
- [x] `BitflyerClientFactory` を参照できる
- [x] `PlainTextApiCredentialProviderFactory` を参照できる
- [x] `Redactor` を参照できる
- [x] secret が `[REDACTED]` になる
- [x] token / secret が stdout / stderr に出ない

## 9. Release Gate

- [x] Git working tree が release 前に clean である
- [x] generated `.nupkg` は `local/nuget/` 配下にのみ存在し、git 管理対象に含めない
- [x] generated release assets は `local/publish/` 配下にのみ存在し、git 管理対象に含めない
- [x] `local/evidence/` の run directory は git 管理対象に含めない
- [x] GitHub Packages publish 前に `2.2.0-local.*` で preflight している
- [x] GitHub Packages publish 後、対象 package が見える
- [x] v3.0.0 方針が roadmap に残っている

実施結果:

```text
date: 2026-04-26
deterministic tests: dotnet test ExchangeApi.slnx --no-restore passed
local pack: bash scripts/pack-local-nuget.sh 2.2.0-local.checklist passed
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 2.2.0-local.checklist passed
release asset helper: bash scripts/create-release-assets.sh 2.2.0-local.checklist linux-x64 Release passed
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
package publish: GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 2.2.0 passed
GitHub Packages consumer smoke: bash scripts/smoke-github-packages-consumer.sh 2.2.0 passed
release assets:
  local/publish/release-assets/v2.2.0/exchangeapi-linux-x64
  local/publish/release-assets/v2.2.0/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v2.2.0/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v2.2.0/exchangeapi-mcp-linux-x64.sha256
release asset checksum: passed
```
