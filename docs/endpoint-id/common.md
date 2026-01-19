# EndpointId 規範（common）

## 1. 目的

本書は、本リポジトリにおける **EndpointId の定義と運用を共通規範として固定**し、
取引所間および実装間の揺らぎを排除することを目的とする。

本リポジトリでは **公式 API ドキュメントを仕様の正本**とし、本書および関連文書はそれを置き換えない。
本書で定義するのは、あくまで次の点に限定される。

* EndpointId という識別子の意味と責務
* EndpointId と公式 API（HTTP Method / Path）との対応関係
* EndpointId を列挙・管理するための最小ルール

---

## 2. EndpointId の位置付け（正本宣言）

* **EndpointId は、取引所ごとの API Endpoint を一意に識別するための ID である**

* EndpointId は **取引所内で一意でなければならない**

* EndpointId は **機械的に決定可能でなければならない**

* 上記を満たしたうえで、**可能な限り人間にとって分かりやすい識別子であること**を求める

* EndpointId は **文字列値そのものではなく、識別子（定数名・enum 名・静的メンバ名）として扱う**

* **EndpointId が正本であり、HTTP Method や Path は EndpointId から導出される**

EndpointId は、少なくとも以下を満たさなければならない。

* EndpointId から **HTTP Method** が一意に定まること
* EndpointId から **公式 API の Path** が一意に定まること
* 取引所内で EndpointId が衝突しないこと

---

## 3. EndpointId の責務と非責務

### 3.1 責務

EndpointId の責務は、**endpoint を識別し、公式 API 上の endpoint と対応付けること**に限定される。

### 3.2 非責務（保証しないもの）

EndpointId は以下を保証しない。

* Request / Response の構造や型
* paging / cursor / limit 等の振る舞い
* Capability として提供されるか否か
* 上位 API（Facade / Application）における統一インターフェースの存在

これらは、各層（Wire / Raw / Normalized / Contracts）および Capability 定義の責務である。

---

## 4. 共通ルールと取引所ルール

EndpointId は、**共通ルール**と**取引所ルール**の二層構造で定義する。

* 共通ルール：EndpointId を構成するための要素と形式を定める
* 取引所ルール：共通ルールで定義された要素を、どのように組み合わせるかを定める

この分離により、EndpointId を機械的に決定可能としつつ、
取引所ごとの差異を命名規則として明示化する。

---

## 5. 共通ルール（構成要素と形式）

### 5.1 構成要素

共通ルールでは、EndpointId を構成するために用いられる **要素** を定義する。

* **Path**：公式 API ドキュメントに記載された path
* **HTTP Method**：GET / POST / DELETE / etc
* **Scope**：Public / Private

### 5.2 表記制約

共通ルールでは、EndpointId の表記に関する最小限の制約を定める。

* EndpointId は **PascalCase** とする
* EndpointId には **スラッシュ（`/`）を含めない**

  * Path をそのまま文字列として埋め込むことはしない

### 5.3 一般単語境界（共通定義）

一般単語境界とは、英字↔数字の切替、英大文字↔小文字の切替、`-` / `_` 等の記号、
および **英字のみからなる連続列に対する英単語境界** を指す。

共通ルールは、**要素と形式を定義するのみであり、要素の並び順や省略規則は定義しない**。

---

## 6. 取引所ルール（組み立て規則）

取引所ルールでは、共通ルールで定義された要素（Path / Method / Scope）と形式を前提に、
**それらをどのように組み合わせて EndpointId を構成するか**を定める。

取引所ルールで定める事項には、少なくとも次を含む。

* 要素の並び順
* 要素の連結方法
* 要素の省略可否
* 衝突回避のための補助要素の扱い

これにより、

* EndpointId を機械的に決定可能とする
* 取引所内での命名の一貫性を保つ
* 取引所間の差異を命名規則として明確化する

ことを目的とする。

---

## 7. Endpoint 定義モデル（inventory）

inventory は、**endpoint の列挙情報と、その列挙を成立させる取引所ルールを併せて保持する文書**である。

EndpointId は、取引所別の inventory において、以下の情報と組で定義される。

### 7.1 Endpoint 列挙（必須・固定）

* EndpointId
* Method（HTTP Method）
* Path（公式 API の path）
* Scope（Public / Private）

この 4 項目の列挙のみが、本リポジトリにおける **endpoint 定義の正本**である。

### 7.2 取引所ルール（必須）

inventory には、その取引所における **EndpointId の組み立て規則（取引所ルール）**を必ず含める。

取引所ルールでは、共通ルールで定義された要素（Path / Method / Scope）と形式を前提に、
少なくとも以下を明示する。

* 要素の並び順
* 要素の連結方法
* 要素の省略規則
* 衝突回避のための補助要素の扱い

取引所ルールは、当該 inventory に記載された EndpointId が
**機械的に再構成可能であること**を保証しなければならない。

---

## 8. inventory の運用原則

* inventory は **取引所ごとに 1 ファイル**とする
* inventory には、**取引所ルールと endpoint 列挙の両方**を含める
* inventory に記載された EndpointId は、取引所ルールに基づき **機械的に再構成可能**でなければならない
* inventory には endpoint の列挙と規則のみを記載し、詳細仕様説明は含めない
* 詳細仕様（request / response / error 等）は **必ず公式 API ドキュメントを参照**する

inventory は、コード生成や実装検証の基点となり得るが、
本書では実装方式を規定しない。

---

## 9. 派生規則（将来方針）

EndpointId から以下を機械的に派生させることは、**将来の実装方針として想定**している。

* API メソッド名（例：`<EndpointId>CallAsync`）
* Wire / Raw / Normalized 各層での共通命名

ただし、**現時点では必須要件ではない**。

---

## 10. endpoint 追加・変更時の原則

* 新しい endpoint を扱う場合、**最初に inventory を更新**する
* inventory に記載されていない endpoint は、本リポジトリでは未定義とみなす
* 命名に迷いがある場合は、取引所ルールまたは Notes に裁定理由を残す
