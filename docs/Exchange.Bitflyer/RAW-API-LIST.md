# bitFlyer Lightning REST API 実装一覧（Raw 層）

現在 Raw 層で実装済みの HTTP エンドポイント一覧と、抽象層での対応状況。

### Public (GET)
| Endpoint | Raw | 抽象 | 抽象メソッド |
| --- | --- | --- | --- |
| /v1/getticker (/v1/ticker) | ○ | ○ | `BitflyerMarketApi.GetTickerAsync` |
| /v1/getboard (/v1/board) | ○ | ○ | `BitflyerMarketApi.GetOrderBookAsync` |
| /v1/getexecutions (/v1/executions) | ○ | ○ | `BitflyerMarketApi.GetMarketExecutionsAsync` |
| /v1/getmarkets (/v1/markets) + /usa + /eu | ○ | - | - |
| /v1/getchats (/usa,/eu) | ○ | - | - |
| /v1/gethealth | ○ | - | - |
| /v1/getboardstate | ○ | - | - |
| /v1/getcorporateleverage | ○ | - | - |
| /v1/getfundingrate | ○ | - | - |

### Private (GET)
| Endpoint | Raw | 抽象 | 抽象メソッド |
| --- | --- | --- | --- |
| /v1/me/getpermissions | ○ | - | - |
| /v1/me/getbalance | ○ | ○ | `BitflyerAccountApi.GetBalancesAsync` |
| /v1/me/getcollateral | ○ | ○ | `BitflyerMarginApi.GetCollateralAsync` |
| /v1/me/getcollateralaccounts | ○ | - | - |
| /v1/me/getchildorders | ○ | ○ | `BitflyerTradingApi.GetOpenOrdersAsync` 等で利用 |
| /v1/me/getparentorders | ○ | - | - |
| /v1/me/getparentorder | ○ | - | - |
| /v1/me/getexecutions | ○ | ○ | `BitflyerAccountApi.GetAccountExecutionsAsync` |
| /v1/me/getbalancehistory | ○ | - | - |
| /v1/me/getpositions | ○ | ○ | `BitflyerMarginApi.GetOpenPositionsAsync` |
| /v1/me/getcollateralhistory | ○ | - | - |
| /v1/me/gettradingcommission | ○ | - | - |
| /v1/me/getaddresses | ○ | - | - |
| /v1/me/getcoinins | ○ | - | - |
| /v1/me/getcoinouts | ○ | - | - |
| /v1/me/getdeposits | ○ | - | - |
| /v1/me/getwithdrawals | ○ | - | - |
| /v1/me/getbankaccounts | ○ | - | - |

### Private (POST)
| Endpoint | Raw | 抽象 | 抽象メソッド |
| --- | --- | --- | --- |
| /v1/me/sendchildorder | ○ | ○ | `BitflyerTradingApi.SendOrderAsync` |
| /v1/me/cancelchildorder | ○ | ○ | `BitflyerTradingApi.CancelOrderAsync` |
| /v1/me/cancelallchildorders | ○ | △ | 抽象で未露出（必要に応じ拡張） |
| /v1/me/sendparentorder | ○ | - | - |
| /v1/me/cancelparentorder | ○ | - | - |
| /v1/me/withdraw | ○ | - | - |
