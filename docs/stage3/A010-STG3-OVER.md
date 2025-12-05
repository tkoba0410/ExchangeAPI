# A010-STG3-OVER Stage3 ゴール定義（send child order 縦スライス）

## 1. Stage3 の目的
Stage3 では、Stage2 で確立した Private GET（`/v1/me/getbalance`）の縦スライスを前提に、**bitFlyer Private POST の最初の一歩として `/v1/me/sendchildorder` を抽象層まで通すこと** を目的とする。

具体的には、以下を満たす SDK 状態を Stage3 の完了とする：
- 抽象インターフェース `IExchangeTradingClient.SendOrderAsync()` を経由して、
- bitFlyer の `/v1/me/sendchildorder` が呼び出され、
- `OrderRequest` ドメインモデルから MARKET 注文を最小スコープで送信でき、
- `OrderResult` として `child_order_acceptance_id` を取得できること。

Stage3 は **「Private POST 呼び出しパターンのテンプレート確立ステージ」** と位置付ける。

---

## 2. スコープ（Stage3 でやること / やらないこと）

### 2.1 Stage3 でやること
1. **ドメインモデル（Trading）**
   - `OrderSide`（Buy / Sell）
   - `OrderType`（MARKET のみ）
   - `OrderRequest`（ProductCode / Side / Size）
   - `OrderResult`（OrderId）

2. **抽象インターフェース**
   - `IExchangeTradingClient.SendOrderAsync(OrderRequest req, CancellationToken ct = default)`
   - `IExchangeClient` に Trading 機能を統合

3. **Infrastructure（POST 対応）**
   - `IRestClient.PostAsync<TRequest, TResponse>()` を追加
   - `IRequestSigner` を POST 用に拡張（body 署名）
   - `RestClient` の POST 実装

4. **Bitflyer Private API 層**
   - DTO: `BitflyerSendChildOrderRequest` / `BitflyerSendChildOrderResponse`
   - `IBitflyerPrivateApi.SendChildOrderAsync`
   - `BitflyerPrivateApi` に POST 実装

5. **Bitflyer Adapter 層**
   - Domain ⇄ DTO 変換
   - `BitflyerExchangeClient.SendOrderAsync` の実装

6. **Factory**
   - Stage2 と同様の構築フローに POST 対応を追加

7. **ドキュメント（A010〜A070 Stage3 版）作成**

### 2.2 Stage3 でやらないこと（一部抜粋）
- LIMIT / STOP / IFDOCO などの複合注文
- キャンセル（`cancelchildorder` / `cancelallchildorders`）
- Private GET の横展開（collateral / positions / executions）
- エラー処理 E2（詳細分類・リトライ）

---

## 3. レイヤ構成（Stage3 範囲）
```
ExchangeApi.Contracts
  ├─ Domain: OrderRequest / OrderResult
  └─ Interfaces: IExchangeTradingClient

ExchangeApi.Transport
  ├─ IRequestSigner（POST 対応）
  ├─ IRestClient.PostAsync
  └─ ExchangeApiException

ExchangeApi.Adapter.Bitflyer (Private API)
  ├─ DTO: SendChildOrder Request/Response
  └─ BitflyerPrivateApi.SendChildOrderAsync

ExchangeApi.Adapter.Bitflyer (Adapter)
  └─ BitflyerExchangeClient.SendOrderAsync
```

---

## 4. Stage3 完了条件（Definition of Done）
1. 抽象層に Trading インターフェースが追加されている
2. `IRestClient` に POST が実装されている
3. `BitflyerPrivateApi.SendChildOrderAsync` が正しく動作する
4. Adapter が Domain ⇄ DTO を正しく変換できる
5. Factory を通じて MARKET 注文が end-to-end で通る
6. `child_order_acceptance_id` が取得できることを実口座で確認済み

---

## 5. Stage4 以降への接続
Stage3 により以下が確立される：
- Private POST 呼び出し基盤
- Trading の最小単位（MARKET 注文）
- POST の署名・例外処理のテンプレ

これを基に、Stage4 では次を扱う：
- Private GET の横展開（collateral / positions / executions）
- LIMIT / STOP / キャンセル系 API
- エラー処理の高度化（E2）

Stage3 は **「MARKET 注文を 1 本通す縦スライス」** の確立に専念する。

