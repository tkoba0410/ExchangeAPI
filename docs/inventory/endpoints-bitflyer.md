# endpoints-bitflyer inventory（全一覧・生成結果）

> 注意: これは bitFlyer 公式ドキュメント（Lightning API HTML）から抽出した `METHOD + /v1/...` の全出現をベースに、inventory 形式へ整形した結果。
> `docs/inventory/endpoints-bitflyer.md` に反映する場合は、本ファイル内容で置き換える。

## Inventory

| EndpointId            | Method | Path                         | Scope   | Notes                                          | OfficialDocRef                             |
| --------------------- | ------ | ---------------------------- | ------- | ---------------------------------------------- | ------------------------------------------ |
| MarketsGetLegacy      | GET    | /v1/getmarkets               | Public  | legacy alias of `/v1/markets`                  | Lightning API (REST) - Markets             |
| MarketsGet            | GET    | /v1/markets                  | Public  |                                                | Lightning API (REST) - Markets             |
| BoardGetLegacy        | GET    | /v1/getboard                 | Public  | legacy alias of `/v1/board`                    | Lightning API (REST) - Board               |
| BoardGet              | GET    | /v1/board                    | Public  |                                                | Lightning API (REST) - Board               |
| TickerGetLegacy       | GET    | /v1/getticker                | Public  | legacy alias of `/v1/ticker`                   | Lightning API (REST) - Ticker              |
| TickerGet             | GET    | /v1/ticker                   | Public  |                                                | Lightning API (REST) - Ticker              |
| ExecutionsGetLegacy   | GET    | /v1/getexecutions            | Public  | legacy alias of `/v1/executions`; cursor/limit | Lightning API (REST) - Executions          |
| ExecutionsGet         | GET    | /v1/executions               | Public  | cursor/limit                                   | Lightning API (REST) - Executions          |
| BoardStateGet         | GET    | /v1/getboardstate            | Public  |                                                | Lightning API (REST) - Board state         |
| HealthGet             | GET    | /v1/gethealth                | Public  |                                                | Lightning API (REST) - Health              |
| FundingRateGet        | GET    | /v1/getfundingrate           | Public  |                                                | Lightning API (REST) - Funding rate        |
| CorporateLeverageGet  | GET    | /v1/getcorporateleverage     | Public  |                                                | Lightning API (REST) - Corporate leverage  |
| ChatsGet              | GET    | /v1/getchats                 | Public  |                                                | Lightning API (REST) - Chats               |
| PermissionsGet        | GET    | /v1/me/getpermissions        | Private | signed                                         | Lightning API (REST) - Permissions         |
| BalanceGet            | GET    | /v1/me/getbalance            | Private | signed                                         | Lightning API (REST) - Balance             |
| CollateralGet         | GET    | /v1/me/getcollateral         | Private | signed                                         | Lightning API (REST) - Collateral          |
| CollateralAccountsGet | GET    | /v1/me/getcollateralaccounts | Private | signed                                         | Lightning API (REST) - Collateral accounts |
| AddressesGet          | GET    | /v1/me/getaddresses          | Private | signed                                         | Lightning API (REST) - Addresses           |
| CoinInsGet            | GET    | /v1/me/getcoinins            | Private | signed                                         | Lightning API (REST) - Coin ins            |
| CoinOutsGet           | GET    | /v1/me/getcoinouts           | Private | signed                                         | Lightning API (REST) - Coin outs           |
| BankAccountsGet       | GET    | /v1/me/getbankaccounts       | Private | signed                                         | Lightning API (REST) - Bank accounts       |
| DepositsGet           | GET    | /v1/me/getdeposits           | Private | signed                                         | Lightning API (REST) - Deposits            |
| WithdrawCreate        | POST   | /v1/me/withdraw              | Private | signed                                         | Lightning API (REST) - Withdraw            |
| WithdrawalsGet        | GET    | /v1/me/getwithdrawals        | Private | signed                                         | Lightning API (REST) - Withdrawals         |
| ChildOrderPlace       | POST   | /v1/me/sendchildorder        | Private | signed                                         | Lightning API (REST) - Child order         |
| ChildOrderCancel      | POST   | /v1/me/cancelchildorder      | Private | signed                                         | Lightning API (REST) - Child order         |
| ChildOrdersCancelAll  | POST   | /v1/me/cancelallchildorders  | Private | signed                                         | Lightning API (REST) - Child order         |
| ChildOrdersGet        | GET    | /v1/me/getchildorders        | Private | signed; cursor/limit                           | Lightning API (REST) - Child orders        |
| ParentOrderPlace      | POST   | /v1/me/sendparentorder       | Private | signed                                         | Lightning API (REST) - Parent order        |
| ParentOrderCancel     | POST   | /v1/me/cancelparentorder     | Private | signed                                         | Lightning API (REST) - Parent order        |
| ParentOrdersGet       | GET    | /v1/me/getparentorders       | Private | signed; cursor/limit                           | Lightning API (REST) - Parent orders       |
| ParentOrderGet        | GET    | /v1/me/getparentorder        | Private | signed                                         | Lightning API (REST) - Parent order        |
| ExecutionsGetPrivate  | GET    | /v1/me/getexecutions         | Private | signed; cursor/limit                           | Lightning API (REST) - Executions          |
| BalanceHistoryGet     | GET    | /v1/me/getbalancehistory     | Private | signed; cursor/limit                           | Lightning API (REST) - Balance history     |
| PositionsGet          | GET    | /v1/me/getpositions          | Private | signed; cursor/limit                           | Lightning API (REST) - Positions           |
| CollateralHistoryGet  | GET    | /v1/me/getcollateralhistory  | Private | signed; cursor/limit                           | Lightning API (REST) - Collateral history  |
| TradingCommissionGet  | GET    | /v1/me/gettradingcommission  | Private | signed                                         | Lightning API (REST) - Trading commission  |
