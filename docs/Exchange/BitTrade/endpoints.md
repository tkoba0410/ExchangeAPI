# BitTrade Endpoints

Source URL:
- https://api-doc.bittrade.co.jp/

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
| bittrade:GET:/v1/common/symbols | GET | /v1/common/symbols | unclassified | official | https://api-doc.bittrade.co.jp/ (取引ペア情報) | scope_unknown,needs_verification |
| bittrade:GET:/v1/common/currencys | GET | /v1/common/currencys | unclassified | official | https://api-doc.bittrade.co.jp/ (対応取引通貨) | scope_unknown,needs_verification |
| bittrade:GET:/v1/common/timestamp | GET | /v1/common/timestamp | unclassified | official | https://api-doc.bittrade.co.jp/ (システム時間を調べる) | scope_unknown,needs_verification |
| bittrade:GET:/market/history/kline | GET | /market/history/kline | unclassified | official | https://api-doc.bittrade.co.jp/ (ローソク足) | scope_unknown,needs_verification |
| bittrade:GET:/market/detail/merged | GET | /market/detail/merged | unclassified | official | https://api-doc.bittrade.co.jp/ (ティッカー) | scope_unknown,needs_verification |
| bittrade:GET:/market/tickers | GET | /market/tickers | unclassified | official | https://api-doc.bittrade.co.jp/ (全取引ペアの相場情報) | scope_unknown,needs_verification |
| bittrade:GET:/market/depth | GET | /market/depth | unclassified | official | https://api-doc.bittrade.co.jp/ (板情報) | scope_unknown,needs_verification |
| bittrade:GET:/market/trade | GET | /market/trade | unclassified | official | https://api-doc.bittrade.co.jp/ (直近の取引データ) | scope_unknown,needs_verification |
| bittrade:GET:/market/history/trade | GET | /market/history/trade | unclassified | official | https://api-doc.bittrade.co.jp/ (取引履歴の取得) | scope_unknown,needs_verification |
| bittrade:GET:/v1/account/accounts | GET | /v1/account/accounts | private | official | https://api-doc.bittrade.co.jp/ (ユーザアカウント) |  |
| bittrade:GET:/v1/account/accounts/{account-id}/balance | GET | /v1/account/accounts/{account-id}/balance | private | official | https://api-doc.bittrade.co.jp/ (残高照合) |  |
| bittrade:POST:/v1/order/orders/place | POST | /v1/order/orders/place | unclassified | official | https://api-doc.bittrade.co.jp/ (注文実行) | scope_unknown,needs_verification |
| bittrade:GET:/v1/order/openOrders | GET | /v1/order/openOrders | unclassified | official | https://api-doc.bittrade.co.jp/ (未約定注文一覧) | scope_unknown,needs_verification |
| bittrade:POST:/v1/order/orders/{order-id}/submitcancel | POST | /v1/order/orders/{order-id}/submitcancel | unclassified | official | https://api-doc.bittrade.co.jp/ (注文キャンセル) | scope_unknown,needs_verification |
| bittrade:POST:/v1/order/orders/batchcancel | POST | /v1/order/orders/batchcancel | unclassified | official | https://api-doc.bittrade.co.jp/ (注文の一括キャンセル) | scope_unknown,needs_verification |
| bittrade:POST:/v1/order/orders/batchCancelOpenOrders | POST | /v1/order/orders/batchCancelOpenOrders | unclassified | official | https://api-doc.bittrade.co.jp/ (条件付き注文の一括キャンセル) | scope_unknown,needs_verification |
| bittrade:GET:/v1/order/orders/{order-id} | GET | /v1/order/orders/{order-id} | unclassified | official | https://api-doc.bittrade.co.jp/ (注文の照会) | scope_unknown,needs_verification |
| bittrade:GET:/v1/order/orders/{order-id}/matchresults | GET | /v1/order/orders/{order-id}/matchresults | unclassified | official | https://api-doc.bittrade.co.jp/ (注文の約定詳細) | scope_unknown,needs_verification |
| bittrade:GET:/v1/order/orders | GET | /v1/order/orders | unclassified | official | https://api-doc.bittrade.co.jp/ (注文履歴の検索) | scope_unknown,needs_verification |
| bittrade:GET:/v1/order/matchresults | GET | /v1/order/matchresults | unclassified | official | https://api-doc.bittrade.co.jp/ (約定履歴の検索) | scope_unknown,needs_verification |
| bittrade:POST:/v1/dw/withdraw/api/create | POST | /v1/dw/withdraw/api/create | private | official | https://api-doc.bittrade.co.jp/ (暗号資産の出金申請) |  |
| bittrade:POST:/v1/dw/withdraw-virtual/{withdraw-id}/cancel | POST | /v1/dw/withdraw-virtual/{withdraw-id}/cancel | private | official | https://api-doc.bittrade.co.jp/ (暗号資産の出金のキャンセル) |  |
| bittrade:GET:/v1/query/deposit-withdraw | GET | /v1/query/deposit-withdraw | private | official | https://api-doc.bittrade.co.jp/ (入出金記録) |  |
| bittrade:POST:/v1/retail/order/place | POST | /v1/retail/order/place | private | official | https://api-doc.bittrade.co.jp/ (販売所での注文) |  |
| bittrade:GET:/v1/retail/order/list | GET | /v1/retail/order/list | unclassified | official | https://api-doc.bittrade.co.jp/ (販売所注文履歴) | scope_unknown,needs_verification |
| bittrade:GET:/v1/retail/maintain/time | GET | /v1/retail/maintain/time | unclassified | official | https://api-doc.bittrade.co.jp/ (販売所メンテナンス時間) | scope_unknown,needs_verification |

Notes:
- /v1/query/deposit-withdraw Query: currency=xrp&type=deposit&from=5&size=12
