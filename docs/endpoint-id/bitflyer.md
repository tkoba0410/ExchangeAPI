# endpoints-bitflyer.md（inventory）

> 本ファイルは、bitFlyer Lightning における endpoint の **列挙の正本**である。
> 公式 API の詳細仕様（request / response / error 等）は、公式 API ドキュメントを参照する。

---

## 1. 対象取引所

* Exchange: `bitFlyer`（Lightning API）

---

## 2. 取引所ルール（EndpointId 組み立て規則）

本節は、common（EndpointId 規範）で定義された要素（Path / Method / Scope）と形式（PascalCase / スラッシュ排除）を前提に、
bitFlyer の EndpointId を **機械的に再構成可能**とするための規則を定義する。

### 2.1 入力（共通要素）

* Path: 公式 API ドキュメントに記載された path（例：`/v1/me/getbalance`）
* Method: HTTP Method（GET / POST / DELETE / etc）
* Scope: Public / Private

### 2.2 正規化（共通形式）

* EndpointId は **PascalCase** とする
* EndpointId には **`/` を含めない**

### 2.3 EndpointIdの生成（bitFlyer 規則）

本節は inventory 生成時の手順を示す。
bitFlyer の EndpointId は、Path から **機械的に**次の手順で導出する。

1. Path 先頭の version セグメント（`/v1/` または `/v1/me/`）を除去する
2. 残りを `/` で分割し、空要素を除外する
3. 各セグメントを **common で定義された一般単語境界** に基づいて分割する
4. 分割された各単語を PascalCase 化し、連結する
5. 上記規則に従っても EndpointId が重複する場合は、**重複する EndpointId の末尾に Scope（`Public` / `Private`）を付与して解決する**

補足：

* bitFlyer では、原則として **HTTP Method を EndpointId に含めない**

---

## 3. 並び順ルール

endpoint 一覧の並び順は、HTTP Public API の公式ドキュメントにおいて
**`GET /v1/getmarkets` を起点とした記載順**とする。

Public API を先に列挙し、その後に Private API を、
それぞれ公式 API ドキュメントの出現順に列挙する。

---

## 4. endpoint 一覧

| EndpointId            | Method | Path                           | Scope   |
| --------------------- | ------ | ------------------------------ | ------- |
| GetMarkets            | GET    | `/v1/getmarkets`               | Public  |
| Markets               | GET    | `/v1/markets`                  | Public  |
| GetBoard              | GET    | `/v1/getboard`                 | Public  |
| Board                 | GET    | `/v1/board`                    | Public  |
| GetTicker             | GET    | `/v1/getticker`                | Public  |
| Ticker                | GET    | `/v1/ticker`                   | Public  |
| GetExecutionsPublic   | GET    | `/v1/getexecutions`            | Public  |
| Executions            | GET    | `/v1/executions`               | Public  |
| GetBoardState         | GET    | `/v1/getboardstate`            | Public  |
| GetHealth             | GET    | `/v1/gethealth`                | Public  |
| GetFundingRate        | GET    | `/v1/getfundingrate`           | Public  |
| GetCorporateLeverage  | GET    | `/v1/getcorporateleverage`     | Public  |
| GetChats              | GET    | `/v1/getchats`                 | Public  |
| GetPermissions        | GET    | `/v1/me/getpermissions`        | Private |
| GetBalance            | GET    | `/v1/me/getbalance`            | Private |
| GetCollateral         | GET    | `/v1/me/getcollateral`         | Private |
| GetCollateralAccounts | GET    | `/v1/me/getcollateralaccounts` | Private |
| GetAddresses          | GET    | `/v1/me/getaddresses`          | Private |
| GetCoinIns            | GET    | `/v1/me/getcoinins`            | Private |
| GetCoinOuts           | GET    | `/v1/me/getcoinouts`           | Private |
| GetBankAccounts       | GET    | `/v1/me/getbankaccounts`       | Private |
| GetDeposits           | GET    | `/v1/me/getdeposits`           | Private |
| Withdraw              | POST   | `/v1/me/withdraw`              | Private |
| GetWithdrawals        | GET    | `/v1/me/getwithdrawals`        | Private |
| SendChildOrder        | POST   | `/v1/me/sendchildorder`        | Private |
| SendParentOrder       | POST   | `/v1/me/sendparentorder`       | Private |
| CancelChildOrder      | POST   | `/v1/me/cancelchildorder`      | Private |
| CancelParentOrder     | POST   | `/v1/me/cancelparentorder`     | Private |
| CancelAllChildOrders  | POST   | `/v1/me/cancelallchildorders`  | Private |
| GetChildOrders        | GET    | `/v1/me/getchildorders`        | Private |
| GetParentOrders       | GET    | `/v1/me/getparentorders`       | Private |
| GetParentOrder        | GET    | `/v1/me/getparentorder`        | Private |
| GetExecutionsPrivate  | GET    | `/v1/me/getexecutions`         | Private |
| GetBalanceHistory     | GET    | `/v1/me/getbalancehistory`     | Private |
| GetPositions          | GET    | `/v1/me/getpositions`          | Private |
| GetCollateralHistory  | GET    | `/v1/me/getcollateralhistory`  | Private |
| GetTradingCommission  | GET    | `/v1/me/gettradingcommission`  | Private |
