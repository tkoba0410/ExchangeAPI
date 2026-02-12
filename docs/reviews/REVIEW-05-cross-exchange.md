# REVIEW-05: Bitflyer / Bittrade Cross-Exchange Parallelism Review

目的: 取引所間（Bitflyer / Bittrade）で「並列性・対称性」が保たれているかを確認し、将来の取引所追加・endpoint追加時の事故防止と保守コスト最小化に資する提案を整理する。

前提: REVIEW-01（命名）/ REVIEW-02（引数）/ REVIEW-03（実装パターン）/ REVIEW-04（層責務・依存境界）は再評価しない。

---

## A. 並列にすべき標準形（Standard Form）

1. 取引所配下は `Wire / Raw / Normalized / Adapter / Application / Composition` の骨格を同形で保持する。  
2. 各層は `Public / Private` を第一分類とし、同名責務の入口（API/Facade）の粒度を揃える。  
3. Wire は `EndpointIds / Paths / EndpointTraits / EndpointIdCatalog / PublicEndpoints / PrivateEndpoints` を固定編集点とする。  
4. Raw は inventory の `EndpointId` と 1:1 で `*CallAsync` を持ち、Wire 経由実行を強制する。  
5. Normalized は `NormalizedPublicApi / NormalizedPrivateApi` の2分割を標準とし、NotSupported は `Internal/NotSupported` に統一する。  
6. Adapter は Contracts 入口責務に限定し、`ApiCallMapper` を必ず通して `component` と `errorKind` の比較可能性を担保する。  
7. `component` 命名は `<Exchange>.<Domain>.<Operation>` を標準とし、Domain 語彙（MarketData/Trading/Account/History/ExchangeInfo）を固定する。  
8. 同一 Contracts endpoint は、Adapter 内部境界で Request DTO を保持し、下流直前でのみ primitive 分解を許容する。  
9. 片側のみ存在する endpoint は仕様差として許容するが、inventory で「なぜ片側のみか」を1文で説明可能にしておく。  
10. endpoint 追加時は `Wire -> Raw -> Normalized -> Adapter` の4層に同じ `EndpointId` トレースを残す。  
11. テストは両取引所で同系統（EndpointId整合 / WireRequestAssertions / ApiCallMapper）を必須セットにする。  
12. 例外（非対称）を入れる場合は「仕様差」「運用差」「実装都合」に分類し、実装都合は原則解消対象にする。

---

## B. 差分一覧

### B-1. 揃えるべき

- Issue: Adapter Public の委譲境界が非対称（Bitflyer は Request DTO をそのまま渡すが、Bittrade は `Symbol` へ早期分解している）。
- Evidence: `Bitflyer/Adapter/Private/Api/ExchangeClient.cs` は `GetTickerAsync(request)` 等で DTO 委譲。`Bittrade/Adapter/Private/Api/ExchangeClient.cs` は `GetTickerAsync(request.Symbol)` / `GetBoardAsync(request.Symbol)` / `GetExecutionsPublicAsync(request.Symbol)`。`Bittrade/Adapter/Public/Api/MarketApi.cs` も `GetTickerAsync(CommonSymbol symbol)` 入口を採用。
- Why it matters: 新規 endpoint 追加時に、どこで DTO を崩すかの判断が取引所依存となり、同一機能でも実装レビュー軸が増える。
- Proposed rule: Contracts 入口～Adapter Public API 境界では Request DTO を保持し、primitive 取り出しは Normalized 呼び出し直前に限定する。
- Severity: P1

- Issue: Adapter テスト命名が非対称（Bittrade 側のみクラス名に取引所接頭辞が無い）。
- Evidence: `tests/Exchanges/Bitflyer/Adapter.Tests/BitflyerApiCallMapperTests.cs` は `BitflyerApiCallMapperTests`。`tests/Exchanges/Bittrade/Adapter.Tests/BittradeApiCallMapperTests.cs` はファイル名に対してクラス名が `ApiCallMapperTests`。
- Why it matters: 横断検索・テンプレート複製時に機械的抽出がしづらくなり、将来取引所追加時のテスト雛形整備コストが上がる。
- Proposed rule: 取引所別テストクラス名は `<Exchange><Subject>Tests` へ統一する。
- Severity: P2

### B-2. 非対称許容

- Issue: Public/Private endpoint 数・種類が非対称（Bitflyer: health/board-state/funding 系、Bittrade: retail/withdraw virtual 系）。
- Evidence: `Wire/Constants/EndpointIdCatalog.cs` で、Bitflyer は `GetHealth` `GetBoardState` `GetFundingRate` 等、Bittrade は `PostRetailOrder*` `PostWithdrawVirtual*` 等を保持。inventory でも同差分を確認可能。
- Why it matters: これは API 提供仕様そのものの差であり、無理な対称化は抽象化過剰を招く。
- Proposed rule: 「同義 endpoint がある範囲のみ」構造対称を強制し、固有 endpoint は exchange 専用 namespace のまま維持する。
- Severity: P2

- Issue: Normalized 生成時の前提が非対称（Bittrade は `AccountId` 必須、Bitflyer は不要）。
- Evidence: `Bittrade/Normalized/Api/NormalizedApi.cs` の `FromRaw(IRawApi, IMarketResolver, AccountId)` は `accountId.IsEmpty` を例外化。`Bitflyer/Normalized/Api/NormalizedApi.cs` は `FromRaw(IRawApi, IMarketResolver)`。
- Why it matters: 認証・口座モデルの取引所仕様差であり、同一化対象ではない。
- Proposed rule: 口座識別が必須な取引所は Factory/Bundle で fail-fast を必須化し、レビュー時は「仕様差」として明記する。
- Severity: P2

### B-3. 不要（現時点で追加対応不要）

- Issue: endpointId/path/method の横断比較基盤。
- Evidence: 両取引所に `Raw.Endpoints.Tests/WireRequestAssertions.cs` と inventory 整合テスト（`*InventoryEndpointIdConsistencyTests.cs` / `*EndpointIdApiNamingTests.cs`）が存在。
- Why it matters: 最低限の比較可能性は確保済みで、追加投資よりも既存テスト維持が優先。
- Proposed rule: 新規取引所では同テスト3点セットを初期必須テンプレートとして複製する。
- Severity: P2

- Issue: `errorKind` 横断比較。
- Evidence: 両取引所に `Adapter.Tests/*ApiCallMapperTests.cs` があり、`CallErrorKind -> ExchangeErrorCategory` の対応を同様に検証している。
- Why it matters: 横断メトリクス比較の基礎は既にあるため、現時点で新規メトリクス追加は不要。
- Proposed rule: 新規取引所でも同一テストケース（Http/Transport など）を最低ラインとして導入する。
- Severity: P2

---

## C. 取引所追加時チェックリスト案（REVIEW-05）

1. `Wire/Raw/Normalized/Adapter/Application/Composition` の骨格が既存2取引所と同形か。  
2. `Public/Private` 第一分類を維持しているか。  
3. `EndpointIdCatalog` と inventory が一致し、`Wire -> Raw -> Normalized` で同じ `EndpointId` が追跡できるか。  
4. `WireRequestAssertions`・Inventory整合テスト・ApiNamingテストの3点セットがあるか。  
5. `Adapter/Public` 入口は Request DTO を受け、primitive 分解は下流直前に限定されているか。  
6. `Normalized/Internal/NotSupported` が存在し、NotSupported 応答生成が集中管理されているか。  
7. `Operations` の Domain 語彙（MarketData/Trading/Account/History/ExchangeInfo）が一致しているか。  
8. `component` が `<Exchange>.<Domain>.<Operation>` 形式か。  
9. `ApiCallMapperTests` で `CallErrorKind -> ExchangeErrorCategory` 対応を検証しているか。  
10. 仕様差による非対称は inventory かレビュー文書に理由（1文）を残しているか。

---

## 総括

- 全体として、層構造・EndpointIdトレース・NotSupported・エラー分類テストは高い並列性を維持できている。  
- 主な改善余地は、**Adapter Public 境界での DTO 保持ルールの統一**（P1）であり、ここを固定すると将来の取引所追加時レビューコストが最小化される。
