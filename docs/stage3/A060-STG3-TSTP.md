# A060-STG3-TSTP Stage3 テスト観点（send child order）

## 1. 本文書の目的
Stage3 では、bitFlyer Private POST API **`/v1/me/sendchildorder`** を用いて、
**MARKET 注文を最小スコープで end-to-end で発注できる縦スライス**を構築する。

本ドキュメントは、その縦スライスを確実に動作させるためのテスト観点を整理し、
Stage4 以降（LIMIT / STOP / cancel / GET 拡張）のテストにも再利用できるテンプレートとして設計する。

---

## 2. テスト対象範囲
### 対象となるレイヤ
- **Infrastructure**
  - `IRestClient.PostAsync`（署名・JSON POST・HTTP エラー処理）
  - `IRequestSigner`（POST 署名）
  - `ExchangeApiException`（E1 例外処理）

- **Bitflyer.Private**
  - `IBitflyerPrivateTradingApi.SendChildOrderAsync`
  - DTO：`BitflyerSendChildOrderRequest` / `BitflyerSendChildOrderResponse`

- **Bitflyer.Adapter**
  - `BitflyerExchangeClient.SendOrderAsync`
  - Domain ⇄ DTO マッピング

- **Factory の最小動作確認**
  - `BitflyerClientFactory.Create(apiKey, apiSecret)` による trading 機能構築

### 対象外（Stage4 以降）
- LIMIT / STOP / TIME_IN_FORCE / 複合注文
- 取消系（cancelchildorder / cancelallchildorders）
- Private GET 拡張（positions / executions / collateral）
- エラー処理 E2（取引所固有コード分類・リトライ）

---

## 3. テスト観点一覧
Stage3 の主要観点をレイヤ別に整理する。

### 3.1 Infrastructure（POST / 署名 / 例外処理）
1. **正しい署名が付与されること（POST 用）**
   - `timestamp + "POST" + "/v1/me/sendchildorder" + bodyJson` が SHA256-HMAC 署名されている
   - `ACCESS-KEY` が API key と一致する
   - `ACCESS-TIMESTAMP` が clock の値と一致する

2. **正しい JSON body が送信されること**
   - `ProductCode` → `product_code`
   - `Side` → `"BUY"` / `"SELL"`
   - `ChildOrderType = "MARKET"`
   - `Size` → decimal 値のまま

3. **正しい URL・メソッドで呼び出すこと**
   - `POST /v1/me/sendchildorder`
   - クエリが付与されない

4. **HTTP ステータス異常時の例外処理（E1）**
   - 400 → `ExchangeApiException`
   - 403（認証エラー）→ `ExchangeApiException`
   - 404 → `ExchangeApiException`
   - 500 → `ExchangeApiException`
   - StatusCode が例外オブジェクトに保持されている

5. **通信エラーの扱い**
   - タイムアウト / DNS エラー → `ExchangeApiException` に包む

---

### 3.2 Bitflyer Private API（Trading）
1. **`/v1/me/sendchildorder` を正しく呼び出すこと**
2. **POST body に DTO が正しく反映されること**
3. **REST の戻り値が DTO（BitflyerSendChildOrderResponse）として返ること**
4. **例外を握りつぶさず、そのまま伝播すること**

---

### 3.3 DTO → Domain / Domain → DTO マッピング
1. **ProductCode のマッピングが正しい**
2. **OrderSide → "BUY" / "SELL" が正しい**
3. **OrderType.Market → "MARKET" が正しい**
4. **decimal の Size がそのままマップされること**
5. **child_order_acceptance_id → OrderId がそのまま変換されること**

---

### 3.4 Adapter（BitflyerExchangeClient）
1. **`SendOrderAsync` が PrivateTradingApi を 1 回だけ呼ぶこと**
2. **DTO → Domain 変換を正しく行うこと**
3. **例外が来たらそのまま呼び出し元に返すこと（例外変換なし）**
4. **Domain → DTO の変換も正確であること**

---

### 3.5 Factory（組み立て）
1. **`Create(apiKey, apiSecret)` が trading 機能を含んだ IExchangeClient を返すこと**
2. **内部構造が正しく組み立てられていること**
   - `BitflyerRequestSigner`（POST 対応）
   - `RestClient`（POST 対応）
   - `BitflyerPrivateApi`
   - `BitflyerExchangeClient`
3. **API key / secret が null や空の場合は ArgumentException を投げる**
4. **CredentialProvider を使った Create も問題なく動作すること**

---

## 4. 擬似テストケース（サンプル）
実装チームが Stage3 を通す際の参考として、代表的テストケースを提示する。

### 4.1 正常系（MARKET 注文）
#### TC-301: BUY MARKET 注文が成功する
- 前提：BTC_JPY で 0.01 BTC の発注が可能な環境
- 手順：
  ```csharp
  var req = new OrderRequest("BTC_JPY", OrderSide.Buy, OrderType.Market, 0.01m);
  var res = await client.SendOrderAsync(req);
  ```
- 期待：
  - `res.OrderId` が非空の文字列
  - bitFlyer 管理画面に注文が反映されている

#### TC-302: SELL MARKET 注文が成功する
- BUY と同様に SELL での動作を確認

---

### 4.2 エラー系（API / 署名 / 通信）

#### TC-303: API Key 不正（403）
- 期待：`ExchangeApiException`（StatusCode = Forbidden）

#### TC-304: 残高不足（400）
- 大きすぎる Size を指定
- 期待：`ExchangeApiException`（StatusCode = BadRequest）
- ※ Stage3 では bitFlyer 固有エラーコードの判断までは行わない

#### TC-305: タイムアウト
- HttpClient に極端に短い Timeout を設定
- 期待：`ExchangeApiException` にラップされる

#### TC-306: 不正 JSON（サーバ側エラー）
- bitFlyer が `{}` を返すなどの異常ケース
- 期待：デシリアライズ例外 → `ExchangeApiException` へ統一

---

### 4.3 Adapter の単体テスト
#### TC-307: DTO → Domain 変換
- PrivateTradingApi をモックし、`ChildOrderAcceptanceId = "XYZ"` を返す
- 期待：`OrderResult.OrderId == "XYZ"`

#### TC-308: Domain → DTO 変換
- OrderRequest(Buy, Market, 0.01) を渡し DTO 値を検証
- 期待：Side="BUY", child_order_type="MARKET", size=0.01

---

### 4.4 Factory の統合テスト（簡易）
#### TC-309: Create → SendOrderAsync（モック）
- RestClient/Signer をモックして IExchangeClient を構築
- `SendOrderAsync` が正常に流れるか確認（HTTP 通信なし）

#### TC-310: Create → SendOrderAsync（実通信、任意）
- 少額注文で end-to-end 確認
- 期待：bitFlyer に注文が生成され OrderId が戻る

---

## 5. Stage3 のテスト完了条件
- POST（署名＋JSON＋HTTP）の動作が RestClient レベルで確認済み
- `IBitflyerPrivateTradingApi.SendChildOrderAsync` が正しく実装されている
- Adapter（`SendOrderAsync`）が Domain ⇄ DTO 変換を誤りなく行う
- 実口座で MARKET 注文が発注でき、OrderId が取得できる
- E1 レベルの例外が適切に発生する（403, 400, 500 など）
- Factory が Trading 対応の IExchangeClient を正常に構築できている

---

## 6. 備考（Stage4 以降への伏線）
- LIMIT 注文を導入する際は、`price` の追加、OrderType の拡張、DTO の拡張が必要。
- cancel 系（cancelchildorder）は POST + query の混合形式のため、RestClient の柔軟性確認が必要。
- E2（bitFlyer 独自エラー分類）に備え、DTO 形式の理解は進めておくと良い。

Stage3 テスト観点は、今後の Trading API 全体の自動テスト設計にそのまま流用できるように構成している。

