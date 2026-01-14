# BitFlyer Endpoints

Source URL:
- https://lightning.bitflyer.com/docs

この文書は実装者向けのエンドポイント一覧です。正本は公式ドキュメントであり、詳細は公式を参照してください。
`basis` は endpoint の根拠種別（official/assumed/observed）を表します。

## Columns

- id: `<exchange-lowercase>:<METHOD>:<PATH>` 形式の一意ID
- method: HTTP メソッド
- path: エンドポイントのパス
- scope: public / private / unclassified
- basis: official / assumed / observed
- source: 公式参照先（URL とセクション）
- flags: 要確認や補足の短いタグ

## Endpoints

| id | method | path | scope | basis | source | flags |
| -- | ------ | ---- | ----- | ----- | ------ | ----- |
| bitflyer:GET:/v1/getmarkets | GET | /v1/getmarkets | public | official | https://lightning.bitflyer.com/docs (マーケットの一覧) | duplicate_alias |
| bitflyer:GET:/v1/markets | GET | /v1/markets | public | official | https://lightning.bitflyer.com/docs (マーケットの一覧) | duplicate_alias |
| bitflyer:GET:/v1/getboard | GET | /v1/getboard | public | official | https://lightning.bitflyer.com/docs (板情報) | duplicate_alias |
| bitflyer:GET:/v1/board | GET | /v1/board | public | official | https://lightning.bitflyer.com/docs (板情報) | duplicate_alias |
| bitflyer:GET:/v1/getticker | GET | /v1/getticker | public | official | https://lightning.bitflyer.com/docs (Ticker) | duplicate_alias |
| bitflyer:GET:/v1/ticker | GET | /v1/ticker | public | official | https://lightning.bitflyer.com/docs (Ticker) | duplicate_alias |
| bitflyer:GET:/v1/getexecutions | GET | /v1/getexecutions | public | official | https://lightning.bitflyer.com/docs (約定履歴) | duplicate_alias |
| bitflyer:GET:/v1/executions | GET | /v1/executions | public | official | https://lightning.bitflyer.com/docs (約定履歴) | duplicate_alias |
| bitflyer:GET:/v1/getboardstate | GET | /v1/getboardstate | public | official | https://lightning.bitflyer.com/docs (板の状態) |  |
| bitflyer:GET:/v1/gethealth | GET | /v1/gethealth | public | official | https://lightning.bitflyer.com/docs (取引所の状態) |  |
| bitflyer:GET:/v1/getfundingrate | GET | /v1/getfundingrate | public | official | https://lightning.bitflyer.com/docs (ファンディングレート) |  |
| bitflyer:GET:/v1/getcorporateleverage | GET | /v1/getcorporateleverage | public | official | https://lightning.bitflyer.com/docs (法人アカウント最大レバレッジ) |  |
| bitflyer:GET:/v1/getchats | GET | /v1/getchats | public | official | https://lightning.bitflyer.com/docs (チャット) |  |
| bitflyer:GET:/v1/me/getpermissions | GET | /v1/me/getpermissions | private | official | https://lightning.bitflyer.com/docs (API キーの権限を取得) |  |
| bitflyer:GET:/v1/me/getbalance | GET | /v1/me/getbalance | private | official | https://lightning.bitflyer.com/docs (資産残高を取得) |  |
| bitflyer:GET:/v1/me/getcollateral | GET | /v1/me/getcollateral | private | official | https://lightning.bitflyer.com/docs (証拠金の状態を取得) |  |
| bitflyer:GET:/v1/me/getcollateralaccounts | GET | /v1/me/getcollateralaccounts | private | official | https://lightning.bitflyer.com/docs (証拠金の状態を取得) |  |
| bitflyer:GET:/v1/me/getaddresses | GET | /v1/me/getaddresses | private | official | https://lightning.bitflyer.com/docs (預入用アドレス取得) |  |
| bitflyer:GET:/v1/me/getcoinins | GET | /v1/me/getcoinins | private | official | https://lightning.bitflyer.com/docs (仮想通貨預入履歴) |  |
| bitflyer:GET:/v1/me/getcoinouts | GET | /v1/me/getcoinouts | private | official | https://lightning.bitflyer.com/docs (仮想通貨送付履歴) |  |
| bitflyer:GET:/v1/me/getbankaccounts | GET | /v1/me/getbankaccounts | private | official | https://lightning.bitflyer.com/docs (銀行口座一覧取得) |  |
| bitflyer:GET:/v1/me/getdeposits | GET | /v1/me/getdeposits | private | official | https://lightning.bitflyer.com/docs (入金履歴) |  |
| bitflyer:POST:/v1/me/withdraw | POST | /v1/me/withdraw | private | official | https://lightning.bitflyer.com/docs (出金) |  |
| bitflyer:GET:/v1/me/getwithdrawals | GET | /v1/me/getwithdrawals | private | official | https://lightning.bitflyer.com/docs (出金履歴) |  |
| bitflyer:POST:/v1/me/sendchildorder | POST | /v1/me/sendchildorder | private | official | https://lightning.bitflyer.com/docs (新規注文を出す) |  |
| bitflyer:POST:/v1/me/cancelchildorder | POST | /v1/me/cancelchildorder | private | official | https://lightning.bitflyer.com/docs (注文をキャンセルする) |  |
| bitflyer:POST:/v1/me/sendparentorder | POST | /v1/me/sendparentorder | private | official | https://lightning.bitflyer.com/docs (新規の親注文を出す（特殊注文）) |  |
| bitflyer:POST:/v1/me/cancelparentorder | POST | /v1/me/cancelparentorder | private | official | https://lightning.bitflyer.com/docs (親注文をキャンセルする) |  |
| bitflyer:POST:/v1/me/cancelallchildorders | POST | /v1/me/cancelallchildorders | private | official | https://lightning.bitflyer.com/docs (すべての注文をキャンセルする) |  |
| bitflyer:GET:/v1/me/getchildorders | GET | /v1/me/getchildorders | private | official | https://lightning.bitflyer.com/docs (注文の一覧を取得) |  |
| bitflyer:GET:/v1/me/getparentorders | GET | /v1/me/getparentorders | private | official | https://lightning.bitflyer.com/docs (親注文の一覧を取得) |  |
| bitflyer:GET:/v1/me/getparentorder | GET | /v1/me/getparentorder | private | official | https://lightning.bitflyer.com/docs (親注文の詳細を取得) |  |
| bitflyer:GET:/v1/me/getexecutions | GET | /v1/me/getexecutions | private | official | https://lightning.bitflyer.com/docs (約定の一覧を取得) |  |
| bitflyer:GET:/v1/me/getbalancehistory | GET | /v1/me/getbalancehistory | private | official | https://lightning.bitflyer.com/docs (残高履歴を取得) |  |
| bitflyer:GET:/v1/me/getpositions | GET | /v1/me/getpositions | private | official | https://lightning.bitflyer.com/docs (建玉の一覧を取得) |  |
| bitflyer:GET:/v1/me/getcollateralhistory | GET | /v1/me/getcollateralhistory | private | official | https://lightning.bitflyer.com/docs (証拠金の変動履歴を取得) |  |
| bitflyer:GET:/v1/me/gettradingcommission | GET | /v1/me/gettradingcommission | private | official | https://lightning.bitflyer.com/docs (取引手数料を取得) |  |
