# Inventory — Bitflyer Endpoints

> 本文書は **inventory（事実一覧）** です。
> 公式 API 文書を参照し、Method / Path / CanonicalSourceUrl / EndpointId / PresentIn の対応関係のみを記録します。
> 本文書は判断規則を定義しません。


## Canonical Source（Entrypoint）

- https://lightning.bitflyer.com/docs

---

## Columns

| Scope | Method | Path | CanonicalSourceUrl | EndpointId | PresentIn | Note | RequestType | ResponseType |
| ----- | ------ | ---- | ------------------ | ---------- | --------- | ---- | ----------- | ------------ |

* **Scope**: public / private
* **Method**: HTTP method（GET/POST/...）
* **Path**: API path（公式表記）
* **CanonicalSourceUrl**: 公式 API 文書における当該 endpoint の正本 URL（アンカー `#...` を含める）
* **EndpointId**: 本リポジトリでの識別子
* **PresentIn**: 当該 endpoint が存在する層（Wire / Raw / Normalized / Contracts）。空集合は `None`。
* **Note**: 任意（公式との差異・注意点など）
* **RequestType**: 取引所側の正準 Request 型名（存在しない場合は `None`）
* **ResponseType**: 取引所側の正準 Response 型名（存在しない場合は `None`）

Note 欄には、以下のような **事実関係（状況）** を記載してよい。

* 重複候補（duplicate candidate）
* 旧版・非推奨の可能性（obsolete candidate）
* 非機能の可能性（non-functional candidate）
* version 並立の事実

Note 欄には、採用可否・実装判断・設計判断を記載してはならない。

---

## Public

| Scope | Method | Path | CanonicalSourceUrl | EndpointId | PresentIn | Note | RequestType | ResponseType |
| ------ | ------ | ------------------------ | ------------------------------------------------------------------- | -------------------- | --------------------- | ---- | ----------- | ------------ |
| public | GET | /v1/getmarkets | https://lightning.bitflyer.com/docs#マーケットの一覧 | GetMarkets | Wire, Raw, Normalized |  | GetMarketsRequest | GetMarketsResponse |
| public | GET | /v1/getboard | https://lightning.bitflyer.com/docs#板情報 | GetBoard | Wire, Raw, Normalized |  | GetBoardRequest | GetBoardResponse |
| public | GET | /v1/getticker | https://lightning.bitflyer.com/docs#ticker | GetTicker | Wire, Raw, Normalized |  | GetTickerRequest | GetTickerResponse |
| public | GET | /v1/getexecutions | https://lightning.bitflyer.com/docs#約定履歴 | GetExecutionsPublic | Wire, Raw, Normalized |  | GetExecutionsPublicRequest | GetExecutionsPublicResponse |
| public | GET | /v1/getboardstate | https://lightning.bitflyer.com/docs#板情報state | GetBoardState | Wire, Raw, Normalized |  | GetBoardStateRequest | GetBoardStateResponse |
| public | GET | /v1/gethealth | https://lightning.bitflyer.com/docs#取引所の状態 | GetHealth | Wire, Raw, Normalized |  | GetHealthRequest | GetHealthResponse |
| public | GET | /v1/getfundingrate | https://lightning.bitflyer.com/docs#ファンディングレート | GetFundingRate | Wire, Raw, Normalized |  | GetFundingRateRequest | GetFundingRateResponse |
| public | GET | /v1/getcorporateleverage | https://lightning.bitflyer.com/docs#法人アカウント最大レバレッジ | GetCorporateLeverage | Wire, Raw, Normalized |  | GetCorporateLeverageRequest | GetCorporateLeverageResponse |
| public | GET | /v1/getchats | https://lightning.bitflyer.com/docs#チャット | GetChats | Wire, Raw, Normalized |  | GetChatsRequest | GetChatsResponse |

---

## Private

| Scope | Method | Path | CanonicalSourceUrl | EndpointId | PresentIn | Note | RequestType | ResponseType |
| ------- | ------ | ---------------------------- | --------------------------------------------------------------------- | --------------------- | --------------------- | ---- | ----------- | ------------ |
| private | GET | /v1/me/getpermissions | https://lightning.bitflyer.com/docs#api-キーの権限を取得 | GetPermissions | Wire, Raw, Normalized |  | GetPermissionsRequest | GetPermissionsResponse |
| private | GET | /v1/me/getbalance | https://lightning.bitflyer.com/docs#資産残高を取得 | GetBalance | Wire, Raw, Normalized |  | GetBalanceRequest | GetBalanceResponse |
| private | GET | /v1/me/getcollateral | https://lightning.bitflyer.com/docs#証拠金の状態を取得 | GetCollateral | Wire, Raw, Normalized |  | GetCollateralRequest | GetCollateralResponse |
| private | GET | /v1/me/getcollateralaccounts | https://lightning.bitflyer.com/docs#証拠金の状態を取得accounts | GetCollateralAccounts | Wire, Raw, Normalized |  | GetCollateralAccountsRequest | GetCollateralAccountsResponse |
| private | GET | /v1/me/getaddresses | https://lightning.bitflyer.com/docs#預入用アドレス取得 | GetAddresses | Wire, Raw, Normalized |  | GetAddressesRequest | GetAddressesResponse |
| private | GET | /v1/me/getcoinins | https://lightning.bitflyer.com/docs#仮想通貨預入履歴 | GetCoinIns | Wire, Raw, Normalized |  | GetCoinInsRequest | GetCoinInsResponse |
| private | GET | /v1/me/getcoinouts | https://lightning.bitflyer.com/docs#仮想通貨送付履歴 | GetCoinOuts | Wire, Raw, Normalized |  | GetCoinOutsRequest | GetCoinOutsResponse |
| private | GET | /v1/me/getbankaccounts | https://lightning.bitflyer.com/docs#銀行口座一覧取得 | GetBankAccounts | Wire, Raw, Normalized |  | GetBankAccountsRequest | GetBankAccountsResponse |
| private | GET | /v1/me/getdeposits | https://lightning.bitflyer.com/docs#入金履歴 | GetDeposits | Wire, Raw, Normalized |  | GetDepositsRequest | GetDepositsResponse |
| private | POST | /v1/me/withdraw | https://lightning.bitflyer.com/docs#出金 | Withdraw | Wire, Raw, Normalized |  | WithdrawRequest | WithdrawResponse |
| private | GET | /v1/me/getwithdrawals | https://lightning.bitflyer.com/docs#出金履歴 | GetWithdrawals | Wire, Raw, Normalized |  | GetWithdrawalsRequest | GetWithdrawalsResponse |
| private | POST | /v1/me/sendchildorder | https://lightning.bitflyer.com/docs#新規注文を出す | SendChildOrder | Wire, Raw, Normalized |  | SendChildOrderRequest | SendChildOrderResponse |
| private | POST | /v1/me/sendparentorder | https://lightning.bitflyer.com/docs#新規の親注文を出す特殊注文 | SendParentOrder | Wire, Raw, Normalized |  | SendParentOrderRequest | SendParentOrderResponse |
| private | POST | /v1/me/cancelchildorder | https://lightning.bitflyer.com/docs#注文をキャンセルする | CancelChildOrder | Wire, Raw, Normalized |  | CancelChildOrderRequest | CancelChildOrderResponse |
| private | POST | /v1/me/cancelparentorder | https://lightning.bitflyer.com/docs#親注文をキャンセルする | CancelParentOrder | Wire, Raw, Normalized |  | CancelParentOrderRequest | CancelParentOrderResponse |
| private | POST | /v1/me/cancelallchildorders | https://lightning.bitflyer.com/docs#すべての注文をキャンセルする | CancelAllChildOrders | Wire, Raw, Normalized |  | CancelAllChildOrdersRequest | CancelAllChildOrdersResponse |
| private | GET | /v1/me/getchildorders | https://lightning.bitflyer.com/docs#注文の一覧を取得 | GetChildOrders | Wire, Raw, Normalized |  | GetChildOrdersRequest | GetChildOrdersResponse |
| private | GET | /v1/me/getparentorders | https://lightning.bitflyer.com/docs#親注文の一覧を取得 | GetParentOrders | Wire, Raw, Normalized |  | GetParentOrdersRequest | GetParentOrdersResponse |
| private | GET | /v1/me/getparentorder | https://lightning.bitflyer.com/docs#親注文の詳細を取得 | GetParentOrder | Wire, Raw, Normalized |  | GetParentOrderRequest | GetParentOrderResponse |
| private | GET | /v1/me/getexecutions | https://lightning.bitflyer.com/docs#約定の一覧を取得 | GetExecutionsPrivate | Wire, Raw, Normalized |  | GetExecutionsPrivateRequest | GetExecutionsPrivateResponse |
| private | GET | /v1/me/getbalancehistory | https://lightning.bitflyer.com/docs#資産残高を取得history | GetBalanceHistory | Wire, Raw, Normalized |  | GetBalanceHistoryRequest | GetBalanceHistoryResponse |
| private | GET | /v1/me/getpositions | https://lightning.bitflyer.com/docs#建玉の一覧を取得 | GetPositions | Wire, Raw, Normalized |  | GetPositionsRequest | GetPositionsResponse |
| private | GET | /v1/me/getcollateralhistory | https://lightning.bitflyer.com/docs#証拠金の状態を取得history | GetCollateralHistory | Wire, Raw, Normalized |  | GetCollateralHistoryRequest | GetCollateralHistoryResponse |
| private | GET | /v1/me/gettradingcommission | https://lightning.bitflyer.com/docs#取引手数料を取得 | GetTradingCommission | Wire, Raw, Normalized |  | GetTradingCommissionRequest | GetTradingCommissionResponse |

---

## Aliases（任意）

本 inventory の `EndpointId` 列に alias を記載してはならない。
alias を記録する場合は、本セクションに `EndpointId` との対応として記載する。

| EndpointId | Alias | Notes |
|---|---|---|
| GetMarkets | Markets | duplicate candidate |
| GetBoard | Board | duplicate candidate |
| GetTicker | Ticker | duplicate candidate |
| GetExecutionsPublic | Executions | duplicate candidate |

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

---

## 配置移行（対象部分）

### 目標ツリー（Bitflyer）

```text
src/Exchanges/Bitflyer/
  Wire/{Public,Private,Internal}
  Raw/{Public,Private,Internal}
  Normalized/{Public,Private,Internal}
  Adapter/{Public,Private,Internal}
  Application/
    ExchangeInfo/
  Composition/
```

### ExchangeInfo の移行対応

| 現在パス | 目標パス | Phase |
| --- | --- | --- |
| `src/Exchanges/Common/ExchangeInfo/...` | `src/Exchanges/Common/Application/ExchangeInfo/...` | 1 |
| `src/Exchanges/Bitflyer/ExchangeInfo/...` | `src/Exchanges/Bitflyer/Application/ExchangeInfo/...` | 2 |

### 移行時の検証コマンド

- `dotnet build`
- `dotnet test`
