# ExchangeAPI v3.2.0 Release Checklist

最終更新: 2026-04-27
位置づけ: v3.2.0 release checklist

状態: `v3.2.0` released

release 完了日: 2026-04-27

完了済み:

- deterministic tests / local pack / local consumer smoke
- live tests safe skip without opt-in
- GitHub Packages publish: library / optional packages `3.2.0`
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Bitflyer`, `ExchangeApi.Exchanges.Binance`, `ExchangeApi.Primitives`, `ExchangeApi.Optional.Credentials`, `ExchangeApi.Optional.Logging`
- tag: `v3.2.0`
- GitHub Release: `v3.2.0`
- release assets: `exchangeapi-linux-x64`, `exchangeapi-linux-x64.sha256`, `exchangeapi-mcp-linux-x64`, `exchangeapi-mcp-linux-x64.sha256`

## 1. Scope Confirmation

- [x] `v3.2.0` は Realtime hardening / venue onboarding preparation release として扱っている
- [x] v3.1.0 の bitFlyer Realtime public API surface を壊していない
- [x] typed stream 終了時の best-effort unsubscribe behavior を固定している
- [x] Realtime sample payload decode tests がある
- [x] bitFlyer Realtime getting started guide がある
- [x] bitFlyer Realtime live verification runbook がある
- [x] venue onboarding guide がある
- [x] private realtime 実装は含めていない
- [x] private realtime は v3.3.0 以降候補として design note に留めている
- [x] new venue の正式実装は含めていない
- [x] `Unified` 実装は含めていない
- [x] order / cancel / deposit / withdraw など state-changing operation は含めていない
- [x] `System.Reactive` dependency は含めていない
- [x] `IObservable<T>` public API は含めていない

## 2. Documentation

- [x] `docs/plan-v3.2.0.md` が scope / non-scope / verification を固定している
- [x] `docs/realtime-bitflyer.md` が v3.2.0 hardening と private realtime design note を含む
- [x] `docs/guides/realtime-bitflyer-getting-started.md` がある
- [x] `verification/bitflyer-realtime-live.md` がある
- [x] `docs/venue-onboarding.md` がある
- [x] `docs/local-nuget-consumer.md` が v3.2.0 local smoke を説明している
- [x] `docs/guides/package-publish.md` が v3.2.0 publish / smoke を説明している
- [x] `docs/release-notes/v3.2.0.md` が追加されている

## 3. Verification

release preflight:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.2.0
bash scripts/smoke-local-nuget-consumer.sh 3.2.0
bash scripts/create-release-assets.sh 3.2.0 linux-x64 Release
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- [x] deterministic tests passed for release version
- [x] package generation passed for `3.2.0`
- [x] local consumer smoke passed for `3.2.0`
- [x] release asset generation passed for `3.2.0`
- [x] release asset checksums passed for `3.2.0`
- [x] live tests skip safely without opt-in
- [x] stdout / stderr / logs / evidence are secret-free

## 4. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.2.0.nupkg
ExchangeApi.Exchanges.Bitflyer.3.2.0.nupkg
ExchangeApi.Optional.Credentials.3.2.0.nupkg
ExchangeApi.Optional.Logging.3.2.0.nupkg
ExchangeApi.Primitives.3.2.0.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.2.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.2.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.2.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.2.0.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.2.0.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.2.0.nupkg
ExchangeApi.Exchanges.Binance.Native.3.2.0.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.2.0.nupkg
```

## 5. GitHub Packages Verification

publish 後に実行する。

```bash
bash scripts/smoke-github-packages-consumer.sh 3.2.0
```

確認項目:

- [x] `ExchangeApi.Exchanges.Bitflyer 3.2.0` を restore / build / run できる
- [x] `ExchangeApi.Exchanges.Binance 3.2.0` を restore / build / run できる
- [x] `ExchangeApi.Primitives 3.2.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Credentials 3.2.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Logging 3.2.0` を restore / build / run できる
- [x] bitFlyer Realtime factory / channel vocabulary を参照できる
- [x] token / secret が stdout / stderr に出ない

## 6. Release Gate

- [x] Git working tree が release 前に clean である
- [x] `main` に `v3.2.0` commit が入っている
- [x] `v3.2.0` tag が remote にある
- [x] GitHub Release が作成されている
- [x] release assets が attach されている
- [x] GitHub Packages smoke が通っている
- [x] `v3.2.0` に private realtime / Unified / new venue / state-changing operation が含まれていない

local preflight result:

```text
date: 2026-04-27
diff check: git diff --check passed
build: dotnet build ExchangeApi.slnx passed
deterministic tests: dotnet test ExchangeApi.slnx --no-restore passed
local pack: bash scripts/pack-local-nuget.sh 3.2.0 passed
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 3.2.0 passed
release asset helper: bash scripts/create-release-assets.sh 3.2.0 linux-x64 Release passed
release asset checksum: sha256sum -c *.sha256 passed in local/publish/release-assets/v3.2.0
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
packages:
  ExchangeApi.Exchanges.Binance.3.2.0.nupkg
  ExchangeApi.Exchanges.Bitflyer.3.2.0.nupkg
  ExchangeApi.Optional.Credentials.3.2.0.nupkg
  ExchangeApi.Optional.Logging.3.2.0.nupkg
  ExchangeApi.Primitives.3.2.0.nupkg
release assets:
  local/publish/release-assets/v3.2.0/exchangeapi-linux-x64
  local/publish/release-assets/v3.2.0/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v3.2.0/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v3.2.0/exchangeapi-mcp-linux-x64.sha256
```

release result:

```text
date: 2026-04-27
main push: passed
tag push: v3.2.0 passed
package publish: GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 3.2.0 passed
GitHub Packages consumer smoke: bash scripts/smoke-github-packages-consumer.sh 3.2.0 passed
GitHub Release: https://github.com/tkoba0410/ExchangeAPI/releases/tag/v3.2.0
```
