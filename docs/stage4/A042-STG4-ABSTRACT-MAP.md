# A042-STG4-ABSTRACT-MAP 抽象 API 対応表（Stage4）

抽象インターフェースと bitFlyer API の対応をまとめた表。基本セットに絞り、未設計は「未」とする。

| DTO | 抽象インターフェース / メソッド | スコープ (Public/Private) | HTTP/WS | HTTPメソッド | 対応 bitFlyer API | ステージ | 実装状況 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Board | GetBoardAsync | Public | HTTP | GET | /v1/getboard | Stage1 | 済 |
| Ticker | GetTickerAsync | Public | HTTP | GET | /v1/getticker | Stage1 | 済 |
| Candlestick[] | ListCandlesticksAsync | Public | HTTP | GET | bitbank（参考） `/v1/candlestick/{pair}/{timescale}` | Stage5 | 未実装 |
| Balance[] | GetBalancesAsync | Private | HTTP | GET | /v1/me/getbalance | Stage2 | 済 |
| Collateral | GetCollateralAsync | Private | HTTP | GET | /v1/me/getcollateral | Stage4 | 済 |
| Position[] | ListPositionsAsync | Private | HTTP | GET | /v1/me/getpositions | Stage4 | 済 |
| Execution[] | ListExecutionsAsync | Private | HTTP | GET | /v1/me/getexecutions | Stage4 | 済 |
| OpenOrder[] | ListOpenOrdersAsync | Private | HTTP | GET | /v1/me/getchildorders?child_order_state=ACTIVE | Stage4 | 済 |
| OrderResult | PlaceOrderAsync | Private | HTTP | POST | /v1/me/sendchildorder（STOP系で親注文を使う場合は sendparentorder） | Stage3/4 | 子注文: 済（MARKET/LIMIT/STOP/STOP_LIMIT） / 親注文: 未 |
| CancelResult | CancelOrderAsync | Private | HTTP | POST | /v1/me/cancelchildorder | Stage4 | 済 |
| CancelResult | CancelAllOrdersAsync | Private | HTTP | POST | /v1/me/cancelallchildorders | Stage4 | 済 |
| (WS DTO 検討中) | SubscribeTicker/Board/Executions（仮） | Public | WS | - | WS (ticker/board/executions) | Stage6 | 未 |
| (補助) | OrderId マッピング | N/A | - | - | InMemoryOrderIdMapper (optional) | N/A | 任意 |

備考:
- 親注文系・入出金系・履歴系は現時点では抽象に含めない。必要になれば追加する。
