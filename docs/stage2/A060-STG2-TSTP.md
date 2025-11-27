# A060-STG2-TSTP Stage2 テスト観点（get balance）

## 1. 本文書の目的
Stage2 は bitFlyer Private API の最初の実装として、
**`/v1/me/getbalance` を抽象層（IExchangeClient）経由で取得可能にする**ことを目的とする。

本ドキュメントは、その機能を保証するためのテスト観点・テストケースを整理し、
Stage3 以降（collateral / positions / executions / orders）に適用できるテンプレートとして構築する。

---

## 2. テスト対象範囲
**対象となるレイヤ：**
- Infrastructure
  - `RestClient`（GET / 署名 / JSON）
  - `IRequestSigner`（署名）
  - `ExchangeApiException`（HTTPエラー処理）
- Bitflyer.Raw
  - `IBitflyerRawApiClient.GetBalanceAsync`
  - `BalanceResponse` DTO
- Bitflyer.Adapter
  - `BitflyerExchangeClient.GetBalancesAsync`
  - `BitflyerDtoMapper`
- Factory（最小限の動作確認）

**対象外：**（Stage3以降）
- collateral / positions / executions / orders
- POST Private API
- E2 以降のエラー処理（詳細エラーの分類）

---

## 3. テスト観点一覧
Stage2 で検証すべき観点を、階層別に整理する。

### 3.1 Infrastructure（RestClient / RequestSigner）
1. **正しい署名ヘッダが付与されること**
   - `ACCESS-KEY` が API key と一致
   - `ACCESS-TIMESTAMP` が clock の時刻と一致
   - `ACCESS-SIGN` が正しい HMAC-SHA256 で生成されている

2. **正しい URL が生成されること**
   - BaseAddress + `/v1/me/getbalance`
   - クエリパラメータが null の場合はクエリなし

3. **正常系の JSON デシリアライズが成功すること**
   - `BalanceResponse[]` に正しくマップされる

4. **HTTP ステータスコードによる例外発生**（E1 レベル）
   - 400 / 401 / 403 / 404 / 500 などで `ExchangeApiException` が発生
   - StatusCode が保持されること

5. **タイムアウト / 通信エラーへの例外**
   - HttpClient が `TaskCanceledException` を投げる場合 → `ExchangeApiException` にラップされる

---

### 3.2 Bitflyer.Raw（GetBalanceAsync）
1. **正しいパス `/v1/me/getbalance` を呼び出すこと**
2. **RestClient の戻り値を DTO リストとして返すこと**
3. **例外を Raw 層で握りつぶさずそのまま伝播すること**
4. **空配列が返ってきた場合も正しく空のリストを返すこと**

---

### 3.3 DTO → Domain 変換（Mapper）
1. **`currency_code → Currency` のマッピングが正しいこと**
2. **数値フィールド（amount, available）がそのまま保たれること**
3. **Null や欠損がない前提で変換を行う（Stage2 の仕様）**
4. **複数件（JPY, BTC）がある場合の順序は Raw → Domain で維持されること**

---

### 3.4 Adapter（BitflyerExchangeClient）
1. **Raw API 呼び出しが1回行われること**
2. **DTO リストを Domain リストに変換して返すこと**
3. **例外が発生した場合は Raw API の例外をそのまま返すこと**
4. **空リストの場合でも空の `IReadOnlyList<Balance>` が返ること**

---

### 3.5 Factory（組み立て）
1. **Create(apiKey, apiSecret) がエラーなく IExchangeClient を返すこと**
2. **内部で RestClient / RawApiClient / ExchangeClient が正しく組み立てられていること**
3. **API key / secret が null・空文字の場合 ArgumentException を投げること**
4. **Create(IApiCredentialProvider provider, exchangeId, accountId) で provider が返したキーを使って IExchangeClient を生成できること**
5. **provider == null の場合 ArgumentNullException を投げること**
---

## 4. 擬似テストケース（サンプル）
実装チームが Stage2 を通す際に参考とするため、代表的なテストケースを提示する。

### 4.1 正常系
#### TC-201: 通常の残高取得
- 前提：bitFlyer 口座に JPY・BTC 残高がある
- 手順：`client.GetBalancesAsync()` を実行
- 期待結果：
  - `Balance` のリストが2件以上返る
  - JPY の `Amount` と `Available` が 0 以上
  - BTC の `Amount` と `Available` が 0 以上

#### TC-202: 空残高の場合
- Raw が `[]` を返す
- 期待結果：空の `IReadOnlyList<Balance>` が返る

---

### 4.2 エラー系（署名・通信）
#### TC-203: API Key 不正（403）
- Raw API が 403 を返す
- 期待：`ExchangeApiException` / Status = 403

#### TC-204: タイムアウト
- HttpClient が `TaskCanceledException` を投げる
- 期待：`ExchangeApiException` にラップされて返される

#### TC-205: レスポンス JSON が不正
- Raw が `{}` を返す
- 期待：デシリアライズ時に例外が発生 → `ExchangeApiException`（RestClient が統一処理）

---

## 5. Stage2 のテスト完了条件
以下を満たした場合、Stage2 のテストは完了とみなす。

- RestClient が署名付き GET を正しく実行できる
- `/v1/me/getbalance` が Raw → Domain のパイプラインを問題なく通過する
- 正常系で実口座の JPY/BTC が正しく取得できる
- エラー系で `ExchangeApiException` が適切に発生する
- Factory が IExchangeClient を正常に構築する

---

## 6. 実行とインテグレーションテストの扱い
- ユニットテスト（通信なし）は dotnet test tests/ExchangeApi.Bitflyer.Tests/ExchangeApi.Bitflyer.Tests.csproj などで実行する。
- 実通信テストは任意の別プロジェクト/カテゴリに分離し、検証用 API キーを環境変数やシークレットで注入して手動実行とする（デフォルトの dotnet test には含めない）。キー未設定時はスキップする条件分岐を推奨。
- ログや例外に秘密を出さないこと。レートリミットや署名エラーは ExchangeApiException で扱う。

## 7. 次の展開（Stage3 以降）
Stage2 のテスト項目は、次の API にほぼそのまま流用できる：
- `/v1/me/getcollateral`
- `/v1/me/getpositions`
- `/v1/me/getexecutions`

特に：
- "Raw → Domain マッピング" のテストテンプレ
- "RestClient 署名 + JSON" の統合テストパターン
- "例外（E1）処理" のテスト構造

これらは今後の Private API 実装における基礎となる。

Stage2 は「最初の Private GET を確実に通す」ことを目的とするため、本書のテスト観点が Stage3 以降の基礎テンプレートとなる。


---
---

## 8. 追補（A065 より）
- Factory のプロバイダーオーバーロード: Create(IApiCredentialProvider provider, exchangeId, accountId) が provider から取得したキーを使えること、provider == null で ArgumentNullException を投げること。
- 実行方針: 通信なしのユニットテストは通常の dotnet test で実行。実通信テストは別プロジェクト/カテゴリに分離し、検証用キーを環境変数やシークレットで注入して手動実行する（デフォルトの dotnet test には含めない、未設定ならスキップ）。
- 秘密はログ/例外に出さない。レートリミットや署名エラーは ExchangeApiException で扱う。
