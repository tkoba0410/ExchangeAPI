# bitFlyer Lightning REST API 実装一覧（Raw 層）

現在 Raw 層で実装済みの HTTP エンドポイント一覧。

## Public (GET)
- /v1/getmarkets / /v1/markets
- /v1/getmarkets/usa / /v1/markets/usa
- /v1/getmarkets/eu / /v1/markets/eu
- /v1/getboard / /v1/board
- /v1/getticker / /v1/ticker
- /v1/getexecutions / /v1/executions
- /v1/getchats / /v1/getchats/usa / /v1/getchats/eu
- /v1/gethealth
- /v1/getboardstate
- /v1/getcorporateleverage
- /v1/getfundingrate

## Private (GET)
- /v1/me/getpermissions
- /v1/me/getbalance
- /v1/me/getcollateral
- /v1/me/getcollateralaccounts
- /v1/me/getchildorders
- /v1/me/getparentorders
- /v1/me/getparentorder
- /v1/me/getexecutions
- /v1/me/getbalancehistory
- /v1/me/getpositions
- /v1/me/getcollateralhistory
- /v1/me/gettradingcommission
- /v1/me/getaddresses
- /v1/me/getcoinins
- /v1/me/getcoinouts
- /v1/me/getdeposits
- /v1/me/getwithdrawals
- /v1/me/getbankaccounts

## Private (POST)
- /v1/me/sendchildorder
- /v1/me/sendparentorder
- /v1/me/cancelchildorder
- /v1/me/cancelparentorder
- /v1/me/cancelallchildorders
- /v1/me/withdraw
