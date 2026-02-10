# REVIEW-05: Bitflyer / Bittrade Cross-Exchange Parallelism Review

目的: 取引所間（Bitflyer / Bittrade）で、将来の取引所追加・endpoint追加時に事故を起こしにくい「並列性・対称性」が保たれているかを点検する。

前提（再評価しない）: REVIEW-01（命名）/ REVIEW-02（引数）/ REVIEW-03（実装パターン）/ REVIEW-04（層責務・依存境界）。

---

## A. 並列にすべき標準形（Standard Form）

1. 交換所ごとに `Wire / Raw / Normalized / Adapter / Application / Composition` の骨格を同形で維持する。  
2. 各層は `Public / Private` を第一分類とし、意味分類（Market/Trading等）は API 実装内部の委譲単位に限定する。  
3. Wire の endpoint 定義は `EndpointIds / Paths / EndpointTraits / Endpoints` に閉じ、endpoint 追加時の編集点を固定化する。  
4. Raw は inventory の `EndpointId` と 1:1 で `*CallAsync` を持ち、`RawCallExecutor` 経由で Call を生成する。  
5. Normalized は Public/Private の具象サブ API に集約し、NotSupported は `Internal/NotSupported` の静的ヘルパに統一する。  
6. Adapter は Contracts への写像責務に限定し、`ApiCallMapper` と `Operations.*` を必ず通す。  
7. 交換所固有機能（endpoint有無・DTO shape差）は許容するが、同一責務の命名規則と入口形状は統一する。  
8. `component`（ログ/CallMeta）命名は `<Exchange>.<Domain>.<Operation>` の3階層を標準とし、Domain語彙を横断で固定する。  
9. `endpointId` は inventory 正本に一致し、Wireテストで endpointId/path/method を機械検証する。  
10. `errorKind` は Adapter テストで `CallErrorKind -> ExchangeErrorCategory` の対応を取引所横断で比較可能にする。  
11. 非対称は「仕様差」「運用差」「実装都合」を切り分け、仕様差以外は原則揃える。  
12. 新規取引所追加時は、同名責務（Wire executor / Raw API / Normalized API / Adapter facade）の命名テンプレートを必須適用とする。  

---

## B. 差分一覧（3分類）

### B-1. 揃えるべき（将来追加の足かせになる差分）

- Issue: 層内インターフェース命名の非対称（交換所プレフィックス有無）が混在している。  
- Evidence: `Raw` は `IRawApi`（Bitflyer）と `IBittradeRawApi`（Bittrade）、`Normalized` は `INormalizedApi` と `IBittradeNormalizedApi`、`Wire/Internal` は `IWireCallExecutor` と `IBittradeWireCallExecutor`。  
  （`src/Exchanges/Bitflyer/Raw/Api/IRawApi.cs` / `src/Exchanges/Bittrade/Raw/Api/IBittradeRawApi.cs` / `src/Exchanges/Bitflyer/Normalized/Api/INormalizedApi.cs` / `src/Exchanges/Bittrade/Normalized/Api/IBittradeNormalizedApi.cs` / `src/Exchanges/Bitflyer/Wire/Internal/WireCallExecutor.cs` / `src/Exchanges/Bittrade/Wire/Internal/WireCallExecutor.cs`）  
- Why it matters: 新規取引所追加時に「交換所名を型名へ付けるか」の判断が揺れ、DI 登録・検索・テンプレート化のコストが上がる。  
- Proposed rule: 交換所配下層の主I/F名は「常に非プレフィックス」または「常にプレフィックス」のどちらかへ全取引所で統一し、例外は `docs/exceptions.md` へ期限付きでのみ許可。  
- Severity: P1  

- Issue: Adapter の Private 公開面の分割粒度が非対称（Bitflyer は `PrivateApi` 一体、Bittrade は `TradingApi/AccountApi/SpotHistoryApi` 分割）。  
- Evidence: `Bitflyer` は `ExchangeClient` -> `PrivateApi` 単一委譲、`Bittrade` は `ExchangeClient` -> `TradingApi` / `AccountApi` / `SpotHistoryApi` へ分散。  
  （`src/Exchanges/Bitflyer/Adapter/Private/Api/ExchangeClient.cs` / `src/Exchanges/Bitflyer/Adapter/Private/Api/PrivateApi.cs` / `src/Exchanges/Bittrade/Adapter/Private/Api/ExchangeClient.cs` / `src/Exchanges/Bittrade/Adapter/Private/Api/TradingApi.cs` / `src/Exchanges/Bittrade/Adapter/Private/Api/AccountApi.cs` / `src/Exchanges/Bittrade/Adapter/Private/Api/SpotHistoryApi.cs`）  
- Why it matters: 同一 Contracts endpoint を追加する際に実装配置の判断が取引所依存になり、レビュー軸が増えて漏れを生む。  
- Proposed rule: Adapter の「Contracts 入口」の分割単位を全取引所で統一（単一 PrivateApi か固定3分割のどちらか）し、内部実装の委譲のみ可変とする。  
- Severity: P1  

- Issue: `Operations` の domain 命名語彙が非対称（`MarketData` vs `Market`、アクセシビリティも `internal` vs `public`）。  
- Evidence: Bitflyer `Operations.MarketData.*` / `internal static class`、Bittrade `Operations.Market.*` / `public static class`。  
  （`src/Exchanges/Bitflyer/Adapter/Internal/Operations/Operations.cs` / `src/Exchanges/Bittrade/Adapter/Internal/Operations/Operations.cs`）  
- Why it matters: `component` 軸でログ・メトリクス比較する際にドメイン単位の集計キーが割れ、横断比較を妨げる。  
- Proposed rule: `Operations` の Domain 語彙とアクセス修飾子を横断固定（例: `internal static class Operations` + `MarketData/Trading/Account/History/ExchangeInfo`）。  
- Severity: P1  

- Issue: `EndpointIdCatalog` の NotImplemented 表現が非対称（Bitflyer は空配列 + getter、Bittrade は定義自体なし）。  
- Evidence: Bitflyer は `NotImplemented = Array.Empty<string>()` と `GetNotImplementedEndpointIds()` を持つ一方、Bittrade は `GetAllEndpointIds()` のみ。  
  （`src/Exchanges/Bitflyer/Wire/Constants/EndpointIdCatalog.cs` / `src/Exchanges/Bittrade/Wire/Constants/EndpointIdCatalog.cs` / `docs/_references/exchange-parity-policy.md`）  
- Why it matters: 生成コード/監査スクリプトの分岐条件が増え、「未実装の表現」を交換所ごとに特別扱いする必要が出る。  
- Proposed rule: parity policy の記載どおり、`PresentIn=None` がない限り `NotImplemented` フィールド/メソッドは持たない。  
- Severity: P2  

### B-2. 非対称許容（仕様差として妥当）

- Issue: Public/Private endpoint の絶対数・種類が大きく異なる。  
- Evidence: inventory 上で Bitflyer は lightning API 系 endpoint 群、Bittrade は spot/retail/withdraw 系 endpoint 群を保持し、`PresentIn` は双方とも Wire/Raw/Normalized に整合。  
  （`docs/inventory/endpoints-bitflyer.md` / `docs/inventory/endpoints-bittrade.md`）  
- Why it matters: この差分自体は実装欠落ではなく、取引所仕様差。無理に同一 endpoint 群へ寄せると過剰抽象化になる。  
- Proposed rule: endpoint の有無・粒度差は inventory を正本として受容し、同義 endpoint のみ構造対称性を監査対象とする。  
- Severity: P2  

- Issue: Bittrade Normalized に `AccountId` が必須で、Bitflyer Normalized には同等要件がない。  
- Evidence: Bittrade `NormalizedApi.FromRaw(..., AccountId accountId)` と `AccountId` プロパティを持つ。Bitflyer `NormalizedApi.FromRaw(...)` は market resolver のみを要求。  
  （`src/Exchanges/Bittrade/Normalized/Api/NormalizedApi.cs` / `src/Exchanges/Bitflyer/Normalized/Api/NormalizedApi.cs`）  
- Why it matters: これは Bittrade private API の仕様制約反映であり、同一化対象ではない。  
- Proposed rule: 認証・口座識別に起因する前提差は「仕様差」として許容し、Adapter 入口で fail-fast（作成時検証）を必須化する。  
- Severity: P2  

- Issue: Private 操作の operation 数（例: Bitflyer の trading commission、Bittrade の retail order 系）が非対称。  
- Evidence: Operations 定義および inventory の private endpoint 一覧で、取引所固有 API が存在。  
  （`src/Exchanges/Bitflyer/Adapter/Internal/Operations/Operations.cs` / `src/Exchanges/Bittrade/Adapter/Internal/Operations/Operations.cs` / `docs/inventory/endpoints-bitflyer.md` / `docs/inventory/endpoints-bittrade.md`）  
- Why it matters: 仕様差なので許容すべきであり、ここを統一対象にすると Adapter が取引所固有機能を隠蔽しすぎる。  
- Proposed rule: Contracts に露出する共通操作のみを強制対称とし、固有機能は exchange 専用 API 名称空間で隔離する。  
- Severity: P2  

### B-3. 不要（現時点で追加対応不要）

- Issue: Raw/Wire の endpointId 検証テストの存在有無に関する追加タスク。  
- Evidence: 両取引所とも Raw.Endpoints.Tests に endpointId consistency と API naming テストが存在し、`WireRequestAssertions` で endpointId/path/method を検証している。  
  （`tests/Exchanges/Bitflyer/Raw.Endpoints.Tests/Inventory/BitflyerInventoryEndpointIdConsistencyTests.cs` / `tests/Exchanges/Bitflyer/Raw.Endpoints.Tests/Inventory/BitflyerEndpointIdApiNamingTests.cs` / `tests/Exchanges/Bittrade/Raw.Endpoints.Tests/Inventory/BittradeInventoryEndpointIdConsistencyTests.cs` / `tests/Exchanges/Bittrade/Raw.Endpoints.Tests/Inventory/BittradeEndpointIdApiNamingTests.cs` / `tests/Exchanges/Bitflyer/Raw.Endpoints.Tests/WireRequestAssertions.cs` / `tests/Exchanges/Bittrade/Raw.Endpoints.Tests/WireRequestAssertions.cs`）  
- Why it matters: endpointId 系の横断比較性は最低ラインを満たしており、レビュー観点としては維持で十分。  
- Proposed rule: 既存 endpointId 監査テストを gate として維持し、coverage 不足 endpoint のみ追加する。  
- Severity: P2  

- Issue: `errorKind` 変換検証の新規導入。  
- Evidence: 両取引所で `ApiCallMapperTests` が `CallErrorKind -> ExchangeErrorCategory` を同形式で検証している。  
  （`tests/Exchanges/Bitflyer/Adapter.Tests/BitflyerApiCallMapperTests.cs` / `tests/Exchanges/Bittrade/Adapter.Tests/BittradeApiCallMapperTests.cs`）  
- Why it matters: 最低限の比較可能性は既に担保されているため、即時の追加要件は低い。  
- Proposed rule: 既存テストをテンプレート化し、新規取引所では同名テストケース群を複製して開始する。  
- Severity: P2  

---

## C. 取引所追加時チェックリスト案（REVIEW-05）

1. `Wire/Raw/Normalized/Adapter/Application/Composition` の骨格が既存取引所と同形か。  
2. `Public/Private` 第一分類を崩していないか（意味分類フォルダの新設禁止）。  
3. `EndpointId` が inventory 正本と一致し、`Wire -> Raw -> Normalized` の `*CallAsync` が 1:1 か。  
4. `Wire/Constants`（EndpointIds/Paths/Traits/Catalog）の編集点が固定化されているか。  
5. 層内 I/F 命名規則（プレフィックス有無）を既存標準に一致させたか。  
6. `Internal/NotSupported` が存在し、NotSupported 表現が静的ヘルパ経由に統一されているか。  
7. Adapter の Contracts 入口構造（単一/固定分割）が既存標準に一致しているか。  
8. `Operations` の domain 語彙（MarketData/Trading/Account/History/ExchangeInfo）と公開範囲が一致しているか。  
9. `component` 文字列が `<Exchange>.<Domain>.<Operation>` 形式で一貫しているか。  
10. endpointId 整合テスト（Inventory consistency / Api naming / WireRequestAssertions）を追加し、既存2取引所と同種の失敗検知ができるか。  
11. `CallErrorKind -> ExchangeErrorCategory` の mapper テストを実装し、errorKind 比較可能性を確保したか。  
12. 仕様差による非対称は inventory/例外文書で理由を記録し、「揃えない理由」を1文で説明できるか。  

---

## 補足（レビュー総括）

- 現状は「層構造・Endpoint 1:1 検証・NotSupported の基本線」が概ね並列化されている。  
- 主要リスクは、**同責務の命名規則/入口粒度/メトリクス語彙の揺らぎ**であり、ここを標準化すれば新規取引所追加時の判断コストを大きく下げられる。  
