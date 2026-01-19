# Bittrade EndpointId 運用ノート

## 目的

本書は、Bittrade における EndpointId 運用上の **例外規則と命名裁定**を記載する。

Endpoint の列挙や仕様の正本は、公式 API ドキュメントおよび endpoint inventory に委ねる。

---

## 同一 Path に複数 Method が存在する Endpoint

* Bittrade API では、**同一 Path に GET / POST 等が割り当てられるケース**が存在する
* この場合、EndpointId の一意性を保つため、**Method を含めた命名を必須**とする

例：

* `OrdersGet`
* `OrdersPost`

これは common の衝突回避規則の適用例である。

---

## cancel / submitcancel 等の Path 表現について

* Bittrade API では、操作の意味が Path 名に直接現れない場合がある
* EndpointId は Path 名を直訳せず、**操作の意味を優先して命名**する

例：

* `/orders/{id}/submitcancel` → `OrdersCancel`

この裁定理由は inventory の Notes に残す。

---

## pagination / limit の補足

* Bittrade API では endpoint ごとに pagination / limit の挙動が異なる
* 実装依存の詳細は公式ドキュメントを参照する
* inventory の Notes には、

  * cursor 有無
  * limit の上限

など、識別に必要な最小情報のみを記載する。

---

## 運用上の注意

* Bittrade 側で API が追加・変更された場合

  * 公式ドキュメントを確認
  * inventory を更新
  * 命名に迷いがあれば本書に裁定理由を追記
