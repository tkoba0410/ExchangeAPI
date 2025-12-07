# Stage5 テストメモ

## 実行コマンド
- `dotnet test`

## カバー範囲（主要項目）
- Contracts/Transport/Factory/Bitflyer 各テストプロジェクト。
- Trading: sendchildorder のマッピング、エラーハンドリング（RateLimit/Balance/Auth）、ポーリング（完了検知）。
- Account/Margin: 残高・建玉・証拠金のドメインマッピング。
- Market: Ticker/Board/Executions の正規化。

## 備考
- 現状のターゲットは bitFlyer / REST のみ。Realtime は廃止済み。
