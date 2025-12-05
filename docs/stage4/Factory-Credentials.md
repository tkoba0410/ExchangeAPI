# Factory Credentials/Transport ガイド

## 認証情報プロバイダ
- EnvironmentVariableApiCredentialProvider  
  - 形式: `{EXCHANGE}_{ACCOUNT}_API_KEY`, `{EXCHANGE}_{ACCOUNT}_API_SECRET`（大文字、ハイフン/空白はアンダースコアに変換）
- WindowsCredentialManagerApiCredentialProvider  
  - ターゲット名: `exchange/account/api_key` と `exchange/account/api_secret`（小文字）。Windows専用。
- FileApiCredentialProvider（クロスプラットフォーム）  
  - JSON 例:  
    ```json
    {
      "bitFlyer/default": { "ApiKey": "...", "ApiSecret": "..." },
      "another/account": { "ApiKey": "...", "ApiSecret": "..." }
    }
    ```  
  - セキュリティはファイル権限に依存するので、配置先のアクセス制御に注意。
- CompositeCredentialProvider  
  - 複数プロバイダをフォールバック順に束ねる。全て失敗した場合、プロバイダ名と理由をまとめた例外を返す。

## RestClient/Transport 組み立て
- RestClientFactory（Factoryプロジェクト内）で以下を注入可能：
  - `IRequestSigner`（署名）、`IHttpPolicy`（リトライ/タイムアウト等）、`IRestClientLogger`（ログ）
  - `IHttpTransport` または `HttpClient` を差し替え可。
- アダプタ側（例: bitFlyer）は Factory で組んだ RestClient を再利用し、署名/ポリシー/ログの統一を図る。
