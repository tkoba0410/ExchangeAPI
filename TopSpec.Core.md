# TopSpec.Core（法文調版）

> **本書は、TopSpec における憲法（Core）とする。**
> **本書は、常に全文を前提として解釈されるものとし、部分的な読解を禁止する。**
> **本書に対する省略、要約、再解釈および暗黙の補完を、一切許可しない。**
> **補助文書（Guide / Ops 等）と本書との間に矛盾が生じた場合には、常に本書を優先するものとする。**

---

## 1. 決定事項および非決定事項

1. 本書は、境界、責務、依存方向および不変条件のみを決定するものとする。
2. 物理構成、運用方法および具体例については、本書では決定せず、補助文書に委ねるものとする。

---

## 2. 大原則

1. 公開境界は、これを明示しなければならない。
2. 層を跨いだ責務の混在を、これを禁止する。
3. 不変条件に対する変更は、すべて破壊的変更として扱うものとする。

---

## 3. FIX 宣言

1. 本書に定義されるすべての規則は、FIX とする。
2. FIX に違反する設計は、設計エラーとみなすものとする。

---

## 4. ゴールおよび非ゴール

### 4.1 ゴール

1. 第1の提供価値として、取引所ごとの API を前提に、
   Wire / Raw / Normalized（spec）を第一級の API として提供すること。
2. 第2の提供価値として、複数取引所間で抽象化可能な項目のみを
   Cross-Exchange（Domain: Contracts / Common）として定義し、
   その意味論を安定させること。
3. 公開入口（Factory）により、利用者が
   Exchange-Specific（取引所固有）と Cross-Exchange（抽象化）
   の利用意図を一意に選択できること。

### 4.2 非ゴール

1. 取引所 API 仕様の完全な写像（全 endpoint の網羅）。
2. 取引所間仕様差異の完全な抽象化。
3. spec（Wire / Raw / Normalized）を Cross-Exchange と同一水準で
   不変に保つこと。

---

## 5. 論理階層（定義）

本仕様における論理階層を、次のとおり定義する。

1. **Wire**：transport 仕様（spec）
2. **Raw**：API 鏡像 DTO（spec）
3. **Normalized**：正規化 DTO（spec）
4. **Adapter**：spec から domain への翻訳境界
5. **Contracts**：公開契約（domain の入口）
6. **Domain**：横断的なふるまい
7. **Composition**：供給および組立

### 規範

1. Raw および Normalized は、spec 層に属するものとする。
2. Adapter 以降は、domain 側に属するものとする。
3. 層を跨いだ越境を、これを禁止する。

### 依存方向（名前空間参照）規範

1. **下層は上層の名前空間を参照してはならない。**（例：Wire → Raw/Normalized/Contracts/Domain/Composition の参照禁止）
2. **許可される参照は「同一層」または「下層」への参照のみ**とする。
3. 本規範における「上層/下層」は、本節の論理階層（Wire → … → Composition）の順序に従う。

### 層の責務（精密化）

1. **Wire は JSON 文字列（またはバイト列）を transport payload として保持してよいが、DTO を保持してはならない。**
2. **Wire は転送（transport）成立のための情報のみを扱い、値の意味（妥当性）検証や正規化に関与してはならない。**
   * Wire が扱ってよいのは method/path/query/header/body といった転送表現であり、これらは原則として string/bytes で表現される。
   * 値の意味（例：product_code の形式、symbol の正当性、ID の構文）を保証する責務は、Raw/Normalized/Adapter のいずれかに属する。
3. **Raw は取引所公式 API の鏡像 DTO を保持するものとし、鏡像 DTO と JSON payload の相互変換（codec）責務を許可する。**
   * Raw における変換は、JSON 表現上の差（数値/文字列混在、null、日時表現等）を扱う **構文意味（syntax-level / primitive-level）** に限定される。
   * Raw は取引所の意味論（例：注文状態の解釈、銘柄名の正規化、手数料体系の前提）を担ってはならない。
   * **Raw における JsonConverter（codec）は、Deserialize（Read）専用とする。**
     * Serialize（Write）は Raw の責務ではなく、実装してはならない。
     * Raw が扱う Serialize 対象は、上位層（Normalized 等）で既に意味決定されたプリミティブ値とする。
   * **Raw が扱ってよい型は、プリミティブおよびコンテナに限定する（ホワイトリスト）。**
     * 許可：`string` / `bool` / 数値（`int` / `long` / `decimal` 等）/ `DateTimeOffset`（形式変換としてのみ）/ `TimeSpan`（必要時）/ `T?`（nullable）
     * 許可：`IReadOnlyList<T>` / `List<T>` / `T[]` / `IReadOnlyDictionary<string,T>`（動的応答が必要な場合のみ）
   * **Raw は次に掲げる型・表現を定義または公開してはならない（ブラックリスト）。**
     * `enum`（注文種別・売買区分・状態・type 等の意味型すべて）
     * `RawProductCode` / `RawSymbol` / `RawOrderId` 等、意味を持つラッパ型（名前付き string を含む）
     * Normalized / Contracts / Domain の型
   * **Raw の Request/DTO は「JSON ↔ プリミティブ」変換のための入出力に限定し、意味づけ（列挙・既定値注入・妥当性検証）を持ち込んではならない。**
4. **Normalized は単独取引所内での正規化 DTO を保持するものとし、当該取引所の意味論（exchange semantics）に基づく解釈・正規化を担う。**
   * Normalized は取引所間の抽象化を目的としてはならない。
   * **注文種別・売買区分・状態・type 等の意味づけ（列挙化）、既定値注入、妥当性検証は Normalized の責務とする。**
   * **Normalized は原則として情報欠落のない（lossless）正規化を行う。**
     * 正規化とは「意味づけ（解釈）と型付け」を指し、元データの削除を目的としない。
     * Raw に存在する情報は、Normalized DTO のいずれかに保持されなければならない（明示フィールド／退避領域）。
   * **lossless のために、Normalized DTO は Raw 由来の退避領域を持つことを推奨する。**
     * 推奨：`RawSnapshot`（Raw 応答 JSON のスナップショット）
     * 推奨：`Extras`（明示マップされないフィールドの保持）
     * 退避領域は JSON の表現を保持するために用いる。意味解釈は明示フィールド側で行う。
   * **Closed set（列挙）化に伴う未知値は、原則として捨てずに保持する。**
     * 推奨：`Known(enum)` / `Unknown(string raw)` のような表現で未知値を保持する。
     * 「未知値をエラーにする」場合でも、`CallError` に元の raw 値を含める。
   * **「意味が同一な表現差」の変換は欠落扱いにしない。**
     * 例：数値の JSON 表現差（string/number/指数表記）→ `decimal`、日時表現差（ISO/Unix 秒/ms）→ `DateTimeOffset`
5. **Contracts は複数取引所横断の抽象化 DTO を保持するものとし、横断的な意味論（cross-exchange semantics）を担う。**
   * Contracts は transport 情報および JSON 文字列を保持してはならない。

### lossless 正規化の定義

* **lossless（情報欠落なし）**とは、Raw が保持する情報が Normalized のいずれかに残ることを意味する。
  * ただし「意味が同一な表現差」の統一（例：日時/数値の形式統一）は欠落扱いにしない。
* Normalized から Contracts への抽象化は、横断語彙のために情報を落とし得る（これは仕様）。
  * その場合も、必要なら `CallMeta` 等のデバッグ情報に RawSnapshot を残す運用を許容する。

### Call（呼出）結果の標準形

1. **各層の公開 API（Facade / Api）の標準返り値は `Call<Req, Res>` とする。**
   * `Req` と `Res` は **当該層の意味段階**に属する型のみを用いる（下層の Req/Res を漏らさない）。
   * `Call` は `req`（要求）・`res`（応答）・`err`（失敗）を同一の枠で表現し、観測性と失敗分類を統一する。
2. **内部実装では `Res` のみ（Response-only）を返す補助関数を持ってよいが、公開 API の返り値は `Call<Req, Res>` を正とする。**
3. **Wire の `Req` は転送表現（method/path/query/header/bodyJson 等）であり、意味を持たない（string/bytes）。**
4. **Raw の `err` は構文意味（codec/converter）失敗を表現できなければならない。Normalized/Contracts の `err` は意味論（mapper/解釈）失敗を表現できなければならない。**

---

## 6. 公開入口（Factory）

1. 公開入口は、Factory に限定するものとする。
2. Factory は、利用意図を一意に表現しなければならない。

---

## 7. 公開契約（Contracts）

1. 公開 API は、Call と Outcome の関係を持つものとする。
2. Outcome は、Success または Failure のいずれか一方のみが成立する排他構造とする。

---

## 8. Contracts と Common の境界

1. Contracts は、公開契約における形（I/O 構造）を定義するものとする。
2. Common は、公開契約および実装において共有される語彙を定義するものとする。

---

## 11. 共通化対象

取引所横断で共通化する対象は、次に掲げるものに限定する。

1. 型
2. 値
3. ふるまい
4. 契約

---

## 12. 責務分離

1. spec 層は、domain を認識してはならない。
2. domain 層は、spec に依存してはならない。
3. 翻訳に関する責務は、Adapter にのみ存在するものとする。

---

## 13. 横断関心

横断的関心事は、Domain に属するものとする。

---

## 14. Raw / Exchange への明示的アクセス

Raw または Exchange への直接アクセスは、明示的な opt-in によってのみ許可されるものとする。

---

## 15. 依存方向

1. spec 層から domain 層への依存を禁止する。
2. domain 層から spec 層への依存を禁止する。
3. 次に掲げる依存関係のみを、例外的に許可する。

   1. Composition から Domain および Adapter への依存
   2. Adapter から spec への依存
   3. Domain から Common への依存

---

## 16. 不変条件

1. 公開契約の意味論は、後方互換でなければならない。
2. Success と Failure の区別を、これを破壊してはならない。

---

## 18. 禁止事項

次に掲げる行為を、これを禁止する。

1. 層を跨いだ DTO の再利用
2. 暗黙の変換
3. domain 内への transport 情報の流入

---

## 19. 一文要約

**spec と domain を分離し、公開契約を不変に保つものとする。**
