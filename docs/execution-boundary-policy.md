# Execution Boundary Policy

最終更新: 2026-04-28
位置づけ: ExchangeAPI / ExecutionGateway / CTradeBot Platform 責務境界ポリシー

## 1. 目的

ExchangeAPI と CTradeBot の実装が並行して進むため、今後の設計・実装では roadmap boundary だけでなく responsibility boundary を明示的に採用する。

重視する観点:

- 論理性
- 合理性
- 整合性
- 保守性
- 可読性

物理的な配置は段階的に変更してよい。
ただし、責務境界は初期段階から固定する。

## 2. 基本方針

```text
ExchangeAPI        = stateless adapter for exchange I/O
ExecutionGateway   = stateful boundary for external order execution
CTradeBot Platform = SSOT for strategy, accounting, ledger, and bot state
```

ExchangeAPI は再利用可能な取引所 I/O library として維持する。
ExecutionGateway は外部注文状態を管理する境界とする。
CTradeBot Platform は strategy / accounting / ledger / bot state の正本とする。

## 3. ExchangeAPI

役割:

取引所 API 呼び出しのための再利用可能 library。

責務:

- REST / WebSocket API 呼び出し
- 認証
- exchange-native DTO 変換
- 必要最小限の共通 DTO / primitive 変換
- エラー分類
- SymbolSpec / SizeStep / PriceStep / Capability
- stateless validation

許容される状態:

- HTTP client lifecycle
- WebSocket connection lifecycle
- 認証 token / credential session lifecycle
- 短期的な metadata cache

禁止:

- 永続的な注文状態
- `clientOrderKey` 正本管理
- retry / reconcile の正本
- Bot 固有状態
- ledger / position
- Strategy 依存の判断

## 4. ExecutionGateway

役割:

外部注文の実行状態を管理する stateful boundary。
ただし accounting / ledger / position の SSOT ではない。

責務:

- `clientOrderKey` 管理の正本
- order lifecycle 管理
- duplicate 防止 / idempotency
- cancel -> inquiry -> retry
- 最小 reconcile から開始する reconcile boundary
- open order tracking
- rate limit 制御
- audit log
- paper / live 切替

扱う状態:

- 外部注文状態
- 未完了注文
- 取引所注文 ID と `clientOrderKey` の対応
- 最終照会結果

約定の扱い:

- 取引所約定を観測・正規化する
- Platform に渡すまで、会計上の `ExternalFill` ではない

## 5. CTradeBot Platform

役割:

戦略・会計・Bot 状態の正本。

責務:

- `StrategyInstance` 管理
- Intent 生成
- internal netting
- external fill allocation
- ledger 管理
- position size 算出
- `ActionKey` 管理
- Snapshot 生成
- SAFE MODE / HARD STOP 判定

保持する状態:

- Strategy 別 position
- ledger
- internal / external fill
- allocation
- step 状態

## 6. 境界ルール

Rule 1:

ExchangeAPI は stateless adapter とする。
Bot 固有状態を持たない。

Rule 2:

`clientOrderKey` の正本は ExecutionGateway とする。

Rule 3:

ledger の正本は CTradeBot Platform とする。

Rule 4:

retry / reconcile は ExecutionGateway の責務とする。

Rule 5:

internal netting / allocation は CTradeBot Platform の責務とする。

Rule 6:

SAFE MODE は分担する。
ExecutionGateway は重複防止・rate limit などの局所的安全性を扱い、CTradeBot Platform は停止・mode 制御などの全体判断を扱う。

## 7. データフロー

注文方向:

```text
Strategy
  -> Intent
Platform
  -> ExecutionCommand
ExecutionGateway
  -> Request
ExchangeAPI
  -> Exchange
```

観測方向:

```text
Exchange
  -> Response
ExchangeAPI
  -> DTO
ExecutionGateway
  -> OrderState / Fill observation
Platform
  -> Ledger / Allocation
```

## 8. 実装方針

- 既存コードは壊さない
- 新規実装から適用する
- 物理構成の変更は段階的に行う
- 責務境界を優先する

Stage 1 は最小実装でよい。
retry は簡易、reconcile は最低限、tracking は最小から開始する。

ただし、次は禁止する。

- ExchangeAPI に注文状態を置く
- ExchangeAPI に ledger を置く
- Strategy が ExchangeAPI を直接呼ぶ構造を正本にする

## 9. 判断基準

```text
Q1: 取引所 I/O か       -> ExchangeAPI
Q2: 外部注文状態か      -> ExecutionGateway
Q3: 戦略・会計・Bot状態か -> CTradeBot Platform
```

## 10. ExchangeAPI Roadmap への反映

v3.x は現行方針のまま Realtime API foundation track として維持する。

v4.x 以降では、ExchangeAPI に stateful execution boundary を入れない。
ExchangeAPI の v4 候補は ExecutionGateway が使いやすい stateless exchange I/O surface の整備として扱う。

ExchangeAPI v4 候補:

- SymbolSpec / SizeStep / PriceStep / Capability
- stateless order validation
- error taxonomy 整理
- order response / fill observation DTO の整理
- HTTP + Realtime を組み合わせやすい read surface
- ExecutionGateway が inquiry / reconcile しやすい read API 整備
- secret-free audit / evidence 連携

ExchangeAPI v4 でも扱わないもの:

- `clientOrderKey` 正本管理
- retry policy の正本
- reconcile loop
- open order tracking
- execution state machine
- ledger / position / allocation
- Bot 固有 SAFE MODE 判断

最終目標:

```text
Bot は取引所を知らない
ExecutionGateway だけが外部注文を扱う
ExchangeAPI は再利用可能な取引所 I/O library であり続ける
```
