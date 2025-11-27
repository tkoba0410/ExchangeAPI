# A070-STG3-OPS Stage3 動作確認・運用メモ（send child order）

## 1. 本文書の目的
Stage3 では、bitFlyer Private POST API **`/v1/me/sendchildorder`** を用いて、
**MARKET 注文を end-to-end で発注できる縦スライス**を構築する。

本書は、Stage3 実装後に行う **動作確認手順・注意事項・運用上の留意点** を整理し、
Stage4 以降（cancel / LIMIT / STOP / GET 拡張）においても使い回せるテンプレートとする。

---

## 2. 動作確認の前提
- bitFlyer Lightning API の **API Key / Secret** が取得済みであること
  - 読み取り専用（Read）では不可。**トレード権限**が必要
- HTTP 通信が可能な環境（プロキシ・FW 制約がない）で実行する
- Stage3 の実装がリポジトリに反映され、Factory により trading 対応の `IExchangeClient` が生成できる状態

---

## 3. 動作確認手順（最短）
### Step 1. API キーとシークレットを準備
- **環境変数** または **資格情報マネージャー** に保存する
- Git 管理下に書かないこと

推奨環境変数名：
```
BITFLYER_DEFAULT_API_KEY
BITFLYER_DEFAULT_API_SECRET
```

例（PowerShell）：
```powershell
setx BITFLYER_DEFAULT_API_KEY "your_api_key"
setx BITFLYER_DEFAULT_API_SECRET "your_api_secret"
```

例（bash）：
```bash
export BITFLYER_DEFAULT_API_KEY="your_api_key"
export BITFLYER_DEFAULT_API_SECRET="your_api_secret"
```

---

### Step 2. Factory を使って IExchangeClient を生成
環境変数から手動取得する場合：
```csharp
var apiKey = Environment.GetEnvironmentVariable("BITFLYER_DEFAULT_API_KEY")!;
var apiSecret = Environment.GetEnvironmentVariable("BITFLYER_DEFAULT_API_SECRET")!;
var client = BitflyerClientFactory.Create(apiKey, apiSecret);
```

CredentialProvider を利用する場合：
```csharp
var provider = new CompositeCredentialProvider(new IApiCredentialProvider[]
{
    new EnvironmentVariableApiCredentialProvider(),
});

var client = BitflyerClientFactory.Create(provider, "bitflyer", "default");
```

---

### Step 3. MARKET 注文を送信
例：0.01 BTC の買い注文

```csharp
var order = new OrderRequest(
    ProductCode: "BTC_JPY",
    Side: OrderSide.Buy,
    OrderType: OrderType.Market,
    Size: 0.01m);

var result = await client.SendOrderAsync(order);
Console.WriteLine($"OrderId: {result.OrderId}");
```
- **検証は必ず小額で実施**し、本番とテスト口座を `exchangeId` / `accountId` で切り替えられる運用にする。

---

### Step 4. 正常なレスポンスを確認
**期待出力の例：**
```
OrderId: JRF20231225-123456-012345
```
- `child_order_acceptance_id` が返ってくれば発注成功
- bitFlyer 管理画面でも同じ ID の注文が生成されていることを確認する

---

## 4. 想定されるエラーと対処法

### 4.1 認証エラー（403）
**現象：**
- `ExchangeApiException: StatusCode = Forbidden`

**原因：**
- API Key / Secret が誤っている
- 権限（トレード権限）が不足している

**対処：**
- API キー設定を見直す
- bitFlyer 管理画面でトレード権限が有効であることを確認

---

### 4.2 残高不足（400 BadRequest）
**現象：**
- `ExchangeApiException: StatusCode = BadRequest`

**原因：**
- Size（買い注文額）が保有資金を超過

**対処：**
- テスト用口座に十分な資金を入れる、または少額注文に変更

---

### 4.3 タイムスタンプずれ（400 / 403）
**現象：**
- bitFlyer が署名エラーを返す

**原因：**
- PC 時刻と NTP が大きくずれている

**対処：**
- OS の時刻を NTP と同期
- Docker 内で実行する場合はホスト時刻も確認

---

### 4.4 ネットワークタイムアウト
**現象：**
- `ExchangeApiException` にラップされる

**対処：**
- ネットワーク設定を確認
- 約数秒はタイムアウトに猶予を設定

---

### 4.5 署名エラー時の追加チェック（400 / 403）
Stage3 の POST は署名差異が起きやすい。以下を確認する。
- **JSON を RestClient 内で生成 → 署名 → 送信** の一貫性があるか（手動整形しない）。
- decimal の桁がずれていないか（`0.01m` → `"0.01"` となるようシリアライザ任せにする）。
- プロパティ順序を手動で変えていないか（署名は文字列そのものに依存）。
- timestamp がずれていないか（±数秒超のズレは弾かれる）。
- body に余計な空白・改行を入れていないか。

---

## 5. 運用上の留意点

### 5.1 API Key の取り扱い
- API Key / Secret は Git 管理しない
- ログに平文で出さない（マスク推奨）
- 資格情報の取得は `IApiCredentialProvider` に集約し、RestClient や PrivateApi は鍵を知らない設計を維持

### 5.2 レートリミット
- bitFlyer Private API の呼び出しは 1 秒間に **約 2 回** までが安全
- Stage3 ではレート制御を実装しない（Stage4 以降で検討）

### 5.3 ログ
- RestClient の層で API パス・ステータス・レスポンス本文（非機密）をログするのが望ましい
- Adapter や PrivateApi ではログを持たない

### 5.4 誤発注を避けるための運用
- 本番口座とテスト口座を `exchangeId` / `accountId` で切り替えられるよう運用設計
- CredentialProvider の動作ログを DEBUG で確認できると便利
- 事前に GetBalancesAsync で残高を確認してから注文する習慣を持つと安全
- 初回検証は必ず少額で行い、注文後に bitFlyer 管理画面で結果を確認する

---

## 6. Stage3 完了チェックリスト（運用観点）
- [ ] API Key / Secret を環境変数または CredentialProvider で設定できた
- [ ] `BitflyerClientFactory.Create` が Trading 機能対応の `IExchangeClient` を返した
- [ ] `SendOrderAsync` で `child_order_acceptance_id` が返ってきた
- [ ] bitFlyer 管理画面で注文が確認できた
- [ ] 403 / 400 / 500 等エラーケースが `ExchangeApiException` として適切に返された
- [ ] RestClient の POST 実装が署名・JSON ともに正常動作した
- [ ] Stage3 文書（A010〜A070）との整合性が取れている

---

## 7. Stage4 以降への接続
Stage3 の運用テンプレートは、以下の機能拡張時にそのまま適用できる：
- LIMIT / STOP / IFDOCO などの高機能注文
- cancelchildorder / cancelallchildorders の実装
- getchildorders / getexecutions / getpositions など Private GET 拡張
- E2 例外処理（bitFlyer 固有コード分類）

Stage3 は「MARKET 注文を安全に、安定して発行できる運用テンプレート」を確立するステージであり、
これ以降の Trading API 実装の基礎となる。
