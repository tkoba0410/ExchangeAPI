# A065-STG2-TSTP-Addendum （Factory プロバイダーと実通信テストの補足）

既存の A060 を変更せず、追加で把握しておきたいテスト観点をまとめる。

## 1. Factory プロバイダーオーバーロードのテスト観点
1. `Create(IApiCredentialProvider provider, exchangeId, accountId)` が provider から取得したキーを使って `IExchangeClient` を生成できること。
2. `provider == null` の場合に `ArgumentNullException` を投げること。

## 2. 実行とインテグレーションテストの扱い
- ユニットテスト（通信なし）は `dotnet test tests/ExchangeApi.Bitflyer.Tests/ExchangeApi.Bitflyer.Tests.csproj` などで実行する。
- 実通信テストは任意の別プロジェクト/カテゴリに分離し、検証用 API キーを環境変数やシークレットで注入して手動実行とする（デフォルトの `dotnet test` には含めない）。キー未設定時はスキップする条件分岐を推奨。
- ログや例外に秘密を出さないこと。レートリミットや署名エラーは `ExchangeApiException` で扱う。
