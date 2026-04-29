# ExchangeAPI Post-v2 Roadmap

最終更新: 2026-04-29
位置づけ: post-v2 roadmap

本書は、`v2.0.0` には含めないが、`v2.0.0` 以降に検討する候補を記録する。  
ここにある項目は採用済み正本ではない。実装する場合は、該当する正本文書と migration / release 文書を先に更新する。

## 1. 前向きに検討する候補

| 項目 | 状態 | 理由 | 備考 |
| --- | --- | --- | --- |
| `ExchangeApi.Optional.Logging` | v2.1 採用 | core を薄くし、CLI / MCP / live test / bot / local evidence など用途別に適した log writer を作れるため | `BC-V2-022` として記録済み |
| `Unified` 層の実装 | 将来 | 複数 venue 間で意味同一性を保証できる capability だけを載せるため | まず venue-native surface を安定させる |
| optional credentials provider 拡張 | 将来 | `age` 以外の env / keychain / external secret manager へ広げられるため | v2 初手は `PlainText` と `AgeFile` を優先 |
| `ExchangeApi.Optional.Configuration` | 将来候補 | env / config binding / provider factory を adapter 間で共通化できるため | adapter 側の重複が見えてから検討する |
| `ExchangeApi.Optional.Testing` | v3.6 採用 | realtime raw frame replay / decode / diagnostic testing helper を core / venue package から外して提供できるため | simulation / Gateway / Platform / Strategy testing へ拡張しない |
| `ExchangeApi.Optional.Resilience` | 将来候補 | retry / backoff / rate limit / circuit breaker を core 正本に入れずに提供できるため | venue・利用者ごとに要件差が大きい |
| `ExchangeApi.Optional.Reactive` | v3.7 採用 | Realtime stream を `IObservable<T>` として扱いたい利用者向けに Rx integration を core から分離して提供できるため | thin generic `ToObservable(...)` adapter に限定し、core / venue package の主 API は `IAsyncEnumerable<T>` のまま維持する |
| evidence 自動整理 | 将来候補 | `local/evidence/` 標準構成へ artifact / log / notes を自動配置できるため | まず標準構成だけ固定する |
| `samples/` directory | 将来候補 | guide 内サンプルが大きくなった場合に、実行可能サンプルとして分離できるため | 早期作成は保守対象を増やす |
| MCP client / human trial CLI | 将来候補 | 人間が MCP server を試す導線を用意できるため | v2 では MCP server 側の read-only surface を優先 |
| venue 単位 package / project consolidation | v3 採用 | 利用者導線を `ExchangeApi.Exchanges.Bitflyer` / `ExchangeApi.Exchanges.Binance` に整理し、package 数を減らすため | v3.0.0 で package consolidation を採用する |
| bitFlyer Realtime API | v3.1 採用 | HTTP とは別軸の public market stream を venue-native surface として扱えるため | v3.x は bitFlyer realtime foundation track として継続する |
| venue 追加 | v6 候補 | v3 / v4 / v5 で realtime foundation、stable baseline、Exchange I/O semantics foundation を整理した後、venue 単位 project / package 構造の拡張性を実証するため | まず public read MVP に絞る |

## 1.1 v2.1.0 採用項目

`v2.1.0` では次を採用する。

- `ExchangeApi.Optional.Logging`
- safe redaction
- evidence directory helper
- MCP read-only inspection tools
  - `get_collateral_accounts`
  - `get_balance_history`
  - `get_collateral_history`
  - `get_child_orders`

`Unified`、`ExchangeApi.Optional.Resilience`、credentials provider 拡張、`samples/`、MCP client / human trial CLI、package / project consolidation は `v2.1.0` では扱わない。

## 1.2 v2.2.0 採用範囲

`v2.2.0` では、`v2.1.0` で追加した logging / evidence / MCP inspection surface を前提に、運用導線と release verification の整理を採用する。
`v2.2.0` は operational / verification release として扱い、新しい大規模機能や破壊的変更は入れない。

採用範囲:

- evidence helper integration
  - scripts / verification から `local/evidence/<phase>/<yyyymmdd>-<label>/` を作成しやすくする
  - default では evidence / log を作らず、opt-in のみとする
  - CLI option は追加しない
- release verification script 整理
  - local package smoke に `ExchangeApi.Optional.Logging` を含める
  - GitHub Packages consumer smoke の手順を script 化する
  - release asset 作成手順を script 化する
- MCP inspection operational runbook
  - private read inspection tools の live verification を再実行しやすくする

`Unified`、`ExchangeApi.Optional.Resilience`、credentials provider 拡張、full MCP client、write operation の MCP tool、package / project consolidation は `v2.2.0` では扱わない。

## 1.3 v3.0.0 方針

`v2.2.0` の次は `v3.0.0` を想定する。

`v3.0.0` の主題候補:

- package / project consolidation
- venue 単位 package 導線の整理
- package 数と利用者導線の見直し

`v3.0.0` では破壊的変更を許容し、論理性・合理性・可読性を優先する。
`v2.2.0` では v3 詳細設計までは行わず、候補を本 roadmap に残す。

## 1.4 v3.0.0 採用範囲

`v3.0.0` では、venue package / project を `ExchangeApi.Exchanges.Bitflyer` / `ExchangeApi.Exchanges.Binance` に集約する。
layer-specific venue package / project は廃止する。

詳細は [`docs/plan-v3.0.0.md`](./plan-v3.0.0.md)、[`docs/breaking-changes-v3.0.0.md`](./breaking-changes-v3.0.0.md)、[`docs/migration-v3.0.0.md`](./migration-v3.0.0.md) を参照する。

## 1.5 v3.x / v4 / v5+ ロードマップ

現時点の大きな流れは次を基本とする。

```text
v3.0.0: package / project consolidation
v3.1.0: bitFlyer public realtime read MVP
v3.2.0: realtime hardening / realtime foundation preparation
v3.3.0: bitFlyer private realtime read MVP candidate
v3.4.0: bitFlyer realtime resilience foundation
v3.5.0: realtime diagnostics foundation
v3.6.0: realtime replay / testing foundation
v3.7.0: realtime optional reactive integration
v3.8.0: realtime foundation inventory / minimal contract hardening
v3.9.0: realtime verification / release close
v4.0.0: stable baseline inventory / maintenance catch-up
v4.x: stable baseline / release hardening
v5.0.0: Exchange I/O semantics foundation MVP
v5.x: Exchange I/O semantics applications
v6.0.0: new venue public read MVP
v6.x: public read coverage expansion
v7.0.0+: Unified, only if meaning is defensible
```

v3.x は、`v3.0.0` で整理した venue package 構造の上に bitFlyer Realtime API を成熟させる track として扱う。
v4.x は、v2 HTTP と v3 Realtime を合わせて ExchangeAPI の一回目の安定板を目指す stable baseline / maintenance / release hardening track として扱う。
v5.x は、取引所 I/O の意味情報、制約、観測情報を reusable に整える Exchange I/O semantics track として扱う。
新 venue 追加は v6.0.0 へ送り、Unified は v7.0.0 以降へ送る。

責務境界:

ExchangeAPI / ExecutionGateway / CTradeBot Platform の責務境界は [`docs/execution-boundary-policy.md`](./execution-boundary-policy.md) を参照する。
v3.x の Realtime API foundation track は維持する。
v4.x では、ExchangeAPI に stateful execution boundary を入れず、v2 HTTP / v3 Realtime の文書、検証、package、smoke、release 導線を安定板として整える。
v5.x 以降では、Gateway、CLI、検証ツール、監視ツールなどが再利用できる stateless exchange I/O semantics surface を整える方針を採る。

v3 / v4 / v5 境界:

```text
v3:
  API event / response を安全に取得・記録・再現・検証するための汎用基盤

v4:
  v2 HTTP / v3 Realtime を合わせて一回目の安定板にするための仕上げ、メンテナンス吸収、release hardening

v5:
  取得した event / response の意味情報、制約、観測情報を reusable に扱うための stateless semantics support
```

v3 系に残すもの:

- realtime transport / subscription lifecycle
- public / private realtime read
- DTO-only stream
- stream envelope
- reconnect / resubscribe lifecycle
- `ContinuityLost` / `MessageRejected` などの stream status event
- diagnostic event schema
- sanitized raw frame logging
- stream replay for test / diagnostics
- fake transport / payload fixture helper
- live verification / evidence helper
- secret-free rule

v4 系へ送るもの:

- HTTP / Realtime の文書整合
- endpoint matrix と実装のズレ修正
- package / local consumer / GitHub Packages smoke の整理
- release checklist / release notes 整備
- secret-free evidence / log の再確認
- deterministic / live skip / smoke の再現性向上
- 古い文書・古い script 名・古い説明の整理
- CTradeBot / ExecutionGateway が使う前提の不足棚卸し
- 安定板化に必要な最小限の API / docs 修正

v5 系へ送るもの:

- SymbolSpec / SizeStep / PriceStep / Capability
- stateless order validation
- error taxonomy 整理
- order response / fill observation DTO の整理
- HTTP + realtime を組み合わせやすい read surface
- upper layer が inquiry / reconcile しやすい read API 整備
- secret-free audit / evidence 連携
- state freshness / partial failure の観測 contract
- state reconstruction を行う上位層へ渡す observation contract

v4 系以降でも ExchangeAPI に入れないもの:

- `clientOrderKey` 正本管理
- retry / reconcile の正本
- open order tracking
- execution state machine
- ledger / position / allocation
- Bot 固有 SAFE MODE 判断

用語の使い分け:

- v3 の replay は、test / diagnostics のために stream event や raw frame を再生することを指す
- v5 以降の state replay / state reconstruction は、ExchangeAPI 内ではなく Gateway / Platform 側または別建ての上位層で扱う

### v3.1.0 候補

v3.1.0 は、bitFlyer Realtime API の public market read MVP を目的とする。
Realtime API は HTTP endpoint とは別 transport / interaction model として扱う。

候補:

- `docs/realtime-bitflyer.md` の正本化
- JSON-RPC 2.0 over WebSocket
- `IAsyncEnumerable<T>` based typed stream
- `lightning_ticker_<product_code>`
- `lightning_executions_<product_code>`
- `lightning_board_snapshot_<product_code>`
- `lightning_board_<product_code>`
- venue-specific DTO
- opt-in public realtime live verification

v3.1.0 では扱わない:

- private realtime
- Binance realtime
- automatic reconnect / backoff
- full order book state builder
- Reactive Extensions dependency
- `IObservable<T>` public API
- CLI / MCP の本格 integration
- Unified realtime abstraction

### v3.2.0 候補

v3.2.0 は、Realtime hardening と realtime foundation 整理の候補 release とする。
詳細な scope は [`docs/plan-v3.2.0.md`](./plan-v3.2.0.md) に固定する。

候補:

- reconnect / backoff
- resubscribe
- Rx optional integration
- CLI diagnostic command
- deterministic test template
- safe live read verification template
- package / smoke / docs の再利用性改善
- endpoint matrix へ `UnifiedCandidate` などの判定欄を追加するか検討

現行の v3 / v4 / v5 / v6 境界では、board state builder は ExchangeAPI 外または v5+ の semantics 検討、venue onboarding は v6 系へ送る。

### v3.3.0 候補

v3.3.0 は、bitFlyer private realtime read MVP の候補 release とする。
詳細な scope は [`docs/plan-v3.3.0.md`](./plan-v3.3.0.md) に固定する。

候補:

- private realtime auth design
- credential session を使う realtime auth payload signing
- private channel catalog の最小固定
- private event DTO
- typed stream API
- deterministic auth request shape tests
- deterministic private event decode tests
- opt-in private realtime live verification runbook
- secret-free evidence / log / stdout / stderr rule

v3.3.0 では扱わない:

- state-changing operation
- Binance realtime
- Unified realtime abstraction
- reconnect / backoff / resubscribe の本格実装
- Rx dependency の core / venue package 追加
- `IObservable<T>` public API
- CLI / MCP 本格 integration

Rx integration は `ExchangeApi.Optional.Reactive` などの optional package 候補として残す。
導入する場合も、venue DTO と `IAsyncEnumerable<T>` contract を主 API として維持し、Rx は extension / adapter に限定する。

### v3.4.0 候補

v3.4.0 は、bitFlyer Realtime API の resilience foundation release 候補とする。
詳細な scope は [`docs/plan-v3.4.0.md`](./plan-v3.4.0.md) に固定する。

候補:

- realtime reconnect / backoff / resubscribe
- private realtime auth 再実行方針
- idle timeout / heartbeat
- fake transport / replay / sample payload testing helper
- secret-free live verification helper

v3.4.0 では扱わない:

- public board snapshot + delta state builder
- private order event state helper
- `ExchangeApi.Optional.Reactive`
- `ExchangeApi.Optional.Realtime.State`
- Binance realtime
- venue 横断 realtime abstraction
- Unified realtime abstraction
- state-changing realtime operation
- core / venue package への Rx dependency 追加

### v3.5.0 採用範囲

v3.5.0 は、Realtime Diagnostics Foundation release とする。
目的は、realtime stream で何が起きたかを secret-free に見えるようにすることである。
v3.5.0 の実施指示は [`docs/plan-v3.5.0.md`](./plan-v3.5.0.md)、詳細仕様は [`docs/realtime-diagnostics.md`](./realtime-diagnostics.md) に固定する。

採用範囲:

- diagnostic event schema
- sanitized raw frame logging
- secret-free realtime evidence layout
- realtime lifecycle contract table

v3.5.0 では、replay や Rx integration へ深く入らない。
まず「記録する」「説明できる」基盤を優先する。

### v3.6.0 採用範囲

v3.6.0 は、Realtime Replay / Testing Foundation release とする。
目的は、ExchangeAPI の realtime replay / decode / diagnostic testing を最適化することである。
v3.6.0 の実施指示は [`docs/plan-v3.6.0.md`](./plan-v3.6.0.md) に固定する。

採用範囲:

- `ExchangeApi.Optional.Testing`
- raw frame replay for test / diagnostics
- fake transport / payload fixture helper
- sample payload catalog
- deterministic replay tests

v3.6.0 の replay は、state reconstruction ではなく、raw frame の test / diagnostics 用 replay に限定する。
sample payload catalog は scenario catalog ではない。
`ExchangeApi.Optional.Testing` は simulation / Gateway / Platform / Strategy testing へ拡張しない。

### v3.7.0 採用範囲

v3.7.0 は、Realtime Optional Reactive Integration release とする。
目的は、core / venue package を太らせず、利用者が realtime stream を Rx で扱える optional consumer adapter を提供することである。
v3.7.0 の実施指示は [`docs/plan-v3.7.0.md`](./plan-v3.7.0.md) に固定する。

採用範囲:

- `ExchangeApi.Optional.Reactive`
- `IAsyncEnumerable<T>` to `IObservable<T>` adapter
- thin generic `ToObservable(...)`
- Rx consumer smoke
- optional package docs

core / venue package の主 API は `IAsyncEnumerable<T>` のまま維持する。
Rx dependency は optional package に限定する。
v3.7.0 では reconnect / backoff / retry の正本や Gateway / Platform behavior testing は扱わない。
venue-specific helper、envelope-specific helper、scheduler / buffer / retry policy は追加しない。
`ExchangeApi.Optional.Testing` の replay helper と `ExchangeApi.Optional.Reactive` は別責務とし、相互依存を必須にしない。

### v3.8.0 候補

v3.8.0 は、Realtime Foundation Inventory / Minimal Contract Hardening release 候補とする。
目的は、v3.7.0 までに実装した realtime foundation を棚卸しし、v3 realtime track を閉じるために必要な最小 contract hardening を行うことである。

併せて、後続の開発者や自動化エージェントが自律的に仕様確認・差分判断・検証再現を行えるように、decision / rationale、test gap、fixture rule、verification path を明文化する。
これは Codex 自律性向上を含むが、Codex 固有の仕組みにはしない。
一般化した development autonomy / 開発自律性として扱う。

候補:

- realtime foundation inventory
- lifecycle event contract
- stream status event contract
- malformed payload handling
- cancellation / completion / fault behavior
- reconnect / resubscribe contract tests
- realtime error taxonomy の最小整理
- sample payload catalog rule
- decision / rationale ledger
- realtime test gap list
- v4.0 / v5.0 / v6.0 へ送る項目の分類

v3.8.0 では、棚卸しは v3 realtime foundation 全体に対して行うが、修正は v3.7.0 までに入った realtime surface の契約確認に必要な最小範囲へ限定する。

v3.8.0 では扱わない:

- HTTP contract / consumer verification catch-up
- CTradeBot 固有導線
- Gateway / Platform testing
- simulation
- state reconstruction
- broader consumer verification framework
- v4 の stable baseline / maintenance / release hardening
- v5 の Exchange I/O semantics foundation
- v6 の new venue onboarding

### v3.9.0 候補

v3.9.0 は、Realtime Verification / Release Close release 候補とする。
目的は、v3 realtime foundation を閉じ、v4 の stable baseline / maintenance / release hardening へ進める状態にすることである。

候補:

- public / private realtime live verification runbook 強化
- secret scan 手順
- release checklist
- release notes
- package smoke
- docs consolidation
- `ExchangeApi.Optional.Testing` / `ExchangeApi.Optional.Reactive` の package smoke 整理
- realtime live verification / evidence runbook 整理
- v4 へ渡す項目の明文化

v3.9.0 では、v4 の安定板化へ進む前に、v3 realtime foundation の文書、検証、release 導線を閉じる。

v3 系で急がない ExchangeAPI 拡張:

- Binance realtime
- venue 横断 realtime abstraction
- Unified realtime abstraction
- state-changing realtime operation
- core / venue package への Rx dependency 追加

ExchangeAPI 内に直接持ち込まないもの:

- realtime-only local state projection
- HTTP-only state snapshot helper
- HTTP + realtime state coordination
- board / account / position / order state helper
- state replay / state reconstruction

これらは ExecutionGateway / CTradeBot Platform または別建ての上位層で扱い、ExchangeAPI は v5 以降で、そのために再利用できる stateless semantics / observation / inquiry surface を提供する。

### v4.0.0 候補

v4.0.0 は、Stable Baseline Inventory / Maintenance Catch-up release 候補とする。
目的は、v2 HTTP / v3 Realtime で残った contract・verification・consumer usability・release 運用の不足を棚卸しし、ExchangeAPI の一回目の安定板へ進む前提を整えることである。
ただし、[`docs/execution-boundary-policy.md`](./execution-boundary-policy.md) に従い、ExchangeAPI は stateless exchange I/O library として維持する。
`clientOrderKey` 正本管理、retry / reconcile、open order tracking、execution state machine、ledger / position / allocation は ExchangeAPI の責務にしない。

v4.0.0 候補:

- HTTP contract / consumer verification catch-up
- Realtime foundation から stable baseline へ渡す gap の確認
- endpoint matrix / docs / tests / scripts の整合確認
- package consumer smoke の不足確認
- secret-free / evidence / verification 運用の不足確認
- CTradeBot / ExecutionGateway が使う前提の不足整理
- v4.x で直すもの、v5 へ送るもの、v6 へ送るもの、やらないものの分類

v4.0.0 では破壊的変更を許容する。
ただし目的は stable baseline 化、catch-up 整合、contract clarity、verification reproducibility、release hardening に限定する。

v4.0.0 で避ける破壊的変更:

- namespace 全面 rename
- DTO 全面再設計
- factory API 大改修
- Unified 前提の抽象化
- Gateway / Platform 固有都合の API 変更
- 新 venue 追加と同時の大規模整理

v4.0.0 では、state-changing operation を追加しない。
注文、キャンセル、入金、出金などの実行系 operation は別途裁定する。
state reconstruction 自体は ExchangeAPI 内では実装せず、ExecutionGateway / CTradeBot Platform または別建ての上位層で扱う。

### v4.x 候補

v4.x は、Stable Baseline / Maintenance / Release Hardening track とする。
v4.0.0 の棚卸しを前提に、v2 HTTP と v3 Realtime を合わせて ExchangeAPI の一回目の安定板にする。
後から見つかったメンテナンス項目は、stable baseline 化に必要なものに限り v4.x で吸収してよい。

v4.x 候補:

- HTTP / Realtime の文書整合
- endpoint matrix と実装のズレ修正
- deterministic / live skip / smoke の再現性向上
- package / local consumer / GitHub Packages smoke の整理
- release checklist / release notes 整備
- secret-free evidence / log / stdout / stderr の再確認
- 古い文書・古い script 名・古い説明の整理
- CTradeBot / ExecutionGateway が使う前提の不足棚卸し
- 安定板化に必要な最小限の API / docs 修正

v4.x では、Exchange I/O semantics foundation、新 venue、Unified には入らない。

### v5.0.0 候補

v5.0.0 は、Exchange I/O semantics foundation MVP release 候補とする。
v5.0.0 は、v4 stable baseline を前提に、v3 で整理した realtime foundation と既存 HTTP read surface を使って、取引所 I/O の意味情報、制約、観測情報を reusable に整理する release として扱う。

v5.0.0 候補:

- SymbolSpec / SizeStep / PriceStep / Capability
- stateless order validation
- error taxonomy 整理
- order response / fill observation DTO の整理
- HTTP + realtime を組み合わせやすい read surface
- upper layer が inquiry / reconcile しやすい read API 整備
- secret-free audit / evidence 連携
- state freshness / partial failure の観測 contract
- state reconstruction を行う上位層へ渡す observation contract

v5.0.0 では、state-changing operation を追加しない。
注文、キャンセル、入金、出金などの実行系 operation は別途裁定する。
state reconstruction 自体は ExchangeAPI 内では実装せず、ExecutionGateway / CTradeBot Platform または別建ての上位層で扱う。

### v6.0.0 候補

v6.0.0 は、新しい取引所を正式追加するフェーズとする。
v6 は既存 API の大掃除ではなく、v3 / v4 / v5 で整理した venue 構造、realtime foundation、stable baseline、Exchange I/O semantics foundation の拡張性を実証する release として扱う。

v6.0.0 venue 追加 MVP:

- `Vocabulary`
- public read `Protocol`
- public read `Native`
- `Composition` factory
- deterministic tests
- opt-in live read test
- endpoint matrix
- local consumer smoke

private endpoint、order、cancel、withdraw、deposit は v6.0.0 の初期 MVP には含めない。

venue 選定基準:

- public read API が安定している
- authentication が比較的明確
- API docs が機械的に読める
- rate limit / error contract が理解しやすい
- live verification が safe にできる
- state-changing endpoint を後回しにできる
- 日本円ペアや利用想定に合う場合は加点する

### v7.0.0+ 候補

v7.0.0 以降は、Unified を検討してよい。
Unified は、複数 venue の実装経験と state management の境界を得てから設計する。

Unified public read MVP に載せやすい候補:

- market list / supported market discovery
- ticker / price snapshot
- order book snapshot
- kline / candle
- exchange health / market status

v6 でも避ける候補:

- order placement
- cancel
- withdraw / deposit
- margin / collateral
- account balance の完全統一
- fee / commission の統一

Unified の private / account / trading capability は、利用者意図、前提条件、副作用、結果解釈、主要エラー分類の意味同一性を防御できる場合だけ扱う。

候補:

- private read Unified
- account snapshot Unified
- trading capability Unified

これらは venue ごとの差が大きいため、version ありきで採用しない。
意味同一性を説明できない capability は `Native` に留める。

## 2. optional project 候補

`optional` は、core 正本に入れると責務が太るが、実用上あると便利な具体実装を置く場所とする。

候補:

- `ExchangeApi.Optional.Credentials`
  - `PlainTextApiCredentialProvider`
  - `AgeFileApiCredentialProvider`
  - future: environment / keychain / external secret manager provider
- `ExchangeApi.Optional.Logging`
  - JSONL log writer
  - file log writer
  - redaction helper
  - local evidence writer
  - human-readable log writer
- `ExchangeApi.Optional.Configuration`
  - environment binding
  - config file binding
  - provider factory
  - adapter-shared config loader
- `ExchangeApi.Optional.Testing`
  - realtime raw frame replay helper
  - payload fixture helper
  - decode / diagnostic testing helper
  - secret-free fixture validation
  - simulation / Gateway / Platform / Strategy testing は含めない
- `ExchangeApi.Optional.Resilience`
  - retry policy
  - backoff policy
  - rate limit helper
  - circuit breaker integration

## 3. optional に入れないもの

以下は core contract または exact contract に近いため、optional に逃がさない。

- `CallResult`
- `CallError`
- `CallMeta`
- `ProtocolRequest`
- `ProtocolResponse`
- endpoint request / response DTO
- endpoint module
- client factory
- endpoint matrix metadata
- `CallError.Kind` taxonomy

## 4. 基本やらない寄りの項目

次の項目は、現時点では再検討候補というより却下寄りである。

| 項目 | 理由 |
| --- | --- |
| CLI / MCP surface の 1:1 統一 | CLI は endpoint inspection / execution、MCP は bot-oriented tool surface で役割が異なるため |
| test taxonomy / project layout の大規模再編 | 現行の `Architecture / Protocol / Native / Composition / Live / Adapter` 分離が成立しているため |
| `CallError.Kind` taxonomy 再分類 | 現行の `Transport / Http / Codec / Semantic / Mapping` を維持する方が migration risk が小さいため |
| scalar / nullability / enum の横断再設計 | endpoint ごとの exact contract で個別に扱う方が安全なため |

## 5. 運用ルール

- 本書の項目は `v2.0.0` の実装対象に含めない
- 採用する場合は、対象の正本文書へ移してから実装する
- optional project を増やす場合は、core から参照しない依存方向を維持する
- optional project は便利実装であり、core contract の代替正本にしない
