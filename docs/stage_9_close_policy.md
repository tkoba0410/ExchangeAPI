# Stage9 クローズ方針（無限レビューループ抑止・整合版）

---

## 0. 位置づけ（Process）

本書は Stage9 終了判定の運用方針（Process）である。

設計規範の正本は `docs/normative/*` とし、本書は再定義しない。

レビュー観点は `docs/process/review-framework.md`、
重大度運用は `docs/process/codex-review-runbook.md`、
Merge 前最終チェックは `docs/process/process.md` を正とする。

本書は、上記運用を Stage9 クローズ時に適用する停止規則
（無限レビューループ抑止）を定める。

---

## 1. 目的

Stage9 は「安定化」および「監査耐性強化」を目的とする。

同時に、レビュー往復の肥大化を防ぐため、
終了判定の打ち切り条件と裁定移行条件を固定する。

---

## 2. 統治原則（Governing Principles）

1. 改善より整合性を優先する。
2. 柔軟性より決定性（Determinism）を優先する。
3. 暗黙挙動より明示契約を優先する。
4. 修正は局所的かつ最小差分に限定する。
5. 規約追加による整合化は禁止する。
6. 判断が曖昧な場合は fail-open せず、裁定まで close しない。

---

## 3. 必須条件（Mandatory Conditions）

以下をすべて満たすこと。

1. Normative 文書間の論理矛盾ゼロ。
2. Doc ↔ Code の不整合ゼロ（収束アンカー対象）。
3. Inventory ↔ 実装の不一致ゼロ（対象スコープ）。
4. 層境界侵食ゼロ。
5. 公開契約に対する破壊的変更ゼロ（未宣言変更）。
6. すべての自動テスト（Unit / Integration / Validation）が成功。
7. 未解消 `NG (Fatal/High)` がゼロ。
8. 本書 9 章のループ抑止規則が満たされている。

いずれかが満たされない場合、終了不可とする。

---

## 4. 重大度と終了判定（Severity / Closure Gate）

重大度語彙は `docs/process/codex-review-runbook.md` に統一する。

* `NG (Fatal)` / `NG (High)` は終了阻害。
* `要修正 (Medium/Low)` は原則修正対象。繰越す場合は理由と記録先を明示する。
* `OK (Nit)` は任意改善であり終了阻害としない。

判断が曖昧な場合は `NG (High)` として扱い、
`docs/normative/governance.md` に従って裁定する（fail-closed）。

---

## 5. 証跡要件（Evidence Pack）

Stage9 終了判定では、少なくとも次の証跡を揃える。

1. `docs/process/reviews/REVIEW-SUMMARY-<date>-stage9.md`（L3 相当）
2. `docs/process/reviews/REVIEW-MERGE-CHECK-<date>-stage9.md`（7.2 最終チェック）
3. `docs/process/reviews/STAGE9-FINAL-DECISION-<date>.md`（最終裁定）
4. 自動テスト成功を示す実行証跡（CI 実行または同等ログ）

---

## 6. 論理整合要件（Logical Integrity Requirement）

各 Normative ルールは以下を満たすこと。

* Rule（明示制約）
* Rationale（存在理由）
* Failure Impact（違反時の影響）

影響が説明できないルールは許可しない。

---

## 7. 境界保全要件（Boundary Protection Requirement）

層分離は機械的に検証可能でなければならない。

* 層間の型漏れ禁止。
* 検証済み ValueObject を bypass する primitive 受け渡し禁止。
* 共通層における暗黙変換禁止。

違反は `NG (Fatal/High)` として扱う。

---

## 8. 凍結宣言（Freeze Declaration）

Stage9 完了時点で以下を凍結する。

* 品質軸（7 軸）
* 深度モデル（L1 / L2 / L3）
* 重大度モデル（Severity / FatalClass）
* 収束ゲート（収束アンカー、再レビュー範囲、3 回未収束時の裁定移行）
* `docs/process/process.md` 7.2 の最終ゲート運用

実装詳細、内部リファクタ、DX 改善は、
上記凍結対象と Normative 契約を破らない範囲で継続更新できる。

---

## 9. 無限レビューループ抑止規則（Loop Suppression Rule）

1. レビュー開始時に `収束アンカー`（正本文書 1 つ + 主実装 1 箇所）を固定する。
2. 初回レビューは全指摘を許可する。
3. 再レビューは「前回未解消 `NG`」のみを対象とする。
4. 再レビューでの新規指摘は、重大回帰（`Fatal/High`）に限定する。
5. 同一論点が 3 回目でも未収束の場合、レビュー継続を停止し、裁定へ切り替える。
6. 裁定結果は `docs/process/exceptions.md` または `docs/process/CHANGE-*.md` に記録して close する。

---

## 10. 文書決定性（Documentation Determinism）

正本優先順は `docs/normative/governance.md` に従う。

文書と実装が乖離した場合は、正本側へ収束させる。

将来構想・補足説明・提案事項は Informative と明示する。

---

## 11. 終了権限（Closure Authority）

必須条件と証跡要件が満たされたことを確認した上で、
Maintainer の明示的承認により終了とする。

---

## 12. 終了後の変更規律（Post-Closure Rule）

終了後も、次は許可する。

* セキュリティ修正
* 重大欠陥修正
* 非破壊の保守改善（内部整理、DX 改善、運用文書改善）

ただし、凍結対象または公開契約/層構造を変更する場合は、
新 Stage 宣言または裁定手続きを必須とする。
