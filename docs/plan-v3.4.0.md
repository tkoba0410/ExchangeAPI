# v3.4.0 bitFlyer Realtime Resilience Foundation 実施指示

最終更新: 2026-04-28
位置づけ: v3.4.0 実施指示

状態: planning

## 1. 目的

v3.4.0 では、v3.1.0 から v3.3.0 で追加した bitFlyer Realtime API の public / private read surface を前提に、connection lifecycle の再現性と利用時の堅牢性を上げる。

主題は bitFlyer Realtime resilience foundation とする。
具体的には reconnect / backoff / resubscribe / private auth 再実行 / idle timeout の設計と最小実装を扱う。

v3.4.0 は v3 系の Realtime maturity track の一部であり、新 venue、Unified、state-changing operation は扱わない。

## 2. 採用候補

採用候補:

- realtime reconnect policy
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
- resilience は options で opt-in または明示設定にする
- default behavior を過度に変えない
- reconnect による missed message replay は保証しない
- private auth 再実行でも API secret / signature を log / exception / result に出さない

候補 option:

```csharp
public sealed class BitflyerRealtimeClientOptions
{
    public Uri EndpointUri { get; init; }
    public TimeSpan? ConnectTimeout { get; init; }
    public BitflyerRealtimeReconnectOptions? Reconnect { get; init; }
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

初期方針:

- reconnect は明示 option で有効化する
- cancellation は常に reconnect より優先する
- auth failure は原則 retry せず controlled exception とする
- reconnect 後は auth を確認してから private channel を resubscribe する
- board delta の gap-free continuity は保証しない
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
- public realtime API は `IAsyncEnumerable<T>` のまま維持されている
- private realtime API は public realtime API と混ざっていない
- reconnect 後の private auth 再実行順序が deterministic tests で固定されている
- cancellation が reconnect より優先される
- auth failure が secret-free controlled exception になる
- API secret / signature / Authorization 相当値が evidence / log / result / exception / stdout / stderr に出ない
- deterministic tests が通る
- package generation が通る
- local consumer smoke が通る
- live tests は opt-in なしで skip する
- state-changing operation は含まれていない
- Binance realtime / Unified / Rx / state builder は含まれていない
