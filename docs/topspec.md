# TopSpec — Exchange API Architecture Specification

## 1. 目的と設計原則

本仕様は、複数取引所 API を統一的に扱うための
**論理的に一貫した層構造と API 契約**を定義する。

本プロジェクトは以下を最優先とする。

* **論理性を最優先**する
* 揺らぎを排し、実装の自由度より **一貫性と検証可能性**を重視する
* 必要であれば **破壊的変更を容認**する
* 文書とコードの乖離を許容しない

---

## 2. 全体構造

本ライブラリは、以下の **4 層構造**を採用する。

```
Contract
  ↓
Normalized
  ↓
Raw
  ↓
Wire
```

### 2.1 依存関係

* 依存は **一方向のみ**とする
* 上位層は直下層のみを呼び出す
* 逆方向・横断参照は禁止する

---

## 3. 各層の責務

### 3.1 Wire 層（I/O / Transport）

**責務**

* 外部との **送受信を行う唯一の層**
* HTTP / WebSocket / 認証 / 署名 / 再送制御

**禁止事項（MUST NOT）**

* 意味解釈
* 値の正規化
* 単位変換
* 取引所仕様の吸収

**API 特性**

* I/O 実体を持つ
* Response は text 表現を返す

---

### 3.2 Raw 層（表現 / Primitive JSON）

**責務**

* 外部 API の JSON 表現差を吸収する
* プリミティブ JSON として表現を保持する

**許可事項（MAY）**

* string / number 混在の吸収
* null 許容
* フィールド名の写像
* RawJson の保持

**禁止事項（MUST NOT）**

* 意味の確定
* 単位変換
* 列挙値の解釈
* デフォルト補完

---

### 3.3 Normalized 層（取引所内正規化）

**責務**

* 取引所 **内部での意味を確定**する
* 型・単位・列挙値・時刻の正規化

**特性**

* 正規化は **取引所内に閉じる**
* 他取引所との統一は行わない

**重要**

* Normalized 層は **公開安定契約ではない**
* 外部利用者に対する互換性保証は行わない

---

### 3.4 Contract 層（取引所横断契約）

**責務**

* 取引所横断で通用する **公開契約**を定義する
* Capability / 共通 DTO / 共通エラーを提供する

**特性**

* 本ライブラリの **公開安定面**
* 取引所追加・仕様変更時も契約を守る責任を持つ

**方針**

* 無理な共通化は禁止
* 共通化できるもののみ Contract に含める
* 差異・欠損は Contract で明示的に表現する

---

## 4. API サーフェス規則

### 4.1 各層は API を呼び出せる形を持つ（MUST）

* Wire / Raw / Normalized / Contract の **全層に API I/F を持つ**
* 各層は直下層を呼び、変換のみを行う

### 4.2 I/O は Wire のみ（MUST）

* Raw / Normalized / Contract は I/O を行ってはならない
* 外部通信は必ず Wire 層を経由する

---

## 5. Call 抽象

### 5.1 Call の定義

本ライブラリの API は、**例外ではなく Call により結果を返す**。

```
Call<TRequest, TResponse>
```

Call は以下を表現する。

* 成功 / 失敗
* 対応する Request / Response
* 構造化された失敗理由

### 5.2 Call-only 規則（MUST）

* 各層の公開 API は **必ず Call を返す**
* 例外は以下の場合に限定する

  * プログラミングエラー
  * 設定不備
  * プロセス継続不能な内部不整合

---

## 6. Request / Response の扱い

### 6.1 Response（返り値）

**Response 表現は層ごとに固定する（MUST）**

| 層          | Response 表現    |
| ---------- | -------------- |
| Wire       | text           |
| Raw        | primitive JSON |
| Normalized | 正規化型           |
| Contract   | 契約型            |

---

### 6.2 Request（引数）

**Request 表現は Response と一致する必要はない（MAY）**

ただし以下を満たすこと（MUST）。

1. 層の責務を越えない
2. 直下層の Request へ **機械的に変換可能**
3. 意味判断を行わない

---

## 7. Request 変換規則

* 上位層 → 下位層の Request 変換は
  **EndpointId に基づく定義に従う**
* if / else による意味判断は禁止
* Wire は Raw Request を query / header / body(text) に
  **機械的に投影するのみ**

---

## 8. EndpointId 規約（統合）

### 8.1 定義

EndpointId は、**API の意味的単位を識別するための論理識別子**である。

* EndpointId は取引所内で一意でなければならない
* EndpointId は文字列値ではなく、
  **定数名 / enum 名 / 静的メンバ名として扱われる識別子**である
* EndpointId は Request / Response 構造や振る舞いを表現しない

### 8.2 責務と非責務

**責務**

* API endpoint を識別する
* 公式 API（Method / Path / Scope）との対応付けの軸となる

**非責務**

* Request / Response の構造や型
* paging / cursor / limit 等の挙動
* Capability 提供の可否
* 上位 API（Facade / Application 等）の存在

### 8.3 命名規則

* PascalCase を用いる
* 取引所固有用語に引きずられない
* HTTP Method や Path を含めない
* 冗長な接頭辞（Get / Fetch 等）を避ける

### 8.4 レイヤ別派生規則

* EndpointId は各層で 1:1 に対応する
* Raw / Normalized 層の API は `<EndpointId>CallAsync` を基本形とする
* EndpointId と API メソッド名は意味的に一致しなければならない

### 8.5 inventory との関係

* EndpointId の一覧は inventory 文書に列挙される
* inventory は **事実の一覧（Fact）**であり、規範ではない
* inventory に記載のない endpoint は未定義として扱う

### 8.6 取引所差異の扱い

* EndpointId は取引所間で共通である
* 取引所差異は以下に限定される

  * HTTP Method
  * Path
  * Query / Body 構造

* 差異は inventory に事実として記載する

### 8.7 禁止事項

* EndpointId に取引所名を含めること
* EndpointId に HTTP Method を含めること
* inventory のみを根拠に EndpointId を新設すること

---

## 9. エラーと失敗の扱い

* 通信失敗
* パース失敗
* 未対応機能
* 仕様差異

これらは **例外ではなく Call.Fail として返す**。

---

## 10. 公開範囲

* 外部利用者に対する公開安定 API は **Contract 層のみ**
* Raw / Normalized / Wire は内部または高度利用向けとする

---

## 11. 本仕様の位置づけ

本文書は、本リポジトリにおける **技術仕様および設計規範** の正本（Normative）である。

層構造、責務分離、API サーフェス規則、EndpointId を含む技術的判断については、
本文書に記載された内容が最優先される。

公開 API の契約については Contracts 文書を、
設計判断の裁定については Governance 文書を、それぞれ正本とする。

技術仕様・設計規範に関しては、
本文書以外の文書に記載された内容が本文書に優先することはない。
