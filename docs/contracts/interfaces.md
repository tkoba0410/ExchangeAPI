# Interfaces

## 1. Purpose

本ドキュメントは、ExchangeAPI における **層間インターフェース（Interfaces）と責務境界** を定義する。

ここでいう Interface とは、

* 実装言語上の interface / public API
* 層を越えて公開される型・戻り値・責務

を指し、**どこまでが許され、どこからが越境か**を固定することを目的とする。

本書は「何ができるか」を列挙する文書ではなく、
**「何をしてはいけないか」を明確にするための文書**である。

---

## 2. Scope

本ドキュメントの対象は以下に限定する。

* Public / Normalized / Adapter / Raw 各層の公開インターフェース
* 層間で受け渡し可能な型の範囲
* RawJson の保持・通過が許される範囲

以下は対象外とする。

* 実装クラスの内部構造
* private / internal API
* メソッドの処理内容
* テスト専用 API

---

## 3. Layer Model

ExchangeAPI は、概ね以下の層構造を持つ。

* Public / Facade
* Normalized
* Adapter
* Raw
* Transport（対象外）

本ドキュメントでは、**Transport 層は設計判断の対象外**とする。

---

## 4. General Boundary Rules

### 4.1 Direction of Dependency

* 上位層は下位層にのみ依存する
* 下位層は上位層を参照してはならない

特に、Raw / Adapter 層から Normalized / Public 層への依存は禁止する。

---

### 4.2 Call-only Policy

層を越えて公開される API は、原則として **Call<T>** を返す。

* Response / Result の直返しは禁止する
* Transport 層は本ポリシーの例外とする

---

### 4.3 Type Safety at Boundaries

* 層境界で string を直接受け渡さない
* 値オブジェクト・enum・専用型に変換してから上位層へ渡す

string の流入は **Entry Point のみ**で許可される。

---

## 5. Raw Layer Interfaces

### 5.1 Responsibilities

Raw 層は、公式 API 仕様を **そのまま**扱う責務を持つ。

* HTTP endpoint との直接通信
* 公式仕様に基づく request / response の受信
* RawJson の保持

---

### 5.2 Interface Rules

* 戻り値は Raw DTO または JsonElement を含んでよい
* RawJson は Raw 層内、または Normalized 変換直前までに閉じる
* Raw API は公開契約（Contracts）を直接返さない

---

## 6. Adapter Layer Interfaces

### 6.1 Responsibilities

Adapter 層は、Raw 層と Normalized 層の **変換境界**である。

* Raw DTO → Normalized DTO への変換
* Exchange 固有差分の吸収

---

### 6.2 Interface Rules

* Adapter は Raw API を直接公開しない
* Adapter から RawJson を漏らさない
* Adapter は公開用 DTO を再定義しない

---

## 7. Normalized Layer Interfaces

### 7.1 Responsibilities

Normalized 層は、Exchange 非依存の **意味論的 API** を提供する。

* 複数取引所で共通に扱える概念を提供する
* Exchange 固有差分を持ち込まない

---

### 7.2 Interface Rules

* Normalized API は Raw / Adapter の存在を隠蔽する
* RawJson / JsonElement を公開しない
* Exchange 固有 enum / 型を公開しない

---

## 8. Public / Facade Interfaces

### 8.1 Responsibilities

Public / Facade 層は、利用者向けの **最終入口**である。

* Normalized API を集約・再編する
* 利用者に最小限の選択肢を提供する

---

### 8.2 Interface Rules

* Public API は安定性を最優先する
* 利用者に Raw / Adapter の概念を露出しない
* 破壊的変更は原則禁止とする

---

## 9. RawJson Handling Rules

* RawJson の保持は Raw / Normalized 内部に限定する
* Public / Contracts への RawJson 露出は禁止する
* lossless 目的の場合のみ、明示的に例外を認める

例外は必ず `docs/exceptions.md` に記録する。

---

## 10. Interface Evolution Rules

Interface を変更する場合は、以下を満たすこと。

* 境界の責務が明確である
* 上位層への影響を説明できる
* 原則からの逸脱がある場合、例外台帳に記録する

---

## 11. Authority

本ドキュメントは、層間インターフェースおよび境界判断において
`docs/contracts/` 配下の正本である。

判断に迷った場合は、

* TopSpec Guide
* Documentation Policy

を参照し、それでも解決しない場合は
**境界を越えない選択**を優先する。
