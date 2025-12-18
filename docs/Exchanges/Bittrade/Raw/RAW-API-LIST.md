# Bittrade RAW API List

Bittradeアダプターで使用している主要なRESTエンドポイント（2025-01 時点）。

## Public API
- `GET /market/detail/merged?symbol={symbol}`
  - 用途: ティッカー取得
  - 対応コード: `BittradeMarketDataApi.GetTickerAsync`
- `GET /market/depth?symbol={symbol}&type=step0`
  - 用途: 板情報取得
  - 対応コード: `BittradeMarketDataApi.GetOrderBookAsync`
- `GET /market/trade?symbol={symbol}`
  - 用途: 歩み値（マーケット約定）取得
  - 対応コード: `BittradeMarketDataApi.GetMarketExecutionsAsync`
- キャンドル（ローソク足）
  - 現状 REST 未サポート。`GetCandlesticksAsync` は NotSupported を返す。

## Private API (Account / Trading)
- `GET /v1/account/accounts/{account-id}/balance`
  - 用途: 残高取得
  - 対応コード: `BittradeTradingApi.GetBalancesAsync` → `BittradeMapper.MapBalances`
- `GET /v1/order/openOrders?symbol={symbol}&account-id={account-id}`
  - 用途: オープン注文一覧
  - 対応コード: `BittradeTradingApi.GetOrdersAsync` → `BittradeMapper.MapOrderSummary`
- `GET /v1/order/orders/{orderId}`
  - 用途: 注文詳細（ステータス照会）
  - 対応コード: `BittradeTradingApi.PollOrderStatusAsync` (単回照会)
- `POST /v1/order/orders/place`
  - 用途: 新規注文（market/limit）
  - 対応コード: `BittradeTradingApi.PlaceOrderInternal`
- `POST /v1/order/orders/{orderId}/submitcancel`
  - 用途: 注文キャンセル
  - 対応コード: `BittradeTradingApi.CancelOrderAsync`

## Private API (未実装/NotSupported)
- 口座約定履歴（execution history）: REST 経由では未提供のため `GetAccountExecutionsAsync` は NotSupported。
- Stop注文: このアダプターでは stop 系を未サポート。

## Symbol マッピング
- API側 `symbol` は `btcjpy / ethjpy / fxbtcjpy` などの lower 文字列。
- コードでは `Symbol` 列挙にマッピングし、`ToApiSymbol` で REST パラメータへ変換。

## 備考
- エラーハンドリング: `status != "ok"` は `ExchangeApiException` を投げる。
- タイムスタンプ: Unix ms を `DateTimeOffset.FromUnixTimeMilliseconds` で正規化。

