# ExchangeAPI v3.3.0 Release Checklist

最終更新: 2026-04-28
位置づけ: v3.3.0 release checklist

状態: `v3.3.0` released

release 完了日: 2026-04-28

完了済み:

- deterministic tests / local pack / local consumer smoke
- live tests safe skip without opt-in
- GitHub Packages publish: library / optional packages `3.3.0`
- GitHub Packages consumer smoke: `ExchangeApi.Exchanges.Bitflyer`, `ExchangeApi.Exchanges.Binance`, `ExchangeApi.Primitives`, `ExchangeApi.Optional.Credentials`, `ExchangeApi.Optional.Logging`
- tag: `v3.3.0`
- GitHub Release: `v3.3.0`
- release assets: `exchangeapi-linux-x64`, `exchangeapi-linux-x64.sha256`, `exchangeapi-mcp-linux-x64`, `exchangeapi-mcp-linux-x64.sha256`

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

- [x] deterministic tests passed for release version
- [x] package generation passed for `3.3.0`
- [x] local consumer smoke passed for `3.3.0`
- [x] local consumer smoke verifies private realtime surface
- [x] release asset generation passed for `3.3.0`
- [x] release asset checksums passed for `3.3.0`
- [x] live tests skip safely without opt-in
- [x] stdout / stderr / logs / evidence are secret-free

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

- [x] `ExchangeApi.Exchanges.Bitflyer 3.3.0` を restore / build / run できる
- [x] `ExchangeApi.Exchanges.Binance 3.3.0` を restore / build / run できる
- [x] `ExchangeApi.Primitives 3.3.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Credentials 3.3.0` を restore / build / run できる
- [x] `ExchangeApi.Optional.Logging 3.3.0` を restore / build / run できる
- [x] bitFlyer private realtime factory / channel vocabulary を参照できる
- [x] token / secret が stdout / stderr に出ない

## 6. Release Gate

- [x] Git working tree が release 前に clean である
- [x] `main` に `v3.3.0` commit が入っている
- [x] `v3.3.0` tag が remote にある
- [x] GitHub Release が作成されている
- [x] release assets が attach されている
- [x] GitHub Packages smoke が通っている
- [x] `v3.3.0` に reconnect / state builder / Rx / Binance realtime / Unified / state-changing operation が含まれていない

local preflight result:

```text
date: 2026-04-28
diff check: git diff --check passed
build: dotnet build ExchangeApi.slnx passed
deterministic tests: dotnet test ExchangeApi.slnx --no-restore passed
local pack: bash scripts/pack-local-nuget.sh 3.3.0 passed
local consumer smoke: bash scripts/smoke-local-nuget-consumer.sh 3.3.0 passed
release asset helper: bash scripts/create-release-assets.sh 3.3.0 linux-x64 Release passed
release asset checksum: sha256sum -c *.sha256 passed in local/publish/release-assets/v3.3.0
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
packages:
  ExchangeApi.Exchanges.Binance.3.3.0.nupkg
  ExchangeApi.Exchanges.Bitflyer.3.3.0.nupkg
  ExchangeApi.Optional.Credentials.3.3.0.nupkg
  ExchangeApi.Optional.Logging.3.3.0.nupkg
  ExchangeApi.Primitives.3.3.0.nupkg
release assets:
  local/publish/release-assets/v3.3.0/exchangeapi-linux-x64
  local/publish/release-assets/v3.3.0/exchangeapi-linux-x64.sha256
  local/publish/release-assets/v3.3.0/exchangeapi-mcp-linux-x64
  local/publish/release-assets/v3.3.0/exchangeapi-mcp-linux-x64.sha256
```

release result:

```text
date: 2026-04-28
main push: passed
tag push: v3.3.0 passed
package publish: GITHUB_TOKEN="$(gh auth token)" bash scripts/push-github-packages.sh 3.3.0 passed
GitHub Packages consumer smoke: bash scripts/smoke-github-packages-consumer.sh 3.3.0 passed
GitHub Release: https://github.com/tkoba0410/ExchangeAPI/releases/tag/v3.3.0
```
