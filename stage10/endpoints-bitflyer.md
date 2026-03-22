# Stage10 Endpoint Matrix — Bitflyer

本書は、[endpoints-bitflyer.md](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/inventory/endpoints-bitflyer.md) を入力として作成した Stage10 用の判断表である。  
inventory が事実一覧であるのに対し、本書は Stage10 第1段階の実装・DTO 固定・live test 導入順を管理する。

## Values

- `ExposeInWire`
  - `Yes`: Stage10 第1段階で `Wire` 公開面に含める
  - `Later`: Stage10 後段で扱う
- `ExposeInNormalized`
  - `Yes`: Stage10 第1段階で `Normalized` 公開面に含める
  - `Later`: Stage10 後段で扱う
- `LiveTestPhase`
  - `Phase1-Read`: 第1段階の read live test 対象
  - `Phase2-Write`: 第2段階の write live test 対象
  - `Later`: 後段導入
- `RequestDtoStatus` / `ResponseDtoStatus`
  - `Transitional`: 最終固定前
  - `Fixed`: 最終固定済み

## Matrix

| EndpointId | Method | Path | Scope | ExposeInWire | ExposeInNormalized | LiveTestPhase | RequestDtoStatus | ResponseDtoStatus |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| GetMarkets | GET | /v1/getmarkets | public | Later | Later | Later | Transitional | Transitional |
| GetBoard | GET | /v1/getboard | public | Later | Later | Later | Transitional | Transitional |
| GetTicker | GET | /v1/getticker | public | Yes | Yes | Phase1-Read | Transitional | Transitional |
| GetExecutionsPublic | GET | /v1/getexecutions | public | Later | Later | Later | Transitional | Transitional |
| GetBoardState | GET | /v1/getboardstate | public | Later | Later | Later | Transitional | Transitional |
| GetHealth | GET | /v1/gethealth | public | Later | Later | Later | Transitional | Transitional |
| GetFundingRate | GET | /v1/getfundingrate | public | Later | Later | Later | Transitional | Transitional |
| GetCorporateLeverage | GET | /v1/getcorporateleverage | public | Later | Later | Later | Transitional | Transitional |
| GetChats | GET | /v1/getchats | public | Later | Later | Later | Transitional | Transitional |
| GetPermissions | GET | /v1/me/getpermissions | private | Later | Later | Later | Transitional | Transitional |
| GetBalance | GET | /v1/me/getbalance | private | Yes | Yes | Phase1-Read | Transitional | Transitional |
| GetCollateral | GET | /v1/me/getcollateral | private | Later | Later | Later | Transitional | Transitional |
| GetCollateralAccounts | GET | /v1/me/getcollateralaccounts | private | Later | Later | Later | Transitional | Transitional |
| GetAddresses | GET | /v1/me/getaddresses | private | Later | Later | Later | Transitional | Transitional |
| GetCoinIns | GET | /v1/me/getcoinins | private | Later | Later | Later | Transitional | Transitional |
| GetCoinOuts | GET | /v1/me/getcoinouts | private | Later | Later | Later | Transitional | Transitional |
| GetBankAccounts | GET | /v1/me/getbankaccounts | private | Later | Later | Later | Transitional | Transitional |
| GetDeposits | GET | /v1/me/getdeposits | private | Later | Later | Later | Transitional | Transitional |
| Withdraw | POST | /v1/me/withdraw | private | Later | Later | Later | Transitional | Transitional |
| GetWithdrawals | GET | /v1/me/getwithdrawals | private | Later | Later | Later | Transitional | Transitional |
| SendChildOrder | POST | /v1/me/sendchildorder | private | Yes | Yes | Phase2-Write | Transitional | Transitional |
| SendParentOrder | POST | /v1/me/sendparentorder | private | Later | Later | Later | Transitional | Transitional |
| CancelChildOrder | POST | /v1/me/cancelchildorder | private | Later | Later | Later | Transitional | Transitional |
| CancelParentOrder | POST | /v1/me/cancelparentorder | private | Later | Later | Later | Transitional | Transitional |
| CancelAllChildOrders | POST | /v1/me/cancelallchildorders | private | Later | Later | Later | Transitional | Transitional |
| GetChildOrders | GET | /v1/me/getchildorders | private | Later | Later | Later | Transitional | Transitional |
| GetParentOrders | GET | /v1/me/getparentorders | private | Later | Later | Later | Transitional | Transitional |
| GetParentOrder | GET | /v1/me/getparentorder | private | Later | Later | Later | Transitional | Transitional |
| GetExecutionsPrivate | GET | /v1/me/getexecutions | private | Later | Later | Later | Transitional | Transitional |
| GetBalanceHistory | GET | /v1/me/getbalancehistory | private | Later | Later | Later | Transitional | Transitional |
| GetPositions | GET | /v1/me/getpositions | private | Later | Later | Later | Transitional | Transitional |
| GetCollateralHistory | GET | /v1/me/getcollateralhistory | private | Later | Later | Later | Transitional | Transitional |
| GetTradingCommission | GET | /v1/me/gettradingcommission | private | Later | Later | Later | Transitional | Transitional |

## Initial Rule

- Stage10 第1段階では `GetTicker`、`GetBalance`、`SendChildOrder` だけを `Yes` とする
- `GetTicker` と `GetBalance` は read path のため `Phase1-Read` とする
- `SendChildOrder` は write path のため `Phase2-Write` とする
- 初版では DTO 固定前のため、全 endpoint の `RequestDtoStatus` / `ResponseDtoStatus` は `Transitional` から開始する
