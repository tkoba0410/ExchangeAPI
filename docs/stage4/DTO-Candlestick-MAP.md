# DTO-Candlestick-MAP Candlestick マッピング仕様（複数取引所想定）

## 1. 抽象 DTO 要約
- DTO 名: `Candlestick`
- フィールド（推奨）:
  - `Timestamp` : 足の開始時刻（UTC, DateTimeOffset）
  - `Open`, `High`, `Low`, `Close` : 価格（quote 通貨建て, decimal）
  - `Volume` : 取引量（base 通貨単位, decimal）
  - `Timescale` : 足種別（例: 1min, 5min, 15min, 1hour など）※列挙 or 文字列

## 2. 正規化ルール
- 時刻: 取引所の epoch/ISO を UTC の `DateTimeOffset` に正規化し、「足の開始時刻」を入れる。
- 価格: 受信値をそのまま decimal で保持。必要に応じて丸めは利用側で行う。
- ボリューム: base 通貨単位。取引所が quote ボリュームを返す場合は注意。
- Timescale: 取引所の粒度表記（例: 1min, 5min, 1h）を抽象の列挙/文字列にマップする。未対応の粒度は例外 or スキップ。

## 3. 取引所別マッピング
| 取引所 | エンドポイント/フィールド | 備考 | 実装状況 |
| --- | --- | --- | --- |
| Binance（参考） | `/api/v3/klines` → [open time, open, high, low, close, volume, ...] | `open time` は ms epoch (UTC)。シンボルは大文字連結（例: BTCUSDT）。 | 未実装 |
| BitBank（参考） | `/v1/candlestick/{pair}/{timescale}` → `ohlcv` 配列（[open, high, low, close, volume, timestamp]） | `timestamp` は ms epoch (UTC)。`pair` を Symbol に変換。 | 未実装 |
| BitTrade（参考） | （未調査） | 実仕様要確認。 | 未実装 |
| BTCBOX（参考） | （未調査） | 実仕様要確認。 | 未実装 |
| CoinCheck（参考） | `/api/candles/{pair}/{period}` → `opened_at`, `high`, `low`, `close`, `volume` | `opened_at` は epoch (UTC)。`period` は 1min/5min 等。 | 未実装 |
| GMOコイン（参考） | `/public/v1/klines` → `openTime`, `open`, `high`, `low`, `close`, `volume` | `openTime` は ISO8601 (UTC)。`symbol` を Symbol に変換。 | 未実装 |
| bitFlyer（参考） | Publicに公式OHLCVなし（Board/Tickerのみ）。外部の約定から集計が必要。 | 現状は未対応（自前集計）。 | 未実装 |
| （TBD） | - | 追加取引所の実装時に追記。 | - |
| （TBD） | - | 追加取引所の実装時に追記。 | - |

## 4. 注意点
- 足の開始/終了どちらを返すかは取引所差がある。抽象では「開始時刻」を採用し、必要なら終了時刻は利用側で計算する。
- Volume が base か quote か取引所で異なることがあるため、実装時に必ず確認する。
- タイムスケールの命名が取引所ごとに異なる（例: 1m, 1min, 60, 60s）。列挙へのマッピング表を別途管理すること。***
