# Stage8

## Stage8 ゴール（確定）

TopSpec / Contracts / Governance / Process / Exceptions / Inventory の役割が重複せず、設計判断の参照先が常に一意で、README と _references が判断を含まない状態を完成させる。

## DoD（完了条件チェックリスト）

- [ ] README が導線のみで、SSOT指定や判断文が存在しない（→ docs/index.md / docs/process.md に寄せる）
- [ ] docs/index.md が Exceptions を「逸脱台帳/決定記録」として案内し、エラー分類とは混同しない
- [ ] docs/process.md の文書カテゴリ定義と docs 配下の実ファイルが矛盾しない
- [ ] `_references/` が Normative を匂わせる表現を持たない（informative である旨が明確）
- [ ] Inventory（endpoints-*.md）が Fact のみで、判断文を含まない

---

Stage8 のゴールは、  
**設計・実装・文書のどれを見ても同じ判断に到達する状態を作ること**である。

このステージで行うのは、新しい設計判断ではない。  
すでに得られている判断を、

- 文書（TopSpec / Contracts / Process / Decisions / Inventory）
- 物理構成（`src/` 配下の層・フォルダ構造）
- 型構造（メソッドの in / out に現れる型制約）

のすべてで一致させ、**正本を確定すること**である。

Stage8 終了後は、  
「どこに何を書くか」「どこで判断するか」を考える必要がなくなっていなければならない。

---

## 現在ステータス（2026-02-13）

### 実施済み

- Phase8.x（429 / Timeout / Partial Failure 規約化）を実装まで完了
  - Normative 追加: `docs/contracts/resilience.md`
  - 監査更新: `docs/_references/resilience-audit.md`
  - 共通実装: `src/Transport/Policy/*` / `src/Transport/Protocol/*` を規約準拠へ更新
  - Contracts 追加: `BatchResult<TItem>` / `BatchError` / `BatchErrorKind`
  - テスト固定: `tests/Common.Tests/Transport/*` と `tests/Common.Tests/Contracts/BatchResultTests.cs`
- 文書導線の整備を実施
  - `README.md` に Resilience / Governance 導線を追加
  - `docs/process.md` の Normative 一覧と公式仕様記述を現状へ整合
  - `docs/contracts/overview.md` の API 命名記述を実装と整合
  - `docs/document-plan.md` を履歴資料（Archive）として明示

### 継続対象

- 各 `_references` 文書の stale 記述監視（実装変更時に随時更新）
- 新規取引所追加時の同規約適用確認（429 / Timeout / Partial Failure）

---

## Stage8 における達成目標（更新）

Stage8 では、次の状態を到達点とする。

- 設計上の判断は **TopSpec / Contracts のみ**に存在する
- 運用上の判断は **Process** に閉じている
- 原則からの逸脱は **Decisions（exceptions.md）** にのみ記録されている
- Inventory（endpoints / inventory-*）は **事実のみ**を保持している
- README は **判断を一切含まず、Docs 導線のみ**を示している

これにより、  
3 つ目以降の取引所追加は「設計の再検討」ではなく、  
**既存の正本に従った実装作業**として開始できる。

---

## Stage8 の非目的（明確化）

Stage8 は、以下を行わない。

- 新しい横断概念の設計
- 既存契約（Contracts）の意味拡張
- README による仕様説明
- 文書量の増加による安全性確保

安全性は、**文書量ではなく構造と正本性**によって確保する。

---

## 完了条件（Definition of Done：更新）

Stage8 は、以下が満たされたときに完了とする。

- 文書体系が論理的に閉じており、役割の重複がない
- 設計判断の所在が **文書名だけで一意に特定できる**
- 新規取引所追加時に「どの文書を更新するか」が自明
- 判断に迷ったとき、「どこにも書かない」という選択が正しく機能する

Stage8 の完了は、  
**設計を考えなくてよくなったこと**によってのみ確認される。
