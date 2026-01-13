# Contracts

## 1. Purpose

本ドキュメントは、ExchangeAPI における **公開契約（Contracts）** を定義する。

ここでいう契約とは、
- API の**振る舞い**ではなく
- 外部に公開される **型・形状・意味論** を指す。

本書の目的は、DTO や公開型の追加・変更時に  
**命名・粒度・責務の揺れを防ぐ**ことである。

---

## 2. Scope

本ドキュメントの対象は、以下に限定する。

- Public / Normalized API で公開される DTO
- ページング・カーソル・リミット等の共通契約
- 命名規約・型の粒度ルール
- Nullable / Optional の扱い方針

以下は対象外とする。

- API仕様（パラメータ・レスポンス詳細）
- Exchange 固有の生JSON構造
- 実装詳細・変換ロジック
- テスト観点

---

## 3. General Principles

### 3.1 Contracts are Shape, not Behavior

Contracts は **形状と意味論のみ**を定義する。

- 「どう処理するか」は定義しない
- 「どの型が、どの責務を持つか」だけを固定する

---

### 3.2 Stability over Completeness

Contracts は網羅性よりも安定性を優先する。

- 将来使うか不明な項目は追加しない
- 必要になった時点で最小限を追加する

---

### 3.3 No Exchange-Specific Leakage

Contracts に取引所固有の概念・型・命名を持ち込まない。

- Exchange 固有要素は Raw / Adapter 層で閉じる
- Normalized / Public Contracts は中立であること

---

## 4. Naming Conventions

### 4.1 DTO Naming

- 名詞 + Context（例: `OrderSnapshot`, `ExecutionHistoryItem`）
- 意味の異なる DTO を suffix だけで区別しない
- Raw / Normalized / Adapter の区別は namespace / フォルダで行う

---

### 4.2 Property Naming

- PascalCase
- 略語は一般的なもののみ使用する（Id, Url 等）
- Exchange 固有の語彙をそのまま転記しない

---

## 5. Nullable / Optional Policy

### 5.1 Nullable を許可する条件

以下の場合のみ Nullable を許可する。

- 公式API上、常に欠落する可能性がある
- Exchange によって意味が異なる
- 将来的な拡張余地として意図的に空を許容する

Nullable は **設計上の判断**であり、  
「たまたま無い」ことを理由にしてはならない。

---

### 5.2 Non-nullable の原則

- 常に存在する概念は Non-nullable とする
- 値が不明な場合は、別の状態・型で表現する

---

## 6. Page / Cursor / Limit Contracts

### 6.1 Page

- Page は「1回の取得結果」を表す
- 戻り値の件数と、要求した limit は区別される

### 6.2 Limit

- Limit は「要求」であり、保証ではない
- 実際に適用された limit は Meta 情報として保持する

### 6.3 Cursor

- Cursor はページング状態を表す opaque な値とする
- Cursor の内部構造や生成方法は契約に含めない

---

## 7. Error Representation (High-level)

Contracts では、エラーの **分類レベル**のみを扱う。

- 通信失敗
- 認証失敗
- 業務的失敗

エラーコードや詳細分類は、契約に含めない。

---

## 8. Evolution Rules

Contracts を変更する場合は、以下を満たすこと。

- 既存利用者の意味論を壊さない
- 破壊的変更は禁止とする
- 破壊的変更が必要な場合は、必ず `docs/exceptions.md` に記録する

---

## 9. Anti-Rules

以下は禁止とする。

- 公式API仕様の写経
- フィールド単位の詳細説明
- Exchange ごとの差分説明
- 実装都合による一時的 DTO の公開

---

## 10. Authority

本ドキュメントは、Contracts に関する判断において  
`docs/contracts/` 配下の正本である。

判断に迷った場合は、
- TopSpec Guide
- Documentation Policy

を参照し、それでも解決しない場合は  
Contracts を拡張しない選択を優先する。
