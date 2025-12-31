# TopSpec.Guide

> **本書は TopSpec.Core を補足する唯一のガイド文書である。**  
> **本書は FIX ではない。**  
> **TopSpec.Core と矛盾する場合、常に TopSpec.Core を優先する。**

---

## 分離方針（運用規約）

- TopSpec.Core は **不変の憲法**であり、境界・責務・依存方向・不変条件のみを定義する。
- 本書（Guide）は Core を **理解・実装・運用するための補助文書**である。
- 本書が肥大化した場合、以下のいずれかに該当する内容は **独立文書として分離**する。

**分離トリガー**
1. 時間軸（判断履歴・過去事例・経緯）を持ち始めた
2. 手順・チェックリスト・運用方法が主体になった
3. 特定レイヤ（Adapter / Domain / Error 等）専用の内容になった
4. 章単位で Core 本文より長くなった

分離後、本書には **要約と参照（索引）** のみを残す。

---

## 0. 本書の位置づけと読み方

- 本書は TopSpec.Core を前提として読む。
- 本書は Core の再解釈や上書きを行わない。
- 本書は「なぜそう決めたか」「どう誤用されやすいか」を記録する。

Split Candidate: なし（常設）

---

## 1. TopSpec.Core の補足解釈指針（Why）

### 1.1 spec と domain の分離思想
- spec は事実の写像であり、判断を含めない。
- domain は意味と振る舞いを定義する。
- spec を解釈し始めた瞬間に層違反が起きる。

### 1.2 最小共通化への警戒
- 共通化は目的ではなく副作用である。
- 揃えた瞬間に将来の変更コストが確定する。

### 1.3 正本を公式 API 文書に限定した理由
- リポジトリ内仕様は必ず陳腐化する。
- 二重正本は必ず破綻する。

Split Candidate: DecisionLog（設計判断が時系列化した場合）

---

## 2. 論理構成と層設計の補足（How）

### 2.1 各層の理解補足
- Raw / Normalized は spec 層である。
- Adapter は翻訳関所であり判断を持たない。
- Domain は横断的ふるまいのみを持つ。

### 2.2 Adapter が壊れ始める兆候
- if(exchange) が増え始める。
- 再試行・分岐・ポリシー判断が入る。

Split Candidate: AdapterNotes / DomainNotes

---

## 3. 物理構成（フォルダ構成と正本管理）

### 3.1 推奨物理構成の考え方
- 物理構成は規範ではない。
- 論理境界は参照制約で担保される。

### 3.2 正本管理
- 正本は公式 API 文書のみとする。
- spec ディレクトリは鏡像仕様の置き場である。
- Raw/Samples はコピー可だが正本ではない。

### 3.3 API 台帳の位置づけ
- 台帳は導線・網羅性確認用資料である。
- 仕様判断の根拠として使用してはならない。

Split Candidate: PhysicalLayout（構成・運用詳細が増えた場合）

---

## 4. Call / Error / Value 設計の実務補足

### 4.1 Call / Outcome 採用理由
- Request と Outcome を不可分に扱うため。
- 文脈不足なレスポンスを補うため。

### 4.2 エラー設計の背景
- 利用者向け正規化と診断情報を分離する。
- Retryability は判断材料であり結論ではない。

### 4.3 数値・Value 設計
- string を Common に流さない。
- Try / OrThrow を併設する。

Split Candidate: ErrorDesign / ValueDesign

---

## 5. 運用・CI・破壊防止の知見

- 参照制約は CI でのみ長期的に守れる。
- 人手レビューは補助でしかない。

Split Candidate: Operations（手順・チェックリスト化した場合）

---

## 6. よくある誤解・事故・兆候（Anti-pattern）

- Adapter の肥大化
- Raw / Normalized DTO の漏洩
- Debug 用コードの本番残留

Split Candidate: AntiPatterns（事例が増えた場合）

---

## 7. 分離された文書一覧（索引）

- TopSpec.DecisionLog.md : 設計判断の履歴
- TopSpec.AdapterNotes.md : Adapter 専用補足
- TopSpec.DomainNotes.md : Domain 設計補足
- TopSpec.PhysicalLayout.md : 物理構成詳細
- TopSpec.ErrorDesign.md : エラー設計詳細
- TopSpec.ValueDesign.md : Value 設計詳細
- TopSpec.Operations.md : 運用・CI
- TopSpec.AntiPatterns.md : 事故・誤用集

Split Candidate: なし（索引章として常設）

