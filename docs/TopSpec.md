# 最終仕様書（最新版 / Final Top Specification）

本書は、本リポジトリにおける **外部契約（public contract）** と **責務境界** と **正本（source of truth）の所在** を固定する。
実装詳細・最適化・品質評価は対象外とする。

> 目的はただ一つ：
> **仕様と実装、論理と物理、取引所差分とドメインを混線させないこと**

---

<!-- TOC -->
- [1. 本書が決めること / 決めないこと](#1-本書が決めること-決めないこと)
  - [1.1 決めること](#11-決めること)
  - [1.2 決めないこと](#12-決めないこと)
- [2. 大原則（絶対境界）](#2-大原則絶対境界)
- [3. 憲法 FIX 宣言（変更ポリシー）【最終】](#3-憲法-fix-宣言変更ポリシー最終)
  - [3.1 変更の扱い](#31-変更の扱い)
  - [3.2 変更できないもの](#32-変更できないもの)
  - [3.3 互換性および破壊的変更に関する補足規定](#33-互換性および破壊的変更に関する補足規定)
  - [3.4 下位仕様との関係](#34-下位仕様との関係)
- [4. ゴール / 非ゴール](#4-ゴール-非ゴール)
  - [4.1 ゴール](#41-ゴール)
  - [4.2 非ゴール](#42-非ゴール)
- [5. 論理階層（下層 → 上層）](#5-論理階層下層-上層)
  - [5.1 Core（実行基盤）](#51-core実行基盤)
  - [5.2 Raw 層（仕様 / 鏡像）](#52-raw-層仕様--鏡像)
  - [5.3 Normalized 層（仕様）](#53-normalized-層仕様)
  - [5.4 Adapter 層（境界 / 翻訳関所）](#54-adapter-層境界-翻訳関所)
  - [5.5 Contracts（利用の契約 / ドメイン入口）](#55-contracts利用の契約-ドメイン入口)
  - [5.6 Domain（複数取引所抽象化の振る舞い）](#56-domain複数取引所抽象化の振る舞い)
  - [5.7 Composition（供給レイヤ）](#57-composition供給レイヤ)
- [6. 公開エントリポイント（Factory）[確定]](#6-公開エントリポイントfactory確定)
  - [6.1 `CreateExchange(...)` の用途（混乱防止条文）](#61-createexchange-の用途混乱防止条文)
- [7. Contracts 公開面（クライアント集約）[確定]](#7-contracts-公開面クライアント集約確定)
- [8. Contracts DTO と Common 語彙の境界（旧 [Design-Step-04]）[確定]](#8-contracts-dto-と-common-語彙の境界旧-design-step-04確定)
  - [8.1 Contracts（DTO：入出力の形）](#81-contractsdto入出力の形)
  - [8.2 Common（語彙：値・分類・失敗・パース）](#82-common語彙値分類失敗パース)
  - [8.3 DTO 内での Common 利用（推奨）](#83-dto-内での-common-利用推奨)
  - [8.4 例外規約（enum / error の置き場）](#84-例外規約enum-error-の置き場)
  - [8.5 Contracts DTO の粒度（最小共通の暴走防止条文）](#85-contracts-dto-の粒度最小共通の暴走防止条文)
- [9. 物理構成（フォルダ構成）](#9-物理構成フォルダ構成)
- [10. 正本（source of truth）の所在](#10-正本source-of-truthの所在)
- [11. 取引所横断（Cross-Exchange）の共通化対象](#11-取引所横断cross-exchangeの共通化対象)
- [12. Contracts / Common / Domain の責務分離（確定）](#12-contracts-common-domain-の責務分離確定)
  - [12.1 Contracts（Usage Contract）](#121-contractsusage-contract)
  - [12.2 Common（共通語彙）](#122-common共通語彙)
  - [12.3 Domain（横断ふるまい）](#123-domain横断ふるまい)
- [13. 横断4種の Common / Contracts 割り当て（確定）](#13-横断4種の-common-contracts-割り当て確定)
  - [13.1 原則](#131-原則)
  - [13.2 例外](#132-例外)
  - [13.3 Shared 配下で境界を崩さないための補足条文（命名・参照ルール）](#133-shared-配下で境界を崩さないための補足条文命名参照ルール)
    - [13.3.1 名前空間（命名）](#1331-名前空間命名)
    - [13.3.2 参照禁止（compile-time での向き）](#1332-参照禁止compile-time-での向き)
      - [13.3.2.1 自動検査（運用条文）](#13321-自動検査運用条文)
    - [13.3.3 公開面の最小化（internal の活用）](#1333-公開面の最小化internal-の活用)
    - [13.3.4 ファイル/型配置の判定基準（迷ったら）](#1334-ファイル型配置の判定基準迷ったら)
    - [13.3.5 禁止パターン（Shared で起きやすい混線）](#1335-禁止パターンshared-で起きやすい混線)
- [14. Raw / Exchange への明示的アクセス（旧 [Design-Step-05]）[確定]](#14-raw-exchange-への明示的アクセス旧-design-step-05確定)
  - [14.1 目的](#141-目的)
  - [14.2 提供形（明示的 opt-in）](#142-提供形明示的-opt-in)
  - [14.3 禁止](#143-禁止)
- [15. 依存方向（必須）](#15-依存方向必須)
  - [15.1 許可](#151-許可)
  - [15.2 禁止](#152-禁止)
  - [15.3 Domain 公開形（旧 [Design-Step-06]）[確定]](#153-domain-公開形旧-design-step-06確定)
    - [15.3.1 Domain が提供するもの](#1531-domain-が提供するもの)
    - [15.3.2 Domain の入力・出力](#1532-domain-の入力出力)
    - [15.3.3 禁止](#1533-禁止)
  - [15.4 公開 API の命名規約（Raw / Exchange / Client / Facade）[確定]](#154-公開-api-の命名規約raw-exchange-client-facade確定)
    - [15.4.1 Factory 名（入口）](#1541-factory-名入口)
    - [15.4.2 返り値の型名（層が一目で分かること）](#1542-返り値の型名層が一目で分かること)
    - [15.4.3 クライアント型名（利用者が迷わないこと）](#1543-クライアント型名利用者が迷わないこと)
    - [15.4.4 API グループ名とメソッド名（全層で同名）](#1544-api-グループ名とメソッド名全層で同名)
    - [15.4.5 公開面に出してよい語彙](#1545-公開面に出してよい語彙)
- [16. 不変条件（Invariants）](#16-不変条件invariants)
  - [16.1 数値・時刻・識別子に関する補足不変条件](#161-数値時刻識別子に関する補足不変条件)
- [17. Converter / Mapper 規約](#17-converter-mapper-規約)
  - [17.1 Converter](#171-converter)
  - [17.2 Mapper](#172-mapper)
  - [17.3 サンプル JSON を正本とする運用規約](#173-サンプル-json-を正本とする運用規約)
- [18. 禁止事項（破壊防止ルール）](#18-禁止事項破壊防止ルール)
- [19. 一文要約](#19-一文要約)
- [20. 運用および拡張に関する附則](#20-運用および拡張に関する附則)
  - [20.1 エラー正規化方針](#201-エラー正規化方針)
  - [20.2 ページング、レート制限およびキャンセル](#202-ページングレート制限およびキャンセル)
<!-- /TOC -->


## 1. 本書が決めること / 決めないこと

### 1.1 決めること

* 外部契約（public contract）の境界
* 不変条件（invariants）
* 層構造と依存方向
* 物理構成（フォルダ配置）
* 取引所横断（cross-exchange）の共通化対象
* Contracts / Common / Domain の責務分離

### 1.2 決めないこと

* 内部実装の最適解、性能方針
* アルゴリズム選択
* 個別取引所API仕様の全文記述

---

## 2. 大原則（絶対境界）

> **Raw DTO および Normalized DTO までは「仕様（spec）」であり、
> Adapter 以降は「ドメイン（domain）」である。**

この境界は **絶対** とし、越境を禁止する。

---

## 3. 憲法 FIX 宣言（変更ポリシー）【最終】

本書は、本リポジトリにおける **最上位仕様（憲法）** として扱う。

### 3.1 変更の扱い

* 本書の変更は、原則として **破壊的変更** とみなす
* 変更を行う場合は、必ず次を明記する

  * 変更対象の章番号
  * 変更理由（Why）
  * 影響範囲（Contracts / Common / Domain / Exchanges / Composition）

### 3.2 変更できないもの

次に挙げる事項は、本書において **固定（FIX）** とする。

* 層構造および責務境界（spec / domain）
* Cross-Exchange 共通化対象の範囲（4種限定）
* Contracts / Common / Domain の責務分離
* 依存方向および禁止事項
* 正本（source of truth）の所在
* 公開エントリポイント（Factory）と命名規約

### 3.3 互換性および破壊的変更に関する補足規定

本仕様における変更の互換性について、以下を補足規定として定める。

1. Contracts 層における公開インターフェースの変更は、本仕様における互換性判断の基準点とする。

2. 次に該当する変更は、破壊的変更とみなす。

   * 公開インターフェースのシグネチャ変更
   * 必須フィールドの追加または削除
   * 既存フィールドの意味変更
   * 列挙型における既存値の意味変更または削除

3. 次に該当する変更は、非破壊的変更とみなす。

   * Optional フィールドの追加
   * ErrorCode の追加（既存の意味を変更しない場合）
   * Raw 層または Normalized 層におけるフィールド追加

4. Contracts 層に影響する変更には、変更理由および影響範囲を明示した変更履歴を付与するものとする。

### 3.4 下位仕様との関係

* 下位仕様（Contracts API 詳細、DTO フィールド定義、ErrorCode 一覧等）は改訂可能とする
* ただし、下位仕様は **本書の境界・不変条件に反してはならない**

---

> 本書を変更する前に、まず **「本当に憲法を変える必要があるか」** を問うこと。
> 多くの場合、答えは **下位仕様で解決できる**。

## 4. ゴール / 非ゴール

### 4.1 ゴール

* 日本国内の全取引所 API（Public / Private）への対応
* 海外主要取引所の Public API（Market Data 等）への対応
* 取引所 API を以下の層として整理し、仕様差分と責務を分離する

1. **Raw 層（鏡像 spec）**（仕様）
2. **Normalized 層**（仕様）
3. **Adapter〜上位**（ドメイン）

### 4.2 非ゴール

* 取引所ごとの詳細仕様を本書に完全記述すること
* 意味的に一致しない概念を無理に統一すること
* 実装効率・性能・最適化手法そのものを仕様として固定すること

---

## 5. 論理階層（下層 → 上層）

### 5.1 Core（実行基盤）

* HTTP / Retry / Clock / Signer / Serializer など
* 取引所・ドメインの概念を一切持たない
* API を成立させる技術基盤

### 5.2 Raw 層（仕様 / 鏡像）

* 取引所 API の通信表現そのもの（鏡像）
* 正本：**text.json（生レスポンス）**
* JSON形・フィールド名・欠損をそのまま保持（意味判断は禁止）

**変換手段：Converter**（＝JSONを読めることの保証）

* `text.json → rawDto`
* 意味判断は禁止

### 5.3 Normalized 層（仕様）

* 取引所内で一貫した表現に整理した DTO
* 命名・型・精度・時刻表現を統一

**変換手段：Mapper**

* `rawDto → normalizedDto`
* 意味判断は「取引所仕様の範囲」に限定

### 5.4 Adapter 層（境界 / 翻訳関所）

> **仕様（spec）と言語（domain）を翻訳する唯一の関所**

* 上位の利用契約（Contracts interface）を実装
* `normalizedDto → contractDto` を Mapper により変換
* 取引所差分をここで完全に吸収

### 5.5 Contracts（利用の契約 / ドメイン入口）

* 利用者・上位アプリが依存してよい唯一の契約
* interface と抽象 DTO（入出力）
* 取引所を一切知らない

### 5.6 Domain（複数取引所抽象化の振る舞い）

* 複数取引所を横断して扱うための主要ふるまい
* UseCase / Domain Service / Policy
* **入力は Contracts（Interface/DTO）と Common（語彙）に限定**

### 5.7 Composition（供給レイヤ）

* Core / Exchanges を組み立てて提供
* Factory / Options / Credential 注入
* ロジックは持たない（配線のみ）

---

## 6. 公開エントリポイント（Factory）[確定]

Composition は、利用者が「どの層を使うか」を誤らないために、入口（Factory）を **3 系統に限定**する。

* **Raw（spec）入口**：`CreateRaw(...)` → Raw DTO を返す Raw API（鏡像）を生成
* **Normalized（spec）入口**：`CreateExchange(...)` → Normalized DTO を返す取引所固定 API を生成
* **Contracts（cross-exchange）入口**：`CreateClient(...)` → Contracts Interface/DTO を返すクライアントを生成

命名は上記を正とし、公開 API に `Adapter` の語を露出しない（内部層としては `Adapter` を保持してよい）。

### 6.1 `CreateExchange(...)` の用途（混乱防止条文）

`CreateExchange(...)` は **取引所を固定して使いたい利用者**、または **Adapter 実装・差分調査のために Normalized を直接扱いたい実装者**のための入口である。

* 原則：利用者のデフォルト入口は `CreateClient(...)` とする
* `CreateExchange(...)` を選ぶのは次の場合に限る

  * 取引所固有の仕様範囲でのユースケース（取引所固定のボット等）
  * Adapter 実装/デバッグで Normalized を確認したい

> `CreateExchange(...)` は「横断契約」ではなく「取引所固定の便利 API」である。

---

## 7. Contracts 公開面（クライアント集約）[確定]

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

## 8. Contracts DTO と Common 語彙の境界（旧 [Design-Step-04]）[確定]

Contracts と Common の境界は、取引所横断（cross-exchange）の混線を防ぐために、次の規約で固定する。

### 8.1 Contracts（DTO：入出力の形）

* Contracts に置くのは **利用者に公開する I/O の形**に限定する。
* Interface の引数・戻り値に現れるデータ構造（Request/Response/DTO）は **必ず Contracts** に属する。
* Contracts DTO は取引所固有情報（取引所名、取引所固有フィールド、Raw/Normalized DTO）を含んではならない。

### 8.2 Common（語彙：値・分類・失敗・パース）

* Common に置くのは **複数 DTO / 複数 API で再利用される語彙**に限定する。
* Common は次のカテゴリで構成される。

  * **Values**：`Price`, `Size`, `Symbol`, `OrderId`, `Timestamp` 等
  * **Types/Enums**：`OrderSide`, `OrderType`, `TimeInForce`, `ExchangeCode` 等
  * **Errors**：`ErrorCode`, `ExchangeError`, `Retryability` 等
  * **Parsing**：Try/OrThrow 規約、例外型

### 8.3 DTO 内での Common 利用（推奨）

* Contracts DTO のフィールド型として Common の Value/Type/Error を利用してよい（推奨）。
* ただし **Common は Contracts DTO を参照してはならない**（依存方向は 15 章に従う）。

### 8.4 例外規約（enum / error の置き場）

* **DTO 専用 enum**（当該 DTO でしか使わない分類）は Contracts に置いてよい。
* エラーを DTO として返す場合：

  * エラー **DTO（形）** は Contracts
  * エラー **語彙（分類/扱い：ErrorCode 等）** は Common

### 8.5 Contracts DTO の粒度（最小共通の暴走防止条文）

Contracts DTO は「最小共通」であることを要するが、過度に痩せさせて利用性を損なってはならない。
次の原則で粒度を固定する。

* **必須**：横断ユースケースで頻出し、ほぼ全取引所で意味が一致するフィールドは含める
* **禁止**：取引所により意味が揺れる/欠損が常態のフィールドを必須化しない
* **手段**：差分が残る場合は次の順で扱う

  1. `Common` の語彙（Value/Type/Enum）で表現できるなら採用する
  2. Optional（nullable/Option）として保持し、必須化しない
  3. それでも意味が揺れる場合は Contracts に入れず、Raw/Exchange（opt-in）側で観測する

---

## 9. 物理構成（フォルダ構成）

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
│  │  ├─ Raw/
│  │  │  ├─ Samples/          # Raw サンプルJSON（鏡像 / 正本）
│  │  │  │  ├─ Market/
│  │  │  │  │  └─ GetTicker.json
│  │  │  │  └─ Trading/
│  │  │  │     └─ SendChildOrder.json
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

## 10. 正本（source of truth）の所在

* 取引所固有の仕様：`doc-api` と `src/Exchanges/*`（Raw/Samples を含む）
  * Raw サンプル JSON（鏡像 / fact の正本）：`src/Exchanges/<Exchange>/Raw/Samples/<Group>/<Endpoint>.json`
* 通信・基盤の契約：`src/Core`
* 取引所横断の契約：`src/Contracts`
* 取引所横断の語彙：`src/Common`
* 複数取引所抽象化の振る舞い：`src/Domain`
* 組み立て・公開面：`src/Composition`

---

## 11. 取引所横断（Cross-Exchange）の共通化対象

> 取引所横断として共通化される対象は、以下 **4 種**に限定する。

1. **Interface**（操作の入口）
2. **DTO**（入出力の形）
3. **Type / Enum**（語彙・分類）
4. **Error**（失敗の契約）

これ以外（実装・正規化詳細・取引所固有仕様）は、取引所横断の対象としない。

---

## 12. Contracts / Common / Domain の責務分離（確定）

### 12.1 Contracts（Usage Contract）

* 役割：利用者が依存してよい唯一の契約面
* 含む：

  * Interface（例：`IMarketDataApi`, `ITradingApi`, `IAccountApi`）
  * DTO（例：`TickerDto`, `OrderDto`, `PlaceOrderRequest`）
* 禁止：

* 取引所名・取引所固有概念の露出
* Raw/Normalized DTO の混入

### 12.2 Common（共通語彙）

* 役割：契約と実装の双方から参照可能な横断語彙
* 含む：

  * Values：`Price`, `Size`, `Symbol`, `OrderId`, `Timestamp` 等
  * Types/Enums：`OrderSide`, `OrderType`, `TimeInForce`, `ExchangeCode` 等
  * Errors：`ErrorCode`, `ExchangeError`, `Retryability` 等
  * Parsing：Try/OrThrow 規約、例外型

### 12.3 Domain（横断ふるまい）

* 役割：複数取引所を横断して扱うユースケース・サービス
* 依存：

  * **Contracts**（Interface/DTO）
  * **Common**（Value/Type/Error）
* 禁止：

  * Exchanges への直接依存
* 取引所固有 DTO（Raw/Normalized）への依存

---

## 13. 横断4種の Common / Contracts 割り当て（確定）

### 13.1 原則

* **Interface** → `Contracts`
* **DTO** → `Contracts`
* **Type / Enum** → `Common`（例外あり）
* **Error** → `Common`（例外あり）

### 13.2 例外

* DTO 専用の enum（その DTO でしか使わない分類）は `Contracts` に置いてよい
* エラーを DTO として返す場合：

  * エラー **DTO（形）** は `Contracts`
  * エラー **語彙（分類/扱い）** は `Common`

---


### 13.3 Shared 配下で境界を崩さないための補足条文（命名・参照ルール）

`src/Shared/` 配下に `Common / Contracts / Domain` を物理的に集約する場合でも、
境界は **責務** と **参照** により強制されなければならない。
本条はそのための最小ルールを定める。

#### 13.3.1 名前空間（命名）

* `src/Shared/Common/**` は `ExchangeApi.Common.*`
* `src/Shared/Contracts/**` は `ExchangeApi.Contracts.*`
* `src/Shared/Domain/**` は `ExchangeApi.Domain.*`

> 物理階層が近いほど、名前空間は境界の代替となる。

#### 13.3.2 参照禁止（compile-time での向き）

* `Common` は `Contracts` と `Domain` を参照してはならない
* `Contracts` は `Domain` を参照してはならない
* `Domain` は `Exchanges` を参照してはならない
* `Contracts/Common/Domain` は `Composition` を参照してはならない

（許可される参照は「依存方向（必須）」章に従う）

##### 13.3.2.1 自動検査（運用条文）

Shared 配下では物理距離が近いため、参照禁止は **自動検査**により担保することを推奨する。
少なくとも次のいずれかを導入し、CI で失敗させる。

* プロジェクト分割（`Common` / `Contracts` / `Domain` を別 csproj）による参照制約
* 参照禁止を検出する静的解析（analyzer / ルールベース検査）

> 人手レビューのみでの担保は、長期運用で破綻しやすい。

#### 13.3.3 公開面の最小化（internal の活用）

* `Shared` 内部の実装詳細は原則 `internal` とし、公開面は最小にする
* `Contracts` の公開型は「利用契約」に必要なものに限定する
* `Common` の公開型は「語彙（Value/Type/Error/Parsing）」に限定する

#### 13.3.4 ファイル/型配置の判定基準（迷ったら）

* 利用者が依存する呼び口（Interface）と、その入出力（DTO） → `Contracts`
* DTO/エラー等で再利用される語彙（Value/Type） → `Common`
* 複数取引所を横断するふるまい（UseCase/Service/Policy） → `Domain`

#### 13.3.5 禁止パターン（Shared で起きやすい混線）

* `Common` に interface（呼び口）を置くこと
* `Contracts` に横断ふるまい（UseCase/Service）を置くこと
* `Domain` に取引所固有 DTO（Raw/Normalized）や取引所名を持ち込むこと

---

## 14. Raw / Exchange への明示的アクセス（旧 [Design-Step-05]）[確定]

利用者のデフォルト入口は `CreateClient(...) -> IExchangeClient` とし、
Raw（鏡像 spec）や Normalized（Exchange）へのアクセスは **デバッグ・調査用途に限り**、
明示的な操作として提供してよい。

### 14.1 目的

* 業務ロジックが Raw/Normalized に侵入することを防ぐ
* 必要なときだけ仕様観測・障害解析・差分調査を可能にする

### 14.2 提供形（明示的 opt-in）

* `IExchangeClient` は通常、Contracts（Market/Trading/Account/Info）だけを提供する
* Raw/Exchange へのアクセスを提供する場合は、次の **明示的インタフェース**を用いる

  * `IHasRawAccess`：`Raw`（Raw API / Raw DTO）を露出
  * `IHasExchangeAccess`：`Exchange`（Normalized API）を露出

> 利用者はキャスト等により「覗く」意思を明示しなければならない。

### 14.3 禁止

* Contracts の戻り値型として Raw/Normalized DTO を返すこと
* Domain が Raw/Exchange を参照すること

---

## 15. 依存方向（必須）

### 15.1 許可

* `Domain -> Contracts`
* `Domain -> Common`
* `Contracts -> Common`
* `Exchanges(Adapter) -> Contracts, Common`
* `Exchanges(Raw/Normalize) -> Common`（必要な語彙のみ）
* `Composition -> *`（配線のみのため全参照可）

### 15.2 禁止

* `Contracts -> Domain`
* `Common -> Domain`
* `Common -> Exchanges`
* `Domain -> Exchanges`
* `Exchanges -> Exchanges`（取引所間依存）

---


### 15.3 Domain 公開形（旧 [Design-Step-06]）[確定]

Domain は「取引所横断の主要ふるまい」を提供する層であり、
**新たな利用契約（契約 interface / DTO）を定義しない**。

#### 15.3.1 Domain が提供するもの

* UseCase（例：注文ポーリング、状態待機、共通の手順）
* Domain Service（例：横断ルーティング、ポリシー適用）
* Policy（例：再試行方針、タイムアウト戦略）

#### 15.3.2 Domain の入力・出力

* 入力は **Contracts（Interface/DTO）と Common（語彙）に限定**する
* 出力は **Contracts DTO / Common Value / Common Error** に限定する

#### 15.3.3 禁止

* Domain が Exchanges（取引所実装）を参照すること
* Domain が Raw/Normalized DTO を参照すること
* Domain が Composition（Factory/DI）を参照すること
* Domain が契約 interface / 契約 DTO を新規に定義すること

---


### 15.4 公開 API の命名規約（Raw / Exchange / Client / Facade）[確定]

公開 API の命名は、層の誤用と混線を防ぐために次で固定する。

#### 15.4.1 Factory 名（入口）

* Raw（spec）：`CreateRaw(...)`
* Normalized（spec）：`CreateExchange(...)`
* Contracts（cross-exchange）：`CreateClient(...)`

#### 15.4.2 返り値の型名（層が一目で分かること）

* Raw DTO：接頭辞 `Raw` を付ける（例：`RawTickerDto`）
* Normalized DTO：接尾辞 `Normalized` を付ける（例：`BitflyerTickerNormalized`）
* Contracts DTO：取引所名を含めない（例：`TickerDto`）

#### 15.4.3 クライアント型名（利用者が迷わないこと）

* Contracts の基本入口は `IExchangeClient` とする（`CreateClient` の返り値）
* `IExchangeClient` の配下プロパティは API グループ名と一致させる

  * `Market`, `Trading`, `Account`, `Info`

#### 15.4.4 API グループ名とメソッド名（全層で同名）

* グループ名（例：`Market.GetTickerAsync`）は Raw / Normalized / Contracts で揃える
* メソッド名は揃え、**層の違いは返却 DTO の型で表現する**

#### 15.4.5 公開面に出してよい語彙

* 公開面（Contracts）に出してよいのは `Contracts` と `Common` のみ
* 公開面の名称に `Adapter` を含めない（Adapter は内部層として保持してよい）

---

## 16. 不変条件（Invariants）

* **Price/Size パース規約**

  * 共通層へ string を流さない（string は境界でのみ受理）
  * Try 系（非例外）を基本とし、OrThrow 系（例外）を必ず併設する
* **翻訳関所の一貫性**

* `normalizedDto → contractDto` は Adapter（およびその配下 Mapper）のみが行う

### 16.1 数値・時刻・識別子に関する補足不変条件

本仕様における数値、時刻、および識別子の取り扱いについて、以下を補足不変条件として定める。

1. 数値（Price / Size 等）は、正規化以降のすべての層において、文字列として扱ってはならない。
   数値の表現形式、スケール、精度は Common 層にて規定され、Contracts 層では当該型のみを使用するものとする。

2. 数値に対する丸め、切り捨て、切り上げ等の操作は、本仕様に明示的に規定されない限り、行ってはならない。
   表示や外部出力の都合による変換は、本仕様の適用範囲外とする。

3. 時刻は、正規化以降すべて UTC 基準で扱うものとし、基準時刻系および精度は Common 層にて一意に定義されるものとする。

4. シンボル（取引対象識別子）は、Contracts 層において単一の正規形を持つものとする。
   Raw 層および Normalized 層における表現差異は、Contracts 層に到達する前に解消されなければならない。

---

## 17. Converter / Mapper 規約

### 17.1 Converter

* 対象：Raw 層のみ
* JSON を仕様どおり読めるかを保証
* 失敗意味：取引所仕様不一致・破損

### 17.2 Mapper

* 対象：Normalize / Adapter
* 意味的変換・正規化
* 失敗意味：解釈不能・前提違反

### 17.3 サンプル JSON を正本とする運用規約

本仕様において、Raw 層の正本はサンプル JSON とし、その運用について以下を定める。

1. サンプル JSON は、Raw 層仕様（鏡像）の正本であり、実装および Converter の正当性判断基準とする。

2. サンプル JSON には、正常系のみならず、欠損フィールド、null 値、空配列、境界値、異常値等を含めるものとする。

3. サンプル JSON の追加または変更は、Raw 層仕様の変更とみなし、対応する Converter および Mapper の検証を伴うものとする。

4. Converter がサンプル JSON を正しく処理できない場合、当該事象は仕様不整合または実装不備として扱うものとする。

---

## 18. 禁止事項（破壊防止ルール）

* Raw/Normalized DTO を Contracts から返してはならない
* Contracts が取引所仕様を知ってはならない
* Exchanges 同士が依存してはならない
* Converter に意味判断を書いてはならない
* Mapper に JSON 構文処理を書いてはならない
* Adapter 以外で contractDto を生成してはならない

---

## 19. 一文要約

> **仕様は読む。意味は作る。**
> **Raw/Normalize は仕様、Adapter 以降はドメイン。**
> **取引所横断の共通化は Interface / DTO / Type / Error の4種に限定する。**

---

## 20. 運用および拡張に関する附則

本章は、本仕様の運用および将来的な拡張に関する共通原則を定めるものである。

### 20.1 エラー正規化方針

1. エラーは Common 層において正規化され、Contracts 層では正規化後のエラーのみを扱うものとする。

2. エラーには、少なくとも以下の属性を含めるものとする。

   * 種別（例：認証、レート制限、通信、取引所拒否、解析失敗、未知）
   * 再試行可否
   * 元エラー情報（取引所固有のコードおよびメッセージ）

3. 元エラー情報は、解析および診断目的で保持されるが、Contracts 層の利用者に対して必須とはしない。

### 20.2 ページング、レート制限およびキャンセル

1. ページングを伴う操作については、継続トークン等を用いた共通的な表現を採用するものとする。

2. レート制限は、エラーとして扱われ、必要に応じて再試行に関する情報を付与できるものとする。

3. Contracts 層で公開される操作は、可能な限りキャンセル可能であることを前提とする。
