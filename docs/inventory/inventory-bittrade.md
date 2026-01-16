# 注意

本 inventory 文書は **仕様書ではない**。

これは実装および公式 API 文書の観測結果を整理したものであり、
Contracts や TopSpec における必須要件を定義するものではない。

仕様としての正本は各取引所の公式 API 文書のみである。

# Inventory: Bittrade

本書は、Bittrade に関して **本リポジトリが扱っている API の実装有無一覧**を示す。
本書は **Normative ではない**。

判断・理由・計画・注意書きは含めない。

---

## Public APIs

| Endpoint | Implemented | Notes |
|----------|-------------|-------|
| GET /v1/common/symbols | Yes |  |
| GET /market/depth | Yes |  |

---

## Private APIs

| Endpoint | Implemented | Notes |
|----------|-------------|-------|
| GET /v1/account/accounts/{account-id}/balance | Yes |  |

---

※ Notes 欄は **空欄のまま**使用する（説明を書かない）
