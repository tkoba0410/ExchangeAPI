# ExchangeAPI v3.5.0 Release Checklist

最終更新: 2026-04-28
位置づけ: v3.5.0 release checklist

状態: local preflight passed

## 1. Scope Confirmation

- [x] `v3.5.0` は Realtime Diagnostics Foundation release として扱っている
- [x] `RealtimeDiagnosticEvent` public contract を追加している
- [x] `RealtimeDiagnosticEventTypes` / `RealtimeDiagnosticSeverities` constants を追加している
- [x] `BitflyerRealtimeDiagnostic<T>` envelope event を追加している
- [x] 外部取引所データと内部診断データは同じ envelope stream 上の別種 event として扱う
- [x] DTO-only `Subscribe*Async` API は維持している
- [x] HTTP 側 public API / 実装は変更していない
- [x] `ExchangeApi.Optional.Logging` に realtime raw frame log record helper を追加している
- [x] raw frame body logging は default disabled
- [x] raw frame body は opt-in + per-frame size limit
- [x] limit 超過 body は truncate しない
- [x] `replay` 実装は含めていない
- [x] `System.Reactive` dependency は含めていない
- [x] `IObservable<T>` public API は含めていない
- [x] state builder / state projection は含めていない
- [x] Binance realtime は含めていない
- [x] Unified は含めていない
- [x] state-changing operation は含めていない

## 2. Documentation

- [x] `docs/plan-v3.5.0.md` が scope / non-scope / implementation result を固定している
- [x] `docs/realtime-diagnostics.md` が Realtime diagnostics 設計正本である
- [x] `docs/realtime-bitflyer.md` が v3.5 diagnostic event と envelope stream 方針を参照している
- [x] `docs/spec.md` が HTTP / Realtime observability responsibility split を含む
- [x] `docs/roadmap-post-v2.md` が v3.5 を採用範囲として扱っている
- [x] `docs/release-notes/v3.5.0.md` が追加されている

## 3. Verification

local release-candidate preflight:

```bash
bash scripts/run-release-preflight.sh 3.5.0-local.preflight linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- [x] deterministic tests passed
- [x] package generation passed for `3.5.0-local.preflight`
- [x] local consumer smoke passed for `3.5.0-local.preflight`
- [x] local consumer smoke verifies realtime diagnostics surface
- [x] release asset generation passed for `3.5.0-local.preflight`
- [x] release asset checksums generated for `3.5.0-local.preflight`
- [x] live tests skip safely without opt-in
- [x] stdout / stderr / logs / evidence are secret-free

## 4. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.3.5.0-local.preflight.nupkg
ExchangeApi.Optional.Credentials.3.5.0-local.preflight.nupkg
ExchangeApi.Optional.Logging.3.5.0-local.preflight.nupkg
ExchangeApi.Primitives.3.5.0-local.preflight.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Native.3.5.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.5.0-local.preflight.nupkg
```

## 5. Release Gate

- [ ] Git working tree が release 前に clean である
- [x] local preflight が通っている
- [x] live tests が opt-in なしで skip する
- [x] `v3.5.0` に replay / Rx / state management / Binance realtime / Unified / state-changing operation が含まれていない

local preflight result:

```text
date: 2026-04-28
diff check: git diff --check passed
release preflight: bash scripts/run-release-preflight.sh 3.5.0-local.preflight linux-x64 passed
build: dotnet build ExchangeApi.slnx passed
deterministic tests: dotnet test ExchangeApi.slnx --no-build passed
local pack: bash scripts/pack-local-nuget.sh 3.5.0-local.preflight passed
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 3.5.0-local.preflight passed
local consumer smoke coverage: realtime diagnostics surface verified
release asset helper: bash scripts/create-release-assets.sh 3.5.0-local.preflight linux-x64 Release passed
release asset checksum: sha256sum -c *.sha256 passed in local/publish/release-assets/v3.5.0-local.preflight
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
safe live preflight: skipped without EXCHANGEAPI_RUN_SAFE_LIVE_PREFLIGHT=1
packages:
  ExchangeApi.Exchanges.Binance.3.5.0-local.preflight.nupkg
  ExchangeApi.Exchanges.Bitflyer.3.5.0-local.preflight.nupkg
  ExchangeApi.Optional.Credentials.3.5.0-local.preflight.nupkg
  ExchangeApi.Optional.Logging.3.5.0-local.preflight.nupkg
  ExchangeApi.Primitives.3.5.0-local.preflight.nupkg
release assets:
  local/publish/release-assets/v3.5.0-local.preflight/exchangeapi-linux-x64
  local/publish/release-assets/v3.5.0-local.preflight/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v3.5.0-local.preflight/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v3.5.0-local.preflight/exchangeapi-mcp-linux-x64.sha256
```
