# A090-STG4-SUMM Stage4 まとめ（REST+WS 抽象 API 確定）

## 1. Stage4 の狙い
- ExchangeAPI の抽象 API（REST+WS）を正式に確定し凍結する
- Market/Trading/Account/Margin/Realtime/ExchangeInfo の 6 区分で責務を分割し、薄い境界を作る
- Margin は建玉・証拠金サマリに限定し、抽象の肥大化を避ける
- 実装は Stage5 以降に送り、Stage4 は設計整合性に専念する

## 2. 主要成果物
- 抽象インターフェース: IMarketDataApi / ITradingApi / IAccountApi / IMarginAccountApi / IRealtimeMarketDataApi / IExchangeInfoApi
- ドメイン型: Ticker / OrderBook / Execution / OrderRequest / OrderResult / OpenOrder / Position / Collateral（最小セット）
- WS 抽象の追加と REST との責務分離
- Raw API に逃がす領域の明示（親注文・入出金・履歴など）
- ドキュメント: Stage4 A010〜A070 を「抽象確定」方針に刷新

## 3. 完了の確認ポイント
- 6 区分すべてで抽象 IF が揃い、REST と WS が混在していない
- Margin は最小能力に限定され、継承構造が整合する
- Stage3 までの機能との互換を維持したまま新抽象を参照できる
- 実装タスクが Stage5 以降に明確に切り出されている

## 4. Stage5 以降への接続
- Stage5: bitFlyer など実取引所で REST/WS を実装し、抽象の妥当性を実証する
- Stage6: 信頼性・運用・DX を強化し、リアルタイム運用パターンを仕上げる
- Stage7+: 複数取引所対応とドキュメント仕上げを行い、公開水準に高める
