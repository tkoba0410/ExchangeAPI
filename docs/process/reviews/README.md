# reviews Directory Policy

`docs/process/reviews/` は、実施済みレビューの監査ログ（履歴）を格納するディレクトリである。

## 1. 位置づけ

- `docs/process/reviews/templates/*.md`: 現行テンプレート（規範運用対象）
- `docs/process/reviews/REVIEW-*.md` / `docs/process/reviews/STAGE*-*.md`: 履歴監査ログ（スナップショット）

履歴監査ログは、実施時点の語彙・判定表現を保持し、原則として後追いで語彙統一しない。

## 2. 語彙Lintの対象範囲

重大度語彙Lint（`Severity` / `FatalClass`）は次のみを対象とする。

- `docs/process/codex-review-runbook.md`
- `docs/process/review-framework.md`
- `docs/process/reviews/templates/*.md`

次は対象外とする。

- `docs/process/reviews/REVIEW-*.md`
- `docs/process/reviews/STAGE*-*.md`
- `docs/archive/**`
