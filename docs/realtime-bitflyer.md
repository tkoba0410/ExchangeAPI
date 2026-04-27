# bitFlyer Realtime API

最終更新: 2026-04-28
位置づけ: bitFlyer Realtime API 設計正本

## 1. 目的

本書は、bitFlyer Realtime API の設計境界、公開方針、channel contract、test / live verification 方針を定義する。

Realtime API は HTTP endpoint とは別 transport / interaction model である。
そのため、HTTP endpoint matrix である [`docs/endpoints-bitflyer.md`](./endpoints-bitflyer.md) には Realtime channel を混ぜない。

外部参照:

- [bitFlyer Lightning API Documentation](https://lightning.bitflyer.com/docs/api)
- [bitFlyer Lightning Realtime API Playground](https://lightning.bitflyer.com/docs/playgroundrealtime?lang=en)

## 2. Positioning

bitFlyer Realtime API は `ExchangeApi.Exchanges.Bitflyer` package 内に置く。

Realtime surface は、HTTP endpoint の `request -> response` model ではなく、次の model を正本とする。

- connection
- subscription
- stream
- message decode
- cancellation / close

`Protocol` / `Native` / `Composition` / `Vocabulary` は、v3.0.0 以降の方針どおり package / project 境界ではなく folder / namespace / tests 上の設計境界である。
Realtime API にも同じ境界を適用する。

## 3. Scope

v3.1.0 の Realtime MVP は bitFlyer public market realtime read に限定する。
v3.3.0 では bitFlyer private realtime read MVP を追加する。

採用:

- JSON-RPC 2.0 over WebSocket
- public market stream
- typed stream
- venue-specific DTO
- `IAsyncEnumerable<T>` based public API
- opt-in live verification

対象 channel:

```text
lightning_ticker_<product_code>
lightning_executions_<product_code>
lightning_board_snapshot_<product_code>
lightning_board_<product_code>
```

v3.1.0 implementation surface:

- `BitflyerRealtimeClientFactory.CreatePublicClient(...)`
- `IBitflyerPublicRealtimeClient`
- `SubscribeTickerAsync(...)`
- `SubscribeExecutionsAsync(...)`
- `SubscribeBoardSnapshotsAsync(...)`
- `SubscribeBoardDeltasAsync(...)`
- `BitflyerRealtimeChannels`

v3.3.0 private implementation surface:

- `BitflyerRealtimeClientFactory.CreatePrivateClient(...)`
- `IBitflyerPrivateRealtimeClient`
- `SubscribeChildOrderEventsAsync(...)`
- `SubscribeParentOrderEventsAsync(...)`

v3.4.0 stream envelope implementation surface:

- `SubscribeTickerStreamAsync(...)`
- `SubscribeExecutionsStreamAsync(...)`
- `SubscribeBoardSnapshotsStreamAsync(...)`
- `SubscribeBoardDeltasStreamAsync(...)`
- `SubscribeChildOrderEventsStreamAsync(...)`
- `SubscribeParentOrderEventsStreamAsync(...)`
- `BitflyerRealtimeStreamEvent<T>` and concrete lifecycle event types
- `BitflyerRealtimeReconnectOptions`
- `BitflyerRealtimeErrorKind`

## 4. Non-Scope

Realtime API では次を扱わない。

- HTTP endpoint contract 変更
- order / cancel / deposit / withdraw などの state-changing operation
- Binance realtime
- Unified realtime abstraction
- full order book state builder
- Reactive Extensions / `System.Reactive` dependency
- `IObservable<T>` public API
- CLI / MCP の本格 integration
- venue 横断 market semantics interface

## 5. Transport

基盤技術:

- `ClientWebSocket`
- JSON-RPC 2.0 over WebSocket
- `System.Text.Json`
- `CancellationToken`
- `IAsyncDisposable`
- 必要なら internal `Channel<T>`

採用しない:

- `System.Reactive`
- SignalR
- Websocket.Client などの外部 WebSocket wrapper
- full JSON-RPC framework

Protocol contract:

- connect
- subscribe
- unsubscribe
- read raw channel message
- close / dispose

JSON-RPC contract:

- subscribe method は `subscribe`
- unsubscribe method は `unsubscribe`
- private auth method は `auth`
- params は channel name
- channel message は channel name と raw payload を保持する

private auth contract:

- private channel 購読前に `auth` response を確認する
- auth params は `api_key`, `timestamp`, `nonce`, `signature`
- `timestamp` は Unix timestamp millisecond を使う
- `nonce` は request ごとに 16 byte 以上の random string を使う
- `signature` は `timestamp` と `nonce` の文字列連結を `IApiCredentialSession.Sign(payload)` で署名した値とする
- API secret は public API に出さない
- API key、API secret、signature、Authorization 相当の値は evidence / log / result / exception / stdout / stderr に含めない

## 6. Channel Catalog

v3.1.0 では public market read channel だけを扱う。
v3.3.0 では private read channel を追加する。

| Channel Pattern | Scope | Message DTO | Notes |
| --- | --- | --- | --- |
| `lightning_ticker_<product_code>` | public market | `BitflyerRealtimeTickerMessage` | ticker event |
| `lightning_executions_<product_code>` | public market | `BitflyerRealtimeExecutionMessage` | execution event; payload can contain multiple executions |
| `lightning_board_snapshot_<product_code>` | public market | `BitflyerRealtimeBoardSnapshotMessage` | order book snapshot event |
| `lightning_board_<product_code>` | public market | `BitflyerRealtimeBoardDeltaMessage` | order book delta event; v3.1.0 does not build full local book state |
| `child_order_events` | private read | `BitflyerRealtimeChildOrderEventMessage` | order event; payload can contain multiple events |
| `parent_order_events` | private read | `BitflyerRealtimeParentOrderEventMessage` | parent order event; payload can contain multiple events |

Channel name は利用者に手書きさせない。
`Vocabulary` に channel name builder を置き、`ProductCodes` と併用できるようにする。

## 7. Layer Boundaries

想定配置:

```text
src/Exchanges/Bitflyer/
  Vocabulary/
  Protocol/
    Realtime/
  Native/
    Realtime/
  Composition/
    Realtime/
```

責務:

- `Vocabulary`
  - product code
  - channel name builder
  - string vocabulary
- `Protocol/Realtime`
  - WebSocket transport
  - JSON-RPC 2.0 shape
  - subscribe / unsubscribe
  - raw channel message
- `Native/Realtime`
  - bitFlyer native DTO
  - typed decode
  - typed stream API
- `Composition/Realtime`
  - factory
  - options
  - default endpoint
  - wiring

依存方向:

```text
Composition/Realtime -> Native/Realtime -> Protocol/Realtime -> Vocabulary / Primitives
Native/Realtime -> Vocabulary / Primitives
Protocol/Realtime -> Vocabulary / Primitives
```

禁止:

- `Protocol/Realtime` -> `Native/Realtime`
- `Native/Realtime` -> `Composition/Realtime`
- Realtime -> HTTP endpoint module
- HTTP endpoint module -> Realtime

## 8. Public API Shape

主 API は `IAsyncEnumerable<T>` とする。

```csharp
public interface IBitflyerPublicRealtimeClient : IAsyncDisposable
{
    IAsyncEnumerable<BitflyerRealtimeTickerMessage> SubscribeTickerAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeExecutionMessage> SubscribeExecutionsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeBoardSnapshotMessage> SubscribeBoardSnapshotsAsync(
        string productCode,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeBoardDeltaMessage> SubscribeBoardDeltasAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}
```

Private realtime は public realtime と client を分ける。

```csharp
public interface IBitflyerPrivateRealtimeClient : IAsyncDisposable
{
    IAsyncEnumerable<BitflyerRealtimeChildOrderEventMessage> SubscribeChildOrderEventsAsync(
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeParentOrderEventMessage> SubscribeParentOrderEventsAsync(
        CancellationToken cancellationToken = default);
}
```

v3.4.0 では lifecycle-aware stream envelope API を追加する。
既存 `Subscribe*Async` は DTO-only stream として維持し、lifecycle event を混ぜない。
`Subscribe*StreamAsync` は data と lifecycle event を同じ `IAsyncEnumerable<T>` 上に流す envelope stream とする。

```csharp
public interface IBitflyerPublicRealtimeClient : IAsyncDisposable
{
    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeTickerMessage>> SubscribeTickerStreamAsync(
        string productCode,
        CancellationToken cancellationToken = default);
}
```

```csharp
public interface IBitflyerPrivateRealtimeClient : IAsyncDisposable
{
    IAsyncEnumerable<BitflyerRealtimeStreamEvent<BitflyerRealtimeChildOrderEventMessage>> SubscribeChildOrderEventsStreamAsync(
        CancellationToken cancellationToken = default);
}
```

利用例:

```csharp
await using var client = BitflyerRealtimeClientFactory.CreatePublicClient();

await foreach (var ticker in client.SubscribeTickerAsync(ProductCodes.BtcJpy, cancellationToken))
{
    Console.WriteLine(ticker.Ltp);
}
```

方針:

- 利用者に JSON-RPC request を組み立てさせない
- 利用者に channel name を手書きさせない
- `ProductCodes` を使えるようにする
- `Subscribe*Async` は `IAsyncEnumerable<T>` を返す
- `Subscribe*StreamAsync` は `IAsyncEnumerable<BitflyerRealtimeStreamEvent<T>>` を返す
- `IObservable<T>` は返さない
- Rx 変換 extension は core package に入れない

## 8.1 Stream Envelope Contract

v3.4.0 の envelope event 型:

- `BitflyerRealtimeData<T>`
- `BitflyerRealtimeReconnecting<T>`
- `BitflyerRealtimeReconnected<T>`
- `BitflyerRealtimeAuthenticationReplayed<T>`
- `BitflyerRealtimeResubscribed<T>`
- `BitflyerRealtimeContinuityLost<T>`
- `BitflyerRealtimeMessageRejected<T>`

`StreamError` event は採用しない。
recoverable lifecycle は envelope event として流し、fatal error は controlled exception として stream を終了する。

`MessageRejected` は malformed JSON / decode failed message の通知に限定する。
`MessageRejected` は raw payload、API key、API secret、signature、Authorization 相当値を持たない。
`MessageRejected` は continuity-impacting event として扱う。

## 9. DTO Contract

共通 interface は envelope metadata に限定する。

```csharp
public interface IRealtimeMessage
{
    string Channel { get; }
    DateTimeOffset ReceivedAt { get; }
}

public interface IProductRealtimeMessage : IRealtimeMessage
{
    string ProductCode { get; }
}
```

venue-specific DTO:

- `BitflyerRealtimeTickerMessage`
- `BitflyerRealtimeExecutionMessage`
- `BitflyerRealtimeBoardSnapshotMessage`
- `BitflyerRealtimeBoardDeltaMessage`
- `BitflyerRealtimeBoardLevel`
- `BitflyerRealtimeChildOrderEventMessage`
- `BitflyerRealtimeParentOrderEventMessage`

禁止:

- `ITickerMessage`
- `IExecutionMessage`
- `IBoardMessage`
- `IBoardLevel`
- venue 横断 market semantics interface
- Unified-like DTO abstraction

理由:

- ticker / execution / board の意味は venue 間で差がある
- v3.1.0 では bitFlyer native contract を正確に固定する
- semantic unification は v5 以降の Unified 候補に残す

## 10. Error / Cancellation / Reconnect

DTO-only stream contract:

- cancellation: 正常終了
- dispose: connection close
- remote close: controlled exception
- invalid JSON: controlled exception
- unknown channel: ignore または controlled diagnostic
- typed stream 終了時: subscribed channel へ best-effort unsubscribe を送る
- automatic reconnect: disabled

Envelope stream contract:

- reconnect: default enabled
- reconnect target: remote close / transport exception / idle timeout
- cancellation / dispose: normal completion
- non-target channel message: ignore
- malformed target message: `MessageRejected` event and continue
- auth failure: controlled exception
- resubscribe failure: controlled exception
- reconnect attempts exhausted: controlled exception
- fatal error after stream end: restart decision belongs to the user
- gap-free continuity: not guaranteed
- reconnect / resubscribe after transport interruption emits `ContinuityLost`

Private reconnect order:

```text
Reconnecting
Reconnected
AuthenticationReplayed
Resubscribed
ContinuityLost
Data...
```

Public reconnect order:

```text
Reconnecting
Reconnected
Resubscribed
ContinuityLost
Data...
```

Backoff default:

```text
MaxAttempts = 3
InitialDelay = 1 second
MaxDelay = 10 seconds
Jitter = none
```

Backoff values are configurable.
`MaxAttempts = 0` means reconnect disabled.
Representative presets:

| Preset | MaxAttempts | InitialDelay | MaxDelay | Intended use |
| --- | ---: | --- | --- | --- |
| no reconnect | 0 | 1 second | 10 seconds | DTO-like fail-fast operation |
| interactive | 3 | 1 second | 10 seconds | terminal / short-running tools |
| conservative | 5 | 2 seconds | 30 seconds | manual monitoring |
| long-running | 10 | 1 second | 60 seconds | supervised services |

Custom values are allowed when they pass option validation.

Idle timeout:

- default disabled
- configured with `TimeSpan? IdleTimeout`
- valid value is `null` or `> TimeSpan.Zero`
- timeout is reconnect target
- reconnect after idle timeout emits `ContinuityLost`

Representative presets:

| Preset | IdleTimeout | Intended use |
| --- | --- | --- |
| disabled | `null` | default; no local no-message timeout |
| interactive | 30 seconds | short diagnostic run |
| monitoring | 60 seconds | supervised public stream |
| aggressive | 10 seconds | local transport tests / quick failure detection |

Fatal realtime exceptions expose `BitflyerRealtimeErrorKind`.
Exception messages must be secret-free.

理由:

- DTO-only API は既存利用者向けに単純な typed stream として維持する
- envelope API は lifecycle event を混ぜられるため reconnect / resubscribe / continuity loss を明示できる
- reconnect 後の gap-free continuity は保証しないため、利用者は `ContinuityLost` を見て再同期要否を判断する

## 11. Testing

deterministic tests:

- channel name generation
- subscribe request JSON-RPC shape
- unsubscribe request JSON-RPC shape
- raw channel notification parse
- ticker decode
- executions decode
- board snapshot decode
- board delta decode
- cancellation behavior
- invalid JSON behavior
- stream envelope event behavior
- `MessageRejected` behavior
- reconnect / resubscribe event order
- private auth replay event order
- error kind behavior
- unknown channel behavior
- private auth request JSON-RPC shape
- private auth error behavior
- child order event decode
- parent order event decode
- architecture dependency rule

test taxonomy:

- `tests/Exchanges/Bitflyer/Protocol.Tests/Realtime/`
- `tests/Exchanges/Bitflyer/Native.Tests/Realtime/`
- `tests/Exchanges/Bitflyer/Composition.Tests/Realtime/`
- `tests/Exchanges/Bitflyer/Architecture.Tests/`
- `tests/Exchanges/Bitflyer/LiveTests/`

consumer smoke:

- local / GitHub Packages consumer smoke should restore `ExchangeApi.Exchanges.Bitflyer`
- smoke should verify the Realtime factory and channel vocabulary are visible from the aggregate venue package

## 12. Live Verification

live verification は opt-in only とする。

方針:

- public realtime only
- short duration
- credentials 不要
- evidence under `local/evidence/local-live/<yyyymmdd>-v3.1.0-bitflyer-realtime/`
- stdout / stderr / evidence secret-free

default では live connection を行わない。

## 12.1 Private Realtime Design Note

private realtime は v3.3.0 で read-only MVP として扱う。
public realtime とは別 client / auth scope として設計する。

採用:

- `auth` payload
- API key / API secret / signature handling
- credential session lifetime
- private channel catalog
- private event DTO
- secret-free evidence / log / exception rule

対象 channel:

```text
child_order_events
parent_order_events
```

必須条件:

- API key、API secret、signature、Authorization 相当の値を evidence / log / result / exception / stdout / stderr に含めない
- raw credential profile を evidence にコピーしない
- private realtime live verification は opt-in only とする
- state-changing operation は扱わない
- HTTP private endpoint の credential provider 方針と矛盾させない

v3.3.0 でやらないこと:

- private channel への default live 接続
- order / cancel / deposit / withdraw などの state-changing operation
- raw credential profile / raw auth payload の evidence 化
- reconnect / resubscribe 時の auth 再実行方針の本格実装

## 13. Future Candidates

v3.2 以降候補:

- reconnect / backoff
- resubscribe
- board state builder
- Rx optional integration
- CLI diagnostic command
- MCP diagnostic surface

後段候補:

- private realtime
- Binance realtime
- Unified realtime abstraction
