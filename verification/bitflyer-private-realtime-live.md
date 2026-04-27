# bitFlyer Private Realtime Live Verification

最終更新: 2026-04-28
位置づけ: bitFlyer private realtime opt-in live verification runbook

## 1. 目的

本書は v3.3.0 bitFlyer private realtime read MVP の live verification 手順を定義する。
private realtime は read-only event stream として扱い、order / cancel / deposit / withdraw などの state-changing operation は含めない。

## 2. 実行条件

live verification は opt-in only とする。
default の test / preflight では live connection しない。

必要条件:

```text
EXCHANGEAPI_RUN_LIVE_TESTS=1
credential profile configured at local/credentials/credential-profile.json
API key has permission to receive order events
```

## 3. 対象

対象 channel:

- `child_order_events`
- `parent_order_events`

確認観点:

- `auth` response を確認してから subscribe する
- response shape が DTO に decode できる
- stream は cancellation で終了できる
- stream 終了時に best-effort unsubscribe を行う
- state-changing operation が増えていない
- result / error / stdout / stderr / evidence に secret がない

## 4. Evidence

evidence を残す場合の標準配置:

```text
local/evidence/local-live/<yyyymmdd>-v3.3.0-bitflyer-private-realtime/
  runtime/
    artifacts/
    logs/
  notes/
```

禁止:

- raw credential profile のコピー
- API key
- API secret
- signature
- Authorization 相当値
- raw auth payload

secret scan は stdout / stderr / evidence notes / sanitized artifact を対象に行う。

## 5. 実行

deterministic tests:

```bash
dotnet test ExchangeApi.slnx --no-restore --filter Realtime
```

live verification:

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter PrivateRealtime
```

private realtime は注文イベントが発生しないと payload が届かない場合がある。
payload が届かない場合でも、auth / subscribe / cancellation / unsubscribe の secret-free 確認を優先する。
