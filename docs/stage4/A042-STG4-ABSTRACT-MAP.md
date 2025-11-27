# A042-STG4-ABSTRACT-MAP 抽象 API 対応表（Stage4）

抽象インターフェースと bitFlyer API の対応をまとめた表。基本セットに絞り、未設計は「未」とする。

| 抽象インターフェース / メソッド | 区分 | 対応 bitFlyer API | DTO/Mapping | ステージ | 実装状況 |
| --- | --- | --- | --- | --- | --- |
| GetTickerAsync | Public | GET /v1/getticker | BitflyerTickerRaw → Ticker | Stage1 | 済 |
| GetBalancesAsync | Private GET | GET /v1/me/getbalance | BitflyerBalanceResponse → Balance | Stage2 | 済 |
| GetCollateralAsync | Private GET | GET /v1/me/getcollateral | BitflyerCollateralResponse → Collateral | Stage4 | 済 |
| GetPositionsAsync | Private GET | GET /v1/me/getpositions | BitflyerPositionResponse → Position | Stage4 | 済 |
| GetExecutionsAsync | Private GET | GET /v1/me/getexecutions | BitflyerExecutionResponse → Execution | Stage4 | 済 |
| PlaceOrderAsync | Private POST | POST /v1/me/sendchildorder / （STOP系で親注文を使う場合は sendparentorder） | OrderRequest → BitflyerSendChildOrderRequest（親注文は未設計） | Stage3/4 | 子注文: 済（MARKET/LIMIT/STOP/STOP_LIMIT） / 親注文: 未 |
| CancelOrderAsync | Private POST | POST /v1/me/cancelchildorder | BitflyerCancelChildOrderRequest | Stage4 | 済 |
| CancelAllOrdersAsync | Private POST | POST /v1/me/cancelallchildorders | BitflyerCancelAllChildOrdersRequest | Stage4 | 済 |
| SubscribeTicker/Board/Executions（仮） | WS | WS (ticker/board/executions) | 未設計 | Stage6 | 未 |
| OrderId マッピング（補助） | N/A | InMemoryOrderIdMapper (optional) | local ↔ server ID を保持 | N/A | 任意 |

備考:
- 親注文系・入出金系・履歴系は現時点では抽象に含めない。必要になれば追加する。
