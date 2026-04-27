# v3.3.0 bitFlyer Private Realtime Read MVP 実施指示

最終更新: 2026-04-28
位置づけ: v3.3.0 実施指示

状態: implementation

## 1. 目的

v3.3.0 では、v3.1.0 / v3.2.0 で整備した bitFlyer Realtime API 基盤を前提に、bitFlyer private realtime read MVP を検討・実装する。

主目的は、private realtime を public realtime と同じ transport / stream model 上に載せつつ、credentials、auth payload、secret-free evidence、live verification の安全条件を明確にすることである。

v3.3.0 では、private realtime を read-only event stream として扱う。
order / cancel / deposit / withdraw など state-changing operation は含めない。

v3 系は、`v3.0.0` で整理した venue package 構造の上に bitFlyer Realtime API を成熟させる track として扱う。
v3.3.0 はその中で private realtime read MVP を固定する release とし、reconnect / state builder / optional integration は v3.4.0 以降の候補に残す。

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
- Reactive Extensions は v3.3.0 core / venue package には入れず、`ExchangeApi.Optional.Reactive` などの optional package 候補として roadmap に残す

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
- Rx を導入する場合も optional extension / adapter に限定し、主 API は `IAsyncEnumerable<T>` のまま維持する

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

## 10. v3.3.0 Release Close 実施指示

目的:

v3.3.0 は bitFlyer private realtime read MVP release として閉じる。
v3.3.0 に reconnect / state builder / Rx / Binance realtime / Unified / state-changing operation は含めない。

release 前に行うこと:

1. `docs/release-checklist-v3.3.0.md` を追加する。
2. `docs/release-notes/v3.3.0.md` を追加する。
3. local / GitHub Packages consumer smoke が private realtime surface を参照できることを確認する。
4. `dotnet build ExchangeApi.slnx` を実行する。
5. `dotnet test ExchangeApi.slnx --no-restore` を実行する。
6. `bash scripts/pack-local-nuget.sh 3.3.0` を実行する。
7. `bash scripts/smoke-local-nuget-consumer.sh 3.3.0` を実行する。
8. `bash scripts/create-release-assets.sh 3.3.0 linux-x64 Release` を実行する。
9. `dotnet restore ExchangeApi.LiveTests.slnx` を実行する。
10. `dotnet test ExchangeApi.LiveTests.slnx --no-restore` を実行し、opt-in なしで skip することを確認する。

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

release 手順:

1. release preflight 済み commit を `main` に fast-forward merge する。
2. `v3.3.0` tag を作成して push する。
3. `bash scripts/push-github-packages.sh 3.3.0` で GitHub Packages に publish する。
4. `bash scripts/smoke-github-packages-consumer.sh 3.3.0` を実行する。
5. GitHub Release `v3.3.0` を作成し、release assets を attach する。
6. release checklist に preflight / release result を記録する。

完了条件:

- `main` に v3.3.0 commit が入っている
- `v3.3.0` tag が remote にある
- GitHub Release が作成されている
- GitHub Packages consumer smoke が通っている
- release assets が attach されている
- live tests は opt-in なしで skip する
- evidence / logs / stdout / stderr に secret が残らない
