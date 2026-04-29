# v3.9.0 Realtime Verification / Foundation Close 実施指示

最終更新: 2026-04-29
位置づけ: v3.9.0 Realtime Verification / Foundation Close preparation 指示

状態: preparation / scope decision

## 1. 目的

v3.9.0 は、v3.1.0 から v3.8.0 までに実装・整理した Realtime API foundation を閉じる release とする。

目的は、新しい realtime feature を追加することではない。
v3 realtime track を、文書、runbook、package smoke、release checklist、secret-free verification の観点で一度閉じ、v4 の Stable Baseline / Maintenance / Release Hardening へ渡せる状態にする。

v3.9.0 の作業は、整理、修正、仕上げに限定する。

## 2. 大方針

v3.9.0 の判断基準は次を基本とする。

```text
v3.9.0 = Realtime Foundation Close
```

採用する考え方:

- v3.9.0 は release close のための整理・修正・仕上げである
- 実装追加より、正本整理、runbook 整理、検証再現性、release 導線を優先する
- 修正は、v3 realtime foundation を閉じるために必要な小さいズレに限定する
- v4 stable baseline、v5 semantics、v6 new venue、v7+ Unified の先取りをしない
- 裁定が必要な場合は、本書に裁定内容と理由を残してから進める

入れてよい変更:

- docs / verification / scripts / release checklist / release notes の整合修正
- runbook の古い記述や不足の修正
- package smoke / consumer smoke の説明不足修正
- secret-free rule の抜け修正
- v3.8 で分類済みの realtime gap のうち、v3.9 close に必要な最小修正
- deterministic test が露出した contract mismatch の最小修正

入れない変更:

- 新しい realtime channel
- new public API sugar
- Binance realtime
- venue 横断 realtime abstraction
- Unified realtime abstraction
- state reconstruction
- Gateway / Platform behavior
- simulation
- HTTP contract / consumer verification catch-up
- v4 stable baseline 作業そのもの
- v5 Exchange I/O semantics foundation
- v6 new venue onboarding

## 3. 作業順

1. Realtime 文書の正本整理
2. Realtime live verification runbook の仕上げ
3. Package / consumer smoke の仕上げ
4. v3 realtime gap の最終分類
5. Secret-free / evidence の最終確認
6. Release checklist / release notes の追加
7. release preflight
8. release execution

一つずつ進め、裁定が必要な内容は該当 step の前に本書へ追記する。

## 4. 具体作業

### 4.1 Realtime 文書の正本整理

対象:

- [`docs/realtime-bitflyer.md`](./realtime-bitflyer.md)
- [`docs/realtime-diagnostics.md`](./realtime-diagnostics.md)
- [`docs/verification.md`](./verification.md)

確認:

- bitFlyer Realtime の stream / channel / DTO / lifecycle contract は `docs/realtime-bitflyer.md` に置く
- diagnostic vocabulary / secret-free observability / raw frame logging は `docs/realtime-diagnostics.md` に置く
- 実行手順と evidence layout は `verification/` 配下に置く
- 同じ exact contract を複数文書に重複保持しない
- v3.8 で追加した `NonTargetMessageIgnored` の説明と矛盾しない

裁定が必要な可能性:

- `docs/verification.md` に realtime 手順の概要をどこまで置くか
- `docs/realtime-bitflyer.md` と `docs/realtime-diagnostics.md` の重複を削る範囲

初期方針:

- `docs/verification.md` は入口と参照先に留める
- exact realtime contract は `docs/realtime-bitflyer.md` に寄せる
- diagnostic event vocabulary は `docs/realtime-diagnostics.md` に寄せる

### 4.2 Realtime live verification runbook の仕上げ

対象:

- [`verification/bitflyer-realtime-live.md`](../verification/bitflyer-realtime-live.md)
- [`verification/bitflyer-private-realtime-live.md`](../verification/bitflyer-private-realtime-live.md)
- [`verification/bitflyer-realtime-resilience.md`](../verification/bitflyer-realtime-resilience.md)

確認:

- opt-in 条件が明確である
- evidence layout が `local/evidence/` の標準に合っている
- public realtime / private realtime / resilience verification の目的が分かれている
- secret scan 手順がある
- raw credential profile を evidence にコピーしない
- stdout / stderr / logs / evidence に secret を出さない

裁定が必要な可能性:

- live verification を v3.9 release gate に必須化するか、runbook 整備と opt-in skip 確認に留めるか

初期方針:

- live verification は opt-in のままにする
- v3.9 release gate は deterministic / smoke / live skip を必須にし、actual live run は必須にしない

### 4.3 Package / consumer smoke の仕上げ

対象:

- [`scripts/smoke-local-nuget-consumer.sh`](../scripts/smoke-local-nuget-consumer.sh)
- [`scripts/smoke-github-packages-consumer.sh`](../scripts/smoke-github-packages-consumer.sh)
- [`docs/distribution.md`](./distribution.md)
- [`docs/local-nuget-consumer.md`](./local-nuget-consumer.md)
- [`docs/guides/package-publish.md`](./guides/package-publish.md)

確認:

- venue package を確認している
- `ExchangeApi.Optional.Credentials` を確認している
- `ExchangeApi.Optional.Logging` を確認している
- `ExchangeApi.Optional.Testing` を確認している
- `ExchangeApi.Optional.Reactive` を確認している
- token / credentials を stdout / stderr に出さない

裁定が必要な可能性:

- smoke を増やすか、現行 script の確認対象を文書化するだけにするか

初期方針:

- まず現行 smoke script の実態を棚卸しする
- v3.9 close に必要な不足がある場合だけ最小修正する

### 4.4 v3 realtime gap の最終分類

分類:

```text
v3.9 で閉じる
v4 stable baseline へ送る
v5 semantics へ送る
v6 new venue へ送る
v7+ Unified へ送る
やらない
```

確認:

- HTTP catch-up は v4
- stable baseline は v4
- Exchange I/O semantics は v5
- new venue は v6
- Unified は v7+
- state reconstruction は ExchangeAPI 外または上位層

裁定が必要な可能性:

- v3.9 で閉じるべき gap と v4 stable baseline へ送るべき gap の境界

初期方針:

- Realtime foundation だけで完結する文書・runbook・smoke・secret-free の gap は v3.9
- HTTP や ExchangeAPI 全体の安定板化に関わる gap は v4

### 4.5 Secret-free / evidence の最終確認

確認:

- stdout に secret が出ない
- stderr に secret が出ない
- evidence に API key / secret / signature / Authorization / auth payload が出ない
- raw frame logging は sanitized
- private realtime runbook が credential profile をコピーしない
- GitHub Packages token を出さない

裁定が必要な可能性:

- secret scan を script 化するか、runbook 手順に留めるか

初期方針:

- v3.9 では runbook と release checklist に固定する
- script 化は、v4 stable baseline の verification hardening 候補に送る

### 4.6 Release checklist / release notes

追加:

- `docs/release-checklist-v3.9.0.md`
- `docs/release-notes/v3.9.0.md`

記載:

- v3.9.0 は Realtime Foundation Close
- 新機能追加ではない
- v3 realtime track を閉じる
- v4 stable baseline へ渡す
- verification summary
- migration impact はなし、または最小
- secret-free safety note

## 5. Verification

preflight:

```bash
bash scripts/run-release-preflight.sh 3.9.0-local.close linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

release:

```bash
bash scripts/run-release-preflight.sh 3.9.0 linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

## 6. 完了条件

- v3.9.0 の大方針が本書に固定されている
- Realtime 文書の正本分担が明確である
- Realtime live verification runbook が v3.9 close 用に整っている
- local / GitHub Packages consumer smoke の確認対象が明確である
- v3 realtime gap の送り先が明確である
- secret-free / evidence rule が release checklist に反映されている
- `docs/release-checklist-v3.9.0.md` が追加されている
- `docs/release-notes/v3.9.0.md` が追加されている
- deterministic tests が通る
- package generation が通る
- local consumer smoke が通る
- live tests が opt-in なしで skip する
- GitHub Packages consumer smoke が release 後に通る
- v3.9.0 に新 feature / Binance realtime / Unified / state reconstruction / Gateway / Platform behavior が含まれていない
