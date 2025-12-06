# A010-STG4-OVER Stage4 ゴール定義（REST+WS 抽象 API 確定）

## 1. Stage4 の目的
Stage4 を「ExchangeAPI の抽象 API（REST+WS）を正式に確定し凍結するステージ」として再定義する。Market / Trading / Account / Margin / Realtime / ExchangeInfo の 6 区分で責務を分割し、以降の実装ステージ（Stage5 以降）が迷いなく進められるように**薄い抽象インターフェースと最小ドメインモデルを揃える**。

## 2. スコープ（Stage4 でやること / やらないこと）

### 2.1 Stage4 でやること
- 6 区分（Market/Trading/Account/Margin/Realtime/ExchangeInfo）で抽象インターフェースを定義・整理  
  - REST: `IMarketDataApi`（Ticker/Board/Executions/Candlesticks）、`ITradingApi`（Send/Cancel/OpenOrders）、`IAccountApi`（Balances）、`IMarginAccountApi`（Positions/Collateral）
  - WS: `IRealtimeMarketDataApi`（Subscribe Ticker/Board/Executions）
  - ExchangeInfo: 今後の拡張用の入口のみ用意
- ドメイン型を最小セットに揃える（Ticker/OrderBook/Execution/OrderRequest/OrderResult/Position/Collateral など）。Margin は建玉・証拠金に限定する。
- 抽象化できない領域を Raw API として切り出す方針を明記する。
- ドキュメント（A010〜A070）を新方針に合わせて刷新し、実装や横展開は Stage5 以降に送ることを明示する。

### 2.2 Stage4 でやらないこと
- bitFlyer など特定取引所への実装・テスト（REST/WS 両方とも Stage5 以降で実装）
- 注文拡張やエラー分類の詳細実装（抽象の整合性確認まで）
- WebSocket の再接続やストリーム制御などの運用ロジック
- 信頼性パターン/レート制御の実装、DX 仕上げ
- 複数取引所対応の実装

## 3. 完了条件（Definition of Done）
1. 6 区分すべてに抽象インターフェースが定義され、REST/WS の責務が分離されている
2. ドメイン型が抽象インターフェースに対応し、Margin は建玉・証拠金に限定されている
3. ExchangeInfo の骨子（スケルトン）があり、Raw API に逃がす領域が明記されている
4. Stage3 までの機能との互換を壊さず、Stage4 追加要素がビルド時に参照できる
5. ドキュメント: Stage4 A010〜A070 が「REST+WS 抽象 API の確定」を中心に更新されている

## 4. Stage5 以降への接続
- Stage5: Stage4 で凍結した抽象を bitFlyer で実装（REST+WS の縦深実装を開始）
- Stage6: 信頼性・運用・DX を強化し、リアルタイム運用パターンを仕上げる
- Stage7 以降: 複数取引所対応・ドキュメント整備を進め、公開水準に仕上げる
