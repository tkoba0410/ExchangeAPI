# A010-STG2-OVER Stage2 ゴール定義（get balance）

> 状態: Stage2 は FIX 済み（変更凍結）。以降の変更は Stage3 以降で扱うこと。

## 1. Stage2 の目的

Stage2 では、Stage1 で整備した設計・インターフェースとレイヤ構造を前提に、
**bitFlyer Private API の最初の一歩として `/v1/me/getbalance` を抽象層まで通すこと**を目的とする。

具体的には、以下を満たす SDK 状態を Stage2 の完了とする。

- 抽象インターフェース `IExchangeAccountClient.GetBalancesAsync()` を経由して、
- bitFlyer の `/v1/me/getbalance` が呼び出され、
- `Balance` ドメインモデルの一覧として、
  - JPY 残高
  - BTC 残高
  を取得できること。

この Stage2 は、以降の Private GET（証拠金・ポジション・履歴）および POST（発注・キャンセル）を実装するための
**「Private API 呼び出しパターンのテンプレート確立ステージ」**と位置付ける。

---

## 2. スコープ（Stage2 でやること / やらないこと）

### 2.1 Stage2 で「やること」

1. **ドメインモデル（Abstractions）**
   - `Balance` の定義
     - `Currency`（例: "JPY", "BTC"）
     - `Amount`（総残高）
     - `Available`（発注可能残高）

2. **抽象インターフェース（Abstractions）**
   - `IExchangeAccountClient` に以下のメソッドを追加:
     - `Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken ct = default);`
   - 既存の `IExchangeClient` を、`IExchangeAccountClient` を継承する形に拡張。

3. **Infrastructure（Protocol + Transport）**
   - Private GET を実現するための共通基盤を整備する。
     - `IExchangeClock` / `SystemClock`
       - 現在時刻（UTC）を提供するインターフェースと実装。
     - `IRequestSigner`
       - HTTP リクエストに対して署名・認証ヘッダを付与するインターフェース。
     - `IRestClient`
       - サイン済み HTTP 呼び出しを行う共通 REST クライアント抽象。
       - 最低限、以下のシグネチャを提供する:
         - `Task<T> GetAsync<T>(string path, object? query = null, CancellationToken ct = default);`
     - `RestClient` 実装
       - `HttpClient` を用いた JSON ベースの GET 呼び出し実装。
       - `IRequestSigner` により署名付きリクエストを生成する。
       - HTTP 4xx/5xx の場合は `ExchangeApiException` を送出する（エラー処理レベルは E1 とする）。
     - `ExchangeApiException`
       - REST 呼び出しにおけるエラー情報を保持する共通例外クラス。

4. **bitFlyer Private API 層（現: Exchange.Bitflyer 内 Raw/Http 層）**
   - `/v1/me/getbalance` に対応する DTO と REST 呼び出しを実装する。
     - `BitflyerBalanceResponse`
       - `CurrencyCode`（string）
       - `Amount`（decimal）
       - `Available`（decimal）
     - `IBitflyerPrivateApi`
       - `Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken ct = default);`
     - `BitflyerPrivateApi : IBitflyerPrivateApi`
       - 内部で `IRestClient.GetAsync<IReadOnlyList<BitflyerBalanceResponse>>("/v1/me/getbalance", ...)` を呼び出す。

5. **Bitflyer Adapter 層（現: Exchange.Bitflyer の Facade/Factory）**
   - DTO → ドメイン変換と `IExchangeAccountClient` 実装を担う。
     - `BitflyerExchangeClient : IExchangeClient`
       - コンストラクタで `IBitflyerPublicApi` と `IBitflyerPrivateApi` を受け取る。
       - `GetBalancesAsync` の実装:
         1. `_privateApi.GetBalancesAsync(ct)` を呼び出す。
         2. 取得した `BitflyerBalanceResponse` をそのまま `Balance` に変換して返す（専用 Mapper は不要）。
       - その他のメソッド（発注・キャンセル等）は Stage2 時点では `NotImplementedException` または TODO として残してよい。

6. **組み立て（Factory）**
   - `BitflyerClientFactory` に、Balance を取得可能な最小構成を用意する。
     - `IExchangeClient BitflyerClientFactory.Create(string apiKey, string apiSecret)`
       - `HttpClient` → `HttpTransport` → `BitflyerSigningTransport` → `RestClient` → `BitflyerPublicApi/BitflyerPrivateApi` → `BitflyerExchangeClient` の順に組み立てる。

### 2.2 Stage2 で「やらないこと」（次ステージ以降に回すもの）

- Private GET のうち、balance 以外の API:
  - 証拠金: `/v1/me/getcollateral`
  - ポジション: `/v1/me/getpositions`
  - 注文一覧: `/v1/me/getchildorders`
  - 約定履歴: `/v1/me/getexecutions`
- POST Private API（発注・キャンセル）:
  - `/v1/me/sendchildorder`
  - `/v1/me/cancelchildorder`
  - `/v1/me/cancelallchildorders`
- FundingRate / 手数料 / 各種履歴（balancehistory, collateralhistory 等）の取得
- エラー処理レベルの高度化（E2 以降: 取引所固有エラーの解釈、リトライ制御など）
- CLI / GUI 等のユーザインターフェース整備

---

## 3. レイヤ構成と責務（Stage2 範囲）

Stage2 で対象とするレイヤと責務を以下に整理する。

```text
ExchangeApi.Contracts
  ├─ Domain: Balance
  └─ Interfaces: IExchangeAccountClient.GetBalancesAsync, IExchangeClient

ExchangeApi.Transport (旧)/Common.Core (現)
  ├─ IExchangeClock / SystemClock
  ├─ IRequestSigner
  ├─ IRestClient / RestClient (GET + JSON + 署名)
  └─ ExchangeApiException

Exchange.Bitflyer（旧 Exchange.Bitflyer）
  ├─ DTO: BitflyerBalanceResponse
  ├─ Private API: IBitflyerPrivateApi.GetBalancesAsync
  ├─ Private API 実装: BitflyerPrivateApi
  └─ Adapter: BitflyerExchangeClient.GetBalancesAsync（bitFlyerレスポンスを直接 Balance に変換）
```

Stage2 の時点では、`BitflyerExchangeClient` の他メソッド（板情報・注文系）は Stage1 の範囲または未実装であってよく、
`GetBalancesAsync` が end-to-end で動作することを優先する。

---

## 4. Stage2 の完了条件（Definition of Done）

Stage2 は、以下の条件をすべて満たした時点で完了とみなす。

1. **Abstractions の観点**
   - `Balance` ドメインモデルが定義されている。
   - `IExchangeAccountClient` に `GetBalancesAsync` が定義されている。
   - `IExchangeClient` が `IExchangeAccountClient` を継承している。

2. **Infrastructure の観点**
   - `IExchangeClock` / `SystemClock` が実装されている。
   - `IRequestSigner` が定義され、bitFlyer 用の署名ロジックを持つ実装（`BitflyerRequestSigner` 等）が存在する。
   - `IRestClient` インターフェースと `RestClient` 実装が存在し、
     - `/v1/me/getbalance` に対して GET リクエストを行える。
     - 成功時にレスポンス JSON を `IReadOnlyList<BitflyerBalanceResponse>` にデシリアライズできる。
     - HTTP 4xx/5xx の場合に `ExchangeApiException` を送出する。

3. **Bitflyer Adapter の観点**
   - `BitflyerBalanceResponse` DTO が定義されている。
   - `IBitflyerPrivateApi.GetBalancesAsync` が定義され、`BitflyerPrivateApi` により実装されている。
   - `BitflyerExchangeClient.GetBalancesAsync` が `IExchangeAccountClient` の実装として存在し、
     - 内部で `IBitflyerPrivateApi.GetBalancesAsync` を呼び出す。
     - 取得した DTO 一覧を `Balance` 一覧に変換して返す。

4. **組み立て・動作確認の観点**
   - `BitflyerClientFactory.Create(apiKey, apiSecret)` で `IExchangeClient` を生成できる。
   - API キーとシークレットを設定した環境で、
     - `await client.GetBalancesAsync()` を呼び出すと、
     - 実際の bitFlyer アカウントの残高情報（JPY, BTC 等）が `Balance` のリストとして取得できることを、
       手動または簡易テストコードにより確認している。

5. **ドキュメントの観点**
   - 本ドキュメント（A010-STG2-OVER）が Stage2 ゴールとしてリポジトリに配置されている。
   - `getbalance` に関する最低限の設計メモまたは API マッピング表（Private API → Domain）が整理されている。

---

## 5. Stage3 以降への接続

Stage2 で `getbalance` を end-to-end で通すことで、以下が確立される:

- Private API 呼び出しに必要なインフラ（署名付き `IRestClient`）のパターン
- DTO → ドメイン変換のパターン
- `BitflyerExchangeClient` における Private API 実装の基本スタイル

これらをテンプレートとして、Stage3 以降では次のように拡張していく:

- Stage3: Private GET の拡張（`getcollateral`, `getpositions`, `getexecutions` 等）
- Stage4: Private POST の導入（`sendchildorder`, `cancelchildorder` 等）とエラー処理の強化

Stage2 はあくまでその最初の一歩として、
**「bitFlyer の口座残高を、安全かつ一貫した抽象インターフェースを通じて取得できる状態」**を確立することに専念する。


---

## 6. 制約と今後見直す可能性のある点

Stage2（get balance）では、スコープを意図的に絞っているため、以下の点については今後のステージで見直す余地がある。

- **エラー処理レベルは E1 に限定していること**  
  HTTP ステータスコードを元にした大まかな分類（認証エラー / レートリミット / その他）に留めており、
  bitFlyer 独自のエラーコードやメッセージを解釈してドメイン例外にマップする設計（E2 以降）は Stage3 以降で再検討する。

- **Private GET の API パターンが `/v1/me/getbalance` に依存していること**  
  クエリレス・シンプルな配列レスポンスのみを対象としているため、
  ページングやフィルタ条件を持つ API（`getexecutions`, `getchildorders`, `getbalancehistory` など）を実装する際には、
  `IRestClient` のインターフェースやクエリ表現方法を微調整する可能性がある。

- **ドメインモデルの情報粒度が最小限であること**  
  `Balance` は現時点で通貨コード・総残高・発注可能残高のみを持つが、
  将来、証拠金口座との関連や、より詳細な区分（ロック中残高など）が必要になった場合には、
  既存プロパティを維持しつつフィールド追加や構造拡張を行う可能性がある。

- **POST / Private API の設計は確定していないこと**  
  本ステージでは GET / Private に特化しているため、
  発注・キャンセル等のコマンド系 API に対するエラー設計・ドメインモデル（OrderId の扱い、ドライラン機構など）は、
  Stage3 以降の議論・実装結果を踏まえて `IExchangeTradingClient` 側のインターフェースも含めて見直す余地がある。

これらの制約は、Stage2 を「Private API 呼び出しパターンのテンプレート確立ステージ」として位置付けるための意図的なものであり、
今後のステージでの知見を踏まえて、必要に応じて段階的に解消・改訂していく。
