# REVIEW-CONTRACTS

本レビューは Contracts（公開面/契約/型境界）軸に基づく確認を行う。

対象: （PR番号 / 対象範囲を記載）

---

# Scope

Contracts の変更点（I/F, DTO, 失敗表現, public surface の増減）を記載。

---

# Checklist

* [ ] public surface の増減が明示されている（追加/削除/変更）
* [ ] Try / OrThrow の統一が維持されている
* [ ] `string` 流入禁止が維持されている（型境界が守られている）
* [ ] DTO / ValueObject 境界が維持されている（意味確定の置き場が適切）
* [ ] Contracts に取引所固有語彙が混入していない

---

# Findings

## Must

## Should

## Nit

---

# Conclusion

本変更は Contracts 軸において重大な問題はない / 修正が必要。

