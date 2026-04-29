# ExchangeAPI v3.9.0 Release Checklist

最終更新: 2026-04-29
位置づけ: v3.9.0 release checklist

状態: v3.9.0 released

## 1. Scope Confirmation

- [x] `v3.9.0` は Realtime Verification / Foundation Close release として扱っている
- [x] v3.9.0 は整理 / 修正 / 仕上げの release である
- [x] 新しい realtime feature を追加していない
- [x] bitFlyer Realtime の stream / channel / DTO contract は `docs/realtime-bitflyer.md` を正本としている
- [x] realtime diagnostic vocabulary / raw frame logging / secret-free observability は `docs/realtime-diagnostics.md` を正本としている
- [x] verification policy / release gate / runbook 参照は `docs/verification.md` に置いている
- [x] public / private / resilience の具体的な live verification 手順は `verification/` 配下に置いている
- [x] actual live run は opt-in 補助確認に留めている
- [x] release gate は deterministic tests / package smoke / live skip / secret-free rule を中心にしている
- [x] local consumer smoke は Bitflyer / Binance / Optional.Credentials / Optional.Logging / Optional.Testing / Optional.Reactive を確認している
- [x] GitHub Packages consumer smoke は同じ package set を確認する
- [x] v4 stable baseline へ送る項目が分類されている
- [x] v5 semantics へ送る項目が分類されている
- [x] v6 new venue へ送る項目が分類されている
- [x] v7+ Unified へ送る項目が分類されている

## 2. Non-Scope Confirmation

- [x] 新しい realtime channel を追加していない
- [x] new public API sugar を追加していない
- [x] Binance realtime を追加していない
- [x] venue 横断 realtime abstraction を追加していない
- [x] Unified realtime abstraction を追加していない
- [x] state reconstruction を追加していない
- [x] Gateway / Platform behavior を追加していない
- [x] simulation を追加していない
- [x] HTTP contract / consumer verification catch-up を含めていない
- [x] v4 stable baseline 作業そのものを含めていない
- [x] v5 Exchange I/O semantics foundation を含めていない
- [x] v6 new venue onboarding を含めていない

## 3. Documentation

- [x] `docs/plan-v3.9.0.md` が大方針 / scope / non-scope / 作業順 / gap classification を固定している
- [x] `docs/realtime-bitflyer.md` が bitFlyer Realtime contract 正本として整理されている
- [x] `docs/realtime-diagnostics.md` が diagnostic vocabulary / observability 正本として整理されている
- [x] `docs/verification.md` が v3.9 Realtime Foundation Close verification を参照している
- [x] `verification/bitflyer-realtime-live.md` が v3.9 close 用に整理されている
- [x] `verification/bitflyer-private-realtime-live.md` が v3.9 close 用に整理されている
- [x] `verification/bitflyer-realtime-resilience.md` が v3.9 close 用に整理されている
- [x] `docs/distribution.md` が v3.9 package smoke 方針を反映している
- [x] `docs/local-nuget-consumer.md` が v3.9 local smoke 対象を反映している
- [x] `docs/guides/package-publish.md` が v3.9 GitHub Packages smoke 対象を反映している
- [x] `docs/release-notes/v3.9.0.md` が追加されている

## 4. Verification

close preflight:

```bash
bash scripts/run-release-preflight.sh 3.9.0-local.close linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

確認項目:

- [x] deterministic tests passed
- [x] package generation passed for `3.9.0-local.close`
- [x] local consumer smoke passed for `3.9.0-local.close`
- [x] release asset generation passed for `3.9.0-local.close`
- [x] release asset checksums generated for `3.9.0-local.close`
- [x] live tests skip safely without opt-in
- [x] GitHub Packages consumer smoke script was verified against a published package
- [x] stdout / stderr / logs / evidence are secret-free
- [x] forbidden layer-specific venue package was not generated

## 5. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.3.9.0-local.close.nupkg
ExchangeApi.Optional.Credentials.3.9.0-local.close.nupkg
ExchangeApi.Optional.Logging.3.9.0-local.close.nupkg
ExchangeApi.Optional.Reactive.3.9.0-local.close.nupkg
ExchangeApi.Optional.Testing.3.9.0-local.close.nupkg
ExchangeApi.Primitives.3.9.0-local.close.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Native.3.9.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.9.0-local.close.nupkg
```

## 6. Secret-Free Gate

- [x] stdout に secret が出ていない
- [x] stderr に secret が出ていない
- [x] package smoke output に secret が出ていない
- [x] evidence / logs を残す場合、API key / API secret / signature / Authorization 相当値 / private auth payload / raw credential profile を含めていない
- [x] GitHub Packages token が stdout / stderr / logs / evidence に出ていない

## 7. v4 Handoff

v4 stable baseline へ渡す内容:

- HTTP contract / consumer verification catch-up
- ExchangeAPI 全体の docs / tests / scripts / package / smoke / release hardening
- secret scan / evidence hardening の script 化候補
- CTradeBot / ExecutionGateway が使う前提の不足棚卸し

v5 以降へ送る内容:

- Exchange I/O semantics foundation -> v5
- new venue public read MVP -> v6
- Unified -> v7+

ExchangeAPI に入れない内容:

- state reconstruction
- Gateway / Platform behavior
- ledger / position / allocation

## 8. Release Gate

- [x] Git working tree が close preparation 前に clean である
- [x] close preflight が通っている
- [x] live tests が opt-in なしで skip する
- [x] `v3.9.0` の release notes がある
- [x] `v3.9.0` の release checklist がある
- [x] v3.9.0 に新 feature / Binance realtime / Unified / state reconstruction / Gateway / Platform behavior が含まれていない

close preflight result:

```text
2026-04-29 passed

commands:
- bash scripts/run-release-preflight.sh 3.9.0-local.close linux-x64
- dotnet test ExchangeApi.LiveTests.slnx --no-restore
- git diff --check
- bash scripts/smoke-github-packages-consumer.sh 3.8.0

package output:
- ExchangeApi.Exchanges.Binance.3.9.0-local.close.nupkg
- ExchangeApi.Exchanges.Bitflyer.3.9.0-local.close.nupkg
- ExchangeApi.Optional.Credentials.3.9.0-local.close.nupkg
- ExchangeApi.Optional.Logging.3.9.0-local.close.nupkg
- ExchangeApi.Optional.Reactive.3.9.0-local.close.nupkg
- ExchangeApi.Optional.Testing.3.9.0-local.close.nupkg
- ExchangeApi.Primitives.3.9.0-local.close.nupkg

release assets:
- local/publish/release-assets/v3.9.0-local.close/exchangeapi-linux-x64
- local/publish/release-assets/v3.9.0-local.close/exchangeapi-linux-x64.sha256
- local/publish/release-assets/v3.9.0-local.close/exchangeapi-mcp-linux-x64
- local/publish/release-assets/v3.9.0-local.close/exchangeapi-mcp-linux-x64.sha256

checksum verification:
- exchangeapi-linux-x64: OK
- exchangeapi-mcp-linux-x64: OK

forbidden layer package check:
- no layer-specific venue package matched v3.9.0-local.close

safe live preflight:
- skipped by default; opt-in requires EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1

live tests:
- dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely without opt-in

GitHub Packages smoke script check:
- verified against published 3.8.0 package set
```

release result:

```text
2026-04-29 released

commands:
- bash scripts/run-release-preflight.sh 3.9.0 linux-x64
- dotnet test ExchangeApi.LiveTests.slnx --no-restore
- git diff --check
- git merge --ff-only codex/v3.9-dev
- git tag -a v3.9.0 -m "Release v3.9.0"
- git push origin main
- git push origin v3.9.0
- GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 3.9.0
- bash scripts/smoke-github-packages-consumer.sh 3.9.0
- gh release create v3.9.0 ...

package output:
- ExchangeApi.Exchanges.Binance.3.9.0.nupkg
- ExchangeApi.Exchanges.Bitflyer.3.9.0.nupkg
- ExchangeApi.Optional.Credentials.3.9.0.nupkg
- ExchangeApi.Optional.Logging.3.9.0.nupkg
- ExchangeApi.Optional.Reactive.3.9.0.nupkg
- ExchangeApi.Optional.Testing.3.9.0.nupkg
- ExchangeApi.Primitives.3.9.0.nupkg

release assets:
- local/publish/release-assets/v3.9.0/exchangeapi-linux-x64
- local/publish/release-assets/v3.9.0/exchangeapi-linux-x64.sha256
- local/publish/release-assets/v3.9.0/exchangeapi-mcp-linux-x64
- local/publish/release-assets/v3.9.0/exchangeapi-mcp-linux-x64.sha256

checksum verification:
- exchangeapi-linux-x64: OK
- exchangeapi-mcp-linux-x64: OK

forbidden layer package check:
- no layer-specific venue package matched v3.9.0

live tests:
- dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely without opt-in

GitHub Packages:
- publish passed for v3.9.0 package set
- consumer smoke passed for v3.9.0

GitHub Release:
- https://github.com/tkoba0410/ExchangeAPI/releases/tag/v3.9.0
```
