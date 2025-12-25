# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# bitFlyer Raw API — Requests（最小版）

本ドキュメントは、bitFlyer Raw API で使用する **Request DTO（Body / Query 集約）** を、
「DTO → 用途」の観点で逆引きできるようにまとめた **索引（カタログ）**です。

- Body を伴う API は **必ず Request DTO** を使用します
- Query は原則引数で受けますが、以下の条件を満たす場合は **Request DTO に集約**します
  - **Query が 3 個以上**
  - **Query のカテゴリが 2 種以上**（Filter / Range / Paging / Sort など）

> 命名規則は `../../Raw/Naming.md` を参照。

---

## Request DTO 一覧（索引）

| DTO | 種別 | 対応 Endpoint | 対応 Raw メソッド | 主なプロパティ（概要） |
|---|---|---|---|---|
| CreateChildOrderRequest | Body | POST /v1/me/sendchildorder | CreateChildOrderAsync(request) | ProductCode / Side / Size<br>ChildOrderType<br>Price? / MinuteToExpire? / TimeInForce? |
| CancelChildOrderRequest | Body | POST /v1/me/cancelchildorder | CancelChildOrderAsync(request) | ProductCode<br>ChildOrderId? / ChildOrderAcceptanceId? |
| CancelAllChildOrdersRequest | Body | POST /v1/me/cancelallchildorders | CancelAllChildOrdersAsync(request) | ProductCode（必須） |
| CreateParentOrderRequest | Body | POST /v1/me/sendparentorder | CreateParentOrderAsync(request) | ProductCode / OrderMethod<br>Parameters[]<br>MinuteToExpire? / TimeInForce? |
| CancelParentOrderRequest | Body | POST /v1/me/cancelparentorder | CancelParentOrderAsync(request) | ProductCode<br>ParentOrderId? / ParentOrderAcceptanceId? |
| CreateWithdrawalRequest | Body | POST /v1/me/withdraw | CreateWithdrawalAsync(request) | CurrencyCode / Amount<br>BankAccountId / Code? |

---

## 運用ルール

- 本ファイルは **DTO の索引**であり、詳細仕様は DTO の型定義および公式 API を参照します
- 新規 DTO を追加したら、必ずこの表に 1 行追加します
- `Dictionary<string, string>`（escape hatch）を追加した場合も、対応する行にメモを残します

---

> Requests は “DTO の入口”。ApiMap は “Endpoint の入口”。
