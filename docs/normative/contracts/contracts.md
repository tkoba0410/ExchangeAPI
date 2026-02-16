# Contracts（横断契約）

本書は、本リポジトリにおける**公開 API 契約**の正本（Normative）である。  
技術仕様・設計規範については TopSpec を正本とする。  
層境界・I/O 制約などの規範は TopSpec（特に 4.2/4.2.1）に従う。  
ここに記載された MUST / MUST NOT は拘束力を持ち、実装都合よりも優先される。

本書は Contracts 層における公開安定 API の契約条文（Normative）を定める正本であり、
概要説明や背景整理は `docs/normative/contracts/overview.md` に委ねる。

本書は TopSpec に基づく契約内容の記述を目的とするものであり、設計上の裁定権や例外規定は持たない。
Contracts API の採用可否および取引所対応関係の正本（SSOT）は `docs/inventory/endpoints-contracts.md` に記録される。

---

## 1. 目的

- Contracts 層は、取引所をまたいで共通に利用できる **公開 API の契約**を提供する（MUST）。
- Contracts は **振る舞い（Behavior）ではなく、型・形状・意味論（Shape/Semantics）** を定義する（MUST）。
- 網羅性よりも安定性を優先し、必要最小限のみを追加する（MUST）。

---

## 2. 適用範囲

- 本書は `src/Contracts/` 配下の型・インターフェイス・公開 API 仕様に適用される（MUST）。
- Contracts は **取引所非依存**でなければならない（MUST）。
- Contracts は概念最上位フォルダであり、他のカテゴリに含めてはならない。
 - データの整形・変換・補助操作は Contracts の責務ではない（MUST NOT）。
   それらは Utilities レイヤに集約する。

---

## 3. 取引所非依存（横断性）

- Contracts の型・名前空間・識別子に **取引所名を含めてはならない**（MUST NOT）。
- Contracts に置いてよいのは、取引所横断の **Abstract DTO / Request / Response / Error / Result** のみである（MUST）。
- 取引所固有の差分は Contracts で表現してはならない（MUST NOT）。差分は Normalized/Adapter 側で吸収し、必要なら Decisions（Exceptions）に記録する（MUST）。

### 3.1 取引所識別情報の禁止（ExchangeCode 等）

Contracts は **取引所非依存**であり、Contracts の公開型（ContractDTO/Request/Response 等）に
取引所識別情報を含めてはならない（MUST NOT）。

禁止対象（例）：

- `ExchangeCode` / `ExchangeId` / `ExchangeName` 等の取引所識別子
- 「どの取引所から取得されたか」を表すフィールド全般

取引所の選択・識別・束ねは Contracts の責務ではない。
それらは `Composition` / `Application`（利用者境界）側で完結させなければならない（MUST）。

### 3.2 資格情報取得の非責務（Credential Provider）

Contracts は、API 資格情報（API Key / Secret 等）の **取得方法・保存方法・解決方法**を
責務としてはならない（MUST NOT）。

資格情報の取得・解決は、実行環境や配線に依存するため、
**Composition（DI / Factory / Provider）層の責務**とする（MUST）。

そのため、以下は Contracts に含めてはならない：

- 資格情報取得用インターフェース（例：`IApiCredentialProvider`）
- 環境変数・ファイル・OS 依存の認証情報解決ロジック

---

## 4. API 返却形式（Call-only）

- 公開 API は **Call を唯一の返却形式**とする（MUST）。
- Response（DTO）を直接返してはならない（MUST NOT）。
- Call は **成功/失敗/メタ情報**を一体として表現できなければならない（MUST）。

---

## 5. エラーとメタ情報

- エラーは Call のメタ情報（例：CallMeta）に集約する（MUST）。
- 例外（throw）は、エントリポイント（例：OrThrow / ParseOrThrow 等）以外では使用してはならない（MUST NOT）。
- エラー表現は **分類レベル**（例：通信失敗 / 認証失敗 / 業務的失敗）までに留め、詳細コード体系を契約に含めない（MUST）。
- リトライ可否・HTTP ステータス・レート制限などの運用情報は、必要に応じて Call のメタで表現する（MUST）。

## 5.1 NotSupported（Shape / Semantics）

NotSupported は、Contracts API における **capability 不足**を示す語彙として予約する。
ただし、未対応 capability は Facade の nullable capability により **事前に判定可能**でなければならず、
NotSupported を通常制御フローとして常用してはならない（原則使用しない）。

- CallErrorKind: Semantic
- Tags:
  - Retryable = false
- Message:
  - "NotSupported:<feature>"

## 6. 型の所有権と返却責務

- Abstract DTO（公開契約の型）は **Contracts 層が定義元（オーナー）**である（MUST）。
- Contracts で定義された型を **返却する責務を持つのは Adapter（および Contracts 実装）** である（MUST）。
- Normalized 層は取引所固有の意味確定 DTO を返し、Adapter が受け取り Contracts 型へ写像する（MUST）。

---

## 6.1 Facade API の Public / Private（署名有無）

Contracts の Facade API は Public / Private に分離する（MUST）。
ここでの Public/Private は **署名の有無**のみを表す（MUST）。

- Public/Private を MarketData / Trading / Account 等の意味分類の代替として用いてはならない（MUST NOT）。
- 分離の目的は「認証境界の明確化」であり、Contracts の Shape / Semantics を変更してはならない（MUST NOT）。
 - 公開 API 面の分類は **Public / Private のみ**とし、意味分類語彙を公開 I/F 名称や namespace に使用してはならない（MUST NOT）。

---

## 6.2 Facade API 引数順序（Argument Order）

Contracts の公開メソッドは、**引数順序を一貫した規則**で定義する（MUST）。
順序は呼び出し時の認知負荷を下げる目的で固定し、メソッドごとに恣意的に変更してはならない（MUST NOT）。

**標準順序（必須）**

1. **ルーティング要素**: `Symbol` / `Market` など対象市場を特定する引数
2. **主要操作パラメータ**: `Side` / `Size` / `Price` / `OrderKey` など
3. **絞り込み・ページング**: `Limit` / `Cursor` / `From` / `To` / `Since` など
4. **`CancellationToken`**: 常に最後

**補足**

- 引数が無い API は `CancellationToken` のみを受け取る（MUST）。
- 追加引数が必要になった場合も、上記順序に従って挿入する（MUST）。

---

## 6.3 Request DTO と利便性オーバーロード

Contracts の公開 API は **Request DTO を第一の契約**とする（MUST）。  
利用者の使い勝手を確保するため、必要に応じて **DTO を生成する利便性オーバーロード**を追加してよい（MAY）。

**ルール（必須）**

- DTO 受け取り版を **正本（canonical）** とする（MUST）。
- オーバーロードは **DTO を内部で生成して委譲するだけ**に留める（MUST）。
- オーバーロードの引数順序は **6.2 の順序規則**に従う（MUST）。

**配置ポリシー（層）**

- 利便性オーバーロードを置いてよい層は **Contracts と Normalized** のみ（MUST）。
- Adapter / Raw / Wire には **置いてはならない**（MUST NOT）。

---

## 6.4 取引所実装の物理配置ルール

取引所実装の物理構造は、以下の 3 軸を基本とする（MUST）。

- 取引所: `Bitflyer`, `Bittrade`, ...
- レイヤ: `Wire`, `Raw`, `Normalized`, `Adapter`, `Application`
- 可視性: `Public`, `Private`, `Internal`

意味分類（例: `Account` / `Trading` / `Market`）は、公開構造の第一軸としては採用しない（MUST NOT）。

### MarketCatalog の位置づけ

`MarketCatalog` は取引所固有の市場定義（`Symbol` / `ProductCode` / `Type` など）を保持する
Application 配下の内部要素として扱う（MUST）。

- 取引所固有処理: `src/Exchanges/{Exchange}/Application/MarketCatalog`
- Facade 公開境界は ExecutionContext の塊（AccountInfo 相当を含む）に依存しない（MUST）。

### Application / Composition の責務分担

- `src/Application`: 取引所横断のユースケース
- `src/Exchanges/{Exchange}/Application`: 取引所固有ユースケース
- `src/Composition`: 取引所横断の配線（DI / Bootstrap）
- `src/Exchanges/{Exchange}/Composition`: 取引所固有の配線

### 目標ディレクトリツリー

```text
src/
  Application/
  Composition/
  Contracts/
  Primitives/
  Utilities/
  Transport/
  Exchanges/
    Common/
      Application/
    Bitflyer/
      Wire/{Public,Private,Internal}
      Raw/{Public,Private,Internal}
      Normalized/{Public,Private,Internal}
      Adapter/{Public,Private,Internal}
      Application/MarketCatalog/
      Composition/
    Bittrade/
      Wire/{Public,Private,Internal}
      Raw/{Public,Private,Internal}
      Normalized/{Public,Private,Internal}
      Adapter/{Public,Private,Internal}
      Application/MarketCatalog/
      Composition/
```

### 移行フェーズ

1. Facade 公開境界から ExecutionContext の塊依存を除去する。
2. 取引所差分を `Application/MarketCatalog` と resolver 実装へ集約する。
3. namespace / using / 参照を統一する。
4. `dotnet build` / `dotnet test` を通す。
5. inventory / 契約文書を同期更新する。

---

## 7. 層境界の型制約

- Contracts の公開メソッド（インターフェイス）は、Contracts で許可された型のみを in/out に用いる（MUST）。
- Raw DTO / Exchange DTO / Wire string を Contracts の in/out に含めてはならない（MUST NOT）。
- Contracts の公開 API で **生の `string`** を in/out に使用してはならない（MUST NOT）。
- 自由記述は `FreeText` 等の明示的なラッパ型で表現する（MUST）。
- Contracts で意味が確定できる値（識別子・価格/数量・列挙的概念等）は専用型で表現する（MUST）。
- 列挙的概念は未知値を保持できる表現（例: `Closed<T>`）を用いる（MUST）。
- 専用型化可能な値を `FreeText` に留めてはならない（MUST NOT）。
- `string` を保持する DTO が必要な場合は、例外として Decisions に記録する（MUST）。

---

## 8. 命名規約

### 8.1 DTO 命名

- 型名は **名詞 + Context** を基本とする（MUST）。
- 意味の異なる DTO を suffix だけで区別してはならない（MUST NOT）。
- Raw / Normalized / Adapter の区別を型名 suffix で表現してはならない（MUST NOT）。区別は namespace / フォルダで行う（MUST）。

### 8.2 プロパティ命名

- 公開プロパティは PascalCase とする（MUST）。
- 略語は一般的なもののみ使用する（MUST）。
- 取引所固有の語彙をそのまま転記してはならない（MUST NOT）。

### 8.3 Contracts API 命名

- ContractApiId は **動詞を省略した名詞**で表現する（MUST）。例: `Ticker`, `Board`, `ExecutionsPublic`。
- Request/Response 型は **`<ContractApiId>Request` / `<ContractApiId>Response`** とする（MUST）。
- Facade メソッド名は **`Get` + `<ContractApiId>` + `Async`** を基本とし、`Call` は付与しない（MUST）。例: `GetTickerAsync`。

---

## 9. Nullable / Optional ポリシー

- Nullable を許可するのは、次の場合に限る（MUST）。
  - 公式 API 上、常に欠落する可能性がある
  - 取引所によって意味が異なる
  - 将来的な拡張余地として意図的に空を許容する
- Nullable は設計上の判断であり、「たまたま無い」ことを理由にしてはならない（MUST NOT）。
- 常に存在する概念は Non-nullable とする（MUST）。
- 値が不明な場合は Nullable に逃がさず、別の状態・型で表現する（MUST）。

---

## 10. Page / Cursor / Limit 契約

- Page は「1回の取得結果」を表す（MUST）。
- 戻り値の件数と要求した limit は区別される（MUST）。
  - Limit は「要求」であり、保証ではない（MUST）
  - 実際に適用された limit は Meta 情報として保持する（MUST）
- Cursor はページング状態を表す **opaque な値**とする（MUST）。
  Cursor の内部構造や生成方法は契約に含めない（MUST NOT）。

---

<!-- 運用・進化方針は process.md / exceptions.md に移設済み -->

## 11. Anti-Rules（禁止事項）

- 公式 API 仕様の写経をしてはならない（MUST NOT）。
- フィールド単位の詳細説明を契約に含めてはならない（MUST NOT）。
- 取引所ごとの差分説明を契約に含めてはならない（MUST NOT）。
- 実装都合による一時的 DTO を公開してはならない（MUST NOT）。

---
