# REVIEW-SECURITY

本レビューは Security（安全性）軸に基づく確認を行う。

対象: （PR番号 / 対象範囲を記載）

---

# Scope

変更対象の概要を記載。

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

* [ ] 署名対象データの順序・形式が仕様と一致している
* [ ] Canonicalize の整合性が保たれている
* [ ] 秘密情報（APIキー/署名素材）がログや例外に出力されない
* [ ] nonce / timestamp の扱いが安全
* [ ] 認証情報が不要に長期保存されていない

---

# Findings

## Must

## Should

## Nit

---

# Conclusion

本変更は Security 軸において重大な問題はない / 修正が必要。
