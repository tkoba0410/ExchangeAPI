# A030-STG2-ARCL Stage2 レイヤ構成（get balance）

## 1. 本文書の目的
Stage2（get balance）において、必要となるレイヤ構成と責務分担を明確化し、
後続ステージ（Collateral / Positions / POST Private API）の拡張に耐えられる基盤構造を定義する。

本ドキュメントは、**Abstractions → Infrastructure → Bitflyer.Raw → Bitflyer.Adapter** の流れを整理し、
依存方向・レイヤ境界・役割を統一的に示すことを目的とする。

---

## 2. レイヤ構成（全体像）
Stage2 時点で必要となるレイヤと責務を以下に示す。

```
ExchangeApi.Abstractions
   ├─ Domain（Balance）
   └─ Interfaces（IExchangeAccountClient, IExchangeClient）

ExchangeApi.Infrastructure
   ├─ IExchangeClock / SystemClock
   ├─ IRequestSigner（bitFlyer 署名アルゴリズム）
   ├─ IRestClient / RestClient（署名付き GET）
   └─ ExchangeApiException

ExchangeApi.Bitflyer (Raw API)
   ├─ DTO: BalanceResponse
   ├─ Raw API IF: IBitflyerRawApiClient
   └─ Raw API 実装: BitflyerRawApiClient

ExchangeApi.Bitflyer (Adapter)
   ├─ Mapper: BitflyerDtoMapper
   └─ ExchangeClient: BitflyerExchangeClient
       └─ GetBalancesAsync（Stage2 完了対象）
```

依存方向は必ず上位→下位に制限し、循環依存を禁止する。

---

## 3. レイヤ別の責務（詳細）

### 3.1 ExchangeApi.Abstractions
#### ■ Domain モデル
- `Balance`（通貨コード・総残高・発注可能残高）
- イミュータブル record として定義し、取引所固有の仕様に依存しない。

#### ■ 抽象インターフェース
- `IExchangeAccountClient.GetBalancesAsync()` を定義。
- `IExchangeClient` は MarketData + Account を束ねる入口。
- ここでは **Private GET（balance）に必要な最低限のみ** 提供する。

---

### 3.2 ExchangeApi.Infrastructure（Protocol + Transport）
#### ■ IExchangeClock / SystemClock
- 署名生成に必要となる UTC 時刻を抽象化。
- テスト容易性の観点からインターフェースとする。

#### ■ IRequestSigner
- bitFlyer Private API の署名仕様に従い、リクエストに認証ヘッダを付与する。
- 責務：
  - timestamp の生成
  - access key / sign の付与
  - Content-Type の付与
- リクエストのメソッド（GET/POST）やパスを解釈して署名文字列を形成する。

#### ■ IRestClient / RestClient
- HTTP 呼び出しを統一する REST クライアント。
- 責務：
  - `HttpClient` を用いた GET 呼び出し
  - JSON シリアライズ/デシリアライズ
  - `IRequestSigner` による署名の適用
  - HTTP エラー（4xx/5xx）を `ExchangeApiException` として送出（E1）

#### ■ ExchangeApiException
- ステータスコード・メッセージを保持する共通例外クラス。
- Signer / RestClient / Raw API / Adapter のすべてで共通利用する。

---

### 3.3 ExchangeApi.Bitflyer.Raw（Raw API 層）
#### ■ DTO（BalanceResponse）
- bitFlyer のレスポンス構造をそのまま表現。
- Domain とは切り離して定義する（疎結合の維持）。

#### ■ Raw API インターフェース
```
Task<IReadOnlyList<BalanceResponse>> GetBalanceAsync(CancellationToken ct = default);
```
- Private GET 用の最小単位。

#### ■ Raw API 実装
- `RestClient.GetAsync` を内部で使用し、API パス `/v1/me/getbalance` を呼び出す。
- インフラ層に署名処理を委譲し、Raw 層は **bitFlyer の API 仕様にだけ従う**。

---

### 3.4 ExchangeApi.Bitflyer.Adapter（ExchangeClient 層）
#### ■ DTO → Domain 変換（Mapper）
- `BalanceResponse → Balance` の純粋変換のみ実施。
- エラー判定やロジックは行わない（責務の一貫性）。

#### ■ BitflyerExchangeClient
- `IExchangeAccountClient` の実装クラス。
- `GetBalancesAsync` の処理フロー：
  1. Raw API の `GetBalanceAsync` を呼ぶ。
  2. DTO を Domain に変換する。
  3. `IReadOnlyList<Balance>` を返す。
- 他の Private API（collateral, positions, orders 等）は Stage3 以降で実装する。

---

## 4. 依存関係ルール
- Abstractions →（インターフェース定義のみ）
- Infrastructure → Abstractions のみ参照可（取引所情報に依存しない）
- Bitflyer.Raw → Infrastructure のみ参照可（bitFlyer 専用処理）
- Bitflyer.Adapter → Bitflyer.Raw + Abstractions
- 組み立て（Factory）は依存グラフのルートとして扱う。

これにより、循環参照を防ぎ、取引所追加やテスト容易性が向上する。

---

## 5. Stage2 のレイヤ境界における注意点
- Private GET は **RestClient / RequestSigner / BitflyerRawApiClient** の 3 層で担当範囲が明確であること。
- Domain に取引所固有の項目を入れないこと（`Balance` は抽象化された値のみを保持）。
- Raw 層は **bitFlyer API に従うだけ** に徹し、例外処理ロジックを持たないこと。
- Adapter 層は **DTO → Domain の変換とインターフェース実装以外の責務を持たない** こと。

---

## 6. Stage2 の完了条件（レイヤ観点）
- Infrastructure が Private GET に必要な最低限の機能を提供している。
- Bitflyer.Raw が `/v1/me/getbalance` を正しく呼び出せる。
- Bitflyer.Adapter が `GetBalancesAsync` を実装し、end-to-end のデータ取得が完成している。
- 依存構造が循環せず、レイヤ境界が破綻していないことを確認済み。

---

## 7. Stage3 以降への展望
Stage2 で確立したレイヤ構造と実装パターンは、次の拡張にそのまま転用できる。
- Private GET 拡張（collateral / positions / executions）
- Private POST（sendchildorder / cancelchildorder）
- エラー処理強化（E2 以降の対応）
- マルチ取引所への横展開

Stage2 は「最初の Private GET の縦スライス」を通すステージとして、最低限のレイヤ整合性を確立することを主目的とする。

