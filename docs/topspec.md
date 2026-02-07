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

### Utilities（補助レイヤ）

Contracts に含めない便利機能（整形・変換・補助的操作）は、
独立レイヤ **Utilities** に集約する。
Utilities は 4 層構造に含めず、Contracts/Primitives に依存してよいが、
Contracts/Primitives から Utilities への依存は禁止する。
詳細は `docs/utilities.md` を参照する。

**責務（MUST）**

* データの整形・変換・補助的操作を提供する
* Contracts の Shape/Semantics を拡張しない

**禁止事項（MUST NOT）**

* 外部 I/O を行う
* 取引所固有仕様の吸収（Normalized/Adapter の責務）
* Contracts を参照する側への逆依存

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

#### 3.1.1 Wire 層の物理配置（MUST）

Wire 層の API/実装は、**署名有無（Public / Private）によってのみ分離**する。
MarketData / Trading / Account 等の**意味分類（種類別フォルダ分割）は行わない**。
`Internal` は実装補助の**非公開置き場**であり、分類軸として扱わない。
意味分類語彙は**物理配置・namespace・公開 API**に使用してはならない。

* 取引所配下の Wire 物理配置は次を基準形（Canon）とする。

```
src/Exchanges/<Exchange>/Wire/
  Public/
    Endpoints/
  Private/
    Endpoints/
  Constants/
  Properties/
  Internal/
```

* namespace は物理配置に一致させる（例）:
  * `ExchangeApi.Exchanges.<Exchange>.Wire.Public.*`
  * `ExchangeApi.Exchanges.<Exchange>.Wire.Private.*`
  * `ExchangeApi.Exchanges.<Exchange>.Wire.Internal.*`

* Endpoints は `WireCallSpec` を返すビルダであり、Wire 内部利用（internal）を前提とする。

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

#### 3.2.1 Raw 層の物理配置（MUST）

Raw 層の API/実装は、Wire と同様に **署名有無（Public / Private）によってのみ分離**する。
MarketData / Trading / Account 等の**意味分類（種類別フォルダ分割）は行わない**。
`Internal` は実装補助の**非公開置き場**であり、分類軸として扱わない。
意味分類語彙は**物理配置・namespace・公開 API**に使用してはならない。

* 取引所配下の Raw 物理配置は次を基準形（Canon）とする。

```
src/Exchanges/<Exchange>/Raw/
  Public/
  Private/
  Internal/
```

* namespace は物理配置に一致させる（例）:
  * `ExchangeApi.Exchanges.<Exchange>.Raw.Public.*`
  * `ExchangeApi.Exchanges.<Exchange>.Raw.Private.*`
  * `ExchangeApi.Exchanges.<Exchange>.Raw.Internal.*`

* Raw の Public/Private は「署名の有無」を表す。意味分類の代替として用いない（= Public/Private 以外で分けない）。

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

#### 3.3.1 Normalized 層の物理配置（MUST）

Normalized 層の API/実装は、Wire/Raw と同様に **署名有無（Public / Private）によってのみ分離**する。
MarketData / Trading / Account 等の**意味分類（種類別フォルダ分割）は行わない**。
`Internal` は実装補助の**非公開置き場**であり、分類軸として扱わない。
意味分類語彙は**物理配置・namespace・公開 API**に使用してはならない。

* 取引所配下の Normalized 物理配置は次を基準形（Canon）とする。

```
src/Exchanges/<Exchange>/Normalized/
  Public/
  Private/
  Internal/
```

* namespace は物理配置に一致させる（例）:
  * `ExchangeApi.Exchanges.<Exchange>.Normalized.Public.*`
  * `ExchangeApi.Exchanges.<Exchange>.Normalized.Private.*`
  * `ExchangeApi.Exchanges.<Exchange>.Normalized.Internal.*`

* Normalized の Public/Private は「署名の有無」を表す。意味分類の代替として用いない（= Public/Private 以外で分けない）。
* Normalized は「取引所内の正規化」であり、Contracts（取引所横断）を侵食しない。

---

#### 3.3.2 ExchangeInfo の Application 配置（MUST）

ExchangeInfo は「取引所仕様メタ情報」の統合・解決を担う
**複合写像（オーケストレーション）**である。
そのため、ExchangeInfo は Application 層として扱う。

* ExchangeInfo の DTO は **独自定義**とするが、**Contracts と同構成**を必須とする。
  * 独自 DTO は **Contracts と同構成 + 追加情報**を許容する（例: `Local` 拡張）。
  * Contracts への変換では **追加情報を落とす**（欠落ではなく境界）。
* ExchangeInfo は **Static + Dynamic の合成**で構成する。
  * Static: 仕様・固定値・手動更新が必要な情報（市場一覧、手数料テーブル等）
  * Dynamic: API から取得可能な状態・制約（稼働状態、手数料率、最小数量等）
  * Compose: Static をベースに Dynamic で上書き・拡張する（市場の増減を許可）
* ExchangeInfo の **Contracts への適合は Application/ExchangeInfo 配下の Adapter で行う**。
  * 変換元は **Local（独自）DTO** を正とし、Contracts へ写像する。
* ExchangeInfo は **Normalized への依存を許容**し、API Adapter への依存を禁止する。
  * Dynamic 取得のために Normalized API を呼び出してよい。
* ExchangeInfo は **共通サブシステム**を持てる。
  * 共通化対象（例）: CallMapper / MarketResolver / Compose Merge / Static JSON Loader
  * 共通サブシステムの配置は `src/Exchanges/Common/Application/ExchangeInfo/` を基準とする。
* 物理配置は次を基準形（Canon）とする。

```
src/Exchanges/<Exchange>/Application/ExchangeInfo/
  Public/
  Private/
  Internal/
```

* 取引所横断で共通化する場合は次を追加してよい。

```
src/Exchanges/Common/Application/ExchangeInfo/
  Adapter/
  Compose/
  Static/
```

* namespace は物理配置に一致させる（例）:
  * `ExchangeApi.Exchanges.<Exchange>.Application.ExchangeInfo.*`
  * `ExchangeApi.Exchanges.Common.Application.ExchangeInfo.*`

* 詳細な合成規則・マッピングは `_references/exchangeinfo.md` に記載する（非正本）。

---

### 3.4 Adapter 層（Contracts への適合）

**責務**

* Contracts の I/F・DTO を実装するための変換層
* Normalized（取引所内正規化）を Contracts（取引所横断）へ適合させる

**禁止事項（MUST NOT）**

* 取引所固有 endpoint の公開（取引所固有 endpoint は Normalized まで）
* 変換方向の逆流（Contracts → Normalized）

---

#### 3.4.1 Adapter 層の物理配置（MUST）

Adapter 層の API/実装は、Wire/Raw/Normalized と同様に
**署名有無（Public / Private）によってのみ分離**する。
MarketData / Trading / Account 等の**意味分類（種類別フォルダ分割）は行わない**。
`Internal` は実装補助の**非公開置き場**であり、分類軸として扱わない。
意味分類語彙は**物理配置・namespace・公開 API**に使用してはならない。

* 取引所配下の Adapter 物理配置は次を基準形（Canon）とする。

```
src/Exchanges/<Exchange>/Adapter/
  Public/
  Private/
  Internal/
```

* namespace は物理配置に一致させる（例）:
  * `ExchangeApi.Exchanges.<Exchange>.Adapter.Public.*`
  * `ExchangeApi.Exchanges.<Exchange>.Adapter.Private.*`
  * `ExchangeApi.Exchanges.<Exchange>.Adapter.Internal.*`

* Adapter の Public/Private は「署名の有無」を表す。意味分類の代替として用いない（= Public/Private 以外で分けない）。

#### 3.4.2 Adapter の責務境界（MUST）

* Adapter は **Contracts の I/F・DTO を実装するための変換層**である。
* Adapter は取引所固有の endpoint を公開しない（取引所固有 endpoint は Normalized まで）。
* 変換は「Normalized（取引所内正規化）→ Contracts（取引所横断）」の一方向とする。
  * 本項は **API 系統の Adapter**に適用する。ExchangeInfo 系統は 3.3.2 の規定に従う。

---

#### 3.4.3 Adapter と ExchangeInfo の関係（MUST）

ExchangeInfo は Application 系統であり、Contracts への適合は
`Application/ExchangeInfo` 配下で完結させる。
API 系統の Adapter とは **役割分離**して扱う。

* API 系統の Adapter は `src/Exchanges/<Exchange>/Adapter/` に置く。
* ExchangeInfo の Adapter は `src/Exchanges/<Exchange>/Application/ExchangeInfo/` 配下に置く。

---

#### 3.4.4 取引所スコープと混線防止（MUST）

取引所ごとの Raw / Normalized 層は、
**単一の API 呼び出し実装、または単一の Adapter / Raw / Normalized API インスタンスの内部**において、
複数取引所の Raw / Normalized API や DTO を混在させてはならない。

ここでいう「単一の API 呼び出し実装」とは、
1つの公開 API メソッド実装、または 1つの Adapter / Raw / Normalized クラス内部を指す。

例として、複数のメソッドに分割されていても、同一クラス内で同一の公開 API 呼び出し（同一 EndpointId）を構成する範囲は、1つの API 呼び出し実装とみなす。

ただし、取引所選択・ルーティング・フォールバック等を目的とした構成は、
設計上の例外として許容される。
これらの例外は、governance の裁定ルールに従い、採用理由および判断根拠を文書として記録すること。

### 3.5 Contract 層（取引所横断契約）

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

**Public / Private（署名有無）**

* Contracts でも Public/Private を「署名の有無」を表す分類として用いる（MUST）。
* Public/Private を MarketData / Trading / Account 等の意味分類の代替として用いてはならない（MUST NOT）。
  * 意味分類（種類別フォルダ分割）は行わない。

---

## 4. API サーフェス規則

### 4.1 各層は API を呼び出せる形を持つ（MUST）

* Wire / Raw / Normalized / Contract の **全層に API I/F を持つ**
* 各層は直下層を呼び、変換のみを行う

### 4.2 I/O は Wire のみ（MUST）

* Raw / Normalized / Contract は I/O を行ってはならない
* 外部通信は必ず Wire 層を経由する

#### 4.2.1 送信とパースの分離（MUST）

* Wire 層で `SendAsync` を実行し、Raw 層は `Call<WireCallSpec, WireResponse>` を受け取って JSON 解析のみを行う。
* Raw 層は `IWireTransport` を参照してはならない。
* 実装例の一貫性を保つため、Wire は送信実行クラス（例: `*WireCallExecutor`）、Raw はパース専用クラス（例: `*RawCallExecutor`）を持つ。

### 4.3 文字列の流入禁止（MUST）

* Wire 以外の層（Raw / Normalized / Contract）の API の in/out に **生の `string`** を使用してはならない。
* 文字列は entry point でのみ許可し、必ず TryParse/OrThrow で明示的に型化する。
* 自由記述（message/status/notes 等）は **`FreeText` 等の明示的なラッパ型**として表現する。
* 例外がある場合は Decisions に記録する。

#### 4.3.1 Normalized / Contract の専用型化（MUST）

* **Contract 層および Normalized 層**では、意味が確定できる値を専用型で表現しなければならない（MUST）。
* 専用型化の対象は少なくとも以下を含む（MUST）。
  * 識別子（`OrderId` / `AccountId` / `ExchangeOrderId` 等）
  * 価格・数量・金額・時刻・シンボル
  * 状態・種別・売買方向などの列挙的概念
* 列挙的概念は、未知値を保持できる表現（例: `Closed<T>`）で扱うこと（MUST）。
* `FreeText` は「自由記述」または「外部仕様上、意味確定不能な値」に限定して使用すること（MUST）。
* 専用型化できる値を `FreeText` で保持し続けてはならない（MUST NOT）。

### 4.4 取引所差分の配置（MUST）

* 取引所固有の既定値・ポリシーは `Exchanges/<Exchange>` 配下に閉じる。
* Transport / Utilities / Primitives / Contracts など共通層に取引所名を含む既定値や規約を置いてはならない。

---

### 4.5 例外（throw）と CallError の境界（MUST）

* entry point は「外部入力を最初に受け取る境界」を指す（Wire API / Public API / CLI 等）。
* entry point 以外での throw は禁止（MUST NOT）。
* Raw / Normalized は Try 系で失敗を CallError に変換し、Call として返す（MUST）。
* throw を許容するのは以下に限定する（MUST）:
  * プログラミングエラー
  * 設定不備
  * 内部不整合（プロセス継続不能）

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

### 8.1 設計方針（共通）

* EndpointId の設計において、取引所横断で規定するのは以下とする。
  * 構成要素
    * HTTP Method
    * Path
    * Scope（Public / Private 等）
  * 表現方法
    * PascalCase を用いること
    * 一般的な単語境界を保つこと
* EndpointId は、当該取引所内で一意に識別可能でなければならない。
* EndpointId は、可読性および一貫性が確保されていることを求める。

### 8.2 取引所固有ルール

* 各取引所は、8.1 に示した構成要素および表現方法を前提として、
  どの要素を採用するか、または省略するかを決定してよい。
* 各取引所における EndpointId の命名規約・一覧は、
  当該取引所の inventory 文書にて定義される。
* inventory に記載された EndpointId は、
  その取引所における **正本（Source of Truth）**とする。

### 8.3 定義

EndpointId は、**API の意味的単位を識別するための論理識別子**である。

* EndpointId は取引所内で一意でなければならない。
* EndpointId は文字列値ではなく、
  **定数名 / enum 名 / 静的メンバ名として扱われる識別子**である。
* EndpointId は Request / Response 構造や振る舞いを表現しない。

### EndpointId の一意性と識別性

* EndpointId は、本プロジェクトにおける **唯一の規範的識別子（Normative Identifier）** である。
* EndpointId は、inventory 文書に列挙されたもののみが有効であり、それ以外の識別子は存在しない。
* EndpointId は、別名（alias）、通称、代表名を持たない。
* 人間向けの便宜的名称、分類語、ナビゲーション用語は、EndpointId の同義語・代替識別子として扱ってはならない。
* EndpointId の解釈に、裁定者の判断を要してはならない。

### 8.4 責務と非責務

**責務**

* API endpoint を識別する
* 公式 API（Method / Path / Scope）との対応付けの軸となる

**非責務**

* Request / Response の構造や型
* paging / cursor / limit 等の挙動
* Capability 提供の可否
* 上位 API（Facade / Application 等）の存在

### 8.5 命名規則

* EndpointId は元の HTTP Method / Path / Scope（Public / Private 等）に由来する要素から構成する。
* 冗長な情報を含めず、意味が直感的に理解できる表現とする。
* HTTP Method を表す語（Get / Post 等）や Path 由来の語彙の採用・省略は、
  当該取引所の inventory に定義された命名規約に従う。

#### 8.5.1 取引所プレフィックス排除時の例外

* 取引所層（Wire / Raw / Normalized / Adapter）は、型名から取引所プレフィックスを排除する（MUST）。
* 取引所層の Composition は、公開型名に取引所プレフィックスを付与する（MUST）。
  例: `BitflyerFactory`, `BitflyerFactoryOptions`
* 取引所層の Application（`Application/ExchangeInfo` を含む）は、公開型名に取引所プレフィックスを付与する（MUST）。
  例: `BitflyerExchangeInfoService`, `BitflyerExchangeInfoSnapshot`
* ただし、同一コンパイル単位で型衝突または曖昧参照が生じる場合は、衝突回避のための意味名を付与すること（MUST）。
  例: `ExchangeSide`, `OpenOrderNormalized`
* 衝突回避の意味名は次の優先順で選択する（MUST）。
  1. `Contract`
  2. `Exchange`
  3. `Normalized`
  4. `Raw`
  5. `Wire`
* 衝突回避においては、取引所名プレフィックスよりも、役割を表す語（`Exchange`, `Normalized` など）を優先すること（SHOULD）。
* 型衝突または曖昧参照が発生した場合は、衝突の発生源となった下位層の型名を変更して解消する（MUST）。
* 上位層での `using alias` 等による回避は、下位層修正までの暫定対応に限定する（SHOULD）。
* 衝突回避を理由に取引所プレフィックスを再導入してはならない（MUST NOT）。
* 衝突回避目的の命名は例外ではなく、設計上の正規ルールとして扱う。

### 8.6 レイヤ別派生規則

* EndpointId は各層で 1:1 に対応する
* Raw / Normalized 層の API は `<EndpointId>CallAsync` を基本形とする
* EndpointId と API メソッド名は意味的に一致しなければならない
* コレクション応答は、コンテナ型を `<EndpointId>Response`、要素型を `<EndpointId>Item` とする（SHOULD）。
  例: `GetChildOrdersResponse` / `GetChildOrdersItem`
* 上記の 1:1 対応は、inventory において当該層が実装対象として指定されている場合に限る。
  実装対象の指定は inventory の `PresentIn` 列により行う。

### 8.7 inventory との関係

* EndpointId の一覧は inventory 文書に列挙される
* inventory は **取引所横断の規範ではない事実の一覧（Fact）**である
* 取引所固有の EndpointId の命名規約および一覧は、当該取引所の inventory 文書にて定義される
* inventory に記載された EndpointId は、その取引所における正本（Source of Truth）とする
* inventory に記載のない endpoint は未定義として扱う

* inventory の endpoint 一覧には `PresentIn` 列を設ける。
* `PresentIn` は、当該 endpoint が存在する層を表す集合であり、語彙は `{Wire, Raw, Normalized, Contracts}` とする。
* `PresentIn` は必須であり、省略してはならない（Wire のみ提供する場合も `Wire` を明示する）。
* `PresentIn` が空集合（どの層にも存在しない）であることを表す必要がある場合、`None` を用いる。
* `PresentIn` に含まれない層に当該 endpoint の API が存在しないことは、実装漏れではなく仕様である。

* inventory の `EndpointId` 列には正規 EndpointId のみを記載し、別名（alias）を記載してはならない。
  alias を記録する場合は、inventory 末尾の任意の `Aliases` 表に `EndpointId` との対応として記載する。

### 8.8 取引所差異の扱い

* EndpointId は取引所スコープの識別子であり、取引所横断で同一である必要はない。
* 取引所固有の差異は、8.1 の構成要素と表現方法を前提とした取捨選択として扱う。
* 差異は inventory に事実として記載する

※ 取引所実装間で「統一する/しない」を運用として管理する場合は、参考として `docs/_references/exchange-parity-policy.md` を用いる。

### 8.9 CallMeta.EndpointId の運用（MUST）

* `CallMeta.EndpointId` は、観測用途（ログ・表示・トレース・メトリクス）に限定して使用する（MUST）。
* 規範的な識別・分岐・対応可否判定は、EndpointId 定数または型で行う（MUST）。
* `CallMeta.EndpointId` の文字列比較を、仕様判断や業務分岐の根拠にしてはならない（MUST NOT）。
* `CallMeta.InternalEndpointId` 等の補助識別子は、規範的識別子として扱ってはならない（MUST NOT）。
* `CallMeta.EndpointId` と規範識別子が乖離した場合、規範識別子を正とし、`CallMeta` は観測値として補正する（MUST）。
* 乖離補正の責務は Adapter 層に置く（MUST）。

### 8.10 禁止事項

* EndpointId に取引所名を含めること
* 当該取引所の inventory に明示的な規定がない場合、
  EndpointId に HTTP Method を含めること
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
