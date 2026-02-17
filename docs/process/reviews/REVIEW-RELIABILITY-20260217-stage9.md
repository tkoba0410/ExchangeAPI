# REVIEW-RELIABILITY（2026-02-17 / stage9）

本レビューは Reliability（信頼性）軸に基づく確認を行う。

対象: branch `stage9`（Contracts capability 分離・エラー契約同期）

---

# Scope

変更対象の信頼性影響を確認した。

- `BatchError` から取引所識別を除去し、エラー契約の責務境界を明確化
- 未対応機能の表現を「NotSupported 常用」から「nullable capability 事前判定」に移行
- 既存テスト（Common / Composition / Adapter / Docs.Inventory）で回帰がないことを確認

---

# Failure Modes

以下の観点を確認する。

* 429（Rate Limit）
* timeout
* partial failure
* retry
* idempotency

---

# Checklist

* [x] 429 / timeout / partial failure が混在していない
* [x] Expected / Unexpected の分類が維持されている
* [x] 再試行が安全である（重複実行の危険がない）
* [x] エラー種別が呼び出し側に予測可能
* [x] 診断可能な情報が残る（secret除外）

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

本変更は Reliability 軸において重大な問題はない。
