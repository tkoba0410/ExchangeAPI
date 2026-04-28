# ExchangeAPI v3.6.0 Release Checklist

最終更新: 2026-04-29
位置づけ: v3.6.0 release checklist

状態: `v3.6.0` released

release 完了日: 2026-04-29

完了済み:

- deterministic tests / local pack / local consumer smoke
- live tests safe skip without opt-in
- GitHub Packages publish: library / optional packages `3.6.0`
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Bitflyer`, `ExchangeApi.Exchanges.Binance`, `ExchangeApi.Primitives`, `ExchangeApi.Optional.Credentials`, `ExchangeApi.Optional.Logging`, `ExchangeApi.Optional.Testing`
- tag: `v3.6.0`
- GitHub Release: `v3.6.0`
- release assets: `exchangeapi-linux-x64`, `exchangeapi-linux-x64.sha256`, `exchangeapi-mcp-linux-x64`, `exchangeapi-mcp-linux-x64.sha256`

## 1. Scope Confirmation

- [x] `v3.6.0` は Realtime Replay / Testing Foundation release として扱っている
- [x] `ExchangeApi.Optional.Testing` を追加している
- [x] `RealtimeReplayFrame` を replay input の主モデルとして公開している
- [x] `RealtimeReplayResult<T>` を replay result として公開している
- [x] `BitflyerRealtimeReplayRunner` を bitFlyer public realtime raw frame replay の最小 public API として追加している
- [x] replay 対象は raw frame 起点の decode / diagnostic testing に限定している
- [x] sample payload catalog は tests 配下の payload catalog であり scenario catalog ではない
- [x] `ExchangeApi.Optional.Testing` は `ExchangeApi.Optional.Logging` を参照していない
- [x] local / GitHub Packages consumer smoke は `ExchangeApi.Optional.Testing` を確認する
- [x] `ExchangeApi.Optional.Reactive` は含めていない
- [x] `System.Reactive` dependency は含めていない
- [x] `IObservable<T>` public API は含めていない
- [x] JSONL log replay は含めていない
- [x] simulation / Gateway / Platform / Strategy testing は含めていない
- [x] state reconstruction は含めていない
- [x] Binance realtime は含めていない
- [x] Unified は含めていない
- [x] state-changing operation は含めていない

## 2. Documentation

- [x] `docs/plan-v3.6.0.md` が scope / non-scope / implementation result / close preparation を固定している
- [x] `docs/roadmap-post-v2.md` が v3.6 を採用範囲として扱っている
- [x] `docs/distribution.md` が `ExchangeApi.Optional.Testing` package を反映している
- [x] `docs/local-nuget-consumer.md` が local consumer smoke の `ExchangeApi.Optional.Testing` 確認を反映している
- [x] `docs/guides/package-publish.md` が `ExchangeApi.Optional.Testing` publish 対象を反映している
- [x] `docs/release-notes/v3.6.0.md` が追加されている

## 3. Verification

release-candidate preflight:

```bash
bash scripts/run-release-preflight.sh 3.6.0-local.preflight linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

release preflight:

```bash
bash scripts/run-release-preflight.sh 3.6.0 linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

確認項目:

- [x] deterministic tests passed
- [x] package generation passed for `3.6.0-local.preflight`
- [x] package generation passed for `3.6.0`
- [x] local consumer smoke passed for `3.6.0-local.preflight`
- [x] local consumer smoke passed for `3.6.0`
- [x] local consumer smoke verifies `ExchangeApi.Optional.Testing`
- [x] release asset generation passed for `3.6.0-local.preflight`
- [x] release asset generation passed for `3.6.0`
- [x] release asset checksums generated for `3.6.0-local.preflight`
- [x] release asset checksums passed for `3.6.0`
- [x] live tests skip safely without opt-in
- [x] stdout / stderr / logs / evidence are secret-free

## 4. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.3.6.0-local.preflight.nupkg
ExchangeApi.Optional.Credentials.3.6.0-local.preflight.nupkg
ExchangeApi.Optional.Logging.3.6.0-local.preflight.nupkg
ExchangeApi.Optional.Testing.3.6.0-local.preflight.nupkg
ExchangeApi.Primitives.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.3.6.0.nupkg
ExchangeApi.Exchanges.Bitflyer.3.6.0.nupkg
ExchangeApi.Optional.Credentials.3.6.0.nupkg
ExchangeApi.Optional.Logging.3.6.0.nupkg
ExchangeApi.Optional.Testing.3.6.0.nupkg
ExchangeApi.Primitives.3.6.0.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Native.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.6.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.6.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.6.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.6.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.6.0.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.6.0.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.6.0.nupkg
ExchangeApi.Exchanges.Binance.Native.3.6.0.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.6.0.nupkg
```

## 5. GitHub Packages Verification

publish 後に実行する。

```bash
bash scripts/smoke-github-packages-consumer.sh 3.6.0
```

確認項目:

- [x] `ExchangeApi.Exchanges.Bitflyer 3.6.0` を restore / build / run できる
- [x] `ExchangeApi.Exchanges.Binance 3.6.0` を restore / build / run できる
- [x] `ExchangeApi.Primitives 3.6.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Credentials 3.6.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Logging 3.6.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Testing 3.6.0` を restore / build / run できる
- [x] token / secret が stdout / stderr に出ない

## 6. Release Gate

- [x] Git working tree が release 前に clean である
- [x] local preflight が通っている
- [x] release preflight が通っている
- [x] live tests が opt-in なしで skip する
- [x] `main` に `v3.6.0` commit が入っている
- [x] `v3.6.0` tag が remote にある
- [x] GitHub Release が作成されている
- [x] release assets が attach されている
- [x] GitHub Packages smoke が通っている
- [x] `v3.6.0` に Rx / lifecycle hardening / state reconstruction / simulation / Gateway / Platform / Strategy testing が含まれていない

local preflight result:

```text
date: 2026-04-29
diff check: git diff --check passed
release preflight: bash scripts/run-release-preflight.sh 3.6.0-local.preflight linux-x64 passed
build: dotnet build ExchangeApi.slnx passed
deterministic tests: dotnet test ExchangeApi.slnx --no-build passed
local pack: bash scripts/pack-local-nuget.sh 3.6.0-local.preflight passed
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 3.6.0-local.preflight passed
local consumer smoke coverage: ExchangeApi.Optional.Testing verified
release asset helper: bash scripts/create-release-assets.sh 3.6.0-local.preflight linux-x64 Release passed
release asset checksum: sha256sum -c *.sha256 passed in local/publish/release-assets/v3.6.0-local.preflight
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
safe live preflight: skipped without EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1
forbidden layer packages: none found for 3.6.0-local.preflight
packages:
  ExchangeApi.Exchanges.Binance.3.6.0-local.preflight.nupkg
  ExchangeApi.Exchanges.Bitflyer.3.6.0-local.preflight.nupkg
  ExchangeApi.Optional.Credentials.3.6.0-local.preflight.nupkg
  ExchangeApi.Optional.Logging.3.6.0-local.preflight.nupkg
  ExchangeApi.Optional.Testing.3.6.0-local.preflight.nupkg
  ExchangeApi.Primitives.3.6.0-local.preflight.nupkg
release assets:
  local/publish/release-assets/v3.6.0-local.preflight/exchangeapi-linux-x64
  local/publish/release-assets/v3.6.0-local.preflight/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v3.6.0-local.preflight/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v3.6.0-local.preflight/exchangeapi-mcp-linux-x64.sha256
```

release preflight result:

```text
date: 2026-04-29
diff check: git diff --check passed
release preflight: bash scripts/run-release-preflight.sh 3.6.0 linux-x64 passed
build: dotnet build ExchangeApi.slnx passed
deterministic tests: dotnet test ExchangeApi.slnx --no-build passed
local pack: bash scripts/pack-local-nuget.sh 3.6.0 passed
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 3.6.0 passed
local consumer smoke coverage: ExchangeApi.Optional.Testing verified
release asset helper: bash scripts/create-release-assets.sh 3.6.0 linux-x64 Release passed
release asset checksum: sha256sum -c *.sha256 passed in local/publish/release-assets/v3.6.0
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
safe live preflight: skipped without EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1
forbidden layer packages: none found for 3.6.0
packages:
  ExchangeApi.Exchanges.Binance.3.6.0.nupkg
  ExchangeApi.Exchanges.Bitflyer.3.6.0.nupkg
  ExchangeApi.Optional.Credentials.3.6.0.nupkg
  ExchangeApi.Optional.Logging.3.6.0.nupkg
  ExchangeApi.Optional.Testing.3.6.0.nupkg
  ExchangeApi.Primitives.3.6.0.nupkg
release assets:
  local/publish/release-assets/v3.6.0/exchangeapi-linux-x64
  local/publish/release-assets/v3.6.0/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v3.6.0/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v3.6.0/exchangeapi-mcp-linux-x64.sha256
```

release result:

```text
date: 2026-04-29
main push: passed
tag push: v3.6.0 passed
package publish: GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 3.6.0 passed
GitHub Packages consumer smoke: bash scripts/smoke-github-packages-consumer.sh 3.6.0 passed
GitHub Release: https://github.com/tkoba0410/ExchangeAPI/releases/tag/v3.6.0
packages:
  ExchangeApi.Exchanges.Binance.3.6.0.nupkg
  ExchangeApi.Exchanges.Bitflyer.3.6.0.nupkg
  ExchangeApi.Optional.Credentials.3.6.0.nupkg
  ExchangeApi.Optional.Logging.3.6.0.nupkg
  ExchangeApi.Optional.Testing.3.6.0.nupkg
  ExchangeApi.Primitives.3.6.0.nupkg
release assets:
  local/publish/release-assets/v3.6.0/exchangeapi-linux-x64
  local/publish/release-assets/v3.6.0/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v3.6.0/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v3.6.0/exchangeapi-mcp-linux-x64.sha256
```
