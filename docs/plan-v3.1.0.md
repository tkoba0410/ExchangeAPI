# v3.1.0 bitFlyer Realtime API 仕様書整備 実施指示

最終更新: 2026-04-27
位置づけ: v3.1.0 実施指示

## 1. 目的

v3.1.0 では bitFlyer Realtime API の public market read MVP を実装する予定である。
実装に入る前に、Realtime API を HTTP endpoint とは別 transport / interaction model として文書上で固定する。

本フェーズでは仕様書・計画書の整備だけを行い、Realtime API 実装は行わない。

## 2. 文書運用ルール

`docs/work-instruction-policy.md` に従う。

- 実施指示は本書 `docs/plan-v3.1.0.md` に固定する
- 継続的な設計正本は `docs/realtime-bitflyer.md` に分離する
- 将来候補や見送り理由は `docs/roadmap-post-v2.md` に残す
- `docs/release-notes/v3.1.0.md` は release 時まで作らなくてよい
- 本フェーズで裁定した内容は、後続の実装フェーズでも正本として扱う

## 3. 本フェーズの Scope

対象:

- `docs/realtime-bitflyer.md` を追加する
- `docs/spec.md` に Realtime surface の位置づけを最小追記する
- `docs/roadmap-post-v2.md` に v3.1.0 Realtime 方針と後段候補を反映する
- `docs/document-inventory.md` に `docs/realtime-bitflyer.md` を追加する
- 必要なら `AGENTS.md` の Read First に `docs/realtime-bitflyer.md` を追加する

非対象:

- Realtime API のコード実装
- project / package 構成変更
- public API 追加
- DTO 追加
- test 追加
- CLI / MCP integration
- private realtime
- Binance realtime
- Reactive Extensions dependency
- full order book state builder
- automatic reconnect / backoff 実装

## 4. `docs/realtime-bitflyer.md` に書く内容

位置づけ:

- bitFlyer Realtime API の設計正本
- HTTP endpoint matrix とは別文書
- v3.1.0 以降の Realtime 実装・test が従う契約

必須構成:

```markdown
# bitFlyer Realtime API

## 1. 目的
## 2. Positioning
## 3. Scope
## 4. Non-Scope
## 5. Transport
## 6. Channel Catalog
## 7. Layer Boundaries
## 8. Public API Shape
## 9. DTO Contract
## 10. Error / Cancellation / Reconnect
## 11. Testing
## 12. Live Verification
## 13. Future Candidates
```

## 5. Positioning

`docs/realtime-bitflyer.md` には次を明記する。

- Realtime API は HTTP API とは別 transport / interaction model
- HTTP endpoint matrix に Realtime channel を混ぜない
- Realtime は connection / subscription / stream の surface
- bitFlyer venue package `ExchangeApi.Exchanges.Bitflyer` 内に置く
- `Protocol` / `Native` / `Composition` / `Vocabulary` は folder / namespace boundary

## 6. v3.1.0 Realtime Scope

v3.1.0 MVP:

- bitFlyer public market realtime read
- JSON-RPC 2.0 over WebSocket
- typed stream
- venue-specific DTO
- opt-in live verification

対象 channel:

```text
lightning_ticker_<product_code>
lightning_executions_<product_code>
lightning_board_snapshot_<product_code>
lightning_board_<product_code>
```

## 7. v3.1.0 Realtime Non-Scope

`docs/realtime-bitflyer.md` と本書に次を非対象として固定する。

- HTTP endpoint contract 変更
- private realtime
- auth / credentials for realtime
- order / cancel / deposit / withdraw
- Binance realtime
- Unified
- full order book builder
- automatic reconnect / backoff
- Rx dependency
- `IObservable<T>` public API
- CLI / MCP 本格 integration
- market data semantics の共通 interface 化

## 8. Layer Boundaries

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

## 9. Public API Shape

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

## 10. DTO / Interface 方針

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

## 11. Transport / Protocol 方針

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
- Websocket.Client など外部 WebSocket wrapper
- full JSON-RPC framework

Protocol contract:

- connect
- subscribe
- unsubscribe
- read raw channel message
- close / dispose

JSON-RPC:

- subscribe method は `subscribe`
- unsubscribe method は `unsubscribe`
- params は channel name
- channel message は channel name と raw payload を保持する

## 12. Error / Cancellation / Reconnect

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

## 13. Testing 方針

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

live verification:

- opt-in only
- public realtime only
- short duration
- credentials 不要
- evidence under `local/evidence/local-live/<yyyymmdd>-v3.1.0-bitflyer-realtime/`
- stdout / stderr / evidence secret-free

## 14. `docs/spec.md` 更新方針

最小追記に留める。

追記内容:

- Realtime surface は HTTP endpoint surface とは別 transport / interaction model
- Realtime は connection / subscription / stream を扱う
- 現時点の Realtime 正本は `docs/realtime-bitflyer.md`
- `Protocol` / `Native` / `Composition` の folder / namespace boundary は Realtime にも適用する
- Realtime は HTTP endpoint module と相互依存しない

書かないこと:

- bitFlyer channel catalog の詳細
- DTO field 一覧
- reconnect 詳細
- v3.1.0 固有の実装順

## 15. `docs/roadmap-post-v2.md` 更新方針

追記内容:

- v3.1.0 は bitFlyer public realtime read MVP
- v3.2 以降候補:
  - reconnect / backoff
  - board state builder
  - Rx optional integration
  - CLI diagnostic command
- v4 は new venue public read MVP 候補として維持
- v5 以降に Unified を残す
- private realtime は後段検討として残す

## 16. `docs/document-inventory.md` 更新方針

`docs/realtime-bitflyer.md` を Keep に追加する。

説明:

- bitFlyer Realtime API の設計正本として維持する

## 17. `AGENTS.md` 更新方針

Read First に追加するか判断する。

追加する場合:

```markdown
- bitFlyer Realtime API 正本: [`docs/realtime-bitflyer.md`](docs/realtime-bitflyer.md)
```

ただし、Realtime 作業時に読む文書として追加し、全作業の必読にしすぎないよう記述を簡潔にする。

## 18. Verification

本フェーズでは docs-only なので、最低限:

```bash
git diff --check
```

可能なら:

```bash
dotnet test ExchangeApi.slnx --no-restore
```

ただしコード変更がない場合、`dotnet test` は必須ではない。

## 19. 完了条件

- `docs/plan-v3.1.0.md` が追加されている
- `docs/realtime-bitflyer.md` が追加されている
- `docs/spec.md` に Realtime surface の位置づけが最小追記されている
- `docs/roadmap-post-v2.md` に v3.1.0 と後段候補が反映されている
- `docs/document-inventory.md` に `docs/realtime-bitflyer.md` が追加されている
- 必要なら `AGENTS.md` に `docs/realtime-bitflyer.md` への導線がある
- Realtime API が HTTP endpoint matrix と分離されている
- v3.1.0 では public market read MVP に限定されている
- DTO / interface / Rx / reconnect / private realtime の裁定が文書化されている
- 実装コード変更は含まれていない
- `git diff --check` が通る
