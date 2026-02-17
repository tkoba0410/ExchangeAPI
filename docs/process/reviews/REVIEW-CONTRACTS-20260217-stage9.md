# REVIEW-CONTRACTS（2026-02-17 / stage9）

本レビューは Contracts（公開面/契約/型境界）軸に基づく確認を行う。

対象: branch `stage9`（全体監査後の Must 修正反映）

---

# Scope

Contracts の変更点（I/F, DTO, 失敗表現, public surface の増減）を確認した。

- `BatchError` から `Exchange` を除去し、取引所非依存を回復
- `Candlesticks` を `IPublicApi` から分離し、`ICandlesticksApi`（nullable capability）へ移行
- `IExchangeClient` に `Candlesticks` capability を追加
- 契約文書と inventory の対応を同期

---

# Checklist

* [x] public surface の増減が明示されている（追加/削除/変更）
* [x] Try / OrThrow の統一が維持されている
* [x] `string` 流入禁止が維持されている（型境界が守られている）
* [x] DTO / ValueObject 境界が維持されている（意味確定の置き場が適切）
* [x] Contracts に取引所固有語彙が混入していない

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

本変更は Contracts 軸において重大な問題はない。
