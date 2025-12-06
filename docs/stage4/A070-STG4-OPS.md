# A070-STG4-OPS Stage4 運用メモ（抽象利用の指針）

Stage4 は抽象設計フェーズのため、ここでは「REST と WS の使い分け」や「抽象 API の利用方針」を整理する。実環境での手順や設定例は Stage5 以降で具体化する。

## 1. 前提
- 6 区分の抽象インターフェースが揃っていること
- Stage3 までのビルド/テストが緑であること（後方互換）

## 2. 利用パターン（設計時の確認）
1) スナップショット取得は REST（IMarketDataApi / ITradingApi / IAccountApi / IMarginAccountApi）を使う  
2) リアルタイム更新は WS（IRealtimeMarketDataApi）を購読し、必要に応じて REST と併用する（イベント DTO は TickerTick / OrderBookDelta / ExecutionTick を使用）
3) Margin 情報は IMarginAccountApi を通じて建玉・証拠金のみを取得し、詳細な履歴は Raw に任せる  
4) ExchangeInfo で対象市場/機能の有無を確認し、存在しない機能は Raw やフォールバックで扱う  
5) ClientOrderId を使わない場合は null を渡す/受け取るだけでよい。相関管理が必要な場合は別層の `IOrderIdMapper` 実装（例: InMemory）を差し込み、サーバーIDとの対応付けを行う。
6) Candlesticks は抽象で定義するが、取引所によっては未サポート（bitFlyer など）。`ExchangeInfo.Features.SupportsCandlestick` を確認し、未対応は例外（NotImplemented）を前提に運用する。
7) Realtime（WS）は bitFlyer では未実装のためスタブのみ。WS 利用は Stage5 以降の実装を前提にし、現状は REST 併用で運用する。

## 3. 運用メモ（ガイドライン）
- REST と WS を明確に役割分担する（REST=スナップショット/同期、WS=増分/リアルタイム）。
- Raw API を使う場合は抽象層と混在させず、境界をドキュメントに残す。
- 再接続/レート制御/エラーハンドリングは Stage5 以降の実装・運用で詰める前提とする。

## 4. ドキュメント/サンプルの扱い
- Stage4 ではサンプルコードを必要最小限の形で示すか、プレースホルダに留める。
- 実 API を用いた手順（鍵設定/発注/キャンセル/購読）は Stage5 以降で具体化する。
