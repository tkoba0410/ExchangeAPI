# 注意

本 inventory 文書は **仕様書ではない**。

これは実装および公式 API 文書の観測結果を整理したものであり、
Contracts や TopSpec における必須要件を定義するものではない。

仕様としての正本は各取引所の公式 API 文書のみである。

# Inventory: Bitflyer

本書は、bitFlyer に関して **本リポジトリが扱っている API の実装有無一覧**を示す。
本書は **Normative ではない**。

判断・理由・計画は含めない。

---

## Public APIs

| Endpoint | Implemented | Notes |
|----------|-------------|-------|
| GET /v1/markets | Yes |  |
| GET /v1/board | Yes |  |

---

## Private APIs

| Endpoint | Implemented | Notes |
|----------|-------------|-------|
| GET /v1/me/getbalance | Yes |  |

---

※ Notes 欄は **空欄のまま**使用する（説明を書かない）
