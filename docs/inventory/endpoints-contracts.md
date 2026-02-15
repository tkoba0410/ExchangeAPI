# Inventory — Contracts APIs

> 本文書は **inventory（事実一覧）** です。
> Contracts API の実装対応状況と取引所 EndpointId との対応関係を記録します。
> 仕様判断や運用判断の根拠は `docs/contracts/contracts.md` / `docs/governance.md` / `docs/process.md` を参照してください。


## Scope

本書は、以下の対応関係を記録する。

* Contracts が提供する **API（ContractApiId）** の一覧
* 各 Contracts API の **Public / Private** 区分
* 各 Contracts API の **Facade メソッド名（ContractMethod）**
* 各 Contracts API の **RequestType / ResponseType**
* 各取引所への **対応関係（Mapping）** と、未対応の明示（`None` / `Internal`）
* 対応状態に関する **補足メモ（MappingStatus）**

以下は本書の対象外とする。

* 取引所の公式 API の事実（HTTP Method / Path / 公式URL 等）そのもの
* EndpointId の命名規範や導出・派生規則そのもの
* 実装手順、内部クラス構成、生成コードの詳細

---

## Canonical Source（参照）

* Contracts API 署名参照: `src/Contracts/Facade/Interfaces/*`
* 利便呼び出し（非規範）: `src/Contracts/Facade/Extensions/*`
* 取引所 endpoint inventory: `docs/inventory/endpoints-*.md`
* Contracts 契約条文参照: `docs/contracts/contracts.md`

※ 本書は「どの Contracts API がどの取引所 EndpointId に対応しているか」の対応一覧を記録する。
　署名定義や命名ルールの説明は、参照先文書に集約されている。

---

## Columns

| ContractScope | ContractApiId | ContractMethod | RequestType | ResponseType | PresentIn | BitflyerEndpointId | BittradeEndpointId | MappingStatus |
| ------------- | ------------- | -------------- | ----------- | ------------ | --------- | ------------------ | ------------------ | ------------ |

* **ContractScope**: `public` / `private`
* **ContractApiId**: Contracts 側の論理識別子（例: `Ticker`, `OrderLimit`）
* **ContractMethod**: Facade の公開メソッド名（例: `GetTickerAsync`）
* **RequestType / ResponseType**: Contracts の `Call<TRequest, TOk>` における `TRequest` / `TOk` 型
* **PresentIn**: 当該 Contracts API が存在する層（`Contracts`, `Adapter`, `Normalized`, `Application` 等）。通常は `Contracts`
* **BitflyerEndpointId / BittradeEndpointId**: 各取引所 inventory における EndpointId。未対応は `None` / `Internal`。
* **MappingStatus**: 対応状況に関する事実メモ（例: `bitflyer: NotSupported`）

---

## Public

| ContractScope | ContractApiId        | ContractMethod                | RequestType                 | ResponseType                   | PresentIn | BitflyerEndpointId   | BittradeEndpointId | MappingStatus |
| ------------- | -------------------- | ----------------------------- | --------------------------- | ------------------------------ | --------- | -------------------- | ------------------ | ------------ |
| public        | Ticker               | GetTickerAsync                | TickerRequest               | TickerResponse                 | Contracts | GetTicker            | GetDetailMerged    |              |
| public        | Board                | GetBoardAsync                 | BoardRequest                | BoardResponse                  | Contracts | GetBoard             | GetDepth           |              |
| public        | ExecutionsPublic     | GetExecutionsPublicAsync      | ExecutionsPublicRequest     | ExecutionsPublicResponse       | Contracts | GetExecutionsPublic  | GetTrade           |              |
| public        | Candlestick          | GetCandlesticksAsync          | CandlesticksRequest         | CandlesticksResponse           | Contracts | None                 | GetHistoryKline    | bitflyer: NotSupported |

---

## Private

| ContractScope | ContractApiId         | ContractMethod                 | RequestType                  | ResponseType                     | PresentIn | BitflyerEndpointId    | BittradeEndpointId              | MappingStatus |
| ------------- | --------------------- | ------------------------------ | ---------------------------- | -------------------------------- | --------- | --------------------- | -------------------------------- | ------------ |
| private       | Balance               | GetBalanceAsync                | BalanceRequest               | BalanceResponse                  | Contracts | GetBalance            | GetAccountsBalanceByAccountId   |              |
| private       | ExecutionsPrivate     | GetExecutionsPrivateAsync      | ExecutionsPrivateRequest     | ExecutionsPrivateResponse        | Contracts | GetExecutionsPrivate  | GetMatchResults                 |              |
| private       | Orders                | GetOrdersAsync                 | OrdersRequest                | OrdersResponse                   | Contracts | GetChildOrders        | GetOpenOrders                    |              |
| private       | OrderLimit            | OrderLimitAsync                | OrderLimitRequest            | OrderLimitResponse               | Contracts | SendChildOrder        | PostOrdersPlace                  |              |
| private       | CancelOrder           | CancelOrderAsync               | CancelOrderRequest           | CancelOrderResponse              | Contracts | CancelChildOrder      | PostOrdersSubmitCancelByOrderId  |              |

---

## Non-Normative Convenience Overloads

以下は開発者向けの利便 API であり、契約仕様本文ではない。

- `GetTickerAsync(Symbol)` など: `src/Contracts/Facade/Extensions/PublicApiExtensions.cs`
- `OrderLimitAsync(Symbol, Side, Size, Price)` など: `src/Contracts/Facade/Extensions/PrivateApiExtensions.cs`
