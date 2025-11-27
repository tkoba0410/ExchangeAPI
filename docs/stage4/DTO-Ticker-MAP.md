# DTO-Ticker-MAP Ticker マッピング仕様（複数取引所想定）

## 1. 抽象 DTO 要約
- DTO 名: `Ticker`
- フィールド:
  - `Symbol` : 抽象シンボル（例: `BTC/JPY`）。取引所 product_code から変換する。
  - `BestBid`, `BestAsk`, `LastTradedPrice` : 価格。quote 通貨建て。
  - `Timestamp` : 取引所タイムスタンプを UTC に正規化。
  - （必要に応じて Volume 系を拡張する場合は、quote/ base のどちらかを明記すること）

## 2. 正規化ルール
- Symbol: 取引所の product_code を抽象シンボルへ静的/動的にマップする。未対応の product_code は `SymbolNotSupportedException` とする。
- 価格: 受信値をそのまま decimal で保持（スケールは取引所返却値に依存）。後段で丸めが必要なら呼び出し側で行う。
- Timestamp: 取引所が返す時刻を `DateTimeOffset` で受け取り、UTC へ変換して格納する。
- 通貨単位: 価格は quote 通貨、Symbol によって暗黙的に決まる（例: BTC/JPY の場合 JPY）。
- LTP (LastTradedPrice): 「直近約定価格」であり、その約定時刻を表すものではない。約定時刻が必要な場合は executions（約定一覧）を参照する。
- Best Bid/Ask: 価格のみを提供する。サイズが必要な場合は Board から取得する。

## 3. 取引所別マッピング
| 取引所 | ソースフィールド | 備考 | 実装状況 |
| --- | --- | --- | --- |
| bitFlyer | `/v1/getticker` → `best_bid`, `best_ask`, `ltp`, `timestamp` | `timestamp` は ISO8601 (UTC)。`product_code` を `Symbol` に変換。 | 実装済 |
| Binance（参考） | `/api/v3/ticker/bookTicker` → `bidPrice`, `askPrice`; `/api/v3/ticker/price` → `price`; `eventTime` は WebSocket系。REST/WS で取得元を明示すること。 | RESTはUTC。シンボルは大文字連結（例: BTCJPY / BTCUSDT）。`last` 相当は `/ticker/price` の `price` を使用。 | 未実装 |
| BitBank（参考） | `/v1/ticker` → `buy`, `sell`, `last`, `timestamp` | `timestamp` はミリ秒 epoch（JSTではなくUTC）。`pair` を Symbol に変換。 | 未実装 |
| BitTrade（参考） | `/public/ticker` → `best_bid`, `best_ask`, `last`, `timestamp` ※要実仕様確認 | フィールド名は典型例。実際のAPIレスポンスとタイムゾーン仕様を要確認のうえ反映すること。 | 未実装 |
| BTCBOX（参考） | `/api/v1/ticker/` → `buy`, `sell`, `last`, `time` | `time` は秒 epoch（UTC想定、要確認）。 | 未実装 |
| CoinCheck（参考） | `/api/ticker` → `bid`, `ask`, `last`, `timestamp` | `timestamp` は秒 epoch（UTC）。ペアはクエリ or デフォルト BTC/JPY、Symbol 変換が必要。 | 未実装 |
| GMOコイン（参考） | `/public/v1/ticker` → `bid`, `ask`, `last`, `timestamp` | `timestamp` は ISO8601 (UTC)。`symbol` を Symbol に変換（例: BTC_JPY）。 | 未実装 |
| （TBD） | - | 追加取引所の実装時に追記。 | - |
