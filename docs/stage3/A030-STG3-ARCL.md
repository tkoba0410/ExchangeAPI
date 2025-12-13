# A030-STG3-ARCL Stage3 レイヤ構成（send child order 縦スライス）

## 1. 本文書の目的
Stage3 では、Private POST（`/v1/me/sendchildorder`）を end-to-end で通すために、
**Trading（発注系）の縦スライスを成立させるためのレイヤ構成と責務分担** を定義する。

Stage2 で確立した GET ベースの構造をそのまま継承しつつ、
POST（body 付き / 署名あり）に対応できる形へ拡張することが目的である。
※ Stage4 以降の命名: ExchangeApi.Contracts（旧 Abstractions）、ExchangeApi.Transport（旧 Infrastructure/Protocol）、Exchange.Bitflyer（旧 Bitflyer）、ExchangeApi.Factory（旧 Orchestration）

---

## 2. レイヤ構成（全体像）
Stage3 のレイヤ構成を以下に示す。

```
ExchangeApi.Contracts（旧 ExchangeApi.Contracts）
   ├─ Domain（Balance / OrderRequest / OrderResult）
   └─ Interfaces（IExchangeAccountClient / IExchangeTradingClient / IExchangeClient）

ExchangeApi.Transport（旧 ExchangeApi.Transport）
   ├─ IExchangeClock / SystemClock
   ├─ IRequestSigner（GET + POST 署名対応）
   ├─ IRestClient
   │     ├─ GetAsync<T>()
   │     └─ PostAsync<TReq, TRes>()
   └─ ExchangeApiException（共通例外）

ExchangeApi.Factory（旧 ExchangeApi.Factory、Credential Provider）
   ├─ ApiCredentials
   ├─ IApiCredentialProvider
   └─ Provider 実装（Environment / Windows / Composite など）

Exchange.Bitflyer (Private API)
   ├─ IBitflyerPrivateApi（Account 系: balance など）
   ├─ IBitflyerPrivateTradingApi（Trading 系: sendchildorder）
   ├─ DTO: BitflyerBalanceResponse（既存）
   ├─ DTO: BitflyerSendChildOrderRequest / Response（Stage3）
   └─ BitflyerPrivateApi（Account + Trading の両方を単一クラスで実装）

Exchange.Bitflyer (Adapter)
   └─ BitflyerExchangeClient : IExchangeClient
         ├─ GetBalancesAsync（Stage2）
         └─ SendOrderAsync（Stage3）
```

依存方向は必ず **上位 → 下位** に限定し、循環参照を禁止する。

---

## 3. レイヤ別の責務定義

### 3.1 ExchangeApi.Contracts
#### ■ Domain モデル
- `OrderSide`（Buy / Sell）
- `OrderType`（Market のみ、将来拡張可）
- `OrderRequest`（ProductCode / Side / Size）
- `OrderResult`（OrderId）
- Stage2 の `Balance` も引き続き保持

#### ■ 抽象インターフェース
- `IExchangeTradingClient.SendOrderAsync`
- `IExchangeAccountClient.GetBalancesAsync`
- `IExchangeClient` は Account + Trading の統合インターフェース

Abstractions 層は **取引所固有の仕様を一切含まず、純粋なドメイン表現のみ** を提供する。

---

### 3.2 ExchangeApi.Transport（Protocol + Transport）
#### ■ IRequestSigner
- bitFlyer の署名仕様を実装するインターフェース。
- GET／POST の差分を吸収し、以下を担う：
  - `ACCESS-TIMESTAMP`
  - `ACCESS-KEY`
  - `ACCESS-SIGN`
  - `Content-Type: application/json`
- 署名対象：`timestamp + method + path + body`
  - GET の場合 body は空文字列
  - POST の場合 body は JSON 文字列

#### ■ IRestClient / RestClient
- GET / POST の共通 HTTP 呼び出しロジックを提供する。
- Query / Body の違いは RestClient 内で吸収し、Private API には漏らさない。
- 責務：
  - `GetAsync<T>(path, query, ct)`
  - `PostAsync<TReq, TRes>(path, body, ct)`
  - 署名付与（IRequestSigner）
  - JSON シリアライズ / デシリアライズ
  - HTTP エラーを `ExchangeApiException` に統一

#### ■ ExchangeApiException
- HTTP ステータスコード・エラー情報を保持
- Stage3 では E1 レベル（HTTP ベース）の例外分類のみ

---

### 3.3 ExchangeApi.Factory（Credential Provider）
- API key / secret を取得する責務を専任させる層。
- REST / Private API 層が資格情報取得をしないように分離する。
- Provider の実装は環境変数／Windows Credential Manager／Composite など複数用意可能。

---

### 3.4 Exchange.Bitflyer.Private（Private API 層）
#### ■ 役割ベースのインターフェース
- `IBitflyerPrivateApi`（Account 系）
  - `GetBalancesAsync()`（Stage2）
  - 将来：`GetCollateralAsync()`, `ListPositionsAsync()`

- `IBitflyerPrivateTradingApi`（Trading 系）
  - `SendChildOrderAsync()`（Stage3）
  - 将来：`CancelChildOrderAsync()` など

#### ■ DTO
- Stage2 の `BitflyerBalanceResponse`
- Stage3 の `BitflyerSendChildOrderRequest`
- Stage3 の `BitflyerSendChildOrderResponse`

#### ■ BitflyerPrivateApi（実装クラス）
- Account (`IBitflyerPrivateApi`) と Trading (`IBitflyerPrivateTradingApi`) の両インターフェースを単一クラスで実装
- RestClient の GET / POST を呼び出すだけに徹し、**変換ロジックや例外処理ロジックは持たない**
- Private API 層は「bitFlyer の HTTP API を忠実に呼び出すこと」だけが責務

---

### 3.5 Exchange.Bitflyer.Adapter（ExchangeClient 層）
#### ■ BitflyerExchangeClient
- `IExchangeTradingClient` の実装として `SendOrderAsync` を提供
- `IBitflyerPrivateTradingApi` を呼び、DTO → Domain 変換を行う
- Stage2 の `GetBalancesAsync`（Account 系）も同居
- **API の選択ロジックやドメイン変換のみを担当し、HTTP ロジックは Infrastructure 層に委譲**

Adapter は「bitFlyer 固有表現（DTO）」と「抽象ドメイン表現（OrderRequest / OrderResult）」の橋渡しが唯一の責務。

---

## 4. 依存関係ルール
- Abstractions →（依存なし）
- Infrastructure → Abstractions のみ参照（取引所依存なし）
- Orchestration → Abstractions
- Bitflyer.Private → Infrastructure のみ参照
- Bitflyer.Adapter → Bitflyer.Private + Abstractions
- Factory（組み立て）→ すべてを最終的に構築するルート

**循環依存禁止。**

---

## 5. Stage3 におけるレイヤ境界の注意点
- Account / Trading という「役割ベース」の分割を優先し、HTTP メソッド（GET / POST）でクラスを分割しない。
- Query パラメータ / Body パラメータの違いは RestClient・Signer に隠蔽する。
- Adapter 層は bitFlyer DTO に依存するが、Infrastructure 層は bitFlyer を知らない設計を維持する。
- Domain / Abstractions は他取引所でも流用できる構造を保つ（OrderSide / OrderType 等）。

---

## 6. Stage3 の完了条件（レイヤ観点）
- Infrastructure が GET + POST をサポートし、署名処理も両対応できている。
- Bitflyer.Private API 層が `/v1/me/sendchildorder` を正しく呼び出せる。
- Adapter 層が DTO ⇄ Domain のマッピングを正しく行える。
- `BitflyerClientFactory.Create()` が残高取得（Stage2）と MARKET 発注（Stage3）の両縦スライスを構築可能。
- 依存構造が正しく分離され、循環がないことが確認済み。

---

## 7. Stage4 以降への展望
Stage3 で Trading 縦スライスを確立したことで、以下の拡張が可能となる：

- Private GET の横展開（collateral / positions / executions）
- Private POST の拡張（cancelchildorder / cancelallchildorders）
- LIMIT / STOP / IFDOCO 等の注文種別
- エラー処理 E2 以降（bitFlyer 固有コード対応 / リトライ / 取引所別例外）

Stage3 は「最小の MARKET 注文が end-to-end で通る Trading 基盤を確立するステージ」であり、
以降の Trading API 拡張のすべての雛形となる。
