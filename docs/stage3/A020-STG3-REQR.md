# A020-STG3-REQR Stage3 要件定義（send child order 縦スライス）

## 1. 本文書の目的
Stage3 では、Stage2 で確立した Private GET（`/v1/me/getbalance`）の縦スライスを前提に、
**`/v1/me/sendchildorder` に対応する抽象インターフェースと実装を確立し、最小限の MARKET 注文を end-to-end で発行できること** を目的とする。

本ドキュメントは、そのために必要な機能要件・非機能要件・完了条件を整理する。

---

## 2. 対象範囲

### 2.1 Stage3 で扱う対象
- bitFlyer Private POST API：`/v1/me/sendchildorder`
- Trading ドメインモデルの最小セット：
  - `OrderSide`（Buy / Sell）
  - `OrderType`（MARKET に限定）
  - `OrderRequest`（プロダクトコード / 売買方向 / 数量）
  - `OrderResult`（受け付け ID）
- 抽象インターフェース：`IExchangeTradingClient.SendOrderAsync()`
- 署名付き POST リクエストを実行するための Infrastructure 拡張
  - `IRestClient.PostAsync<TRequest, TResponse>()`
  - `IRequestSigner`（POST 対応）
- bitFlyer Private API 層
  - `BitflyerSendChildOrderRequest` / `BitflyerSendChildOrderResponse` DTO
  - `IBitflyerPrivateTradingApi.SendChildOrderAsync()`（名称は暫定）
- `BitflyerExchangeClient` における `SendOrderAsync` の実装
- Factory から `IExchangeClient` を生成し、残高取得（Stage2）と MARKET 発注（Stage3）が両方使える状態

### 2.2 Stage3 の対象外
- Private GET の拡張：
  - 証拠金：`/v1/me/getcollateral`
  - ポジション：`/v1/me/getpositions`
  - 注文一覧：`/v1/me/getchildorders`
  - 約定履歴：`/v1/me/getexecutions`
- Private POST のうち、以下は除外：
  - `cancelchildorder`
  - `cancelallchildorders`
- 注文種別の拡張：
  - LIMIT / STOP / IFDOCO / IFDO など
  - `time_in_force`（IOC / FOK）の柔軟指定
- エラー処理レベル E2 以降（bitFlyer 固有エラーコードの分類・ドメイン例外化・自動リトライ）
- CLI / GUI などの UI 実装

---

## 3. 機能要件（Functional Requirements）

### REQ-301: Trading ドメインモデル定義
- `OrderSide` を定義する。
  - 値：`Buy`, `Sell`
- `OrderType` を定義する。
  - Stage3 では `Market` のみを対象とする（将来 LIMIT 等を追加可能とする）。
- `OrderRequest` は以下の情報を保持する。
  - `ProductCode`（string）
  - `Side`（OrderSide）
  - `OrderType`（OrderType）
  - `Size`（decimal）
- `OrderResult` は以下の情報を保持する。
  - `OrderId`（string）: `child_order_acceptance_id` 相当
  - 必要に応じて、拡張用のプロパティを将来追加する余地を残す。

### REQ-302: 抽象インターフェース IExchangeTradingClient
- `IExchangeTradingClient` インターフェースを追加する。
  - シグネチャ：
    - `Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken ct = default);`
- `IExchangeClient` は、`IExchangeAccountClient`（Stage2）に加えて、`IExchangeTradingClient` を継承する。
- メソッド名・引数名は、bitFlyer 固有ではなく「他取引所でも同じ意味で使える」ことを前提とする。

### REQ-303: Infrastructure（IRestClient）POST 対応
- `IRestClient` に、JSON POST 用のメソッドを追加する。
  - シグネチャ例：
    - `Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct = default);`
- 要件：
  - `HttpClient` を用いて `path` に対して POST を行う。
  - `body` は JSON にシリアライズし、`application/json` で送信する。
  - 送信前に `IRequestSigner` を用いて署名付きリクエストに変換する。
  - HTTP ステータスコードが 2xx 以外の場合は `ExchangeApiException` を送出する（E1 レベル）。

### REQ-304: IRequestSigner の POST 対応
- `IRequestSigner` は、以下を満たすこと。
  - GET / POST の両方に対応した署名を生成できる。
  - POST の場合、**body（JSON 文字列）を署名対象に含める**。
- 署名仕様（概要）：
  - `ACCESS-TIMESTAMP`（clock から取得）、`ACCESS-KEY`（API key）をヘッダに付与。
  - `timestamp + method + path + body` を連結した文字列を HMAC-SHA256 で署名し、`ACCESS-SIGN` としてヘッダに付与する。
  - GET の場合、`body` は空文字列として扱う。

### REQ-305: Bitflyer Private API（Trading）
- `/v1/me/sendchildorder` を呼び出すメソッドを定義する。
  - インターフェース例：
    - `Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(BitflyerSendChildOrderRequest request, CancellationToken ct = default);`
- `BitflyerSendChildOrderRequest` DTO は以下のフィールドを持つ。
  - `product_code`（string）
  - `child_order_type`（string）: Stage3 では "MARKET" 固定
  - `side`（string）: "BUY" or "SELL"
  - `size`（decimal）
  - `time_in_force` などは Stage3 では省略または固定値でよい（将来の拡張余地）。
- `BitflyerSendChildOrderResponse` DTO は以下のフィールドを持つ。
  - `child_order_acceptance_id`（string）
- JSON デシリアライズ／シリアライズにより bitFlyer のリクエスト／レスポンスを正しく扱えること。

### REQ-306: DTO → Domain 変換（Trading）
- `OrderRequest` と `BitflyerSendChildOrderRequest` の間で、以下のマッピングを実装する。
  - `ProductCode` → `product_code`
  - `Side` → `side`（`Buy` → "BUY", `Sell` → "SELL"）
  - `OrderType`（Market）→ `child_order_type` = "MARKET"
  - `Size` → `size`
- `BitflyerSendChildOrderResponse` → `OrderResult` のマッピング：
  - `child_order_acceptance_id` → `OrderId`
- 型変換は例外を発生させず、bitFlyer のフィールドを忠実にマッピングする（変換に失敗するような複雑なロジックは Stage3 では持たない）。

### REQ-307: Bitflyer Adapter（IExchangeTradingClient 実装）
- `BitflyerExchangeClient` に `SendOrderAsync` を実装する。
  - 処理フロー：
    1. `OrderRequest` を受け取る。
    2. Domain → DTO 変換を行い、`BitflyerSendChildOrderRequest` を生成する。
    3. `IBitflyerPrivateTradingApi.SendChildOrderAsync` を呼び出す。
    4. 戻り値の `BitflyerSendChildOrderResponse` を `OrderResult` に変換する。
    5. `OrderResult` を返す。
- Stage2 で実装した `GetBalancesAsync` はそのまま維持し、アカウント系とトレーディング系の責務を混在させないように注意する。

### REQ-308: Factory（組み立て）
- `BitflyerClientFactory.Create(apiKey, apiSecret)` は、Stage2 と同様に以下を構築できること：
  - `HttpClient`
  - `SystemClock`
  - `BitflyerRequestSigner`（GET/POST 対応）
  - `RestClient`（GET + POST 対応）
  - `BitflyerPublicApi`
  - `BitflyerPrivateApi`（Account + Trading 両方のインターフェースを実装）
  - `BitflyerExchangeClient`（Account + Trading 両インターフェースの実装）
- 戻り値は `IExchangeClient` とし、残高取得と MARKET 注文が両方利用可能な状態で返す。
- 必要に応じて、`IApiCredentialProvider` を利用するオーバーロード（Stage2 と同等の仕様）を提供する。

### REQ-309: 手動検証（Trading）
- API キーおよびシークレットを設定した実口座環境で、`client.SendOrderAsync()` を実行し、以下を確認する。
  - 小額の MARKET 注文が実際に bitFlyer 上で受け付けられていること。
  - 戻り値の `OrderResult.OrderId` が、bitFlyer の `child_order_acceptance_id` と一致していること。

---

## 4. 非機能要件（Non-Functional Requirements）

### NFR-301: 安全性（API キー・発注）
- Stage2 同様、API キー・シークレットは環境変数・資格情報マネージャー・シークレットストアなどで管理し、Git 管理下に置かない。
- 発注系 API であるため、テスト時は必ず **小額・テスト用のアカウント** を使うこと。
- ログや例外メッセージに、API キー・シークレット・完全な注文内容を出力しない（必要に応じてマスク）。

### NFR-302: 運用切り替え容易性
- 認証情報の取得は `IApiCredentialProvider` の差し替えだけで切り替えられるようにする。
- テスト用アカウント、本番アカウントを `exchangeId` / `accountId` で切り替え可能にし、誤発注を避ける運用が取りやすい構造にする。

### NFR-303: 保守性
- Private API 層とドメイン変換（Adapter）を分離し、責務を明確にする。
- `SendOrderAsync` は「発注要求の受け渡しと単純な変換」に徹し、複雑なビジネスロジック（ポジション管理など）は持たない。

### NFR-304: 再利用性
- `IRestClient`, `IRequestSigner`, `OrderRequest`, `OrderResult`, `IExchangeTradingClient` は bitFlyer 固有に依存しない形で定義し、将来的に他取引所アダプタからも利用できるようにする。

---

## 5. 本ステージの留意点（制約）
- Stage3 は **MARKET 注文 1 本の縦スライス** に意図的にスコープを絞る。
- エラー処理は Stage2 と同じく E1 レベル（HTTP ステータスベース）に限定し、bitFlyer 固有エラーコードの解釈は Stage4 以降に回す。
- `OrderRequest` / `OrderResult` は、将来 LIMIT / STOP / IFDOCO などに拡張可能なよう、最小限のフィールド構成に留める。
- Query パラメータと Body パラメータの違いは **Infrastructure 層（RestClient / Signer）で吸収し、BitflyerPrivateApi のインターフェース設計には持ち込まない**。

---

## 6. 完了条件（Definition of Done）
Stage3 は、以下を満たした時点で完了とみなす。

1. `OrderSide` / `OrderType` / `OrderRequest` / `OrderResult` が Abstractions に定義されている。
2. `IExchangeTradingClient.SendOrderAsync` が定義され、`BitflyerExchangeClient` が実装している。
3. `IRestClient` に POST メソッドが追加され、`BitflyerPrivateApi.SendChildOrderAsync` を通じて `/v1/me/sendchildorder` を呼び出せる。
4. `IRequestSigner` が POST 署名（body 含む）に対応している。
5. 実口座環境で、小額の MARKET 注文を `client.SendOrderAsync` から発注し、`child_order_acceptance_id` が `OrderResult.OrderId` に格納されることを確認済みである。
6. Stage3 用ドキュメント（Stage3 A010〜A070）がリポジトリに配置され、Stage2 文書との整合が取れている。

