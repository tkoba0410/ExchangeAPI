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
* RawJson の保持・通過に関する境界

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

層を越えて公開される API は、**Call<T>** を返す。

* Response / Result の直返しは禁止する
* Transport 層は本ポリシーの例外とする

---

### 4.3 Type Safety at Boundaries

* **Contracts / Domain 側へ** string を直接受け渡さない
* 値オブジェクト・enum・専用型に変換してから上位層へ渡す

string の流入は **Entry Point のみ**に限定される。
ただし Wire は transport 層であり、Wire 境界の in/out は text（string/bytes）を許可する。

---

### 4.4 Data Shape by Layer

境界を越えて流れてよいデータ形状は、次に固定する。

- wire：text のみ
- raw：RawJson 鏡像（プリミティブDTO）
- normalized：正規化（enum/type DTO）
- contracts：取引所間抽象化（enum/type DTO）

---

## 5. Wire Layer Interfaces

### 5.1 Responsibilities

Wire 層は、**転送（transport）を成立させる**責務を持つ。

* HTTP/WS 等のリクエスト組み立て（method/path/query/header/body）
* 署名（認証）・送信・受信
* ステータスコード・ヘッダ等の transport 情報の保持

Wire は値の意味（妥当性）検証や正規化に関与してはならない。

---

### 5.1.1 Internal Structure: core.transport within Wire

Wire 層は、次の 2 つの要素から構成される。

- **共通 transport（`core.transport`）**：取引所概念を持たない。HTTP/WS 実行・共通の転送処理を担う。
- **exchange wire**：取引所固有。endpoint 組み立て・署名・送信パラメータ整形を担う。

依存方向は **exchange wire → core.transport** のみ許可する。
`core.transport` は exchange wire（取引所固有）を参照してはならない。

---

### 5.2 Interface Rules

* Wire の in/out は **text（string/bytes）** に限定する
* Wire は JSON を **パースしない**（DTO 化しない）
* Wire は Raw/Normalized/Contracts の DTO を返してはならない
  * DTO 化は Raw（鏡像）以降で行う

`core.transport` は次を禁止する。

- 取引所固有概念（product/symbol/order state 等）の導入
- JSON の解釈（パース、enum/type 化）
- Contracts/Domain 型の取り扱い

---

## 6. Raw Layer Interfaces

### 6.1 Responsibilities

Raw 層は、公式 API 仕様を **そのまま**扱う責務を持つ。

* 公式仕様に基づく **鏡像 DTO（Raw）** の保持
* JSON payload ⇄ 鏡像 DTO の相互変換（codec）
* RawJson の保持

Raw は HTTP endpoint と直接通信しない（transport は Wire の責務）。

---

### 6.2 Interface Rules

* 戻り値は Raw DTO に限定する（JsonElement は Raw 層内の処理に限る）
* RawJson は Raw 層内、または Normalized 変換直前までに閉じる
* Raw API は公開契約（Contracts）を直接返さない

RawJson 鏡像は、プリミティブDTO（RawValue）として扱う。
RawValue は次の閉じた集合に限定する。

- string
- bool
- long
- decimal
- null
- IReadOnlyList<RawValue>
- IReadOnlyDictionary<string, RawValue>

数値・日時・列挙などの意味論的な解釈（enum/type DTO 化）は Normalized 層が行う。

---

## 7. Adapter Layer Interfaces

### 7.1 Responsibilities

Adapter 層は、Raw 層と Normalized 層の **変換境界**である。

* Raw DTO → Normalized DTO への変換
* Exchange 固有差分の吸収

---

### 7.2 Interface Rules

* Adapter は Raw API を直接公開しない
* Adapter から RawJson を漏らさない
* Adapter は公開用 DTO を再定義しない

---

## 8. Normalized Layer Interfaces

### 8.1 Responsibilities

Normalized 層は、**単独取引所内**での意味確定（exchange semantics）を担う。

#### Meaning（この層が保証すること）

- Raw（鏡像）から **enum/type DTO** へ正規化する（意味論の確定）
- 数値/日時等の表現ゆらぎを意味として確定する（Try/OrThrow の方針に従う）
- 原則として lossless（必要なら退避領域を持つ）

#### Exchange-specific（取引所固有に属するもの）

- 取引所固有の状態体系・種別の解釈（注文状態、キャンセル理由など）
- 取引所固有フィールドの意味付け（公式仕様に依存する振る舞い）
- unknown 値の保持/エラー化などの適用（取引所内の都合）

#### Cross-exchange / Common（取引所横断に属するもの）

- 正規化の作法（Try/OrThrow、unknown を捨てない等）の共通方針
- 共通の基礎型ポリシー（例：Price/Size 等。置き場所が Contracts の場合は参照のみ）

Normalized 層は、Exchange 非依存の **意味論的 API** を提供する。

* 複数取引所で共通に扱える概念を提供する
* Exchange 固有差分を持ち込まない

---

### 8.2 Interface Rules

Normalized は **Contracts（横断抽象）を定義しない**。

- Normalized で作る enum/type は「当該取引所内で閉じる」
- 複数取引所で同一意味として扱える語彙が必要になった場合は Contracts に移す
- 取引所間で同一意味にできない差異は Normalized（取引所固有）に留める

* Normalized API は Raw / Adapter の存在を隠蔽する
* RawJson / JsonElement を公開しない
* Exchange 固有 enum / 型を公開しない

---

## 9. Public / Facade Interfaces

### 9.1 Responsibilities

Public / Facade 層は、利用者向けの **最終入口**である。

* Normalized API を集約・再編する
* 利用者に最小限の選択肢を提供する

---

### 9.2 Interface Rules

* Public API は安定性を最優先する
* 利用者に Raw / Adapter の概念を露出しない
* 破壊的変更は禁止とする

---

## 10. RawJson Handling Rules

* RawJson の保持は Raw / Normalized 内部に限定する
* Public / Contracts への RawJson 露出は禁止する
* 原則からの逸脱が必要な場合は、必ず `docs/exceptions.md` に記録する

---

## 11. Interface Evolution Rules

Interface を変更する場合は、以下を満たすこと。

* 境界の責務が明確である
* 上位層への影響を説明できる
* 原則からの逸脱がある場合、例外台帳に記録する

---

## 12. Authority

本ドキュメントは、層間インターフェースおよび境界判断において
`docs/boundaries.md` を正本とする。

判断に迷った場合は、

* TopSpec Guide
* Documentation Policy

を参照し、それでも解決しない場合は
**境界を越えない選択**を優先する。

---

## Appendix: Contracts Layer Clarification

### Meaning（Contracts が保証すること）

- **複数取引所で同じ意味になる部分だけ**を抽象化する（cross-exchange semantics）
- 公開契約として長期安定する語彙を提供する
- transport 文字列や RawJson を持たない

### Exchange-specific（取引所固有）

- 原則として **入れない**
- 必要になった場合は `docs/exceptions.md` に例外として記録し、最終的に解消（撤回）する

### Cross-exchange / Common（取引所横断）

- Contracts DTO 群（enum/type DTO）そのもの
- 横断の値オブジェクト群（Price/Size 等）

### Admission Rule（Contracts に入れる条件）

次の全条件を満たす場合のみ Contracts に追加してよい：

1) 2 つ以上の取引所で **同じ意味**にできる
2) その意味が将来も安定すると説明できる（破壊的変更を招きにくい）
3) 取引所固有の例外（exceptions）を増やさずに運用できる
