# ExchangeAPI v2.0.0 Breaking Changes Ledger

最終更新: 2026-04-23  
位置づけ: version 単位文書  
状態: draft

## 1. 目的

本書は、`v2.0.0` で検討・採用する breaking change を台帳として管理する。  
各項目は `検討中 / 採用 / 却下 / 保留` のいずれかで管理し、採用済みのものだけを最終的に現行正本へ反映する。

## 2. 記録単位

各変更は少なくとも次を持つ。

- ID
- 分類
  - `rename`
  - `remove`
  - `split`
  - `merge`
  - `contract tighten`
- 対象
- 状態
- 理由
- 利用者影響
- 関連正本
- migration 方針

## 3. 一覧

| ID | 分類 | 対象 | 状態 | 理由 | 関連正本 | migration 方針 |
| --- | --- | --- | --- | --- | --- | --- |
| BC-V2-001 | rename | `Call<TRequest, TResponse>` と `*CallAsync(...)` | 採用 | `Call` 語彙は維持しつつ、返り値型は結果コンテナとして明示し、method 名は一般的な `*Async(...)` に寄せる方が library 外部 surface として自然だから | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | `Call` を `CallResult` へ rename し、facade public method を `*Async(...)` へ rename する |
| BC-V2-002 | rename | `CallError` / `CallMeta` | 却下 | `CallResult` 採用後も `call` 概念語彙は残すため、`CallError` / `CallMeta` を無理に `ResultError` / `ResultMeta` へ寄せるとむしろ境界が曖昧になるため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | 変更なし |
| BC-V2-003 | rename | `CreateProtocolClient(...)` / `CreateNativeClient(...)` | 採用 | `Client` の利用者理解を残しつつ、返り値が bundle 単位であることも method 名で示した方が、正確性と可読性のバランスがよいため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | `CreateProtocolClientBundle(...)` / `CreateNativeClientBundle(...)` へ rename する |
| BC-V2-004 | rename | `BitflyerClientFactory` / `BinanceClientFactory` class naming | 却下 | `Factory` の役割自体は現状実装と整合し、method 名の改善だけで利用者理解に十分効くため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | 変更なし |
| BC-V2-005 | rename | endpoint `*Request` / `*ClientOptions` naming | 却下 | 現行 naming は利用者理解が十分高く、変更しても改善幅が小さい一方で波及範囲が広いため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | 変更なし |
| BC-V2-006 | contract tighten | `CallError.Kind` taxonomy | 却下 | 現行の `Transport / Http / Codec / Semantic / Mapping` は adapter と test にすでに浸透しており、変更利益より互換破壊コストが大きいため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | 変更なし |
| BC-V2-007 | contract tighten | error observation payload (`CallError` detail fields) | 採用 | `Kind` taxonomy を維持したまま解析性を上げられ、CLI/MCP が raw body 全体に頼らず要点を扱いやすくなるため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | `HttpStatusCode`, `VenueErrorCode`, `VenueErrorMessage` を additive field として追加する |
| BC-V2-008 | contract tighten | CLI / MCP での `CallError` detail field 露出 | 採用 | 追加 field をどこにも露出しないと利用者利益が薄く、逆に summary へ常時混ぜるとノイズが増えるため、verbose/details に限定するのが最も自然だから | `docs/cli.md`, `docs/mcp-server.md` | CLI は verbose 時のみ、MCP は `upstream_error.details` に optional で追加する |
| BC-V2-009 | contract tighten | `ProtocolResponse.BodyText` と `CallError` detail の境界 | 採用 | raw body と抽出 field の二重正本化を防ぎ、adapter が raw と要約 detail を混同しないようにするため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | raw の正本は `ProtocolResponse.BodyText`、`CallError` は抽出済み narrow detail のみを持つ |
| BC-V2-010 | contract tighten | scalar base contract (`decimal`, `DateTimeOffset`, invariant parse) | 却下 | 現行 `spec` と実装はすでに数量系 `decimal`、timestamp `DateTimeOffset`、culture 非依存 parse へ揃っており、v2 で横断的な再設計を掲げる利益が小さいため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | 変更なし |
| BC-V2-011 | contract tighten | Native DTO の `required / nullable` 一括 tighten | 却下 | `required / optional` は endpoint ごとの exact contract で決めるべきで、version-wide な一括 tighten を掲げると venue 文書未固定の field まで過剰に固定しやすいため | `docs/spec.md`, `docs/endpoints-bitflyer.md`, `docs/endpoints-binance.md` | 変更なし。必要な是正は endpoint 単位で扱う |
| BC-V2-012 | contract tighten | enum と string vocabulary の境界 | 却下 | 現行の venue-local enum と open-set string の切り分けは妥当で、cross-venue 共通 enum 化や string 一律 enum 化は正本をむしろ不安定にするため | `docs/spec.md`, `docs/endpoints-bitflyer.md`, `docs/endpoints-binance.md` | 変更なし |
| BC-V2-013 | split | test taxonomy / project layout の大規模再編 | 却下 | 現行の `Architecture / Protocol / Native / Composition / Live / Adapter` 分離はすでに成立しており、v2 で taxonomy 自体を壊す利益が小さいため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | 変更なし |
| BC-V2-014 | contract tighten | v2 breaking change を固定する test 追加 | 採用 | rename や additive field を ledger だけで管理すると後続 patch で後退しやすいため、public surface と adapter 出力を test で固定する必要があるため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | surface lock test を追加または更新する |
| BC-V2-015 | split | CLI / MCP surface の 1:1 統一 | 却下 | CLI は library endpoint の inspection / execution 導線、MCP は bot 向け責務単位 tool であり、surface そのものを統一すると両 adapter の役割が崩れるため | `docs/cli.md`, `docs/mcp-server.md`, `docs/mcp-tool-catalog.md` | 変更なし |
| BC-V2-016 | contract tighten | CLI / MCP の基底語彙整合 | 採用 | surface は分けたままでも、error detail、timestamp、decimal string、venue / symbol などの基底語彙は揃えた方が migration と運用が安定するため | `docs/cli.md`, `docs/mcp-server.md`, `docs/mcp-tool-catalog.md` | shared vocabulary を文書と test で固定する |
| BC-V2-017 | contract tighten | human-facing な時刻表示方針 | 採用 | `DateTimeOffset` と UTC 基準の機械契約を維持したまま、CLI / debug log / live log の可読性を上げられるため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | internal canonical は維持し、human-facing 出力だけ local with offset を優先する |
| BC-V2-018 | split | private credentials 取扱いの責務分離 | 採用 | `ExchangeAPI` core から特定の secret storage / encryption 方式を外し、auth provider 契約へ責務を寄せた方が library と運用 recipe の境界が明確になるため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | core は auth provider 契約を持ち、storage / encryption recipe は外部化する |
| BC-V2-019 | split | auth provider の具体 shape | 採用 | 高コスト provider に対応しつつ、通常利用では複雑さを露出しないためには、session 境界を明示できる shape と自動 session 導線の併用が最も収まりがよいため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md` | `IApiCredentialProvider.OpenSessionAsync(...)` 型を採用し、通常利用では client 側が session を隠蔽して扱う |
| BC-V2-020 | split | MCP read-only surface の拡張方針 | 採用 | CTradeBot の開発中確認や運用 inspection で、library にはある read-only 情報が MCP から見えない状態を減らしつつ、副作用禁止の原則を維持するため | `docs/mcp-server.md`, `docs/mcp-tool-catalog.md`, `docs/migration-v2.0.0.md` | MCP は副作用系を除く read-only 情報を原則サポートし、tool は `Core Bot Tools` と `Inspection Read Tools` に分けて管理する |
| BC-V2-021 | split | verification の物理構成と危険度分類 | 採用 | CTradeBot と近い verification 運用へ寄せつつ、ExchangeAPI 側では API 契約分類を維持したまま、手動 verification / evidence / local artifact を整理できるため | `docs/spec.md`, `docs/docs-architecture.md`, `docs/verification.md`, `docs/migration-v2.0.0.md` | API 正本の `public/private`, `read/write`, `CleanupPolicy` は維持し、verification は `safe/tolerable/dangerous` 分類で別層に整理し、evidence は `local/evidence/` へ集約する |
| BC-V2-022 | split | optional logging への将来切り出し | 保留 | core の責務を薄くし、CLI/MCP/live test/bot/local evidence など用途別に適した log writer を作れるようにする方向は妥当だが、v2 初手では credentials optional を優先するため | `docs/spec.md`, `docs/verification.md` | v2 初手では具体移動しない。将来 `ExchangeApi.Optional.Logging` として JSONL/file/redaction/evidence writer を検討する |
| BC-V2-023 | contract tighten | credential failure notification | 採用 | auth provider が失敗を typed に返しても adapter が通知しなければ利用者が原因を特定できず、逆に core が通知責務を持つと用途別 UI / MCP / CLI の境界が崩れるため | `docs/spec.md`, `docs/cli.md`, `docs/mcp-server.md`, `docs/guides/credentials-and-auth-provider.md` | `ApiCredentialException.Kind` を CLI は stderr/exit code、MCP は tool 公開制御/structured error/stderr diagnostic へ写像する |
| BC-V2-024 | split | distribution artifact shape | 採用 | core library、optional credentials、CLI executable、MCP executable の責務と配布単位を分けると、最小利用者へ不要な storage/decrypt 実装を強制せず、adapter は単体実行可能 artifact として維持できるため | `docs/distribution.md`, `docs/guides/package-publish.md`, `docs/local-nuget-consumer.md` | library / optional は NuGet package、CLI / MCP は executable release asset とし、生成物は `local/` 配下に置き git 管理しない |

## 3.0 現時点の要約

現時点で `v2.0.0` に採用している変更は、大きく次の 6 群である。

- public naming の整理
  - `CallResult`
  - `*Async(...)`
  - `Create*ClientBundle(...)`
- error observation の拡張
  - `CallError` additive detail field
  - CLI verbose / MCP details への限定露出
  - raw body の正本維持
- v2 migration lock の明示
  - breaking change を固定する test 追加または更新
- adapter 間の shared vocabulary 整合
  - surface 自体は統一しない
  - error / timestamp / numeric / venue-symbol 語彙は揃える
- private auth の責務整理
  - core は auth provider 契約を持つ
  - storage / encryption recipe は外部化する
  - `OpenSessionAsync(...)` 型を採用し、通常利用では session を隠蔽する
  - credential failure は `ApiCredentialException.Kind` で分類し、CLI/MCP が通知へ写像する
- MCP read-only surface の拡張
  - 副作用系を除く read-only 情報は原則サポート対象にする
  - tool は `Core Bot Tools` と `Inspection Read Tools` に分ける
- verification 運用の再編
  - API 契約分類と verification 分類を分ける
  - verification は `repo/local` 配置と `safe/tolerable/dangerous` で整理する
- 配布成果物の整理
  - library / optional は NuGet package とする
  - CLI / MCP は executable artifact とする
  - `ExchangeApi.Optional.Credentials` を v2 package として扱う

将来案:

- optional logging への切り出し
  - core は観測データを提供し、具体 log writer は用途別 optional に寄せる

現時点で採用していない変更は、大きく次の 4 群である。

- `CallError.Kind` taxonomy の再分類
- scalar / nullability / enum 境界の横断再設計
- test taxonomy / project layout の大規模再編
- CLI / MCP surface の 1:1 統一

## 3.1 解決済み論点

## 3.2 推奨実装順

`v2.0.0` の採用済み変更は、次の順で実装する。

1. library public surface rename
   - `Call` -> `CallResult`
   - `*CallAsync(...)` -> `*Async(...)`
   - `Create*Client(...)` -> `Create*ClientBundle(...)`
2. `CallError` additive detail field
   - `HttpStatusCode`
   - `VenueErrorCode`
   - `VenueErrorMessage`
3. adapter 追従
   - CLI verbose / stderr
   - MCP `upstream_error.details`
4. human-facing timestamp display
   - CLI summary / verbose
   - protocol debug log
   - live test log
5. migration lock test
   - public surface
   - adapter output
   - shared vocabulary
6. 正本文書の最終反映
   - `docs/spec.md`
   - `docs/cli.md`
   - `docs/mcp-server.md`
   - release note / migration

順序理由:

- rename を先に終えると、以後の adapter / test 更新を最終名で揃えられる
- `CallError` detail field は adapter 表示より前に library 契約へ入れる方が自然である
- human-facing timestamp は machine contract に触れないため、rename と error detail の後に独立して入れやすい
- test は最終 surface を固定する役割なので、public shape が固まった後に置く

### BC-V2-001 `CallResult + *Async(...)`

結論:

- `v2.0.0` では `Call<TRequest, TResponse>` を `CallResult<TRequest, TResponse>` へ rename する
- `Protocol` / `Native` facade の主公開面は `*CallAsync(...)` から `*Async(...)` へ rename する
- `call` 語彙自体は設計説明と周辺語彙に残す

検討した代替案:

- 現状維持: `Call<TRequest, TResponse>` + `*CallAsync(...)`
- `ApiResult<TRequest, TResponse>` + `*Async(...)`

採用理由:

- `Call` 系語彙の一貫性を大きく壊さずに、返り値型を結果コンテナとして明示できる
- method 名は `GetTickerAsync(...)` のような一般的な .NET 命名へ寄せられる
- `ApiResult` ほど概念語彙の全面再設計を要求せず、`child call` や `CallError` / `CallMeta` との接続を残しやすい
- 外部利用者向けの自然さと、ExchangeAPI 内部の `call` 概念の両立がしやすい

影響メモ:

- 返り値型 rename
  - `Call<TRequest, TResponse>` -> `CallResult<TRequest, TResponse>`
- facade public method rename
  - `GetTickerCallAsync(...)` -> `GetTickerAsync(...)`
  - `SendChildOrderCallAsync(...)` -> `SendChildOrderAsync(...)`
- `CallError` / `CallMeta` はこの論点では rename 対象に含めない

### BC-V2-002 `CallError` / `CallMeta`

結論:

- `v2.0.0` では `CallError` / `CallMeta` の型名は変更しない
- `CallResult` 採用後も、`call` は ExchangeAPI の実行単位と観測単位を表す概念語彙として残す

検討した代替案:

- `CallError` -> `CallResultError`
- `CallMeta` -> `CallResultMeta`
- `CallError` -> `ApiError`, `CallMeta` -> `ApiMeta`

却下理由:

- `CallError` と `CallMeta` は `CallResult` の所有物というより、call 実行の失敗分類と観測情報を表す型である
- `child call`、`Protocol call`、`Native call` の説明語彙と自然につながる
- `CallResult` 導入だけでも migration 影響は十分にあり、周辺型名まで一括 rename すると説明コストが増える
- `ResultError` / `ResultMeta` は役割よりも container 従属に見え、意味が弱くなる

### BC-V2-003 `Create*Client(...)`

結論:

- `v2.0.0` では `CreateProtocolClient(...)` を `CreateProtocolClientBundle(...)` へ rename する
- `v2.0.0` では `CreateNativeClient(...)` を `CreateNativeClientBundle(...)` へ rename する
- `BitflyerClientFactory` / `BinanceClientFactory` の class 名はこの論点では維持する

検討した代替案:

- 現状維持: `Create*Client(...)`
- `Create*Bundle(...)`
- class 名まで一括変更して `BitflyerClients.CreateNative(...)` のように寄せる

採用理由:

- 現在の method 名は返り値の `ProtocolBundle` / `NativeBundle` と不一致で、IDE 補完上の期待をずらす
- `Bundle` 単独だと利用者には抽象的だが、`ClientBundle` なら「client 群を束ねた生成物」だと理解しやすい
- 返り値変数は引き続き `client` と置けるため、利用者コードの読み味も保ちやすい
- class 名まで一括変更すると namespace、guide、test、adapter への波及が大きく、この論点としては変更範囲が広すぎる
- `Factory` class は生成導線としてまだ説明可能であり、まずは method 名の不一致解消を優先する

影響メモ:

- method rename
  - `BitflyerClientFactory.CreateProtocolClient(...)` -> `BitflyerClientFactory.CreateProtocolClientBundle(...)`
  - `BitflyerClientFactory.CreateNativeClient(...)` -> `BitflyerClientFactory.CreateNativeClientBundle(...)`
  - `BinanceClientFactory.CreateProtocolClient(...)` -> `BinanceClientFactory.CreateProtocolClientBundle(...)`
  - `BinanceClientFactory.CreateNativeClient(...)` -> `BinanceClientFactory.CreateNativeClientBundle(...)`
- overload shape は維持する
  - `(...options)`
  - `(HttpClient, ...options)`

### BC-V2-004 `BitflyerClientFactory` / `BinanceClientFactory`

結論:

- `v2.0.0` でも `BitflyerClientFactory` / `BinanceClientFactory` の class 名は維持する

検討した代替案:

- `BitflyerBundleFactory` / `BinanceBundleFactory`
- `BitflyerClients` / `BinanceClients`

却下理由:

- `Factory` は static な生成入口として現状の責務に合っている
- 利用者理解への寄与は method 名の方が大きく、`Create*ClientBundle(...)` まで整えれば class 名変更の効果は小さい
- class 名まで変更すると using、guide、tests、adapter、namespace 説明へ広く波及し、変更コストに対する改善幅が小さい
- `ClientFactory` という語も、最終的に API 利用用の client 群を生成する入口としてはなお説明可能である

### BC-V2-005 endpoint `*Request` / `*ClientOptions`

結論:

- `v2.0.0` でも endpoint request DTO は `*Request` のまま維持する
- `v2.0.0` でも client 構成型は `*ClientOptions` のまま維持する

検討した代替案:

- `*Request` -> `*Input`, `*Params`
- `*ClientOptions` -> `*BundleOptions`

却下理由:

- `GetTickerRequest`、`SendChildOrderRequest` のような naming は endpoint 入力 DTO として十分自然である
- `BitflyerClientOptions` / `BinanceClientOptions` も生成入口へ渡す構成として分かりやすい
- `ClientBundle` を採用したからといって `BundleOptions` へ寄せる利益は小さく、むしろ不自然になりやすい
- guide、tests、adapter、docs への波及に対して改善幅が小さい

### BC-V2-006 `CallError.Kind` taxonomy

結論:

- `v2.0.0` でも `CallError.Kind` の分類は `Transport / Http / Codec / Semantic / Mapping` を維持する

検討した代替案:

- `Http` を `Status` のような別名へ rename する
- `Codec` / `Semantic` を統合して decode failure を粗くまとめる
- venue 固有 business error taxonomy を `Kind` に持ち込む

却下理由:

- CLI が `CallError.Kind` をそのまま summary と verbose 出力に使っている
- MCP 側も `callErrorKind` detail をすでに前提にしている
- 現行 taxonomy は `Transport` と `Http`、`Codec` と `Semantic` の責務境界が文書上かなり明確で、再分類の利益が小さい
- venue 固有 taxonomy を `Kind` に持ち込むと、cross-venue 利用者向けの最小公開面が崩れる

### BC-V2-007 `CallError` detail fields

結論:

- `v2.0.0` では `CallError` に次の additive detail field を追加する
  - `HttpStatusCode`
  - `VenueErrorCode`
  - `VenueErrorMessage`

採用条件:

- すべて optional field とする
- `CallError.Kind` の判定結果を上書きしてはならない
- raw error body の正本は引き続き `ProtocolResponse.BodyText` とする
- raw JSON object や venue envelope 全体を `CallError` に持ち込んではならない

採用理由:

- `Kind` taxonomy を維持したまま障害解析性を上げられる
- CLI / MCP が raw body 全体を直接扱わずに、要点だけを表示・伝播しやすくなる
- `HttpStatusCode` は `Http` failure の理解を助ける
- `VenueErrorCode` / `VenueErrorMessage` は venue 依存失敗の最小 detail として有益である

公開境界メモ:

- `HttpStatusCode` は主に `Http` failure で使うが、detail field であり `Kind` を置き換えない
- `VenueErrorCode` / `VenueErrorMessage` は安全に抽出できた場合のみ保持する
- 秘匿値や署名値を `VenueErrorMessage` へ露出してはならない

### BC-V2-008 CLI / MCP での `CallError` detail field 露出

結論:

- CLI は summary の既定 shape を変えない
- CLI は `--verbose` 指定時のみ、`CallError` の additive detail field を出力してよい
- MCP は `upstream_error.details` に、追加 field を optional key として含めてよい

採用理由:

- 追加 field を利用者にまったく見せないと、`CallError` 拡張の価値が小さくなる
- 一方で summary へ常時混ぜると、通常系のエラー表示が冗長になる
- CLI は verbose、MCP は structured details という現行 adapter の責務にそのまま載せるのが自然である

露出方針:

- CLI
  - 既定 summary は維持する
  - `--verbose` 時は既存の `CallError.Kind`, endpoint 情報に加えて次を出してよい
    - `CallError.HttpStatusCode`
    - `CallError.VenueErrorCode`
    - `CallError.VenueErrorMessage`
- MCP
  - `upstream_error.details` に既存の `callErrorKind`, `callErrorMessage` を維持する
  - 追加で次を optional key として含めてよい
    - `callHttpStatusCode`
    - `callVenueErrorCode`
    - `callVenueErrorMessage`
- 両 adapter とも、field が `null` の場合は無理に出力しなくてよい
- secret, signature, credential value は従来どおり露出禁止とする

### BC-V2-009 `ProtocolResponse.BodyText` と `CallError` detail の境界

結論:

- raw error body の唯一の正本は引き続き `ProtocolResponse.BodyText` とする
- `CallError` は raw body の複製を保持しない
- `CallError` が保持してよいのは、raw body から抽出した narrow detail field のみとする

採用理由:

- raw text と extracted detail の両方を同じ責務に置くと、どちらが正本か曖昧になる
- adapter は通常表示では extracted detail を使い、必要時のみ raw body を参照する方が責務分離しやすい
- venue ごとの error envelope 全体を `CallError` に持ち込まないことで、秘匿境界と互換性を保ちやすい

固定する境界:

- `ProtocolResponse.BodyText`
  - raw error body の正本
  - protocol envelope や debug 用の参照元
- `CallError.HttpStatusCode`
  - status の狭い detail
- `CallError.VenueErrorCode`
  - venue error code の狭い detail
- `CallError.VenueErrorMessage`
  - venue error message の狭い detail
- `CallError` に raw JSON object、error envelope 全体、dictionary bag を追加してはならない

### BC-V2-010 scalar base contract

結論:

- `v2.0.0` でも数量系 scalar の基本型は `decimal` を維持する
- `v2.0.0` でも timestamp scalar の基本型は `DateTimeOffset` を維持する
- 数値 parse は引き続き culture 非依存、invariant culture 前提を維持する

検討した代替案:

- 数量系を `double` / `float` ベースへ寄せる
- timestamp を `DateTime` や string に戻す
- adapter ごとに parse / 表示の都合で scalar 契約を分ける

却下理由:

- `docs/spec.md` の scalar contract は現状でもかなり明確で、コード側も `decimal` と `DateTimeOffset` に揃っている
- bot 判断に使う数量系を `double` / `float` へ落とす利益より、丸め差や比較不安定の方が問題になる
- timestamp を `DateTimeOffset` から弱めると timezone 解釈の責務境界が曖昧になる
- version-wide な breaking change として宣言するより、現行契約を維持した方が利用者影響が小さい

### BC-V2-011 Native DTO の `required / nullable`

結論:

- `v2.0.0` では Native DTO 全体に対する一括 `required / nullable` tighten は採用しない
- `required / optional` の是正は endpoint ごとの exact contract と venue matrix を基準に個別に行う
- conditional required field は引き続き `Semantic` contract として endpoint module で扱う

検討した代替案:

- 現行 Native DTO を横断して `nullable` をできるだけ削減する
- response DTO の `required` を一律に増やす
- request DTO でも conditional field を非 nullable 型へ押し込む

却下理由:

- `required / optional` は venue API と endpoint contract の exact shape に依存し、横断ルールだけでは決め切れない
- docs で timezone や omission rule が未固定の field まで一括 tighten すると、正本より先に DTO だけが硬直する
- 現行コードにも `price?`、`trigger_price?`、`margin_call_due_date?` のように contract 上妥当な nullable field が混在している
- version-wide な rename / tighten として扱うより、endpoint 単位の修正として ledger 外で追う方が安全である

運用メモ:

- `required` にできるのは、venue response で安定して存在し、endpoint matrix でも exact contract が固定された field に限る
- `nullable` を残すのは、API 文書上 optional、または conditional required の field に限る
- endpoint matrix で exact contract が未確定の field は、`Fixed` 扱いに上げる前に文書側を先に確定する

### BC-V2-012 enum と string vocabulary の境界

結論:

- `v2.0.0` でも closed-set string vocabulary は venue-local enum を維持する
- `v2.0.0` でも open-set、docs が弱い field、inventory が流動的な field は string のまま維持する
- enum は cross-venue 共通化せず、各 venue の `Vocabulary` project に置く

検討した代替案:

- `Side` や `Interval` を cross-venue 共通 enum に寄せる
- known values 定数を正本化し、string field を広く enum へ昇格させる
- enum をやめて raw string に戻す

却下理由:

- 現行 `spec` は closed-set を venue-local enum、open-set を string とする境界をすでに明確にしている
- cross-venue 共通 enum は一見便利でも、venue ごとの差異や将来拡張で無理な統合を招きやすい
- known values 定数は convenience に留める方が安全で、validation 正本へ格上げすると docs より定数側が先に硬直する
- enum を raw string に戻すと、現行の `Vocabulary` project と serializer 契約の利点を失う

運用メモ:

- enum 化の判断基準は「docs で closed set が確認できるか」を優先する
- `ProductCode`, `CurrencyCode`, `Symbol`, `ReasonCode` のような値集合が変動しやすい field は string を維持する
- enum 化しても wire JSON の値は API string literal を維持しなければならない

### BC-V2-013 test taxonomy / project layout

結論:

- `v2.0.0` では test taxonomy と project layout の大規模再編は採用しない
- 現行の `Architecture / Protocol / Native / Composition / Live / Adapter` 分離を維持する
- flaky live test の問題は taxonomy の変更ではなく、opt-in と safety 条件の局所化で扱う
- ここで却下するのは unit / contract test taxonomy と source/test project layout の大規模再編であり、`BC-V2-021` の verification 運用分類追加とは矛盾しない

検討した代替案:

- `ContractTests` や `ParityTests` を新 top-level taxonomy として再編する
- exchange test と adapter test の project 粒度を大きく組み替える
- live test を別 solution / 別分類へ広く移し替える

却下理由:

- `docs/spec.md` の Verification 契約と実際の `tests/` 配置は概ね一致している
- すでに venue ごとに `Architecture.Tests`, `Protocol.Tests`, `Native.Tests`, `Composition.Tests`, `LiveTests` が分かれている
- adapter 側も `Cli.Tests`, `McpServer.Tests`, `McpServer.LiveTests` に分離されており、v2 で taxonomy 自体を壊す必要が薄い
- taxonomy を再編しても、rename や contract tighten の regression を直接防ぐ効果は小さい

### BC-V2-014 v2 breaking change を固定する test

結論:

- `v2.0.0` で採用した breaking change には、対応する surface lock test を追加または更新する
- 追加対象は少なくとも library public surface、CLI verbose/error rendering、MCP details shape を含む
- live test は従来どおり opt-in のままとし、v2 migration lock の主戦場にはしない

採用理由:

- `CallResult`、`*Async(...)`、`Create*ClientBundle(...)` の rename は compile error だけで検出できる部分もあるが、public surface の意図まで固定するには test が必要である
- `CallError` additive detail field と CLI/MCP の露出方針は、文書だけでは後退しやすい
- live test は venue 状態と rate limit の影響を受けるため、v2 migration risk の固定には unit / adapter / architecture test の方が向いている

固定対象メモ:

- library
  - public surface 上に `CallResult` が現れること
  - facade method が `*Async(...)` であること
  - factory method が `Create*ClientBundle(...)` であること
- CLI
  - 既定 summary では additive detail field を出しすぎないこと
  - verbose 時のみ `HttpStatusCode`, `VenueErrorCode`, `VenueErrorMessage` を出せること
- MCP
  - `upstream_error.details` が `callHttpStatusCode`, `callVenueErrorCode`, `callVenueErrorMessage` を optional に持てること

運用メモ:

- v2 migration lock test は live test より先に unit / adapter test で固定する
- live test は parity と real-world drift 検知に集中し、rename や surface naming の正本にしない

### BC-V2-015 CLI / MCP surface の 1:1 統一

結論:

- `v2.0.0` では CLI と MCP の surface を 1:1 には統一しない
- CLI は引き続き library endpoint に近い command surface を持ってよい
- MCP は引き続き bot / LLM 向け責務単位 tool surface を持ってよい

検討した代替案:

- CLI command を MCP tool 名へ寄せる
- MCP tool を library endpoint の 1:1 mirror に寄せる
- CLI と MCP の両方で同一の surface inventory を正本化する

却下理由:

- CLI は `native` / `protocol` の inspection と canonical request input を目的にしており、library endpoint との近さに意味がある
- MCP は `get_market_snapshot` や `evaluate_order` のように、複数 endpoint を束ねた bot-oriented abstraction に意味がある
- 両者を無理に統一すると、CLI は冗長になり、MCP は tool minimalism を失う
- 現行文書でも CLI は runtime registry、MCP は tool ledger をそれぞれ正本にしており、正本の層が異なる

### BC-V2-016 CLI / MCP の基底語彙整合

結論:

- `v2.0.0` では CLI / MCP の surface は分けたまま、基底語彙だけを揃える
- 少なくとも次を shared vocabulary として扱う
  - `CallError.Kind`
  - `HttpStatusCode`
  - `VenueErrorCode`
  - `VenueErrorMessage`
  - `venue`
  - `symbol`
  - timestamp
  - decimal string / numeric representation の境界

採用理由:

- adapter surface は違っても、error と scalar の読み方までズレると migration と運用が難しくなる
- CLI の `protocol` / `native`、MCP の `upstream_error` は、根底では同じ ExchangeAPI call を基にしている
- `CallError` additive field を追加した以上、CLI と MCP で detail key の意味を揃える価値が高い

固定対象メモ:

- CLI
  - stderr verbose は `CallError.Kind`、endpoint id、status code、venue error detail を同じ意味で扱う
  - `native` は数値を JSON number のまま出し、`protocol` は `ProtocolResponse.BodyText` を raw string として扱う
- MCP
  - tool input/output は decimal string と UTC timestamp を維持する
  - `upstream_error.details` は CLI verbose と意味対応する detail key を持つ
- 両 adapter とも、secret / signature / credential value は露出しない

### BC-V2-017 human-facing な時刻表示方針

結論:

- `v2.0.0` でも内部契約の timestamp 型は `DateTimeOffset` を維持する
- machine canonical は UTC 基準を維持する
- ExAPI 自身が出す human-facing 出力では、local with offset を優先してよい
- この方針は CLI summary / verbose、protocol debug log、live test log に適用してよい

採用理由:

- 現状の UTC 基本方針は機械処理には強いが、人間が読む debug / 運用ログでは不便である
- `DateTimeOffset` を維持すれば、機械契約を壊さずに表示だけ local へ寄せられる
- ExAPI 自身が持つ log / debug 出力は外部 consumer に委ねきれず、repo 内で方針を決める価値が高い

固定する境界:

- `Native` / `Protocol` DTO の timestamp contract は表示都合で暗黙変換しない
- MCP の structured response は UTC / structured contract を維持してよい
- CLI summary / verbose、protocol debug log、live test log は local with offset を優先してよい
- raw / forensic 用に canonical UTC が必要な場合、その参照は削除しない

運用メモ:

- 初手では timezone override 設定を必須にしない
- `local` は既定では実行環境の local time zone として扱う
- 必要なら UTC 併記は許容するが、human-facing 既定表示は local with offset を優先する

### BC-V2-018 private credentials 取扱いの責務分離

結論:

- `v2.0.0` では `ExchangeAPI` core は private credentials の storage / encryption 方式を正本に含めない
- core は `apiKey` と署名生成を委譲できる auth provider 契約を持つ
- lib 同梱サンプルは平文 credentials を受ける最小実装でよい
- `age` などの運用方式は core 正本から外し、CLI / guide / optional 実装へ寄せる

採用理由:

- `age-backed source` を core 契約に含めると、library が secret storage policy まで背負う
- 実行時には平文 secret が必要でも、storage と signing を外へ委譲すれば core の責務を軽くできる
- 平文サンプルを sample / test 用として限定し、production recipe は外部 provider に委ねる方が誤解が少ない

固定する境界:

- core
  - auth provider interface を持つ
  - private auth の実行規約と header / signing 契約だけを持つ
- bundled sample
  - 平文 credentials を受ける最小実装を置いてよい
  - ただし production 推奨にはしない
- adapter / external recipe
  - `age`
  - env injection
  - secret manager
  - encrypted string
  などの具体方式はここで扱う

運用メモ:

- CLI canonical は引き続き API key / secret の直接引数入力を許可しない
- credentials setup の operational helper を追加するかどうかは別論点で扱う
- secret / signature / credential value の redact 方針は従来どおり維持する

### BC-V2-019 auth provider の具体 shape

結論:

- `v2.0.0` では auth provider の具体 shape として `IApiCredentialProvider.OpenSessionAsync(...)` 型を採用する
- session は `ApiKey` property と `Sign(string payload)` を持つ
- v2 の署名 API は `Sign(string payload)` のみとし、byte sequence overload は post-v2 検討に回す
- provider は venue-specific class とし、runtime venue selector を持たせない
- `PlainText` / `AgeFile` provider は `ExchangeApi.Optional.Credentials` に置く
- 通常利用では client 側が session を内部で開閉し、初見利用者に session を必須で意識させない
- 高コスト provider や寿命を明示管理したい利用者には、明示 session を使う導線を残す

想定 shape:

- provider
  - `OpenSessionAsync(...)`
- session
  - `ApiKey`
  - `Sign(string payload)`
  - `DisposeAsync()`

検討した代替案:

- `GetApiKey() + GetApiSecret()` 型
- `GetApiKey() + Sign(...)` 型
- `OpenSessionAsync(...)` を持たず、provider 実装内部の暗黙 cache に任せる

採用理由:

- `age` のような高コスト provider を都度フル解決せずに扱いやすい
- auth material の寿命境界を session に閉じ込められる
- client の寿命と auth material の寿命を固定的に結び付けなくてよい
- 明示 session を持ちつつ、通常利用では client が自動 session を使えば初見利用者への負荷を抑えられる
- in-process 擬似保護 cache のような中途半端な仕組みを正本にせずに済む

利用者導線:

- 通常利用
  - auth provider を client へ注入する
  - private call 実行時に client 側が内部で session を扱ってよい
- 明示利用
  - 利用者が `OpenSessionAsync(...)` して複数 private call の間だけ再利用してよい
  - 処理単位の終了時に `DisposeAsync()` する

明示 session overload:

```csharp
Task<CallResult<TRequest, TResponse>> EndpointAsync(
    TRequest request,
    IApiCredentialSession credentialSession,
    CancellationToken cancellationToken = default);
```

ルール:

- 通常 overload は provider から session を内部で開閉する
- 明示 session overload は caller が渡した session を dispose しない
- 明示 session overload は private endpoint にだけ用意する

optional credentials public type set:

- `ExchangeVenue`
- `IAgeCredentialFileDecryptor`
- `AgeCliCredentialFileDecryptor`
- `BitflyerPlainTextApiCredentialProvider`
- `BinancePlainTextApiCredentialProvider`
- `BitflyerAgeFileApiCredentialProvider`
- `BinanceAgeFileApiCredentialProvider`
- `PlainTextApiCredentialProviderFactory`
- `AgeFileApiCredentialProviderFactory`

運用メモ:

- session の標準推奨寿命は operation 単位の bounded lifetime とする
- `PrivateClientBundle` 寿命や client 寿命まで伸ばす実装は許容するが、正本の推奨にはしない
- session を採用しても、API key / secret の log / exception / result / debug detail 露出は禁止する

### BC-V2-020 MCP read-only surface の拡張方針

結論:

- `v2.0.0` では、資産状態を変える操作を除く read-only 情報は MCP で原則サポート対象にする
- 注文、キャンセル、入金、出金などの副作用系は引き続き MCP 非対応とする
- MCP tool は `Core Bot Tools` と `Inspection Read Tools` の二層で管理する
- `Core Bot Tools` は bot-oriented abstraction を維持し、`Inspection Read Tools` は開発中確認と運用 inspection のための read-only tool とする

採用理由:

- CTradeBot 開発中の確認要求に対して、library にはあるが MCP では見えない read-only 情報を減らしたい
- それでも `read / evaluate only` と `no side effects` の原則は維持したい
- bot 本番導線向けの集約 tool と、inspection 用の read tool を分けると、tool 追加による責務混濁を抑えやすい

固定する境界:

- MCP でサポート対象にする
  - market/account/order-history などの read-only 情報
  - evaluation
- MCP でサポート対象にしない
  - 注文
  - キャンセル
  - 入金
  - 出金
  - その他の副作用操作

運用メモ:

- `Core Bot Tools` は現行の `get_market_snapshot`、`get_account_snapshot`、`evaluate_order`、`evaluate_margin_order` のような責務単位 tool を維持する
- `Inspection Read Tools` は `GetCollateralAccounts`、`GetBalanceHistory`、`GetCollateralHistory`、`GetChildOrders` のような read-only endpoint を候補にできる
- `Inspection Read Tools` を追加しても、CLI / library endpoint との 1:1 mirror を正本として要求しない

### BC-V2-021 verification の物理構成と危険度分類

結論:

- `v2.0.0` では verification の物理構成を `repo` と `local` に分けて整理する
- verification の運用分類には `safe` / `tolerable` / `dangerous` を導入する
- API 正本における `public/private`, `read/write`, `CleanupPolicy` は維持し、verification 分類で置き換えない
- `tests/` は契約固定と deterministic verification を中心に保ち、live / manual verification の本体は `tests/` または `verification/` に置く
- 実行結果、artifact、log、手動確認メモは `local/evidence/` に集約する

採用理由:

- 現行の `public/private`, `read/write`, `Phase`, `CleanupPolicy` は API 契約としては妥当だが、運用時の実害判断とは少しずれる
- CTradeBot の verification 構成は test 本体と `local/evidence/` を分け、live/manual verification と証跡を切り離しており、運用理解がしやすい
- ExchangeAPI でも API 契約分類を保ったまま verification を別層で整理した方が、実行判断と artifact 管理が明確になる
- CTradeBot の `local/evidence/` 型の整理は、テスト本体とエビデンスを分離できるため ExchangeAPI にも有効である

固定する境界:

- API 正本
  - `public/private`
  - `read/write`
  - `CleanupPolicy`
  - `LiveTestPhase`
- verification 正本
  - `repo/local`
  - `safe/tolerable/dangerous`

物理構成:

- `tests/`
  - unit / contract / adapter / migration lock test
- `verification/`
  - live/manual verification code
  - runbook
  - replay / verification scenario
- `local/evidence/`
  - static
  - verification
  - local-live
  - test-operation
  - runtime artifact / log / notes

運用メモ:

- `safe`
  - state を変更しない verification
  - private read は認証を要しても `safe` に置いてよい
- `tolerable`
  - 実害はありうるが、最小影響・cleanup 前提で許容できる verification
- `dangerous`
  - 影響範囲が広い、cleanup 不可、または資産移動を伴う verification
- `local/app/` のような通常実行 I/O 正本は library repo である ExchangeAPI には導入しない

## 4. 運用ルール

- draft 段階の案もここへ集約する
- 採用済みでも、正本反映前は `採用` のまま残してよい
- 正本反映後も、利用者移行のため release 完了までは削除しない
- 却下理由も簡潔に残す

## 5. 実装チェックリスト

v2 実装時は、次の順で進める。

1. `Primitives`
   - `Call<TRequest, TResponse>` を `CallResult<TRequest, TResponse>` へ rename する
   - `CallError` に `HttpStatusCode`, `VenueErrorCode`, `VenueErrorMessage` を追加する
   - `IApiCredentialProvider`, `IApiCredentialSession`, `ApiCredentialException`, `ApiCredentialErrorKind` を配置する
2. `Protocol` / `Native`
   - public facade method を `*Async(...)` へ rename する
   - private endpoint に明示 session overload を追加する
   - private endpoint の session overload は `EndpointAsync(request, credentialSession, cancellationToken)` の順にする
   - raw body は `ProtocolResponse.BodyText` に残し、`CallError` detail は抽出 field のみを保持する
3. `Composition`
   - `CreateProtocolClientBundle(...)` / `CreateNativeClientBundle(...)` へ rename する
   - `BitflyerClientFactory` / `BinanceClientFactory` class 名は維持する
   - options は `ApiCredentialProvider` を受ける
   - 通常 private call では provider から session を内部で開閉する
4. `Optional.Credentials`
   - `src/Optional/Credentials/ExchangeApi.Optional.Credentials.csproj` を追加する
   - `ExchangeApi.Optional.Credentials` package として pack 対象に含める
   - `ExchangeVenue`, `IAgeCredentialFileDecryptor`, `AgeCliCredentialFileDecryptor` を追加する
   - venue-specific `PlainText` / `AgeFile` provider を追加する
   - provider factory を追加する
   - `AgeFile` provider は credentials JSON schema と `ApiCredentialErrorKind` を test で固定する
5. CLI
   - factory / facade rename に追従する
   - credential failure を stderr と exit code `2` へ写像する
   - verbose detail key を `credentialErrorKind`, `venue`, `provider`, `reason` に揃える
   - `CallError` additive detail を `--verbose` に出す
6. MCP
   - factory / facade rename に追従する
   - credential failure 時は private tool 非公開または `account_unavailable` details へ写像する
   - credential failure details は `credentialErrorKind`, `venue`, `provider`, `reason` を持つ
   - `upstream_error.details` に `callHttpStatusCode`, `callVenueErrorCode`, `callVenueErrorMessage` を optional で追加する
7. Verification / package
   - migration lock test を追加または更新する
   - `ExchangeApi.Optional.Credentials` を solution と pack script 対象へ追加する
   - local NuGet consumer smoke test を v2 API 名で更新する
   - CLI / MCP publish script は executable artifact 方針を維持する

## 6. 関連文書

- [`docs/docs-architecture.md`](./docs-architecture.md)
- [`docs/spec.md`](./spec.md)
- [`docs/migration-v2.0.0.md`](./migration-v2.0.0.md)
- [`docs/archive/drafts/v2.0.0-overview.md`](./archive/drafts/v2.0.0-overview.md)
- [`docs/archive/drafts/v2.0.0-details.md`](./archive/drafts/v2.0.0-details.md)
