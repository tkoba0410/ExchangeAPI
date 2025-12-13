# bitFlyer Lightning REST API 実装一覧（Raw 層）

現在 Raw 層で実装済みの HTTP エンドポイント一覧と、抽象層での対応状況。

表は提示順に並べています。種別は Public/Private（Auth）。Raw=実装あり、抽象=抽象層で公開しているか、抽象メソッドは代表メソッド名。

| Endpoint | 種別 | Raw | 抽象 | 抽象メソッド |
| --- | --- | --- | --- | --- |
| GET /v1/getmarkets | Public | ○ | - | - |
| GET /v1/getboard | Public | ○ | ○ | `BitflyerMarketApi.GetOrderBookAsync` |
| GET /v1/getticker | Public | ○ | ○ | `BitflyerMarketApi.GetTickerAsync` |
| GET /v1/getexecutions | Public | ○ | ○ | `BitflyerMarketApi.GetMarketExecutionsAsync` |
| GET /v1/getboardstate | Public | ○ | - | - |
| GET /v1/gethealth | Public | ○ | - | - |
| GET /v1/getfundingrate | Public | ○ | - | - |
| GET /v1/getcorporateleverage | Public | ○ | - | - |
| GET /v1/getchats | Public | ○ | - | - |
| GET /v1/me/getpermissions | Private | ○ | - | - |
| GET /v1/me/getbalance | Private | ○ | ○ | `BitflyerAccountApi.GetBalancesAsync` |
| GET /v1/me/getcollateral | Private | ○ | ○ | `BitflyerMarginApi.GetCollateralAsync` |
| GET /v1/me/getcollateralaccounts | Private | ○ | - | - |
| GET /v1/me/getaddresses | Private | ○ | - | - |
| GET /v1/me/getcoinins | Private | ○ | - | - |
| GET /v1/me/getcoinouts | Private | ○ | - | - |
| GET /v1/me/getbankaccounts | Private | ○ | - | - |
| GET /v1/me/getdeposits | Private | ○ | - | - |
| POST /v1/me/withdraw | Private | ○ | - | - |
| GET /v1/me/getwithdrawals | Private | ○ | - | - |
| POST /v1/me/sendchildorder | Private | ○ | ○ | `BitflyerTradingApi.SendOrderAsync` |
| POST /v1/me/cancelchildorder | Private | ○ | ○ | `BitflyerTradingApi.CancelOrderAsync` |
| POST /v1/me/sendparentorder | Private | ○ | - | - |
| POST /v1/me/cancelparentorder | Private | ○ | - | - |
| POST /v1/me/cancelallchildorders | Private | ○ | △ | 抽象で未露出（必要なら拡張） |
| GET /v1/me/getchildorders | Private | ○ | ○ | `BitflyerTradingApi.GetOpenOrdersAsync` 等で利用 |
| GET /v1/me/getparentorders | Private | ○ | - | - |
| GET /v1/me/getparentorder | Private | ○ | - | - |
| GET /v1/me/getexecutions | Private | ○ | ○ | `BitflyerAccountApi.GetAccountExecutionsAsync` |
| GET /v1/me/getbalancehistory | Private | ○ | - | - |
| GET /v1/me/getpositions | Private | ○ | ○ | `BitflyerMarginApi.GetOpenPositionsAsync` |
| GET /v1/me/getcollateralhistory | Private | ○ | - | - |
| GET /v1/me/gettradingcommission | Private | ○ | - | - |
