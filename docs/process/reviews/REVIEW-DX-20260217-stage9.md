# REVIEW-DX（2026-02-17 / stage9）

本レビューは DX（開発者体験/誤用耐性/診断しやすさ）軸に基づく確認を行う。

対象: branch `stage9`（Contracts capability 分離・再監査後）

---

# Scope

利用形態の変化（呼び出し方、エラー時の挙動、診断可能性）を確認した。

- `GetCandlesticksAsync` は `IPublicApi` 直呼び出しから、`IExchangeClient.Candlesticks`（nullable capability）経由へ移行
- 未対応機能は通常制御フローで `NotSupported` を受ける形から、事前 capability 判定（`null`）へ移行
- `BatchError` から取引所識別を除去し、エラー契約を単純化

---

# Checklist

* [x] 自然な利用形態が維持されている（過度な前提・手順増加がない）
* [x] 誤用しにくい設計が維持されている（危険なデフォルト/曖昧な型が増えていない）
* [x] エラー時に次行動が分かる（メッセージ/分類/診断情報が適切）
* [x] ログ/観測は診断可能だが secret を含まない

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

本変更は DX 軸において重大な問題はない。
