# TopSpec.Guide（Core 章対応版）

> **本書は TopSpec.Core を補足する唯一のガイド文書である。**
> **本書は FIX ではない。**
> **TopSpec.Core と矛盾する場合、常に TopSpec.Core を優先する。**

---

## 分離方針（運用規約）

* TopSpec.Core は **不変の憲法**であり、境界・責務・依存方向・不変条件のみを定義する。
* 本書（Guide）は Core を **理解・実装・運用するための補助文書**である。
* 本書が肥大化した場合、以下のいずれかに該当する内容は **独立文書として分離**する。

**分離トリガー**

1. 時間軸（判断履歴・過去事例・経緯）を持ち始めた
2. 手順・チェックリスト・運用方法が主体になった
3. 特定レイヤ（Adapter / Domain / Error 等）専用の内容になった
4. 章単位で Core 本文より長くなった

分離後、本書には **要約と参照（索引）** のみを残す。

---

## 0. 本書の位置づけと読み方（Core 前提）

* 本書は TopSpec.Core を前提として読む。
* 本書は Core の再解釈や上書きを行わない。
* 本書は「なぜそう決めたか」「どう誤用されやすいか」を記録する。

### 0.1 なぜ Core は短くあるべきか

* 憲法は短く、不変でなければ機能しない。
* 理由・背景・運用を含めた瞬間に、変更圧が Core に流入する。
* したがって Core は **規範のみ** を保持し、理解補助は必ず外部に置く。

Split Candidate: なし（常設）

---

## 1. 決定事項／非決定事項（Core §1 対応）

### 1.1 Core が決めること

* 境界・責務・依存方向・不変条件のみを決定する理由
* 仕様と実装の混線を防ぐための最小決定

### 1.2 Core が決めないこと

* 物理構成・運用・具体例を Core から排除した理由
* それらを Guide に置く必然性

Split Candidate: DecisionLog

---

## 2. 大原則の背景（Core §2 対応）

### 2.1 公開境界を明示する理由

* 境界が曖昧な API は破壊的変更を検知できない

### 2.2 層跨ぎ禁止の意味

* 層を跨ぐ責務混在が引き起こす破壊

### 2.3 不変条件を破壊的変更扱いとする理由

* 不変条件は利用者との契約そのものである

Split Candidate: DecisionLog

---

## 3. FIX 宣言の運用上の意味（Core §3 対応）

* なぜ Core の規則は FIX でなければならないか
* FIX を変更する場合に発生するコスト

Split Candidate: DecisionLog

---

## 4. ゴール／非ゴールの補足（Core §4 対応）

### 4.1 ゴールの解釈

* Domain 隔離が意味する設計上の利点

### 4.2 非ゴールの再確認

* 公式 API 仕様を写経しない理由

Split Candidate: なし

---

## 5. 論理階層の理解補足（Core §5 対応）

### 5.1 spec / domain 境界の思想

* Raw / Normalized を spec に留める理由

### 5.2 Adapter を翻訳関所とする理由

* なぜ判断を Adapter に持ち込まないか

### 5.3 Domain を肥大させない原則

* Domain が持つべき責務と持ってはならない責務

Split Candidate: AdapterNotes / DomainNotes

---

## 6. 公開入口（Factory）の設計意図（Core §6 対応）

* Factory を入口に限定する理由
* 利用意図を型で表現する設計思想

Split Candidate: なし

---

## 7. 公開契約（Contracts）の背景（Core §7–8 対応）

### 7.1 Call / Outcome 採用理由

* Request と結果を不可分に扱う理由

### 7.2 Contracts と Common を分ける理由

* 形と語彙を分離する設計判断

Split Candidate: ErrorDesign / ValueDesign

---

## 8. Cross-Exchange 共通化の思想（Core §11 対応）

### 8.1 なぜ共通化対象を 4 種に限定したか

* Interface / DTO / Type / Error 以外を共通化しない理由

Split Candidate: DecisionLog

---

## 9. 責務分離と横断関心（Core §12–13 対応）

* Contracts / Common / Domain の役割分離
* 横断関心を Domain に集約する理由

Split Candidate: DomainNotes

---

## 10. Raw / Exchange 明示アクセスの背景（Core §14 対応）

* opt-in を強制する理由
* 調査用途と業務用途の分離

Split Candidate: Operations

---

## 11. 依存方向の設計理由（Core §15 対応）

* なぜ依存方向を固定する必要があるか
* 境界破壊が起きる典型パターン

Split Candidate: AdapterNotes

---

## 12. 不変条件の補足解説（Core §16 対応）

* 後方互換を絶対条件とする理由
* Success / Failure 排他の意味

Split Candidate: ValueDesign

---

## 13. 禁止事項の背景（Core §18 対応）

* 各禁止事項が防いでいる事故

Split Candidate: AntiPatterns

---

## 14. 変更の扱い（運用補足）

* Core 変更を原則破壊的とみなす理由
* 変更理由・影響範囲を明示する運用

Split Candidate: DecisionLog

---

## 15. 物理構成と正本管理（Core 補完章）

* なぜ物理構成を規範にしないか
* 正本を公式 API 文書に限定した理由
* API 台帳の位置づけ

Split Candidate: PhysicalLayout

---

## 16. 分離された文書一覧（索引）

* TopSpec.DecisionLog.md : 設計判断の履歴
* TopSpec.AdapterNotes.md : Adapter 専用補足
* TopSpec.DomainNotes.md : Domain 設計補足
* TopSpec.PhysicalLayout.md : 物理構成詳細
* TopSpec.ErrorDesign.md : エラー設計詳細
* TopSpec.ValueDesign.md : Value 設計詳細
* TopSpec.Operations.md : 運用・CI
* TopSpec.AntiPatterns.md : 事故・誤用集

Split Candidate: なし（索引章として常設）
