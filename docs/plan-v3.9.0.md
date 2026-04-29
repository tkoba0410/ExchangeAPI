# v3.9.0 Realtime Verification / Foundation Close 実施指示

最終更新: 2026-04-29
位置づけ: v3.9.0 Realtime Verification / Foundation Close preparation 指示

状態: close preparation passed

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

採用:

- 初期方針を採用する。
- `docs/verification.md` は verification policy / release gate / runbook 参照先に留める。
- `docs/realtime-bitflyer.md` は bitFlyer stream / channel / DTO contract を保持する。
- `docs/realtime-diagnostics.md` は diagnostic event vocabulary / raw frame logging / secret-free observability を保持する。
- public / private / resilience の具体的な live verification 手順は `verification/` 配下の runbook に置く。

反映:

- `docs/realtime-bitflyer.md` の目的と関連文書を v3.9 close 方針に合わせた
- `docs/realtime-diagnostics.md` の責務を diagnostic vocabulary / observability に寄せた
- `docs/verification.md` に v3.9 Realtime Foundation Close Verification を追加した
- public / private / resilience runbook の evidence label を v3.9 close 用に更新した

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

採用:

- 初期方針を採用する。
- public / private / resilience の actual live run は opt-in 補助確認に留める。
- v3.9 release gate は deterministic tests、package smoke、live tests の opt-in skip、secret-free rule を必須にする。
- private realtime は credentials 未設定時 safe skip を許容する。
- raw credential profile と raw auth payload は evidence にコピーしない。

反映:

- public realtime runbook に opt-in skip 確認と secret scan example を追加した
- private realtime runbook に credentials 未設定時 safe skip、secret scan、raw auth payload 禁止を明記した
- resilience runbook に actual live run を release gate に必須化しない方針を明記した

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

採用:

- 初期方針を採用する。
- consumer smoke は local / GitHub Packages ともに aggregate venue package を確認する。
- bitFlyer は Realtime foundation surface を持つため、factory / channel vocabulary / stream envelope / replay / reactive adapter を確認する。
- Binance は v3 package consolidation の aggregate venue package として、`BinanceClientFactory` を restore / build / run で確認する。
- optional packages は `Credentials` / `Logging` / `Testing` / `Reactive` を確認する。
- smoke は実 API に接続しない。
- token、credentials、API key、API secret、signature、Authorization 相当値を stdout / stderr に出さない。

反映:

- local consumer smoke に `ExchangeApi.Exchanges.Binance` と `BinanceClientFactory` 確認を追加した
- GitHub Packages consumer smoke に `ExchangeApi.Exchanges.Binance` と `BinanceClientFactory` 確認を追加した
- `docs/local-nuget-consumer.md` に smoke 対象 package / public surface を反映した
- `docs/distribution.md` に v3.9.0 release verification の package smoke 方針を追加した
- `docs/guides/package-publish.md` に v3.9.0 の GitHub Packages consumer smoke 確認対象を追加した

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

採用:

- 初期方針を採用する。
- v3.9.0 は Realtime foundation close に必要な文書、runbook、package smoke、secret-free evidence、release checklist / release notes を閉じる。
- HTTP contract / consumer verification catch-up は v4 stable baseline へ送る。
- Exchange I/O semantics は v5 へ送る。
- new venue onboarding は v6 へ送る。
- Unified は v7+ へ送る。
- state reconstruction / Gateway / Platform behavior は ExchangeAPI に直接持ち込まない。

最終分類:

| 項目 | 分類 | v3.9 action |
| --- | --- | --- |
| Realtime 文書の正本分担 | v3.9で閉じる | `docs/realtime-bitflyer.md` / `docs/realtime-diagnostics.md` / `docs/verification.md` の責務を整理 |
| public realtime live runbook | v3.9で閉じる | opt-in / evidence / secret scan / skip confirmation を明記 |
| private realtime live runbook | v3.9で閉じる | credentials 条件 / safe skip / raw auth payload 禁止 / secret scan を明記 |
| realtime resilience runbook | v3.9で閉じる | deterministic gate / opt-in live / secret-free rule を明記 |
| local consumer smoke | v3.9で閉じる | Bitflyer / Binance / Optional.Credentials / Optional.Logging / Optional.Testing / Optional.Reactive を確認 |
| GitHub Packages consumer smoke | v3.9で閉じる | local smoke と同じ package set を確認 |
| secret-free evidence rule | v3.9で閉じる | runbook / checklist / release notes に反映 |
| release checklist / release notes | v3.9で閉じる | v3.9.0 用文書を追加 |
| HTTP contract / consumer verification catch-up | v4 stable baselineへ送る | v4.0 inventory / maintenance catch-up で扱う |
| ExchangeAPI 全体の stable baseline hardening | v4 stable baselineへ送る | v4.x で扱う |
| Exchange I/O semantics foundation | v5 semanticsへ送る | v5.0 で扱う |
| SymbolSpec / SizeStep / PriceStep / Capability | v5 semanticsへ送る | v5.0 で扱う |
| new venue onboarding | v6 new venueへ送る | v6.0 で扱う |
| Unified | v7+ Unifiedへ送る | 意味同一性を防御できる場合だけ検討 |
| state reconstruction / state replay | やらない / ExchangeAPI外 | Gateway / Platform または別建て上位層で扱う |
| Gateway / Platform behavior | やらない / ExchangeAPI外 | ExchangeAPI は stateless adapter として維持 |

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

採用:

- 初期方針を採用する。
- v3.9.0 では secret scan の script 化をしない。
- secret-free rule は runbook、release checklist、release notes に固定する。
- automated secret scan / evidence hardening は v4 stable baseline の verification hardening 候補に送る。

v3.9.0 で確認する secret-free 対象:

- stdout
- stderr
- local evidence notes
- sanitized artifact
- sanitized raw frame log
- package smoke output
- GitHub Packages smoke output

含めてはいけないもの:

- API key
- API secret
- signature
- Authorization 相当値
- private auth payload
- raw credential profile
- credential file content
- GitHub Packages token

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

反映:

- `docs/release-checklist-v3.9.0.md` を追加した
- `docs/release-notes/v3.9.0.md` を追加した
- `docs/document-inventory.md` に v3.8 / v3.9 checklist と release notes を追加した

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

## 7. v3.9.0 Release Execution 指示

目的:

v3.9.0 を release し、v3 realtime foundation track を閉じる。
v3.9.0 release commit は close preflight 済みの `codex/v3.9-dev` の先端とする。
release 後は v4 stable baseline track へ移るため、`codex/v4.0-dev` を作成する。

前提:

- `codex/v3.9-dev` が clean である
- close preparation が通っている
- actual release preflight を `3.9.0` で実行する
- fast-forward できない場合は止めて差分を確認する
- unrelated change を revert しない

実行:

```bash
bash scripts/run-release-preflight.sh 3.9.0 linux-x64
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check

git checkout main
git pull --ff-only origin main
git merge --ff-only codex/v3.9-dev
git tag -a v3.9.0 -m "Release v3.9.0"
git push origin main
git push origin v3.9.0

bash scripts/push-github-packages.sh 3.9.0
bash scripts/smoke-github-packages-consumer.sh 3.9.0
```

GitHub Release:

- tag: `v3.9.0`
- title: `v3.9.0`
- body: `docs/release-notes/v3.9.0.md`
- assets:
  - `local/publish/release-assets/v3.9.0/exchangeapi-linux-x64`
  - `local/publish/release-assets/v3.9.0/exchangeapi-linux-x64.sha256`
  - `local/publish/release-assets/v3.9.0/exchangeapi-mcp-linux-x64`
  - `local/publish/release-assets/v3.9.0/exchangeapi-mcp-linux-x64.sha256`

release 後:

```bash
git checkout main
git pull --ff-only origin main
git checkout -b codex/v4.0-dev
git push -u origin codex/v4.0-dev
```

確認:

- main に v3.9.0 commit が入っている
- remote に `v3.9.0` tag がある
- GitHub Release が作成されている
- release assets が attach されている
- GitHub Packages publish が通っている
- GitHub Packages consumer smoke が通っている
- `codex/v4.0-dev` が remote にある
- v3.9.0 に新 feature / Binance realtime / Unified / state reconstruction / Gateway / Platform behavior が含まれていない
