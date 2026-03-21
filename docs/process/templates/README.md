# Credentials Templates (age運用)

本ディレクトリは、資格情報運用で必要となるファイルのサンプル/テンプレートを管理する。
実ファイル（機密を含む平文/秘密鍵）は配置しない。

## テンプレート一覧

- `credentials.template.json`
  - 暗号化前の論理フォーマット（作業用）
- `appsettings.local.template.json`
  - アプリに渡すパス設定テンプレート
- `age-paths.template.txt`
  - 環境変数でパスを渡す場合のテンプレート
- `bitflyer-live.env.template`
  - bitFlyer live test 用の opt-in 環境変数テンプレート

## 運用ルール

- `ApiKey` / `ApiSecret` は null/空文字不可。
- `ExpiresAt` はキー必須、値は null または `yyyy-MM-ddTHH:mm:ssZ`（UTC固定）。
- `Version` / `UpdatedAt` / `Comment` はキー必須、値は null 可（`UpdatedAt` に値を入れる場合も同じ UTC 形式）。
- 平文ファイルは作業後に削除する。
- `credentials.enc.json` と `age.key` は別ディレクトリで管理する。

## Live Test 用メモ

- live test は通常の `dotnet test` / CI 既定経路に混ぜない。
- live test の opt-in は `bitflyer-live.env.template` の環境変数を基準にする。
- `EXCHANGEAPI_BITFLYER_API_KEY` / `EXCHANGEAPI_BITFLYER_API_SECRET` はテンプレートに値を書かず、実値は別管理とする。
- 認証あり live test は、上記の direct env に加えて `CREDENTIAL_FILE_PATH` / `AGE_SECRET_KEY_PATH`（既存 age 運用）でも実行できる。
- `CREDENTIAL_FILE_PATH` / `AGE_SECRET_KEY_PATH` が未指定でも、`~/.config/exchangeapi/secrets/credentials.enc.json` と `~/.config/exchangeapi/keys/age.key` が存在すればそれを既定値として使う。
- `EXCHANGEAPI_BITFLYER_LIVE_ALLOW_POST=1` を使う場合は、専用口座・最小数量・即時約定しにくい指値を前提とする。
- live test 実行時は、サニタイズ済みの request / response / error ログを自動で `artifacts/live-logs/bitflyer/<run-id>/` へ保存する。
- 既定のログ保存先を変えたい場合は `EXCHANGEAPI_BITFLYER_LIVE_LOG_DIR` を指定する。相対パスは repository root 基準で解決する。
- 自動ログには `run.json` と `events.jsonl` が含まれる。`events.jsonl` は `test_scope` / `request` / `response` / `error` を 1 行 1 JSON で記録する。
- 自動ログでは auth 系フィールドは mask、order/account 系 identifier は pseudonymize、private balance/collateral 系の数値は mask する。
- live test は `Trait("Category", "Live")` に加えて、`Flow=PublicGet|PrivateGet|PrivatePost` と `Layer=Wire|Raw|Normalized` で分離実行できる。
- 実行例:
  - `dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Flow=PublicGet"`
  - `dotnet test tests/Exchanges/Bitflyer/LiveTests/Exchange.Bitflyer.LiveTests.csproj --filter "Category=Live&Layer=Normalized"`
- 実行証跡は `docs/process/reviews/templates/STAGE10-LIVE-EVIDENCE.md` を雛形にして `docs/process/reviews/` 配下へ保存する。
