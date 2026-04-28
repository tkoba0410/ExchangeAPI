# ExchangeAPI v3.7.0 Release Checklist

最終更新: 2026-04-29
位置づけ: v3.7.0 release checklist

状態: release preflight passed / release pending

## 1. Scope Confirmation

- [x] `v3.7.0` は Realtime Optional Reactive Integration release として扱っている
- [x] `ExchangeApi.Optional.Reactive` を追加している
- [x] public API は `ToObservable<T>(this IAsyncEnumerable<T> source)` のみに限定している
- [x] `ToObservable(...)` は thin generic adapter として実装している
- [x] `ToObservable(...)` は cold observable として動作する
- [x] subscription `Dispose()` で source enumeration を cancel する
- [x] normal completion は `OnCompleted`
- [x] source exception は `OnError`
- [x] dispose / cancellation は terminal notification を送らない
- [x] DTO-only item も stream envelope event も `T` としてそのまま `OnNext` する
- [x] adapter は stream item の意味を解釈しない
- [x] `System.Reactive` dependency は `ExchangeApi.Optional.Reactive` に限定している
- [x] core / venue / `Optional.Logging` / `Optional.Testing` は Rx に依存していない
- [x] `ExchangeApi.Optional.Reactive` は `Optional.Logging` / `Optional.Testing` に直接依存していない
- [x] `ExchangeApi.Optional.Reactive` は secret-neutral adapter として扱っている
- [x] venue-specific reactive helper は含めていない
- [x] envelope-specific helper は含めていない
- [x] scheduler overload は含めていない
- [x] buffering は含めていない
- [x] retry / reconnect / backoff は含めていない
- [x] stream health operator は含めていない
- [x] lifecycle / contract hardening は含めていない
- [x] state reconstruction は含めていない
- [x] simulation / Gateway / Platform behavior は含めていない
- [x] Binance realtime は含めていない
- [x] Unified は含めていない
- [x] state-changing operation は含めていない

## 2. Documentation

- [x] `docs/plan-v3.7.0.md` が scope / non-scope / implementation result / close preparation を固定している
- [x] `docs/roadmap-post-v2.md` が v3.7 を採用範囲として扱っている
- [x] `docs/distribution.md` が `ExchangeApi.Optional.Reactive` package を反映している
- [x] `docs/local-nuget-consumer.md` が local consumer smoke の `ExchangeApi.Optional.Reactive` 確認を反映している
- [x] `docs/guides/package-publish.md` が `ExchangeApi.Optional.Reactive` publish 対象を反映している
- [x] `docs/release-notes/v3.7.0.md` が追加されている

## 3. Verification

release-candidate preflight:

```bash
bash scripts/run-release-preflight.sh 3.7.0-local.preflight linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

release preflight:

```bash
bash scripts/run-release-preflight.sh 3.7.0 linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

確認項目:

- [x] deterministic tests passed
- [x] `ExchangeApi.Optional.Reactive.Tests` passed
- [x] package generation passed for `3.7.0-local.preflight`
- [x] package generation passed for `3.7.0`
- [x] local consumer smoke passed for `3.7.0-local.preflight`
- [x] local consumer smoke passed for `3.7.0`
- [x] local consumer smoke verifies `ExchangeApi.Optional.Reactive`
- [x] release asset generation passed for `3.7.0-local.preflight`
- [x] release asset generation passed for `3.7.0`
- [x] release asset checksums generated for `3.7.0-local.preflight`
- [x] release asset checksums passed for `3.7.0`
- [x] live tests skip safely without opt-in
- [x] stdout / stderr / logs / evidence are secret-free
- [x] `System.Reactive` dependency is limited to `ExchangeApi.Optional.Reactive`

## 4. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.3.7.0-local.preflight.nupkg
ExchangeApi.Optional.Credentials.3.7.0-local.preflight.nupkg
ExchangeApi.Optional.Logging.3.7.0-local.preflight.nupkg
ExchangeApi.Optional.Reactive.3.7.0-local.preflight.nupkg
ExchangeApi.Optional.Testing.3.7.0-local.preflight.nupkg
ExchangeApi.Primitives.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.3.7.0.nupkg
ExchangeApi.Exchanges.Bitflyer.3.7.0.nupkg
ExchangeApi.Optional.Credentials.3.7.0.nupkg
ExchangeApi.Optional.Logging.3.7.0.nupkg
ExchangeApi.Optional.Reactive.3.7.0.nupkg
ExchangeApi.Optional.Testing.3.7.0.nupkg
ExchangeApi.Primitives.3.7.0.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Native.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.7.0-local.preflight.nupkg
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.7.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.7.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.7.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.7.0.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.7.0.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.7.0.nupkg
ExchangeApi.Exchanges.Binance.Native.3.7.0.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.7.0.nupkg
```

## 5. GitHub Packages Verification

publish 後に実行する。

```bash
bash scripts/smoke-github-packages-consumer.sh 3.7.0
```

確認項目:

- [ ] `ExchangeApi.Exchanges.Bitflyer 3.7.0` を restore / build / run できる
- [ ] `ExchangeApi.Exchanges.Binance 3.7.0` を restore / build / run できる
- [ ] `ExchangeApi.Primitives 3.7.0` を restore / build / run できる
- [ ] `ExchangeApi.Optional.Credentials 3.7.0` を restore / build / run できる
- [ ] `ExchangeApi.Optional.Logging 3.7.0` を restore / build / run できる
- [ ] `ExchangeApi.Optional.Reactive 3.7.0` を restore / build / run できる
- [ ] `ExchangeApi.Optional.Testing 3.7.0` を restore / build / run できる
- [ ] token / secret が stdout / stderr に出ない

## 6. Release Gate

- [ ] Git working tree が release 前に clean である
- [x] local preflight が通っている
- [x] release preflight が通っている
- [x] live tests が opt-in なしで skip する
- [ ] `main` に `v3.7.0` commit が入っている
- [ ] `v3.7.0` tag が remote にある
- [ ] GitHub Release が作成されている
- [ ] release assets が attach されている
- [ ] GitHub Packages smoke が通っている
- [ ] `v3.7.0` に scheduler / buffer / retry / reconnect / lifecycle hardening / state reconstruction / simulation / Gateway / Platform behavior が含まれていない

local preflight result:

```text
2026-04-29 passed

commands:
- bash scripts/run-release-preflight.sh 3.7.0-local.preflight linux-x64
- dotnet test ExchangeApi.LiveTests.slnx --no-restore
- git diff --check

package output:
- ExchangeApi.Exchanges.Binance.3.7.0-local.preflight.nupkg
- ExchangeApi.Exchanges.Bitflyer.3.7.0-local.preflight.nupkg
- ExchangeApi.Optional.Credentials.3.7.0-local.preflight.nupkg
- ExchangeApi.Optional.Logging.3.7.0-local.preflight.nupkg
- ExchangeApi.Optional.Reactive.3.7.0-local.preflight.nupkg
- ExchangeApi.Optional.Testing.3.7.0-local.preflight.nupkg
- ExchangeApi.Primitives.3.7.0-local.preflight.nupkg

release assets:
- local/publish/release-assets/v3.7.0-local.preflight/exchangeapi-linux-x64
- local/publish/release-assets/v3.7.0-local.preflight/exchangeapi-linux-x64.sha256
- local/publish/release-assets/v3.7.0-local.preflight/exchangeapi-mcp-linux-x64
- local/publish/release-assets/v3.7.0-local.preflight/exchangeapi-mcp-linux-x64.sha256

checksum verification:
- exchangeapi-linux-x64: OK
- exchangeapi-mcp-linux-x64: OK

forbidden layer package check:
- no layer-specific venue package matched v3.7.0-local.preflight
```

release preflight result:

```text
2026-04-29 passed

commands:
- bash scripts/run-release-preflight.sh 3.7.0 linux-x64
- dotnet test ExchangeApi.LiveTests.slnx --no-restore
- git diff --check

package output:
- ExchangeApi.Exchanges.Binance.3.7.0.nupkg
- ExchangeApi.Exchanges.Bitflyer.3.7.0.nupkg
- ExchangeApi.Optional.Credentials.3.7.0.nupkg
- ExchangeApi.Optional.Logging.3.7.0.nupkg
- ExchangeApi.Optional.Reactive.3.7.0.nupkg
- ExchangeApi.Optional.Testing.3.7.0.nupkg
- ExchangeApi.Primitives.3.7.0.nupkg

release assets:
- local/publish/release-assets/v3.7.0/exchangeapi-linux-x64
- local/publish/release-assets/v3.7.0/exchangeapi-linux-x64.sha256
- local/publish/release-assets/v3.7.0/exchangeapi-mcp-linux-x64
- local/publish/release-assets/v3.7.0/exchangeapi-mcp-linux-x64.sha256

checksum verification:
- exchangeapi-linux-x64: OK
- exchangeapi-mcp-linux-x64: OK

forbidden layer package check:
- no layer-specific venue package matched v3.7.0
```

release result:

```text
pending
```
