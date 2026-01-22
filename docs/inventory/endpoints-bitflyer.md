# Inventory — Bitflyer Endpoints

> 本文書は **一覧（inventory）** です。
> 仕様判断・設計規範は **TopSpec（docs/topspec.md）** を正本とします。
> 公式 API 文書を正本とし、ここでは対応関係のみを管理します。

---

## Columns

| Scope | Category | Method | Path | EndpointId | Note |
| ----- | -------- | ------ | ---- | ---------- | ---- |

* **Scope**: public / private
* **Category**: MarketData / Trading / Account / History / Other
* **Method**: HTTP method（GET/POST/...）
* **Path**: API path（公式表記）
* **EndpointId**: 本リポジトリでの識別子
* **Note**: 任意（公式との差異・注意点など）

---

## Public — MarketData

| Scope  | Category   | Method | Path           | EndpointId    | Note                                  |
| ------ | ---------- | ------ | -------------- | ------------- | ------------------------------------- |
| public | MarketData | GET    | /v1/ticker     | GetTicker     | product_code 指定                       |
| public | MarketData | GET    | /v1/board      | GetOrderBook  | product_code 指定                       |
| public | MarketData | GET    | /v1/executions | GetExecutions | product_code / count / before / after |
| public | MarketData | GET    | /v1/markets    | GetMarkets    | 取扱市場一覧                                |
| public | MarketData | GET    | /v1/health     | GetHealth     | 稼働状態                                  |

---

## Public — Other

| Scope  | Category | Method | Path         | EndpointId | Note   |
| ------ | -------- | ------ | ------------ | ---------- | ------ |
| public | Other    | GET    | /v1/getchats | GetChats   | チャットログ |

---

## Private — Trading

| Scope   | Category | Method | Path                        | EndpointId           | Note |
| ------- | -------- | ------ | --------------------------- | -------------------- | ---- |
| private | Trading  | POST   | /v1/me/sendchildorder       | SendChildOrder       | 新規注文 |
| private | Trading  | POST   | /v1/me/cancelchildorder     | CancelChildOrder     | 注文取消 |
| private | Trading  | POST   | /v1/me/cancelallchildorders | CancelAllChildOrders | 一括取消 |
| private | Trading  | GET    | /v1/me/getchildorders       | GetChildOrders       | 注文一覧 |

---

## Private — Account

| Scope   | Category | Method | Path                         | EndpointId            | Note  |
| ------- | -------- | ------ | ---------------------------- | --------------------- | ----- |
| private | Account  | GET    | /v1/me/getbalance            | GetBalance            | 資産残高  |
| private | Account  | GET    | /v1/me/getcollateral         | GetCollateral         | 証拠金情報 |
| private | Account  | GET    | /v1/me/getcollateralaccounts | GetCollateralAccounts | 証拠金口座 |

---

## Private — History

| Scope   | Category | Method | Path                        | EndpointId           | Note  |
| ------- | -------- | ------ | --------------------------- | -------------------- | ----- |
| private | History  | GET    | /v1/me/getexecutions        | GetMyExecutions      | 約定履歴  |
| private | History  | GET    | /v1/me/gettradingcommission | GetTradingCommission | 取引手数料 |

---

## Notes

* 本 inventory は **一覧のみ** を目的とし、層構造・責務・公開範囲の規範は記載しません。
* EndpointId の意味・命名・層対応は TopSpec を参照してください。
