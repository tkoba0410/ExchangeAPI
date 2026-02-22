# Stage9 クローズ実行チェックリスト

本チェックリストは `docs/stage_9_close_policy.md` に基づき、
Stage9 クローズ判定を 1 ページで実施するための運用テンプレートである。

参照正本:
- `docs/stage_9_close_policy.md`
- `docs/process/review-framework.md`
- `docs/process/codex-review-runbook.md`
- `docs/process/process.md`（7.2）

---

## 0. 実行メタ

- 実施日: `YYYY-MM-DD`
- 対象ブランチ: `<branch>`
- 収束アンカー: `<正本文書 path> + <主実装 path>`
- 実施者: `<name>`
- Maintainer（最終承認者）: `<name>`

---

## 1. 収束運用（無限ループ抑止）

- [ ] 初回レビューで対象スコープの指摘を出し切った
- [ ] 再レビューは前回未解消 `NG` のみを対象とした
- [ ] 再レビューでの新規指摘は重大回帰（`Fatal/High`）のみとした
- [ ] 同一論点が 3 回目でも未収束の場合、レビュー継続を停止して裁定へ切り替えた
- [ ] 裁定結果を `docs/process/exceptions.md` または `docs/process/CHANGE-*.md` に記録した

---

## 2. 必須条件ゲート（Close Policy 3章）

- [ ] Normative 文書間の論理矛盾ゼロ
- [ ] Doc ↔ Code の不整合ゼロ（収束アンカー対象）
- [ ] Inventory ↔ 実装の不一致ゼロ（対象スコープ）
- [ ] 層境界侵食ゼロ
- [ ] 公開契約に対する未宣言の破壊的変更ゼロ
- [ ] 自動テスト（Unit / Integration / Validation）成功
- [ ] 未解消 `NG (Fatal/High)` がゼロ
- [ ] ループ抑止規則（本チェックリスト 1章）を満たした

---

## 3. Merge 前最終ゲート（Process 7.2）

- [ ] `docs/process/process.md` 7.2 必須チェックに No がない
- [ ] 最終レビュー結果が `NG=0 / Fatal=0` である

---

## 4. 証跡パック（Close Policy 5章）

- [ ] `docs/process/reviews/REVIEW-SUMMARY-<date>-stage9.md`
- [ ] `docs/process/reviews/REVIEW-MERGE-CHECK-<date>-stage9.md`
- [ ] `docs/process/reviews/STAGE9-FINAL-DECISION-<date>.md`
- [ ] 自動テスト成功の証跡（CI URL または同等ログ）

CI/ログリンク:
- `<url-or-log-ref>`

---

## 5. 最終裁定

- 判定: `可 / 不可`
- 未解消ブロッカー（あれば）:
  - `<none or issue list>`
- Maintainer 明示承認:
  - 名前: `<name>`
  - 日付: `YYYY-MM-DD`
  - コメント: `<optional>`

クローズ後更新:
- [ ] `stage9.md` の最新判定リンクと日付を更新した
