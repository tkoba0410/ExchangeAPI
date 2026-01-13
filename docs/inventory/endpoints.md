# API Inventory

## 1. Purpose

本ドキュメントは、ExchangeAPI プロジェクトにおいて **使用している外部 API エンドポイントの一覧（インベントリ）** を管理する。

ここで扱うのは、

* どの API を使っているか
* どの層で対応しているか
* 正本となる公式ドキュメントはどれか

という **棚卸し情報のみ**であり、API仕様そのものを記述するものではない。

---

## 2. Scope

本ドキュメントの対象は以下に限定する。

* 外部取引所 API の endpoint 一覧
* HTTP method / path
* 認証要否（大分類）
* 内部実装上の対応点（Raw / Normalized）
* 公式ドキュメントへの参照

以下は **意図的に記載しない**。

* リクエストパラメータの詳細
* レスポンスフィールドの定義
* エラーコードや例外仕様
* レート制限の数値

API仕様の正本は、常に各取引所の公式ドキュメントとする。

---

## 3. Listing Rules

* 1 endpoint = 1 entry とする
* 並び順は Exchange → Domain → Path の順とする
* 命名・分類は内部都合ではなく、公式APIの構造を優先する

---

## 4. Entry Format

各エンドポイントは、以下の形式で記載する。

```md
### <Exchange> / <Domain> / <Short Description>

- Method: GET | POST | ...
- Path: /v1/...
- Auth: None | API Key | Signed
- Official Reference: <公式ドキュメント名 or URL>
- Internal Mapping:
  - Raw: <Raw API entrypoint>
  - Normalized: <Normalized API entrypoint>
```

---

## 5. Example

### bitFlyer / Market / Ticker

* Method: GET
* Path: /v1/ticker
* Auth: None
* Official Reference: bitFlyer API Docs – Ticker
* Internal Mapping:

  * Raw: BitflyerRawMarketApi.GetTicker
  * Normalized: BitflyerNormalizedMarketDataApi.GetTicker

---

## 6. Domains

以下は、本プロジェクト内で使用する **Domain 分類の一覧**である。

* Market
* Order
* Execution
* Position
* Account
* History

Domain は、意味論的な分類であり、
公式APIのカテゴリと完全一致する必要はないが、
**安易に増やさない**こと。

---

## 7. Update Rules

本ドキュメントは、以下の場合に更新する。

* 新しい API エンドポイントを使用する場合
* 既存 API の使用を停止・置換した場合
* Internal Mapping（Raw / Normalized）が変更された場合

API仕様の変更のみで、
内部での使用内容が変わらない場合は更新しない。

---

## 8. Authority

本ドキュメントは、API の **使用有無・対応範囲** に関する判断において正とする。

API仕様や振る舞いに関する判断は、
必ず公式ドキュメントを参照すること。

本書は API仕様書ではない。
