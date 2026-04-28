# v3.6.0 Realtime Replay / Testing Foundation 準備指示

最終更新: 2026-04-28
位置づけ: v3.6.0 scope framing / preparation

状態: preparing

## 1. 目的

v3.6.0 は、v3.5.0 で追加した Realtime Diagnostics Foundation の上に、Realtime Replay / Testing Foundation を検討する release として開始する。

この準備指示では、実装 scope をまだ確定しない。
まず branch と文書の入口を用意し、次に方針を裁定してから v3.6.0 の正式 scope / non-scope / verification / 完了条件を本書へ固定する。

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
- v3.6.0 の詳細 scope は未確定として残っている
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
