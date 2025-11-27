# A041-STG4-API-MAP bitFlyer API リストと実装状況（Stage4）

Stage4 時点での bitFlyer API 対応状況、抽象インターフェース対応表、固有エラーコード表（ドラフト）。

## 0. 分類（大区分）
- Public GET: 市場データ・システム状態（例: getticker, getboard, gethealth）
- Private GET: 口座/ポジション/約定/証拠金/注文照会/権限・状態（例: getbalance, getpositions, getexecutions, getcollateral, getchildorders, getparentorders, getpermissions, getboardstate）
- Private POST: 注文送信/キャンセル（例: sendchildorder, cancelchildorder, sendparentorder, cancelparentorder）
- WebSocket: リアルタイム配信（ticker/board/executions）

## 1. bitFlyer API リストと実装状況
| API | Method | 区分 | ステージ | 実装状況 | 備考 |
| --- | --- | --- | --- | --- | --- |
| /v1/getticker | GET | Public | Stage1 | 済 | 現物ティッカー |
| /v1/getboard | GET | Public | Stage1? | 未実装 | 板情報 |
| /v1/gethealth | GET | Public | Stage5+ | 未実装 | システムヘルス |
| /v1/me/getbalance | GET | Private GET | Stage2 | 済 | 残高 |
| /v1/me/getcollateral | GET | Private GET | Stage4 | 済 | 証拠金 |
| /v1/me/getpositions | GET | Private GET | Stage4 | 済 | product_code 必須 |
| /v1/me/getexecutions | GET | Private GET | Stage4 | 済 | product_code 必須 |
| /v1/me/getchildorders | GET | Private GET | Stage5+ | 未実装 | Open/History 取得 |
| /v1/me/getparentorders | GET | Private GET | Stage5+ | 未実装 | 親注文一覧 |
| /v1/me/getparentorder | GET | Private GET | Stage5+ | 未実装 | 親注文詳細 |
| /v1/me/getboardstate | GET | Private GET | Stage5+ | 未実装 | 相場状態 |
| /v1/me/getpermissions | GET | Private GET | Stage5+ | 未実装 | APIキー権限確認 |
| /v1/me/sendchildorder (MARKET) | POST | Private POST | Stage3 | 済 |  |
| /v1/me/sendchildorder (LIMIT/STOP/STOP_LIMIT) | POST | Private POST | Stage4 | 済 | time_in_force / minute_to_expire 対応 |
| /v1/me/cancelchildorder | POST | Private POST | Stage4 | 済 | child_order_acceptance_id 優先 |
| /v1/me/cancelallchildorders | POST | Private POST | Stage4 | 済 | product_code 指定 |
| /v1/me/sendparentorder | POST | Private POST | Stage5+ | 未実装 | IFD/OCO/IFDOCO など |
| /v1/me/cancelparentorder | POST | Private POST | Stage5+ | 未実装 | 親注文キャンセル |
| WebSocket (ticker/board/executions) | WS | WS | Stage6 | 未実装 | リアルタイム系 |
| /v1/me/getaddresses | GET | 入出金 | Scope外 | 未実装 | 仮想通貨入金アドレス |
| /v1/me/getcoinins | GET | 入出金 | Scope外 | 未実装 | 仮想通貨入金履歴 |
| /v1/me/getcoinouts | GET | 入出金 | Scope外 | 未実装 | 仮想通貨出金履歴 |
| /v1/me/getbankaccounts | GET | 入出金 | Scope外 | 未実装 | 銀行口座一覧 |
| /v1/me/getdeposits | GET | 入出金 | Scope外 | 未実装 | 日本円入金履歴 |
| /v1/me/getwithdrawals | GET | 入出金 | Scope外 | 未実装 | 日本円出金履歴 |
| /v1/me/withdraw | POST | 入出金 | Scope外 | 未実装 | 日本円出金リクエスト |
| /v1/me/getcollateralhistory | GET | 入出金 | Scope外 | 未実装 | 証拠金履歴 |

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
| SendParentOrderAsync（仮） | /v1/me/sendparentorder | 未設計 | 未実装 |
| CancelParentOrderAsync（仮） | /v1/me/cancelparentorder | 未設計 | 未実装 |
| GetBoardStateAsync（仮） | /v1/me/getboardstate | 未設計 | 未実装 |
| GetHealthAsync（仮） | /v1/gethealth | 未設計 | 未実装 |
| GetBoardAsync（仮） | /v1/getboard | 未設計 | 未実装 |
| GetPermissionsAsync（仮） | /v1/me/getpermissions | 未設計 | 未実装 |
| （入出金系は Scope 外） | /v1/me/getaddresses 他 | 未設計 | 未実装 |

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
