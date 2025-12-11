# A010 STAGE7 OVERVIEW

Stage7 の目標とスコープのドラフトです。Stage6 までの REST-only bitFlyer 実装をベースに、複数取引所対応の実証と DX 仕上げを行います。

## ゴール（案）
- 追加取引所で抽象 API 縦スライスを実装し、Transport/Contracts/Factory/Adapter の汎用性を検証する。
- ExchangeInfo の外部化（JSON ローダ切替）と設定導線を整備し、手数料/メンテ等の静的メタの運用を明確化する。
- ポリシー/観測性の既定値を実測で微調整し、運用ガイドに反映する。
- ドキュメント（Factory/Transport/Contracts/Adapter/Stage7 まとめ）とサンプルコードを拡充し、オンボーディングを改善する。

## スコープ候補
- 新規取引所アダプター（最小縦スライス）: Ticker/Order/Cancel/Balance の通過。エラー分類・署名・刻み/手数料メタの確認。
- ExchangeInfo: JSON 読み込みの設定フラグ/DI サンプル追加、外部化運用の手順化。
- ポリシー調整: Timeout/Retry/RateLimit/CircuitBreaker の実測値に基づくデフォルト見直し、Observer のメトリクス露出サンプル。
- DX: クックブック/サンプル（注文送信、ポリシー変更、JSON ExchangeInfo 切替）、リファレンスの更新。

## 非スコープ（例）
- WebSocket/Realtime の復活（Stage7 では REST-only を維持）。
- 高頻度最適化や専用ハンドラーのチューニング（必要なら Stage8 以降）。

## 成果物
- 新規取引所アダプター最小実装 + テスト
- ExchangeInfo JSON 切替/フォールバックの実装とドキュメント
- 調整後のポリシー既定値・観測性サンプル
- Stage7 サマリー/クローズチェックリスト
