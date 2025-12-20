# bitFlyer Lightning REST API 実装一覧（Raw 層・正本）

本ドキュメントは、**bitFlyer Lightning REST API** に対して、
現在 **Raw 層で実装済みの HTTP エンドポイント**を、
**Raw 層の要素のみに限定して**一覧化した正本ドキュメントです。

* 表は **公式 API の提示順**に並べています
* 本表は **Raw 層の事実のみ**を扱います
* 抽象層（Adapter / Facade）での公開状況は本表の責務外です

> Raw 層は公式 API の鏡像であり、未露出 API も意図的に保持されます。

---

## API 実装対応表（Raw only）

| HTTP METHOD | Path                              | 種別      | Raw Verb | Raw メソッド              | Request DTO                 |
| ----------- | --------------------------------- | ------- | -------- | ------------------------- | --------------------------- |
| GET         | /v1/getmarkets                    | Public  | Get      | GetMarketsAsync           | -                           |
| GET         | /v1/markets                       | Public  | Get      | GetMarketsAsync           | -                           |
| GET         | /v1/getmarkets/usa                | Public  | Get      | GetMarketsAsync           | -                           |
| GET         | /v1/markets/usa                   | Public  | Get      | GetMarketsAsync           | -                           |
| GET         | /v1/getmarkets/eu                 | Public  | Get      | GetMarketsAsync           | -                           |
| GET         | /v1/markets/eu                    | Public  | Get      | GetMarketsAsync           | -                           |
| GET         | /v1/getboard（/v1/board）           | Public  | Get      | GetBoardRawAsync          | -                           |
| GET         | /v1/getticker（/v1/ticker）         | Public  | Get      | GetTickerRawAsync         | -                           |
| GET         | /v1/getexecutions（/v1/executions） | Public  | Get      | GetExecutionsRawAsync     | -                           |
| GET         | /v1/getboardstate                 | Public  | Get      | GetBoardStateAsync        | -                           |
| GET         | /v1/gethealth                     | Public  | Get      | GetHealthAsync            | -                           |
| GET         | /v1/getfundingrate                | Public  | Get      | GetFundingRateAsync       | -                           |
| GET         | /v1/getcorporateleverage          | Public  | Get      | GetCorporateLeverageAsync | -                           |
| GET         | /v1/getchats                      | Public  | Get      | GetChatsAsync             | -                           |
| GET         | /v1/getchats/usa                  | Public  | Get      | GetChatsAsync             | -                           |
| GET         | /v1/getchats/eu                   | Public  | Get      | GetChatsAsync             | -                           |
| GET         | /v1/me/getpermissions             | Private | Get      | GetPermissionsAsync       | -                           |
| GET         | /v1/me/getbalance                 | Private | Get      | GetBalancesAsync          | -                           |
| GET         | /v1/me/getcollateral              | Private | Get      | GetCollateralAsync        | -                           |
| GET         | /v1/me/getcollateralaccounts      | Private | Get      | GetCollateralAccountsAsync| -                           |
| GET         | /v1/me/getaddresses               | Private | Get      | GetAddressesAsync         | -                           |
| GET         | /v1/me/getcoinins                 | Private | Get      | GetCoinInsAsync           | -                           |
| GET         | /v1/me/getcoinouts                | Private | Get      | GetCoinOutsAsync          | -                           |
| GET         | /v1/me/getbankaccounts            | Private | Get      | GetBankAccountsAsync      | -                           |
| GET         | /v1/me/getdeposits                | Private | Get      | GetDepositsAsync          | -                           |
| POST        | /v1/me/withdraw                   | Private | Create   | CreateWithdrawalAsync     | CreateWithdrawalRequest     |
| GET         | /v1/me/getwithdrawals             | Private | Get      | GetWithdrawalsAsync       | -                           |
| POST        | /v1/me/sendchildorder             | Private | Create   | CreateChildOrderAsync     | CreateChildOrderRequest     |
| POST        | /v1/me/cancelchildorder           | Private | Cancel   | CancelChildOrderAsync     | CancelChildOrderRequest     |
| POST        | /v1/me/sendparentorder            | Private | Create   | CreateParentOrderAsync    | CreateParentOrderRequest    |
| POST        | /v1/me/cancelparentorder          | Private | Cancel   | CancelParentOrderAsync    | CancelParentOrderRequest    |
| POST        | /v1/me/cancelallchildorders       | Private | Cancel   | CancelAllChildOrdersAsync | CancelAllChildOrdersRequest |
| GET         | /v1/me/getchildorders             | Private | Get      | GetChildOrdersAsync       | -                           |
| GET         | /v1/me/getparentorders            | Private | Get      | GetParentOrdersAsync      | -                           |
| GET         | /v1/me/getparentorder             | Private | Get      | GetParentOrderAsync       | -                           |
| GET         | /v1/me/getexecutions              | Private | Get      | GetExecutionsAsync        | -                           |
| GET         | /v1/me/getbalancehistory          | Private | Get      | GetBalanceHistoryAsync    | -                           |
| GET         | /v1/me/getpositions               | Private | Get      | GetPositionsAsync         | -                           |
| GET         | /v1/me/getcollateralhistory       | Private | Get      | GetCollateralHistoryAsync | -                           |
| GET         | /v1/me/gettradingcommission       | Private | Get      | GetTradingCommissionAsync | -                           |

---

## 注記

* 本表は Raw 層の **唯一の正本**です
* 抽象層での公開状況は Adapter 側ドキュメントを参照してください
* Raw API の命名および Request DTO は `../../Raw/Naming.md` の規則に従います

---

> Raw first. Faithful mapping. No abstraction.
