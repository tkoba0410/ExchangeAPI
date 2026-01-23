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

## Public

| Scope  | Category   | Method | Path                       | EndpointId          | Note |
| ------ | ---------- | ------ | -------------------------- | ------------------- | ---- |
| public | MarketData | GET    | /v1/getmarkets             | GetMarkets          |      |
| public | MarketData | GET    | /v1/markets                | Markets             |      |
| public | MarketData | GET    | /v1/getboard               | GetBoard            |      |
| public | MarketData | GET    | /v1/board                  | Board               |      |
| public | MarketData | GET    | /v1/getticker              | GetTicker           |      |
| public | MarketData | GET    | /v1/ticker                 | Ticker              |      |
| public | MarketData | GET    | /v1/getexecutions          | GetExecutionsPublic |      |
| public | MarketData | GET    | /v1/executions             | Executions          |      |
| public | MarketData | GET    | /v1/getboardstate          | GetBoardState       |      |
| public | MarketData | GET    | /v1/gethealth              | GetHealth           |      |
| public | MarketData | GET    | /v1/getfundingrate         | GetFundingRate      |      |
| public | MarketData | GET    | /v1/getcorporateleverage   | GetCorporateLeverage |      |
| public | MarketData | GET    | /v1/getchats               | GetChats            |      |

---

## Private

| Scope   | Category | Method | Path                           | EndpointId                 | Note |
| ------- | -------- | ------ | ------------------------------ | -------------------------- | ---- |
| private | Other    | GET    | /v1/me/getpermissions          | GetPermissions             |      |
| private | Account  | GET    | /v1/me/getbalance              | GetBalance                 |      |
| private | Account  | GET    | /v1/me/getcollateral           | GetCollateral              |      |
| private | Account  | GET    | /v1/me/getcollateralaccounts   | GetCollateralAccounts      |      |
| private | Account  | GET    | /v1/me/getaddresses            | GetAddresses               |      |
| private | Account  | GET    | /v1/me/getcoinins              | GetCoinIns                 |      |
| private | Account  | GET    | /v1/me/getcoinouts             | GetCoinOuts                |      |
| private | Account  | GET    | /v1/me/getbankaccounts         | GetBankAccounts            |      |
| private | Account  | GET    | /v1/me/getdeposits             | GetDeposits                |      |
| private | Account  | POST   | /v1/me/withdraw                | Withdraw              |      |
| private | Account  | GET    | /v1/me/getwithdrawals          | GetWithdrawals             |      |
| private | Trading  | POST   | /v1/me/sendchildorder          | SendChildOrder             |      |
| private | Trading  | POST   | /v1/me/sendparentorder         | SendParentOrder            |      |
| private | Trading  | POST   | /v1/me/cancelchildorder        | CancelChildOrder           |      |
| private | Trading  | POST   | /v1/me/cancelparentorder       | CancelParentOrder          |      |
| private | Trading  | POST   | /v1/me/cancelallchildorders    | CancelAllChildOrders       |      |
| private | Trading  | GET    | /v1/me/getchildorders          | GetChildOrders             |      |
| private | Trading  | GET    | /v1/me/getparentorders         | GetParentOrders            |      |
| private | Trading  | GET    | /v1/me/getparentorder          | GetParentOrder             |      |
| private | History  | GET    | /v1/me/getexecutions           | GetExecutionsPrivate       |      |
| private | History  | GET    | /v1/me/getbalancehistory       | GetBalanceHistory          |      |
| private | Other    | GET    | /v1/me/getpositions            | GetPositions               |      |
| private | History  | GET    | /v1/me/getcollateralhistory    | GetCollateralHistory       |      |
| private | Other    | GET    | /v1/me/gettradingcommission    | GetTradingCommission       |      |

---

## Notes

* 本 inventory は **一覧のみ** を目的とし、層構造・責務・公開範囲の規範は記載しません。
* EndpointId の意味・命名・層対応は TopSpec を参照してください。
