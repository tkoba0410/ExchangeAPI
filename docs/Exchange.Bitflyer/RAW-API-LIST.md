# bitFlyer Lightning REST API 実装一覧（Raw 層）

現在 Raw 層で実装済みの HTTP エンドポイント一覧と、抽象層での対応状況。

表は提示順に並べています（Raw=実装あり、抽象=抽象層で公開しているか、抽象メソッドは代表メソッド名）。

| Endpoint | Raw | 抽象 | 抽象メソッド |
| --- | --- | --- | --- |
| GET /v1/getmarkets | ○ | - | - |
| GET /v1/getboard | ○ | ○ | `BitflyerMarketApi.GetOrderBookAsync` |
| GET /v1/getticker | ○ | ○ | `BitflyerMarketApi.GetTickerAsync` |
| GET /v1/getexecutions | ○ | ○ | `BitflyerMarketApi.GetMarketExecutionsAsync` |
| GET /v1/getboardstate | ○ | - | - |
| GET /v1/gethealth | ○ | - | - |
| GET /v1/getfundingrate | ○ | - | - |
| GET /v1/getcorporateleverage | ○ | - | - |
| GET /v1/getchats | ○ | - | - |
| GET /v1/me/getpermissions | ○ | - | - |
| GET /v1/me/getbalance | ○ | ○ | `BitflyerAccountApi.GetBalancesAsync` |
| GET /v1/me/getcollateral | ○ | ○ | `BitflyerMarginApi.GetCollateralAsync` |
| GET /v1/me/getcollateralaccounts | ○ | - | - |
| GET /v1/me/getaddresses | ○ | - | - |
| GET /v1/me/getcoinins | ○ | - | - |
| GET /v1/me/getcoinouts | ○ | - | - |
| GET /v1/me/getbankaccounts | ○ | - | - |
| GET /v1/me/getdeposits | ○ | - | - |
| POST /v1/me/withdraw | ○ | - | - |
| GET /v1/me/getwithdrawals | ○ | - | - |
| POST /v1/me/sendchildorder | ○ | ○ | `BitflyerTradingApi.SendOrderAsync` |
| POST /v1/me/cancelchildorder | ○ | ○ | `BitflyerTradingApi.CancelOrderAsync` |
| POST /v1/me/sendparentorder | ○ | - | - |
| POST /v1/me/cancelparentorder | ○ | - | - |
| POST /v1/me/cancelallchildorders | ○ | △ | 抽象で未露出（必要なら拡張） |
| GET /v1/me/getchildorders | ○ | ○ | `BitflyerTradingApi.GetOpenOrdersAsync` 等で利用 |
| GET /v1/me/getparentorders | ○ | - | - |
| GET /v1/me/getparentorder | ○ | - | - |
| GET /v1/me/getexecutions | ○ | ○ | `BitflyerAccountApi.GetAccountExecutionsAsync` |
| GET /v1/me/getbalancehistory | ○ | - | - |
| GET /v1/me/getpositions | ○ | ○ | `BitflyerMarginApi.GetOpenPositionsAsync` |
| GET /v1/me/getcollateralhistory | ○ | - | - |
| GET /v1/me/gettradingcommission | ○ | - | - |
