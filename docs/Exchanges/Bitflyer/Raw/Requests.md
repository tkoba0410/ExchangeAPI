# bitFlyer Raw API — Requests（最小版）

本ドキュメントは、bitFlyer Raw API で使用する **Request DTO（Body / Query 集約）** を、
「DTO → 用途」の観点で逆引きできるようにまとめた **索引（カタログ）**です。

- Body を伴う API は **必ず Request DTO** を使用します
- Query は原則引数で受けますが、以下の条件を満たす場合は **Request DTO に集約**します
  - **Query が 3 個以上**
  - **Query のカテゴリが 2 種以上**（Filter / Range / Paging / Sort など）

> 命名規則は `doc/Exchanges/Raw/Naming.md` を参照。

---

## Request DTO 一覧（索引）

| DTO | 種別 | 対応 Endpoint | 対応 Raw メソッド（候補） | 主なプロパティ（概要） |
|---|---|---|---|---|
| GetExecutionsRequest | Query | GET /v1/getexecutions（/v1/executions）<br>GET /v1/me/getexecutions | GetExecutionsAsync(request)<br>GetAccountExecutionsAsync(request) | ProductCode（必須）<br>Count / Before / After |
| GetOrdersRequest | Query | GET /v1/me/getchildorders | GetOrdersAsync(request) | ProductCode（必須）<br>ChildOrderState / ChildOrderAcceptanceId<br>Count / Before / After |
| CreateChildOrderRequest | Body | POST /v1/me/sendchildorder | CreateChildOrderAsync(request) | ProductCode / Side / Size<br>ChildOrderType<br>Price? / MinuteToExpire? / TimeInForce? |
| CancelChildOrderRequest | Body | POST /v1/me/cancelchildorder | CancelChildOrderAsync(request) | ProductCode<br>ChildOrderId? / ChildOrderAcceptanceId? |
| CancelAllOrdersRequest | Body | POST /v1/me/cancelallchildorders | CancelOrdersAsync(request) | ProductCode（必須） |
| CreateParentOrderRequest | Body | POST /v1/me/sendparentorder | CreateParentOrderAsync(request) | ProductCode / OrderMethod<br>Parameters[]<br>MinuteToExpire? / TimeInForce? |
| CancelParentOrderRequest | Body | POST /v1/me/cancelparentorder | CancelParentOrderAsync(request) | ProductCode<br>ParentOrderId? / ParentOrderAcceptanceId? |
| CreateWithdrawalRequest | Body | POST /v1/me/withdraw | CreateWithdrawalAsync(request) | CurrencyCode / Amount<br>BankAccountId? / AddressId? |

---

## 運用ルール

- 本ファイルは **DTO の索引**であり、詳細仕様は DTO の型定義および公式 API を参照します
- 新規 DTO を追加したら、必ずこの表に 1 行追加します
- `Dictionary<string, string>`（escape hatch）を追加した場合も、対応する行にメモを残します

---

> Requests は “DTO の入口”。ApiMap は “Endpoint の入口”。

