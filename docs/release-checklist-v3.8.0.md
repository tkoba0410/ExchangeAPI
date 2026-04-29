# ExchangeAPI v3.8.0 Release Checklist

最終更新: 2026-04-29
位置づけ: v3.8.0 release checklist

状態: v3.8.0 released

## 1. Scope Confirmation

- [x] `v3.8.0` は Realtime Foundation Inventory / Minimal Contract Hardening release として扱っている
- [x] v3.1.0 から v3.7.0 までの bitFlyer Realtime foundation を棚卸ししている
- [x] lifecycle / continuation rule の正本位置を固定している
- [x] DTO-only stream は data-only convenience API として扱っている
- [x] envelope stream は lifecycle / diagnostics / continuity の正本 API として扱っている
- [x] malformed payload / DTO decode failure は envelope stream で `MessageRejected` として扱う
- [x] non-target / unknown channel は target data として流さない
- [x] non-target / unknown channel は stream fault にしない
- [x] non-target / unknown channel は envelope stream で `NonTargetMessageIgnored` diagnostic として観測可能にしている
- [x] cancellation / dispose / normal completion / remote close / transport failure の扱いを plan に固定している
- [x] reconnect / resubscribe / auth replay / `ContinuityLost` の event order を plan に固定している
- [x] realtime error taxonomy は bitFlyer-local の最小整理に限定している
- [x] sample payload catalog rule を contract fixture に限定している
- [x] public API / `src` 変更を evidence-gated にしている
- [x] v3.9.0 への送り出し条件を固定している
- [x] v4.0 stable baseline inventory へ送る項目を分類している
- [x] v5.0 Exchange I/O semantics foundation へ送る項目を分類している
- [x] v6.0 new venue onboarding へ送る項目を分類している

## 2. Non-Scope Confirmation

- [x] 新しい realtime channel を追加していない
- [x] Binance realtime を追加していない
- [x] venue 横断 realtime abstraction を追加していない
- [x] Unified realtime abstraction を追加していない
- [x] board / account / position / order state reconstruction を追加していない
- [x] HTTP + realtime state coordination を追加していない
- [x] Gateway / Platform behavior を追加していない
- [x] simulation を追加していない
- [x] order / cancel / deposit / withdraw などの state-changing operation を追加していない
- [x] core / venue package へ Rx dependency を追加していない
- [x] `ExchangeApi.Optional.Reactive` の public API を拡張していない
- [x] `ExchangeApi.Optional.Testing` を simulation / Gateway / Platform / Strategy testing へ拡張していない
- [x] HTTP contract / consumer verification catch-up を v3.8.0 に含めていない
- [x] CTradeBot 固有導線を追加していない
- [x] broader consumer verification framework を追加していない

## 3. Documentation

- [x] `docs/plan-v3.8.0.md` が scope / non-scope / 裁定 / close preparation を固定している
- [x] `docs/realtime-bitflyer.md` が non-target / unknown channel contract を反映している
- [x] `docs/realtime-diagnostics.md` が `NonTargetMessageIgnored` を反映している
- [x] `docs/roadmap-post-v2.md` が v4 stable baseline、v5 semantics、v6 new venue、v7+ Unified を反映している
- [x] `docs/release-notes/v3.8.0.md` が追加されている

## 4. Verification

close preflight:

```bash
bash scripts/run-release-preflight.sh 3.8.0-local.close linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

確認項目:

- [x] deterministic tests passed
- [x] package generation passed for `3.8.0-local.close`
- [x] local consumer smoke passed for `3.8.0-local.close`
- [x] release asset generation passed for `3.8.0-local.close`
- [x] release asset checksums generated for `3.8.0-local.close`
- [x] live tests skip safely without opt-in
- [x] `NonTargetMessageIgnored` deterministic tests passed
- [x] stdout / stderr / logs / evidence are secret-free
- [x] forbidden layer-specific venue package was not generated

## 5. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.3.8.0-local.close.nupkg
ExchangeApi.Optional.Credentials.3.8.0-local.close.nupkg
ExchangeApi.Optional.Logging.3.8.0-local.close.nupkg
ExchangeApi.Optional.Reactive.3.8.0-local.close.nupkg
ExchangeApi.Optional.Testing.3.8.0-local.close.nupkg
ExchangeApi.Primitives.3.8.0-local.close.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Native.3.8.0-local.close.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.8.0-local.close.nupkg
```

## 6. v3.9.0 Handoff

v3.9.0 へ渡す内容:

- realtime release checklist の最終化
- realtime release notes の最終化
- package / smoke 再確認
- `ExchangeApi.Optional.Testing` / `ExchangeApi.Optional.Reactive` の package smoke 整理
- public / private realtime live verification runbook 確認
- secret-free evidence / log / stdout / stderr close check
- v4 stable baseline へ渡す項目の明文化

v3.9.0 で扱わない内容:

- 新しい realtime feature
- HTTP catch-up
- v4 stable baseline 作業そのもの
- v5 Exchange I/O semantics foundation
- v6 new venue onboarding
- Unified

## 7. Release Gate

- [x] Git working tree が close preparation 前に clean である
- [x] close preflight が通っている
- [x] release preflight が通っている
- [x] live tests が opt-in なしで skip する
- [x] `v3.8.0` の release notes がある
- [x] `v3.8.0` の release checklist がある
- [x] `main` に `v3.8.0` commit が入っている
- [x] `v3.8.0` tag が remote にある
- [x] GitHub Release が作成されている
- [x] release assets が attach されている
- [x] GitHub Packages publish が通っている
- [x] GitHub Packages consumer smoke が通っている
- [x] v3.8.0 に新 channel / Binance realtime / Unified / state reconstruction / Gateway / Platform behavior が含まれていない

close preflight result:

```text
2026-04-29 passed

commands:
- bash scripts/run-release-preflight.sh 3.8.0-local.close linux-x64
- dotnet test ExchangeApi.LiveTests.slnx --no-restore
- git diff --check

package output:
- ExchangeApi.Exchanges.Binance.3.8.0-local.close.nupkg
- ExchangeApi.Exchanges.Bitflyer.3.8.0-local.close.nupkg
- ExchangeApi.Optional.Credentials.3.8.0-local.close.nupkg
- ExchangeApi.Optional.Logging.3.8.0-local.close.nupkg
- ExchangeApi.Optional.Reactive.3.8.0-local.close.nupkg
- ExchangeApi.Optional.Testing.3.8.0-local.close.nupkg
- ExchangeApi.Primitives.3.8.0-local.close.nupkg

release assets:
- local/publish/release-assets/v3.8.0-local.close/exchangeapi-linux-x64
- local/publish/release-assets/v3.8.0-local.close/exchangeapi-linux-x64.sha256
- local/publish/release-assets/v3.8.0-local.close/exchangeapi-mcp-linux-x64
- local/publish/release-assets/v3.8.0-local.close/exchangeapi-mcp-linux-x64.sha256

checksum verification:
- exchangeapi-linux-x64: OK
- exchangeapi-mcp-linux-x64: OK

forbidden layer package check:
- no layer-specific venue package matched v3.8.0-local.close

safe live preflight:
- skipped by default; opt-in requires EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1

live tests:
- dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely without opt-in
```

release result:

```text
2026-04-29 released

release commit:
- f8af354b Add v3.8 release execution instructions

commands:
- bash scripts/run-release-preflight.sh 3.8.0 linux-x64
- dotnet test ExchangeApi.LiveTests.slnx --no-restore
- git diff --check
- git checkout main
- git pull --ff-only origin main
- git merge --ff-only codex/v3.8-dev
- git tag -a v3.8.0 -m "Release v3.8.0"
- git push origin main
- git push origin v3.8.0
- GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 3.8.0
- bash scripts/smoke-github-packages-consumer.sh 3.8.0
- gh release create v3.8.0 ...

package output:
- ExchangeApi.Exchanges.Binance.3.8.0.nupkg
- ExchangeApi.Exchanges.Bitflyer.3.8.0.nupkg
- ExchangeApi.Optional.Credentials.3.8.0.nupkg
- ExchangeApi.Optional.Logging.3.8.0.nupkg
- ExchangeApi.Optional.Reactive.3.8.0.nupkg
- ExchangeApi.Optional.Testing.3.8.0.nupkg
- ExchangeApi.Primitives.3.8.0.nupkg

release assets:
- local/publish/release-assets/v3.8.0/exchangeapi-linux-x64
- local/publish/release-assets/v3.8.0/exchangeapi-linux-x64.sha256
- local/publish/release-assets/v3.8.0/exchangeapi-mcp-linux-x64
- local/publish/release-assets/v3.8.0/exchangeapi-mcp-linux-x64.sha256

checksum verification:
- exchangeapi-linux-x64: OK
- exchangeapi-mcp-linux-x64: OK

GitHub Release:
- https://github.com/tkoba0410/ExchangeAPI/releases/tag/v3.8.0

GitHub Packages:
- publish passed
- consumer smoke passed
```
