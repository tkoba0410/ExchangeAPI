# bitFlyer Realtime API

最終更新: 2026-04-27
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

## 4. Non-Scope

v3.1.0 では次を扱わない。

- HTTP endpoint contract 変更
- private realtime
- auth / credentials for realtime
- API key / API secret を使う realtime auth
- order / cancel / deposit / withdraw などの state-changing operation
- Binance realtime
- Unified realtime abstraction
- full order book state builder
- automatic reconnect / backoff
- resubscribe policy
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
- params は channel name
- channel message は channel name と raw payload を保持する

## 6. Channel Catalog

v3.1.0 では public market read channel だけを扱う。

| Channel Pattern | Scope | Message DTO | Notes |
| --- | --- | --- | --- |
| `lightning_ticker_<product_code>` | public market | `BitflyerRealtimeTickerMessage` | ticker event |
| `lightning_executions_<product_code>` | public market | `BitflyerRealtimeExecutionMessage` | execution event; payload can contain multiple executions |
| `lightning_board_snapshot_<product_code>` | public market | `BitflyerRealtimeBoardSnapshotMessage` | order book snapshot event |
| `lightning_board_<product_code>` | public market | `BitflyerRealtimeBoardDeltaMessage` | order book delta event; v3.1.0 does not build full local book state |

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
- `IObservable<T>` は返さない
- Rx 変換 extension は core package に入れない

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

v3.1.0 initial contract:

- cancellation: 正常終了
- dispose: connection close
- remote close: controlled exception
- invalid JSON: controlled exception
- unknown channel: ignore または controlled diagnostic
- automatic reconnect: 実装しない

理由:

- reconnect は board delta の連続性と欠落検知を伴う
- v3.1.0 では接続中に受けた event の typed stream 化を優先する
- reconnect / backoff / resubscribe は v3.2 以降の候補とする

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
- unknown channel behavior
- architecture dependency rule

test taxonomy:

- `tests/Exchanges/Bitflyer/Protocol.Tests/Realtime/`
- `tests/Exchanges/Bitflyer/Native.Tests/Realtime/`
- `tests/Exchanges/Bitflyer/Composition.Tests/Realtime/`
- `tests/Exchanges/Bitflyer/Architecture.Tests/`
- `tests/Exchanges/Bitflyer/LiveTests/`

## 12. Live Verification

live verification は opt-in only とする。

方針:

- public realtime only
- short duration
- credentials 不要
- evidence under `local/evidence/local-live/<yyyymmdd>-v3.1.0-bitflyer-realtime/`
- stdout / stderr / evidence secret-free

default では live connection を行わない。

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
