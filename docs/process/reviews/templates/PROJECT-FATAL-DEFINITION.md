# PROJECT-LEVEL FATAL DEFINITION

本書は、ExchangeAPI における **Fatal（マージ不可）条件の横断定義**である。

すべてのレビューテンプレート（Boundary / Change / Consistency / Contracts / Docs / DX / Reliability / Security）は、本定義を上位基準として適用する。

---

# 1. Fatal の定義

Fatal とは、以下のいずれかに該当する場合を指す。

## F1. 境界破壊（Boundary Violation）

* 層ジャンプ
* 依存方向の逆流
* Core への Exchange 固有依存の混入
* 差異の閉じ込め破壊

## F2. SSOT 逸脱

* 正本（Normative / Governance / Inventory）と矛盾
* 正本に反映されていない実質的ルール追加
* Breaking Change 未記録

## F3. 公開契約破壊（Contracts Breakage）

* public surface の互換性破壊
* Try / OrThrow 規約破壊
* string 境界違反

## F4. Security 重大違反

* 署名仕様不一致
* Canonicalize 不整合
* secret（APIキー/署名素材）の露出
* nonce / timestamp 安全性破壊

## F5. Reliability 重大欠陥

* 再試行による重複実行リスク
* idempotency 不保証
* 失敗分類の崩壊

---

# 2. 運用ルール

* Fatal が 1 件でも検出された場合、PR はマージ不可
* 修正後、再レビュー必須
* Fatal 判定はテンプレ固有定義ではなく、本定義を参照する

---

# 3. テンプレ適用方法

各レビューテンプレートの「FatalClass」欄は、本定義の F1〜F5 のいずれかにマッピングすること。

例:

* Boundary の層ジャンプ → F1
* CHANGE 未記録 → F2
* secret 露出 → F4

---

# 4. 将来拡張

新たな Fatal 条件は、本書を更新することで横断適用される。

テンプレ個別で独自の Fatal 定義を追加してはならない。
