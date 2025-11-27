# A060-STG4-TSTP Stage4 テスト観点（Private 横展開 + 注文強化）

## 1. 単体テスト
- Domain バリデーション: OrderRequest の price/trigger_price/time_in_force/minute_to_expire
- DTO ↔ Domain マッピング: positions/executions/collateral、OrderRequest → sendchildorder DTO、cancel DTO
- エラー分類: bitFlyer code + HTTP ステータス → E2 例外のマッピング
- RestClient/Signer: cancelchildorder/cancelallchildorders の署名・URI 構築
- 429/RateLimit: Retry-After/ヘッダ有無による挙動が設定で制御されるかの確認

## 2. 結合テスト（モック/スタブ）
- Adapter 経由での SendOrderAsync(LIMIT/STOP) → DTO 生成確認
- Cancel API 呼び出しで正しいパラメータが渡ること
- GET 系で DTO スタブが Domain に正しく変換されること
- E2 エラーが抽象層に伝搬すること（再試行可否のフラグ確認）

## 3. 手動/統合チェック（必要に応じて）
- 小額の LIMIT/STOP 注文送信とキャンセルが通ること
- positions/executions/collateral が実口座で取得できること
- 429/400/403 系の代表エラーが期待どおり例外化されること
- OrderRequest バリデーション: 組み合わせ不備（STOP で price/trigger の欠落など）が適切に弾かれること

## 4. 非対象（Stage5+ へ委ねる）
- 複数取引所間の比較テスト
- WebSocket ストリームの録画/リプレイ
- 本格的な負荷試験・レート制御の検証
