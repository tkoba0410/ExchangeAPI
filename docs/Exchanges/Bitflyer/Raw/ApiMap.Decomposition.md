# bitFlyer Lightning REST API 実装一覧（Raw 層・補助：分解ビュー）

本ドキュメントは、Raw-only 正本（`ApiMap`）を **命名規則の構成要素に分解**して可視化する補助ビューです。

* 目的：命名レビュー（Verb / Noun / ByCondition）と Request DTO 判断を機械的に行う
* 本表は補助であり、更新の正本は **Raw-only 正本**を優先する

---

## API 分解表（Raw naming decomposition）

| HTTP METHOD | Path                              | 種別      | Verb   | Noun               | ByCondition     | Method（合成）                             | Request DTO              |
| ----------- | --------------------------------- | ------- | ------ | ------------------ | --------------- | -------------------------------------- | ------------------------ |
| GET         | /v1/getmarkets                    | Public  | Get    | Markets            | -               | GetMarketsAsync                        | -                        |
| GET         | /v1/markets                       | Public  | Get    | Markets            | -               | GetMarketsAsync                        | -                        |
| GET         | /v1/getmarkets/usa                | Public  | Get    | Markets            | ByRegionUsa     | GetMarketsByRegionUsaAsync             | -                        |
| GET         | /v1/markets/usa                   | Public  | Get    | Markets            | ByRegionUsa     | GetMarketsByRegionUsaAsync             | -                        |
| GET         | /v1/getmarkets/eu                 | Public  | Get    | Markets            | ByRegionEu      | GetMarketsByRegionEuAsync              | -                        |
| GET         | /v1/markets/eu                    | Public  | Get    | Markets            | ByRegionEu      | GetMarketsByRegionEuAsync              | -                        |
| GET         | /v1/getboard（/v1/board）           | Public  | Get    | OrderBook          | ByProductCode   | GetOrderBookByProductCodeAsync         | -                        |
| GET         | /v1/getticker（/v1/ticker）         | Public  | Get    | Ticker             | ByProductCode   | GetTickerByProductCodeAsync            | -                        |
| GET         | /v1/getexecutions（/v1/executions） | Public  | Get    | Executions         | ByProductCode   | GetExecutionsByProductCodeAsync        | GetExecutionsRequest     |
| GET         | /v1/getboardstate                 | Public  | Get    | BoardState         | ByProductCode   | GetBoardStateByProductCodeAsync        | -                        |
| GET         | /v1/gethealth                     | Public  | Get    | Health             | -               | GetHealthAsync                         | -                        |
| GET         | /v1/getfundingrate                | Public  | Get    | FundingRate        | -               | GetFundingRateAsync                    | -                        |
| GET         | /v1/getcorporateleverage          | Public  | Get    | CorporateLeverage  | -               | GetCorporateLeverageAsync              | -                        |
| GET         | /v1/getchats                      | Public  | Get    | Chats              | -               | GetChatsAsync                          | -                        |
| GET         | /v1/getchats/usa                  | Public  | Get    | Chats              | ByRegionUsa     | GetChatsByRegionUsaAsync               | -                        |
| GET         | /v1/getchats/eu                   | Public  | Get    | Chats              | ByRegionEu      | GetChatsByRegionEuAsync                | -                        |
| GET         | /v1/me/getpermissions             | Private | Get    | Permissions        | -               | GetPermissionsAsync                    | -                        |
| GET         | /v1/me/getbalance                 | Private | Get    | Balances           | -               | GetBalancesAsync                       | -                        |
| GET         | /v1/me/getcollateral              | Private | Get    | Collateral         | -               | GetCollateralAsync                     | -                        |
| GET         | /v1/me/getcollateralaccounts      | Private | Get    | CollateralAccounts | -               | GetCollateralAccountsAsync             | -                        |
| GET         | /v1/me/getaddresses               | Private | Get    | Addresses          | -               | GetAddressesAsync                      | -                        |
| GET         | /v1/me/getcoinins                 | Private | Get    | CoinIns            | -               | GetCoinInsAsync                        | -                        |
| GET         | /v1/me/getcoinouts                | Private | Get    | CoinOuts           | -               | GetCoinOutsAsync                       | -                        |
| GET         | /v1/me/getbankaccounts            | Private | Get    | BankAccounts       | -               | GetBankAccountsAsync                   | -                        |
| GET         | /v1/me/getdeposits                | Private | Get    | Deposits           | -               | GetDepositsAsync                       | -                        |
| POST        | /v1/me/withdraw                   | Private | Create | Withdrawal         | -               | CreateWithdrawalAsync                  | CreateWithdrawalRequest  |
| GET         | /v1/me/getwithdrawals             | Private | Get    | Withdrawals        | -               | GetWithdrawalsAsync                    | -                        |
| POST        | /v1/me/sendchildorder             | Private | Create | ChildOrder         | -               | CreateChildOrderAsync                  | CreateChildOrderRequest  |
| POST        | /v1/me/cancelchildorder           | Private | Cancel | ChildOrder         | -               | CancelChildOrderAsync                  | CancelChildOrderRequest  |
| POST        | /v1/me/sendparentorder            | Private | Create | ParentOrder        | -               | CreateParentOrderAsync                 | CreateParentOrderRequest |
| POST        | /v1/me/cancelparentorder          | Private | Cancel | ParentOrder        | -               | CancelParentOrderAsync                 | CancelParentOrderRequest |
| POST        | /v1/me/cancelallchildorders       | Private | Cancel | Orders             | ByProductCode   | CancelOrdersByProductCodeAsync         | CancelAllOrdersRequest   |
| GET         | /v1/me/getchildorders             | Private | Get    | Orders             | ByProductCode   | GetOrdersByProductCodeAsync            | GetOrdersRequest         |
| GET         | /v1/me/getparentorders            | Private | Get    | ParentOrders       | -               | GetParentOrdersAsync                   | -                        |
| GET         | /v1/me/getparentorder             | Private | Get    | ParentOrder        | ByParentOrderId | GetParentOrderByParentOrderIdAsync     | -                        |
| GET         | /v1/me/getexecutions              | Private | Get    | Executions         | ByProductCode   | GetExecutionsByProductCodeAsync        | GetExecutionsRequest     |
| GET         | /v1/me/getbalancehistory          | Private | Get    | BalanceHistory     | -               | GetBalanceHistoryAsync                 | -                        |
| GET         | /v1/me/getpositions               | Private | Get    | Positions          | ByProductCode   | GetPositionsByProductCodeAsync         | -                        |
| GET         | /v1/me/getcollateralhistory       | Private | Get    | CollateralHistory  | -               | GetCollateralHistoryAsync              | -                        |
| GET         | /v1/me/gettradingcommission       | Private | Get    | TradingCommission  | ByProductCode   | GetTradingCommissionByProductCodeAsync | -                        |

---

## 注記（レビュー観点）

* **ByCondition は原則 Path 識別子のみ**。

  * 本表では、bitFlyer の多くが `product_code` を必須 Query として扱うため、補助ビューとして `ByProductCode` を明示しています。
  * 実際にメソッド名へ `ByProductCode` を付与するかは、`../../Raw/Naming.md` の「必須 Query 例外」ルールに従います。

* `getmarkets/usa` / `getmarkets/eu` は **Path に埋め込まれた固定条件**であり、
  `ByRegionUsa` / `ByRegionEu` は補助的な表現です。実装は `GetUsaMarketsAsync` のような固定名でも可。

* 本表の `Method（合成）` は規則適用後の候補名であり、既存実装との互換は委譲・Obsolete で段階的に寄せることを推奨します。

---

> 補助ビューは「規則に当てたらこう見える」を提供する。正本は Raw-only ApiMap。
