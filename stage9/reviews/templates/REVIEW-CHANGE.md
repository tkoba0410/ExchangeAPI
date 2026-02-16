# REVIEW-CHANGE

本レビューは Change（変更統治）軸に基づく確認を行う。

対象: （PR番号 / 対象範囲を記載）

---

# Scope

変更内容の概要を記載。

---

# Breaking Change 判定

* public surface に変更があるか
* 既存APIの挙動に変更があるか
* 呼び出しコード修正が必要か
* DTO構造に変更があるか

該当する場合は CHANGE 記録を作成する。

---

# Checklist

* [ ] Breaking Change の有無が明示されている
* [ ] 必要な場合、stage9/changes に記録が追加されている
* [ ] 影響範囲が明確である
* [ ] 移行方法が具体的である
* [ ] Bot 影響が明示されている（該当時）

---

# Findings

## Must

## Should

## Nit

---

# Conclusion

本変更は Change 軸において重大な問題はない / 修正が必要。

---

# レビュー文書の位置づけ

- 本書（stage9/review-framework.md）はレビュー体系案（草案）の正本（暫定SSOT）である
- stage9/reviews/ 配下は具体レビュー資産（成果物）である
- stage9/reviews/templates/ は L2 / L3 用の雛形である
- stage9/changes/ は Breaking Change の記録場所である
