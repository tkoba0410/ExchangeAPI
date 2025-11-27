# A070-STG2-OPS Stage2 動作確認・運用メモ（get balance）

> 状態: Stage2 FIX 版（変更凍結）。以降の変更は Stage3 以降で扱うこと。

## 1. 本文書の目的
Stage2 の目標は、bitFlyer Private API `/v1/me/getbalance` を抽象層 `IExchangeClient` 経由で取得できる状態にすることである。

本書では、実装後に行う **動作確認手順・注意事項・運用上の留意点** をまとめる。
Stage3 以降（collateral / positions / executions / orders）に共通で使える確認フローのテンプレートにもなる。

---

## 2. 動作確認の前提
- bitFlyer Lightning API の **API Key / Secret** を取得済みであること。
  - "読み取り専用（Read）" 権限だけでも `/v1/me/getbalance` は利用可能。
- HTTP 通信が許可された環境（プロキシ等の制限がない）で実行すること。
- リポジトリに Stage2 実装が反映されており、Factory による組み立てが完了していること。

---

## 3. 動作確認手順（最短で必要なもの）

### Step 1. API キーとシークレットを準備
運用では **環境変数・資格情報マネージャー等のプロバイダ** に保存し、ソースコードや設定ファイルに直書きしない。

- 推奨命名規則：`<EXCHANGE>_<ACCOUNT>_API_KEY` / `<EXCHANGE>_<ACCOUNT>_API_SECRET`
  - 例：`BITFLYER_DEFAULT_API_KEY`, `BITFLYER_DEFAULT_API_SECRET`
- 複数口座を切り替える場合は `ACCOUNT` 部分を `DEFAULT`, `TRADING`, `BACKTEST` などに分ける。

#### Windows（PowerShell）で環境変数を登録
```powershell
setx BITFLYER_DEFAULT_API_KEY "your_api_key"
setx BITFLYER_DEFAULT_API_SECRET "your_api_secret"
```

#### Linux / macOS（bash）
```bash
export BITFLYER_DEFAULT_API_KEY="your_api_key"
export BITFLYER_DEFAULT_API_SECRET="your_api_secret"
```

#### Windows 資格情報マネージャーを利用する場合
- 汎用資格情報として `bitflyer/default/api_key` / `bitflyer/default/api_secret` などの名称で登録し、`WindowsCredentialManagerApiCredentialProvider` から読み出す。
- `cmdkey /generic:bitflyer/default/api_key /user:unused /pass:your_api_key` などで登録できる。

> セキュリティ上、APIキーをソースコードに直書きしない。

---

### Step 2. Factory を使って `IExchangeClient` を生成
`IApiCredentialProvider` を使うと複数環境での運用切り替えが容易になる。

```csharp
var provider = new CompositeCredentialProvider(new IApiCredentialProvider[]
{
    new EnvironmentVariableApiCredentialProvider(),
    new WindowsCredentialManagerApiCredentialProvider(),
});

// exchangeId/accountId でキーを選択
var client = BitflyerClientFactory.Create(provider, "bitflyer", "default");
```

環境変数を直接読む簡易手順が必要な場合のみ、従来どおり文字列を取得して `Create(apiKey, apiSecret)` に渡す。

```csharp
var apiKey = Environment.GetEnvironmentVariable("BITFLYER_DEFAULT_API_KEY")!;
var apiSecret = Environment.GetEnvironmentVariable("BITFLYER_DEFAULT_API_SECRET")!;

var client = BitflyerClientFactory.Create(apiKey, apiSecret);
```

### Step 3. `GetBalancesAsync` を実行
```csharp
var balances = await client.GetBalancesAsync();

foreach (var b in balances)
{
    Console.WriteLine($"Currency: {b.Currency}, Amount: {b.Amount}, Available: {b.Available}");
}
```

### Step 4. 正常なレスポンスを確認
**期待結果の例（実際の口座残高に依存）：**
```
Currency: JPY, Amount: xxxx, Available: xxxx
Currency: BTC, Amount: 0.xxxx, Available: 0.xxxx
```

この表示が得られれば Stage2 の実装は end-to-end で正常。

---

## 4. 想定されるエラーと対処

### 4.1 API キー不正（StatusCode: 403）
**現象：**
- `ExchangeApiException: StatusCode = Forbidden (403)` が発生。

**原因：**
- API Key / Secret が誤っている
- bitFlyer 側でキーが無効化されている

**対処：**
- API キー／秘密鍵を再設定
- bitFlyer 管理画面で有効なキーであることを確認

---

### 4.2 タイムスタンプずれ（StatusCode: 400 or 403）
**現象：**
- 不正署名扱い

**原因：**
- PC の時計が数秒以上ずれている場合に発生

**対処：**
- OS 時刻を NTP 同期する（Windows/Mac/Linux 共通で重要）

---

### 4.3 ネットワークタイムアウト
**現象：**
- `ExchangeApiException` にラップされて返される

**対処：**
- ネットワーク接続状態を確認
- Firewall / Proxy 等の影響を確認

---

## 5. 運用上の留意点（Stage2 時点）

### 5.1 API Key の取り扱い
- API Key / Secret は Git 管理しない。必要な場合でも `.user` ファイルやシークレットストアに分離する。
- ログ / 例外 / UI / クリップボードに平文の鍵を出力しない。オンメモリで最小限だけ扱う。
- `IApiCredentialProvider` の実装（環境変数 / Windows 資格情報マネージャー / CI シークレットなど）を呼び出し側で明示的に選択し、ライブラリにデフォルト実装を埋め込まない。
- 環境変数を使う場合は `<EXCHANGE>_<ACCOUNT>_API_KEY` など一貫した命名を守り、`exchangeId` / `accountId` で参照できるようにする。
- 複数プロバイダを組み合わせる場合は `CompositeCredentialProvider` を使い、順序付きフォールバックで移行やローテーションを容易にする。
- Windows 資格情報マネージャーを利用する環境では、登録済みの資格情報名（`bitflyer/default/api_key` 等）と `accountId` の対応表を運用ドキュメントに残す。
### 5.2 呼び出し頻度の上限
`/v1/me/getbalance` は軽量 API だが、bitFlyer の Private API にはレート制限がある：
- Private API: 1 秒間に約 2 回程度が上限

**対応策（Stage3 以降に導入予定）：**
- レートリミット検知
- 自動リトライ
- 呼び出し間隔の調整

### 5.3 例外処理（E1 レベル）
- Stage2 の例外はすべて `ExchangeApiException` に統一
- エラー種類の詳細分類やドメイン例外化は Stage3 以降の検討事項

---

## 6. ログ確認ポイント
以下は実運用でデバッグに役立つ情報：
- 呼び出し URL（`/v1/me/getbalance`）
- HTTP ステータスコード
- 時刻（署名の timestamp と比較できる）
- レスポンス本文（機密情報を除く）

ログを仕込む場合は、Adapter 層ではなく RestClient 層に集約するのが望ましい。

---

## 7. Stage2 完了チェックリスト（運用観点）
- [ ] API キー／シークレットを環境変数で設定できた
- [ ] `BitflyerClientFactory.Create` がエラーなく動作した
- [ ] `GetBalancesAsync` が正常に値を返した
- [ ] 残高表示が bitFlyer 管理画面と整合している
- [ ] エラー系（403 / タイムアウト）を意図的に再現できた
- [ ] ログ（必要に応じて）が適切に出力されている
- [ ] OVER（A010）、要件（A020）、構成（A030）、マッピング（A040）、実装メモ（A050）、テスト（A060）と整合性が取れている

---

## 8. Stage3 以降への接続
Stage2 の OPS 文書は、以下の発展にもそのまま利用できる：
- `/v1/me/getcollateral` の動作確認
- `/v1/me/getpositions` の動作確認
- `/v1/me/getexecutions` の動作確認
- Private POST（注文 / キャンセル）の動作確認

Stage2 は「最初の Private GET の動作確認テンプレート確立」が主目的であり、
本書は今後の API 導入時のベースラインとして再利用する。
