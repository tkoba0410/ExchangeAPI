# TopSpec（統合版）

本書は ExchangeAPI リポジトリの **最上位規範（Normative）** である。  
ここに記載された MUST / MUST NOT は拘束力を持ち、他の文書・コメント・慣習よりも優先される。

---

## 1. 目的

- 文書量を増やさずに、実装時の判断余地（揺らぎ）を最小化する。
- 層の責務、境界、データ形状、依存方向を固定し、取引所追加・変更時も迷いなく維持できるようにする。

---

## 2. 正本と権威（Authority）

### 2.1 正本
- 各取引所の API 仕様の正本は **公式 API 文書**である（MUST）。
- 本リポジトリは公式仕様の意味的仕様を再定義しない（MUST NOT）。

### 2.2 文書の優先順位
1. TopSpec（本書）
2. Contracts Overview（利用者契約）
3. Contracts（Shape / Semantics）
4. Exceptions
5. Inventory / Endpoints

---

## 3. 層モデル（Layer Model）

### 3.1 必須の 4 層
本システムは次の **4 層構成**を必須とする（MUST）。

```
Wire → Raw → Normalized → Contracts

※ `Application` / `Composition` は層ではない（後述）。層モデルの外側に置く。
```

### 3.2 層を跨ぐ呼び出しの禁止
- 依存・呼び出しは **隣接層間に限定**する（MUST）。
- 層を飛び越えた参照・呼び出しは禁止する（MUST NOT）。

### 3.3 Adapter の位置づけ
- Adapter は「層」ではなく、**Normalized（取引所内の意味確定）→ Contracts（横断契約）への翻訳境界**である。
- Adapter は Contracts を実装・返却するが、Contracts の意味論を変更してはならない（MUST NOT）。

### 3.4 Application / Composition の位置づけ
- `Application` は取引所横断の **ユースケース（振る舞い）** を置く。層ではない。
- `Composition` は DI / Factory / Provider 等の **組み立て（配線）** を置く。層ではない。
- `Contracts` は Shape/Semantics のみであり、Application/Composition を含めてはならない（MUST NOT）。
- `Application` は `Contracts.*` を参照してはならない（MUST NOT）。
  - 公開 I/F（Facade）の型は `Composition` が受け取り、`Application` 独自の入出力型へ変換する。

---

## 4. 層の責務と禁止事項

### 4.1 Wire
**責務（MUST）**
- 外部 I/O（HTTP Path / Query / Header / Body など）の入口となる。
- 入口の文字列・バイト列を、下流に流してよい形へ変換する。

**禁止（MUST NOT）**
- JSON の意味解釈を行う。
- Wire 以外の層へ `string` をそのまま流す。

### 4.2 Raw
**責務（MUST）**
- 外部 JSON の表現を **lossless** に保持する（型混在・欠損・null を含む）。

**禁止（MUST NOT）**
- 単位換算・時刻統一・売買方向/注文種別の解釈などの意味確定。
- 意味のあるデフォルト補完。

### RawJson / ClosedSet に関する判断について

RawJson の保持有無や ClosedSet の拡張可否といった判断は、
**TopSpec や Contracts では行わない**。

これらは取引所ごとの差異として **inventory 文書にのみ記録・判断される**。
共通契約やレイヤ責務へ逆流させてはならない。

### 4.3 Normalized
**責務（MUST）**
- 取引所内で意味を確定し、表現差分を統一する（正規化）。

**禁止（MUST NOT）**
- Raw DTO（外部表現）を公開面へ露出する。

### 4.4 Contracts
**責務（MUST）**
- 取引所横断の公開契約（語彙・型）を提供する。

**禁止（MUST NOT）**
- 取引所名を含む型・名前空間を定義する。
- 取引所固有差分を契約に持ち込む。

---

## 5. 境界ポリシー（Boundary Rules）

### 5.1 Call-only
- 公開 API の返却形式は **Call** に統一する（MUST）。
- Response DTO を直接返してはならない（MUST NOT）。

### 5.2 型安全（in/out の固定）
- 層の型の統一は、**メソッドの in/out（入力/出力）**で合わせる（MUST）。
- 各層の公開メソッドは、その層で許可された型のみを in/out に用いる（MUST）。

---

## 6. データ形状（Data Shape）

### 6.1 型カテゴリ
- **Wire string**：外部 I/O 由来の文字列表現
- **Primitive DTO**：プリミティブ型のみで構成された DTO（意味付けをしない）
- **Exchange DTO**：取引所固有の DTO（取引所名を含む）
- **Abstract DTO**：取引所横断の抽象 DTO（公開契約）

### 6.2 層ごとの許可型
- Wire：Wire string（入口のみ）
- Raw：Primitive DTO / Exchange DTO（lossless）
- Normalized：Exchange DTO（意味確定済み）および Contracts への変換材料
- Contracts：Abstract DTO（公開契約）のみ

### 6.3 命名規約（機械判定）
- Exchange DTO は型名または名前空間に取引所名を必須とする（MUST）。
- 横断型（Abstract/Contract）は型名・名前空間に取引所名を含めてはならない（MUST NOT）。

---

## 7. 固有／横断（Variation）

- 横断基盤は **概念単位の最上位フォルダ**に配置する（MUST）。
- 取引所固有コードは常に `src/Exchanges/<Exchange>/...` に閉じる（MUST）。
- Shared という物理カテゴリは使用してはならない（MUST NOT）。

補足（カテゴリ固定）：
- `Transport`：通信基盤（HTTP/JSON/Retry/署名/共通Wire入出力の枠など）
- `Primitives`：横断の基礎型・パーサ・小さな純粋関数（ユースケース禁止）
- `Application`：横断ユースケース（例：MarketResolver / Polling 等）
- `Composition`：組み立て（Factory/CredentialProvider/JsonExchangeInfoApi 等）
- `Contracts`：公開契約（Shape/Semantics）※振る舞い禁止

### 7.1 取引所間共通実装の正規形（Canon）

本節は、**取引所追加時に必ず従う実装形（構造の正規形）**を定義する。
ここに定義された形は、取引所間で一致させなければならない（MUST）。

NOTE: 本リポジトリには Canon 未達の実装が残る場合がある。Canon は常に目標形であり、未達は移行タスクとして扱う。

#### 7.1.X Canon: 同名 Contracts API の前提条件差を作らない

- **同名の Contracts API は、取引所間で呼び出し前提条件差を持たない。**
- 認証・口座情報・ID 等の前提条件が必要な場合、差異は次のいずれかで吸収する。
  1. **Composition が前提条件を自動解決**し、利用者に追加入力を要求しない。
  2. 前提条件が満たせない構築形態では、当該 API を **capability（TryGet/Has 系など）として提供しない**（利用者は事前判定できる）。
- **前提条件不足を理由に NotSupported を返すことを禁止する。**
  - NotSupported は capability 不足にのみ用いる（利用者契約に従う）。

#### 7.1.Y Canon: API 投入用 Market/Symbol 表現の責務位置

- Market 解決（ExchangeInfo / Resolver 等）の結果は、**その取引所の API 呼び出しに直接投入できる Market/Symbol 表現**を含まなければならない。
- API 投入のための表記変換（例：`BTC_JPY` → `btcjpy`）は、
  **Market 解決の段階で完了**し、Contracts / Adapter の呼び出し直前で追加加工してはならない。

#### 7.1.Z Canon: ExecutionItem.Market の設定元を固定する

- `ExecutionItem.Market` は常に **request に含まれる Market** を設定する。
- レスポンス側の市場識別子を参照してはならない。

#### 7.1.W Canon: NotSupported の `<feature>` を capability 名に固定する

- `NotSupported:<feature>` の `<feature>` は、利用者が事前判定に用いる **capability 名（操作名）と一致**しなければならない。
- `AccountIdRequired` 等の「内部理由」を `<feature>` にしてはならない。
  - 内部理由は必要なら `Message` の追記等（`NotSupported:<feature> ...`）に留め、feature 自体は不変とする。

#### 7.1.1 Raw API の正規形
- Raw の bundle（例：`I<Exchange>RawApi`）は **Sub-API のみ**を公開する（MUST）。
- Raw bundle 直下に利便メソッド（直メソッド）を追加してはならない（MUST NOT）。

#### 7.1.2 Normalized API の正規形
- Normalized の factory は **Bundle を 1 つ返す**形に統一する（MUST）。
- Bundle の構造（どの Sub-API を持つか）は取引所間で同型に保つ（MUST）。
- **Bundle メンバ（Sub-API）の optional を `null` で表現してはならない**（MUST NOT）。
  - NoOp 実装は `src/Exchanges/<Exchange>/Normalized/` 配下に配置する（MUST）。
    - Adapter / Contracts / Composition に NoOp を置いてはならない（MUST NOT）。

#### 7.1.3 認証（署名）注入点の正規形
- 署名（認証）は **RestClient の requestSigner（単一注入点）**で行う（MUST）。
- 署名を `IHttpTransport` の wrapper として別系統で提供してはならない（MUST NOT）。

#### 7.1.4 Error 分類の正規形
- ErrorClassifier は Adapter 内部の変換規則である。
- ErrorClassifier は `internal` とし、外部へ公開してはならない（MUST NOT）。
- ErrorClassifier の利用は共有インスタンス（singleton 等）に統一する（MUST）。

---

## 8. 物理構成（src）

### 8.1 正本宣言
- `src/` 配下の物理ディレクトリ構成は **仕様の一部（正本）**である（MUST）。
- 文書と物理構成が矛盾した場合は、原則として文書を修正する（MUST）。

### 8.2 Skeleton（正本）

```
src/
  Transport/
  Primitives/
  Application/
  Composition/
  Contracts/
    Common/
    Facade/
  Exchanges/
    <Exchange>/
      Wire/
      Raw/
      Normalized/
      Adapter/
```

### 8.3 配置規範

- 各物理フォルダは **1 つの概念・責務のみ**を持つ（MUST）。
- 各フォルダ直下に 1 csproj を置き、当該フォルダ配下のみを Compile する（MUST）。
- csproj が他フォルダのソースを Compile してはならない（MUST NOT）。

### 8.4 アセンブリ境界

- アセンブリ境界は物理フォルダ境界と一致させる（MUST）。
- glob による複数 Exchange / Layer の集約アセンブリを作成してはならない（MUST NOT）。

### 8.5 Namespace 規則（例外禁止）

すべての C# ソースコードは物理ディレクトリ構成と一致する namespace を持たなければならない。  
例外は禁止する。

例:
- src/Transport/... → ExchangeApi.Transport...
- src/Primitives/... → ExchangeApi.Primitives...
- src/Application/... → ExchangeApi.Application...
- src/Composition/... → ExchangeApi.Composition...
- src/Contracts/Common/... → ExchangeApi.Contracts.Common...
- src/Contracts/Facade/... → ExchangeApi.Contracts.Facade...
- src/Exchanges/Bitflyer/Raw/... → ExchangeApi.Exchanges.Bitflyer.Raw...

---

## 9. Transport

Transport（HTTP/JSON/Retry 等の横断的通信基盤）は層ではなく、横断的通信基盤である。  
Transport は `src/Transport/` に配置する。  
Wire は Transport を参照するが内包してはならない（MUST）。

### 9.1 Transport に置くもの（例）
- 共通の Wire 入出力枠：`IWireTransport` / `WireTransport` / `WireCallSpec` / `WireResponse`
- HTTP クライアント、署名、リトライ、JSON 低レベル処理（意味解釈をしない）

## 9.2 Application
Application は `src/Application/` に配置する。  
取引所横断のユースケース（例：MarketResolver / OrderPolling 等）を置く。  
Application は `Contracts.*` を参照してはならない（MUST NOT）。
Application の入出力は `Application.*` 配下の型（Command/Query/Result 等）として定義する。

## 9.2.1 公開契約との接続（変換責務）
`Composition` は Facade（公開 I/F）と `Application` の間の変換を担う（MUST）。
Facade の Request/Interface を `Application` に直接流してはならない（MUST NOT）。

## 9.3 Composition
Composition は `src/Composition/` に配置する。  
DI/Factory/CredentialProvider/JsonExchangeInfoApi 等の「組み立て（配線）」を置く。  
Composition は実行側の都合を集約する“終端”であり、他のカテゴリへ実装都合を逆流させてはならない（MUST NOT）。
Composition は Facade 型 ⇄ Application 型の変換を内包し、契約都合を Application へ逆流させてはならない（MUST NOT）。

---

## 10. RawJson の扱い

- RawJson は Raw の結果として保持し、必要に応じて下流へ伝播してよい（MUST）。
- RawJson の解釈（意味確定）は Normalized 以降でのみ行う（MUST）。

---

## 11. 契約の進化（Interface Evolution）

- 公開契約（Contracts）の破壊的変更は原則禁止し、必要な場合は互換レイヤまたはメジャーバージョンで表現する（MUST）。
- 境界（in/out 型、Call-only、層ジャンプ禁止）を破る変更は **例外**として扱い、Decisions に理由を記録しなければならない（MUST）。

---

## 12. 例外

- 本書の規範に従えない場合は Decisions / Exceptions に理由を記録しなければならない（MUST）。
- 例外として認められるのは次の場合に限る。
  - 公式 API 仕様による不可避な制約
  - 後方互換性維持のための必要性
  - セキュリティ・性能・法令上の理由
