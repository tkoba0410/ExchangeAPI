# REVIEW-CHANGE（2026-02-17 / stage9）

本レビューは Change（変更統治）軸に基づく確認を行う。

対象: branch `stage9`（全体監査後の Must 修正反映）

---

# Scope

公開契約の変更（`IPublicApi` / `IExchangeClient` / `BatchError`）と、関連文書更新の整合を確認した。

---

# Breaking Change 判定

* public surface に変更があるか  
  → あり（`IPublicApi` から `GetCandlesticksAsync` 除去、`ICandlesticksApi` 追加、`IExchangeClient.Candlesticks` 追加）
* 既存APIの挙動に変更があるか  
  → あり（未対応機能の表現を nullable capability 判定に変更）
* 呼び出しコード修正が必要か  
  → あり（`IPublicApi` 直接呼び出しから capability 経由に移行）
* DTO構造に変更があるか  
  → あり（`BatchError` のプロパティ変更）

CHANGE 記録: `docs/process/CHANGE-20260217-contracts-capability-boundary.md`

---

# Checklist

* [x] Breaking Change の有無が明示されている
* [x] 必要な場合、docs/process に記録が追加されている
* [x] 影響範囲が明確である
* [x] 移行方法が具体的である
* [x] Bot 影響が明示されている（該当時）

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

本変更は Change 軸において重大な未解消項目はない。
