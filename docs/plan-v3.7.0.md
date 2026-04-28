# v3.7.0 Realtime Optional Reactive Integration 実施指示

最終更新: 2026-04-29
位置づけ: v3.7.0 Realtime Optional Reactive Integration 実施指示

状態: close preparation

## 1. 目的

v3.7.0 は、`ExchangeApi.Optional.Reactive` を追加する release とする。

`ExchangeApi.Optional.Reactive` は、Rx 標準 semantics に従う最小の generic bridge として扱う。
主 API である `IAsyncEnumerable<T>` based realtime API は変更しない。
Rx 利用者向けに `IAsyncEnumerable<T>` から `IObservable<T>` への薄い adapter を提供する。

## 2. 採用方針

採用:

- Thin Generic Adapter
- API 名は `ToObservable(...)`
- public API は `ToObservable<T>(this IAsyncEnumerable<T> source)` のみにする
- `ToObservable(...)` が返す observable は cold observable とする
- subscription ごとに source を再 enumeration する
- subscription `Dispose()` で source enumeration を cancel する
- normal completion は `OnCompleted`
- source exception は `OnError`
- dispose / cancellation は terminal notification を送らない
- DTO-only item も stream envelope event も `T` としてそのまま `OnNext` する
- adapter は stream item の意味を解釈しない
- buffer / scheduler / concurrency policy は持たない
- hot / shared stream、scheduler、buffer、throttle は Rx 標準 operator に委ねる
- `System.Reactive` dependency は `ExchangeApi.Optional.Reactive` のみに置く
- `ExchangeApi.Optional.Reactive` は他 optional package に直接依存しない
- `ExchangeApi.Optional.Reactive` は secret-neutral adapter とする

非対象:

- venue-specific reactive helper
- envelope-specific helper
- `WhereData` / `WhereDiagnostic`
- options 型
- scheduler overload
- buffering
- retry / reconnect / backoff
- stream health operator
- `ExchangeApi.Optional.Testing` との直接依存
- `ExchangeApi.Optional.Logging` との直接依存
- Gateway / Platform behavior
- simulation

## 3. Rx semantics 方針

`ExchangeApi.Optional.Reactive` は、原則として Rx 標準 semantics に従う。
Rx 標準から外れる挙動は、ExchangeAPI 固有の安全性、再利用性、責務境界の面で明確なメリットがある場合だけ採用する。

v3.7.0 では Rx 標準から外れる独自 policy は追加しない。

## 4. 秘匿性方針

`ExchangeApi.Optional.Reactive` は secret-neutral adapter とする。

実施しない:

- stream item を inspect しない
- stream item を serialize しない
- `ToString()` を呼ばない
- log / stdout / stderr に出さない
- evidence / artifact を作らない
- redaction しない
- exception message を加工しない
- credential / token / signature を扱わない

`Optional.Reactive` 自体は secret を新たに生成、保存、出力しない。
secret-free guarantee は source contract と利用者側の logging / observer 実装に依存する。

## 5. 実装指示

追加:

- `src/Optional/Reactive/ExchangeApi.Optional.Reactive.csproj`
- `src/Optional/Reactive/AsyncEnumerableReactiveExtensions.cs`
- `tests/Optional/Reactive.Tests/ExchangeApi.Optional.Reactive.Tests.csproj`
- `tests/Optional/Reactive.Tests/AsyncEnumerableReactiveExtensionsTests.cs`

更新:

- `ExchangeApi.slnx`
- `scripts/smoke-local-nuget-consumer.sh`
- `scripts/smoke-github-packages-consumer.sh`
- `docs/document-inventory.md`
- `docs/roadmap-post-v2.md`
- `docs/distribution.md`
- `docs/local-nuget-consumer.md`
- `docs/guides/package-publish.md`

実装方針:

- namespace は `ExchangeApi.Optional.Reactive`
- package / project 名は `ExchangeApi.Optional.Reactive`
- `System.Reactive` package version は明示固定する
- core / venue / `Optional.Logging` / `Optional.Testing` には Rx dependency を追加しない
- implementation helper は internal に留める

## 6. Verification

deterministic tests:

- source items are forwarded as `OnNext`
- source normal completion becomes `OnCompleted`
- source exception becomes `OnError`
- subscription dispose cancels enumeration
- dispose sends no terminal notification
- multiple subscriptions re-enumerate source independently
- envelope event remains `OnNext`
- null source throws `ArgumentNullException`
- adapter does not call item `ToString()`

最低限:

```bash
dotnet build ExchangeApi.slnx
dotnet test tests/Optional/Reactive.Tests/ExchangeApi.Optional.Reactive.Tests.csproj --no-restore
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.7.0-local.reactive
bash scripts/smoke-local-nuget-consumer.sh 3.7.0-local.reactive
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

package 期待:

```text
ExchangeApi.Exchanges.Binance.3.7.0-local.reactive.nupkg
ExchangeApi.Exchanges.Bitflyer.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Credentials.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Logging.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Reactive.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Testing.3.7.0-local.reactive.nupkg
ExchangeApi.Primitives.3.7.0-local.reactive.nupkg
```

## 7. 完了条件

- `docs/plan-v3.7.0.md` が scope / non-scope / verification を固定している
- `ExchangeApi.Optional.Reactive` project が追加されている
- `ExchangeApi.Optional.Reactive` は `System.Reactive` のみに外部 Rx dependency を持つ
- core / venue / `Optional.Logging` / `Optional.Testing` は Rx に依存していない
- public API は `ToObservable<T>(this IAsyncEnumerable<T> source)` のみに限定されている
- adapter は cold observable として動作する
- dispose が enumeration cancellation に接続されている
- normal completion / source exception / cancellation semantics が deterministic tests で固定されている
- stream item の意味を解釈しない
- secret-neutral 方針が docs / tests に反映されている
- local consumer smoke が `ExchangeApi.Optional.Reactive` を確認している
- package generation に `ExchangeApi.Optional.Reactive` が含まれる
- live tests は opt-in なしで skip する

## 8. Implementation Result

実装済み:

- `ExchangeApi.Optional.Reactive` project 追加
- `AsyncEnumerableReactiveExtensions.ToObservable<T>(...)` 追加
- `System.Reactive` dependency を `ExchangeApi.Optional.Reactive` のみに追加
- `ToObservable(...)` contract tests 追加
- local / GitHub Packages consumer smoke に `ExchangeApi.Optional.Reactive` restore / build / run 確認を追加
- distribution / local consumer / package publish docs に `ExchangeApi.Optional.Reactive` を反映

verification:

```text
dotnet build ExchangeApi.slnx passed
dotnet test tests/Optional/Reactive.Tests/ExchangeApi.Optional.Reactive.Tests.csproj --no-restore passed
dotnet test ExchangeApi.slnx --no-restore passed
bash scripts/pack-local-nuget.sh 3.7.0-local.reactive passed
bash scripts/smoke-local-nuget-consumer.sh 3.7.0-local.reactive passed
dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
git diff --check passed
```

dependency review:

```text
System.Reactive dependency: src/Optional/Reactive/ExchangeApi.Optional.Reactive.csproj only
Optional.Reactive project reference from tests only
core / venue / Optional.Logging / Optional.Testing do not reference System.Reactive
```

generated packages:

```text
ExchangeApi.Exchanges.Binance.3.7.0-local.reactive.nupkg
ExchangeApi.Exchanges.Bitflyer.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Credentials.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Logging.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Reactive.3.7.0-local.reactive.nupkg
ExchangeApi.Optional.Testing.3.7.0-local.reactive.nupkg
ExchangeApi.Primitives.3.7.0-local.reactive.nupkg
```

## 9. Release Close Preparation 指示

目的:

v3.7.0 を Realtime Optional Reactive Integration release として閉じる。
v3.7.0 には `ExchangeApi.Optional.Reactive` と thin generic `ToObservable(...)` adapter を含める。
v3.8.0 以降の lifecycle / contract hardening、release close 整理は含めない。

実施する:

- `docs/release-checklist-v3.7.0.md` を追加する
- `docs/release-notes/v3.7.0.md` を追加する
- `docs/document-inventory.md` に v3.7 release checklist / release notes を登録する
- release-candidate preflight を実行する
- live tests が opt-in なしで skip することを確認する
- package generation に `ExchangeApi.Optional.Reactive` が含まれることを確認する
- `System.Reactive` dependency が `ExchangeApi.Optional.Reactive` に閉じていることを確認する
- stdout / stderr / logs / evidence に secret が残らないことを確認する

実施しない:

- venue-specific reactive helper
- envelope-specific helper
- scheduler overload
- buffering
- retry / reconnect / backoff
- stream health operator
- lifecycle / contract hardening
- state reconstruction
- simulation / Gateway / Platform behavior
- Binance realtime
- Unified
- state-changing operation

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

release 完了後に実施する:

- `main` に merge する
- `v3.7.0` tag を作成 / push する
- `bash scripts/push-github-packages.sh 3.7.0` を実行する
- `bash scripts/smoke-github-packages-consumer.sh 3.7.0` を実行する
- GitHub Release `v3.7.0` を作成する
- release checklist に release result を反映する
- v3.8.0 preparation に進む

完了条件:

- `docs/release-checklist-v3.7.0.md` が preflight / release gate を持っている
- `docs/release-notes/v3.7.0.md` が利用者向け変更点を説明している
- release-candidate preflight が通っている
- release preflight が通っている
- live tests が opt-in なしで skip する
- GitHub Packages smoke が `ExchangeApi.Optional.Reactive` を確認している
- v3.7.0 に scheduler / buffer / retry / reconnect / lifecycle hardening / state reconstruction / simulation / Gateway / Platform behavior が含まれていない
