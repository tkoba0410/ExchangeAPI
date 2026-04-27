# v3.1.0 bitFlyer Realtime API 実施指示

最終更新: 2026-04-27
位置づけ: v3.1.0 実施指示

状態: implementation complete / release preflight pending

## 1. 目的

v3.1.0 では bitFlyer Realtime API の public market read MVP を実装する。
Realtime API は HTTP endpoint とは別 transport / interaction model として扱い、bitFlyer venue package `ExchangeApi.Exchanges.Bitflyer` 内に追加する。

本 release では、public market stream の typed read surface、deterministic tests、opt-in live verification を追加する。
private realtime、Unified、Binance realtime、reconnect / backoff の高度化は扱わない。

## 2. 文書運用ルール

`docs/work-instruction-policy.md` に従う。

- 実施指示は本書 `docs/plan-v3.1.0.md` に固定する
- 継続的な設計正本は `docs/realtime-bitflyer.md` に分離する
- 将来候補や見送り理由は `docs/roadmap-post-v2.md` に残す
- `docs/release-notes/v3.1.0.md` は release preflight 以降に利用者向け結果として更新する
- 本フェーズで裁定した内容は、後続の実装フェーズでも正本として扱う

## 3. 仕様書整備 Scope

対象:

- `docs/realtime-bitflyer.md` を追加する
- `docs/spec.md` に Realtime surface の位置づけを最小追記する
- `docs/roadmap-post-v2.md` に v3.1.0 Realtime 方針と後段候補を反映する
- `docs/document-inventory.md` に `docs/realtime-bitflyer.md` を追加する
- 必要なら `AGENTS.md` の Read First に `docs/realtime-bitflyer.md` を追加する

仕様書整備フェーズの非対象:

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

仕様書整備フェーズは完了済みである。
実装は本書の `## 20. Realtime API 実装フェーズ 実施指示` 以降に従って進める。

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

## 18. 仕様書整備フェーズ Verification

本フェーズでは docs-only なので、最低限:

```bash
git diff --check
```

可能なら:

```bash
dotnet test ExchangeApi.slnx --no-restore
```

ただしコード変更がない場合、`dotnet test` は必須ではない。

## 19. 仕様書整備フェーズ完了条件

- [x] `docs/plan-v3.1.0.md` が追加されている
- [x] `docs/realtime-bitflyer.md` が追加されている
- [x] `docs/spec.md` に Realtime surface の位置づけが最小追記されている
- [x] `docs/roadmap-post-v2.md` に v3.1.0 と後段候補が反映されている
- [x] `docs/document-inventory.md` に `docs/realtime-bitflyer.md` が追加されている
- [x] 必要なら `AGENTS.md` に `docs/realtime-bitflyer.md` への導線がある
- [x] Realtime API が HTTP endpoint matrix と分離されている
- [x] v3.1.0 では public market read MVP に限定されている
- [x] DTO / interface / Rx / reconnect / private realtime の裁定が文書化されている
- [x] 仕様書整備フェーズでは実装コード変更を含めていない
- [x] `git diff --check` が通る

## 20. Realtime API 実装フェーズ 実施指示

目的:
仕様書整備フェーズで固定した [`docs/realtime-bitflyer.md`](./realtime-bitflyer.md) に従い、bitFlyer public realtime read MVP を実装する。

実装フェーズでは、Realtime API を HTTP endpoint module とは別 transport / interaction model として追加する。
HTTP endpoint contract、既存 facade / factory / endpoint public API は変更しない。

## 21. 実装フェーズ Scope

対象:

- bitFlyer public realtime read MVP
- `lightning_ticker_<product_code>`
- `lightning_executions_<product_code>`
- `lightning_board_snapshot_<product_code>`
- `lightning_board_<product_code>`
- JSON-RPC 2.0 over WebSocket
- `ClientWebSocket`
- `System.Text.Json`
- `IAsyncEnumerable<T>` based typed stream
- venue-specific DTO
- envelope metadata の共通 interface
- deterministic tests
- opt-in live verification runbook または live test

非対象:

- HTTP endpoint contract 変更
- Binance realtime
- private realtime
- API key / secret を使う realtime auth
- order / cancel / deposit / withdraw など state-changing operation
- full order book state builder
- automatic reconnect / backoff
- resubscribe policy
- Reactive Extensions / `System.Reactive` dependency
- `IObservable<T>` public API
- CLI / MCP の本格 integration
- Unified realtime abstraction
- venue 横断 market semantics interface

## 22. 実装配置

追加候補:

```text
src/Exchanges/Bitflyer/
  Vocabulary/
    BitflyerRealtimeChannels.cs
  Protocol/
    Realtime/
      BitflyerRealtimeProtocolClient.cs
      BitflyerRealtimeJsonRpcRequest.cs
      BitflyerRealtimeJsonRpcResponse.cs
      BitflyerRealtimeChannelMessage.cs
      IBitflyerRealtimeTransport.cs
      WebSocketBitflyerRealtimeTransport.cs
  Native/
    Realtime/
      Public/
        IBitflyerPublicRealtimeClient.cs
        BitflyerPublicRealtimeClient.cs
      Models/
        IRealtimeMessage.cs
        IProductRealtimeMessage.cs
        BitflyerRealtimeTickerMessage.cs
        BitflyerRealtimeExecutionMessage.cs
        BitflyerRealtimeBoardSnapshotMessage.cs
        BitflyerRealtimeBoardDeltaMessage.cs
        BitflyerRealtimeBoardLevel.cs
  Composition/
    Realtime/
      BitflyerRealtimeClientFactory.cs
      BitflyerRealtimeClientOptions.cs
```

test 追加候補:

```text
tests/Exchanges/Bitflyer/Protocol.Tests/Realtime/
tests/Exchanges/Bitflyer/Native.Tests/Realtime/
tests/Exchanges/Bitflyer/Composition.Tests/Realtime/
tests/Exchanges/Bitflyer/Architecture.Tests/
tests/Exchanges/Bitflyer/LiveTests/
```

## 23. 実装ルール

必須:

- `docs/realtime-bitflyer.md` を正本として実装する
- Realtime は HTTP endpoint module に依存しない
- HTTP endpoint module は Realtime に依存しない
- `Protocol/Realtime` は `Native/Realtime` を参照しない
- `Native/Realtime` は `Composition/Realtime` を参照しない
- `Composition/Realtime` だけが concrete wiring を所有する
- channel name は vocabulary helper で生成し、利用者に手書きさせない
- public API は `IAsyncEnumerable<T>` を返す
- cancellation は正常終了として扱う
- remote close / invalid JSON は controlled exception として扱う
- unknown channel は ignore または controlled diagnostic として扱う
- default では live connection を行わない

禁止:

- `System.Reactive` package reference の追加
- `IObservable<T>` の公開
- HTTP `CallResult<TRequest, TResponse>` への無理な押し込み
- HTTP endpoint matrix への realtime channel 追加
- private credentials / API secret を Realtime 実装へ持ち込む
- automatic reconnect を暗黙実装すること

## 24. Public API 候補

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

factory 候補:

```csharp
await using var client = BitflyerRealtimeClientFactory.CreatePublicClient();

await foreach (var ticker in client.SubscribeTickerAsync(ProductCodes.BtcJpy, cancellationToken))
{
    Console.WriteLine(ticker.Ltp);
}
```

options 候補:

```csharp
public sealed class BitflyerRealtimeClientOptions
{
    public Uri EndpointUri { get; init; }
    public TimeSpan ConnectTimeout { get; init; }
}
```

default endpoint は bitFlyer Realtime API の public WebSocket endpoint とし、変更可能にする。

## 25. DTO 実装方針

共通 interface:

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

DTO:

- `BitflyerRealtimeTickerMessage`
- `BitflyerRealtimeExecutionMessage`
- `BitflyerRealtimeBoardSnapshotMessage`
- `BitflyerRealtimeBoardDeltaMessage`
- `BitflyerRealtimeBoardLevel`

方針:

- DTO は bitFlyer native contract として固定する
- JSON field は `JsonPropertyName` で明示する
- timestamp は既存 bitFlyer timestamp 方針に合わせて `DateTimeOffset` へ decode する
- `ReceivedAt` は local receive boundary で付与する
- execution payload が array の場合、typed stream の 1 event をどう表現するかを実装前に test で固定する

## 26. Protocol 実装方針

Protocol は raw channel message までを所有する。

候補 contract:

```csharp
public interface IBitflyerRealtimeProtocolClient : IAsyncDisposable
{
    ValueTask ConnectAsync(CancellationToken cancellationToken = default);

    ValueTask SubscribeAsync(string channel, CancellationToken cancellationToken = default);

    ValueTask UnsubscribeAsync(string channel, CancellationToken cancellationToken = default);

    IAsyncEnumerable<BitflyerRealtimeChannelMessage> ReadMessagesAsync(
        CancellationToken cancellationToken = default);
}
```

transport abstraction は deterministic tests のために置く。

```csharp
public interface IBitflyerRealtimeTransport : IAsyncDisposable
{
    ValueTask ConnectAsync(Uri endpointUri, CancellationToken cancellationToken = default);
    ValueTask SendTextAsync(string text, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> ReadTextAsync(CancellationToken cancellationToken = default);
}
```

## 27. Test 方針

deterministic tests:

- channel name generation
- subscribe request JSON-RPC shape
- unsubscribe request JSON-RPC shape
- raw channel notification parse
- ticker decode
- executions decode
- board snapshot decode
- board delta decode
- `IAsyncEnumerable<T>` cancellation behavior
- remote close behavior
- invalid JSON behavior
- unknown channel behavior
- architecture dependency rule
- `System.Reactive` package reference が入っていないこと
- venue aggregate package generation に影響がないこと

fake transport:

- in-memory text frame queue を使う
- real WebSocket へ接続しない
- deterministic sample JSON を使う
- time-dependent field は injectable clock または test-friendly boundary で固定する

## 28. Live Verification 方針

live verification は opt-in only とする。

候補:

- `EXCHANGEAPI_RUN_LIVE_TESTS=1`
- public realtime only
- short duration
- credentials 不要
- evidence under `local/evidence/local-live/<yyyymmdd>-v3.1.0-bitflyer-realtime/`
- stdout / stderr / evidence secret-free

確認対象:

- ticker stream が 1 件以上受信できる
- executions stream が短時間で受信できる場合は shape を確認する
- board snapshot stream が 1 件以上受信できる
- board delta stream が短時間で受信できる場合は shape を確認する
- opt-in なしでは skip する

live verification が market 状況に依存して不安定な場合、必須 completion gate は deterministic tests とし、live verification は runbook / optional test に留める。

## 29. 実装順

1. Realtime channel vocabulary を追加
2. Protocol JSON-RPC model を追加
3. transport abstraction と fake transport を追加
4. Protocol subscribe / unsubscribe / raw message parse を実装
5. Protocol deterministic tests を追加
6. DTO interface / DTO model を追加
7. Native decode tests を追加
8. Native typed stream client を追加
9. Composition factory / options を追加
10. Architecture tests を更新
11. opt-in live verification を追加
12. docs に実装結果と使い方の最小追記を行う
13. build / test / pack / smoke を実行

## 30. Verification

最低限:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.1.0-local.bitflyer-realtime
bash scripts/smoke-local-nuget-consumer.sh 3.1.0-local.bitflyer-realtime
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

必要に応じて:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter Realtime
```

## 31. 実装フェーズ完了条件

- [x] `docs/realtime-bitflyer.md` の scope に沿っている
- [x] HTTP endpoint contract が変更されていない
- [x] HTTP endpoint matrix に Realtime channel が追加されていない
- [x] bitFlyer public realtime typed stream が実装されている
- [x] ticker / executions / board snapshot / board delta の deterministic tests がある
- [x] public API は `IAsyncEnumerable<T>` based
- [x] `IObservable<T>` public API がない
- [x] `System.Reactive` dependency がない
- [x] private realtime が実装されていない
- [x] automatic reconnect が実装されていない
- [x] full order book state builder が実装されていない
- [x] CLI / MCP 本格 integration が含まれていない
- [x] live tests は opt-in なしで skip する
- [x] deterministic tests が通る
- [x] package generation が通る
- [x] local consumer smoke が通る
- [x] secret-free rule が守られている

実装結果:

- `BitflyerRealtimeClientFactory.CreatePublicClient(...)`
- `IBitflyerPublicRealtimeClient`
- `SubscribeTickerAsync(...)`
- `SubscribeExecutionsAsync(...)`
- `SubscribeBoardSnapshotsAsync(...)`
- `SubscribeBoardDeltasAsync(...)`
- `BitflyerRealtimeChannels`

検証結果:

```text
dotnet build ExchangeApi.slnx --no-restore passed
dotnet test ExchangeApi.slnx --no-restore passed
bash scripts/pack-local-nuget.sh 3.1.0-local.bitflyer-realtime passed
bash scripts/smoke-local-nuget-consumer.sh 3.1.0-local.bitflyer-realtime passed
dotnet restore ExchangeApi.LiveTests.slnx passed
dotnet test ExchangeApi.LiveTests.slnx --no-restore passed; live tests skipped safely without opt-in
```

## 32. Release Preflight

release 前に次を実行する。

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore

bash scripts/pack-local-nuget.sh 3.1.0

find local/nuget -maxdepth 1 -name '*3.1.0.nupkg' -printf '%f\n' | sort

bash scripts/smoke-local-nuget-consumer.sh 3.1.0

bash scripts/create-release-assets.sh 3.1.0 linux-x64 Release

dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

期待 package:

```text
ExchangeApi.Exchanges.Binance.3.1.0.nupkg
ExchangeApi.Exchanges.Bitflyer.3.1.0.nupkg
ExchangeApi.Optional.Credentials.3.1.0.nupkg
ExchangeApi.Optional.Logging.3.1.0.nupkg
ExchangeApi.Primitives.3.1.0.nupkg
```

GitHub Packages publish 後に次を実行する。

```bash
bash scripts/smoke-github-packages-consumer.sh 3.1.0
```
