# v3.3.0 bitFlyer Private Realtime Read MVP 実施指示

最終更新: 2026-04-28
位置づけ: v3.3.0 実施指示

状態: implementation

## 1. 目的

v3.3.0 では、v3.1.0 / v3.2.0 で整備した bitFlyer Realtime API 基盤を前提に、bitFlyer private realtime read MVP を検討・実装する。

主目的は、private realtime を public realtime と同じ transport / stream model 上に載せつつ、credentials、auth payload、secret-free evidence、live verification の安全条件を明確にすることである。

v3.3.0 では、private realtime を read-only event stream として扱う。
order / cancel / deposit / withdraw など state-changing operation は含めない。

## 2. 採用候補

bitFlyer private realtime read MVP:

- private realtime auth design
- credential session を使う realtime auth payload signing
- private channel catalog の最小固定
- private event DTO の最小追加
- typed stream API
- deterministic auth request shape tests
- deterministic private event decode tests
- opt-in live verification runbook
- secret-free evidence / log / stdout / stderr rule の強化

候補 channel:

- child order events
- parent order events

正式 channel 名と response shape は `docs/realtime-bitflyer.md` に固定する。

## 3. 非対象

v3.3.0 では次を扱わない。

- order / cancel / deposit / withdraw など state-changing operation
- HTTP endpoint contract 変更
- Binance realtime
- Unified realtime abstraction
- reconnect / backoff / resubscribe の本格実装
- full order book state builder
- Rx dependency の core package 追加
- `IObservable<T>` public API
- credentials provider 拡張
- CLI / MCP の本格 integration
- private realtime live verification の default 実行
- raw credentials / signature / Authorization 相当値の evidence 化

## 4. 必須裁定

実装前に次を裁定する。

- bitFlyer private realtime の auth request shape
- auth payload の signing input
- credential session lifetime
- private channel catalog の v3.3.0 MVP 範囲
- private DTO の venue-native field shape
- public realtime client と private realtime client を分ける API shape
- private live verification の opt-in 条件
- secret-free scan / evidence rule

裁定済み:

- auth method は JSON-RPC `auth`
- auth params は `api_key`, `timestamp`, `nonce`, `signature`
- signature input は `timestamp` と `nonce` の文字列連結
- signing は `IApiCredentialSession.Sign(payload)` を使う
- v3.3.0 MVP channel は `child_order_events` / `parent_order_events`
- public / private realtime client は分ける
- private live verification は opt-in only

## 5. API 方針候補

public realtime と private realtime は client を分ける。

候補:

```csharp
await using var client = BitflyerRealtimeClientFactory.CreatePrivateClient(credentialProvider);

await foreach (var message in client.SubscribeChildOrderEventsAsync(cancellationToken))
{
    Console.WriteLine(message.EventType);
}
```

方針:

- private realtime client は credentials を必須にする
- API secret は public API に出さない
- signing は `IApiCredentialSession.Sign(payload)` を使う
- DTO は bitFlyer venue-specific とする
- common interface は envelope metadata に限定する
- `IObservable<T>` は返さない

## 6. 文書更新候補

更新:

- `docs/realtime-bitflyer.md`
- `docs/verification.md`
- `docs/guides/realtime-bitflyer-getting-started.md`
- `docs/roadmap-post-v2.md`
- `docs/document-inventory.md`

追加候補:

- `verification/bitflyer-private-realtime-live.md`

## 7. 実装候補

候補配置:

```text
src/Exchanges/Bitflyer/
  Protocol/Realtime/
    Private/
  Native/Realtime/
    Private/
    Models/
  Composition/Realtime/
```

test 追加候補:

```text
tests/Exchanges/Bitflyer/Protocol.Tests/Realtime/
tests/Exchanges/Bitflyer/Native.Tests/Realtime/
tests/Exchanges/Bitflyer/Composition.Tests/Realtime/
tests/Exchanges/Bitflyer/LiveTests/
```

## 8. Verification

最低限:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.3.0-local.check
bash scripts/smoke-local-nuget-consumer.sh 3.3.0-local.check
dotnet restore ExchangeApi.LiveTests.slnx
dotnet test ExchangeApi.LiveTests.slnx --no-restore
```

private realtime live verification を行う場合:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter PrivateRealtime
```

live verification は opt-in only とし、default では接続しない。

## 9. 完了条件候補

- v3.3.0 の scope / non-scope が本書に固定されている
- private realtime auth / secret-free rule が `docs/realtime-bitflyer.md` に固定されている
- private realtime public API が public realtime API と混ざっていない
- API secret / signature / Authorization 相当値が evidence / log / result / exception / stdout / stderr に出ない
- deterministic tests が通る
- package generation が通る
- local consumer smoke が通る
- live tests は opt-in なしで skip する
- state-changing operation は含まれていない
- Binance realtime / Unified / Rx / full reconnect は含まれていない
