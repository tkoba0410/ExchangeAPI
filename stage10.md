# Stage10（暫定仕様 / 第1段階）

最終更新: 2026-03-23  
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

実装着手は白紙再設計を許容する。  
ただし、最終的な責務境界、公開面、物理構成が同一であるなら、
現行の Stage10 試作実装を土台として段階的に寄せてもよく、
全面作り直しを必須条件にはしない。

---

## 2. Stage10 のゴール

- 当初スコープを `bitFlyer` 専用に限定する
- `Contract` 層は Stage10 の対象から外し、当初方針として廃止する
- `Protocol` / `Native` の 2 層を bitFlyer 専用 client 面として再定義する
- `Protocol` を実行基盤として定義し、認証・署名・transport・debug logging / diagnostics などの基本機能を集約する
- `Native` は、transport を持たない request / response 契約層として再定義する
- 公開 client 面は facade、内部実装は endpoint module を基本形とする
- endpoint 固有実装は独立 class に分離し、facade は薄い forward に徹する
- `Raw` 層は公開層としては廃止し、request encoding / response decode は internal codec として扱う
- 上位層 client から下位層 client へアクセスできるようにする
- 変換後に下位層へ戻って再取得・再試行・フォールバックする処理を上位層へ持たせない
- 取引所内共通化と取引所横断共通化を明確に分離し、Stage10 では前者のみを扱う
- 最終構成が同一なら既存 Stage10 実装を土台として再編してよく、白紙作り直しは必須としない

---

## 3. 基本方針

### 3.1 既存文書の扱い

- 既存の Normative / Process 文書の方針は、本 Stage の設計拘束としては一旦無視する
- 既存文書との整合は、Stage10 の後続タスクとして扱う
- 既存コードから読み取れる構造・依存・実装コストは、判断材料として利用してよい

### 3.2 bitFlyer 専用で始める理由

- 取引所内共通化内容を固める前に取引所横断抽象化を入れると、責務境界が濁りやすい
- `Contract` 層は取引所横断共通化のための層であり、Stage10 当初の焦点とずれる
- まず bitFlyer だけで `Protocol` / `Native` の責務と client モデルを固める
- その後、取引所横断で本当に共通化すべき内容だけを別トラックで再抽出する

### 3.3 対応 API の基準

- Stage10 で対応対象とする bitFlyer API の母集団は、既存 inventory [endpoints-bitflyer.md](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/inventory/endpoints-bitflyer.md) を import source として参照してよい
- 本 inventory は事実一覧であり、Method / Path / CanonicalSourceUrl / EndpointId / RequestType / ResponseType の母集団としてのみ使う
- Stage10 の設計判断は inventory ではなく本書 `stage10.md` と [`stage10/endpoints-bitflyer.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/stage10/endpoints-bitflyer.md) で定義する
- Stage10 では inventory の `PresentIn` と `Note` は引き継がない
- Stage10 の対応 API 一覧は、inventory を入力として Stage10 側で新たに書き起こす
- Stage10 で日常運用する endpoint 正本は `stage10/endpoints-bitflyer.md` とし、旧 inventory は import source に留める
- Stage10 では inventory に記載された endpoint を `Protocol` / `Native` の 2 層へ再配置する前提で扱う
- 初期実装対象は `GetTicker`、`GetBalance`、`SendChildOrder` の 3 endpoint に限定する

### 3.4 層の基本役割

- `Protocol`
  - DTO を持たない
  - HTTP 実行、認証、署名、debug logging / diagnostics、baseUri、transport 設定を担う
  - 単独で使用可能とする
  - 公開 client 面は facade とし、endpoint ごとの送信ロジックは独立 endpoint module へ切り出してよい
- `Native`
  - bitFlyer-native DTO を持つ
  - request 側では internal `Encoder` を用いて request validation、request marshalling、必要な serialize を行う
  - すべての endpoint は、`Native` が `Protocol` の endpoint-level API を利用して解決する
  - `Native` は request shaping を内部 `Encoder` に閉じ込めた上で、`Protocol` response を受けて `JSON検証変換 -> 意味変換 -> 意味検証` を担い、transport 実行責務は `Protocol` に残す
  - response 側では `Protocol` の raw JSON text に対して、`JSON検証変換 -> 意味変換 -> 意味検証` の順に処理して bitFlyer-native DTO を返す
  - `JsonConverter` は主に `JSON検証変換` と `意味変換` の段階で用い、再取得・再試行・fallback は持たない
  - 公開 client 面は facade とし、endpoint ごとの request / response 契約と実行ロジックは独立 endpoint module へ切り出す
  - `Encoder`、`JsonValidation`、`Conversion`、`MeaningValidation`、`Errors` は論理責務として維持する
  - 物理構成は endpoint-first を優先し、endpoint 固有実装は `Public/Endpoints/<Endpoint>/` または `Private/Endpoints/<Endpoint>/` に寄せ、複数 endpoint で共有する helper だけを `Internal/Shared/` に置く

### 3.5 実行基盤の集中

- 認証は全層で共通して使えるようにする
- ただし、各層が独自に認証を持つのではなく、同一の `Protocol` 実体を共有する形で達成する
- 上位層は認証・署名・transport を再定義しない
- Private/Public の差は、`Protocol` 実行基盤が持つ資格情報・署名能力の有無で表現する
- `Public` / `Private` の責務は各層で分離する
- ただし、公開 client 面は `PublicXxxClient` / `PrivateXxxClient` を層ごとに乱立させず、bundle 形でまとめる
- Stage10 では API 機能単位で公開 facade を分割しない
- facade の内側は endpoint module 単位で分割してよい
- shared helper は `Internal/Shared/` へ、endpoint 固有実装は各 endpoint フォルダへ寄せる
- `Private` runtime は `Public` runtime を包含し、認証付き構成では `Public` と `Private` の両方にアクセスできる
- `BaseUri` は `Protocol` runtime の必須構成とする
- `TransportConfig` は `Protocol` runtime の必須構成とする
- `HttpPolicy` は Stage10 当初方針では採用しない
- 再送、rate limit、circuit breaker、fallback などの回復戦略は `Protocol` に持たせない
- 1 回の送信をどう再実行するかは、外部オーケストレーション側で明示的に制御する

### 3.6 Protocol runtime の必須構成

#### 3.6.1 BaseUri

- `BaseUri` は「どこへ送るか」を定義する
- `BaseUri` は `Protocol` client 生成時に必ず解決済みでなければならない
- 利用者入力としては省略可能でもよいが、その場合は bitFlyer 用既定値へ解決されなければならない
- `BaseUri` は host / root path の決定までを責務とし、endpoint 選択や業務意味は持たない

#### 3.6.2 TransportConfig

- `TransportConfig` は「何で送るか」と「誰が送信資源を所有するか」を定義する
- `TransportConfig` は `Protocol` client 生成時に必ず解決済みでなければならない
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
- `Native` は bitFlyer API の入力 field 集合を鏡像化した request DTO を受け取る
- `Native` の公開 API 面は path / query / body / headers を直接構築しない
- `Native` は internal `Encoder` を通じて `Protocol` endpoint-level API へ渡す request 材料を構築する
- `Encoder` は `Native` の内部実装であり、公開層は構成しない
- query / path / body の組み立て、body JSON の serialize、request DTO の検証は `Native` の `Encoder` に閉じ込める
- すべての endpoint で `Native` は `Protocol` endpoint-level API を利用し、transport 実行は `Protocol` に残す
- request 側では `null` を「未指定」として扱う
- `null` の request パラメータは、`Encoder` が query / body / path 上の項目として出力しない
- 必須項目が `null` の場合は省略して送らず、`Semantic` error とする
- bitFlyer API 仕様が明示的に `null` 送信を要求する場合のみ、endpoint 個別例外を許容する
- `Protocol` は transport request だけを受け取る
- `Protocol` が受け取るのは method / path / query / body / headers / endpoint identity などの transport 情報であり、上位層 DTO ではない

#### 3.7.2 レスポンス境界

- レスポンスは下位層から上位層へ向かう
- `Protocol` は transport response を返す
- `Protocol` が返すのは status / headers / raw JSON text などの transport 結果であり、DTO 化は行わない
- `Native` は `Protocol` の raw JSON text を `TEXT -> JSON検証変換 -> 意味変換 -> 意味検証` の順に処理して、bitFlyer-native DTO を返す
- `JSON検証変換` は raw JSON text を JSON object として読めるかを確認し、decode 開始可能な形へ変換する
- `意味変換` は JSON object を bitFlyer-native DTO 候補へ落とす
- `意味検証` は bitFlyer-native DTO が公開契約として成立しているかを確認する
- `Native` は公開レスポンスとして中間 JSON object を露出しない

#### 3.7.3 逆流禁止

- `Protocol` が `Native` request / response を知ることは禁止する
- `Native -> Protocol` の逆戻り制御を持たせない
- 変換後にエラーが出た場合に、上位層内部で下位層へ戻って再取得する処理は持たせない
- 変換後の回復戦略、再実行、別経路取得、fallback は外部で明示的に扱う

#### 3.7.4 エラー分類の基本方針

- Stage10 では、新しい独自分類を追加する前に、既存 `CallErrorKind` を基底分類として引き継ぐ
- 基底分類は `Transport` / `Http` / `Codec` / `Mapping` / `Semantic` / `Unknown` の 6 種とする
- この基底分類は「どの段階で失敗したか」を表す
- 認証、rate limit、request 不正、server 異常などの運用上の意味分類は、基底分類とは別軸の補助情報として扱う
- `Unknown` は最後の退避先であり、既知の失敗パターンを安易に `Unknown` へ逃がさない

#### 3.7.5 層ごとのエラー確定責務

- `Protocol`
  - 接続失敗、タイムアウト、キャンセル、TLS、送信失敗などの transport レベル失敗のみを `Transport` として扱う
  - `Protocol` は HTTP status を見て `Http` へ変換しない
  - `Protocol` 単独利用時は、利用者が status / headers / raw body を直接見る
- `Native/JsonValidation`
  - `Protocol` response の status が非 `2xx` の場合は `Http` を確定する
  - raw JSON text の parse 失敗、JSON shape decode 失敗は `Codec` を確定する
- `Native/Conversion`
  - bitFlyer 値表現から native DTO / native value へ落とす過程の失敗は `Mapping` を確定する
  - `JsonConverter` は主にこの段階で bitFlyer 値表現から native value への変換に用いる
  - 例: 未知の enum 値、想定外の値型、symbol / product_code / market の変換不能
- `Native/MeaningValidation`
  - native DTO が公開契約として成立しない場合は `Semantic` を確定する
- `Native/Encoder`
  - request 不足、値範囲不正、引数組み合わせ不正、上位 API 契約違反は `Semantic` を確定する
- `Native/Public/` と `Native/Private/`
  - 公開 API 面として internal pipeline を束ねるが、変換・検証ロジック本体の置き場にはしない
- Stage10 では、変換後エラーを契機に `Native` 内部で `Protocol` を再実行しない

#### 3.7.6 エラー情報の保持方針

- 全エラーで `EndpointId` と発生層を追跡できることを前提とする
- `Http` では `HttpStatus` を保持する
- `Http` と `Codec` では、失敗時診断のために `BodySnippet` を保持してよい
- `BodySnippet` は error 情報に限定し、success 時の raw diagnostics としては扱わない
- 取引所固有 `error_code` や運用カテゴリを特定できる場合は、基底分類とは別軸の補助情報として保持してよい
- 将来 MCP や外部公開面へ接続する場合も、まず基底分類を保ち、その上に表示用分類を重ねる

#### 3.7.7 Call 型の扱い

- Stage10 の公開 API は、`Protocol` / `Native` のいずれも `Call` を唯一の返却形式とする
- DTO や `WireResponse` を単体で直接返さない
- `Protocol` の主公開 API は facade 経由の endpoint-level API とし、各 endpoint-level API は `Task<Call<WireCallSpec, WireResponse>>` を返す
- Stage10 では shared transport project の既存型名として `WireCallSpec` / `WireResponse` を暫定流用してよい
- ただし、これは shared transport 語彙の再利用にすぎず、Stage10 の設計正本が `Wire` であることを意味しない
- Stage10 の層判断、責務判断、公開面判断は `Protocol` を正本語彙として行う
- `WireCallSpec` を直接受ける汎用 `SendAsync(...)` を持つ場合、それは internal / debug / 補助面として扱い、主公開面にはしない
- `Call.Request` に保持する `WireCallSpec` は、署名前の canonical request とし、公開契約として扱う
- `WireCallSpec.Headers` は原則空とし、header を公開契約へ持ち込む必要がある場合のみ allowlist 方式で保持してよい
- `WireCallSpec` に保持してよい header の初期 allowlist は `Content-Type` と `Accept` に限定する
- 署名値、`Authorization` header、nonce、timestamp、body hash などの認証情報は `WireCallSpec` に含めない
- 認証・署名に必要な付加情報は `Protocol/Internal/Auth` が実行直前に付与し、公開 `Call.Request` へは露出しない
- `Native` の公開 API は `Task<Call<TRequest, TResponse>>` を返す
- `Call` は、成功/失敗、対応する request/response、実行メタ情報を一体として表現する
- `Call.Request` には、その層の公開 request を保持する
- `Call.Result` は `CallResult.Ok` または `CallResult.Err` のいずれかとする
- `Call.Meta` は `Layer`、`Component`、`EndpointId` を保持し、必要に応じて `Tags`、`Children` を保持してよい
- `CallMeta.EndpointId` は観測用途に限定し、仕様判断、業務分岐、対応可否判定の根拠に使わない
- `EndpointId` は `CallMeta` に置く
- `HttpStatus` と `BodySnippet` は `CallError` に置く
- `CallMeta.RawJson` は Stage10 の公開契約としては前提にしない
- raw response の直接観測が必要な場合は、`WireResponse` または `Protocol` 単独利用で扱う
- 通常の成功/失敗は `CallResult` で表現し、throw はプログラミングエラー、設定不備、プロセス継続不能な内部不整合に限定する

#### 3.7.8 CallMeta の語彙と運用粒度

- `CallMeta.Layer` は粗い責務層だけを表し、Stage10 では `Protocol`、`Native`、`Composition`、`Tests` を基本語彙とする
- `CallMeta.Layer` に endpoint 名、internal stage 名、詳細 component 名を混在させない
- `CallMeta.Component` は call を返した責務単位を表し、Stage10 では `PublicFacade`、`PrivateFacade`、`PublicEndpointModule`、`PrivateEndpointModule`、`Transport`、`Factory`、`Bootstrap` を基本語彙とする
- `CallMeta.Component` に endpoint 名を重複して入れず、endpoint 固有性は `EndpointId` にのみ持たせる
- facade が endpoint module の結果をそのまま返す場合、`Component` は module 側を優先し、facade 側で上書きしない
- `CallMeta.EndpointId` は canonical endpoint id を保持し、観測用途に限定する
- `CallMeta.Tags` は補助情報に限定し、低カーディナリティな語彙だけを入れる
- Stage10 で許容する `Tags` の初期語彙は `Scope`、`Stage`、`Auth`、`Retryable` とする
- `Tags[\"Scope\"]` は `Public` / `Private` を取る
- `Tags[\"Stage\"]` は `Encoder` / `JsonValidation` / `Conversion` / `MeaningValidation` を取る
- `Tags[\"Auth\"]` は `None` / `Required` を取る
- `Tags[\"Retryable\"]` は `true` / `false` を取る
- `CallMeta.Children` は orchestration や fan-out がある場合のみ使い、単純な 1 対 1 の `Native -> Protocol` 呼び出しでは必須にしない
- `CallMeta.InternalEndpointId` は endpoint 非対応の内部 call にのみ使い、公開 endpoint の代用にしない

#### 3.7.9 Debug Logging / Diagnostics の方針

- Stage10 の logging / diagnostics は、運用監査ログではなく debug と live test 証跡のための local trace として扱う
- Stage10 では既存 `IRestClientLogger` / `IRestCallObserver` の sanitize 前提設計を、そのまま公開契約として継承しない
- 安全性は event の sanitize / mask ではなく、出力先制約で担保する
- debug log の出力先はローカルファイルに限定し、`stdout`、remote sink、telemetry export、CI artifact へ直接出力しない
- request header は常に記録対象外とし、debug log に含めない
- `OperationId` を必須とし、少なくとも `WireRequest`、`ProtocolCall`、`NativeCall` を同一 `OperationId` で相関できるようにする
- `Protocol` は送信前に request text を、送信後に `Call<WireCallSpec, WireResponse>` 相当の結果を記録してよい
- `Native` は native contract 確定後に `Call<TRequest, TResponse>` 相当の結果を記録してよい
- `Protocol` 側の debug log は text / transport 結果を正本とし、`Native` 側の debug log は request DTO / response contract / error を正本とする
- sanitize / mask は debug log 生成時には行わない
- artifact が必要な場合は、ローカル debug log から別工程で mask 後に生成する
- logging / diagnostics の具体 sink 実装は初手では no-op でもよいが、後続実装は本方針に従う
- logging / diagnostics の失敗は API 呼び出し本体を失敗させる理由にしない

### 3.8 下位層アクセス

- `Native` client は `Protocol` client へアクセスできる
- ただし、この下位層アクセスは外部利用者の明示的制御のために提供するものであり、
  上位層内部での暗黙 fallback を正当化するものではない

### 3.9 Raw 層の扱い

- Stage10 当初方針では `Raw` 層を公開層として扱わない
- request encoding / response decode / JSON parse は `Native` 配下の internal codec として持ってよい
- `Raw` 相当の中間表現は external client 面へ露出しない
- 既存 `Raw` 実装は、Stage10 設計の試算材料・移行材料として利用してよい

### 3.10 Contract 層の扱い

- Stage10 当初方針では `Contract` 層を新仕様の構成要素として扱わない
- `Contract` は bitFlyer 専用設計が固まるまで設計対象外とする
- 取引所横断 DTO / 取引所横断 client / 取引所横断 capability は Stage10 第1段階の対象外とする
- 将来再導入する場合でも、bitFlyer 内部で固めた責務の上に後付けする

### 3.11 Native DTO の安定方針

- Stage10 の最終目標は、bitFlyer 用 `Native DTO` 全体を安定公開契約として固定することである
- そのため、`Native DTO` は最終的に将来の MCP や外部公開面へ接続可能な意味契約へ収束させる
- 最終固定形の `Native DTO` は、bitFlyer API の返却フィールド集合を正本とした鏡像であることを原則とする
- 最終固定対象の `Native` response contract は object DTO だけでなく top-level collection 契約を含みうる
- bitFlyer API の response が top-level array の endpoint では、synthetic wrapper を作らず `IReadOnlyList<T>` を最終固定形としてよい
- 最終固定形では、bitFlyer API の返却 field 名に対して PascalCase 化、型変換、nullable 化、property-based immutable 化のみを許容する
- bitFlyer API の返却 field に存在しない補助情報、raw diagnostics、意味補完による rename は最終固定形へ持ち込まない
- ただし移行期間中は、先に安定形へ到達した DTO を `Stable Core DTO`、見直し前の DTO を `Transitional DTO` として区別してよい
- `Stable Core DTO` / `Transitional DTO` の区別は移行概念であり、最終状態では `Native DTO` 全体固定へ収束させる
- Stage10 の `Native` 公開契約では、`RawSnapshot`、`Extras`、`RawJson` などの raw / diagnostics 情報をサポートしない
- success 時の raw 観測や lossless 保持が必要な場合は、`Native` ではなく `Protocol` を直接利用する

#### 3.11.1 Breaking Change の扱い

- 最終的に固定対象とする `Native DTO` では、以下を breaking change として扱う
  - 型名変更
  - プロパティ名変更
  - プロパティ型変更
  - プロパティ削除
  - optional だったプロパティの必須化
  - 同名プロパティの意味変更
- 移行期間中も、`Stable Core DTO` へ昇格したものには同じ breaking 規則を先行適用する
- optional な新規プロパティ追加を non-breaking として扱いたい場合、公開 CLR 形状は「プロパティ集合」を優先し、constructor 署名の変化を公開契約に含めない

#### 3.11.2 DTO 形状の方針

- 最終固定対象の `Native DTO` は primary-constructor record を採らない
- 理由は、`public sealed record Xxx(...)` では constructor 引数列、引数順、`Deconstruct(...)` が公開契約に含まれ、後からの optional プロパティ追加でも CLR 的に breaking になりやすいため
- 最終固定対象の `Native DTO` は property-based immutable type を基本とする
- 第1候補は `sealed class` + `init` property とし、同等に constructor / deconstruct を公開契約へ過剰に含めない形であれば許容する
- 最終固定対象の `Native DTO` の必須性は public constructor ではなく、`MeaningValidation` 完了時点で内部的に確定してから DTO 化する
- `Transitional DTO` では既存 record 形を暫定利用してよいが、固定対象へ昇格させる時点で公開形状を見直す

#### 3.11.3 Naming Rule の方針

- 最終固定対象の `Native DTO` のプロパティ名は、bitFlyer API が返すフィールド名を唯一の語源とする
- 公開名は raw field 名をそのまま露出せず、PascalCase へ正規化して用いる
- 意味補完のために別語彙へ改名しない
- 例:
  - `product_code` -> `ProductCode`
  - `tick_id` -> `TickId`
  - `best_bid` -> `BestBid`
  - `child_order_acceptance_id` -> `ChildOrderAcceptanceId`
  - `exec_date` -> `ExecDate`
- `RawJson`、`RawSnapshot`、`Extras` は bitFlyer API の返却フィールド名由来ではないため、最終固定対象の `Native DTO` の naming rule には含めない
- 上記の情報は `Native DTO` の naming 例外として残さず、`Native` 公開契約では非サポートとする
- `GetAddressesResponse(FreeText RawJson)` のように、意味 DTO ではなく raw payload 保持を主目的とするものは、そのまま固定対象にしない

#### 3.11.4 Nullability Rule の方針

- 最終固定対象の `Native DTO` では、`null` は「値が存在しない / API が返していない」場合にのみ使う
- `Closed<T>` は「値はあるが語彙が未知」という別概念であり、最終固定対象の `Native DTO` には原則出さない
- unknown 値は `Closed<T>` で保持するのではなく、`Conversion` または `MeaningValidation` で `Mapping` error / `Semantic` error として確定する
- `Empty` sentinel は最終固定対象の `Native DTO` に出さず、内部 parse / helper に限定する
- API が実際に空文字を返す項目で、その空文字自体に意味がある場合のみ `""` を公開値として保持してよい
- `ProductCode`、`AcceptanceId`、`ExchangeOrderId`、enum 相当値など、code / id / closed vocabulary 系の項目では `""` を `null` や unknown の代用品にしない
- exposed DTO では `null` と `Closed<T>` と `Empty` を混在させない
- request 側では `null` は「未指定」を意味し、response 側では「値が返っていない」を意味する

#### 3.11.5 JSON Serializable Rule の方針

- 最終固定対象の `Native DTO` は、`JsonSerializer.Serialize(dto)` が既定設定で通ることを必須条件とする
- 呼び出し側に専用 `JsonSerializerOptions` や custom converter 登録を要求しない
- 最終固定対象の `Native DTO` は serializer-native な公開形を優先し、`string` / `decimal` / `bool` / `DateTimeOffset` / `IReadOnlyList<T>` / `IReadOnlyDictionary<string, T>` など、既定 serializer で安定して扱える型を基本とする
- `RawJson`、`RawSnapshot`、`Extras` などの diagnostics / lossless payload は `Native DTO` 本体に含めず、`Native` 公開契約ではサポートしない
- `IReadOnlyDictionary<FreeText, JsonElement>` のような custom key 型 dictionary は最終固定対象の `Native DTO` で採用しない
- key/value の追加保持が必要な場合は、`Native` ではなく `Protocol` を通じて raw response を参照する
- `Price`、`Size`、`ProductCode`、`AcceptanceId` などの domain wrapper は、最終固定対象の `Native DTO` では原則として `decimal` / `string` などの serializer-native scalar へ寄せる
- 既存 wrapper を内部表現として保持することは許容するが、最終固定対象の `Native DTO` の公開形には持ち込まない
- `Transitional DTO` では既存 wrapper や raw diagnostics 混在を暫定利用してよいが、固定対象へ昇格させる時点で serializer-native な公開形へ再設計する

### 3.12 Request DTO の方針

- `Native` の request DTO は、bitFlyer API の入力フィールド集合を正本とした鏡像であることを原則とする
- request DTO のプロパティ名は、bitFlyer API の入力 field 名を唯一の語源とし、PascalCase 化して用いる
- request DTO に対して許容する変換は、PascalCase 化、型変換、nullable 化、property-based immutable 化に限定する
- request DTO は path / query / body の transport 配置を表現しない
- path / query / body への配置決定は `Native` の `Encoder` が担う
- request 側の `null` は「未指定」を意味し、`Encoder` はその項目を出力しない
- 必須項目が `null` の場合は request DTO のまま送らず、`Semantic` error として確定する
- bitFlyer API 仕様が明示的に `null` 送信を要求する場合のみ、endpoint 個別例外を許容する
- 入力項目が存在しない endpoint でも、`Call<TRequest, TResponse>` の request 側契約を揃えるため、空 request DTO を持ってよい

---

## 4. 目標 client モデル

### 4.1 生成単位

- `CreateProtocolClient(...)`
  - `Protocol` のみを切り出して使用する
  - `Protocol` bundle を返し、`Public` 面を必須で持つ
- `CreateNativeClient(...)`
  - `Protocol` + `Native` を切り出して使用する
  - `Native` bundle を返し、`Native.Public` と `Protocol` へアクセスできる

### 4.2 Public / Private 公開面

- 各層の内部責務は `Public` / `Private` で分離する
- ただし、factory と top-level client は層ごとに `Public` / `Private` を完全二重化しない
- Stage10 の公開 client 面は API 機能別には分割しない
- facade は endpoint 別 module へ薄く forward するだけに留める
- 公開面の第一分割軸は `Protocol` / `Native`、第二分割軸は `Public` / `Private` とする
- `bundle.Public` は常に利用可能とする
- `bundle.Private` は認証情報を持つ `Protocol` runtime が構築できる場合にのみ利用可能とする
- `Private` 側を持つ bundle でも、`Public` 面は同じ runtime 共有のまま利用可能とする

### 4.3 共有物

- `CreateNativeClient(...)` は、内部で使う `Protocol` 実体を公開できること
- 認証、署名、transport、および有効化された debug logging / diagnostics 設定は、同一 runtime を共有する
- `BaseUri` と `TransportConfig` は、同一 `Protocol` runtime を識別する中核構成とする

### 4.4 Composition の扱い

- `Composition` は論理 2 層には含めない
- `Composition` は client runtime の組み立てと共有資源の配線だけを担う
- `Composition` は変換責務を持たない

### 4.5 Protocol endpoint-level API 署名方針

- `Protocol` の主公開面は facade 経由の endpoint-level API とする
- 各 endpoint-level API は `Task<Call<WireCallSpec, WireResponse>>` を返す
- `Protocol` の endpoint-level API は transport-ready な scalar / primitive 引数だけを受ける
- `Protocol` の endpoint-level API は `Native` の request DTO 型を受けない
- `Protocol` の endpoint-level API は response DTO を返さず、必ず `WireResponse` を返す
- `WireCallSpec` を直接受ける汎用 `SendAsync(...)` を持つ場合、それは internal / debug / 補助面に限定する
- facade 本体には endpoint 固有ロジックを置かず、実体は endpoint module に委譲する
- 初期 3 endpoint の署名方針は次の通りとする
  - `GetTickerAsync(string? productCode = null, CancellationToken ct = default)`
  - `GetBalanceAsync(CancellationToken ct = default)`
  - `SendChildOrderAsync(string bodyJson, CancellationToken ct = default)`
- `SendChildOrderAsync(...)` の `bodyJson` は `Native` の `Encoder` が生成する

### 4.6 Endpoint Matrix の方針

- Stage10 用の endpoint 一覧は、inventory を入力として `stage10/endpoints-bitflyer.md` に切り出す
- `stage10/endpoints-bitflyer.md` を Stage10 の endpoint 運用正本とし、旧 inventory は import source に留める
- 一覧は 1 endpoint 1 行を原則とする
- 最低限の列は `EndpointId`、`Method`、`Path`、`Scope`、`ExposeInProtocol`、`ExposeInNative`、`LiveTestPhase`、`RequestDtoStatus`、`ResponseDtoStatus` とする
- `DTOStatus` は request / response で意味が異なるため、単一列ではなく `RequestDtoStatus` と `ResponseDtoStatus` に分ける
- `RequestDtoStatus` と `ResponseDtoStatus` は、少なくとも `Transitional` / `Fixed` を取る
- 初期 3 endpoint 以外は、当初 `ExposeInNative = No`、`LiveTestPhase = Later` として明示してよい
- endpoint matrix は Stage10 の実装対象、DTO 固定対象、live test 対象を同時に管理するための正本とする
- `stage10/endpoints-bitflyer.md` は Stage10 第1段階で作成する成果物とする

#### 4.6.1 DTO Status の昇格条件

- `RequestDtoStatus = Fixed` は、API 入力 field 鏡像、naming / null / omission 規則固定、`Encoder` による path / query / body 配置固定、対応 unit test 通過を満たしたときに限る
- `ResponseDtoStatus = Fixed` は、API 返却 field 鏡像、naming / nullability / JSON serializable 規則固定、raw diagnostics 非依存、`JsonValidation -> Conversion -> MeaningValidation` の境界固定、対応 test 通過を満たしたときに限る
- live test 成功は強い証跡として扱うが、`Fixed` 判定の必須条件にはせず、`LiveTestPhase` と別軸で管理する

### 4.7 Runtime 所有権と Dispose 方針

- shared runtime の owner は bundle のみとする
- `CreateProtocolClient(...)` は `ProtocolBundle` を返し、`ProtocolBundle` が runtime を所有する
- `CreateNativeClient(...)` は `NativeBundle` を返し、`NativeBundle` が runtime を所有する
- bundle 配下の `Protocol` / `Native` client は runtime の view であり、個別に `Dispose` しない
- `ManagedHttp` では bundle が内部生成した送信資源を破棄する
- `ExternalHttpClient` と `ExternalTransport` では、送信資源の所有権と破棄責務は呼び出し側に残す
- runtime の所有権は 1 つに固定し、二重 dispose や所有権逆転を許容しない
- runtime ownership 自体は Stage10 で確定事項とし、未確定として残さない

### 4.8 参照ガード方針

- project 境界と namespace 境界の両方で依存方向を固定する
- `Vocabulary` は `Primitives` まで、または完全独立とする
- `Protocol` は `Transport`、`Primitives`、`Vocabulary` にのみ依存する
- `Native` は `Protocol`、`Primitives`、`Vocabulary` に依存する
- `Composition` は `Protocol`、`Native`、既存 `Composition`、`Primitives` に依存する
- `Protocol` から `Native` への参照は禁止する
- `Composition` から `*.Internal.*` への参照は禁止する
- `Native/Public` と `Native/Private` は、公開面に `WireResponse`、`JsonElement`、raw diagnostics 型を露出しない
- `Native/JsonValidation -> Conversion -> MeaningValidation` の response pipeline は一方向依存のみを許容する
- 参照ガードは、文書ルールだけでなく arch test による機械検証を前提とする
- Stage10 用の機械検証は `tests-stage10/Bitflyer/Architecture.Tests` を追加候補とし、project reference、公開面 forbidden type、namespace forbidden dependency を対象にする
- `project reference` では `Protocol -> Transport, Primitives, Vocabulary`、`Native -> Protocol, Primitives, Vocabulary`、`Composition -> Protocol, Native, ExchangeApi.Composition, Primitives` 以外を拒否する
- `public surface forbidden type` では `Native.Public/Private` の公開面に `WireResponse`、`WireCallSpec`、`JsonElement`、`JsonDocument`、`HttpRequestMessage`、`HttpResponseMessage`、raw diagnostics 型が露出しないことを検査する
- `namespace forbidden dependency` では `Protocol -> Native`、`Composition -> *.Internal.*`、response pipeline の逆参照を禁止する

### 4.9 初期 3 endpoint の具体化方針

- 初期実装対象の `GetTicker`、`GetBalance`、`SendChildOrder` については、抽象規則だけでなく具体契約も Stage10 文書で固定する
- 各 endpoint で最低限固定する内容は、`Protocol` 署名、`Native` request DTO、`Native` response DTO、`CallMeta`、`Encoder` の責務とする
- `GetTicker`
  - `Protocol.Public.GetTickerAsync(string? productCode = null, CancellationToken ct = default)`
  - `Native` は request validation 後に `Protocol.Public.GetTickerAsync(...)` を呼び、response を `JsonValidation -> Conversion -> MeaningValidation` で native contract へ落とす
  - canonical path は `/v1/getticker` とし、`/v1/ticker` は `Protocol` 内部の互換 alias path としてのみ扱ってよい
  - `GetTickerRequest` は `ProductCode?` を持つ request DTO とし、`null` の場合は query の `product_code` を送らず bitFlyer 既定値 `BTC_JPY` に委ねる
  - `GetTickerResponse` は `ProductCode`、`State`、`Timestamp`、`TickId`、`BestBid`、`BestAsk`、`BestBidSize`、`BestAskSize`、`TotalBidDepth`、`TotalAskDepth`、`MarketBidSize`、`MarketAskSize`、`Ltp`、`Volume`、`VolumeByProduct` を持つ
- `GetBalance`
  - `Protocol.Private.GetBalanceAsync(CancellationToken ct = default)`
  - `Native` は空 request DTO を受け、response を native contract へ落とす処理だけを担う
  - `GetBalanceRequest` は空 request DTO に固定する
  - `GetBalance` の `Native` response contract は `GetBalanceResponse` wrapper を作らず、top-level array 契約として `IReadOnlyList<GetBalance.Item>` を返す
  - `GetBalance.Item` は `CurrencyCode`、`Amount`、`Available` を持つ
- `SendChildOrder`
  - `Protocol.Private.SendChildOrderAsync(string bodyJson, CancellationToken ct = default)`
  - `Native` の `Encoder` が request DTO を body JSON に serialize し、その結果を `Protocol.Private.SendChildOrderAsync(...)` へ渡す
  - `SendChildOrderRequest` は `ProductCode`、`ChildOrderType`、`Side`、`Size`、`Price?`、`MinuteToExpire?`、`TimeInForce?` を持つ
  - `Price` は `ChildOrderType = LIMIT` のとき必須、`MARKET` のときは未指定とする
  - `MinuteToExpire` と `TimeInForce` は省略可能とし、`Encoder` は未指定時に body へ出力せず bitFlyer API の既定値に委ねる
  - `SendChildOrderResponse` は `ChildOrderAcceptanceId` のみを持つ
- `SendChildOrder` は request encode が最も強く現れる endpoint として、Stage10 の request 契約・encoder 契約の基準例とする

---

## 5. 取引所内共通化と取引所横断共通化

### 5.1 Stage10 で扱うもの

- bitFlyer の `Protocol` 実行基盤
- bitFlyer の `Native DTO`
- `Protocol` transport response から `Native` DTO への internal decode / `JsonConverter` ベース変換
- `Native` request から `Protocol` request への internal encode
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
- `Protocol` を単独で使った低レベル検証やデバッグが可能になる
- 公開 client 面が `Protocol` / `Native` に絞られ、利用導線が明確になる
- request encode / response decode を公開層から外せるため、client 面の肥大化を抑えやすい
- 認証や transport 設定が `Protocol` に集約され、実行条件の差異が減る
- 上位層で暗黙 retry/fallback を持たないため、挙動の追跡がしやすい
- 回復戦略を外部へ出すことで、再送条件と再送回数を利用側で明示制御できる

---

## 7. この方針のデメリット / リスク

- 当初は bitFlyer 専用となるため、Bittrade との parity は一旦後退する
- `Contract` 廃止により、取引所横断利用の既存導線は Stage10 の主対象から外れる
- 将来の再抽象化時に、再度境界整理が必要になる
- `Protocol` に責務を集約しすぎると肥大化しやすい
- 中間の公開観測点が減るため、bitFlyer ネイティブ JSON を client 面から直接確認しにくくなる
- request encoder / response decoder を internal で強く管理しないと、`Native` が肥大化しやすい
- 共有 runtime の所有権、dispose、ライフサイクル設計を先に決めないと破綻しやすい

---

## 8. 第1段階のスコープ

### In Scope

- 本書で bitFlyer 専用の新しい層モデルを固定する
- `Protocol` / `Native` client の責務と依存方向を固定する
- `Raw` 層を公開層から外し、internal codec として扱う方針を固定する
- `Contract` を Stage10 対象から外すことを明記する
- Stage10 の展開方法として案 A を採用することを明記する
- 認証・署名・transport を `Protocol` へ集約する方針を固定する
- `Native` を transport を持たない request / response 契約層として定義し、「下位層へ戻らない」原則を固定する
- `Composition` の位置づけを「配線のみ」として固定する
- Stage10 向け live test の配置方針と初期スコープを固定する
- 初期実装 endpoint を `GetTicker`、`GetBalance`、`SendChildOrder` に固定する
- 既存コードを使って、実装可能性と主要な衝突点を洗い出す

### Out of Scope

- Bittrade への同時適用
- 取引所横断 DTO / client の設計
- `Contract` 層の維持・再設計
- 既存文書との整合完了
- 新仕様への全面移行
- POST を含む live test の全面展開
- 初期実装対象以外の endpoint 実装
- stage10 以外の文書更新

---

## 9. 採用する展開案（案 A）

### 9.1 方針

- Stage10 は専用ブランチで進める
- 既存 `src/Exchanges/Bitflyer/*` を直接崩さず、別配置で新実装を並行構築する
- root に残す Stage10 文書は `stage10.md` のみとし、補助文書は `stage10/` フォルダに集約する
- 新実装が固まるまで、既存実装は比較対象・回帰対象として維持する

### 9.2 採用ブランチ

- 作業は Stage10 専用 branch で進める
- 既存の `stage10` 系 branch を継続利用してよく、専用 branch を新設するかは運用都合で決めてよい

### 9.3 採用ディレクトリ案

- 文書:
  - `stage10/architecture.md`
  - `stage10/runtime.md`
  - `stage10/dto-stability.md`
  - `stage10/migration.md`
  - `stage10/endpoints-bitflyer.md`
- 新コード:
  - `src-stage10/Bitflyer/Vocabulary`
    - `EndpointIds.cs`
  - `src-stage10/Bitflyer/Protocol`
    - `ExchangeApi.Stage10.Bitflyer.Protocol.csproj`
    - `Public/`
      - `Api/`
        - facade interface / facade implementation
      - `Endpoints/`
        - `<EndpointName>/`
          - `*ProtocolEndpoint.cs`
    - `Private/`
      - `Api/`
        - facade interface / facade implementation
      - `Endpoints/`
        - `<EndpointName>/`
          - `*ProtocolEndpoint.cs`
    - `Internal/Auth/`
    - `Internal/Runtime/`
    - `Internal/Shared/`
  - `src-stage10/Bitflyer/Native`
    - `ExchangeApi.Stage10.Bitflyer.Native.csproj`
    - `Public/`
      - `Api/`
        - facade interface / facade implementation
      - `Endpoints/`
        - `<EndpointName>/`
          - `*NativeEndpoint.cs`
          - `*Request.cs`
          - `*Response.cs`
    - `Private/`
      - `Api/`
        - facade interface / facade implementation
      - `Endpoints/`
        - `<EndpointName>/`
          - `*NativeEndpoint.cs`
          - `*Request.cs`
          - `*Response.cs`
    - `Internal/Shared/`
  - `src-stage10/Bitflyer/Composition`
    - `ExchangeApi.Stage10.Bitflyer.Composition.csproj`
    - `Bootstrap/`
    - `Factory/`
    - `Options/`
- 新テスト:
  - `tests-stage10/Bitflyer/Protocol.Tests`
    - `ExchangeApi.Stage10.Bitflyer.Protocol.Tests.csproj`
  - `tests-stage10/Bitflyer/Native.Tests`
    - `ExchangeApi.Stage10.Bitflyer.Native.Tests.csproj`
  - `tests-stage10/Bitflyer/Composition.Tests`
    - `ExchangeApi.Stage10.Bitflyer.Composition.Tests.csproj`
  - `tests-stage10/Bitflyer/LiveTests`
    - `ExchangeApi.Stage10.Bitflyer.LiveTests.csproj`
    - `Infrastructure/`

### 9.3.1 Project 境界

- `Protocol` project は `Transport` と `Primitives` にのみ依存する
- `Protocol` project は `Vocabulary/EndpointIds.cs` を共有してよいが、`Native` や `Composition` には依存しない
- `Native` project は `Protocol` と `Primitives` に依存する
- `Composition` project は `Protocol`、`Native`、既存 `Composition`、`Primitives` に依存する
- `Raw` は独立 project にせず、`Native/Internal` の codec 実装へ吸収する
- live test project は `Protocol`、`Native`、`Composition` を参照し、既存 live test 資産は流用候補とする

### 9.3.2 Facade + Endpoint Module の責務と物理配置

- `Protocol/Public/Api/` と `Protocol/Private/Api/`
  - facade interface と facade 実装だけを置く
  - facade は endpoint module へ薄く forward するだけに留める
  - facade 本体へ path / query / body 組み立てや送信ロジックを持ち込まない
- `Protocol/Public/Endpoints/<EndpointName>/` と `Protocol/Private/Endpoints/<EndpointName>/`
  - endpoint ごとの独立 module class を置く
  - method、path、query、body 受け渡し、`WireCallSpec` 組み立て、送信呼び出しをここへ置く
  - endpoint 固有ロジックは facade ではなく module 側へ寄せる
- `Native/Public/Api/` と `Native/Private/Api/`
  - facade interface と facade 実装だけを置く
  - facade は endpoint module を呼び出す公開導線としてのみ使う
- `Native/Public/Endpoints/<EndpointName>/` と `Native/Private/Endpoints/<EndpointName>/`
  - endpoint ごとの独立 module class を置く
  - request DTO、response DTO、native call、endpoint 固有の encode / decode / validation をこの endpoint フォルダへ寄せてよい
  - DTO は public 契約であり、endpoint module に無理に入れ子化しなくてよい
- `Native/Internal/Shared/`
  - 複数 endpoint で共有する helper だけを置く
  - 例: generic error factory、generic JSON scalar reader、共通 JSON validation helper
- `Encoder`、`JsonValidation`、`Conversion`、`MeaningValidation`、`Errors`
  - これらは論理責務として維持する
  - ただし物理配置は top-level フォルダ固定ではなく、endpoint 固有実装を endpoint フォルダへ寄せてもよい
  - response 側の読み順は `JsonValidation -> Conversion -> MeaningValidation` を維持する
- `Protocol` と `Native` の公開 client は partial 巨大化を避けるため、endpoint module への委譲を前提とする
- `Vocabulary/EndpointIds.cs` は `Protocol` と `Native` の両方が参照できる bitFlyer 共通語彙として独立配置する
- この物理構成へ収束できるなら、現行 Stage10 実装を土台として段階的に寄せてよく、全面破棄を必須にしない

### 9.3.3 既存試作の扱い

- 現行 Stage10 コードは移行材料であり、物理構成の正本ではない
- 現行 Stage10 に存在する `partial` 前提の endpoint 実装、`Api` class へ直接 endpoint 実装を生やす構成、top-level `Internal/Encoder` / `Internal/Conversion` / `Internal/MeaningValidation` の中央集約配置は、そのまま最終形へ持ち込む前提にしない
- 既存試作から流用してよいのは、transport、signer、runtime、DTO 契約、encoder / converter / validator の中身、test assertion、live test 基盤である
- 既存試作の file 配置や namespace 配置と、新しい facade / endpoint module 構成が衝突する場合は、既存配置ではなく新しい構成を優先する
- 「今ある場所に合わせて責務を説明する」のではなく、「固定した責務に合わせて既存コードを移し替える」ことを原則とする

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
- 初期実装対象は `GetTicker`、`GetBalance`、`SendChildOrder` とする
- live test は、まず read path の `GetTicker` / `GetBalance` を押さえ、その後 write path の `SendChildOrder` を導入する
- 既存 bitFlyer live test 資産は、Stage10 live test 設計の回帰資産・流用候補として扱う
- `SendChildOrder` 以外の `POST` / 注文 lifecycle live test は、`Native` request 境界の固定後に後段で導入する
- debug logging / diagnostics の具体 local-file sink は初手では no-op でもよく、初期 3 endpoint の core 実装後に追加してよい
- 必要なら白紙再設計でやり直してよいが、同一の責務境界と物理構成へ収束するなら既存 Stage10 コードを土台として再編してよい
- bitFlyer 専用設計が固まった後に、取引所横断共通化の再導入可否を判断する
- 既存文書との整合は、試作で方針が固まった後に行う

### 10.1 Facade + Endpoint Module への修正順

- Step 1: 文書を正本として固定する
  - `Protocol` / `Native`
  - facade
  - endpoint module
  - `stage10/endpoints-bitflyer.md`
- Step 2: `Protocol` の public read endpoint を 1 本だけ新構成へ移す
  - 対象は `GetTicker`
  - facade は forward だけにし、実体を `Protocol/Public/Endpoints/GetTicker/GetTickerProtocolEndpoint.cs` へ寄せる
- Step 3: `Native` の同 endpoint を新構成へ移す
  - `GetTickerRequest` / `GetTickerResponse`
  - native call
  - endpoint 固有 encode / decode / validation
  - を `Native/Public/Endpoints/GetTicker/` 配下へ寄せる
- Step 4: `GetTicker` を template として private endpoint へ展開する
  - `GetBalance`
  - `SendChildOrder`
  - `CancelChildOrder`
- Step 5: facade の constructor 肥大化を避けるため、module 集約 object を導入する
  - `PublicProtocolModules`
  - `PrivateProtocolModules`
  - `PublicNativeModules`
  - `PrivateNativeModules`
- Step 6: `Composition` を最後に寄せる
  - facade と endpoint module の境界が固まってから wiring を更新する
- Step 7: test を facade test / endpoint module test / composition test に役割分離する
- Step 8: 旧 `partial` 構成、中央集約 codec 配置、不要 helper を整理する

### 10.2 設計優先の判断基準

- 既存コードの file 配置より、`Facade + Endpoint Module` の責務境界を優先する
- 既存実装が使えるかどうかは、「そのまま残せるか」ではなく「新しい endpoint module へ安全に移せるか」で判断する
- `Composition` は最後に触る
- まず read endpoint 1 本で形を固め、その後に private / write endpoint へ広げる
- shared helper は複数 endpoint で再利用されると確認できたものだけ `Internal/Shared/` へ残す

---

## 11. DoD

- `Protocol` / `Native` の 2 層定義が文書上で明確である
- 各層のデータ表現と所有権が文書上で明確である
- `Contract` を Stage10 第1段階の対象外とすることが文書上で明確である
- 案 A の展開方針、ブランチ、文書配置、新コード配置が文書上で明確である
- Stage10 の物理構成案と project 境界が文書上で明確である
- facade と endpoint module の役割分担が文書上で明確である
- `Native` の責務と物理配置の対応が文書上で明確である
- `Vocabulary/EndpointIds.cs` を含む bitFlyer 共通語彙の配置が文書上で明確である
- `Protocol` が単独利用可能な実行基盤であることが文書上で明確である
- 認証・署名・transport が `Protocol` に集約されることが文書上で明確である
- `BaseUri` と `TransportConfig` が `Protocol` runtime の必須構成であることが文書上で明確である
- `HttpPolicy` を採用せず、回復戦略を外部で扱うことが文書上で明確である
- `Native` が transport を持たない request / response 契約層であることが文書上で明確である
- `Raw` を公開層として持たず、`Native` が internal codec と `JsonConverter` を用いて native contract へ落とすことが文書上で明確である
- 各層の request / response 境界が文書上で明確である
- response 側の `TEXT -> JSON検証変換 -> 意味変換 -> 意味検証` の 4 段階が文書上で明確であり、物理構成と対応している
- `Transport` / `Http` / `Codec` / `Mapping` / `Semantic` / `Unknown` の基底分類と、各層での確定責務が文書上で明確である
- Stage10 の公開 API が `Call` を唯一の返却形式とし、`EndpointId` / `HttpStatus` / `BodySnippet` の保持場所が文書上で明確である
- `WireCallSpec` が未署名 canonical request として公開契約に固定され、認証情報を露出しないことが文書上で明確である
- `Protocol` / `Native` の公開 client が facade、内部実装が endpoint module であることが文書上で明確である
- `CallMeta.Layer` / `Component` / `Tags` / `Children` の語彙と運用粒度が文書上で明確である
- Stage10 の debug logging / diagnostics が local-only、request header 非記録、`OperationId` 相関、artifact 後処理という方針で文書上明確である
- request DTO の naming / null / transport 配置規則が文書上で明確である
- `Protocol` endpoint-level API の主公開面と初期 3 endpoint の署名方針が文書上で明確である
- Stage10 endpoint matrix の列定義と用途が文書上で明確である
- bundle を owner とする runtime 所有権と dispose 方針が文書上で明確である
- project / namespace の参照ガード方針が文書上で明確である
- 初期 3 endpoint の具体契約が文書上で明確である
- Stage10 用 architecture test の追加候補と検査対象が文書上で明確である
- `Native DTO` 全体を最終的に固定する前提、移行中の `Stable Core DTO` / `Transitional DTO` の区別、および breaking change 規則が文書上で明確である
- 最終固定対象の `Native DTO` の naming rule が、bitFlyer API の返却フィールド名由来であることと、raw / diagnostics 情報を `Native` 公開契約でサポートしない方針が文書上で明確である
- request 側の `null = 未指定` 規則と、最終固定対象 DTO の nullability rule が文書上で明確である
- 最終固定対象の `Native DTO` が既定 `JsonSerializer.Serialize(dto)` を前提とし、serializer-native な公開形を採る方針が文書上で明確である
- 各層で `Public` / `Private` の責務を分けつつ、公開面は bundle として整理することが文書上で明確である
- 上位層が下位層へ戻って再取得・再試行しないことが文書上で明確である
- 下位層アクセス可能だが暗黙 fallback は禁止することが文書上で明確である
- 初期実装対象が `GetTicker` / `GetBalance` / `SendChildOrder` に固定されていることと、live test の導入順が文書上で明確である
- bitFlyer 専用で開始する理由と、取引所横断共通化を後段へ送る理由が文書上で明確である
- 最終構成が同一なら既存 Stage10 実装を土台として再編してよく、全面作り直しを必須としないことが文書上で明確である
- 次段階で `stage10/` フォルダへ分割する前提が明記されている

---

## 12. 現時点の未確定事項

- endpoint フォルダ内で `Encoder` / `JsonValidation` / `Conversion` / `MeaningValidation` をどこまで分けるか
- facade が依存する endpoint module 群を、constructor 引数と集約 object のどちらで束ねるか
- どの endpoint のどの `Transitional DTO` をどの順で固定対象へ収束させるか
- `ProtocolBundle` / `NativeBundle` の具体名と公開プロパティ名をどう固定するか
- `stage10/endpoints-bitflyer.md` の運用粒度をどう固定するか
- 補助的な運用分類（Auth / RateLimit / Server / Request など）をどこまで Stage10 で固定するか
- bitFlyer 専用 runtime のうち、どこまでを後に取引所横断共通化できるか
- `POST` / 注文 lifecycle の live test をどの段階で導入するか
- 既存 `Adapter` / `Contracts` をどう段階的に外すか
- 既存 factory 名と新 factory 名をどう移行するか

---

## 13. 廃止条件（Sunset）

Stage 文書（`stage*.md`）は初回リリース前の暫定文書。  
`v1.0.0` 時点で本書を `docs/archive/` へ移動し、以後の追跡は別の正本へ統合する。
