# A041-STG4-API-MAP bitFlyer API リストと実装状況（Stage4）

Stage4 時点での bitFlyer API 対応状況、抽象インターフェース対応表、固有エラーコード表（ドラフト）。

## 1. bitFlyer API リストと実装状況
| API | Method | ステージ | 実装状況 | 備考 |
| --- | --- | --- | --- | --- |
| /v1/getticker | GET | Stage1 | 済 | Public |
| /v1/me/getbalance | GET | Stage2 | 済 | Private GET |
| /v1/me/sendchildorder (MARKET) | POST | Stage3 | 済 | Private POST |
| /v1/me/sendchildorder (LIMIT/STOP/STOP_LIMIT) | POST | Stage4 | 済 | time_in_force / minute_to_expire 対応 |
| /v1/me/cancelchildorder | POST | Stage4 | 済 | child_order_acceptance_id 優先 |
| /v1/me/cancelallchildorders | POST | Stage4 | 済 | product_code 指定 |
| /v1/me/getpositions | GET | Stage4 | 済 | product_code 必須 |
| /v1/me/getexecutions | GET | Stage4 | 済 | product_code 必須 |
| /v1/me/getcollateral | GET | Stage4 | 済 |  |
| /v1/me/getchildorders | GET | Stage5+ | 未実装 | Open/History 取得 |
| /v1/me/getparentorders | GET | Stage5+ | 未実装 | 親注文一覧 |
| /v1/me/getparentorder | GET | Stage5+ | 未実装 | 親注文詳細 |
| WebSocket (ticker/board/executions) | WS | Stage6 | 未実装 | リアルタイム系 |

## 2. 抽象インターフェース対応表
| 抽象インターフェース | bitFlyer API | DTO/Mapping | 実装状況 |
| --- | --- | --- | --- |
| GetTickerAsync | /v1/getticker | BitflyerTickerRaw → Ticker | 済 |
| GetBalancesAsync | /v1/me/getbalance | BitflyerBalanceResponse → Balance | 済 |
| SendOrderAsync | /v1/me/sendchildorder | OrderRequest → BitflyerSendChildOrderRequest | 済（MARKET/LIMIT/STOP/STOP_LIMIT） |
| CancelOrderAsync | /v1/me/cancelchildorder | BitflyerCancelChildOrderRequest | 済 |
| CancelAllOrdersAsync | /v1/me/cancelallchildorders | BitflyerCancelAllChildOrdersRequest | 済 |
| GetPositionsAsync | /v1/me/getpositions | BitflyerPositionResponse → Position | 済 |
| GetExecutionsAsync | /v1/me/getexecutions | BitflyerExecutionResponse → Execution | 済 |
| GetCollateralAsync | /v1/me/getcollateral | BitflyerCollateralResponse → Collateral | 済 |
| GetChildOrdersAsync（仮） | /v1/me/getchildorders | 未設計 | 未実装 |
| GetParentOrdersAsync（仮） | /v1/me/getparentorders | 未設計 | 未実装 |
| GetParentOrderAsync（仮） | /v1/me/getparentorder | 未設計 | 未実装 |
| SubscribeTicker/Board/Executions（仮） | WS | 未設計 | 未実装 |

## 3. bitFlyer 固有エラーコード表（ドラフト）
| error_code | カテゴリ | リトライ可否 | 実装状況 | 備考 |
| --- | --- | --- | --- | --- |
| INSUFFICIENT_FUNDS | Balance | No | 済 |  |
| NO_POSITION | Balance | No | 済 |  |
| INVALID_ORDER | Request | No | 済 |  |
| INVALID_PRODUCT / PRODUCT_NOT_FOUND | Request | No | 済 |  |
| LIMIT_OVER | Request | No | 済 |  |
| ORDER_NOT_ACCEPTABLE | Request | No | 済 |  |
| INVALID_REQUEST | Request | No | 済 |  |
| PARAM_ERROR | Request | No | 済 |  |
| AUTHENTICATION_ERROR | Auth | No | 済 |  |
| PERMISSION_DENIED | Auth | No | 済 |  |
| TOO_MANY_REQUESTS | RateLimit | Yes | 済 | Retry-After 尊重 |
| TIMEOUT | Network | Yes | 済 |  |
| SERVICE_UNAVAILABLE | Server | Yes | 済 | バックオフ |
| INTERNAL_ERROR | Server | Yes | 済 | バックオフ |
| （その他未分類） | Unknown | Case by case | 未 | 追加次第反映 |

※ bitFlyer の公式ドキュメントに合わせて精査し、追加があればこの表とコード（MapErrorCategory）を同期すること。
