# v3.5.0 Environment Setup / Scope Framing 実施指示

最終更新: 2026-04-28
位置づけ: v3.5.0 初期環境整備 / scope framing 指示

状態: scope framing

## 1. 目的

v3.5.0 では、v3.4.0 の bitFlyer Realtime resilience foundation の上に、次の realtime maturity work を検討・実装する準備を行う。

本指示では、v3.5.0 の実装 scope をまだ確定しない。
まず release 後の clean な `main` から `codex/v3.5-dev` を作成し、文書・verification・branch baseline を整える。

## 2. 前提

- `v3.4.0` は release 済みである
- `main` は `v3.4.0` release completion commit を含む
- working tree は clean である
- `v3.5.0` の正式 scope は別途裁定して本書へ追記する

## 3. 環境整備 Scope

実施する:

- `main` を最新化する
- `codex/v3.5-dev` branch を `main` から作成する
- `docs/plan-v3.5.0.md` を追加する
- `docs/document-inventory.md` に v3.5 plan を追加する
- `docs/roadmap-post-v2.md` の v3.5 候補を必要最小限更新する
- baseline verification を実行する
- 初期 setup commit を作成して push する

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

## 4. v3.5.0 Scope 候補

候補は次の通り。
採用する場合は、実装前に本書と関連 topic doc を更新する。

- fake transport / replay / sample payload testing helper の optional 化
- sanitized raw frame logging
- diagnostic event schema
- stream replay for test / diagnostics
- realtime lifecycle contract table
- `ExchangeApi.Optional.Reactive`
- `ExchangeApi.Optional.Realtime.Resilience`

判断基準:

- core / venue package の主 API は `IAsyncEnumerable<T>` のまま維持する
- Rx は採用する場合も optional package に限定する
- v3.5.0 では、API event / response を安全に取得・記録・再現・検証するための汎用基盤に限定する
- 取引所ステート管理に直接関係するものは、基盤であっても v4.0.0 以降へ送る
- secret-free rule を維持する
- state-changing operation は v3.5.0 に含めない

## 4.1 v3 / v4 境界

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

この境界により、v3.5.0 の詳細 scope を決める前に、state management へ踏み込む候補を v4.0.0 以降へ送る。

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
