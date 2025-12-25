# 最終仕様書（最新版 / Final Top Specification）

本書は、本リポジトリにおける **外部契約（public contract）** と **責務境界** と **正本（source of truth）の所在** を固定する。
実装詳細・最適化・品質評価は対象外とする。

> 目的はただ一つ：
> **仕様と実装、論理と物理、取引所差分とドメインを混線させないこと**

---

## 0. 本書が決めること / 決めないこと

### 決めること

* 外部契約（public contract）の境界
* 不変条件（invariants）
* 層構造と依存方向
* 物理構成（フォルダ配置）
* 取引所横断（cross-exchange）の共通化対象
* Contracts / Common / Domain の責務分離

### 決めないこと

* 内部実装の最適解、性能方針
* アルゴリズム選択
* 個別取引所API仕様の全文記述

---

## 1. 大原則（絶対境界）

> **Wire DTO および Normalized DTO までは「仕様（spec）」であり、
> Adapter 以降は「ドメイン（domain）」である。**

この境界は **絶対** とし、越境を禁止する。

---

## 2. ゴール / 非ゴール

### 2.1 ゴール

* 日本国内の全取引所 API（Public / Private）への対応
* 海外主要取引所の Public API（Market Data 等）への対応
* 取引所 API を以下の層として整理し、仕様差分と責務を分離する

1. **Wire（Raw）層**（仕様）
2. **Normalized 層**（仕様）
3. **Adapter〜上位**（ドメイン）

### 2.2 非ゴール

* 取引所ごとの詳細仕様を本書に完全記述すること
* 意味的に一致しない概念を無理に統一すること
* 実装効率・性能・最適化手法そのものを仕様として固定すること

---

## 3. 論理階層（下層 → 上層）

### 3.1 Core（実行基盤）

* HTTP / Retry / Clock / Signer / Serializer など
* 取引所・ドメインの概念を一切持たない
* API を成立させる技術基盤

### 3.2 Wire 層（仕様）

* 取引所 API の通信表現そのもの
* 正本：**text.json（生レスポンス）**
* JSON形・フィールド名・欠損をそのまま保持

**変換手段：Converter**

* `text.json → wireDto`
* 意味判断は禁止

### 3.3 Normalized 層（仕様）

* 取引所内で一貫した表現に整理した DTO
* 命名・型・精度・時刻表現を統一

**変換手段：Mapper**

* `wireDto → normalizedDto`
* 意味判断は「取引所仕様の範囲」に限定

### 3.4 Adapter 層（境界 / 翻訳関所）

> **仕様（spec）と言語（domain）を翻訳する唯一の関所**

* 上位の利用契約（Contracts interface）を実装
* `normalizedDto → contractDto` を Mapper により変換
* 取引所差分をここで完全に吸収

### 3.5 Contracts（利用の契約 / ドメイン入口）

* 利用者・上位アプリが依存してよい唯一の契約
* interface と抽象 DTO（入出力）
* 取引所を一切知らない

### 3.6 Domain（複数取引所抽象化の振る舞い）

* 複数取引所を横断して扱うための主要ふるまい
* UseCase / Domain Service / Policy
* **入力は Contracts（Interface/DTO）と Common（語彙）に限定**

### 3.7 Composition（供給レイヤ）

* Core / Exchanges を組み立てて提供
* Factory / Options / Credential 注入
* ロジックは持たない（配線のみ）

---

## 3.8 公開エントリポイント（Factory）[確定]

Composition は、利用者が「どの層を使うか」を誤らないために、入口（Factory）を **3 系統に限定**する。

* **Wire（spec）入口**：`CreateRaw(...)` → Wire DTO を返す Raw API を生成
* **Normalized（spec）入口**：`CreateExchange(...)` → Normalized DTO を返す取引所固定 API を生成
* **Contracts（cross-exchange）入口**：`CreateClient(...)` → Contracts Interface/DTO を返すクライアントを生成

命名は上記を正とし、公開 API に `Adapter` の語を露出しない（内部層としては `Adapter` を保持してよい）。

### 3.8.1 `CreateExchange(...)` の用途（混乱防止条文）

`CreateExchange(...)` は **取引所を固定して使いたい利用者**、または **Adapter 実装・差分調査のために Normalized を直接扱いたい実装者**のための入口である。

* 原則：利用者のデフォルト入口は `CreateClient(...)` とする
* `CreateExchange(...)` を選ぶのは次の場合に限る

  * 取引所固有の仕様範囲でのユースケース（取引所固定のボット等）
  * Adapter 実装/デバッグで Normalized を確認したい

> `CreateExchange(...)` は「横断契約」ではなく「取引所固定の便利 API」である。

---

## 3.9 Contracts 公開面（クライアント集約）[確定]

Contracts は、横断利用における公開面を次の形で提供する。

* 基本の横断インタフェース群：

  * `IMarketDataApi`
  * `ITradingApi`
  * `IAccountApi`
  * `IExchangeInfoApi`（任意 / 実装可能な範囲で）

* 利用者の基本入口は **集約クライアント**とする：

  * `IExchangeClient`

    * `Market : IMarketDataApi`
    * `Trading : ITradingApi`
    * `Account : IAccountApi`
    * `Info : IExchangeInfoApi`

`CreateClient(...)` の返り値は `IExchangeClient` を正とする。

---

## 3.10 Contracts DTO と Common 語彙の境界（Step 4）[確定]

Contracts と Common の境界は、取引所横断（cross-exchange）の混線を防ぐために、次の規約で固定する。

### 3.10.1 Contracts（DTO：入出力の形）

* Contracts に置くのは **利用者に公開する I/O の形**に限定する。
* Interface の引数・戻り値に現れるデータ構造（Request/Response/DTO）は **必ず Contracts** に属する。
* Contracts DTO は取引所固有情報（取引所名、取引所固有フィールド、Wire/Normalized DTO）を含んではならない。

### 3.10.2 Common（語彙：値・分類・失敗・パース）

* Common に置くのは **複数 DTO / 複数 API で再利用される語彙**に限定する。
* Common は次のカテゴリで構成される。

  * **Values**：`Price`, `Size`, `Symbol`, `OrderId`, `Timestamp` 等
  * **Types/Enums**：`OrderSide`, `OrderType`, `TimeInForce`, `ExchangeCode` 等
  * **Errors**：`ErrorCode`, `ExchangeError`, `Retryability` 等
  * **Parsing**：Try/OrThrow 規約、例外型

### 3.10.3 DTO 内での Common 利用（推奨）

* Contracts DTO のフィールド型として Common の Value/Type/Error を利用してよい（推奨）。
* ただし **Common は Contracts DTO を参照してはならない**（依存方向は 9 章に従う）。

### 3.10.4 例外規約（enum / error の置き場）

* **DTO 専用 enum**（当該 DTO でしか使わない分類）は Contracts に置いてよい。
* エラーを DTO として返す場合：

  * エラー **DTO（形）** は Contracts
  * エラー **語彙（分類/扱い：ErrorCode 等）** は Common

### 3.10.5 Contracts DTO の粒度（最小共通の暴走防止条文）

Contracts DTO は「最小共通」であることを要するが、過度に痩せさせて利用性を損なってはならない。
次の原則で粒度を固定する。

* **必須**：横断ユースケースで頻出し、ほぼ全取引所で意味が一致するフィールドは含める
* **禁止**：取引所により意味が揺れる/欠損が常態のフィールドを必須化しない
* **手段**：差分が残る場合は次の順で扱う

  1. `Common` の語彙（Value/Type/Enum）で表現できるなら採用する
  2. Optional（nullable/Option）として保持し、必須化しない
  3. それでも意味が揺れる場合は Contracts に入れず、Raw/Exchange（opt-in）側で観測する

---

## 4. 物理構成（フォルダ構成）

本リポジトリでは **論理構成（責務境界）を最優先で固定** する。
その上で、取引所非依存の上層（Contracts / Common / Domain）は
**物理的には同一ルート配下にまとめてもよい**。

以下は推奨される物理構成の一例である。

```
src/
├─ Core/
│  ├─ Abstractions/
│  └─ Transport/
│
├─ Shared/                    # 取引所非依存の上層
│  ├─ Common/                 # 共通語彙（Value / Type / Error / Parsing）
│  ├─ Contracts/              # 利用契約（Interface / DTO）
│  └─ Domain/                 # 複数取引所横断のふるまい
│
├─ Exchanges/
│  ├─ AA/
│  │  ├─ Wire/
│  │  │  ├─ Samples/          # text.json（正本）
│  │  │  ├─ Converters/
│  │  │  └─ Dtos/
│  │  ├─ Normalize/
│  │  │  ├─ Dtos/
│  │  │  └─ Mappers/
│  │  └─ Adapter/
│  │     └─ Mappers/
│  └─ BB/
│     └─ （同構成）
│
└─ Composition/
   ├─ Factories/
   └─ Options/
```

---

## 5. 正本（source of truth）の所在

* 取引所固有の仕様：`doc-api` と `src/Exchanges/*`（Wire/Samples を含む）
* 通信・基盤の契約：`src/Core`
* 取引所横断の契約：`src/Contracts`
* 取引所横断の語彙：`src/Common`
* 複数取引所抽象化の振る舞い：`src/Domain`
* 組み立て・公開面：`src/Composition`

---

## 6. 取引所横断（Cross-Exchange）の共通化対象

> 取引所横断として共通化される対象は、以下 **4 種**に限定する。

1. **Interface**（操作の入口）
2. **DTO**（入出力の形）
3. **Type / Enum**（語彙・分類）
4. **Error**（失敗の契約）

これ以外（実装・正規化詳細・取引所固有仕様）は、取引所横断の対象としない。

---

## 7. Contracts / Common / Domain の責務分離（確定）

### 7.1 Contracts（Usage Contract）

* 役割：利用者が依存してよい唯一の契約面
* 含む：

  * Interface（例：`IMarketDataApi`, `ITradingApi`, `IAccountApi`）
  * DTO（例：`TickerDto`, `OrderDto`, `PlaceOrderRequest`）
* 禁止：

  * 取引所名・取引所固有概念の露出
  * Wire/Normalized DTO の混入

### 7.2 Common（共通語彙）

* 役割：契約と実装の双方から参照可能な横断語彙
* 含む：

  * Values：`Price`, `Size`, `Symbol`, `OrderId`, `Timestamp` 等
  * Types/Enums：`OrderSide`, `OrderType`, `TimeInForce`, `ExchangeCode` 等
  * Errors：`ErrorCode`, `ExchangeError`, `Retryability` 等
  * Parsing：Try/OrThrow 規約、例外型

### 7.3 Domain（横断ふるまい）

* 役割：複数取引所を横断して扱うユースケース・サービス
* 依存：

  * **Contracts**（Interface/DTO）
  * **Common**（Value/Type/Error）
* 禁止：

  * Exchanges への直接依存
  * 取引所固有 DTO（Wire/Normalized）への依存

---

## 8. 横断4種の Common / Contracts 割り当て（確定）

### 8.1 原則

* **Interface** → `Contracts`
* **DTO** → `Contracts`
* **Type / Enum** → `Common`（例外あり）
* **Error** → `Common`（例外あり）

### 8.2 例外

* DTO 専用の enum（その DTO でしか使わない分類）は `Contracts` に置いてよい
* エラーを DTO として返す場合：

  * エラー **DTO（形）** は `Contracts`
  * エラー **語彙（分類/扱い）** は `Common`

---

## 4.1 Shared 配下で境界を崩さないための補足条文（命名・参照ルール）

`src/Shared/` 配下に `Common / Contracts / Domain` を物理的に集約する場合でも、
境界は **責務** と **参照** により強制されなければならない。
本条はそのための最小ルールを定める。

### 4.1.1 名前空間（命名）

* `src/Shared/Common/**` は `ExchangeApi.Common.*`
* `src/Shared/Contracts/**` は `ExchangeApi.Contracts.*`
* `src/Shared/Domain/**` は `ExchangeApi.Domain.*`

> 物理階層が近いほど、名前空間は境界の代替となる。

### 4.1.2 参照禁止（compile-time での向き）

* `Common` は `Contracts` と `Domain` を参照してはならない
* `Contracts` は `Domain` を参照してはならない
* `Domain` は `Exchanges` を参照してはならない
* `Contracts/Common/Domain` は `Composition` を参照してはならない

（許可される参照は「依存方向（必須）」章に従う）

#### 自動検査（運用条文）

Shared 配下では物理距離が近いため、参照禁止は **自動検査**により担保することを推奨する。
少なくとも次のいずれかを導入し、CI で失敗させる。

* プロジェクト分割（`Common` / `Contracts` / `Domain` を別 csproj）による参照制約
* 参照禁止を検出する静的解析（analyzer / ルールベース検査）

> 人手レビューのみでの担保は、長期運用で破綻しやすい。

### 4.1.3 公開面の最小化（internal の活用）

* `Shared` 内部の実装詳細は原則 `internal` とし、公開面は最小にする
* `Contracts` の公開型は「利用契約」に必要なものに限定する
* `Common` の公開型は「語彙（Value/Type/Error/Parsing）」に限定する

### 4.1.4 ファイル/型配置の判定基準（迷ったら）

* 利用者が依存する呼び口（Interface）と、その入出力（DTO） → `Contracts`
* DTO/エラー等で再利用される語彙（Value/Type） → `Common`
* 複数取引所を横断するふるまい（UseCase/Service/Policy） → `Domain`

### 4.1.5 禁止パターン（Shared で起きやすい混線）

* `Common` に interface（呼び口）を置くこと
* `Contracts` に横断ふるまい（UseCase/Service）を置くこと
* `Domain` に取引所固有 DTO（Wire/Normalized）や取引所名を持ち込むこと

---

## 3.11 Raw / Exchange への明示的アクセス（Step 5）[確定]

利用者のデフォルト入口は `CreateClient(...) -> IExchangeClient` とし、
Wire（Raw）や Normalized（Exchange）へのアクセスは **デバッグ・調査用途に限り**、
明示的な操作として提供してよい。

### 3.11.1 目的

* 業務ロジックが Wire/Normalized に侵入することを防ぐ
* 必要なときだけ仕様観測・障害解析・差分調査を可能にする

### 3.11.2 提供形（明示的 opt-in）

* `IExchangeClient` は通常、Contracts（Market/Trading/Account/Info）だけを提供する
* Raw/Exchange へのアクセスを提供する場合は、次の **明示的インタフェース**を用いる

  * `IHasRawAccess`：`Raw`（Wire / Raw API）を露出
  * `IHasExchangeAccess`：`Exchange`（Normalized API）を露出

> 利用者はキャスト等により「覗く」意思を明示しなければならない。

### 3.11.3 禁止

* Contracts の戻り値型として Wire/Normalized DTO を返すこと
* Domain が Raw/Exchange を参照すること

---

## 9. 依存方向（必須）

### 9.1 許可

* `Domain -> Contracts`
* `Domain -> Common`
* `Contracts -> Common`
* `Exchanges(Adapter) -> Contracts, Common`
* `Exchanges(Wire/Normalize) -> Common`（必要な語彙のみ）
* `Composition -> *`（配線のみのため全参照可）

### 9.2 禁止

* `Contracts -> Domain`
* `Common -> Domain`
* `Common -> Exchanges`
* `Domain -> Exchanges`
* `Exchanges -> Exchanges`（取引所間依存）

---

## 3.12 Domain 公開形（Step 6）[確定]

Domain は「取引所横断の主要ふるまい」を提供する層であり、
**新たな利用契約（契約 interface / DTO）を定義しない**。

### 3.12.1 Domain が提供するもの

* UseCase（例：注文ポーリング、状態待機、共通の手順）
* Domain Service（例：横断ルーティング、ポリシー適用）
* Policy（例：再試行方針、タイムアウト戦略）

### 3.12.2 Domain の入力・出力

* 入力は **Contracts（Interface/DTO）と Common（語彙）に限定**する
* 出力は **Contracts DTO / Common Value / Common Error** に限定する

### 3.12.3 禁止

* Domain が Exchanges（取引所実装）を参照すること
* Domain が Wire/Normalized DTO を参照すること
* Domain が Composition（Factory/DI）を参照すること
* Domain が契約 interface / 契約 DTO を新規に定義すること

---

## 3.13 公開 API の命名規約（Raw / Exchange / Client / Facade）[確定]

公開 API の命名は、層の誤用と混線を防ぐために次で固定する。

### 3.13.1 Factory 名（入口）

* Wire（spec）：`CreateRaw(...)`
* Normalized（spec）：`CreateExchange(...)`
* Contracts（cross-exchange）：`CreateClient(...)`

### 3.13.2 返り値の型名（層が一目で分かること）

* Wire DTO：接頭辞 `Wire` を付ける（例：`WireTickerDto`）
* Normalized DTO：接尾辞 `Normalized` を付ける（例：`BitflyerTickerNormalized`）
* Contracts DTO：取引所名を含めない（例：`TickerDto`）

### 3.13.3 クライアント型名（利用者が迷わないこと）

* Contracts の基本入口は `IExchangeClient` とする（`CreateClient` の返り値）
* `IExchangeClient` の配下プロパティは API グループ名と一致させる

  * `Market`, `Trading`, `Account`, `Info`

### 3.13.4 API グループ名とメソッド名（全層で同名）

* グループ名（例：`Market.GetTickerAsync`）は Wire / Normalized / Contracts で揃える
* メソッド名は揃え、**層の違いは返却 DTO の型で表現する**

### 3.13.5 公開面に出してよい語彙

* 公開面（Contracts）に出してよいのは `Contracts` と `Common` のみ
* 公開面の名称に `Adapter` を含めない（Adapter は内部層として保持してよい）

---

## 10. 不変条件（Invariants）

* **Price/Size パース規約**

  * 共通層へ string を流さない（string は境界でのみ受理）
  * Try 系（非例外）を基本とし、OrThrow 系（例外）を必ず併設する
* **翻訳関所の一貫性**

  * `normalizedDto → contractDto` は Adapter（およびその配下 Mapper）のみが行う

---

## 11. Converter / Mapper 規約

### Converter

* 対象：Wire 層のみ
* JSON を仕様どおり読めるかを保証
* 失敗意味：取引所仕様不一致・破損

### Mapper

* 対象：Normalize / Adapter
* 意味的変換・正規化
* 失敗意味：解釈不能・前提違反

---

## 12. 禁止事項（破壊防止ルール）

* Wire/Normalized DTO を Contracts から返してはならない
* Contracts が取引所仕様を知ってはならない
* Exchanges 同士が依存してはならない
* Converter に意味判断を書いてはならない
* Mapper に JSON 構文処理を書いてはならない
* Adapter 以外で contractDto を生成してはならない

---

## 13. 一文要約

> **仕様は読む。意味は作る。**
> **Wire/Normalize は仕様、Adapter 以降はドメイン。**
> **取引所横断の共通化は Interface / DTO / Type / Error の4種に限定する。**

---

## 14. 憲法 FIX 宣言（変更ポリシー）【最終】

本書は、本リポジトリにおける **最上位仕様（憲法）** として扱う。

### 14.1 変更の扱い

* 本書の変更は、原則として **破壊的変更** とみなす
* 変更を行う場合は、必ず次を明記する

  * 変更対象の章番号
  * 変更理由（Why）
  * 影響範囲（Contracts / Common / Domain / Exchanges / Composition）

### 14.2 変更できないもの

次に挙げる事項は、本書において **固定（FIX）** とする。

* 層構造および責務境界（spec / domain）
* Cross-Exchange 共通化対象の範囲（4種限定）
* Contracts / Common / Domain の責務分離
* 依存方向および禁止事項
* 正本（source of truth）の所在
* 公開エントリポイント（Factory）と命名規約

### 14.3 下位仕様との関係

* 下位仕様（Contracts API 詳細、DTO フィールド定義、ErrorCode 一覧等）は改訂可能とする
* ただし、下位仕様は **本書の境界・不変条件に反してはならない**

---

> 本書を変更する前に、まず **「本当に憲法を変える必要があるか」** を問うこと。
> 多くの場合、答えは **下位仕様で解決できる**。
