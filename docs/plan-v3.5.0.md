# v3.5.0 Realtime Diagnostics Foundation 実施指示

最終更新: 2026-04-28
位置づけ: v3.5.0 Realtime Diagnostics Foundation 実施指示

状態: implementation-ready scope

## 1. 目的

v3.5.0 では、v3.4.0 の bitFlyer Realtime resilience foundation の上に、Realtime Diagnostics Foundation を実装する。
目的は、realtime stream で何が起きたかを secret-free に見えるようにすることである。

v3.5.0 の詳細仕様は [`docs/realtime-diagnostics.md`](./realtime-diagnostics.md) を正本とする。

## 2. 前提

- `v3.4.0` は release 済みである
- `main` は `v3.4.0` release completion commit を含む
- working tree は clean である
- `docs/realtime-diagnostics.md` の `Open Decisions` に必須未裁定項目はない

## 3. 環境整備 Scope

実施する:

- `main` を最新化する
- `codex/v3.5-dev` branch を `main` から作成する
- `docs/plan-v3.5.0.md` を追加する
- `docs/document-inventory.md` に v3.5 plan を追加する
- `docs/roadmap-post-v2.md` の v3.5 候補を必要最小限更新する
- baseline verification を実行する
- 初期 setup commit を作成して push する

setup は完了済みである。

実施しない:

- public board snapshot + delta state builder の実装
- private order event state helper の実装
- `ExchangeApi.Optional.Reactive` の実装
- `ExchangeApi.Optional.Realtime.State` の実装
- `ExchangeApi.Optional.Realtime.Resilience` の実装
- Binance realtime
- venue 横断 realtime abstraction
- Unified realtime abstraction
- state-changing realtime operation
- core / venue package への Rx dependency 追加

## 4. v3.5.0 Scope

v3.5.0 は、Realtime Diagnostics Foundation release とする。
目的は、realtime stream で何が起きたかを secret-free に見えるようにすることである。

v3.5.0 scope:

- diagnostic event schema
- sanitized raw frame logging
- secret-free realtime evidence layout
- realtime lifecycle contract table

詳細仕様は [`docs/realtime-diagnostics.md`](./realtime-diagnostics.md) に置く。

判断基準:

- core / venue package の主 API は `IAsyncEnumerable<T>` のまま維持する
- v3.5.0 では replay や Rx integration へ深く入らない
- v3.5.0 では、API event / response を安全に取得・記録・再現・検証するための汎用基盤に限定する
- 取引所ステート管理に直接関係するものは、基盤であっても v4.0.0 以降へ送る
- secret-free rule を維持する
- state-changing operation は v3.5.0 に含めない

## 4.1 v3.6.0 - v3.8.0 候補

v3.5.0 以降、v4.0.0 未満の段階化は次を基本とする。
各 release の正式 scope は、対象 release の plan 文書で固定する。

```text
v3.5.0: realtime diagnostics foundation
v3.6.0: realtime replay / testing foundation
v3.7.0: realtime optional integration
v3.8.0: realtime verification / release hardening
```

v3.6.0 候補:

- stream replay for test / diagnostics
- fake transport / scenario helper
- sample payload catalog
- deterministic replay tests

v3.7.0 候補:

- `ExchangeApi.Optional.Reactive`
- `ExchangeApi.Optional.Realtime.Resilience`
- `IAsyncEnumerable<T>` to `IObservable<T>` adapter
- optional retry / backoff helper

v3.8.0 候補:

- public / private realtime live verification runbook 強化
- secret scan 手順
- release checklist
- package smoke
- docs consolidation

## 4.2 v3 / v4 境界

v3 系は Realtime API foundation track とする。
v4 系は Exchange State Management foundation / application track とする。

境界:

```text
v3:
  API event / response を安全に取得・記録・再現・検証するための汎用基盤

v4:
  取得した event / response から取引所ステートを構築・管理するための基盤と応用
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
- fake transport / scenario helper
- live verification / evidence helper
- secret-free rule

v4 系へ送るもの:

- realtime-only local state projection
- HTTP-only state snapshot helper
- HTTP + realtime state coordination
- board / account / position / order state helper
- event history as state
- state replay / state reconstruction
- resync policy
- state invalidation policy
- state freshness / partial failure contract
- bot-oriented live state helper

用語の使い分け:

- v3 の replay は、test / diagnostics のために stream event や raw frame を再生することを指す
- v4 の replay は、取引所ステートを再構築する state replay / state reconstruction を指す

この境界により、state management へ踏み込む候補は v4.0.0 以降へ送る。

## 4.3 文書修正指示

目的:

- `docs/realtime-diagnostics.md` を draft から v3.5.0 実装前の設計正本へ昇格する
- `docs/plan-v3.5.0.md` を scope framing から implementation-ready scope へ更新する
- 文書棚卸しを正本状態に合わせる

更新対象:

- [`docs/plan-v3.5.0.md`](./plan-v3.5.0.md)
- [`docs/realtime-diagnostics.md`](./realtime-diagnostics.md)
- [`docs/document-inventory.md`](./document-inventory.md)

反映内容:

- v3.5.0 scope は Realtime Diagnostics Foundation として確定する
- `docs/realtime-diagnostics.md` は設計正本として扱う
- `Open Decisions` に必須未裁定項目がないことを明記する
- HTTP 側は v3.5.0 で public API / 実装を変更しない
- `replay`、Rx、state management、Binance realtime、Unified、state-changing operation は v3.5.0 に含めない

verification:

```bash
git diff --check
```

docs-only のため、文書修正指示の完了確認では `dotnet test` は必須にしない。

## 4.4 実装指示

目的:

- `docs/realtime-diagnostics.md` に従い、v3.5.0 Realtime Diagnostics Foundation を実装する
- 既存 DTO-only stream は維持する
- envelope stream に public diagnostic event を追加する
- default では file log / evidence を作らない

実装 scope:

- `RealtimeDiagnosticEvent`
- `RealtimeDiagnosticEventTypes`
- `RealtimeDiagnosticSeverities`
- `BitflyerRealtimeDiagnostic<T>`
- lifecycle / message decode / reconnect の diagnostic event emission
- raw frame metadata の observability source
- deterministic tests

実装しない:

- file output / evidence writer の venue package 実装
- HTTP 側 public API / 実装修正
- stream replay implementation
- fake transport / scenario helper の optional package 化
- `ExchangeApi.Optional.Reactive`
- `IObservable<T>` public API
- `ExchangeApi.Optional.Realtime.Resilience`
- state builder / state projection
- Binance realtime
- Unified realtime abstraction
- state-changing operation

実装方針:

- core / venue package は observability event / source emission までを担当する
- JSONL / file / evidence output は `ExchangeApi.Optional.Logging` 側の責務として残す
- raw frame body 保存は opt-in + per-frame size limit の契約だけを実装対象にし、file rotation / sampling / channel filtering は扱わない
- private auth payload は保存対象にしない

verification:

```bash
dotnet test ExchangeApi.slnx --no-restore --filter Realtime
dotnet test ExchangeApi.slnx --no-restore --filter Optional.Logging
git diff --check
```

## 5. 環境整備手順

```bash
git checkout main
git pull --ff-only origin main
git checkout -b codex/v3.5-dev

dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
dotnet test ExchangeApi.LiveTests.slnx --no-restore

git status --short --branch
```

push:

```bash
git push -u origin codex/v3.5-dev
```

## 6. 完了条件

- `codex/v3.5-dev` が remote に存在する
- `docs/plan-v3.5.0.md` が追加されている
- `docs/document-inventory.md` が v3.5 plan を参照している
- `docs/roadmap-post-v2.md` が v3.5 候補を保持している
- `docs/realtime-diagnostics.md` が v3.5.0 Realtime Diagnostics の設計正本として維持されている
- `docs/realtime-diagnostics.md` の必須未裁定項目がない
- deterministic tests が通る
- live tests が opt-in なしで skip する
- working tree が clean である

## 7. Setup Result

```text
date: 2026-04-28
base branch: main
working branch: codex/v3.5-dev
base commit: 131d5771 Record v3.4 release completion
build: dotnet build ExchangeApi.slnx passed
deterministic tests: dotnet test ExchangeApi.slnx --no-restore passed
live tests without opt-in: dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
```

## 8. Implementation Result

```text
date: 2026-04-28
scope: Realtime Diagnostics Foundation
implementation:
  - RealtimeDiagnosticEvent public contract added
  - RealtimeDiagnosticEventTypes / RealtimeDiagnosticSeverities constants added
  - BitflyerRealtimeDiagnostic<T> envelope event added
  - public / private bitFlyer realtime stream emits diagnostic events
  - channel message carries raw frame metadata
  - Optional.Logging realtime raw frame log record factory added
  - raw frame body logging defaults to disabled
  - per-frame maxRawFrameBodyBytes support added
  - oversized / disabled / redaction-failed body is skipped without truncation
verification:
  - dotnet test ExchangeApi.slnx --no-restore --filter "Realtime|Optional.Logging" passed
  - dotnet test ExchangeApi.slnx --no-restore passed
  - dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
  - git diff --check passed
```

## 9. Close Preparation 指示

目的:

- v3.5.0 の release close 前に必要な release 文書と local preflight を完了する
- GitHub Packages publish / GitHub Release / tag 作成の前段までを整える

実施する:

- `docs/release-checklist-v3.5.0.md` を追加する
- `docs/release-notes/v3.5.0.md` を追加する
- `docs/document-inventory.md` に v3.5 release 文書を登録する
- local consumer smoke で realtime diagnostics surface を確認する
- local release-candidate preflight を実行する
- live tests が opt-in なしで skip することを確認する

実施しない:

- `main` への merge
- `v3.5.0` tag 作成
- GitHub Packages publish
- GitHub Release 作成

verification:

```bash
bash scripts/run-release-preflight.sh 3.5.0-local.preflight linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```
