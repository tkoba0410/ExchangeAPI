# Stage10（暫定仕様 / 第1段階）

最終更新: 2026-03-22  
対象ブランチ: `stage10`

## 1. 位置づけ

Stage10 は、既存文書の方針整合よりも先に、
新しい層モデルと client 生成モデルを固めるための設計フェーズとする。

本 Stage では、既存文書の層方針や公開面方針は拘束条件として扱わない。  
ただし、既存コード・既存 test・既存 live 検証資産は、
新仕様を設計するための試算材料として利用してよい。

当面の一次正本は本書 `stage10.md` とする。  
仕様が固まり次第、詳細文書は `stage10/` フォルダへ分割する。

Stage10 の展開方法は、既存実装を直接全面置換する方式ではなく、
別ブランチ・別配置で並行実装を進める案 A を採用する。

---

## 2. Stage10 のゴール

- 当初スコープを `bitFlyer` 専用に限定する
- `Contract` 層は Stage10 の対象から外し、当初方針として廃止する
- `Wire` / `Normalized` の 2 層を bitFlyer 専用 client 面として再定義する
- `Wire` を実行基盤として定義し、認証・署名・transport・logging などの基本機能を集約する
- `Normalized` は、上位へ向かうデータ変換のみを担う層として再定義する
- `Raw` 層は公開層としては廃止し、request encoding / response decode は internal codec として扱う
- 上位層 client から下位層 client へアクセスできるようにする
- 変換後に下位層へ戻って再取得・再試行・フォールバックする処理を上位層へ持たせない
- 取引所内共通化と取引所横断共通化を明確に分離し、Stage10 では前者のみを扱う

---

## 3. 基本方針

### 3.1 既存文書の扱い

- 既存の Normative / Process 文書の方針は、本 Stage の設計拘束としては一旦無視する
- 既存文書との整合は、Stage10 の後続タスクとして扱う
- 既存コードから読み取れる構造・依存・実装コストは、判断材料として利用してよい

### 3.2 bitFlyer 専用で始める理由

- 取引所内共通化内容を固める前に取引所横断抽象化を入れると、責務境界が濁りやすい
- `Contract` 層は取引所横断共通化のための層であり、Stage10 当初の焦点とずれる
- まず bitFlyer だけで `Wire` / `Normalized` の責務と client モデルを固める
- その後、取引所横断で本当に共通化すべき内容だけを別トラックで再抽出する

### 3.3 対応 API の基準

- Stage10 で対応対象とする bitFlyer API 一覧は [endpoints-bitflyer.md](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/inventory/endpoints-bitflyer.md) を使う
- 本 inventory は事実一覧であり、Method / Path / CanonicalSourceUrl / EndpointId / RequestType / ResponseType の母集団として参照する
- Stage10 の設計判断は inventory ではなく本書 `stage10.md` で定義する
- Stage10 では inventory の `PresentIn` と `Note` は引き継がない
- Stage10 の対応 API 一覧は、inventory を入力として Stage10 側で新たに書き起こす
- Stage10 では inventory に記載された endpoint を `Wire` / `Normalized` の 2 層へ再配置する前提で扱う

### 3.4 層の基本役割

- `Wire`
  - DTO を持たない
  - HTTP 実行、認証、署名、logging、observer、baseUri、transport 設定を担う
  - 単独で使用可能とする
- `Normalized`
  - 正規化 DTO を持つ
  - request 側では internal request encoder を用いて `Wire` request を構築する
  - response 側では `Wire` の raw JSON text に対して、`JSON検証変換 -> 意味変換 -> 意味検証` の順に処理して正規化 DTO を返す
  - `JsonConverter` は主に `JSON検証変換` と `意味変換` の段階で用い、再取得・再試行・fallback は持たない
  - 物理構成上は `Public/`、`Private/`、`Internal/RequestEncoding/`、`Internal/JsonValidation/`、`Internal/MeaningConversion/`、`Internal/MeaningValidation/`、`Internal/Errors/` に責務を分割する

### 3.5 実行基盤の集中

- 認証は全層で共通して使えるようにする
- ただし、各層が独自に認証を持つのではなく、同一の `Wire` 実体を共有する形で達成する
- 上位層は認証・署名・transport を再定義しない
- Private/Public の差は、`Wire` 実行基盤が持つ資格情報・署名能力の有無で表現する
- `Public` / `Private` の責務は各層で分離する
- ただし、公開 client 面は `PublicXxxClient` / `PrivateXxxClient` を層ごとに乱立させず、bundle 形でまとめる
- Stage10 では API 機能単位では公開面を分割しない
- 層内分割は、まず `Public` / `Private`、次に request encoding / JSON validation / meaning conversion / meaning validation / error の責務単位で行う
- `Private` runtime は `Public` runtime を包含し、認証付き構成では `Public` と `Private` の両方にアクセスできる
- `BaseUri` は `Wire` runtime の必須構成とする
- `TransportConfig` は `Wire` runtime の必須構成とする
- `HttpPolicy` は Stage10 当初方針では採用しない
- 再送、rate limit、circuit breaker、fallback などの回復戦略は `Wire` に持たせない
- 1 回の送信をどう再実行するかは、外部オーケストレーション側で明示的に制御する

### 3.6 Wire runtime の必須構成

#### 3.6.1 BaseUri

- `BaseUri` は「どこへ送るか」を定義する
- `BaseUri` は `Wire` client 生成時に必ず解決済みでなければならない
- 利用者入力としては省略可能でもよいが、その場合は bitFlyer 用既定値へ解決されなければならない
- `BaseUri` は host / root path の決定までを責務とし、endpoint 選択や業務意味は持たない

#### 3.6.2 TransportConfig

- `TransportConfig` は「何で送るか」と「誰が送信資源を所有するか」を定義する
- `TransportConfig` は `Wire` client 生成時に必ず解決済みでなければならない
- `TransportConfig` は送信経路の差し替えと所有権の明示を責務とする
- `TransportConfig` は `BaseUri`、認証、署名、再送戦略を責務に含めない
- Stage10 当初方針では、少なくとも以下の 3 形態を持てること
  - `ManagedHttp`
    - ライブラリ既定の `HttpClient` を生成して使う
  - `ExternalHttpClient`
    - 呼び出し側が管理する `HttpClient` を共有して使う
  - `ExternalTransport`
    - 呼び出し側が管理する `IHttpTransport` を使う

#### 3.6.3 3 形態の使い分け

- `ManagedHttp`
  - 標準利用向け
  - ライブラリ側が `HttpClient` を所有し、破棄責務も持つ
  - 想定利用シーン
    - ライブラリ既定の送信構成で、そのまま利用を開始したい
    - 接続資源の生成と破棄をライブラリ側へ任せたい
    - まず最小構成で動作確認し、外部共有や独自差し替えは後段で検討したい
- `ExternalHttpClient`
  - 外部 `HttpClient` 共有向け
  - 呼び出し側が `HttpClient` を所有する
  - 想定利用シーン
    - アプリケーション側で `HttpClient` の寿命と設定を一元管理したい
    - 接続プールや送信設定を他の client と共有したい
    - ライブラリに送信資源を所有させたくない
- `ExternalTransport`
  - テスト、モック、record/replay、独自送信制御向け
  - 呼び出し側が transport 実装を所有する
  - 想定利用シーン
    - 実通信を伴わない検証や再現実行を行いたい
    - 送信経路そのものを差し替えたい
    - 標準の `HttpClient` 抽象より低いレベルで独自制御を入れたい

### 3.7 リクエスト / レスポンス境界

#### 3.7.1 リクエスト境界

- リクエストは上位層から下位層へ向かう
- `Normalized` は bitFlyer 内意味で整理された request を受け取る
- `Normalized` は path / query / body / headers を直接構築しない
- `Normalized` は internal request encoder を通じて `Wire` 用 request を構築する
- request encoder は `Normalized` の内部実装であり、公開層は構成しない
- `Wire` は transport request だけを受け取る
- `Wire` が受け取るのは method / path / query / body / headers / endpoint identity などの transport 情報であり、上位層 DTO ではない

#### 3.7.2 レスポンス境界

- レスポンスは下位層から上位層へ向かう
- `Wire` は transport response を返す
- `Wire` が返すのは status / headers / raw JSON text などの transport 結果であり、DTO 化は行わない
- `Normalized` は `Wire` の raw JSON text を `TEXT -> JSON検証変換 -> 意味変換 -> 意味検証` の順に処理して、正規化 DTO を返す
- `JSON検証変換` は raw JSON text を JSON object として読めるかを確認し、decode 開始可能な形へ変換する
- `意味変換` は JSON object を bitFlyer 内意味の正規化 DTO へ落とす
- `意味検証` は正規化 DTO が公開契約として成立しているかを確認する
- `Normalized` は公開レスポンスとして中間 JSON object を露出しない

#### 3.7.3 逆流禁止

- `Wire` が `Normalized` request / response を知ることは禁止する
- `Normalized -> Wire` の逆戻り制御を持たせない
- 変換後にエラーが出た場合に、上位層内部で下位層へ戻って再取得する処理は持たせない
- 変換後の回復戦略、再実行、別経路取得、fallback は外部で明示的に扱う

#### 3.7.4 エラー分類の基本方針

- Stage10 では、新しい独自分類を追加する前に、既存 `CallErrorKind` を基底分類として引き継ぐ
- 基底分類は `Transport` / `Http` / `Codec` / `Mapping` / `Semantic` / `Unknown` の 6 種とする
- この基底分類は「どの段階で失敗したか」を表す
- 認証、rate limit、request 不正、server 異常などの運用上の意味分類は、基底分類とは別軸の補助情報として扱う
- `Unknown` は最後の退避先であり、既知の失敗パターンを安易に `Unknown` へ逃がさない

#### 3.7.5 層ごとのエラー確定責務

- `Wire`
  - 接続失敗、タイムアウト、キャンセル、TLS、送信失敗などの transport レベル失敗のみを `Transport` として扱う
  - `Wire` は HTTP status を見て `Http` へ変換しない
  - `Wire` 単独利用時は、利用者が status / headers / raw body を直接見る
- `Normalized/Internal/JsonValidation`
  - `Wire` response の status が非 `2xx` の場合は `Http` を確定する
  - raw JSON text の parse 失敗、JSON shape decode 失敗は `Codec` を確定する
- `Normalized/Internal/MeaningConversion`
  - bitFlyer 値表現から正規化 DTO / 正規化値へ落とす過程の失敗は `Mapping` を確定する
  - `JsonConverter` は主にこの段階で bitFlyer 値表現から正規化値への変換に用いる
  - 例: 未知の enum 値、想定外の値型、symbol / product_code / market の変換不能
- `Normalized/Internal/MeaningValidation`
  - 正規化 DTO が公開契約として成立しない場合は `Semantic` を確定する
- `Normalized/Internal/RequestEncoding`
  - request 不足、値範囲不正、引数組み合わせ不正、上位 API 契約違反は `Semantic` を確定する
- `Normalized/Public/` と `Normalized/Private/`
  - 公開 API 面として internal pipeline を束ねるが、変換・検証ロジック本体の置き場にはしない
- Stage10 では、変換後エラーを契機に `Normalized` 内部で `Wire` を再実行しない

#### 3.7.6 エラー情報の保持方針

- 全エラーで `EndpointId` と発生層を追跡できることを前提とする
- `Http` では `HttpStatus` を保持する
- `Http` と `Codec` では、診断用にサニタイズ済み `BodySnippet` を保持してよい
- 取引所固有 `error_code` や運用カテゴリを特定できる場合は、基底分類とは別軸の補助情報として保持してよい
- 将来 MCP や外部公開面へ接続する場合も、まず基底分類を保ち、その上に表示用分類を重ねる

### 3.8 下位層アクセス

- `Normalized` client は `Wire` client へアクセスできる
- ただし、この下位層アクセスは外部利用者の明示的制御のために提供するものであり、
  上位層内部での暗黙 fallback を正当化するものではない

### 3.9 Raw 層の扱い

- Stage10 当初方針では `Raw` 層を公開層として扱わない
- request encoding / response decode / JSON parse は `Normalized` 配下の internal codec として持ってよい
- `Raw` 相当の中間表現は external client 面へ露出しない
- 既存 `Raw` 実装は、Stage10 設計の試算材料・移行材料として利用してよい

### 3.10 Contract 層の扱い

- Stage10 当初方針では `Contract` 層を新仕様の構成要素として扱わない
- `Contract` は bitFlyer 専用設計が固まるまで設計対象外とする
- 取引所横断 DTO / 取引所横断 client / 取引所横断 capability は Stage10 第1段階の対象外とする
- 将来再導入する場合でも、bitFlyer 内部で固めた責務の上に後付けする

### 3.11 Normalized DTO の安定方針

- Stage10 の最終目標は、bitFlyer 用 `Normalized DTO` 全体を安定公開契約として固定することである
- そのため、`Normalized DTO` は最終的に将来の MCP や外部公開面へ接続可能な意味契約へ収束させる
- ただし移行期間中は、先に安定形へ到達した DTO を `Stable Core DTO`、見直し前の DTO を `Transitional DTO` として区別してよい
- `Stable Core DTO` / `Transitional DTO` の区別は移行概念であり、最終状態では `Normalized DTO` 全体固定へ収束させる
- `RawSnapshot`、`Extras`、`RawJson` などの lossless / diagnostics 用情報を含む DTO は、そのまま固定するのではなく、診断情報分離または安定公開形への再設計を経て最終固定対象へ取り込む

#### 3.11.1 Breaking Change の扱い

- 最終的に固定対象とする `Normalized DTO` では、以下を breaking change として扱う
  - 型名変更
  - プロパティ名変更
  - プロパティ型変更
  - プロパティ削除
  - optional だったプロパティの必須化
  - 同名プロパティの意味変更
- 移行期間中も、`Stable Core DTO` へ昇格したものには同じ breaking 規則を先行適用する
- optional な新規プロパティ追加を non-breaking として扱いたい場合、公開 CLR 形状は「プロパティ集合」を優先し、constructor 署名の変化を公開契約に含めない

#### 3.11.2 DTO 形状の方針

- 最終固定対象の `Normalized DTO` は primary-constructor record を採らない
- 理由は、`public sealed record Xxx(...)` では constructor 引数列、引数順、`Deconstruct(...)` が公開契約に含まれ、後からの optional プロパティ追加でも CLR 的に breaking になりやすいため
- 最終固定対象の `Normalized DTO` は property-based immutable type を基本とする
- 第1候補は `sealed class` + `init` property とし、同等に constructor / deconstruct を公開契約へ過剰に含めない形であれば許容する
- 最終固定対象の `Normalized DTO` の必須性は public constructor ではなく、`MeaningValidation` 完了時点で内部的に確定してから DTO 化する
- `Transitional DTO` では既存 record 形を暫定利用してよいが、固定対象へ昇格させる時点で公開形状を見直す

---

## 4. 目標 client モデル

### 4.1 生成単位

- `CreateWireClient(...)`
  - `Wire` のみを切り出して使用する
  - `Wire` bundle を返し、`Public` 面を必須で持つ
- `CreateNormalizedClient(...)`
  - `Wire` + `Normalized` を切り出して使用する
  - `Normalized` bundle を返し、`Normalized.Public` と `Wire` へアクセスできる

### 4.2 Public / Private 公開面

- 各層の内部責務は `Public` / `Private` で分離する
- ただし、factory と top-level client は層ごとに `Public` / `Private` を完全二重化しない
- Stage10 の公開 client 面は API 機能別には分割しない
- 公開面の第一分割軸は `Wire` / `Normalized`、第二分割軸は `Public` / `Private` とする
- `bundle.Public` は常に利用可能とする
- `bundle.Private` は認証情報を持つ `Wire` runtime が構築できる場合にのみ利用可能とする
- `Private` 側を持つ bundle でも、`Public` 面は同じ runtime 共有のまま利用可能とする

### 4.3 共有物

- `CreateNormalizedClient(...)` は、内部で使う `Wire` 実体を公開できること
- 認証、署名、transport、logger、observer は、同一 runtime を共有する
- `BaseUri` と `TransportConfig` は、同一 `Wire` runtime を識別する中核構成とする

### 4.4 Composition の扱い

- `Composition` は論理 2 層には含めない
- `Composition` は client runtime の組み立てと共有資源の配線だけを担う
- `Composition` は変換責務を持たない

---

## 5. 取引所内共通化と取引所横断共通化

### 5.1 Stage10 で扱うもの

- bitFlyer の `Wire` 実行基盤
- bitFlyer の正規化 DTO
- `Wire` transport response から `Normalized` DTO への internal decode / `JsonConverter` ベース変換
- `Normalized` request から `Wire` request への internal encode
- bitFlyer 内での Public/Private 実行条件差分
- bitFlyer 内で再利用可能な runtime / client 組み立て

### 5.2 Stage10 で扱わないもの

- bitFlyer / Bittrade 間の DTO 共通化
- 取引所横断の抽象 request/response
- 取引所横断の capability 設計
- 取引所横断の公開安定 client 面

### 5.3 分離原則

- bitFlyer の内部都合だけで成立する共通化は、bitFlyer 専用として定義してよい
- 取引所横断共通化は、「複数取引所で同じ責務・同じ意味・同じ lifecycle が確認できたもの」に限定する
- Stage10 第1段階では、取引所横断共通化を目的に bitFlyer 側の責務を歪めない

---

## 6. この方針のメリット

- 取引所内共通化と取引所横断共通化を混ぜずに整理できる
- `Contract` の都合で bitFlyer の責務を歪めずに済む
- `Wire` を単独で使った低レベル検証やデバッグが可能になる
- 公開 client 面が `Wire` / `Normalized` に絞られ、利用導線が明確になる
- request encode / response decode を公開層から外せるため、client 面の肥大化を抑えやすい
- 認証や transport 設定が `Wire` に集約され、実行条件の差異が減る
- 上位層で暗黙 retry/fallback を持たないため、挙動の追跡がしやすい
- 回復戦略を外部へ出すことで、再送条件と再送回数を利用側で明示制御できる

---

## 7. この方針のデメリット / リスク

- 当初は bitFlyer 専用となるため、Bittrade との parity は一旦後退する
- `Contract` 廃止により、取引所横断利用の既存導線は Stage10 の主対象から外れる
- 将来の再抽象化時に、再度境界整理が必要になる
- `Wire` に責務を集約しすぎると肥大化しやすい
- 中間の公開観測点が減るため、bitFlyer ネイティブ JSON を client 面から直接確認しにくくなる
- request encoder / response decoder を internal で強く管理しないと、`Normalized` が肥大化しやすい
- 共有 runtime の所有権、dispose、ライフサイクル設計を先に決めないと破綻しやすい

---

## 8. 第1段階のスコープ

### In Scope

- 本書で bitFlyer 専用の新しい層モデルを固定する
- `Wire` / `Normalized` client の責務と依存方向を固定する
- `Raw` 層を公開層から外し、internal codec として扱う方針を固定する
- `Contract` を Stage10 対象から外すことを明記する
- Stage10 の展開方法として案 A を採用することを明記する
- 認証・署名・transport を `Wire` へ集約する方針を固定する
- 上位層の「変換のみ」原則と「下位層へ戻らない」原則を固定する
- `Composition` の位置づけを「配線のみ」として固定する
- Stage10 向け live test の配置方針と初期スコープを固定する
- 既存コードを使って、実装可能性と主要な衝突点を洗い出す

### Out of Scope

- Bittrade への同時適用
- 取引所横断 DTO / client の設計
- `Contract` 層の維持・再設計
- 既存文書との整合完了
- 新仕様への全面移行
- POST を含む live test の全面展開
- stage10 以外の文書更新

---

## 9. 採用する展開案（案 A）

### 9.1 方針

- Stage10 は専用ブランチで進める
- 既存 `src/Exchanges/Bitflyer/*` を直接崩さず、別配置で新実装を並行構築する
- 文書は `stage10/` フォルダに集約する
- 新実装が固まるまで、既存実装は比較対象・回帰対象として維持する

### 9.2 採用ブランチ

- 作業ブランチ名の基準は `stage10-bitflyer-runtime` とする
- `stage10` ブランチ上で直接全面改変しない

### 9.3 採用ディレクトリ案

- 文書:
  - `stage10/architecture.md`
  - `stage10/runtime.md`
  - `stage10/dto-stability.md`
  - `stage10/migration.md`
- 新コード:
  - `src-stage10/Bitflyer/Wire`
    - `ExchangeApi.Stage10.Bitflyer.Wire.csproj`
    - `Public/`
    - `Private/`
    - `Internal/Auth/`
    - `Internal/Endpoints/`
    - `Internal/Runtime/`
  - `src-stage10/Bitflyer/Normalized`
    - `ExchangeApi.Stage10.Bitflyer.Normalized.csproj`
    - `Public/`
    - `Private/`
    - `Internal/RequestEncoding/`
    - `Internal/JsonValidation/`
    - `Internal/MeaningConversion/`
    - `Internal/MeaningValidation/`
    - `Internal/Errors/`
  - `src-stage10/Bitflyer/Composition`
    - `ExchangeApi.Stage10.Bitflyer.Composition.csproj`
    - `Bootstrap/`
    - `Factory/`
    - `Options/`
- 新テスト:
  - `tests-stage10/Bitflyer/Wire.Tests`
    - `ExchangeApi.Stage10.Bitflyer.Wire.Tests.csproj`
  - `tests-stage10/Bitflyer/Normalized.Tests`
    - `ExchangeApi.Stage10.Bitflyer.Normalized.Tests.csproj`
  - `tests-stage10/Bitflyer/Composition.Tests`
    - `ExchangeApi.Stage10.Bitflyer.Composition.Tests.csproj`
  - `tests-stage10/Bitflyer/LiveTests`
    - `ExchangeApi.Stage10.Bitflyer.LiveTests.csproj`
    - `Infrastructure/`

### 9.3.1 Project 境界

- `Wire` project は `Transport` と `Primitives` にのみ依存する
- `Normalized` project は `Wire`、`Application`、`Primitives` に依存する
- `Composition` project は `Wire`、`Normalized`、既存 `Composition`、`Primitives` に依存する
- `Raw` は独立 project にせず、`Normalized/Internal` の codec 実装へ吸収する
- live test project は `Wire`、`Normalized`、`Composition` を参照し、既存 live test 資産は流用候補とする

### 9.3.2 Normalized の責務と物理配置

- `Normalized/Public/`
  - Public endpoint 向けの公開 API 面だけを置く
  - response 変換ロジック本体は置かない
- `Normalized/Private/`
  - Private endpoint 向けの公開 API 面だけを置く
  - response 変換ロジック本体は置かない
- `Normalized/Internal/RequestEncoding/`
  - 正規化 request を `Wire` request 材料へ落とす責務を置く
  - request 側の意味検証もここで行う
- `Normalized/Internal/JsonValidation/`
  - `Wire` の raw JSON text を JSON object として検証・decode 開始可能な形へ変換する責務を置く
  - response 側の `TEXT -> JSON検証変換` 段階に対応する
- `Normalized/Internal/MeaningConversion/`
  - JSON object を bitFlyer 内意味の正規化 DTO 候補へ変換する責務を置く
  - `JsonConverter` と値揺れ吸収、symbol / product_code / market 変換補助をここへ置く
  - response 側の `意味変換` 段階に対応する
- `Normalized/Internal/MeaningValidation/`
  - 正規化 DTO 候補が公開契約として成立しているかを検証する責務を置く
  - response 側の `意味検証` 段階に対応する
- `Normalized/Internal/Errors/`
  - `Http` / `Codec` / `Mapping` / `Semantic` の確定規則と補助情報整形を置く
  - `Unknown` は最後の退避先としてのみ扱い、既知ケースの常用先にしない
- `Normalized` project では API 機能単位の物理分割を採らず、責務単位の物理分割を優先する
- `Normalized` の response 側は、物理構成上も `JsonValidation -> MeaningConversion -> MeaningValidation` の順で読める形を維持する

### 9.4 この案を採用する理由

- 既存 `src/Exchanges/*` を前提にした guard / parity test と途中状態で衝突しにくい
- bitFlyer の新 2 層 runtime を、旧実装と比較しながら育てられる
- 差分が大きくなりすぎず、責務整理と移行整理を分離できる
- 文書と実装試作を同時進行しやすい

### 9.5 当面の制約

- Stage10 でまず固めるのは `stage10.md` と `stage10/` 配下の文書である
- 新コード配置は採用方針として固定するが、この時点ではまだ実装開始を意味しない
- 既存本配置への統合は、新 runtime が固まった後の後続作業とする

---

## 10. 実装・文書化の進め方

- まず `stage10.md` で方針を固定する
- 次に `stage10/` フォルダを作り、bitFlyer client モデル、runtime モデル、責務分解、移行計画を分割記述する
- live test は `tests-stage10/Bitflyer/LiveTests` を第1配置候補とする
- live test の初期対象は `GET` 系とし、`Wire` / `Normalized` の read path を先に押さえる
- 既存 bitFlyer live test 資産は、Stage10 live test 設計の回帰資産・流用候補として扱う
- `POST` / 注文 lifecycle を含む live test は、`Normalized` request 境界の固定後に後段で導入する
- その後、既存コードを利用しながら試作実装を進める
- bitFlyer 専用設計が固まった後に、取引所横断共通化の再導入可否を判断する
- 既存文書との整合は、試作で方針が固まった後に行う

---

## 11. DoD

- `Wire` / `Normalized` の 2 層定義が文書上で明確である
- 各層のデータ表現と所有権が文書上で明確である
- `Contract` を Stage10 第1段階の対象外とすることが文書上で明確である
- 案 A の展開方針、ブランチ、文書配置、新コード配置が文書上で明確である
- Stage10 の物理構成案と project 境界が文書上で明確である
- `Normalized` の責務と物理配置の対応が文書上で明確である
- `Wire` が単独利用可能な実行基盤であることが文書上で明確である
- 認証・署名・transport が `Wire` に集約されることが文書上で明確である
- `BaseUri` と `TransportConfig` が `Wire` runtime の必須構成であることが文書上で明確である
- `HttpPolicy` を採用せず、回復戦略を外部で扱うことが文書上で明確である
- 上位層がデータ変換のみを担うことが文書上で明確である
- `Raw` を公開層として持たず、`Normalized` が internal codec と `JsonConverter` を用いて正規化することが文書上で明確である
- 各層の request / response 境界が文書上で明確である
- response 側の `TEXT -> JSON検証変換 -> 意味変換 -> 意味検証` の 4 段階が文書上で明確であり、物理構成と対応している
- `Transport` / `Http` / `Codec` / `Mapping` / `Semantic` / `Unknown` の基底分類と、各層での確定責務が文書上で明確である
- `Normalized DTO` 全体を最終的に固定する前提、移行中の `Stable Core DTO` / `Transitional DTO` の区別、および breaking change 規則が文書上で明確である
- 各層で `Public` / `Private` の責務を分けつつ、公開面は bundle として整理することが文書上で明確である
- 上位層が下位層へ戻って再取得・再試行しないことが文書上で明確である
- 下位層アクセス可能だが暗黙 fallback は禁止することが文書上で明確である
- Stage10 live test の配置先と `GET` 先行の初期スコープが文書上で明確である
- bitFlyer 専用で開始する理由と、取引所横断共通化を後段へ送る理由が文書上で明確である
- 次段階で `stage10/` フォルダへ分割する前提が明記されている

---

## 12. 現時点の未確定事項

- 各 bundle の具体名と公開プロパティ名をどう固定するか
- internal request encoder / `JsonValidation` / `MeaningConversion` / `MeaningValidation` の配置と命名をどう固定するか
- `Normalized/Internal/*` の型名・namespace・ファイル分割粒度をどう固定するか
- どの endpoint のどの `Transitional DTO` をどの順で固定対象へ収束させるか
- 補助的な運用分類（Auth / RateLimit / Server / Request など）をどこまで Stage10 で固定するか
- 共有 runtime の dispose 責務をどこに置くか
- bitFlyer 専用 runtime のうち、どこまでを後に取引所横断共通化できるか
- `POST` / 注文 lifecycle の live test をどの段階で導入するか
- 既存 `Adapter` / `Contracts` をどう段階的に外すか
- 既存 factory 名と新 factory 名をどう移行するか

---

## 13. 廃止条件（Sunset）

Stage 文書（`stage*.md`）は初回リリース前の暫定文書。  
`v1.0.0` 時点で本書を `docs/archive/` へ移動し、以後の追跡は別の正本へ統合する。
