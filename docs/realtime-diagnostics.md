# Realtime Diagnostics

最終更新: 2026-04-28
位置づけ: Realtime diagnostics 設計正本 draft

## 1. 目的

本書は、Realtime API の diagnostic event、sanitized raw frame logging、secret-free evidence layout、lifecycle contract の設計境界を定義する。

v3.5.0 は Realtime Diagnostics Foundation release として扱う。
目的は、realtime stream で何が起きたかを secret-free に見えるようにすることである。

本書は bitFlyer Realtime API の上に最初に適用する。
ただし、内容は将来の venue realtime にも使えるよう、venue-specific DTO や market semantics には踏み込まない。

関連文書:

- [`docs/realtime-bitflyer.md`](./realtime-bitflyer.md)
- [`docs/plan-v3.5.0.md`](./plan-v3.5.0.md)
- [`docs/verification.md`](./verification.md)
- [`verification/bitflyer-realtime-resilience.md`](../verification/bitflyer-realtime-resilience.md)
- [`verification/release-evidence.md`](../verification/release-evidence.md)

## 2. Scope

v3.5.0 で扱う:

- diagnostic event schema
- public `RealtimeDiagnosticEvent` contract
- sanitized raw frame logging
- secret-free realtime evidence layout
- realtime lifecycle contract table
- deterministic tests for diagnostic classification and redaction
- opt-in live verification runbook update

v3.5.0 で扱わない:

- stream replay implementation
- fake transport / scenario helper の optional package 化
- `ExchangeApi.Optional.Reactive`
- `IObservable<T>` public API
- `ExchangeApi.Optional.Realtime.Resilience`
- state builder / state projection
- HTTP + realtime state coordination
- Binance realtime
- venue 横断 realtime abstraction
- Unified realtime abstraction
- order / cancel / deposit / withdraw などの state-changing operation

## 3. Design Principles

- Realtime diagnostics は、stream の観測と説明のための contract であり、取引所ステート管理ではない。
- secret-free rule を最優先する。
- default では file log / evidence を作らない。
- raw frame logging は opt-in とする。
- diagnostic event は machine-readable な構造を持つ。
- diagnostic event は利用者が stream lifecycle を判断するための情報を持つが、reconnect や resubscribe の policy 自体を決めすぎない。
- venue package の主 API は `IAsyncEnumerable<T>` のまま維持する。
- Rx dependency は core / venue package に追加しない。
- diagnostics は public / private realtime の両方に適用できるが、private auth payload や credential を記録しない。

## 4. Diagnostic Event Model

Diagnostic event は、realtime stream lifecycle、message decode、raw frame handling、secret-free logging の状態を表す。
`RealtimeDiagnosticEvent` は public API として扱う。

目的:

- 利用者が stream lifecycle / diagnostics を structured data として扱えるようにする
- logging / evidence / replay / monitoring が同じ診断単位を参照できるようにする
- 将来の venue realtime にも流用できる診断語彙を固定する

一般化する範囲:

- connection lifecycle
- subscription lifecycle
- message decode / reject
- reconnect / resubscribe
- continuity loss
- close / failure
- sanitized raw frame handling

一般化しない範囲:

- ticker / execution / board / order event の market semantics
- order / account / position state
- symbol / product code の横断正規化
- venue 横断 realtime abstraction
- Unified market data / account / trading model

API shape:

```csharp
public sealed record RealtimeDiagnosticEvent
{
    public required string EventType { get; init; }
    public required DateTimeOffset ObservedAt { get; init; }
    public string? Venue { get; init; }
    public string? Channel { get; init; }
    public string? ProductCode { get; init; }
    public string? ConnectionId { get; init; }
    public string? SubscriptionId { get; init; }
    public string? Severity { get; init; }
    public string? Reason { get; init; }
    public string? ErrorKind { get; init; }
    public IReadOnlyDictionary<string, string>? Attributes { get; init; }
}
```

`EventType` / `Severity` の実体は `string` とする。
公式値は constants として提供し、利用者が typo を避けやすい形にする。

候補 API 名:

```csharp
public static class RealtimeDiagnosticEventTypes
{
    public const string Connecting = "Connecting";
    public const string Connected = "Connected";
    public const string SubscribeRequested = "SubscribeRequested";
    public const string Subscribed = "Subscribed";
    public const string RawFrameReceived = "RawFrameReceived";
    public const string RawFrameLogged = "RawFrameLogged";
    public const string MessageDecoded = "MessageDecoded";
    public const string MessageRejected = "MessageRejected";
    public const string ContinuityLost = "ContinuityLost";
    public const string Reconnecting = "Reconnecting";
    public const string Reconnected = "Reconnected";
    public const string Resubscribed = "Resubscribed";
    public const string Closed = "Closed";
    public const string Failed = "Failed";
}

public static class RealtimeDiagnosticSeverities
{
    public const string Trace = "Trace";
    public const string Info = "Info";
    public const string Warning = "Warning";
    public const string Error = "Error";
}
```

`EventType` 候補:

| EventType | 意味 | Stream 継続 |
| --- | --- | --- |
| `Connecting` | WebSocket 接続を開始した | 継続前 |
| `Connected` | WebSocket 接続が成立した | 継続 |
| `SubscribeRequested` | subscribe request を送信した | 継続 |
| `Subscribed` | subscribe が成立した | 継続 |
| `RawFrameReceived` | raw frame を受信した | 継続 |
| `RawFrameLogged` | sanitized raw frame を log / evidence へ記録した | 継続 |
| `MessageDecoded` | payload decode が成功した | 継続 |
| `MessageRejected` | payload decode / validation が失敗し、message を捨てた | 原則継続 |
| `ContinuityLost` | reconnect / resubscribe 等により連続性を保証できない | 継続可能 |
| `Reconnecting` | reconnect を開始した | 継続可能 |
| `Reconnected` | reconnect が成立した | 継続可能 |
| `Resubscribed` | reconnect 後の resubscribe が成立した | 継続可能 |
| `Closed` | stream が正常終了した | 終了 |
| `Failed` | stream が制御された例外で終了した | 終了 |

`Severity` 候補:

| Severity | 意味 |
| --- | --- |
| `Trace` | 通常の低レベル lifecycle |
| `Info` | 利用者が知ってよい通常状態 |
| `Warning` | stream は継続できるが注意が必要 |
| `Error` | stream が終了する、または利用者判断が必要 |

## 5. Stream Continuation Rule

diagnostic event は stream を続けるか終了するかを明確にする。

外部取引所データと内部診断データは、同じ envelope stream 上の別種 event として扱う。
DTO-only stream は既存どおり維持し、market data だけを簡単に読みたい利用者向けの API とする。
運用・診断が必要な利用者は envelope stream を使う。

```text
SubscribeTickerAsync:
  Ticker
  Ticker

SubscribeTickerStreamAsync:
  Connected
  Subscribed
  Ticker
  MessageRejected
  ContinuityLost
  Ticker
```

この設計では、取引所データと診断データを同じ market data として扱わない。
同じ時系列 stream 上の別種 event として扱い、event kind で明確に区別する。

基本方針:

- malformed JSON、unknown channel、DTO decode failure は `MessageRejected` として扱い、原則 stream を継続する。
- reconnect / resubscribe 後は、欠落がない前提を置かず `ContinuityLost` を出す。
- connection close、reconnect exhausted、authentication failure など recovery できない失敗は `Failed` として stream を終了する。
- cancellation / dispose は `Closed` として正常終了する。

利用者判断:

- `MessageRejected` は「その message は捨てたが、stream は継続している」ことを示す。
- `ContinuityLost` は「stream は継続可能だが、event の連続性は保証しない」ことを示す。
- state management 側が必要な場合、`ContinuityLost` を受けて v4 の resync / invalidation policy で処理する。

## 6. Sanitized Raw Frame Logging

raw frame logging は、WebSocket で受信した frame を調査可能な形で残す opt-in 機能である。

記録してよいもの:

- public channel の received raw frame
- private channel の event payload after redaction
- channel name
- received timestamp
- connection id / subscription id
- payload byte length
- decode result
- diagnostic event id

記録してはいけないもの:

- API key
- API secret
- signature
- Authorization 相当の値
- private auth request payload
- raw credential profile
- credential file path の詳細
- exception message に混入した secret

redaction rule:

- `api_key`, `apiKey`, `key`, `api_secret`, `apiSecret`, `secret`, `signature`, `Authorization`, `authorization` は `[REDACTED]` に置換する。
- key 名の大小文字差は redaction 対象にする。
- redaction 不能な raw frame は記録せず、`RawFrameLoggingSkipped` 相当の diagnostic event を残す。

## 7. Evidence Layout

evidence は opt-in only とする。
default の library usage、deterministic tests、package smoke では evidence を作らない。

標準配置:

```text
local/evidence/local-live/<yyyymmdd>-v3.5.0-realtime-diagnostics/
  runtime/
    artifacts/
      diagnostic-events.jsonl
      sanitized-raw-frames.jsonl
    logs/
  notes/
    summary.md
```

JSONL は 1 line 1 JSON object とする。
JSONL writer を使う場合も、secret-free rule を満たす。

`local/evidence/` 配下の run directory は repository の正本ではない。
commit しない。

## 8. Lifecycle Contract Table

| Scenario | Expected Diagnostic Events | Stream Result |
| --- | --- | --- |
| normal public subscribe | `Connecting -> Connected -> SubscribeRequested -> Subscribed -> RawFrameReceived -> MessageDecoded` | data continues |
| malformed JSON frame | `RawFrameReceived -> MessageRejected` | stream continues |
| unknown channel frame | `RawFrameReceived -> MessageRejected` | stream continues |
| DTO decode failure | `RawFrameReceived -> MessageRejected` | stream continues |
| connection closed then reconnect succeeds | `Reconnecting -> Reconnected -> Resubscribed -> ContinuityLost` | stream continues |
| reconnect exhausted | `Reconnecting -> Failed` | stream terminates with controlled exception |
| private auth failure | `Failed` | stream terminates with controlled exception |
| cancellation requested | `Closed` | normal termination |
| dispose called | `Closed` | normal termination |

## 9. Verification

Deterministic tests:

- diagnostic event schema can be serialized to JSON
- lifecycle scenarios emit expected event sequence
- malformed JSON becomes `MessageRejected`
- unknown channel becomes `MessageRejected`
- DTO decode failure becomes `MessageRejected`
- reconnect / resubscribe emits `ContinuityLost`
- cancellation / dispose emits `Closed`
- redaction removes API key / secret / signature / Authorization
- raw private auth payload is not logged
- default configuration creates no file log / evidence

Live verification:

- opt-in only
- short duration
- public realtime can emit diagnostic events without credentials
- private realtime can emit diagnostic events without logging auth payload
- stdout / stderr / logs / evidence are secret-free
- evidence uses the standard layout

## 10. Open Decisions

次の項目は実装前に裁定する。

1. sanitized raw frame logging を core venue package に置くか、`ExchangeApi.Optional.Logging` の extension として置くか。
2. raw frame の body を常に保存するか、size limit と sampling を持つか。

推奨初期案:

- file output は `ExchangeApi.Optional.Logging` 側に寄せ、venue package は event emission までに留める。
- raw frame body は opt-in かつ size limit 付きで保存する。

## 11. Decision Log

### 11.1 Diagnostic Event Stream Placement

採用: 案C。

外部取引所データと内部診断データは、同じ envelope stream に載せる。
ただし、DTO-only stream は維持する。

採用理由:

- data と diagnostic event の時系列関係を保てる
- `ContinuityLost` や `MessageRejected` がどの data の前後で起きたか判断しやすい
- 既存の `Subscribe*StreamAsync` / `BitflyerRealtimeStreamEvent<T>` 方針と整合する
- 別 stream / callback / sink を増やさずに lifecycle を扱える

非採用:

- DTO-only stream だけにして診断データを外へ出さない案
- market data stream と diagnostic stream を分ける案
- callback / sink で diagnostic event を渡す案

### 11.2 Public Diagnostic Event Contract

採用: 案A。

`RealtimeDiagnosticEvent` は public API として扱う。
ただし、この層で一般化するのは stream lifecycle / diagnostics に限定する。

採用理由:

- bitFlyer 内部に閉じると、診断情報が局所化し、logging / evidence / replay / monitoring から扱いにくくなる
- public contract にすることで、上位層や optional package が同じ診断単位を参照できる
- `MessageRejected`、`ContinuityLost`、`Failed` などを利用者が structured data として判断できる
- 将来の venue realtime にも使えるが、Unified や market semantics の抽象化には踏み込まない

非採用:

- bitFlyer-specific stream event の内側に閉じる案
- internal logging / evidence 専用の非公開診断モデルに留める案

### 11.3 Event Type / Severity Representation

採用: 案C。

`EventType` / `Severity` の実体は `string` とする。
公式値は `RealtimeDiagnosticEventTypes` / `RealtimeDiagnosticSeverities` の constants として提供する。

採用理由:

- v3.5.0 の diagnostics foundation 初期では、enum で閉じすぎない方が変更に強い
- JSONL / evidence / future replay と自然に接続できる
- constants を提供することで、利用者の typo と magic string を減らせる
- 将来 venue-specific diagnostic event を足す余地を残せる

非採用:

- `EventType` / `Severity` を enum にする案
- constants なしの自由 string だけにする案
