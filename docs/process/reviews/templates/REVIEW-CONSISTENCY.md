# REVIEW-CONSISTENCY

本レビューは Consistency（命名/語彙/整合）軸に基づく確認を行う。

対象: （PR番号 / 対象範囲を記載）

---

# Scope

命名・語彙・構造の変更点（取引所内/取引所間）を記載。

---

# Checklist

* [ ] EndpointId 起点の命名が維持されている
* [ ] 同概念が分裂していない（同じ意味の型/定数/語彙が複数できていない）
* [ ] 定数 / enum の統一が守られている（直書きの増殖がない）
* [ ] Cross-exchange parity が維持されている（同責務のAPI/DTOが不必要に差分化していない）
* [ ] 取引所差異の閉じ込めが崩れていない（共通層に差分理由を持ち込んでいない）

---

# Findings

## Must

## Should

## Nit

---

# Conclusion

本変更は Consistency 軸において重大な問題はない / 修正が必要。

