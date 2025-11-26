# A080-STG2-APIKey 追補（資格情報管理の追加方針）

## 目的
Stage2 に以下を追記する差分ドキュメント。既存文書は変更せず、本書を併読する。
1. API キー/シークレットの取得・選択を組み立て側の責務とする。
2. 多取引所・多アカウントを見据えて、プロバイダ経由で資格情報を受け渡しできるようにする。
3. 旧プロジェクトのキー保管（Auth/Config/Crypt）は廃止し、新方式に統一する。

## ゴール
- `IApiCredentialProvider`（新設）を介して `(ApiKey, ApiSecret)` を取得し、`BitflyerClientFactory.Create(apiKey, apiSecret)` に渡すだけで組み立てられること。
- RestClient/Signer は鍵の取得を行わず、「渡された鍵を使うだけ」を維持。
- 環境変数・資格情報マネージャーなど、プロバイダ差し替えで運用を選べること。
- 平文はディスクに残さず、UI表示しないことを基本とする（誤操作防止のみの簡易オブフスケーションは可）。

## 推奨アーキテクチャ（追加）
- インターフェース: `IApiCredentialProvider` を Abstractions あるいは Orchestration 層で定義  
  - `ApiCredentials Get(string exchangeId, string accountId);`  
  - `ApiCredentials` は `ApiKey` / `ApiSecret` を保持する単純な DTO。
- 利用箇所: クライアント組み立て（DI/Factory）で `provider.Get(exchangeId, accountId)` を呼び、得たキーを `BitflyerClientFactory.Create(apiKey, apiSecret)` に渡す。  
  - 例: `var creds = provider.Get("bitflyer", "default");`
- 鍵保持の最小化: RestClient/Signer では鍵を保持しない。Signer が鍵を受け取るなら、署名処理内の寿命を最小化する（byte 配列のクリアなど）。
- デフォルト実装（Windows 想定）: 資格情報マネージャーの汎用資格情報を `exchangeId/accountId/api_key|api_secret` 形式で登録し、プロバイダで `CredRead` から取得する。標準 UI では平文表示できないが、同一ユーザーなら API で取得可能な点は留意。
- オブフスケーション（誤操作防止のみ、セキュリティ目的ではない）: 保存時に Base64 等で潰し、取得時に戻す。ログ/表示/クリップボードには出さない。

## 多取引所・多アカウント対応
- 命名規則（例）: `<EXCHANGE>_<ACCOUNT>_API_KEY` / `<EXCHANGE>_<ACCOUNT>_API_SECRET`  
  - 例: `BITFLYER_DEFAULT_API_KEY`, `BITFLYER_TRADING_API_SECRET`, `BINANCE_MAIN_API_KEY`
- プロバイダ実装例: 環境変数版 / シークレットストア版 / user secrets 版など。  
目的に応じて差し替え可能とする。
- フォールバック用に `CompositeCredentialProvider`（複数プロバイダを順に試す）を用意しておくと移行が容易。

## 旧方式の扱い
- `sample/config` の Auth/Config/Crypt 方式は廃止（新しいプロバイダ方式に統一）。必要なら別途アダプタを作るが、既定運用には含めない。

## 運用上の注意（NFR 追加事項）
- API キー/シークレットは Git 管理下に置かず、環境変数・資格情報マネージャー・シークレットストア等で管理する。平文ファイルは置かない。
- ログ/表示/クリップボードに平文を出さない。平文はオンメモリ短期利用のみ。
- どうしても表示が必要な場合は、表示前に本人認証（例: Windows Hello）を挟み、一時表示→即クリア。
- 運用に応じてプロバイダを差し替えられるようにし、ローテーションや移行を支援する。
