# REVIEW-SECURITY（2026-02-17 / stage9）

本レビューは Security（安全性）軸に基づく確認を行う。

対象: branch `stage9`（Contracts capability 分離・契約同期）

---

# Scope

変更対象の安全性影響を確認した。

- 主変更は Contracts / Facade / Adapter の capability 境界整理
- Signer / Canonicalize / nonce / timestamp のアルゴリズム変更はなし
- 認証情報の保存/出力パスに変更なし

---

# Threat Focus

以下の観点を確認する。

* Signer / Signature
* Canonicalize 処理
* 認証情報（APIキー等）の扱い
* nonce / timestamp 処理
* ログ出力内容

---

# Checklist

* [x] 署名対象データの順序・形式が仕様と一致している（変更なし）
* [x] Canonicalize の整合性が保たれている（変更なし）
* [x] 秘密情報（APIキー/署名素材）がログや例外に出力されない（変更なし）
* [x] nonce / timestamp の扱いが安全（変更なし）
* [x] 認証情報が不要に長期保存されていない（変更なし）

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

本変更は Security 軸において重大な問題はない。
