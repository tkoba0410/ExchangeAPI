# REVIEW-TEMPLATES

本書は、すべてのレビューテンプレート（REVIEW-BOUNDARY / REVIEW-CHANGE / REVIEW-CONSISTENCY / REVIEW-CONTRACTS / REVIEW-DOCS / REVIEW-DX / REVIEW-RELIABILITY / REVIEW-SECURITY / REVIEW-USER-GUIDE / REVIEW-MULTI-PASS）に対する
**メタレビュー基準（品質統治基準）**である。

目的は、レビューテンプレート自体の品質を保証し、以下を防止すること。

* 抽象的で判定不能な観点
* 属人化されたレビュー
* SSOT逸脱
* CI化可能性の取りこぼし

---

# 0. 対象

* 対象テンプレート名:
* 改訂PR:
* 改訂概要:

---

# 1. 判定サマリ

| 観点 | 判定 (OK / 要修正 / NG) | Severity (Fatal/High/Medium/Low/Nit) | FatalClass (F1-F5/None) | 備考 |
| --- | --- | --- | --- | --- |
| 判定可能性 |  |  |  |  |
| 重大度定義 |  |  |  |  |
| SSOT整合 |  |  |  |  |
| レイヤ明確性 |  |  |  |  |
| CI化余地 |  |  |  |  |
| 再利用性 |  |  |  |  |

---

# 2. 観点別メタレビュー基準

## 2.1 判定可能性（最重要）

### 必須条件

* 各チェック項目に「判定基準」がある
* OK条件が明示されている
* NG条件が明示されている
* 不合格例（具体例）がある
* 抽象語のみ（例: 適切 / 妥当 / 維持 等）で終わっていない

### NG例

* "整合していること" のみで基準がない
* 判定不能な表現
* 不合格例が存在しない

---

## 2.2 重大度定義

### 必須条件

* 重大度が「優先度（Severity）」と「Fatal分類（FatalClass）」の2軸で明確に定義されている
* `Severity=Fatal` がマージ不可条件と連動している
* `Severity=Fatal` の指摘は `FatalClass=F1〜F5` が必須
* `Severity!=Fatal` の指摘は `FatalClass=None` を使用する

### 推奨定義

* Severity: `Fatal / High / Medium / Low / Nit`
* FatalClass: `F1 / F2 / F3 / F4 / F5 / None`
* 互換マッピング（旧語彙）: `Major -> High`, `Minor -> Medium`

---

## 2.3 SSOT整合

### 必須条件

* 観点ごとに根拠（Normative / Governance / Inventory / Decision）を示せる
* テンプレ内に独自ルールを増殖させていない

### NG例

* 正本に存在しない規約がテンプレ内で事実上のルール化
* docs/process と矛盾

---

## 2.4 レイヤ明確性

### 必須条件

* 対象層が明示されている
* 層ジャンプ検出観点がある
* 依存逆流検出観点がある
* Exchange差異閉じ込め観点がある

---

## 2.5 CI化余地

### 必須条件

* 各観点に「CI自動化可否」欄がある
* 自動化可能な観点が抽出されている

### NG例

* 人間依存のみの観点
* 将来的自動化を想定していない

---

## 2.6 再利用性（属人化防止）

### 必須条件

* 別PRでも同一基準で判定可能
* 自由記述依存が少ない
* 判例参照欄がある

### NG例

* 担当者の裁量で評価が変わる構造

---

# 3. テンプレ共通必須ブロック

すべてのレビューテンプレートは以下ブロックを含むべきである。

```
## 判定サマリ表
## 観点詳細（判定基準 / OK条件 / NG条件 / 不合格例 / 修正方針）
## 重大度明記（Severity / FatalClass）
## CI自動化候補
## 関連Normative / 判例
```

---

# 4. テンプレ破壊耐性確認

* 新しい取引所追加時にも有効か
* EndpointId増殖時にも機能するか
* Contracts拡張時にも粒度が維持されるか

---

# 5. 最終判定

* OK（構造問題なし）
* 要修正（軽微修正）
* NG（構造的欠陥あり）

---

# 運用方針

* テンプレ変更PRは必ず本 REVIEW-TEMPLATES を通す
* OK判定が出たテンプレのみ正本化可能
* 重大な観点は Normative への昇格を検討する
* CI化可能観点はテスト候補リストへ回収する
