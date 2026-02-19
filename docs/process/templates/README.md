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

## 運用ルール

- `ApiKey` / `ApiSecret` は null/空文字不可。
- `ExpiresAt` はキー必須、値は null または ISO-8601 文字列。
- `Version` / `UpdatedAt` / `Comment` はキー必須、値は null 可。
- 平文ファイルは作業後に削除する。
- `credentials.enc.json` と `age.key` は別ディレクトリで管理する。
