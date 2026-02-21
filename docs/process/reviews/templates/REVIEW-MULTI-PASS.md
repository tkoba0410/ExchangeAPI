# REVIEW-MULTI-PASS

本テンプレートは、**モデル / 実行環境（CLI, Web） / 深度**を変えて
同一変更を複数回レビューするための実行様式を定義する。

重大度定義は `PROJECT-FATAL-DEFINITION.md` を参照する。
Fatal 判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 変更範囲:
* 監査目的（例: 論理整合 / 回帰検出 / 規範整合）:
* 正本（TopSpec / LayoutShape / Contracts / Governance / Inventory）:

---

## 判定サマリ表

| 観点 | 判定 (OK / 要修正 / NG) | 重大度 (F番号明示) | 備考 |
| --- | --- | --- | --- |
| P1（CLI: 事実確認） |  |  |  |
| P2（別モデル/環境: 規範整合） |  |  |  |
| P3（CLI: 反証） |  |  |  |
| 総合判定 |  |  |  |

---

## 1. パス設計（固定）

| Pass | 環境 | 深度 | 主目的 | 主な入力 | 期待成果 |
| --- | --- | --- | --- | --- | --- |
| P1 | CLI | L1-L2 | 事実確認（差分 / 依存 / build/test） | diff, csproj, test結果 | 機械検証可能な事実 |
| P2 | Web or 別モデル | L2 | 文書・規範整合 | normative docs, inventory | 条文衝突 / 解釈差の抽出 |
| P3 | CLI | L3 | 反証（将来変更時の破綻点） | P1/P2結果, 主要コード | 過拘束/過緩和・盲点の抽出 |

補足:

* P1→P2→P3 の順序を固定する（MUST）。
* 各Passで判定根拠を記録し、推測のみで結論を出さない（MUST）。

---

## 2. Pass別チェック項目

### P1（CLI: 事実確認）

* [ ] `dotnet build ExchangeApi.slnx -warnaserror` を実行し結果を記録した
* [ ] `dotnet test ExchangeApi.slnx` を実行し結果を記録した
* [ ] 変更ファイルの責務境界（層 / 依存方向）を確認した
* [ ] 追加した規範が既存テストで検証可能か確認した

### P2（別モデル/環境: 規範整合）

* [ ] TopSpec / LayoutShape / Contracts の条文・拘束が衝突していない
* [ ] Process / Checklist の運用項目が正本に反していない
* [ ] 重複規定がある場合、正本参照へ整理されている

### P3（CLI: 反証）

* [ ] 想定外ディレクトリ・名前空間混入の耐性を確認した
* [ ] 新規取引所追加時に破綻しない拘束か確認した
* [ ] 過剰拘束（実装自由度を不必要に奪う）を確認した
* [ ] 回避不能な例外がある場合、`docs/process/exceptions.md` への記録要否を判定した

---

## 3. 指摘フォーマット（必須）

すべての指摘は次の形式で記録する。

| ID | Severity | Pass | File:Line | 事実 | 影響 | 最小修正案 | CI化可否 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| R-001 | High | P1 | path:line | 何が起きているか | 何が壊れるか | 最小変更 | Yes/No |

記法:

* `Severity` は `Fatal / High / Medium / Low / Nit` を使用する。
* `File:Line` は 1-based で記載する。
* 事実と推論を分離して記載する。

---

## 4. 重複排除ルール

* 同一事象の重複指摘は 1件へ統合する。
* 競合する指摘は、**実行で再現できる事実**を優先する。
* 条文解釈の衝突は TopSpec 優先で裁定する。

---

## 5. マージ判定ゲート

* Fatal: 1件でも存在したら NG
* High: 0件であること（未解消は NG）
* Medium: 修正または例外記録が必須
* Low/Nit: 次PR繰越可（繰越理由を記録）

---

## 6. 最終出力

* 判定: `OK / 要修正 / NG`
* Must一覧（High 以上）
* 推奨改善（Medium 以下）
* 未解決リスク
* 実行ログ要約（build/test）

---

## CI自動化候補

* `dotnet build ExchangeApi.slnx -warnaserror` / `dotnet test ExchangeApi.slnx` 実行結果の記録検査
* 指摘フォーマット（`ID / Severity / Pass / File:Line / 事実 / 影響 / 最小修正案 / CI化可否`）の列欠落検査
* `Severity` 語彙（`Fatal / High / Medium / Low / Nit`）の妥当性検査
* マージ判定ゲート（Fatal/High未解消件数）の自動集計

---

## 7. 関連Normative / 運用

* `docs/normative/topspec.md`
* `docs/normative/layout/exchange-module-shape.json`
* `docs/normative/contracts/contracts.md`
* `docs/normative/governance.md`
* `docs/process/process.md`
* `docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md`
