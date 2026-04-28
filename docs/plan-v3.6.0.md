# v3.6.0 Realtime Replay / Testing Foundation 実施指示

最終更新: 2026-04-28
位置づけ: v3.6.0 Realtime Replay / Testing Foundation 実施指示

状態: implementation-ready scope

## 1. 目的

v3.6.0 は、v3.5.0 で追加した Realtime Diagnostics Foundation の上に、Realtime Replay / Testing Foundation を実装する release とする。

v3.6.0 の中心は `ExchangeApi.Optional.Testing` MVP である。
これは ExchangeAPI 層内で閉じる testing utility とし、bitFlyer public realtime raw frame replay / decode / diagnostic testing の最適化に限定する。
simulation、Gateway / Platform behavior testing、strategy logic testing は含めない。

## 2. 前提

- `v3.5.0` は release 済みである
- `main` は `v3.5.0` release completion commit を含む
- `codex/v3.6-dev` branch は `main` から作成する
- v3.x は Realtime API foundation track として継続する
- v4.x は exchange state management foundation / application track として扱う

## 3. 初期候補

v3.6.0 候補:

- stream replay for test / diagnostics
- fake transport / scenario helper
- sample payload catalog
- deterministic replay tests
- replay 入力形式の整理
- v3.5 realtime diagnostic event / raw frame log helper との接続整理

## 4. 初期非対象

v3.6.0 の方針決定前でも、次は初期非対象として扱う。

- exchange state reconstruction
- order book state builder
- private order state builder
- HTTP / Realtime をまたぐ取引所ステート管理
- state replay / state projection
- `System.Reactive` dependency の追加
- `IObservable<T>` public API
- Binance realtime
- new venue implementation
- Unified
- state-changing operation

## 5. 裁定が必要な項目

方針決定では、少なくとも次を裁定する。

- replay 対象を raw frame、diagnostic event、typed stream event のどこに置くか
- replay helper を production library surface に置くか、tests / verification 専用に置くか
- sample payload catalog を repository 正本として持つか
- replay 入力形式を JSONL に寄せるか、test fixture object に寄せるか
- Optional.Logging の raw frame log record と replay input の関係
- public API を追加するか、internal / test helper に留めるか
- live captured evidence を deterministic tests に使う範囲

裁定済み:

- v3.6.0 の replay 主対象は raw frame とする
- diagnostic event は replay 結果の assertion / timeline 検証の補助対象とする
- typed stream event replay は v3.6.0 の主対象にしない
- v3.6.0 では `ExchangeApi.Optional.Testing` を小さく追加する
- `ExchangeApi.Optional.Testing` は ExchangeAPI 層内で閉じる testing utility とする
- `ExchangeApi.Optional.Testing` の拡張性は ExchangeAPI の replay / decode / diagnostic testing 改善に限定する
- `ExchangeApi.Optional.Testing` を simulation framework、Gateway / Platform behavior testing、strategy logic testing へ拡張してはならない
- sample payload catalog は repository 正本として持つ
- sample payload catalog は ExchangeAPI replay / decode / diagnostic testing 用の payload catalog に限定する
- sample payload catalog を simulation / strategy / Gateway / Platform scenario catalog として扱ってはならない
- v3.6.0 の replay 入力形式は fixture-first とする
- 主入力は repository 正本の sample payload fixture file と in-memory `RealtimeReplayFrame` とする
- JSONL は `ExchangeApi.Optional.Logging` / evidence との接続候補として残すが、v3.6.0 の主入力形式にしない
- simulation scenario format は扱わない
- `RealtimeReplayFrame` を replay input の主モデルとする
- `ExchangeApi.Optional.Logging` の raw frame log record は replay input の正本にしない
- `ExchangeApi.Optional.Testing` は `ExchangeApi.Optional.Logging` に直接依存しない
- `ExchangeApi.Optional.Logging` / evidence から `RealtimeReplayFrame` への変換は将来候補または verification helper に留める
- JSONL log replay は v3.6.0 の主機能にしない
- `ExchangeApi.Optional.Testing` には最小 public API を追加する
- public API は ExchangeAPI realtime raw frame replay / decode / diagnostic testing に必要なものに限定する
- internal helper は許容するが、package として使う入口は public にする
- simulation / scenario / Gateway / Platform / Strategy testing API は追加しない
- live captured evidence は deterministic tests の直接入力にしない
- live captured evidence は sanitized / minimized / reviewed fixture の元材料としてのみ使う
- deterministic tests は repository 正本の sample payload fixture にのみ依存する
- `local/evidence/` 配下の raw artifact は commit しない
- credential / signature / auth / account detail を含む payload は fixture 化しない

理由:

- Realtime API の入力源は WebSocket frame であり、raw frame を起点にすると通常の parse / channel routing / DTO decode / diagnostic event path を再検証できる
- v3.5.0 の raw frame logging / diagnostic event foundation と自然につながる
- typed stream event replay だけでは parser / decoder の検証にならない
- diagnostic event replay だけでは取引所 payload の decode 検証にならない
- state reconstruction には踏み込まないため、v4 の exchange state management track との境界を維持できる
- `ExchangeApi.Optional.Testing` を optional package として分離すると、core / venue package を太らせず、testing helper を再利用できる
- ただし simulation / Gateway / Platform / Strategy testing まで含めると責務が重複し、最適化対象が混ざる
- sample payload catalog は raw frame replay の deterministic input として必要であり、decode regression / diagnostic regression の保守性を上げる
- payload catalog と scenario catalog を分けることで、ExchangeAPI testing の最適化方向を維持できる
- fixture-first にすると deterministic test の可読性、差分管理、CI 安定性を優先できる
- JSONL-first にすると logging / evidence replay に寄り、v3.6.0 の ExchangeAPI testing 最適化から scope が広がりやすい
- `RealtimeReplayFrame` を主モデルにすると testing API が logging format に従属しない
- Optional package 同士の直接依存を避けることで、`ExchangeApi.Optional.Testing` を小さく保てる
- `ExchangeApi.Optional.Logging` / evidence との接続導線は残しつつ、v3.6.0 の実装 scope を fixture-first testing に集中できる
- optional package として追加する以上、利用可能な public entry point が必要である
- public API を replay frame / runner / result 近辺に限定すると、保守性と将来変更余地を両立できる
- live captured evidence を直接 test input にすると、CI 再現性、secret-free 保証、repository 正本性が弱くなる
- live evidence を sanitized fixture の元材料に限定すると、実運用由来の payload variation を取り込みながら deterministic test の安定性を維持できる

`ExchangeApi.Optional.Testing` に含める候補:

- raw frame replay helper
- replay input model
- minimal public API
- replay runner
- replay result
- decode / reject assertion helper
- diagnostic timeline assertion helper
- sample payload fixture support
- malformed payload fixture support
- secret-free fixture rule
- fixture file loader
- in-memory replay frame input
- `RealtimeReplayFrame` input model

sample payload catalog に含める候補:

- bitFlyer public realtime raw frame samples
- channel-specific payload samples
- payload variation samples
- malformed payload samples
- unknown channel / unsupported shape samples
- diagnostic assertion 用 payload samples
- secret-free validation 用 samples
- live evidence 由来の sanitized / minimized / reviewed payload samples

`ExchangeApi.Optional.Testing` の拡張候補:

- fixture catalog 追加
- replay runner の高速化
- failure message 改善
- malformed payload matrix
- diagnostic assertion の読みやすさ改善
- CI で軽く回せる replay suite
- live evidence から sanitized fixture を作る導線
- `ExchangeApi.Optional.Logging` raw frame log record から `RealtimeReplayFrame` への変換 helper

sample payload catalog の最適化候補:

- channel 追加
- payload variation 追加
- malformed payload matrix 追加
- diagnostic assertion coverage 追加
- secret-free validation 強化
- fixture naming convention 整理
- venue 追加時の payload fixture template 追加
- CI で replay suite として軽く回せる構成への整理
- JSONL replay / evidence replay との接続整理

v3.6.0 で禁止するもの:

- simulation engine
- strategy logic test framework
- ExecutionGateway behavior test framework
- CTradeBot Platform behavior test framework
- virtual clock / scheduler
- fill model
- order lifecycle model
- ledger / position model
- Gateway / Platform state model
- abstract scenario event model
- simulation scenario catalog
- strategy scenario catalog
- Gateway / Platform behavior scenario catalog
- price path / order lifecycle / fill / latency scenario catalog
- JSONL を主入力形式にすること
- `ExchangeApi.Optional.Testing` から `ExchangeApi.Optional.Logging` への直接 project reference
- JSONL log replay を v3.6.0 の主機能にすること
- simulation / scenario / Gateway / Platform / Strategy testing public API
- `local/evidence/` 配下 artifact への deterministic test dependency
- raw live evidence dump の commit
- credential / signature / auth / account detail を含む fixture

## 6. 準備手順

```bash
git checkout main
git pull --ff-only origin main
git checkout -b codex/v3.6-dev
```

準備として実施する:

- `docs/plan-v3.6.0.md` を追加する
- `docs/document-inventory.md` に v3.6 plan を登録する
- `codex/v3.6-dev` を remote に push する
- working tree を clean にする

準備では実施しない:

- v3.6.0 の実装
- public API 追加
- package / project 構成変更
- release checklist / release notes 作成

## 7. 準備完了条件

- `codex/v3.6-dev` が `main` から作成されている
- `docs/plan-v3.6.0.md` が追加されている
- `docs/document-inventory.md` が v3.6 plan を参照している
- v3.6.0 の詳細 scope は後続の裁定で固定する
- working tree が clean である

## 8. Execution Boundary Policy 文書化指示

目的:

ExchangeAPI / ExecutionGateway / CTradeBot Platform の責務境界を roadmap と分離した独立文書として固定する。
v3.x の Realtime API foundation track は変更しない。
v4.x 以降では、ExchangeAPI に stateful execution boundary を入れず、ExecutionGateway が使いやすい stateless exchange I/O surface を整える方針を明記する。

実施する:

- `docs/execution-boundary-policy.md` を追加する
- `docs/roadmap-post-v2.md` から `docs/execution-boundary-policy.md` を参照する
- `docs/document-inventory.md` に `docs/execution-boundary-policy.md` を登録する

実施しない:

- v3.6.0 の実装 scope 確定
- public API 追加
- package / project 構成変更
- ExecutionGateway 実装
- CTradeBot Platform 実装

完了条件:

- ExchangeAPI / ExecutionGateway / CTradeBot Platform の責務境界が独立文書で読める
- roadmap が責務境界ポリシーを参照している
- v3.x track は変更されていない
- v4.x 以降で ExchangeAPI に置かない stateful execution 責務が明記されている

## 9. v3.6.0 Scope

v3.6.0 は Realtime Replay / Testing Foundation release とする。

実施する:

- `ExchangeApi.Optional.Testing` を小さく追加する
- `ExchangeApi.Optional.Testing` は ExchangeAPI 層内で閉じる testing utility とする
- bitFlyer realtime raw frame replay / decode / diagnostic testing を対象にする
- `RealtimeReplayFrame` を replay input の主モデルにする
- repository 正本の sample payload fixture file と in-memory `RealtimeReplayFrame` を主入力にする
- sample payload catalog を tests 配下に追加する
- replay runner / replay result の最小 public API を追加する
- deterministic replay tests を追加する
- local consumer smoke で `ExchangeApi.Optional.Testing` を restore / build / run できることを確認する
- pack / publish 対象に `ExchangeApi.Optional.Testing` を含める

実施しない:

- simulation engine
- strategy logic test framework
- ExecutionGateway behavior test framework
- CTradeBot Platform behavior test framework
- virtual clock / scheduler
- fill model
- order lifecycle model
- ledger / position model
- Gateway / Platform state model
- abstract scenario event model
- simulation / strategy / Gateway / Platform scenario catalog
- JSONL を主入力形式にすること
- JSONL log replay を v3.6.0 の主機能にすること
- `ExchangeApi.Optional.Testing` から `ExchangeApi.Optional.Logging` への直接 project reference
- `local/evidence/` 配下 artifact への deterministic test dependency
- raw live evidence dump の commit
- credential / signature / auth / account detail を含む fixture
- Rx / `IObservable<T>` public API
- Binance realtime
- Unified
- state-changing operation

## 10. 実装指示

追加:

- `src/Optional/Testing/ExchangeApi.Optional.Testing.csproj`
- `src/Optional/Testing/Realtime/RealtimeReplayFrame.cs`
- `src/Optional/Testing/Realtime/RealtimeReplayResult.cs`
- `src/Optional/Testing/Realtime/BitflyerRealtimeReplayRunner.cs`
- `tests/Optional/Testing.Tests/ExchangeApi.Optional.Testing.Tests.csproj`
- `tests/Optional/Testing.Tests/Fixtures/Realtime/Bitflyer/RawFrames/`

更新:

- `ExchangeApi.slnx`
- `scripts/smoke-local-nuget-consumer.sh`
- 必要なら `scripts/smoke-github-packages-consumer.sh`
- 必要なら `scripts/push-github-packages.sh`
- `docs/document-inventory.md`

実装方針:

- `ExchangeApi.Optional.Testing` は `ExchangeApi.Exchanges.Bitflyer` を参照してよい
- `ExchangeApi.Optional.Testing` は `ExchangeApi.Optional.Logging` を参照しない
- `BitflyerRealtimeReplayRunner` は bitFlyer public realtime raw frame replay の MVP とする
- replay runner は raw frame から DTO decode / reject / diagnostic assertion に必要な結果を返す
- sample payload catalog は sanitized public realtime fixture に限定する
- private auth / credential / signature / account detail fixture は追加しない
- public API は replay frame / runner / result 近辺に限定する

## 11. Verification

最低限:

```bash
dotnet build ExchangeApi.slnx
dotnet test ExchangeApi.slnx --no-restore
bash scripts/pack-local-nuget.sh 3.6.0-local.replay-testing
bash scripts/smoke-local-nuget-consumer.sh 3.6.0-local.replay-testing
dotnet test ExchangeApi.LiveTests.slnx --no-restore
git diff --check
```

package 期待:

```text
ExchangeApi.Exchanges.Binance.3.6.0-local.replay-testing.nupkg
ExchangeApi.Exchanges.Bitflyer.3.6.0-local.replay-testing.nupkg
ExchangeApi.Optional.Credentials.3.6.0-local.replay-testing.nupkg
ExchangeApi.Optional.Logging.3.6.0-local.replay-testing.nupkg
ExchangeApi.Optional.Testing.3.6.0-local.replay-testing.nupkg
ExchangeApi.Primitives.3.6.0-local.replay-testing.nupkg
```

## 12. 完了条件

- `docs/plan-v3.6.0.md` が scope / non-scope / verification を固定している
- `ExchangeApi.Optional.Testing` project が追加されている
- `ExchangeApi.Optional.Testing` は `ExchangeApi.Optional.Logging` を参照していない
- `RealtimeReplayFrame` が replay input の主モデルとして公開されている
- bitFlyer public realtime raw frame replay の最小 public API がある
- sample payload catalog が tests 配下にある
- sample payload catalog は payload catalog であり scenario catalog ではない
- deterministic replay tests が通る
- local consumer smoke が `ExchangeApi.Optional.Testing` を確認している
- package generation に `ExchangeApi.Optional.Testing` が含まれる
- live tests は opt-in なしで skip する
- simulation / Gateway / Platform / Strategy testing は含まれていない
- JSONL log replay は主機能になっていない

## 13. Implementation Result

実装済み:

- `ExchangeApi.Optional.Testing` project 追加
- `RealtimeReplayFrame` / `RealtimeReplayResult<T>` / `BitflyerRealtimeReplayRunner` 追加
- bitFlyer public realtime ticker / executions / board snapshot / board delta の raw frame replay tests 追加
- malformed ticker fixture の reject diagnostic test 追加
- sample payload catalog を `tests/Optional/Testing.Tests/Fixtures/Realtime/Bitflyer/RawFrames/` に追加
- local / GitHub Packages consumer smoke に `ExchangeApi.Optional.Testing` restore / build / run 確認を追加
- distribution / local consumer / package publish docs に `ExchangeApi.Optional.Testing` を反映

verification:

```text
dotnet build ExchangeApi.slnx passed
dotnet test tests/Optional/Testing.Tests/ExchangeApi.Optional.Testing.Tests.csproj --no-restore passed
dotnet test ExchangeApi.slnx --no-restore passed
bash scripts/pack-local-nuget.sh 3.6.0-local.replay-testing passed
bash scripts/smoke-local-nuget-consumer.sh 3.6.0-local.replay-testing passed
dotnet test ExchangeApi.LiveTests.slnx --no-restore skipped safely
git diff --check passed
```

generated packages:

```text
ExchangeApi.Exchanges.Binance.3.6.0-local.replay-testing.nupkg
ExchangeApi.Exchanges.Bitflyer.3.6.0-local.replay-testing.nupkg
ExchangeApi.Optional.Credentials.3.6.0-local.replay-testing.nupkg
ExchangeApi.Optional.Logging.3.6.0-local.replay-testing.nupkg
ExchangeApi.Optional.Testing.3.6.0-local.replay-testing.nupkg
ExchangeApi.Primitives.3.6.0-local.replay-testing.nupkg
```

## 14. Review 指示

目的:

v3.6.0 実装が、裁定済みの責務境界と scope guard に沿っていることを確認する。

確認する:

- `ExchangeApi.Optional.Testing` の public API が最小である
- `ExchangeApi.Optional.Testing` が ExchangeAPI 層内で閉じている
- `ExchangeApi.Optional.Testing` から `ExchangeApi.Optional.Logging` への直接 project reference がない
- `ExchangeApi.Optional.Testing` に simulation / scenario / Gateway / Platform / Strategy testing API がない
- sample payload catalog が payload catalog であり、scenario catalog ではない
- sample payload catalog に credential / signature / auth / account detail が含まれていない
- local / GitHub Packages consumer smoke が `ExchangeApi.Optional.Testing` を確認している

必要なら修正する:

- scope が広く見える命名や文書
- missing deterministic tests
- secret-free fixture validation
- stale preparation wording

review result:

```text
public API review: minimal replay frame / runner / result surface only
dependency review: no ExchangeApi.Optional.Logging reference from ExchangeApi.Optional.Testing
scope review: no simulation / Gateway / Platform / Strategy testing API in Optional.Testing
fixture review: sample payload catalog stays under tests/Optional/Testing.Tests/Fixtures/Realtime/Bitflyer/RawFrames
documentation review: stale preparation wording corrected
additional test: fixture catalog secret-free validation added
```
