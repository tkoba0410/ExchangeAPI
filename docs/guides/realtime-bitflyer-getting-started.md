# bitFlyer Realtime Getting Started

最終更新: 2026-04-28
位置づけ: bitFlyer Realtime API 利用 guide

本書は `ExchangeApi.Exchanges.Bitflyer` package から bitFlyer Realtime API を使うための最小手順を示す。
継続的な設計正本は [`docs/realtime-bitflyer.md`](../realtime-bitflyer.md) とする。

## 1. Package

参照する package:

```text
ExchangeApi.Exchanges.Bitflyer
```

Realtime API は v3.0.0 以降の aggregate venue package 内にある。
layer-specific package は使わない。

## 2. Ticker

```csharp
using ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await using var client = BitflyerRealtimeClientFactory.CreatePublicClient();

await foreach (var ticker in client.SubscribeTickerAsync(ProductCodes.BtcJpy, cancellation.Token))
{
    Console.WriteLine($"{ticker.ProductCode} {ticker.Ltp}");
    break;
}
```

## 3. Executions

```csharp
await foreach (var execution in client.SubscribeExecutionsAsync(ProductCodes.BtcJpy, cancellation.Token))
{
    Console.WriteLine($"{execution.Side} {execution.Price} {execution.Size}");
}
```

`lightning_executions_<product_code>` の payload は array で届く。
library の typed stream は array item を `BitflyerRealtimeExecutionMessage` として flatten する。

## 4. Board Snapshot / Delta

```csharp
await foreach (var snapshot in client.SubscribeBoardSnapshotsAsync(ProductCodes.BtcJpy, cancellation.Token))
{
    Console.WriteLine(snapshot.MidPrice);
    break;
}

await foreach (var delta in client.SubscribeBoardDeltasAsync(ProductCodes.BtcJpy, cancellation.Token))
{
    Console.WriteLine(delta.MidPrice);
    break;
}
```

v3.2.0 では board state builder は提供しない。
`SubscribeBoardSnapshotsAsync(...)` と `SubscribeBoardDeltasAsync(...)` は event DTO を返すだけで、local order book state は構築しない。

## 5. Lifecycle

- `Subscribe*Async(...)` は対象 channel へ subscribe する
- stream 終了時は対象 channel へ best-effort unsubscribe を送る
- cancellation は利用者の正常終了意図として扱う
- remote close / invalid JSON は controlled exception として扱う
- DTO-only stream は lifecycle event を混ぜず、automatic reconnect もしない
- lifecycle を扱う場合は `Subscribe*StreamAsync(...)` を使う

```csharp
await foreach (var item in client.SubscribeTickerStreamAsync(ProductCodes.BtcJpy, cancellation.Token))
{
    if (item is BitflyerRealtimeData<BitflyerRealtimeTickerMessage> data)
    {
        Console.WriteLine(data.Value.Ltp);
    }
    else if (item is BitflyerRealtimeContinuityLost<BitflyerRealtimeTickerMessage>)
    {
        Console.WriteLine("continuity lost; consider resync");
    }
}
```

`Subscribe*StreamAsync(...)` は reconnect / resubscribe / continuity loss を envelope event として通知する。
default reconnect は `MaxAttempts = 3`, `InitialDelay = 1 second`, `MaxDelay = 10 seconds`, jitter なしである。
`BitflyerRealtimeClientOptions.Reconnect` と `IdleTimeout` で変更できる。

## 6. Private Read

private realtime read は public realtime とは別 client を使う。
credential は `IApiCredentialProvider` から開き、API secret は public API に出さない。

```csharp
using ExchangeApi.Exchanges.Bitflyer.Composition.Realtime;
using ExchangeApi.Optional.Credentials.PlainText;
using ExchangeApi.Primitives.Credentials;

var credentialProvider = PlainTextApiCredentialProviderFactory.Create(
    ExchangeVenue.Bitflyer,
    apiKey,
    apiSecret);
await using var client = BitflyerRealtimeClientFactory.CreatePrivateClient(credentialProvider);

await foreach (var item in client.SubscribeChildOrderEventsAsync(cancellation.Token))
{
    Console.WriteLine($"{item.ProductCode} {item.EventType}");
}
```

対象 private channel:

- `child_order_events`
- `parent_order_events`

private realtime は read-only event stream として扱う。
order / cancel / deposit / withdraw などの state-changing operation は含めない。

## 7. Credentials

public realtime read には credentials は不要である。

private realtime read には credentials が必要である。
API key、API secret、signature、Authorization 相当の値を Realtime evidence / log / result / exception / stdout / stderr に含めてはならない。
