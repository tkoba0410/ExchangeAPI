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
- 文書構造 / SSOT 変更: `docs/process/reviews/templates/REVIEW-DOCS.md`（補助監査）

※ `REVIEW-DOCS` は `review-framework` の 7品質軸とは別の補助監査であり、軸件数集計には含めない。

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

---

## 9. codex-cli コピペ用指示

以下は `codex-cli` にそのまま貼り付けて使うテンプレートである。

### 9.1 標準（L1 + 必要時L2）

```text
対象: このPR差分のみ（branch: <feature-branch>, base: <base-branch>）
出力: Must / Should / Nit（各項目に file:line 必須）
基準: docs/process/codex-review-runbook.md と関連テンプレートに厳密準拠

実施手順:
1. まず L1 を実施する（docs/process/process.md 7.2 を基準）。
2. 差分内容から L2 トリガを判定し、該当軸のみ追加レビューする。
3. 例外が必要な指摘は docs/process/exceptions.md への記録要否を明示する。
4. 形式は必ず次の1行形式を使う:
   [Severity] <要約> - <file:line> - <根拠（どの規約か）>
5. 最後に未解消 Must 件数を出す。
```

### 9.2 再レビュー（Mustのみ）

```text
対象: このPR差分のみ（branch: <feature-branch>, base: <base-branch>）
出力: Must のみ（各項目に file:line 必須）
基準: 前回レビュー結果と docs/process/codex-review-runbook.md に厳密準拠

実施手順:
1. 前回の Must 指摘が解消されたかのみ判定する。
2. 未解消 Must だけ列挙する。
3. 新規指摘は重大な回帰に限定する。
4. 最後に未解消 Must 件数を出す。
```

### 9.3 軸固定（L2個別）

```text
対象: このPR差分のみ（branch: <feature-branch>, base: <base-branch>）
出力: Must / Should / Nit（各項目に file:line 必須）
基準: docs/process/reviews/templates/REVIEW-<AXIS>.md に厳密準拠
例: REVIEW-CONTRACTS.md / REVIEW-SECURITY.md / REVIEW-BOUNDARY.md

実施手順:
1. 指定軸のみでレビューする。
2. 形式は次の1行形式を使う:
   [Severity] <要約> - <file:line> - <根拠（どの規約か）>
3. 最後に未解消 Must 件数を出す。
```

---

## 10. 全体監査モード（Repository-Wide Audit）

本章は PR 差分ではなく、リポジトリ全体を対象に監査する場合の運用を定義する。
通常の `L1/L2` 運用に置き換えるものではなく、`L3` 相当の節目監査として実施する。

### 10.1 実施タイミング

- Stage 締め前
- リリース前（例: `v1.0.0` 直前）
- 大規模リファクタ後
- 規範文書を横断的に更新した後

### 10.2 手順

1. 監査対象ブランチを固定する（例: `stage9` / `main`）。
2. 監査スコープを明示する（コード + 文書、または文書のみ）。
3. 7軸（Boundary / Consistency / Contracts / Reliability / Security / DX / Change）を全適用する。
4. 出力を `Must / Should / Nit` に統一し、`file:line` を必須化する（`Docs` は補助監査として別枠）。
5. 軸ごとの件数サマリを作成する。
6. 最後に `最優先 Must Top10` を提示する。
7. 例外が必要な項目は `docs/process/exceptions.md` への記録要否を明示する。

### 10.3 codex-cli / codex-web 共通テンプレート

```text
対象: リポジトリ全体（branch: <target-branch>）
出力: Must / Should / Nit（各項目に file:line 必須）
基準: docs/process/review-framework.md と docs/process/codex-review-runbook.md に厳密準拠

実施手順:
1. PR差分ではなく、現行ブランチの全体を監査する。
2. 7軸（Boundary / Consistency / Contracts / Reliability / Security / DX / Change）を全適用する。
   Docs は補助監査（REVIEW-DOCS）として別枠で実施する。
3. 各指摘は次の1行形式を使う:
   [Severity] <要約> - <file:line> - <根拠（どの規約か）>
4. 軸ごとに Must / Should / Nit 件数を集計する。
5. 最後に「最優先 Must Top10」と「未解消 Must 総件数」を出す。
6. 例外が必要な項目は docs/process/exceptions.md への記録要否を明示する。
```

### 10.4 再監査テンプレート（全体）

```text
対象: リポジトリ全体（branch: <target-branch>）
出力: Must のみ（各項目に file:line 必須）
基準: 前回全体監査結果と docs/process/codex-review-runbook.md に厳密準拠

実施手順:
1. 前回の Must 指摘の解消状況のみ再判定する。
2. 未解消 Must を重大順で列挙する。
3. 新規指摘は重大な回帰に限定する。
4. 最後に「未解消 Must 総件数」を出す。
```

---

## 11. 監査後の修正手順（チェックリスト）

全体監査の結果に `Must` が出た場合は、次の順で修正を進める。

### 11.1 実施順

1. `Must` の一覧を確定する（重複指摘を統合する）。
2. 裁定が必要な項目を先に分離する（規範衝突 / 設計方針の分岐）。
3. 規範変更が必要なら、実装より先に `docs/normative/*` を更新する。
4. 規範に合わせて `src/` と関連文書（inventory/process）を修正する。
5. `L2` テンプレートで該当軸のみレビューする（例: Contracts/Change）。
6. `Mustのみ再監査` を実施し、未解消件数を更新する。
7. `docs/process/process.md` の 7.2 チェックリストで Merge 前最終確認を行う。
8. 修正不能な逸脱が残る場合のみ `docs/process/exceptions.md` に登録する。

### 11.2 codex-cli コピペ用（修正フェーズ）

```text
対象: 前回監査で検出した Must 項目
出力: 修正計画（裁定要/不要の分類）と修正パッチ案
基準: docs/process/codex-review-runbook.md と docs/process/process.md 7.2

実施手順:
1. Must を「裁定が必要」「裁定不要」に分類する。
2. 裁定不要の項目から先に修正パッチを作成する。
3. 裁定必要の項目は、分岐案A/Bと影響範囲を提示する。
4. 修正後に Mustのみ再監査を実施し、未解消 Must 件数を出す。
```
