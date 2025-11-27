# A042-STG4-ABSTRACT-MAP 抽象 API 対応表（Stage4）

抽象インターフェースと bitFlyer API の対応をまとめた表。ステージ進行に伴い更新する。

| 抽象インターフェース / メソッド | 区分 | 対応 bitFlyer API | DTO/Mapping | ステージ | 実装状況 |
| --- | --- | --- | --- | --- | --- |
| GetTickerAsync | Public | GET /v1/getticker | BitflyerTickerRaw → Ticker | Stage1 | 済 |
| GetBalancesAsync | Private GET | GET /v1/me/getbalance | BitflyerBalanceResponse → Balance | Stage2 | 済 |
| GetCollateralAsync | Private GET | GET /v1/me/getcollateral | BitflyerCollateralResponse → Collateral | Stage4 | 済 |
| GetPositionsAsync | Private GET | GET /v1/me/getpositions | BitflyerPositionResponse → Position | Stage4 | 済 |
| GetExecutionsAsync | Private GET | GET /v1/me/getexecutions | BitflyerExecutionResponse → Execution | Stage4 | 済 |
| GetChildOrdersAsync（仮） | Private GET | GET /v1/me/getchildorders | 未設計 | Stage5+ | 未 |
| GetParentOrdersAsync（仮） | Private GET | GET /v1/me/getparentorders | 未設計 | Stage5+ | 未 |
| GetParentOrderAsync（仮） | Private GET | GET /v1/me/getparentorder | 未設計 | Stage5+ | 未 |
| GetPermissionsAsync（仮） | Private GET | GET /v1/me/getpermissions | 未設計 | Stage5+ | 未 |
| GetBalanceHistoryAsync（仮） | Private GET | GET /v1/me/getbalancehistory | 未設計 | 時期未定 | 未 |
| GetCollateralAccountsAsync（仮） | Private GET | GET /v1/me/getcollateralaccounts | 未設計 | 時期未定 | 未 |
| GetCollateralHistoryAsync（仮） | Private GET | GET /v1/me/getcollateralhistory | 未設計 | 時期未定 | 未 |
| GetTradingCommissionAsync（仮） | Private GET | GET /v1/me/gettradingcommission | 未設計 | 時期未定 | 未 |
| SendOrderAsync | Private POST | POST /v1/me/sendchildorder | OrderRequest → BitflyerSendChildOrderRequest | Stage3/4 | 済（MARKET/LIMIT/STOP/STOP_LIMIT） |
| CancelOrderAsync | Private POST | POST /v1/me/cancelchildorder | BitflyerCancelChildOrderRequest | Stage4 | 済 |
| CancelAllOrdersAsync | Private POST | POST /v1/me/cancelallchildorders | BitflyerCancelAllChildOrdersRequest | Stage4 | 済 |
| SendParentOrderAsync（仮） | Private POST | POST /v1/me/sendparentorder | 未設計 | Stage5+ | 未 |
| CancelParentOrderAsync（仮） | Private POST | POST /v1/me/cancelparentorder | 未設計 | Stage5+ | 未 |
| SubscribeTicker/Board/Executions（仮） | WS | WS (ticker/board/executions) | 未設計 | Stage6 | 未 |

備考:
- 「未設計」は抽象メソッド・DTO 形状を未定義。実装時に追加する。
- 入出金系は抽象インターフェース未定義のため本表では割愛（時期未定）。
