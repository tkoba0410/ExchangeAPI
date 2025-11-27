# A090-STG4-SUMM Stage4 まとめ（Private 拡張 + 注文強化）

## 1. Stage4 の狙い
- Private GET/POST を横展開し、取引ライフサイクルを完結できる基盤を整える
- 注文種別を拡張し、実運用で求められる基本的な注文オプションを揃える
- bitFlyer 固有コードを扱う E2 エラー分類を導入し、運用診断性を高める

## 2. 主要成果物
- Abstractions: Position/Execution/Collateral 追加、OrderRequest 拡張、キャンセル API 追加
- Infrastructure: RestClient/Signer 拡張、ExchangeApiException の E2 対応、レートリミットフック
- Bitflyer: GET/POST DTO 拡張、sendchildorder の LIMIT/STOP 対応、キャンセル API 実装
- Adapter: DTO ⇄ Domain マッピング、E2 エラー分類の適用
- Factory: 新 API/設定の統合
- Docs: Stage4 A0xx、Quick Start 更新、README 再編のドラフト

## 3. 完了の確認ポイント
- LIMIT/STOP + time_in_force/minute_to_expire の注文が通る
- cancelchildorder/cancelallchildorders が抽象経由で動作する
- positions/executions/collateral が取得できる
- 代表的な bitFlyer エラーコードが E2 として分類される
- 代表テストが緑、手動チェックリストが完了

## 4. Stage5 以降への接続
- Stage5: 複数取引所対応を通じて抽象化とエラー分類の汎用性を検証する
- Stage6: WebSocket/リアルタイム拡張に着手する（再接続/ストリーム制御/バックプレッシャー）
- Stage7+: 信頼性・運用・DX を仕上げ、API リファレンス/サンプル群を充実させる
