# Bittrade Adapter 概要（最小スコープ）

Bittrade（Huobi 系 REST）向けの最小実装メモ。現状は Public/Private REST の一部に対応し、WebSocket/Candle/履歴系は未対応。

## 現在の対応
- Public: Ticker (`market/detail/merged`), OrderBook (`market/depth` step0), MarketExecutions (`market/trade`), Candles 未対応（NotSupported）。
- Private: 残高 (`v1/account/accounts/{id}/balance`), 注文送信 (`v1/order/orders/place` LIMIT/MARKET), キャンセル (`submitcancel`), 未約定一覧 (`openOrders`), 注文詳細（単回ポーリング）。
- ExchangeInfo: `/v1/common/symbols` から刻み・最小数量・最小ノッチを取得し `ExchangeMarketInfo` にマッピング（手数料は未取得）。
- Factory: `BittradeClientFactory.CreatePublic()` / `CreatePrivate(accessKey, secretKey, accountId)` で構築。署名/HmacSHA256 + ポリシー/エラー分類は Factory でセット。
- 署名: Huobi形式（AccessKeyId/SignatureVersion=2/HmacSHA256/Timestamp を canonical string 署名、Signature をクエリ付与）。
- エラー分類: HTTP ステータス中心の簡易分類（Auth/RateLimit/Server/Request/Unknown）。

## 未対応/今後
- ExchangeInfo: 手数料通貨/種別・メンテ情報は API 非提供のため JSON 等で手動設定（例: `configs/exchangeinfo/bittrade.json` で手数料 0/null を指定）。メンテ情報は不明のため null。
- AccountExecutions/履歴系/ポジション: 未実装。API 仕様確認後に対応を検討（現状 NotSupported）。
- WebSocket: 非対応。必要なら別モジュールで検討。
- エラーコード詳細マッピング: 現状はHTTPステータス中心。公式エラーコード表に基づき `ExchangeErrorCategory` への正規化を拡張する。
- 設定参照: 人間可読の設定メモは `docs/configs/exchangeinfo/bittrade.md`、機械読込は `configs/exchangeinfo/bittrade.json`。

## 利用例（Public）
```csharp
var market = BittradeClientFactory.CreatePublic();
var ticker = await market.GetTickerAsync("BTC/JPY");
```

## 利用例（Private）
```csharp
var (market, trading, account) = BittradeClientFactory.CreatePrivate(accessKey, secretKey, accountId);
var balances = await account.GetBalancesAsync();
var order = await trading.SendOrderAsync(new OrderRequest("BTC_JPY", OrderSide.Buy, OrderType.Market, 0.01m));
var status = await trading.PollOrderStatusAsync("BTC_JPY", order.OrderAcceptanceId);
```
