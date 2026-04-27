# v3.4.0 bitFlyer Realtime Resilience Foundation 実施指示

最終更新: 2026-04-28
位置づけ: v3.4.0 実施指示

状態: released

## 1. 目的

v3.4.0 では、v3.1.0 から v3.3.0 で追加した bitFlyer Realtime API の public / private read surface を前提に、connection lifecycle の再現性と利用時の堅牢性を上げる。

主題は bitFlyer Realtime resilience + stream envelope foundation とする。
具体的には lifecycle-aware stream envelope API、reconnect / backoff / resubscribe / private auth 再実行 / idle timeout の設計と最小実装を扱う。

v3.4.0 は v3 系の Realtime maturity track の一部であり、新 venue、Unified、state-changing operation は扱わない。

## 2. 採用候補

採用候補:

- realtime reconnect policy
- lifecycle-aware stream envelope API
- bounded backoff policy
- reconnect 後の public channel resubscribe
- reconnect 後の private `auth` 再実行
- reconnect 後の private channel resubscribe
- idle timeout / no-message timeout の controlled error
- cancellation / dispose / remote close の lifecycle contract 整理
- deterministic fake transport tests
- secret-free diagnostics / error message rule
- live verification runbook の resilience 観点追記

API 方針候補:

- 主 API は `IAsyncEnumerable<T>` のまま維持する
- public / private realtime client は分けたまま維持する
- 既存 DTO-only API は維持する
- DTO-only API は reconnect disabled のまま維持する
- envelope API は `Subscribe*StreamAsync` suffix を使う
- envelope API は reconnect default enabled とする
- reconnect による missed message replay は保証しない
- private auth 再実行でも API secret / signature を log / exception / result に出さない

候補 option:

```csharp
public sealed class BitflyerRealtimeClientOptions
{
    public Uri EndpointUri { get; init; }
    public TimeSpan? ConnectTimeout { get; init; }
    public BitflyerRealtimeReconnectOptions Reconnect { get; init; }
    public TimeSpan? IdleTimeout { get; init; }
}
```

```csharp
public sealed class BitflyerRealtimeReconnectOptions
{
    public int MaxAttempts { get; init; }
    public TimeSpan InitialDelay { get; init; }
    public TimeSpan MaxDelay { get; init; }
}
```

上記の exact API は実装前に `docs/realtime-bitflyer.md` で固定する。

stream envelope event:

```text
Data
Reconnecting
Reconnected
AuthenticationReplayed
Resubscribed
ContinuityLost
MessageRejected
```

## 3. 非対象

v3.4.0 では次を扱わない。

- order / cancel / deposit / withdraw など state-changing operation
- HTTP endpoint contract 変更
- Binance realtime
- new venue implementation
- Unified realtime abstraction
- venue 横断 realtime abstraction
- full order book state builder
- private order event state builder
- `ExchangeApi.Optional.Reactive`
- `System.Reactive` dependency の core / venue package 追加
- `IObservable<T>` public API
- `ExchangeApi.Optional.Realtime.State`
- `StreamError` event
- credentials provider 拡張
- CLI / MCP の本格 integration
- exactly-once delivery guarantee
- missed message replay guarantee
- private realtime live verification の default 実行

## 4. 必須裁定

実装前に次を裁定する。

- reconnect を default enabled にするか opt-in にするか
- reconnect attempt の上限
- backoff delay の計算式
- cancellation と reconnect の優先順位
- remote close / transport exception / idle timeout の扱い
- subscribe request 送信済み channel の tracking 方法
- private reconnect 時の auth 再実行順序
- auth failure 時に retry するか即時終了するか
- board delta stream で reconnect 後に continuity を保証しないことの文書化
- error / diagnostic message の secret-free rule

裁定済み:

- stream envelope API を採用する
- 既存 DTO-only API は維持し、obsolete にしない
- envelope API 名は `Subscribe*StreamAsync` とする
- DTO-only API は reconnect disabled のまま維持する
- envelope API は reconnect default enabled とする
- envelope event 型は具象型とする
- event 型は `Data`, `Reconnecting`, `Reconnected`, `AuthenticationReplayed`, `Resubscribed`, `ContinuityLost`, `MessageRejected`
- `StreamError` は採用しない
- envelope API では malformed / decode failed message を `MessageRejected` event として通知して stream を継続する
- DTO-only API では malformed / decode error を controlled exception として扱い stream を終了する
- fatal error 後の restart は利用者判断とする
- reconnect 対象は remote close / transport exception / idle timeout とする
- auth failure / resubscribe failure / reconnect exhausted / unrecoverable transport failure は controlled exception とする
- non-target channel message は ignore する
- cancellation は常に reconnect より優先する
- private reconnect order は `Reconnecting -> Reconnected -> AuthenticationReplayed -> Resubscribed -> ContinuityLost -> Data...` とする
- board delta の gap-free continuity は保証しない
- backoff default は `MaxAttempts = 3`, `InitialDelay = 1s`, `MaxDelay = 10s`, jitter なしとする
- backoff values は options で変更可能とし、docs に conservative / interactive / long-running / no reconnect preset を示す
- `MaxAttempts = 0` は reconnect disabled とする
- idle timeout は default disabled とし、`TimeSpan? IdleTimeout` で変更可能とする
- idle timeout docs に disabled / interactive / monitoring / aggressive preset を示す
- idle timeout 発生後の reconnect では `ContinuityLost` を通知する
- `BitflyerRealtimeErrorKind` enum を追加し、`BitflyerRealtimeException` に `Kind` property を持たせる
- exception message は secret-free human-readable explanation に限定する
- Rx は v3.4.0 に含めない
- state builder は v3.5.0 以降候補に残す

## 5. 文書更新候補

更新:

- `docs/realtime-bitflyer.md`
- `docs/guides/realtime-bitflyer-getting-started.md`
- `docs/verification.md`
- `docs/roadmap-post-v2.md`
- `docs/document-inventory.md`

追加候補:

- `verification/bitflyer-realtime-resilience.md`

## 6. 実装候補

候補配置:

```text
src/Exchanges/Bitflyer/
  Protocol/Realtime/
  Native/Realtime/
  Composition/Realtime/
```

候補:

- reconnect options
- stream envelope event models
- `Subscribe*StreamAsync` API
- `BitflyerRealtimeErrorKind`
- reconnect-capable protocol wrapper or transport lifecycle helper
- subscribed channel tracking
- private auth callback / session handling
- idle timeout controlled exception
- deterministic transport failure simulation

test 追加候補:

```text
tests/Exchanges/Bitflyer/Protocol.Tests/Realtime/
tests/Exchanges/Bitflyer/Native.Tests/Realtime/
tests/Exchanges/Bitflyer/Composition.Tests/Realtime/
```

## 7. Verification

最低限:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.4.0-local.check
bash scripts/smoke-local-nuget-consumer.sh 3.4.0-local.check
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

resilience live verification を行う場合:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter Realtime
```

live verification は opt-in only とし、default では接続しない。

## 8. 完了条件候補

- v3.4.0 の scope / non-scope が本書に固定されている
- `docs/realtime-bitflyer.md` が reconnect / backoff / resubscribe / idle timeout の contract を固定している
- `docs/realtime-bitflyer.md` が stream envelope API と event 型を固定している
- public realtime API は `IAsyncEnumerable<T>` のまま維持されている
- existing DTO-only API は維持されている
- `Subscribe*StreamAsync` envelope API が追加されている
- private realtime API は public realtime API と混ざっていない
- reconnect 後の private auth 再実行順序が deterministic tests で固定されている
- public reconnect / resubscribe order が deterministic tests で固定されている
- envelope API で `MessageRejected` が malformed / decode failed message を通知し stream を継続する
- cancellation が reconnect より優先される
- auth failure が secret-free controlled exception になる
- `BitflyerRealtimeException.Kind` が restart 判断に使える
- API secret / signature / Authorization 相当値が evidence / log / result / exception / stdout / stderr に出ない
- deterministic tests が通る
- package generation が通る
- local consumer smoke が通る
- live tests は opt-in なしで skip する
- state-changing operation は含まれていない
- Binance realtime / Unified / Rx / state builder は含まれていない

## 9. Release Close 指示

目的:

`v3.4.0` を bitFlyer Realtime resilience foundation release として閉じる。
release close では新機能を追加せず、local preflight 済み commit を `main` に入れ、tag / package publish / GitHub Release / checklist 更新まで行う。

前提:

- `codex/v3.4-dev` が release candidate として clean
- local preflight が `3.4.0-local.preflight` で通っている
- `docs/release-checklist-v3.4.0.md` が local preflight result を持っている
- `docs/release-notes/v3.4.0.md` がある
- state-changing operation / Binance realtime / Unified / Rx / state builder は含めない

release close 手順:

```bash
git checkout main
git pull --ff-only origin main
git merge --ff-only codex/v3.4-dev

dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.4.0
bash scripts/smoke-local-nuget-consumer.sh 3.4.0
bash scripts/create-release-assets.sh 3.4.0 linux-x64 Release
dotnet test ExchangeApi.LiveTests.slnx --no-restore

git tag -a v3.4.0 -m "Release v3.4.0"
git push origin main
git push origin v3.4.0

bash scripts/push-github-packages.sh 3.4.0
bash scripts/smoke-github-packages-consumer.sh 3.4.0
```

GitHub Release:

- tag: `v3.4.0`
- title: `v3.4.0`
- body source: `docs/release-notes/v3.4.0.md`
- attach:
  - `local/publish/release-assets/v3.4.0/exchangeapi-linux-x64`
  - `local/publish/release-assets/v3.4.0/exchangeapi-linux-x64.sha256`
  - `local/publish/release-assets/v3.4.0/exchangeapi-mcp-linux-x64`
  - `local/publish/release-assets/v3.4.0/exchangeapi-mcp-linux-x64.sha256`

release 後:

- `docs/release-checklist-v3.4.0.md` の release result を更新する
- 必要なら `docs/release-notes/v3.4.0.md` の verification summary を実行済み結果に揃える
- release completion commit を `main` に追加して push する
- working tree を clean にする
