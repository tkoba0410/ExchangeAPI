# EndpointId 命名規則: Bittrade

## 位置づけ

本書は、Bittrade における **EndpointId（Constants）の命名規則**および
**Endpoint 一覧（Public / Private × 種類別）**を定める。

* 正本: `ExchangeApi.Exchanges.Bittrade.Wire.Constants.BittradeEndpoints`
* EndpointId は **公式 API endpoint を一意に識別する識別子**である
* API 名・メソッド名は EndpointId から共通派生規則で生成される

---

## 命名規則（Bittrade）

Bittrade では、同一 path に対して GET / POST が存在するため、
**EndpointId に HTTP Method を含める**。

```
<Method><PascalCase(PathWithoutVersion)>
```

### 規則

1. HTTP Method を必ず先頭に含める（Get / Post）
2. `/v1` は EndpointId に含めない
3. パス残部を英単語境界で分割し PascalCase とする
4. EndpointId は Bittrade 内で一意でなければならない

---

## Public API

### Market

| Method | Path                 | EndpointId            |
| ------ | -------------------- | --------------------- |
| GET    | market/detail/merged | GetMarketDetailMerged |
| GET    | market/depth         | GetMarketDepth        |
| GET    | market/trade         | GetMarketTrade        |
| GET    | market/history/kline | GetMarketHistoryKline |
| GET    | market/tickers       | GetMarketTickers      |
| GET    | market/history/trade | GetMarketHistoryTrade |

---

### Common

| Method | Path                    | EndpointId            |
| ------ | ----------------------- | --------------------- |
| GET    | v1/common/timestamp     | GetCommonTimestamp    |
| GET    | v1/common/symbols       | GetCommonSymbols      |
| GET    | v1/common/currencys     | GetCommonCurrencies   |
| GET    | v1/retail/maintain/time | GetRetailMaintainTime |

---

## Private API

### Account

| Method | Path                | EndpointId         |
| ------ | ------------------- | ------------------ |
| GET    | v1/account/accounts | GetAccountAccounts |

---

### Order

| Method | Path                                  | EndpointId                           |
| ------ | ------------------------------------- | ------------------------------------ |
| GET    | v1/order/openOrders                   | GetOrderOpenOrders                   |
| GET    | v1/order/orders                       | GetOrderOrders                       |
| GET    | v1/order/matchresults                 | GetOrderMatchResults                 |
| POST   | v1/order/orders/place                 | PostOrderOrdersPlace                 |
| POST   | v1/order/orders/batchcancel           | PostOrderOrdersBatchCancel           |
| POST   | v1/order/orders/batchCancelOpenOrders | PostOrderOrdersBatchCancelOpenOrders |

---

### Retail

| Method | Path                  | EndpointId           |
| ------ | --------------------- | -------------------- |
| GET    | v1/retail/order/list  | GetRetailOrderList   |
| POST   | v1/retail/order/place | PostRetailOrderPlace |

---

### Finance (Deposit / Withdraw)

| Method | Path                      | EndpointId              |
| ------ | ------------------------- | ----------------------- |
| GET    | v1/query/deposit-withdraw | GetQueryDepositWithdraw |
| POST   | v1/dw/withdraw/api/create | PostDwWithdrawApiCreate |
| POST   | v1/dw/withdraw-virtual    | PostDwWithdrawVirtual   |

---

## 注記

* 本一覧は Wire.Constants に存在する endpoint のみを列挙している
* HTTP Method は公式仕様および Wire 実装を正とする
* EndpointId からの派生規則は `endpoint-id/common.md` を参照
