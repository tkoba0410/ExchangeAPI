# REVIEW-TEMPLATES-20260219-stage9

`docs/process/reviews/templates/REVIEW-TEMPLATES.md` に基づくメタ準拠監査結果（`REVIEW-USER-GUIDE` 追加）。

---

## 0. 対象

* 対象テンプレート:
  * `docs/process/reviews/templates/REVIEW-USER-GUIDE.md`
* 対象ブランチ: `stage9`
* 実施日: 2026-02-19

---

## 1. 判定サマリ

| 観点 | 判定 (OK / 要修正 / NG) | 重大度 | 備考 |
| --- | --- | --- | --- |
| 判定可能性 | OK | High | 判定基準/OK/NG/不合格例/修正方針を全観点で明示 |
| 重大度定義 | OK | Medium | PROJECT-FATAL-DEFINITION を参照し、F番号を明示 |
| SSOT整合 | OK | Medium | SSOTは参照リンクで担保（写経しない方針） |
| レイヤ明確性 | OK | Medium | 対象は User Guide（Docs）で固定。層ジャンプ観点は適用外 |
| CI化余地 | OK | Medium | CI自動化候補を列挙 |
| 再利用性 | OK | Medium | 0章に対象/想定読者/環境を持ち、別PRでも再利用可能 |

---

## 2. 指摘（根拠付き）

* [OK] 判定基準/OK/NG/不合格例/修正方針が観点ごとに存在 - `docs/process/reviews/templates/REVIEW-USER-GUIDE.md:37` - `docs/process/reviews/templates/REVIEW-TEMPLATES.md:42`
* [OK] Fatal定義はPROJECT横断定義を参照し、F番号明示を要求 - `docs/process/reviews/templates/REVIEW-USER-GUIDE.md:5` - `docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md:9`
* [OK] SSOT侵食防止（写経しない）を修正方針に明記 - `docs/process/reviews/templates/REVIEW-USER-GUIDE.md:47` - `docs/process/reviews/templates/REVIEW-TEMPLATES.md:77`
* [OK] CI自動化候補を持つ - `docs/process/reviews/templates/REVIEW-USER-GUIDE.md:115` - `docs/process/reviews/templates/REVIEW-TEMPLATES.md:108`
* [OK] 関連Normative/判例ブロックを持つ - `docs/process/reviews/templates/REVIEW-USER-GUIDE.md:123` - `docs/process/reviews/templates/REVIEW-TEMPLATES.md:125`

---

## 3. 最終判定

* OK（メタ基準準拠）

---

## 4. 運用メモ

1. `README.md` / `docs/guides/*` / `docs/process/templates/*` の変更PRでは本テンプレを適用する
2. ガイドに規範本文を写経しない（SSOTリンクに寄せる）

