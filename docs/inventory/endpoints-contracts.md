# Inventory — Contracts APIs

> 本文書は **一覧（inventory）** である。
> 本書は、裁定者により決定された **Contracts API の採用・対応関係** を列挙する **Normative Inventory（SSOT）** とする。
> 取引所の公式 API 仕様（Method / Path / 公式URL 等）の正本は、各取引所 inventory（例: `docs/inventory/endpoints-*.md`）および公式 API 文書である。

---

## Normative Scope

本書は、以下を **正（SSOT）**として記録する。

* Contracts が提供する **API（ContractApiId）** の一覧
* 各 Contracts API の **Public / Private** 区分
* 各 Contracts API の **メソッド署名（Parameters）**
* 各 Contracts API の **RequestType / ResponseType**
* 各取引所への **対応関係（Mapping）** と、未対応の明示（`None` / `Internal`）
* 採用・不採用・留保に関する **裁定理由（DecisionNote）**

以下は本書の対象外とする。

* 取引所の公式 API の事実（HTTP Method / Path / 公式URL 等）そのもの
* EndpointId の命名規範や導出・派生規則そのもの
* 実装手順、内部クラス構成、生成コードの詳細

---

## Canonical Source（参照）

* Contracts API 署名の正本: `src/Contracts/Facade/Interfaces/*`
* 取引所 endpoint inventory: `docs/inventory/endpoints-*.md`

※ 本書は「どの Contracts API を採用し、どの取引所 EndpointId に対応づけるか」を記録する正本であり、
　署名の詳細や命名規範そのものは上記文書を参照する。

---

## Columns

| ContractScope | ContractApiId | ContractMethod | Parameters | RequestType | ResponseType | PresentIn | BitflyerEndpointId | BittradeEndpointId | DecisionNote |
| ------------- | ------------- | -------------- | ---------- | ----------- | ------------ | --------- | ------------------ | ------------------ | ------------ |

* **ContractScope**: `public` / `private`
* **ContractApiId**: Contracts 側の論理識別子（例: `GetTicker`, `PlaceLimitOrder`）
* **ContractMethod**: Facade の公開メソッド名（例: `GetTickerCallAsync`）
* **Parameters**: メソッド引数の型一覧（例: `Symbol` / `Symbol, Side, Size, Price` / `MarketLimitCursorRequest`）
* **RequestType / ResponseType**: `Call<TRequest, TOk>` の `TRequest` / `TOk` 型
* **PresentIn**: 当該 Contracts API が存在する層（`Contracts`, `Adapter`, `Normalized` 等）。通常は `Contracts`
* **BitflyerEndpointId / BittradeEndpointId**: 各取引所 inventory における EndpointId。未対応は `None`。
* **DecisionNote**: 裁定者による判断理由・留保理由（NotSupported もここに記す）

---

## Rules（運用ルール）

1. 本 inventory は **Contracts API 採用可否・対応関係の正本**である。
2. 実装は本 inventory に従うこと。
3. `DecisionNote` には **裁定理由のみ**を記載する（事実や仕様説明は書かない）。
4. 取引所側の事実（Method / Path / URL 等）は取引所 inventory にのみ記載する。

---

## Public

| ContractScope | ContractApiId        | ContractMethod                | Parameters | RequestType                 | ResponseType                   | PresentIn | BitflyerEndpointId   | BittradeEndpointId | DecisionNote        |
| ------------- | -------------------- | ----------------------------- | ---------- | --------------------------- | ------------------------------ | --------- | -------------------- | ------------------ | ------------------- |
| public        | GetExchangeInfo      | GetExchangeInfoCallAsync      | (none)     | GetExchangeInfoRequest      | ExchangeInfo                   | Contracts | Internal             | None               |                     |
| public        | GetTicker            | GetTickerCallAsync            | Symbol     | GetTickerRequest            | Ticker                         | Contracts | GetTicker            | GetDetailMerged    |                     |
| public        | GetBoard             | GetBoardCallAsync             | Symbol     | GetOrderBookRequest         | OrderBook                      | Contracts | GetBoard             | GetDepth           |                     |
| public        | GetExecutionsPublic  | GetExecutionsPublicCallAsync  | Symbol     | GetMarketExecutionsRequest  | IReadOnlyList<ExecutionMarket> | Contracts | GetExecutionsPublic  | GetTrade           |                     |

---

## Private

| ContractScope | ContractApiId         | ContractMethod                 | Parameters         | RequestType                  | ResponseType                     | PresentIn | BitflyerEndpointId    | BittradeEndpointId | DecisionNote |
| ------------- | --------------------- | ------------------------------ | ------------------ | ---------------------------- | -------------------------------- | --------- | --------------------- | ------------------ | ------------ |
| private       | GetBalance            | GetBalanceCallAsync            | (none)             | GetBalancesRequest           | IReadOnlyList<Balance>           | Contracts | GetBalance            | GetAccountsBalanceByAccountId |              |
| private       | GetExecutionsPrivate  | GetExecutionsPrivateCallAsync  | MarketLimitCursorRequest | MarketLimitCursorRequest  | Page<ExecutionItem>              | Contracts | GetExecutionsPrivate  | GetMatchResults     |              |
| private       | GetOrders             | GetOrdersCallAsync             | MarketLimitCursorRequest | MarketLimitCursorRequest  | Page<OrderSnapshotItem>          | Contracts | GetChildOrders        | GetOpenOrders        |              |
| private       | OrderLimit            | OrderLimitCallAsync            | Symbol, Side, Size, Price | PlaceLimitOrderRequest   | OrderResult                      | Contracts | SendChildOrder        | PostOrdersPlace      |              |
| private       | CancelOrder           | CancelOrderCallAsync           | CancelOrderRequest | CancelOrderRequest           | CancelResult                     | Contracts | CancelChildOrder      | PostOrdersSubmitCancelByOrderId |              |
