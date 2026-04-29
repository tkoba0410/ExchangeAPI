# bitFlyer Realtime Resilience Verification

最終更新: 2026-04-29
位置づけ: bitFlyer realtime resilience verification runbook

## 1. 目的

本書は v3.4.0 bitFlyer realtime resilience foundation の verification 手順を定義する。
設計正本は [`docs/realtime-bitflyer.md`](../docs/realtime-bitflyer.md) とする。
Realtime diagnostics / secret-free observability の正本は [`docs/realtime-diagnostics.md`](../docs/realtime-diagnostics.md) とする。

## 2. 対象

対象:

- `Subscribe*StreamAsync(...)` envelope API
- reconnect / backoff / resubscribe lifecycle
- private auth replay lifecycle
- `MessageRejected`
- `ContinuityLost`
- `BitflyerRealtimeException.Kind`
- idle timeout option

非対象:

- DTO-only API の reconnect
- full order book state builder
- private order state builder
- Binance realtime
- Unified realtime abstraction
- Rx / `IObservable<T>`
- order / cancel / deposit / withdraw などの state-changing operation

## 3. Deterministic Verification

release gate は deterministic tests を主とする。

```bash
dotnet test ExchangeApi.slnx --no-restore --filter Realtime
```

確認観点:

- envelope data event が DTO を保持する
- malformed / decode failed target message が `MessageRejected` になる
- public reconnect order が `Reconnecting -> Reconnected -> Resubscribed -> ContinuityLost -> Data...` になる
- private reconnect order が `Reconnecting -> Reconnected -> AuthenticationReplayed -> Resubscribed -> ContinuityLost -> Data...` になる
- reconnect exhausted が controlled exception になる
- exception message に secret が含まれない

## 4. Live Verification

live verification は opt-in only とする。
default の test / preflight では live connection しない。

public live:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter Realtime
```

private live:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter PrivateRealtime
```

private live には `local/credentials/credential-profile.json` が必要である。

## 5. Evidence

evidence を残す場合の標準配置:

```text
local/evidence/local-live/<yyyymmdd>-v3.9.0-bitflyer-realtime-resilience/
  runtime/
    artifacts/
    logs/
  notes/
```

secret-free rule:

- credentials を evidence / logs / stdout / stderr / result / exception に含めない
- API key を evidence / logs / stdout / stderr / result / exception に含めない
- API secret を evidence / logs / stdout / stderr / result / exception に含めない
- signature を evidence / logs / stdout / stderr / result / exception に含めない
- Authorization 相当の値を evidence / logs / stdout / stderr / result / exception に含めない
- raw credential profile を evidence にコピーしない
- raw auth payload を evidence にコピーしない

## 6. Release Gate

v3.9.0 close release gate:

- deterministic realtime tests が通る
- local consumer smoke が stream envelope / realtime options surface を参照できる
- live tests は opt-in なしで skip する
- secret-free rule を満たす
- state-changing operation が増えていない
