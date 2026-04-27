# bitFlyer Realtime Live Verification

最終更新: 2026-04-27
位置づけ: bitFlyer Realtime live verification runbook

本書は bitFlyer public Realtime API の opt-in live verification 手順を定義する。
Realtime API の設計正本は [`docs/realtime-bitflyer.md`](../docs/realtime-bitflyer.md) とする。

## 1. Scope

対象:

- public realtime only
- `lightning_ticker_<product_code>`
- `lightning_executions_<product_code>`
- `lightning_board_snapshot_<product_code>`
- `lightning_board_<product_code>`

非対象:

- private realtime
- credentials / auth
- order / cancel / deposit / withdraw
- reconnect / backoff
- full order book state builder

## 2. Preconditions

実行条件:

```text
EXCHANGEAPI_RUN_LIVE_TESTS=1
```

credentials は不要である。
default では live connection を行わない。

## 3. Evidence

証跡を残す場合の標準配置:

```text
local/evidence/local-live/<yyyymmdd>-v3.2.0-bitflyer-realtime/
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

public realtime verification では credentials を使わないため、secret を生成しないことを基本にする。

## 4. Command

```bash
EXCHANGEAPI_RUN_LIVE_TESTS=1 dotnet test ExchangeApi.LiveTests.slnx --no-restore --filter Realtime
```

## 5. Confirmation

確認項目:

- ticker stream が短時間で 1 件以上受信できる
- stream 終了時に client disposal が正常に完了する
- stdout / stderr に secret が出ない
- opt-in なしでは skip する

market 状況に依存する channel は必須 gate にしない。
deterministic tests を release gate とし、live verification は補助確認に留める。
