# ExchangeAPI v3.3.0 Release Checklist

最終更新: 2026-04-28
位置づけ: v3.3.0 release checklist

状態: release candidate

## 1. Scope Confirmation

- [x] `v3.3.0` は bitFlyer private realtime read MVP release として扱っている
- [x] private realtime は public realtime と別 client として公開している
- [x] 対象 private channel は `child_order_events` / `parent_order_events`
- [x] realtime `auth` request shape を deterministic tests で固定している
- [x] private event DTO decode を deterministic tests で固定している
- [x] API secret は public API に出していない
- [x] signing は `IApiCredentialSession.Sign(payload)` を使っている
- [x] state-changing operation は含めていない
- [x] Binance realtime は含めていない
- [x] `Unified` 実装は含めていない
- [x] reconnect / backoff / resubscribe の本格実装は含めていない
- [x] full order book / order state builder は含めていない
- [x] `System.Reactive` dependency は含めていない
- [x] `IObservable<T>` public API は含めていない

## 2. Documentation

- [x] `docs/plan-v3.3.0.md` が scope / non-scope / verification / release close instructions を固定している
- [x] `docs/realtime-bitflyer.md` が private realtime auth / channel / DTO 方針を含む
- [x] `docs/guides/realtime-bitflyer-getting-started.md` が private realtime read の導線を含む
- [x] `verification/bitflyer-private-realtime-live.md` がある
- [x] `docs/roadmap-post-v2.md` が v3 realtime maturity track と optional reactive 方針を含む
- [x] `docs/release-notes/v3.3.0.md` が追加されている

## 3. Verification

release preflight:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.3.0
bash scripts/smoke-local-nuget-consumer.sh 3.3.0
bash scripts/create-release-assets.sh 3.3.0 linux-x64 Release
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

確認項目:

- [ ] deterministic tests passed for release version
- [ ] package generation passed for `3.3.0`
- [ ] local consumer smoke passed for `3.3.0`
- [ ] local consumer smoke verifies private realtime surface
- [ ] release asset generation passed for `3.3.0`
- [ ] release asset checksums passed for `3.3.0`
- [ ] live tests skip safely without opt-in
- [ ] stdout / stderr / logs / evidence are secret-free

## 4. Package Expectations

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.3.0.nupkg
ExchangeApi.Exchanges.Bitflyer.3.3.0.nupkg
ExchangeApi.Optional.Credentials.3.3.0.nupkg
ExchangeApi.Optional.Logging.3.3.0.nupkg
ExchangeApi.Primitives.3.3.0.nupkg
```

生成されてはいけない package:

```text
ExchangeApi.Exchanges.Bitflyer.Vocabulary.3.3.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Protocol.3.3.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Native.3.3.0.nupkg
ExchangeApi.Exchanges.Bitflyer.Composition.3.3.0.nupkg
ExchangeApi.Exchanges.Binance.Vocabulary.3.3.0.nupkg
ExchangeApi.Exchanges.Binance.Protocol.3.3.0.nupkg
ExchangeApi.Exchanges.Binance.Native.3.3.0.nupkg
ExchangeApi.Exchanges.Binance.Composition.3.3.0.nupkg
```

## 5. GitHub Packages Verification

publish 後に実行する。

```bash
bash scripts/smoke-github-packages-consumer.sh 3.3.0
```

確認項目:

- [ ] `ExchangeApi.Exchanges.Bitflyer 3.3.0` を restore / build / run できる
- [ ] `ExchangeApi.Exchanges.Binance 3.3.0` を restore / build / run できる
- [ ] `ExchangeApi.Primitives 3.3.0` を restore / build / run できる
- [ ] `ExchangeApi.Optional.Credentials 3.3.0` を restore / build / run できる
- [ ] `ExchangeApi.Optional.Logging 3.3.0` を restore / build / run できる
- [ ] bitFlyer private realtime factory / channel vocabulary を参照できる
- [ ] token / secret が stdout / stderr に出ない

## 6. Release Gate

- [ ] Git working tree が release 前に clean である
- [ ] `main` に `v3.3.0` commit が入っている
- [ ] `v3.3.0` tag が remote にある
- [ ] GitHub Release が作成されている
- [ ] release assets が attach されている
- [ ] GitHub Packages smoke が通っている
- [ ] `v3.3.0` に reconnect / state builder / Rx / Binance realtime / Unified / state-changing operation が含まれていない
