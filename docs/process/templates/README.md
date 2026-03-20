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
- `EXCHANGEAPI_BITFLYER_LIVE_ALLOW_POST=1` を使う場合は、専用口座・最小数量・即時約定しにくい指値を前提とする。
