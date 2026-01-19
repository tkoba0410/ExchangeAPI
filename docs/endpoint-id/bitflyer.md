# EndpointId 命名規則: Bitflyer

## 位置づけ

本書は、Bitflyer における **EndpointId（Constants）の命名規則**および
**Endpoint 一覧（Public / Private × 種類別）**を定める。

* 正本: `ExchangeApi.Exchanges.Bitflyer.Wire.Constants.BitflyerEndpoints`
* EndpointId は **公式 API endpoint を一意に識別する識別子**である
* API 名・メソッド名は EndpointId から共通派生規則で生成される

---

## 命名規則（Bitflyer）

Bitflyer では、公式 API パス自体に `get*` / `send*` / `cancel*` といった語が含まれており、
かつ同一 path を GET/POST で共有する形が本一覧では観測されないため、
**EndpointId に HTTP Method を含めない**。

```
<PascalCase(PathWithoutVersionAndScope)>
```

### 規則

1. HTTP Method を EndpointId に含めない
2. `/v1` および `/v1/me` は EndpointId に含めない
3. パス残部を英単語境界で分割し PascalCase とする（単語境界は細かく）
4. EndpointId は Bitflyer 内で一意でなければならない

---

## Public API

### Market

| Method | Path              | EndpointId    |
| ------ | ----------------- | ------------- |
| GET    | /v1/ticker        | Ticker        |
| GET    | /v1/getticker     | GetTicker     |
| GET    | /v1/board         | Board         |
| GET    | /v1/getboard      | GetBoard      |
| GET    | /v1/executions    | Executions    |
| GET    | /v1/getexecutions | GetExecutions |
| GET    | /v1/markets       | Markets       |
| GET    | /v1/getmarkets    | GetMarkets    |
| GET    | /v1/gethealth     | GetHealth     |
| GET    | /v1/getboardstate | GetBoardState |
| GET    | /v1/getchats      | GetChats      |

---

## Private API

### Account

| Method | Path                         | EndpointId            |
| ------ | ---------------------------- | --------------------- |
| GET    | /v1/me/getbalance            | GetBalance            |
| GET    | /v1/me/getbalancehistory     | GetBalanceHistory     |
| GET    | /v1/me/getcollateral         | GetCollateral         |
| GET    | /v1/me/getcollateralaccounts | GetCollateralAccounts |
| GET    | /v1/me/getcollateralhistory  | GetCollateralHistory  |
| GET    | /v1/me/getpermissions        | GetPermissions        |
| GET    | /v1/me/gettradingcommission  | GetTradingCommission  |
| GET    | /v1/me/getpositions          | GetPositions          |

---

### Order / Trading

| Method | Path                        | EndpointId           |
| ------ | --------------------------- | -------------------- |
| GET    | /v1/me/getchildorders       | GetChildOrders       |
| GET    | /v1/me/getparentorder       | GetParentOrder       |
| GET    | /v1/me/getparentorders      | GetParentOrders      |
| GET    | /v1/me/getexecutions        | GetExecutions        |
| POST   | /v1/me/sendchildorder       | SendChildOrder       |
| POST   | /v1/me/sendparentorder      | SendParentOrder      |
| POST   | /v1/me/cancelchildorder     | CancelChildOrder     |
| POST   | /v1/me/cancelparentorder    | CancelParentOrder    |
| POST   | /v1/me/cancelallchildorders | CancelAllChildOrders |

---

## 注記

* 本一覧は現時点でプロジェクトが対象としている Bitflyer endpoint を列挙している
* HTTP Method は公式仕様および Wire 実装を正とする
* EndpointId からの派生規則は `endpoint-id/common.md` を参照
* Bitflyer は EndpointId に Method を含めない（取引所固有ルール）
