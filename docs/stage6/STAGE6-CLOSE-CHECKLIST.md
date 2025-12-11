# Stage6 クローズ手順書（REST-only 信頼性・運用強化）

Stage6 を締める際に確認するチェックリストです。完了後はタグ付けとリリースノート更新を行います。

## コード/ドキュメント
- [ ] Transport 改修（ポリシーObserver、エラーペイロードパーサ、HttpTransport ハンドラ注入）が main ブランチに反映されている。
- [ ] ExchangeInfo 拡張（手数料通貨/種別、メンテ、JSON ローダ）と bitFlyer 実装が揃っている。
- [ ] Contracts/Transport/Factory/Adapter の README が更新済み（docs/Contracts, docs/Transport, docs/Factory, docs/Adapter/Bitflyer）。
- [ ] Stage6 Summary/Overview に最新の変更点が反映されている（docs/stage6/STAGE6-SUMMARY.md, STAGES-OVERVIEW.md）。

## テスト/品質
- [ ] `dotnet test` 全プロジェクト緑（Contracts/Transport/Factory/Adapter 含む）。
- [ ] 劣化環境 E2E (Bitflyer Tests) が通過することを確認。
- [ ] 主要ポリシー（Retry/RateLimit/CircuitBreaker/Timeout）のユニットテストが緑。

## リリース作業
- [ ] リリースノートに Stage6 のブレイキング変更（FeeCurrency/FeeType 追加、TryGetFeeRates シグネチャ変更、SymbolMeta 廃止）と新機能を記載。
- [ ] バージョン/タグを付与（例: `v6.x.x` または Stage6 用タグ）。
- [ ] main へマージし、CI/CD が成功することを確認。
- [ ] 今後の持ち越し（JSON ExchangeInfo 切替フラグ、手数料実データ埋め、メンテ外部連携等）を Stage7 以降の TODO に移管。

## オプション
- [ ] JSON ExchangeInfo の導入手順/サンプルを docs に追加するか検討。
- [ ] 実測データでポリシー既定値を見直す計測計画を Stage7 TODO に記載。
