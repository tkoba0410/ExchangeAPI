# EndpointId 命名規則: Bittrade

## 位置づけ

本書は、Bittrade における **EndpointId（Constants）の命名規則**を定める。

* 正本は `ExchangeApi.Exchanges.Bittrade.Wire.Constants.BittradeEndpoints`
* 本書は「共通条文」に従属する取引所別規則である

---

## 基本方針（Bittrade）

Bittrade では、同一 path に対して GET / POST が存在するため、
**EndpointId に HTTP Method を含める**。

これにより、

* EndpointId の一意性を保証する
* 後段の派生規則を単純化する

---

## 命名規則

```
<Method><PascalCase(PathWithoutVersion)>
```

### 詳細ルール

1. HTTP Method を必ず先頭に含める（Get / Post）
2. `/v1` は識別子に含めない
3. パス残部を英単語境界で分割し PascalCase とする
4. EndpointId は Bittrade 内で一意でなければならない

---

## EndpointId 一覧（Bittrade）

| EndpointId                           | Method | Path                                  |
| ------------------------------------ | ------ | ------------------------------------- |
| GetMarketDetailMerged                | GET    | market/detail/merged                  |
| GetMarketDepth                       | GET    | market/depth                          |
| GetMarketTrade                       | GET    | market/trade                          |
| GetMarketHistoryKline                | GET    | market/history/kline                  |
| GetMarketTickers                     | GET    | market/tickers                        |
| GetMarketHistoryTrade                | GET    | market/history/trade                  |
| GetCommonTimestamp                   | GET    | v1/common/timestamp                   |
| GetCommonSymbols                     | GET    | v1/common/symbols                     |
| GetCommonCurrencies                  | GET    | v1/common/currencys                   |
| GetRetailMaintainTime                | GET    | v1/retail/maintain/time               |
| GetAccountAccounts                   | GET    | v1/account/accounts                   |
| GetOrderOpenOrders                   | GET    | v1/order/openOrders                   |
| GetOrderOrders                       | GET    | v1/order/orders                       |
| GetOrderMatchResults                 | GET    | v1/order/matchresults                 |
| PostOrderOrdersPlace                 | POST   | v1/order/orders/place                 |
| PostOrderOrdersBatchCancel           | POST   | v1/order/orders/batchcancel           |
| PostOrderOrdersBatchCancelOpenOrders | POST   | v1/order/orders/batchCancelOpenOrders |
| GetRetailOrderList                   | GET    | v1/retail/order/list                  |
| PostRetailOrderPlace                 | POST   | v1/retail/order/place                 |
| GetQueryDepositWithdraw              | GET    | v1/query/deposit-withdraw             |
| PostDwWithdrawApiCreate              | POST   | v1/dw/withdraw/api/create             |
| PostDwWithdrawVirtual                | POST   | v1/dw/withdraw-virtual                |

---

## 備考

* EndpointId からの派生は **共通条文の派生規則**に従う
* 命名の簡略化・意味統一は行わない
* Facade 抽象化は本規則の対象外
