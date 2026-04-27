# ExchangeAPI v3.4.0 Release Checklist

最終更新: 2026-04-28
位置づけ: v3.4.0 release checklist

状態: `v3.4.0` released

release 完了日: 2026-04-28

完了済み:

- deterministic tests / local pack / local consumer smoke
- live tests safe skip without opt-in
- GitHub Packages publish: library / optional packages `3.4.0`
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Bitflyer`, `ExchangeApi.Exchanges.Binance`, `ExchangeApi.Primitives`, `ExchangeApi.Optional.Credentials`, `ExchangeApi.Optional.Logging`
- tag: `v3.4.0`
- GitHub Release: `v3.4.0`
- release assets: `exchangeapi-linux-x64`, `exchangeapi-linux-x64.sha256`, `exchangeapi-mcp-linux-x64`, `exchangeapi-mcp-linux-x64.sha256`

## 1. Scope Confirmation

- [x] `v3.4.0` は bitFlyer realtime resilience foundation release として扱っている
- [x] `Subscribe*StreamAsync` envelope API を追加している
- [x] 既存 DTO-only `Subscribe*Async` API は維持している
- [x] DTO-only API は lifecycle event を混ぜない
- [x] envelope API は reconnect / resubscribe lifecycle event を返す
- [x] public reconnect order は `Reconnecting -> Reconnected -> Resubscribed -> ContinuityLost -> Data...`
- [x] private reconnect order は `Reconnecting -> Reconnected -> AuthenticationReplayed -> Resubscribed -> ContinuityLost -> Data...`
- [x] `MessageRejected` は decode failure を通知し、raw payload / secret を持たない
- [x] `BitflyerRealtimeException.Kind` を restart 判断に使える
- [x] `MaxAttempts = 0` は reconnect disabled として扱う
- [x] idle timeout は default disabled で、指定時だけ reconnect target になる
- [x] state-changing operation は含めていない
- [x] Binance realtime は含めていない
- [x] `Unified` 実装は含めていない
- [x] `System.Reactive` dependency は含めていない
- [x] `IObservable<T>` public API は含めていない
- [x] board / private order state builder は含めていない

## 2. Documentation

- [x] `docs/plan-v3.4.0.md` が scope / non-scope / verification を固定している
- [x] `docs/realtime-bitflyer.md` が stream envelope / reconnect / idle timeout contract を含む
- [x] `docs/guides/realtime-bitflyer-getting-started.md` が stream envelope 利用例を含む
- [x] `docs/verification.md` が v3.4 resilience verification を含む
- [x] `docs/local-nuget-consumer.md` が v3.4 consumer smoke 導線を含む
- [x] `docs/guides/package-publish.md` が v3.4 publish 導線を含む
- [x] `verification/bitflyer-realtime-resilience.md` が opt-in resilience runbook としてある
- [x] `docs/release-notes/v3.4.0.md` が追加されている

## 3. Verification

release preflight:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.4.0
bash scripts/smoke-local-nuget-consumer.sh 3.4.0
bash scripts/create-release-assets.sh 3.4.0 linux-x64 Release
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

local release-candidate preflight:

```bash
bash scripts/run-release-preflight.sh 3.4.0-local.preflight linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- [x] deterministic tests passed for local release-candidate version
- [x] deterministic tests passed for release version
- [x] package generation passed for `3.4.0-local.preflight`
- [x] package generation passed for `3.4.0`
- [x] local consumer smoke passed for `3.4.0-local.preflight`
- [x] local consumer smoke passed for `3.4.0`
- [x] local consumer smoke verifies stream envelope / realtime options surface
- [x] release asset generation passed for `3.4.0-local.preflight`
- [x] release asset generation passed for `3.4.0`
- [x] release asset checksums passed for `3.4.0-local.preflight`
- [x] release asset checksums passed for `3.4.0`
- [x] live tests skip safely without opt-in
- [x] stdout / stderr / logs / evidence are secret-free

## 4. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.4.0.nupkg
ExchangeApi.Exchanges.Bitflyer.3.4.0.nupkg
ExchangeApi.Optional.Credentials.3.4.0.nupkg
ExchangeApi.Optional.Logging.3.4.0.nupkg
ExchangeApi.Primitives.3.4.0.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.4.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.4.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.4.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.4.0.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.4.0.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.4.0.nupkg
ExchangeApi.Exchanges.Binance.Native.3.4.0.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.4.0.nupkg
```

## 5. GitHub Packages Verification

publish 後に実行する。

```bash
bash scripts/smoke-github-packages-consumer.sh 3.4.0
```

確認項目:

- [x] `ExchangeApi.Exchanges.Bitflyer 3.4.0` を restore / build / run できる
- [x] `ExchangeApi.Exchanges.Binance 3.4.0` を restore / build / run できる
- [x] `ExchangeApi.Primitives 3.4.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Credentials 3.4.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Logging 3.4.0` を restore / build / run できる
- [x] bitFlyer stream envelope / realtime options surface を参照できる
- [x] token / secret が stdout / stderr に出ない

## 6. Release Gate

- [x] Git working tree が release 前に clean である
- [x] `main` に `v3.4.0` commit が入っている
- [x] `v3.4.0` tag が remote にある
- [x] GitHub Release が作成されている
- [x] release assets が attach されている
- [x] GitHub Packages smoke が通っている
- [x] `v3.4.0` に state builder / Rx / Binance realtime / Unified / state-changing operation が含まれていない

local preflight result:

```text
date: 2026-04-28
diff check: git diff --check passed
release preflight: bash scripts/run-release-preflight.sh 3.4.0-local.preflight linux-x64 passed
build: dotnet build ExchangeApi.slnx passed
deterministic tests: dotnet test ExchangeApi.slnx --no-build passed
local pack: bash scripts/pack-local-nuget.sh 3.4.0-local.preflight passed
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 3.4.0-local.preflight passed
release asset helper: bash scripts/create-release-assets.sh 3.4.0-local.preflight linux-x64 Release passed
release asset checksum: sha256sum -c *.sha256 passed in local/publish/release-assets/v3.4.0-local.preflight
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
safe live preflight: skipped without EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1
packages:
  ExchangeApi.Exchanges.Binance.3.4.0-local.preflight.nupkg
  ExchangeApi.Exchanges.Bitflyer.3.4.0-local.preflight.nupkg
  ExchangeApi.Optional.Credentials.3.4.0-local.preflight.nupkg
  ExchangeApi.Optional.Logging.3.4.0-local.preflight.nupkg
  ExchangeApi.Primitives.3.4.0-local.preflight.nupkg
release assets:
  local/publish/release-assets/v3.4.0-local.preflight/exchangeapi-linux-x64
  local/publish/release-assets/v3.4.0-local.preflight/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v3.4.0-local.preflight/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v3.4.0-local.preflight/exchangeapi-mcp-linux-x64.sha256
```

release result:

```text
date: 2026-04-28
main push: passed
tag push: v3.4.0 passed
package publish: GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 3.4.0 passed
GitHub Packages consumer smoke: bash scripts/smoke-github-packages-consumer.sh 3.4.0 passed
GitHub Release: https://github.com/tkoba0410/ExchangeAPI/releases/tag/v3.4.0
```
