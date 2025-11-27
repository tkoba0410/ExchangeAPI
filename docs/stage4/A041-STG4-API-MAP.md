# A041-STG4-API-MAP bitFlyer API リストと実装状況（Stage4）

Stage4 時点での bitFlyer API 対応状況、抽象インターフェース対応表、固有エラーコード表（ドラフト）。

## 0. 分類（大区分）
- HTTP / Public GET: 市場データ・システム状態（例: getticker, getboard, getexecutions, getmarkets, gethealth, getboardstate, getfundingrate, getcorporateleverage, getchats）
- HTTP / Private GET: 口座/ポジション/約定/証拠金/注文照会/権限・状態（例: getbalance, getpositions, getexecutions, getcollateral, getchildorders, getparentorders, getpermissions, getboardstate, getbalancehistory, getcollateralaccounts, gettradingcommission）
- HTTP / Private POST: 注文送信/キャンセル（例: sendchildorder, cancelchildorder, sendparentorder, cancelparentorder, withdraw）
- WebSocket: リアルタイム配信（ticker/board/executions）

## 1. bitFlyer API リストと実装状況
| API | Method | プロトコル | 区分 | ステージ | 実装状況 | 備考 |
| --- | --- | --- | --- | --- | --- | --- |
| WebSocket (ticker/board/executions) | WS | WS | WS | Stage6 | 未実装 | リアルタイム系 |
| GET /v1/getmarkets | GET | HTTP | Public | Stage1? | 未実装 | 取扱商品一覧 |
| GET /v1/markets | GET | HTTP | Public | Stage1? | 未実装 | 取扱商品一覧 |
| GET /v1/getboard | GET | HTTP | Public | Stage1? | 未実装 | 板情報 |
| GET /v1/board | GET | HTTP | Public | Stage1? | 未実装 | 板情報 |
| GET /v1/getticker | GET | HTTP | Public | Stage1 | 済 | 現物ティッカー |
| GET /v1/ticker | GET | HTTP | Public | Stage1 | 未実装 | 現物ティッカー |
| GET /v1/getexecutions | GET | HTTP | Public | Stage1? | 未実装 | 約定履歴（板歩み） |
| GET /v1/executions | GET | HTTP | Public | Stage1? | 未実装 | 約定履歴（板歩み） |
| GET /v1/getboardstate | GET | HTTP | Public | Stage5+ | 未実装 | 相場状態 |
| GET /v1/gethealth | GET | HTTP | Public | Stage5+ | 未実装 | 取引所状態 |
| GET /v1/getfundingrate | GET | HTTP | Public | 時期未定 | 未実装 | ファンディングレート |
| GET /v1/getcorporateleverage | GET | HTTP | Public | 時期未定 | 未実装 | 法人最大レバレッジ |
| GET /v1/getchats | GET | HTTP | Public | 時期未定 | 未実装 | チャット |
| GET /v1/me/getpermissions | GET | HTTP | Private GET | Stage5+ | 未実装 | APIキー権限確認 |
| GET /v1/me/getbalance | GET | HTTP | Private GET | Stage2 | 済 | 残高 |
| GET /v1/me/getcollateral | GET | HTTP | Private GET | Stage4 | 済 | 証拠金 |
| GET /v1/me/getcollateralaccounts | GET | HTTP | Private GET | 時期未定 | 未実装 | 証拠金通貨別残高 |
| GET /v1/me/getaddresses | GET | HTTP | 入出金 | 時期未定 | 未実装 | 仮想通貨入金アドレス |
| GET /v1/me/getcoinins | GET | HTTP | 入出金 | 時期未定 | 未実装 | 仮想通貨入金履歴 |
| GET /v1/me/getcoinouts | GET | HTTP | 入出金 | 時期未定 | 未実装 | 仮想通貨出金履歴 |
| GET /v1/me/getbankaccounts | GET | HTTP | 入出金 | 時期未定 | 未実装 | 銀行口座一覧 |
| GET /v1/me/getdeposits | GET | HTTP | 入出金 | 時期未定 | 未実装 | 日本円入金履歴 |
| POST /v1/me/withdraw | POST | HTTP | 入出金 | 時期未定 | 未実装 | 日本円出金リクエスト |
| GET /v1/me/getwithdrawals | GET | HTTP | 入出金 | 時期未定 | 未実装 | 日本円出金履歴 |
| POST /v1/me/sendchildorder | POST | HTTP | Private POST | Stage3 | 済 | |
| POST /v1/me/cancelchildorder | POST | HTTP | Private POST | Stage4 | 済 | child_order_acceptance_id 優先 |
| POST /v1/me/sendparentorder | POST | HTTP | Private POST | Stage5+ | 未実装 | IFD/OCO/IFDOCO など |
| POST /v1/me/cancelparentorder | POST | HTTP | Private POST | Stage5+ | 未実装 | 親注文キャンセル |
| POST /v1/me/cancelallchildorders | POST | HTTP | Private POST | Stage4 | 済 | product_code 指定 |
| GET /v1/me/getchildorders | GET | HTTP | Private GET | Stage5+ | 未実装 | Open/History 取得 |
| GET /v1/me/getparentorders | GET | HTTP | Private GET | Stage5+ | 未実装 | 親注文一覧 |
| GET /v1/me/getparentorder | GET | HTTP | Private GET | Stage5+ | 未実装 | 親注文詳細 |
| GET /v1/me/getexecutions | GET | HTTP | Private GET | Stage4 | 済 | product_code 必須 |
| GET /v1/me/getbalancehistory | GET | HTTP | Private GET | 時期未定 | 未実装 | 残高履歴 |
| GET /v1/me/getpositions | GET | HTTP | Private GET | Stage4 | 済 | product_code 必須 |
| GET /v1/me/getcollateralhistory | GET | HTTP | 入出金 | 時期未定 | 未実装 | 証拠金履歴 |
| GET /v1/me/gettradingcommission | GET | HTTP | Private GET | 時期未定 | 未実装 | 手数料取得 |
