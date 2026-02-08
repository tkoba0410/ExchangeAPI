# REVIEW-02: メソッド引数の順番・形の統一性レビュー

目的: 将来の取引所追加・endpoint追加時の保守性と事故防止を主軸に、引数設計の一貫性を評価する。
前提: 命名の是非は REVIEW-01 に委譲し、本レビューでは扱わない。

---

## 対応状況メモ（2026-02-08）

- `P1-1` のうち Bittrade Normalized 境界の一部を DTO 受けに寄せた。
- 対象: `GetOrders` / `PostOrdersSubmitCancelByOrderId` / `GetOpenOrders` / `GetOrdersByOrderId` / `GetMatchResults`。
- `NormalizedApi -> NormalizedPrivateApi` の委譲は、上記操作で `request` をそのまま渡す形へ変更した。
- `Adapter` 側の呼び出し（`TradingApi` / `SpotHistoryApi`）も DTO 生成経由に追従した。
- `P1-3` について、`OrdersRequest.Cursor` / `ExecutionsPrivateRequest.Cursor` が指定された場合は
  `Bittrade/Bitflyer Adapter.Private` で `NotSupported` を明示的に返すガードを追加した（黙殺を解消）。
- `P1-2` について、`BalanceRequest` を受ける公開メソッドで request 未使用だった経路を修正し、
  `ExchangeClient -> Private/Account API` で request を受け渡す形に揃えた。
- `P2-3` について、Bittrade の配線境界で `accountId` を `AccountId` に変換してから内部へ渡すよう変更し、
  `ApiBundle.FromRestClient` の内部境界は `AccountId?` 受けに統一した。
- `P2-3` の追加対応として、`IApiCredentialProvider` の境界を `Get(AccountId accountId)` に統一し、
  Bittrade/Bitflyer の Composition から provider 呼び出し時に `AccountId` を渡す形へ変更した。
- `P1-1` の追加対応として、Bittrade/Bitflyer の `ExchangeClient -> Trading/PrivateApi` 境界における
  `OrderLimitAsync` / `CancelOrderAsync` の委譲をプリミティブ分解から DTO 受けへ統一した。
- `P2-2` の追加対応として、Bittrade Normalized の retail 系 request で `direct/status` を
  `int` から `RetailOrderDirection` / `RetailOrderStatus` に型昇格し、Raw 直前でのみ数値へ変換する形に変更した。
- `P2-2` の追加対応として、Bittrade Normalized の request で `type/amount/fee/from/size` を
  `RetailOrderType` / `RetailOrderAmount` / `WithdrawAmount` / `WithdrawFee` / `RequestFrom` / `RequestSize`
  に型昇格し、Raw 直前でのみプリミティブへ変換する形に変更した。
- `P2-2` の追加対応として、残差分だった `GetMatchResultsRequest.Limit` / 
  `PostOrdersBatchCancelOpenOrdersRequest.Size` / `Public.GetHistoryKlineRequest.Size` を
  `RequestSize` / `Size` / `RequestSize` へ型昇格した。
- `P2-3` の追加対応として、Bittrade Normalized の内部境界で `accountId` を `FreeText` ではなく
  `AccountId` で受け渡すよう統一し、`NormalizedApi/NormalizedPrivateApi/NormalizedComponentFactory/ApiBundle`
  間の配線を VO 境界へ揃えた。
- `P2-3` の追加対応として、`WindowsCredentialManagerApiCredentialProvider` の内部 `BuildTarget` 境界と、
  Bittrade 旧 `ClientFactory` の internal `CreatePrivate` 境界を `AccountId` 受けへ統一した。
- `P2-3` の追加対応として、Bitflyer Normalized の `SymbolMapper/CommonMapper` で
  `productCode` の受け渡しを `string` から `ProductCode` に統一した（内部 mapper 境界の文字列流通を縮小）。
- `P2-2` の追加対応として、Bitflyer Normalized の request で `count/before/after` を
  `RequestCount/RequestBefore/RequestAfter` に型昇格し、Raw 呼び出し直前でのみプリミティブへ変換する形に変更した。
- `P2-2` の追加対応として、Bitflyer Normalized `WithdrawRequest` の
  `bankAccountId/amount` を `BankAccountId/WithdrawAmount` に型昇格した。

---

## A. 引数設計の「統一ルール案 v1」

### 0) 適用スコープ
- **Public Contract / Facade (`Contracts/Facade`)**: 外部公開境界
- **Exchange Adapter の外部公開メソッド**: `ExchangeClient` から到達する API
- **Normalized / Mapper / Normalizer**: 同形メソッドが多く、揺れが増幅する層

### 1) 公開境界のシグネチャ規約
- 原則: **`Task<Call<TRequest, TResponse>> XxxAsync(TRequest request, CancellationToken cancellationToken = default)`** に統一。
- 例外: なし（引数なし API でも空 request DTO を使用）。
- `CancellationToken` は **必ず末尾**、引数名は **`cancellationToken` に固定**。

### 2) DTO vs プリミティブ規約
- **Facade / ExchangeClient 公開境界**: DTO のみ許容。
- **Adapter 内部境界（MarketApi/TradingApi/SpotHistoryApi など）**: DTO を第一引数として受け、必要なら直下で分解。
- **Raw 層以外**では業務意味を持つ `string` を禁止し、`Symbol / ProductCode / OrderKey / FreeText` などに変換済みで受ける。

### 3) 引数順序規約（DTOフィールド順も同順）
1. 識別子（`symbol` / `product` / `market` / `orderKey`）
2. 期間（`from/to` または `before/after`）
3. ページング（`limit` / `size` / `cursor`）
4. オプション（flags / options / filter）
5. `CancellationToken cancellationToken = default`

### 4) 型ルール（プリミティブ抑制）
- Raw 層以外（Normalized / Adapter / Contracts）では、業務意味を持つ `string` / `int` / `long` / `decimal` を直接使わない。
- 原則として **Normalized 以降は全項目を VO/enum 化**する。
- 例外は `DateTimeOffset` / `bool` / `CancellationToken` / コレクションのみ。
- `string` は外部I/O境界（Raw HTTP/JSON、設定入力）でのみ使用し、境界通過時に VO/enum へ変換する。
- 例外運用が必要な場合は `docs/exceptions.md` に記録する。

### 5) Optional 表現規約
- Optional は **DTO内 nullable + default 値**を標準にし、公開メソッドでの過剰 overload を抑制。
- `nullable` / `default value` / `overload` を同一責務で多重化しない。
- “primitive convenience overload” を置く場合は **Facade Extensions のみに限定**（実装層には置かない）。

### 6) Cross-Exchange 規約
- 同じ業務操作（Ticker/Board/Orders/Executions/Order/Cancel）は、取引所ごとに
  - 引数の形（DTOかプリミティブか）
  - Optional の運び方（DTOかメソッド引数か）
  - CT 命名
  を揃える。

---

## B. 逸脱箇所一覧

### 1)
- **Issue:** Facade では DTO 受けだが、Adapter 内部 API がプリミティブ受けになっており、同一操作で引数形が分裂している。
- **Evidence:**
  - `src/Exchanges/Bitflyer/Adapter/Private/Api/ExchangeClient.cs` `OrderLimitAsync(OrderLimitRequest request, ...)` が `PrivateApi.OrderLimitAsync(request.Symbol, request.Side, request.Size, request.Price, ...)` へ分解委譲。
  - `src/Exchanges/Bitflyer/Adapter/Private/Api/PrivateApi.cs` `OrderLimitAsync(Symbol symbol, Side side, Size size, Price price, ...)`。
  - `src/Exchanges/Bittrade/Adapter/Private/Api/ExchangeClient.cs` `OrderLimitAsync(OrderLimitRequest request, ...)` が `TradingApi.OrderLimitAsync(request.Symbol, request.Side, request.Size, request.Price, ...)` へ分解委譲。
  - `src/Exchanges/Bittrade/Adapter/Private/Api/TradingApi.cs` `OrderLimitAsync(Symbol symbol, Side side, Size size, Price price, ...)`。
- **Why it matters:** DTO に新項目が追加された際、途中層で引数展開漏れが起きやすい。将来の endpoint 追加時に “渡したつもりで渡っていない” 事故を誘発する。
- **Proposed rule:** ExchangeClient 以降も DTO 受けを維持し、プリミティブ展開は最下流直前に限定する。
- **Severity:** P1

### 2)
- **Issue:** `OrdersRequest` / `ExecutionsPrivateRequest` の `Cursor` が Adapter 実装で実質未使用（limit だけで処理）。
- **Evidence:**
  - `src/Contracts/Facade/Requests/OrdersRequest.cs` は `Cursor? Cursor` を保持。
  - `src/Contracts/Facade/Requests/ExecutionsPrivateRequest.cs` は `Cursor? Cursor` を保持。
  - `src/Exchanges/Bitflyer/Adapter/Private/Api/PrivateApi.cs` `GetOrdersAsync` / `GetExecutionsPrivateAsync` は下流呼び出し時に `request.Symbol` のみを使用し、`GetLimits(...)` でも `Limit` のみ参照。
  - `src/Exchanges/Bittrade/Adapter/Private/Api/SpotHistoryApi.cs` `GetOrdersAsync` / `GetExecutionsPrivateAsync` は `request.Symbol` + `request.Limit` までで、`GetLimits(...)` も `Limit` のみ参照。
- **Why it matters:** API 契約として cursor pagination を想定して見える一方、実装が追従していないため、取引所追加時に誤解・実装漏れ・不完全互換の温床になる。
- **Proposed rule:** `Cursor` を契約に残すなら全 exchange 実装で “使用・無視理由・NotSupported 応答” のいずれかを明示する。未対応が確定なら DTO から除去。
- **Severity:** P1

### 3)
- **Issue:** `CancellationToken` の命名が `cancellationToken` と `ct` で混在している。
- **Evidence:**
  - `src/Contracts/Facade/Interfaces/IPublicApi.cs` / `IPrivateApi.cs` は `cancellationToken`。
  - `src/Exchanges/Bitflyer/Normalized/Private/Api/NormalizedPrivateApi.cs` は `cancellationToken`。
  - `src/Exchanges/Bittrade/Normalized/Api/IBittradeNormalizedApi.cs` / `Normalized/Private/Api/NormalizedPrivateApi.cs` / `Normalized/Public/Api/NormalizedPublicApi.cs` は `ct`。
- **Why it matters:** 大規模検索・置換・Analyzer 運用時に揺れがノイズ化し、規約自動化しにくい。
- **Proposed rule:** すべて `cancellationToken` に統一（ローカル変数短縮は許容しても、公開シグネチャは固定）。
- **Severity:** P2

### 4)
- **Issue:** Optional 表現で `DTO + extension overload + 実装層のプリミティブ引数(default付き)` が重なり、同一責務の入口が多重化している。
- **Evidence:**
  - `src/Contracts/Facade/Extensions/PrivateApiExtensions.cs` と `PublicApiExtensions.cs` に convenience overload。
  - `src/Exchanges/Bittrade/Adapter/Public/Api/MarketApi.cs` `GetCandlesticksAsync(Symbol symbol, PeriodDto period, int? size = null, ...)` のように実装層でもプリミティブ optional を再定義。
  - `src/Exchanges/Bittrade/Normalized/Extensions/NormalizedApiExtensions.cs` でさらに多数の primitive overload を提供。
- **Why it matters:** 追加項目が入るたびに同等の forwarding が多層で増殖し、ボイラーコードと変更漏れのリスクが上がる。
- **Proposed rule:** overload は Facade Extensions のみに限定し、Adapter/Normalized 実装は request DTO の単一路線に寄せる。
- **Severity:** P1

### 5)
- **Issue:** 下位層でプリミティブ（`int`, `decimal`, `long`）が業務値として露出し、型での制約表現が弱い（規約違反候補）。
- **Evidence:**
  - `src/Exchanges/Bittrade/Normalized/Private/Requests/TradingRequests.cs` `PostOrdersBatchCancelOpenOrdersRequest` で `Size` が `decimal?`。
  - 同ファイル `GetMatchResultsRequest` で `Limit` が `int?`。
  - `src/Exchanges/Bittrade/Normalized/Public/Requests/MarketDataRequests.cs` `GetHistoryKlineRequest` で `Size` が `int?`。
- **Why it matters:** 値域や単位がシグネチャから読めず、exchange 差分吸収時に誤値混入を型で防げない。
- **Proposed rule:** `direct/status/from/size/amount/fee` などは VO / enum / 専用 record に昇格し、Raw 境界でのみプリミティブに落とす。Normalized 入口で昇格し、下流ではプリミティブを禁止する。
- **Severity:** P2

### 6)
- **Issue:** Request を受ける公開メソッドが実質 request を利用せず、API 形状だけを満たしている箇所がある。
- **Evidence:**
  - `src/Exchanges/Bitflyer/Adapter/Private/Api/ExchangeClient.cs` `GetBalanceAsync(BalanceRequest request, ...) => _privateApi.GetBalanceAsync(...)`（request 未使用）。
  - `src/Exchanges/Bittrade/Adapter/Private/Api/ExchangeClient.cs` `GetBalanceAsync(BalanceRequest request, ...) => _accountApi.GetBalanceAsync(...)`（request 未使用）。
- **Why it matters:** 将来 `BalanceRequest` にオプションが追加された際、使用漏れに気付きにくい。
- **Proposed rule:** request 未使用メソッドには「空 request 固定」をコメント/属性で明示するか、引数なし DTO 方針（`new BalanceRequest()`内包）を一段整理する。
- **Severity:** P2

---

## C. ボイラーコードを生んでいる引数パターンのグルーピング

### Group G1: DTO → プリミティブ展開フォワード
- 典型: `ExchangeClient(Contract DTO)` → `Private/Market/Trading API(primitive list)`。
- 影響: DTO項目追加時に全フォワーダー修正が必要。
- 代表例: `OrderLimitAsync`, `CancelOrderAsync`, `GetCandlesticksAsync`。

### Group G2: Optional の多重入口（DTO + extension overload + implementation primitive default）
- 典型: 同一機能に 2〜3 種類の入り口。
- 影響: 仕様追加時の修正点が散らばり、回帰漏れが増える。
- 代表例: Facade Extensions + Bittrade Normalized Extensions + Adapter メソッド default 引数。

### Group G3: ページング契約と実装差
- 典型: DTO には `Cursor` があるが実装は `Limit` のみ。
- 影響: “対応済みの見かけ” による誤利用。
- 代表例: `OrdersRequest`, `ExecutionsPrivateRequest` の Adapter 実装。

### Group G4: CT 命名揺れ
- 典型: `cancellationToken` と `ct` の混在。
- 影響: 静的チェック導入時の例外対応が増える。

### Group G5: プリミティブ業務値の滞留
- 典型: `int direct`, `decimal amount`, `long from`。
- 影響: 型でドメイン制約を表現できず、レビュー依存が強くなる。

---

## 総評
- Facade 層の「`Request DTO + CancellationToken`」は概ね維持されており、方向性は良い。
- ただし Exchange 実装層・Normalized 拡張層で引数設計が多重化しており、**将来の endpoint 追加時に最も事故を生みやすいのは “DTOとプリミティブの往復フォワード”**。
- 次アクションは、まず **(1) DTO受けの層境界固定**, **(2) Cursor 契約の実装整合**, **(3) CT 命名統一** の3点を規約化するのが効果的。

---

## P2-1（CancellationToken 命名揺れ）解決方針

### ルール
- `CancellationToken` 引数名は **`cancellationToken` に固定**する。
- `CancellationToken` 引数は **末尾配置**とする。
- 既定値は原則 **`= default`** とする。

### 段階移行
1. Phase 1（即時）: `internal` / `private` の `ct` を `cancellationToken` に統一。
2. Phase 2（計画）: `public` / `protected` は named argument 互換性を考慮し、メジャー更新タイミングで統一。
3. Phase 3（固定化）: CI で `ct` 新規流入を禁止し、最終的に全層で `cancellationToken` のみ許可。

### 互換性メモ
- C# では引数名変更が named argument 利用者に破壊的影響を与えるため、公開APIは段階移行を前提とする。

### 検査ルール（CI）
- `CancellationToken` 引数名が `cancellationToken` 以外なら失敗。
- `CancellationToken` が末尾でない場合は失敗。

---

## 運用ルール（追加）

1. 新規 endpoint 追加時は、Raw Request/Response から Normalized Request/Dto へ変換する時点で VO/enum 化を完了させる。
2. Normalized 以降で primitive を新規導入した PR は原則差し戻す。
3. 既存 primitive はバックログ管理し、型昇格の対象・優先度・解消期限を明記する。
