# REVIEW-RELIABILITY

本レビューは Reliability（信頼性）軸に基づく確認を行う。

対象: （PR番号 / 対象範囲を記載）

---

# Scope

変更対象の概要を記載。

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

* [ ] 429 / timeout / partial failure が混在していない
* [ ] Expected / Unexpected の分類が維持されている
* [ ] 再試行が安全である（重複実行の危険がない）
* [ ] エラー種別が呼び出し側に予測可能
* [ ] 診断可能な情報が残る（secret除外）

---

# Findings

## Must

## Should

## Nit

---

# Conclusion

本変更は Reliability 軸において重大な問題はない / 修正が必要。
