# ExchangeAPI Post-v2 Roadmap

最終更新: 2026-04-28
位置づけ: post-v2 roadmap

本書は、`v2.0.0` には含めないが、`v2.0.0` 以降に検討する候補を記録する。  
ここにある項目は採用済み正本ではない。実装する場合は、該当する正本文書と migration / release 文書を先に更新する。

## 1. 前向きに検討する候補

| 項目 | 状態 | 理由 | 備考 |
| --- | --- | --- | --- |
| `ExchangeApi.Optional.Logging` | 保留 | core を薄くし、CLI / MCP / live test / bot / local evidence など用途別に適した log writer を作れるため | `BC-V2-022` として記録済み |
| `Unified` 層の実装 | 将来 | 複数 venue 間で意味同一性を保証できる capability だけを載せるため | まず venue-native surface を安定させる |
| optional credentials provider 拡張 | 将来 | `age` 以外の env / keychain / external secret manager へ広げられるため | v2 初手は `PlainText` と `AgeFile` を優先 |
| `ExchangeApi.Optional.Configuration` | 将来候補 | env / config binding / provider factory を adapter 間で共通化できるため | adapter 側の重複が見えてから検討する |
| `ExchangeApi.Optional.Testing` | 将来候補 | live test helper、fake provider、record/replay support を core から外して提供できるため | まず `tests/` と `verification/` 整理を優先 |
| `ExchangeApi.Optional.Resilience` | 将来候補 | retry / backoff / rate limit / circuit breaker を core 正本に入れずに提供できるため | venue・利用者ごとに要件差が大きい |
| `ExchangeApi.Optional.Reactive` | 将来候補 | Realtime stream を `IObservable<T>` として扱いたい利用者向けに Rx integration を core から分離して提供できるため | core / venue package の主 API は `IAsyncEnumerable<T>` のまま維持する |
| evidence 自動整理 | 将来候補 | `local/evidence/` 標準構成へ artifact / log / notes を自動配置できるため | まず標準構成だけ固定する |
| `samples/` directory | 将来候補 | guide 内サンプルが大きくなった場合に、実行可能サンプルとして分離できるため | 早期作成は保守対象を増やす |
| MCP client / human trial CLI | 将来候補 | 人間が MCP server を試す導線を用意できるため | v2 では MCP server 側の read-only surface を優先 |
| venue 単位 package / project consolidation | v3 採用 | 利用者導線を `ExchangeApi.Exchanges.Bitflyer` / `ExchangeApi.Exchanges.Binance` に整理し、package 数を減らすため | v3.0.0 で package consolidation を採用する |
| bitFlyer Realtime API | v3.1 採用候補 | HTTP とは別軸の public market stream を venue-native surface として扱えるため | v3.1.0 は public read MVP に限定する |
| venue 追加 | v4 候補 | v3 で整理した venue 単位 project / package 構造の拡張性を実証するため | まず public read MVP に絞る |

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
v3.2.0: realtime hardening / venue onboarding preparation
v3.3.0: bitFlyer private realtime read MVP candidate
v4.0.0: new venue public read MVP
v4.x: public read coverage expansion
v5.0.0: Unified public read MVP
v5.x: Unified expansion
v6.0.0+: private/account/trading unified capability, only if meaning is defensible
```

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

v3.2.0 は、Realtime hardening と新 venue 追加準備の候補 release とする。
詳細な scope は [`docs/plan-v3.2.0.md`](./plan-v3.2.0.md) に固定する。

候補:

- reconnect / backoff
- resubscribe
- board state builder
- Rx optional integration
- CLI diagnostic command
- venue onboarding guide
- venue project / endpoint module checklist
- endpoint matrix template
- deterministic test template
- safe live read verification template
- package / smoke / docs の再利用性改善
- 追加 venue candidate の比較
- public read MVP に必要な endpoint の棚卸し
- symbol / product code / timestamp / decimal / nullability の差分調査
- endpoint matrix へ `UnifiedCandidate` などの判定欄を追加するか検討
- 追加 venue spike を行う場合も、正式 surface ではなく調査扱いに留める

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

### v4.0.0 候補

v4.0.0 は、新しい取引所を正式追加するフェーズとする。
v4 は既存 API の大掃除ではなく、v3 で整理した venue 構造の拡張性を実証する release として扱う。

v4.0.0 venue 追加 MVP:

- `Vocabulary`
- public read `Protocol`
- public read `Native`
- `Composition` factory
- deterministic tests
- opt-in live read test
- endpoint matrix
- local consumer smoke

private endpoint、order、cancel、withdraw、deposit は v4.0.0 の初期 MVP には含めない。

venue 選定基準:

- public read API が安定している
- authentication が比較的明確
- API docs が機械的に読める
- rate limit / error contract が理解しやすい
- live verification が safe にできる
- state-changing endpoint を後回しにできる
- 日本円ペアや利用想定に合う場合は加点する

### v5.0.0 候補

v5.0.0 は、Unified public read MVP の候補 release とする。
Unified は、v4 で複数 venue の実装経験を得てから設計する。

v5 Unified MVP に載せやすい候補:

- market list / supported market discovery
- ticker / price snapshot
- order book snapshot
- kline / candle
- exchange health / market status

v5 でも避ける候補:

- order placement
- cancel
- withdraw / deposit
- margin / collateral
- account balance の完全統一
- fee / commission の統一

### v6.0.0+ 候補

v6.0.0 以降は、Unified の private / account / trading capability を検討してよい。
ただし、利用者意図、前提条件、副作用、結果解釈、主要エラー分類の意味同一性を防御できる場合だけ扱う。

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
  - live test helper
  - fake credential provider
  - record/replay support
  - sanitized artifact generator
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
