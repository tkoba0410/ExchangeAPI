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
  - `Task<T> GetAsync<T>(string path, object? query = null, CancellationToken ct = default)`
- `IRequestSigner` は bitFlyer 認証仕様に従い、以下のヘッダを付与できること。
  - `ACCESS-KEY`
  - `ACCESS-TIMESTAMP`
  - `ACCESS-SIGN`
  - `Content-Type: application/json`
- HTTP 4xx/5xx の場合は `ExchangeApiException` を発生させる（エラー処理レベル E1）。

### REQ-204: Raw API 実装（bitFlyer）
- `/v1/me/getbalance` を呼び出すメソッドを定義する。
  - `Task<IReadOnlyList<BalanceResponse>> GetBalanceAsync(CancellationToken ct = default)`
- `BalanceResponse` は以下のフィールドを持つ。
  - `CurrencyCode`（string）
  - `Amount`（decimal）
  - `Available`（decimal）
- JSON デシリアライズにより bitFlyer のレスポンスを正しく扱えること。

### REQ-205: DTO → ドメイン変換
- `BalanceResponse` を `Balance` に変換する Mapper を実装する。
  - `Balance ToBalance(BalanceResponse dto)`
- 型変換は例外を発生させず、bitFlyer のフィールドを忠実にマッピングする。

### REQ-206: Adapter（IExchangeClient）実装
- `BitflyerExchangeClient.GetBalancesAsync` は以下を満たすこと。
  1. `_raw.GetBalanceAsync(ct)` を呼び出す。
  2. 各 `BalanceResponse` を `Balance` に変換する。
  3. `IReadOnlyList<Balance>` として返す。
- 他の Private API に対応するメソッドは未実装（`NotImplementedException`）でよい。

### REQ-207: 組み立て（Factory）
- `BitflyerClientFactory.Create(apiKey, apiSecret)` は以下を構築できること。
  - `HttpClient`
  - `SystemClock`
  - `BitflyerRequestSigner`
  - `RestClient`
  - `BitflyerRawApiClient`
  - `BitflyerExchangeClient`
- 戻り値は `IExchangeClient` とする。

### REQ-208: 手動検証
- API キーおよびシークレットを設定した環境で、`client.GetBalancesAsync()` を実行し、
  - JPY 残高
  - BTC 残高
 などがドメインモデルとして取得できることを確認する。

---

## 4. 非機能要件

### NFR-201: 安全性
- Private API の扱いであるため、API キー・シークレットは環境変数または設定ファイル（.json / .user）を用いて管理する。
- キーはログに出力しないこと。

### NFR-202: 保守性
- Raw API とドメイン変換を分離し、Mapper を介して疎結合を維持すること。
- `GetBalancesAsync` は例外に依存しない純粋なデータ変換を行う。

### NFR-203: 再利用性
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
- `GetBalancesAsync` が抽象層 → Raw API → HTTP 呼び出し → 実データ取得まで一貫して動作する。
- DTO → ドメイン変換の正確性が確認されている。
- API キーを設定した環境で、実口座残高を取得できることを手動で検証している。
- Stage2 OVER（A010）と本書（A020）が整合し、リポジトリに反映されている。
