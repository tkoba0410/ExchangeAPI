# v3.5.0 Environment Setup 実施指示

最終更新: 2026-04-28
位置づけ: v3.5.0 初期環境整備指示

状態: setup complete

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

- public board snapshot + delta state builder
- private order event state helper
- fake transport / replay / sample payload testing helper の optional 化
- `ExchangeApi.Optional.Reactive`
- `ExchangeApi.Optional.Realtime.Resilience`
- `ExchangeApi.Optional.Realtime.State`

判断基準:

- core / venue package の主 API は `IAsyncEnumerable<T>` のまま維持する
- Rx は採用する場合も optional package に限定する
- state builder は gap-free continuity を保証できない前提を明示する
- secret-free rule を維持する
- state-changing operation は v3.5.0 に含めない

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
