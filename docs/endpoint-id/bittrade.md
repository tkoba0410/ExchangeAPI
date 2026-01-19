# endpoints-bittrade.md（inventory）

> 本ファイルは、bitTrade における endpoint の **列挙の正本**である。
> inventory は endpoint 同定のみを目的とし、詳細仕様（request / response / error 等）は公式 API ドキュメントを正本とする。

---

## 1. 対象取引所

* Exchange: `bitTrade`

---

## 2. 取引所ルール（EndpointId 組み立て規則）

本節は、common（EndpointId 規範）で定義された要素（Path / Method / Scope）および
一般単語境界の定義を前提に、bitTrade の EndpointId を **機械的に再構成可能**とするための規則を定義する。

### 2.1 前提（common 参照）

* EndpointId の構成要素・表記制約は **common.md に準拠**する
* 一般単語境界の定義は **common.md に定義されたものを使用**する

### 2.2 Path / Method → EndpointId（bitTrade 規則）

bitTrade の EndpointId は、(Method, Path) から **機械的に**次の手順で導出する。

1. Path 先頭の `/` を除去する
2. 先頭セグメントが version（例：`v1`）である場合は除去する
3. その後の **先頭セグメントを 1 つ除去する**
4. 残りを `/` で分割し、空要素を除外する
5. 各セグメントを **common で定義された一般単語境界** に基づいて分割する

   * `{...}` 形式の path parameter は、**Path 上からは除去**する
   * `{...}` 形式の path parameter が存在した場合は、
     **内部の parameter 名を PascalCase 化し、`By<ParameterName>` として EndpointId の末尾に付与する**
6. 分割された各単語を PascalCase 化し、連結する
7. HTTP Method を PascalCase 化し、EndpointId の **先頭**に付与する

   * 例：`GET` → `Get`、`POST` → `Post`

---

## 3. 並び順ルール

endpoint 一覧の並び順は、公式 API ドキュメントにおける記載順を正とする。

* Public API を先に列挙する
* 次に Private API を列挙する
* 実装都合による並び替えは行わない

---

## 4. endpoint 一覧（新ルール完全適用）

| EndpointId                            | Method | Path                                           | Scope   |
| ------------------------------------- | ------ | ---------------------------------------------- | ------- |
| GetSymbols                            | GET    | `/v1/common/symbols`                           | Public  |
| GetCurrencys                          | GET    | `/v1/common/currencys`                         | Public  |
| GetTimestamp                          | GET    | `/v1/common/timestamp`                         | Public  |
| GetHistoryKline                       | GET    | `/market/history/kline`                        | Public  |
| GetDetailMerged                       | GET    | `/market/detail/merged`                        | Public  |
| GetTickers                            | GET    | `/market/tickers`                              | Public  |
| GetDepth                              | GET    | `/market/depth`                                | Public  |
| GetTrade                              | GET    | `/market/trade`                                | Public  |
| GetHistoryTrade                       | GET    | `/market/history/trade`                        | Public  |
| GetAccounts                           | GET    | `/v1/account/accounts`                         | Private |
| GetAccountsBalanceByAccountId         | GET    | `/v1/account/accounts/{account-id}/balance`    | Private |
| PostOrdersPlace                       | POST   | `/v1/order/orders/place`                       | Private |
| GetOpenOrders                         | GET    | `/v1/order/openOrders`                         | Private |
| PostOrdersSubmitCancelByOrderId       | POST   | `/v1/order/orders/{order-id}/submitcancel`     | Private |
| PostOrdersBatchCancel                 | POST   | `/v1/order/orders/batchcancel`                 | Private |
| PostOrdersBatchCancelOpenOrders       | POST   | `/v1/order/orders/batchCancelOpenOrders`       | Private |
| GetOrdersByOrderId                    | GET    | `/v1/order/orders/{order-id}`                  | Private |
| GetOrdersMatchResultsByOrderId        | GET    | `/v1/order/orders/{order-id}/matchresults`     | Private |
| GetOrders                             | GET    | `/v1/order/orders`                             | Private |
| GetMatchResults                       | GET    | `/v1/order/matchresults`                       | Private |
| PostWithdrawApiCreate                 | POST   | `/v1/dw/withdraw/api/create`                   | Private |
| PostWithdrawVirtualCancelByWithdrawId | POST   | `/v1/dw/withdraw-virtual/{withdraw-id}/cancel` | Private |
| GetDepositWithdraw                    | GET    | `/v1/query/deposit-withdraw`                   | Private |
| PostOrderPlace                        | POST   | `/v1/retail/order/place`                       | Private |
| GetOrderList                          | GET    | `/v1/retail/order/list`                        | Private |
| GetMaintainTime                       | GET    | `/v1/retail/maintain/time`                     | Private |
