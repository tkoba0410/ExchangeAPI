# REVIEW-BOUNDARY

本レビューは Boundary（層/依存/境界）軸に基づく確認を行う。

対象: （PR番号 / 対象範囲を記載）

---

# Scope

対象の層（Wire / Raw / Normalized / Adapter / Composition / Contracts 等）と変更点の概要を記載。

---

# Checklist

* [ ] 層責務の混線がない（層ジャンプ、責務の漏れ込みがない）
* [ ] 依存方向の逆流がない（上位が下位の内部へ侵入していない）
* [ ] Core が exchange 固有へ依存していない
* [ ] 取引所差異が `src/Exchanges/<Ex>/` 配下に閉じ込められている
* [ ] 横断的な情報塊（ExecutionContext 等）の復活がない

---

# Findings

## Must

## Should

## Nit

---

# Conclusion

本変更は Boundary 軸において重大な問題はない / 修正が必要。

