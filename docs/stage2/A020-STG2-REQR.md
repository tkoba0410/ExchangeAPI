# A020-STG2-REQR Stage2 要件定義（get balance）

## 1. 本文書の目的
Stage2 では、bitFlyer Private API を用いた最初の読み取り処理として、
**`/v1/me/getbalance` に対応する抽象インターフェースと実装を確立する**。
本ドキュメントは、そのために必要な要件を簡潔に整理することを目的とする。

---

## 2. 対象範囲
本ステージで扱うのは以下の機能に限定する。
- bitFlyer Private GET API：`/v1/me/getbalance`
- `Balance` ドメインモデルの定義
- 抽象インターフェース `IExchangeAccountClient.GetBalancesAsync()` の定義
- 署名付き GET リクエストを実行するための Infrastructure（`IRestClient`, `IRequestSigner` など）
- DTO → ドメイン変換
- `BitflyerExchangeClient` における `GetBalancesAsync` の実装

以下は Stage2 の対象外とする。
- 証拠金（collateral）取得
- ポジション取得
- 注文・約定履歴取得
- 発注・キャンセル（POST Private API）
- エラー処理レベル E2 以降（独自エラー解釈・リトライ）

---

## 3. 要件一覧（機能要件）

### REQ-201: Balance ドメイン定義
- `Balance` は以下の情報を保持する。
  - 通貨コード（`Currency`）
  - 総残高（`Amount`）
  - 発注可能残高（`Available`）
- プロパティはイミュータブル構造（record）とする。

### REQ-202: 抽象インターフェースの追加
- `IExchangeAccountClient` に以下のメソッドを追加する。
  - `Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken ct = default)`
- 既存の `IExchangeClient` は `IExchangeAccountClient` を継承する形で拡張する。

### REQ-203: Private GET を実行するための基盤（Infrastructure）
- `IRestClient` に以下のメソッドを要求する。
  - `Task<T> GetAsync<T>(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken ct = default)`
  - クエリはキー/値の辞書で渡す。値が null のものは除外される。
  - 同一キーに異なる値が指定された場合は例外（ArgumentException）とする。
  呼び出し例（クエリ不要の場合は null または空辞書でよい）:
  ```csharp
  var balances = await rest.GetAsync<IReadOnlyList<BitflyerBalanceResponse>>(
      "/v1/me/getbalance",
      new Dictionary<string, string?>());
  ```
- `IRequestSigner` は bitFlyer 認証仕様に従い、以下のヘッダを付与できること。
  - `ACCESS-KEY`
  - `ACCESS-TIMESTAMP`
  - `ACCESS-SIGN`
  - `Content-Type: application/json`
- HTTP 4xx/5xx の場合は `ExchangeApiException` を発生させる（エラー処理レベル E1）。`HttpRequestException` は StatusCode/URI を含めてラップする。

### REQ-204: Private API 実装（bitFlyer）
- `/v1/me/getbalance` を呼び出すメソッドを定義する。
  - `Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken ct = default)`
- `BitflyerBalanceResponse` は以下のフィールドを持つ。
  - `CurrencyCode`（string）
  - `Amount`（decimal）
  - `Available`（decimal）
- JSON デシリアライズにより bitFlyer のレスポンスを正しく扱えること。

### REQ-205: DTO → ドメイン変換
- `BitflyerBalanceResponse` を `Balance` に変換するロジックを実装する（`BitflyerExchangeClient` 内で可）。
  - 型変換は例外を発生させず、bitFlyer のフィールドを忠実にマッピングする。

### REQ-206: Adapter（IExchangeClient）実装
- `BitflyerExchangeClient.GetBalancesAsync` は以下を満たすこと。
  1. `_privateApi.GetBalancesAsync(ct)` を呼び出す。
  2. 各 `BitflyerBalanceResponse` を `Balance` に変換する。
  3. `IReadOnlyList<Balance>` として返す。
- 他の Private API に対応するメソッドは未実装（`NotImplementedException`）でよい。

### REQ-207: 組み立て（Factory）
- `BitflyerClientFactory.Create(apiKey, apiSecret)` は以下を構築できること。
  - `HttpClient`
  - `SystemClock`
  - `BitflyerRequestSigner`
  - `RestClient`（BitflyerSigningTransport経由）
  - `BitflyerPublicApi`
  - `BitflyerPrivateApi`
  - `BitflyerExchangeClient`
- 戻り値は `IExchangeClient` とする。
- API key / secret は呼び出し側で取得し、Factory では null / 空チェックのみ行う。
- `Create(IApiCredentialProvider provider, string exchangeId, string accountId)` オーバーロードを提供し、`provider` が返すキーで `IExchangeClient` を生成できること。`provider == null` の場合は `ArgumentNullException`。


### REQ-208: 手動検証
- API キーおよびシークレットを設定した環境で、`client.GetBalancesAsync()` を実行し、
  - JPY 残高
  - BTC 残高
  などがドメインモデルとして取得できることを確認する。

### REQ-209: API 資格情報プロバイダ
- `IApiCredentialProvider` インターフェースを追加し、`ApiCredentials Get(string exchangeId, string accountId)` で `(ApiKey, ApiSecret)` を提供できること。
- `ApiCredentials` は `ApiKey` / `ApiSecret` を保持するイミュータブル DTO であること。
- クライアント組み立て時にプロバイダから資格情報を取得し、そのまま `BitflyerClientFactory.Create(apiKey, apiSecret)` に渡す構成をデフォルトとする。
- RestClient / Signer / Private API クラスは資格情報の取得責務を持たず、「渡された鍵を使うだけ」に徹する。
- プロバイダの実装（環境変数・資格情報マネージャーなど）は呼び出し側で差し替え可能とし、ライブラリにデフォルト実装を内蔵しない。

---

## 4. 非機能要件

### NFR-201: 安全性
- Private API の扱いであるため、API キー・シークレットは環境変数・資格情報マネージャー・シークレットストアなどで管理し、Git 管理下のファイルに残さない。
- キーはログや例外メッセージに出力しないこと。必要に応じてマスクした形でのみ扱う。
- 平文をディスクや UI に表示せず、オンメモリで最小限に扱うこと。
- 多取引所 / 多アカウント運用を想定し、資格情報は `<EXCHANGE>_<ACCOUNT>_API_KEY` など一貫した命名規則で管理できること。

### NFR-202: 運用切り替え容易性
- 認証情報の取得手段を `IApiCredentialProvider` の差し替えだけで切り替えられるようにする（環境変数 / Windows 資格情報マネージャー / CI シークレット等）。
- プロバイダは複数実装を組み合わせられるようにし、フォールバック（Composite）構成で移行やローテーションを容易にする。

### NFR-203: 保守性
- Private API 層とドメイン変換を分離し、責務を明確にすること。
- `GetBalancesAsync` は例外に依存しない純粋なデータ変換を行う。

### NFR-204: 再利用性
- `IRestClient`, `IRequestSigner`, `SystemClock` は bitFlyer に依存しない形で定義し、
  将来的に他取引所アダプタからも利用できるようにする。

---

## 5. 本ステージの留意点（制約）
- 本ステージでは、Private GET のうちもっとも単純な `/v1/me/getbalance` のみを対象とするため、
  ページング・期間フィルタ等を持つ API（childorders, executions など）のパターンは確定しない。
- POST Private API（注文・キャンセル）については Stage3 以降に設計を行う。
- `Balance` の情報粒度は最小限であり、将来的にフィールド追加の可能性がある。

---

## 6. 完了条件（Definition of Done）
- `GetBalancesAsync` が抽象層 → Private API → HTTP 呼び出し → 実データ取得まで一貫して動作する。
- DTO → ドメイン変換の正確性が確認されている。
- API キーを設定した環境で、実口座残高を取得できることを手動で検証している。
- Stage2 OVER（A010）と本書（A020）が整合し、リポジトリに反映されている。
