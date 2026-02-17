# REVIEW-CONSISTENCY（2026-02-17 / stage9）

本レビューは Consistency（命名/語彙/整合）軸に基づく確認を行う。

対象: branch `stage9`（Contracts capability 分離・契約同期）

---

# Scope

命名・語彙・構造の変更点（取引所内/取引所間）を確認した。

- `Candlestick` 系 API を capability 単位で統一（`ICandlesticksApi`）
- bitflyer/bittrade の差異は `IExchangeClient.Candlesticks` の `null/non-null` で整合
- inventory (`endpoints-contracts.md`) と interface の整合テスト結果を確認

---

# Checklist

* [x] EndpointId 起点の命名が維持されている
* [x] 同概念が分裂していない（同じ意味の型/定数/語彙が複数できていない）
* [x] 定数 / enum の統一が守られている（直書きの増殖がない）
* [x] Cross-exchange parity が維持されている（同責務のAPI/DTOが不必要に差分化していない）
* [x] 取引所差異の閉じ込めが崩れていない（共通層に差分理由を持ち込んでいない）

---

# Findings

## Must

なし

## Should

なし

## Nit

なし

---

# Conclusion

本変更は Consistency 軸において重大な問題はない。
