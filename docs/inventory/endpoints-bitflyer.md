# Inventory — Bitflyer Endpoints

> 本文書は **一覧（inventory）** である。
> 技術規範・共通方針は **TopSpec（docs/topspec.md）** を正本とする。
> 公式 API 文書を最上位の正本とし、ここでは対応関係と命名規約を管理する。

## EndpointId ルール（Bitflyer）

- EndpointId は **取引所スコープ**の識別子とする。
- HTTP Method（GET / POST 等）をそのまま表す語は原則として含めない。
- ただし、API 操作の意味を表す **慣用的な接頭辞**（Get / Send / Cancel 等）については、
  既存 EndpointId の識別上必要な範囲で採用を許容する。
- 単語境界は比較的細かく区切り、可読性を優先する。
- Public / Private の差分は、必要に応じて suffix 等で表現する。
- 本文書に記載された EndpointId が、
  Bitflyer における正本である。

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

| Scope  | Category   | Method | Path                     | EndpointId           | Note |
| ------ | ---------- | ------ | ------------------------ | -------------------- | ---- |
| public | MarketData | GET    | /v1/getmarkets           | GetMarkets           |      |
| public | MarketData | GET    | /v1/markets              | Markets              |      |
| public | MarketData | GET    | /v1/getboard             | GetBoard             |      |
| public | MarketData | GET    | /v1/board                | Board                |      |
| public | MarketData | GET    | /v1/getticker            | GetTicker            |      |
| public | MarketData | GET    | /v1/ticker               | Ticker               |      |
| public | MarketData | GET    | /v1/getexecutions        | GetExecutionsPublic  |      |
| public | MarketData | GET    | /v1/executions           | Executions           |      |
| public | MarketData | GET    | /v1/getboardstate        | GetBoardState        |      |
| public | MarketData | GET    | /v1/gethealth            | GetHealth            |      |
| public | MarketData | GET    | /v1/getfundingrate       | GetFundingRate       |      |
| public | MarketData | GET    | /v1/getcorporateleverage | GetCorporateLeverage |      |
| public | MarketData | GET    | /v1/getchats             | GetChats             |      |

---

## Private

| Scope   | Category | Method | Path                         | EndpointId            | Note |
| ------- | -------- | ------ | ---------------------------- | --------------------- | ---- |
| private | Other    | GET    | /v1/me/getpermissions        | GetPermissions        |      |
| private | Account  | GET    | /v1/me/getbalance            | GetBalance            |      |
| private | Account  | GET    | /v1/me/getcollateral         | GetCollateral         |      |
| private | Account  | GET    | /v1/me/getcollateralaccounts | GetCollateralAccounts |      |
| private | Account  | GET    | /v1/me/getaddresses          | GetAddresses          |      |
| private | Account  | GET    | /v1/me/getcoinins            | GetCoinIns            |      |
| private | Account  | GET    | /v1/me/getcoinouts           | GetCoinOuts           |      |
| private | Account  | GET    | /v1/me/getbankaccounts       | GetBankAccounts       |      |
| private | Account  | GET    | /v1/me/getdeposits           | GetDeposits           |      |
| private | Account  | POST   | /v1/me/withdraw              | Withdraw              |      |
| private | Account  | GET    | /v1/me/getwithdrawals        | GetWithdrawals        |      |
| private | Trading  | POST   | /v1/me/sendchildorder        | SendChildOrder        |      |
| private | Trading  | POST   | /v1/me/sendparentorder       | SendParentOrder       |      |
| private | Trading  | POST   | /v1/me/cancelchildorder      | CancelChildOrder      |      |
| private | Trading  | POST   | /v1/me/cancelparentorder     | CancelParentOrder     |      |
| private | Trading  | POST   | /v1/me/cancelallchildorders  | CancelAllChildOrders  |      |
| private | Trading  | GET    | /v1/me/getchildorders        | GetChildOrders        |      |
| private | Trading  | GET    | /v1/me/getparentorders       | GetParentOrders       |      |
| private | Trading  | GET    | /v1/me/getparentorder        | GetParentOrder        |      |
| private | History  | GET    | /v1/me/getexecutions         | GetExecutionsPrivate  |      |
| private | History  | GET    | /v1/me/getbalancehistory     | GetBalanceHistory     |      |
| private | Other    | GET    | /v1/me/getpositions          | GetPositions          |      |
| private | History  | GET    | /v1/me/getcollateralhistory  | GetCollateralHistory  |      |
| private | Other    | GET    | /v1/me/gettradingcommission  | GetTradingCommission  |      |

---

## 並び順について

本 inventory の endpoint 一覧は、**公式 API 文書における記載順**を正とする。
可読性や実装都合を理由とした並び替えは行わない。

## 補足

* 本 inventory は **一覧のみ** を目的とする。
* EndpointId の意味・命名・層対応は TopSpec を参照する。

### EndpointId 導出手順（参考）

以下は、本 inventory に記載された EndpointId を導出する際に用いた手順の一例である。
この手順自体は **設計規範ではない**（衝突時は TopSpec を優先する）。

1. Path 先頭の version セグメント（`/v1/` または `/v1/me/`）を除去する
2. 残りを `/` で分割し、空要素を除外する
3. 各セグメントを TopSpec が定める一般単語境界に基づいて分割する
4. 分割された各単語を PascalCase 化し、連結する
5. 上記規則で EndpointId が重複する場合は、末尾に Scope（`Public` / `Private`）を付与して解決する

- Bitflyer では、HTTP Method（GET / POST 等）を直接反映した
  prefix（Get / Post など）を付与する方式は採用していない。
- 一方で、API 操作の意味を表す慣用的な接頭辞としての
  Get / Send / Cancel 等は EndpointId に含まれている。
