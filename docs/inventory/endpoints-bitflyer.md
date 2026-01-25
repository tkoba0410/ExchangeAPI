# Inventory — Bitflyer Endpoints

> 本文書は **一覧（inventory）** である。
> 技術規範・共通方針は **TopSpec（docs/topspec.md）** を正本とする。
> 公式 API 文書を最上位の正本とし、ここでは対応関係と命名規約を管理する。

## Normative Scope

本書は、EndpointId および対応する事実（Method, Path, Scope 等）を列挙する **Normative Inventory** である。

以下は本書の対象外とする。

* 通称、代表名、便宜的名称
* ナビゲーション目的の分類語
* EndpointId の命名規範や導出・派生規則そのもの
* 実装上の補助的な識別子

これらは、本書の内容から直接または間接に導出されてはならない。

※ Get / Send / Cancel 等の接頭辞は、EndpointId の構文要素であり、
  便宜的名称・通称・代表名には該当しない。

本文書は、TopSpec に基づき決定された EndpointId の一覧を記録するものであり、
命名規範や派生規則そのものを定義するものではない。

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

| Scope | Category | Method | Path | EndpointId | PresentIn | Note |
| ----- | -------- | ------ | ---- | ---------- | ---------- | ---- |

* **Scope**: public / private
* **Category**: MarketData / Trading / Account / History / Other
* **Method**: HTTP method（GET/POST/...）
* **Path**: API path（公式表記）
* **EndpointId**: 本リポジトリでの識別子
* **PresentIn**: 当該 endpoint が存在する層（Wire / Raw / Normalized / Contracts）。空集合は `None`。
* **Note**: 任意（公式との差異・注意点など）

Note 欄には、以下のような **事実関係（状況）** を記載してよい。

* 重複候補（duplicate candidate）
* 旧版・非推奨の可能性（obsolete candidate）
* 非機能の可能性（non-functional candidate）
* version 並立の事実

Note 欄には、採用可否・実装判断・設計判断を記載してはならない。

---

## Public

| Scope  | Category   | Method | Path                     | EndpointId           | PresentIn | Note |
| ------ | ---------- | ------ | ------------------------ | -------------------- | ---------- | ---- |
| public | MarketData | GET    | /v1/getmarkets           | GetMarkets           | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/markets              | Markets              | None                  | duplicate candidate |
| public | MarketData | GET    | /v1/getboard             | GetBoard             | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/board                | Board                | None                  | duplicate candidate |
| public | MarketData | GET    | /v1/getticker            | GetTicker            | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/ticker               | Ticker               | None                  | duplicate candidate |
| public | MarketData | GET    | /v1/getexecutions        | GetExecutionsPublic  | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/executions           | Executions           | None                  | duplicate candidate |
| public | MarketData | GET    | /v1/getboardstate        | GetBoardState        | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/gethealth            | GetHealth            | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/getfundingrate       | GetFundingRate       | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/getcorporateleverage | GetCorporateLeverage | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /v1/getchats             | GetChats             | Wire, Raw, Normalized |      |

---

## Private

| Scope   | Category | Method | Path                         | EndpointId            | PresentIn | Note |
| ------- | -------- | ------ | ---------------------------- | --------------------- | ---------- | ---- |
| private | Other    | GET    | /v1/me/getpermissions        | GetPermissions        | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getbalance            | GetBalance            | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getcollateral         | GetCollateral         | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getcollateralaccounts | GetCollateralAccounts | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getaddresses          | GetAddresses          | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getcoinins            | GetCoinIns            | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getcoinouts           | GetCoinOuts           | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getbankaccounts       | GetBankAccounts       | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getdeposits           | GetDeposits           | Wire, Raw, Normalized |      |
| private | Account  | POST   | /v1/me/withdraw              | Withdraw              | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/me/getwithdrawals        | GetWithdrawals        | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/me/sendchildorder        | SendChildOrder        | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/me/sendparentorder       | SendParentOrder       | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/me/cancelchildorder      | CancelChildOrder      | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/me/cancelparentorder     | CancelParentOrder     | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/me/cancelallchildorders  | CancelAllChildOrders  | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/me/getchildorders        | GetChildOrders        | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/me/getparentorders       | GetParentOrders       | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/me/getparentorder        | GetParentOrder        | Wire, Raw, Normalized |      |
| private | History  | GET    | /v1/me/getexecutions         | GetExecutionsPrivate  | Wire, Raw, Normalized |      |
| private | History  | GET    | /v1/me/getbalancehistory     | GetBalanceHistory     | Wire, Raw, Normalized |      |
| private | Other    | GET    | /v1/me/getpositions          | GetPositions          | Wire, Raw, Normalized |      |
| private | History  | GET    | /v1/me/getcollateralhistory  | GetCollateralHistory  | Wire, Raw, Normalized |      |
| private | Other    | GET    | /v1/me/gettradingcommission  | GetTradingCommission  | Wire, Raw, Normalized |      |

---

## Aliases（任意）

本 inventory の `EndpointId` 列に alias を記載してはならない。
alias を記録する場合は、本セクションに `EndpointId` との対応として記載する。

| EndpointId | Alias | Notes |
|---|---|---|

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
