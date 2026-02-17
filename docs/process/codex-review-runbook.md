# Codex Review Runbook

本書は、`docs/process/review-framework.md` の運用を Codex で実施するための実務手順書である。
レビュー観点の正本は `docs/process/review-framework.md`、PR 最終チェックの正本は `docs/process/process.md` とする。

---

## 1. 目的

- PR ごとのレビュー運用を定型化し、判断の揺らぎを減らす。
- 指摘を `Must / Should / Nit` に統一し、修正優先度を明確化する。
- 例外が必要な場合は `docs/process/exceptions.md` への記録漏れを防ぐ。

---

## 2. 基本運用

- 全 PR で `L1` を実施する。
- `L2` はトリガ該当時のみ実施する。
- `L3` は Stage 締めでのみ実施する。
- 1 PR 1 主題を原則とする（巨大差分は分割）。

---

## 3. 実施フロー（推奨）

1. `L1` 一括レビューを実施する。
2. `Must` を修正する。
3. 必要な `L2` テンプレートだけ追加実施する。
4. 未解消 `Must` のみ再レビューする。
5. Merge 前に `docs/process/process.md` のチェックリストを再確認する。

---

## 4. L2 トリガ対応表

- Contracts / public 変更: `docs/process/reviews/templates/REVIEW-CONTRACTS.md`
- Signer / 認証 / Canonicalize 変更: `docs/process/reviews/templates/REVIEW-SECURITY.md`
- Error / Retry / 429 / timeout 変更: `docs/process/reviews/templates/REVIEW-RELIABILITY.md`
- Normalized / Cross-exchange 構造変更: `docs/process/reviews/templates/REVIEW-BOUNDARY.md` と `docs/process/reviews/templates/REVIEW-CONSISTENCY.md`
- Breaking change 含む: `docs/process/reviews/templates/REVIEW-CHANGE.md`
- 文書構造 / SSOT 変更: `docs/process/reviews/templates/REVIEW-DOCS.md`

---

## 5. Codex 指示テンプレート

### 5.1 L1 一括レビュー

```text
このPR差分をレビューしてください。
基準は docs/process/process.md の「7.2 必須（Merge 前に必ず確認）」です。
出力は Must / Should / Nit の順で、重大度順に列挙してください。
各指摘に対象ファイルと行番号を付けてください。
可能なら修正パッチまで作成してください。
```

### 5.2 L2（軸別）レビュー

```text
このPR差分は Contracts 変更を含みます。
docs/process/reviews/templates/REVIEW-CONTRACTS.md のチェック項目でレビューしてください。
Must / Should / Nit の形式で、重要な順に指摘してください。
```

※ `Contracts` を `Security` / `Reliability` / `Boundary` / `Consistency` / `Change` / `Docs` に置き換えて使用する。

### 5.3 再レビュー（差分最小）

```text
前回レビューの Must 指摘だけを再判定してください。
未解消項目のみ列挙し、解消済みは簡潔に完了と示してください。
新規指摘は重大なものに限定してください。
```

---

## 6. 巨大差分の扱い

- まず PR を主題単位で分割する（Contracts、Security、Refactor、Docs など）。
- 分割できない場合は、最初に `Must` のみ抽出させる。
- `Must` 解消後に `Should / Nit` を再実行する。

---

## 7. 例外が必要なとき

- 規範からの逸脱が必要なら、`docs/process/exceptions.md` に記録する。
- レビュー指摘で「例外記録が必要」と判定された項目は、未記録のまま close しない。

---

## 8. 期待出力フォーマット

- `Must`: Merge 前に必須修正
- `Should`: 可能なら同PRで修正
- `Nit`: 任意改善

各項目は、以下の 1 行形式を推奨する。

`[Severity] <要約> - <file:line> - <根拠（どの規約か）>`

