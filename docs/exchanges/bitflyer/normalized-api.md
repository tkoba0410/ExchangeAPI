# Normalized API 命名規則: Bitflyer（最終確定）

## 目的

Bitflyer の Normalized API は、**公式 API パスをそのまま写像した正本**とする。
意味解釈・共通化・利便性調整は一切行わず、公式 API が持つ癖や冗長性も含めて
**そのまま型に落とす**ことを目的とする。

---

## 基本方針（Bitflyer 固有）

1. **HTTP METHOD は命名に使用しない**
2. **`/v1` および `/v1/me` は命名に使用しない**
3. **残りの公式 API パス文字列のみを使用する**
4. **単語境界は可能な限り細かく分割する**
5. **公式 API endpoint と Normalized API は 1:1 対応**とする

---

## 命名規則

```
NormalizedMethodName = PascalCaseWithWordSplit(残りの公式APIパス) + CallAsync
```

### 変換手順

1. 公式 API パスから `/v1` または `/v1/me` を削除する
2. 残ったパス中の `/` をすべて除去する
3. 英単語境界で可能な限り細かく分割する
4. 各単語を PascalCase で連結する
5. 末尾に `CallAsync` を付与する

---

## 命名例

| HTTP | Path                        | Normalized Method             |
| ---- | --------------------------- | ----------------------------- |
| GET  | /v1/ticker                  | TickerCallAsync               |
| GET  | /v1/getticker               | GetTickerCallAsync            |
| GET  | /v1/executions              | ExecutionsCallAsync           |
| GET  | /v1/getexecutions           | GetExecutionsCallAsync        |
| GET  | /v1/me/getbalance           | GetBalanceCallAsync           |
| GET  | /v1/me/getbalancehistory    | GetBalanceHistoryCallAsync    |
| POST | /v1/me/sendchildorder       | SendChildOrderCallAsync       |
| POST | /v1/me/cancelchildorder     | CancelChildOrderCallAsync     |
| POST | /v1/me/cancelallchildorders | CancelAllChildOrdersCallAsync |

---

## Endpoint 一覧（Bitflyer）

### Public API

| HTTP | Path              | Normalized Method      |
| ---- | ----------------- | ---------------------- |
| GET  | /v1/ticker        | TickerCallAsync        |
| GET  | /v1/getticker     | GetTickerCallAsync     |
| GET  | /v1/board         | BoardCallAsync         |
| GET  | /v1/getboard      | GetBoardCallAsync      |
| GET  | /v1/executions    | ExecutionsCallAsync    |
| GET  | /v1/getexecutions | GetExecutionsCallAsync |
| GET  | /v1/markets       | MarketsCallAsync       |
| GET  | /v1/getmarkets    | GetMarketsCallAsync    |
| GET  | /v1/gethealth     | GetHealthCallAsync     |
| GET  | /v1/getboardstate | GetBoardStateCallAsync |
| GET  | /v1/getchats      | GetChatsCallAsync      |

---

### Private API

| HTTP | Path                         | Normalized Method              |
| ---- | ---------------------------- | ------------------------------ |
| GET  | /v1/me/getbalance            | GetBalanceCallAsync            |
| GET  | /v1/me/getbalancehistory     | GetBalanceHistoryCallAsync     |
| GET  | /v1/me/getpermissions        | GetPermissionsCallAsync        |
| GET  | /v1/me/getcollateral         | GetCollateralCallAsync         |
| GET  | /v1/me/getcollateralaccounts | GetCollateralAccountsCallAsync |
| GET  | /v1/me/getcollateralhistory  | GetCollateralHistoryCallAsync  |
| GET  | /v1/me/gettradingcommission  | GetTradingCommissionCallAsync  |
| GET  | /v1/me/getpositions          | GetPositionsCallAsync          |
| GET  | /v1/me/getparentorders       | GetParentOrdersCallAsync       |
| GET  | /v1/me/getparentorder        | GetParentOrderCallAsync        |
| GET  | /v1/me/getchildorders        | GetChildOrdersCallAsync        |
| GET  | /v1/me/getexecutions         | GetExecutionsCallAsync         |

---

### Order / Trading

| HTTP | Path                        | Normalized Method             |
| ---- | --------------------------- | ----------------------------- |
| POST | /v1/me/sendchildorder       | SendChildOrderCallAsync       |
| POST | /v1/me/sendparentorder      | SendParentOrderCallAsync      |
| POST | /v1/me/cancelchildorder     | CancelChildOrderCallAsync     |
| POST | /v1/me/cancelparentorder    | CancelParentOrderCallAsync    |
| POST | /v1/me/cancelallchildorders | CancelAllChildOrdersCallAsync |

---

## 注意事項

* 本一覧は **公式 API endpoint の写し**であり、利便性のための統合・省略は禁止する。
* 命名は公式パス以外の情報を一切含まない。
* public / private、権限、意味解釈は Normalized の責務外とする。
* 抽象化・横断利用は Facade 層の責務とする。
