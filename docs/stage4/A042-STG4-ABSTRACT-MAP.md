# A042-STG4-ABSTRACT-MAP 抽象 API 対応表（Stage4）

抽象インターフェースと bitFlyer API の対応をまとめた表。基本セットに絞り、未設計は「未」とする。

| 抽象インターフェース / メソッド | スコープ (Public/Private) | HTTP/WS | HTTPメソッド | 対応 bitFlyer API | ステージ | 実装状況 |
| --- | --- | --- | --- | --- | --- | --- |
| GetBoardAsync | Public | HTTP | GET | /v1/getboard | Stage1 | 済 |
| GetTickerAsync | Public | HTTP | GET | /v1/getticker | Stage1 | 済 |
| GetBalancesAsync | Private | HTTP | GET | /v1/me/getbalance | Stage2 | 済 |
| GetCollateralAsync | Private | HTTP | GET | /v1/me/getcollateral | Stage4 | 済 |
| ListPositionsAsync | Private | HTTP | GET | /v1/me/getpositions | Stage4 | 済 |
| ListExecutionsAsync | Private | HTTP | GET | /v1/me/getexecutions | Stage4 | 済 |
| ListOpenOrdersAsync | Private | HTTP | GET | /v1/me/getchildorders?child_order_state=ACTIVE | Stage4 | 済 |
| PlaceOrderAsync | Private | HTTP | POST | /v1/me/sendchildorder（STOP系で親注文を使う場合は sendparentorder） | Stage3/4 | 子注文: 済（MARKET/LIMIT/STOP/STOP_LIMIT） / 親注文: 未 |
| CancelOrderAsync | Private | HTTP | POST | /v1/me/cancelchildorder | Stage4 | 済 |
| CancelAllOrdersAsync | Private | HTTP | POST | /v1/me/cancelallchildorders | Stage4 | 済 |
| SubscribeTicker/Board/Executions（仮） | Public | WS | - | WS (ticker/board/executions) | Stage6 | 未 |
| OrderId マッピング（補助） | N/A | - | - | InMemoryOrderIdMapper (optional) | N/A | 任意 |

備考:
- 親注文系・入出金系・履歴系は現時点では抽象に含めない。必要になれば追加する。
