# REVIEW-07: ボイラーコード総点検（共通化・自動化・規約化）

## 対象と前提
- 対象: `src/` 全体（特に `Exchanges` / `Contracts`）および `tests/Exchanges`。
- 前提: REVIEW-01〜06 は受理済みとして再評価しない。
- 方針: **提案のみ**（コード・ドキュメントの既存内容は変更しない）。
- 評価軸: 将来の「取引所追加」「endpoint追加」時の保守コスト最小化。

---

## A. ボイラーパターン一覧（最重要）

## 1) Call骨格（Adapter APIメソッドの try/catch + MapCall）
**同形グルーピング**
- `GetXxx/OrderXxx/CancelXxx` 系が、
  - `startedAt = UtcNow`
  - `normalized call 実行`
  - `ApiCallMapper.MapCall(...)`
  - `catch -> ApiCallMapper.FromException(...)`
  の同一テンプレートで構成。

**典型ファイル例（Evidence）**
- `src/Exchanges/Bitflyer/Adapter/Private/Api/PrivateApi.cs`
- `src/Exchanges/Bittrade/Adapter/Private/Api/TradingApi.cs`
- `src/Exchanges/Bittrade/Adapter/Private/Api/AccountApi.cs`
- `src/Exchanges/Bittrade/Adapter/Private/Api/SpotHistoryApi.cs`
- `src/Exchanges/Bitflyer/Adapter/Public/Api/MarketApi.cs`
- `src/Exchanges/Bittrade/Adapter/Public/Api/MarketApi.cs`

**なぜボイラー化しているか（原因）**
- `Call<TReq, TRes>` を層ごとに整形する責務が endpoint単位で反復。
- 例外→`CallError` 変換が各メソッドで毎回必要。
- メタ情報（`Operations.*`）の付与が手作業。

**揃い度（並列性）**
- 取引所間: 高（Bitflyer/Bittradeでほぼ同型）
- 層間: 中（Adapterで顕著、Raw/Normalizedは別テンプレート）

---

## 2) ApiCallMapper薄ラッパ（exchange別に同一実装）
**同形グルーピング**
- `AdapterCallMapper` への委譲メソッド群（`FromCall/MapCall/FromException/ToExchangeErrorCategory/ToStatusCode`）が交換所ごとに複製。

**典型ファイル例（Evidence）**
- `src/Exchanges/Bitflyer/Adapter/Internal/ApiCallMapper.cs`
- `src/Exchanges/Bittrade/Adapter/Internal/ApiCallMapper.cs`
- `src/Exchanges/Common/Application/ExchangeInfo/Adapter/Internal/AdapterCallMapper.cs`（実体）

**原因**
- namespace分離を保つために、exchange配下で同一Facadeを再定義。
- ただし差分が実質ゼロで、変更時に二重修正が必要。

**揃い度**
- 取引所間: 非常に高（実質同一）
- 層間: 低（Adapter固有）

---

## 3) Endpoint語彙の二重管理（NormalizedとWireで同名定数）
**同形グルーピング**
- endpoint ID文字列が「Normalized内部定数」と「Wire公開定数」に二重定義。

**典型ファイル例（Evidence）**
- `src/Exchanges/Bitflyer/Normalized/Internal/Constants/EndpointIds.cs`
- `src/Exchanges/Bitflyer/Wire/Constants/EndpointIds.cs`
- `src/Exchanges/Bittrade/Normalized/Internal/Constants/EndpointIds.cs`
- `src/Exchanges/Bittrade/Wire/Constants/EndpointIds.cs`
- `src/Exchanges/Bitflyer/Wire/Constants/EndpointIdCatalog.cs`
- `src/Exchanges/Bittrade/Wire/Constants/EndpointIdCatalog.cs`

**原因**
- 公開境界（Wire）と内部境界（Normalized）で参照都合が分離。
- ただし endpoint追加時に「同名値の転記作業」が発生。

**揃い度**
- 取引所間: 高
- 層間: 高（Normalized↔Wireで構造的に並列）

---

## 4) Wire endpoint builder（Get/Post spec組み立ての反復）
**同形グルーピング**
- `PublicEndpoints/PrivateEndpoints` に endpointごとの `WireSpecBuilder.Get/Post(...)` が列挙される。

**典型ファイル例（Evidence）**
- `src/Exchanges/Bitflyer/Wire/Public/Endpoints/PublicEndpoints.cs`
- `src/Exchanges/Bitflyer/Wire/Private/Endpoints/PrivateEndpoints.cs`
- `src/Exchanges/Bittrade/Wire/Public/Endpoints/PublicEndpoints.cs`
- `src/Exchanges/Bittrade/Wire/Private/Endpoints/PrivateEndpoints.cs`
- `src/Exchanges/Bitflyer/Wire/Internal/WireSpecBuilder.cs`
- `src/Exchanges/Bittrade/Wire/Internal/WireSpecBuilder.cs`

**原因**
- endpoint単位で path/query/body を定義する必要がある。
- しかしメソッド骨格がほぼ固定。

**揃い度**
- 取引所間: 高
- 層間: 中（Wire内で高く反復）

---

## 5) DTOペア増殖（Raw Request/Response + Normalized DTO + Contract DTO）
**同形グルーピング**
- endpoint1個追加ごとに、複数層で DTO/Request が連鎖的に増える。

**典型ファイル例（Evidence）**
- `src/Exchanges/Bittrade/Raw/Public/Requests/PublicRequests.cs`
- `src/Exchanges/Bittrade/Raw/Public/Dtos/GetDetailMergedResponse.cs`
- `src/Exchanges/Bittrade/Raw/Private/Requests/PrivateRequests.cs`
- `src/Exchanges/Bittrade/Raw/Private/Dtos/PostOrdersPlaceResponse.cs`
- `src/Exchanges/Bitflyer/Raw/Public/Requests/MarketDataRequests.cs`
- `src/Exchanges/Bitflyer/Raw/Private/Dtos/GetChildOrdersResponse.cs`
- `src/Contracts/Facade/Requests/TickerRequest.cs`
- `src/Contracts/Common/Dtos/TickerResponse.cs`

**原因**
- 境界ごとの型安全を維持する設計の副作用。
- 手作業での追加時にファイル作成漏れ/命名ずれが発生しやすい。

**揃い度**
- 取引所間: 高
- 層間: 非常に高（Raw→Normalized→Adapter→Contractsの直列）

---

## 6) Mapper / Normalizer の scalar変換反復
**同形グルーピング**
- `Side`, `OrderType`, `Timestamp`, `Price/Size`, `RawSnapshot` などの変換ロジックが endpointごと・exchangeごとに散在。

**典型ファイル例（Evidence）**
- `src/Exchanges/Bitflyer/Normalized/Internal/Mappers/TickerNormalizer.cs`
- `src/Exchanges/Bitflyer/Normalized/Internal/Mappers/TradingMapper.cs`
- `src/Exchanges/Bitflyer/Adapter/Internal/Mappers/MarketMapper.cs`
- `src/Exchanges/Bittrade/Normalized/Internal/Mappers/Normalizer.cs`
- `src/Exchanges/Bittrade/Normalized/Internal/Mappers/TradingMapper.cs`
- `src/Exchanges/Bittrade/Adapter/Internal/Mappers/MarketMapper.cs`
- `src/Utilities/OrderBook/OrderBookNormalizer.cs`

**原因**
- 仕様語彙（raw）と共通語彙（contract/domain）の変換点が多い。
- 共通化可能な最小単位（例: Side mapping）が関数化不足。

**揃い度**
- 取引所間: 中〜高（構造は同じ、語彙は異なる）
- 層間: 高（Normalized/Adapterで同種責務）

---

## 7) Operations定数（Component名）の手動同期
**同形グルーピング**
- `Operations.*` の文字列定数を APIメソッドから参照するパターンが exchangeごとに存在。

**典型ファイル例（Evidence）**
- `src/Exchanges/Bitflyer/Adapter/Internal/Operations/Operations.cs`
- `src/Exchanges/Bittrade/Adapter/Internal/Operations/Operations.cs`
- `src/Exchanges/Bitflyer/Adapter/Private/Api/PrivateApi.cs`
- `src/Exchanges/Bittrade/Adapter/Private/Api/TradingApi.cs`

**原因**
- 観測・追跡用の component名を明示しているため。
- endpoint追加時に Operations 追加漏れが起こり得る。

**揃い度**
- 取引所間: 高
- 層間: 中（主にAdapter）

---

## 8) テストfixture/stub/アサーションヘルパーの反復
**同形グルーピング**
- exchange別テストで、同等の fake/stub/assertion が個別実装される。

**典型ファイル例（Evidence）**
- `tests/Exchanges/Bitflyer/Raw.Endpoints.Tests/WireRequestAssertions.cs`
- `tests/Exchanges/Bittrade/Raw.Endpoints.Tests/WireRequestAssertions.cs`
- `tests/Exchanges/Bitflyer/Adapter.Tests/Fakes/FakeBitflyerPublicApi.cs`
- `tests/Exchanges/Bittrade/Adapter.Tests/Helpers/BittradeRawApiStub.cs`
- `tests/Exchanges/Bitflyer/Raw.Endpoints.Tests/Inventory/BitflyerInventoryEndpointIdConsistencyTests.cs`
- `tests/Exchanges/Bittrade/Raw.Endpoints.Tests/Inventory/BittradeInventoryEndpointIdConsistencyTests.cs`
- `tests/Inventory/InventoryEndpointIdParser.cs`

**原因**
- 各exchangeのAPI面積に合わせて stub が肥大化。
- ただし検証観点（endpoint id整合、wire spec検証）は共通。

**揃い度**
- 取引所間: 高
- 層間: 低（主にテスト層）

---

## B. 共通化の選択肢（パターン別）

> 注: 各パターンで複数手段がありうるが、ここでは「最適候補」を1つ明示。

### 1) Call骨格
- 選択肢: 関数抽出 / template method / source generator
- **最適提案: template method + 小さな高階関数ヘルパー**
  - 理由: 例外処理・MapCall・component付与の骨格は共通だが、request生成/mapperはendpoint固有。高階関数で差分注入が最も実装負担と可読性のバランスが良い。

### 2) ApiCallMapper薄ラッパ
- 選択肢: 共通ヘルパー直接参照 / partial class / source generator
- **最適提案: exchange側ラッパ削減（直接 `AdapterCallMapper` 参照）**
  - 理由: 既に `AdapterCallMapper` が存在し機能が完結。薄ラッパは namespace都合以外の価値が薄く、ドリフト源になりやすい。

### 3) Endpoint語彙二重管理
- 選択肢: 単一正本化 / generatorで複製生成
- **最適提案: Wireを正本にした source generator で Normalized定数を自動生成**
  - 理由: 現在の可視性（Wire公開、Normalized内部）を崩さず転記ミスのみ排除できる。

### 4) Wire endpoint builder
- 選択肢: 共通builder API強化 / 宣言DSL / source generator
- **最適提案: 宣言的 endpoint registry + 生成**
  - 理由: endpoint追加時の作業が「1宣言」になり、`Paths/QueryKeys/EndpointIds/PublicEndpoints/PrivateEndpoints` の更新点を収束できる。

### 5) DTOペア増殖
- 選択肢: ジェネリックDTO / OpenAPI由来生成 / 手動維持
- **最適提案: Raw層は仕様定義（OpenAPI/JSON schema/手書きschema）から生成、上位層は手書き維持**
  - 理由: Raw DTOは機械生成に適し、Normalized/Contractsはドメイン判断を含むため手書きの価値が高い。

### 6) Mapper/Normalizer scalar変換
- 選択肢: 共通ヘルパー / 変換テーブル化 / generator
- **最適提案: 変換テーブル（辞書/レキシコン）+ 小ヘルパー集約**
  - 理由: exchange固有語彙を残しつつ、`Side/OrderType/Timestamp` の反復ロジックを減らせる。テストもしやすい。

### 7) Operations定数
- 選択肢: 手動定義 / nameofベース / generator
- **最適提案: `Operations` を endpoint registry から生成**
  - 理由: 監視ラベルの命名規約逸脱を機械的に防止できる。

### 8) テストfixture/stub
- 選択肢: 共通テストキット / AutoFixture導入 / generator
- **最適提案: テストデータ生成・WireSpec検証を `tests/Common.TestKit` に共通化**
  - 理由: 導入コストが低く、exchange追加時の最低限テンプレートを提供できる。

---

## C. 優先度付きロードマップ

## P0（やらないとドリフト進行）
1. **Endpoint語彙の単一正本化（生成導入）**
   - 狙い: `EndpointIds` 転記ミス防止。
   - 影響範囲: `Exchanges/*/Wire`, `Exchanges/*/Normalized/Internal/Constants`, endpoint inventory tests。
   - リスク: 生成物の差分ノイズ。初期セットアップが必要。

2. **Call骨格のテンプレート化（Adapter層）**
   - 狙い: endpoint追加時の例外処理/MapCallの漏れ防止。
   - 影響範囲: `Exchanges/*/Adapter/Public/Api`, `Exchanges/*/Adapter/Private/Api`。
   - リスク: 過抽象化でデバッグ性が下がる可能性。

3. **Operations定数の生成 or 一元管理**
   - 狙い: component名ドリフト（観測軸ずれ）防止。
   - 影響範囲: `Exchanges/*/Adapter/Internal/Operations`, 各Api実装。
   - リスク: 既存監視クエリのラベル変更に注意。

4. **ApiCallMapper薄ラッパの整理方針決定**
   - 狙い: 実質同一コードの重複排除。
   - 影響範囲: `Exchanges/*/Adapter/Internal/ApiCallMapper.cs`。
   - リスク: using/namespaceの参照変更で小規模な連鎖修正。

## P1（やると大きく効く）
1. **Wire endpoint定義の宣言化（registry化）**
   - 狙い: endpoint追加時の変更点圧縮。
   - 影響範囲: `Exchanges/*/Wire/Constants`, `.../Endpoints`, 関連テスト。
   - リスク: 既存コード生成導入時の学習コスト。

2. **Raw DTO生成パイプラインの導入**
   - 狙い: DTOファイル増殖の手作業削減。
   - 影響範囲: `Exchanges/*/Raw/*/Dtos`, CI。
   - リスク: 仕様差分の取り込みフロー整備が必要。

3. **Mapper scalarヘルパー共通化**
   - 狙い: `Side/OrderType/Timestamp` 変換の重複削減。
   - 影響範囲: `Exchanges/*/Normalized/Internal/Mappers`, `Exchanges/*/Adapter/Internal/Mappers`。
   - リスク: 取引所固有語彙の誤共通化。

4. **テスト共通キット（WireSpec assert / inventory path / fixture builder）**
   - 狙い: exchange別テストの定型削減。
   - 影響範囲: `tests/Exchanges/*`, `tests/Inventory`。
   - リスク: 既存テストの読みやすさが下がらない設計が必要。

5. **endpoint追加時テンプレート（作業雛形）を整備**
   - 狙い: 追加漏れ（DTO/Mapper/Test）を工程で予防。
   - 影響範囲: 開発手順・PR運用。
   - リスク: テンプレートの陳腐化。

## P2（任意）
1. **Facade Request/Response の生成補助（スキャフォールド）**
   - 狙い: Contracts層の単純record作成を半自動化。
   - 影響範囲: `src/Contracts/Facade/Requests`, `src/Contracts/Common/Dtos`。
   - リスク: 契約設計レビューをスキップしない運用が必要。

2. **Adapter PublicClient/PrivateApi の組み立て定型共通化**
   - 狙い: constructor/委譲メソッドの反復削減。
   - 影響範囲: `Exchanges/*/Adapter/Public/Api/PublicClient.cs`, `.../Private/Api/PrivateApi.cs`。
   - リスク: DI・可視性要件との衝突。

---

## D. 統合チェックリスト案（将来のPRテンプレ用）

以下は「機械的にYes/No判定しやすい」文面に寄せたチェック項目。

1. 新規 endpoint を追加した場合、`Wire/Constants/EndpointIds.cs` に ID を追加した。  
2. 新規 endpoint を追加した場合、`Wire/Constants/EndpointIdCatalog.cs` に ID を登録した。  
3. 新規 endpoint を追加した場合、`Wire/Constants/EndpointTraits.cs` の `RequiresAuth` を更新した。  
4. 新規 endpoint を追加した場合、`Wire/Public|Private/Endpoints/*.cs` に spec builder を追加した。  
5. 新規 endpoint を追加した場合、Raw Request 型を追加した（または既存型を拡張した）。  
6. 新規 endpoint を追加した場合、Raw Response DTO を追加した（または既存型を拡張した）。  
7. 新規 endpoint を追加した場合、Normalized Request/Response の必要型を追加した。  
8. 新規 endpoint を追加した場合、Normalizer/Mapper の変換ロジックを追加した。  
9. 新規 endpoint を追加した場合、Adapter API から `ApiCallMapper.MapCall` 経由で返却している。  
10. `catch(Exception)` で握りつぶさず `ApiCallMapper.FromException` へ変換している。  
11. 新規 endpoint を追加した場合、`Adapter/Internal/Operations/Operations.cs` を更新した。  
12. 新規 endpoint を追加した場合、`docs/inventory/endpoints-*.md` を更新した。  
13. endpoint inventory 整合テスト（Raw.Endpoints.Tests/Inventory）が通過している。  
14. Wire endpoint テストで `endpointId/method/path/query/body` を検証している。  
15. Side/OrderType/Timestamp の新規語彙を追加した場合、対応 mapper の双方向変換テストを追加した。  
16. 既存 endpoint の `EndpointId` 文字列を変更していない（変更時は理由をPR本文に記載）。  
17. 新規 public endpoint 追加時、`PublicClient` 経由の呼び出し導線を追加した。  
18. 新規 private endpoint 追加時、`PrivateApi`（または配下Api）経由の呼び出し導線を追加した。  
19. 追加した DTO/Request の命名は既存 `GetXxx/PostXxx + Request/Response` 規約に一致している。  
20. 追加したテストfixture/stubに重複がある場合、共通化候補としてPR本文に明記した。  

---

## 参考: 今回の確認で特に「同形増殖」が強い領域
- Adapter層の Call骨格（Public/Private API）
- Wire層の endpoint 定義（Ids/Catalog/Traits/Endpoints）
- Raw層の Request/DTO ペア
- Normalized/Adapter の mapper scalar変換
- Exchange別テストの assertion helper / inventory整合テスト

